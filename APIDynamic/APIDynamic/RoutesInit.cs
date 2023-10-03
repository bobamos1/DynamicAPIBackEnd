using DynamicSQLFetcher;
using DynamicStructureObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParserLib;
using System.ComponentModel;

namespace APIDynamic
{
    public static class RoutesInit
    {
        public static void InitRoutes(Dictionary<string, DynamicController> controllers, WebApplication app, Dictionary<string, string> connectionStrings)
        {
            SQLExecutor executorData = new SQLExecutor(connectionStrings["data"]);
            DynamicController.initRoutesControllersInfo(app, controllers);
            DynamicController.MakeBaseRoutesDefinition(controllers, executorData);
            //Query qquery = controllers["Commande"].Routes.First().Queries.First().query;
            controllers["Clients"].addRouteAPI("ConnectionFirstFactor",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnection2Factor(bodyData.Get<string>("Email"), bodyData.Get<string>("Password"), queries[0], queries[1]);
                }
            );//, false
            controllers["Clients"].addRouteAPI("ConnectionTwoFactor",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnection(bodyData.Get<string>("TwoFactor"), queries[0], Roles.Client.ID());
                }
            );//, false
            controllers["Clients"].addRouteAPI("CreateUser",
                async (queries, bodyData) =>
                {
                    var userInfo = DynamicConnection.CreatePasswordHash(bodyData.Get<string>("Username"), bodyData.Get<string>("Email"), bodyData.Get<string>("Password"));
                    var userID = await executorData.ExecuteInsertWithLastID(
                        queries[0]
                            .setParam("Username", userInfo.username)
                            .setParam("Email", userInfo.Email)
                            .setParam("PasswordHash", userInfo.passwordHash)
                            .setParam("PasswordSalt", userInfo.passwordSalt)
                   );
                    return Results.Ok(DynamicConnection.CreateToken(userID, userInfo, Roles.Client.ID()));
                }
            );//, false
            controllers["Clients"].addRouteAPI("RecoverPasswordStepOne",
                (queries, bodyData) =>
                {
                    return DynamicConnection.sendRecoveryEmail(bodyData.Get<string>("Email"), queries[0]);
                }
            );//, false
            controllers["Clients"].addRouteAPI("RecoverPasswordStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.recoverPassword(bodyData.Get<string>("TwoFactor"), bodyData.Get<string>("Password"), queries[0]);
                }
            );//, false
            controllers["Employers"].addRouteAPI("ConnectionFirstFactor",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnection2Factor(bodyData.Get<string>("Email"), bodyData.Get<string>("Password"), queries[0], queries[1]);
                }
            );//, false
            controllers["Employers"].addRouteAPI("ConnectionTwoFactor",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnection(bodyData.Get<string>("TwoFactor"), queries[0], queries[1]);
                }
            );//, false
            controllers["Commandes"].addRouteAPI("GetClientCommandes",
                async (queries, bodyData) =>
                {
                    var dictionary = (Dictionary<string, object>)bodyData["conds"];
                    object da = dictionary["CurrentUserID"];
                    return Results.Ok(da);
                }
            );//, true, true
            //Ajoute les routes de l'API ici
            //Get Liste de souhaits selon le id du client
            var panierGetAll = controllers["Panier"].GetGetAllRoute();
            controllers["Panier"].addRouteAPI("GetListeSouhait",
                async (queries, bodyData) =>
                {

                    return Results.Ok(bodyData);
                }
            );//, true, true
            //Get Le panier selon le id du client
            controllers["Panier"].addRouteAPI("GetPanier",
                async (queries, bodyData) =>
                {

                    return Results.Ok(
                        await executorData.SelectQuery(
                        panierGetAll
                            .clearParams()
                            .setParam("etat_commande", 5)
                            .setParam("id_client", bodyData.UserID())
                        , bodyData.AuthProprieties()
                        )
                    );
                }, true, true
            );
            //Delete un produit du panier
            controllers["Panier"].addRouteAPI("DeleteProduitPanier",
                async (queries, bodyData) =>
                {
                    return Results.Ok("Le produit fut supprimé de votre panier.");
                }
                );
            //Delete un produit de la liste de souhait
            controllers["Panier"].addRouteAPI("DeleteProduitListeSouhait",
                async (queries, bodyData) =>
                {
                    return Results.Ok("Le produit fut supprimé de votre liste de souhait.");
                }
                );
            /*
            controllers["Commandes"].addRouteAPI("CommandeCheckout",
                async (queries, body) => {
                    try
                    {

                        //var data = body as Dictionary<string, object>;
                        return Results.Ok(await executorData.SelectQuery(queries[1]));

                    }
                    catch (Exception e)
                    {

                    }
                    return Results.NotFound();
                });
            */
        }
        public static Dictionary<string, string> LoadConnectionStrings(IConfiguration configuration)
        {
            SQLExecutor.Initialize(configuration);
            var connectionStringSection = configuration.GetSection("ConnectionStrings");
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            foreach (var child in connectionStringSection.GetChildren())
            {
                var key = child.Key;
                var value = child.Value;

                if (!string.IsNullOrEmpty(value))
                    connectionStrings[key] = value;
            }
            return connectionStrings;
        }
    }
}