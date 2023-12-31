﻿using DynamicSQLFetcher;
using DynamicStructureObjects;
using Stripe;
using System;

namespace APIDynamic
{
    public static class RoutesInit
    {
        public static void InitRoutes(Dictionary<string, DynamicController> controllers, WebApplication app, Dictionary<string, string> connectionStrings)
        {
            SQLExecutor executorData = new SQLExecutor(connectionStrings["data"]);
            DynamicController.initRoutesControllersInfo(app, controllers);
            DynamicController.MakeBaseRoutesDefinition(controllers, executorData);
            controllers["Clients"].addRouteAPI("ConnexionStepOne",
                (queries, bodyData) =>
                {
                    var password = bodyData.Get<string>("Password");
                    return DynamicConnection.makeConnectionStepOne(executorData, queries[0].setParam("Password", password), queries[1], bodyData.Get<string>("Email"), password);
                }
            );
            controllers["Clients"].addRouteAPI("ConnexionStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnectionStepTwo(executorData, queries[0], bodyData.Get<string>("Token"), false, Roles.Client.ID());
                }
            );
            controllers["Clients"].addRouteAPI("InscriptionClient",
                async (queries, bodyData) =>
                {
                    var nom = bodyData.Get<string>("Nom");
                    var email = bodyData.Get<string>("Email");
                    var userInfo = DynamicConnection.CreatePasswordHash(nom, email, bodyData.Get<string>("Password"));
                    var id = await executorData.ExecuteInsertWithLastID(queries[0]
                        .setParam("Nom", nom)
                        .setParam("Prenom", bodyData.Get<string>("Prenom"))
                        .setParam("DateNaissance", bodyData.Get<string>("DateNaissance"))
                        .setParam("AdresseCourriel", email)
                        .setParam("MDP", userInfo.passwordHash)
                        .setParam("Token", "")
                        .setParam("Sel", userInfo.passwordSalt)
                        .setParam("Actif", bodyData.Get<bool>("Actif"))
                        //.setParam("Password", bodyData.Get<bool>("Password"))
                    );
                    return Results.Ok(DynamicConnection.CreateToken(id, userInfo));
                }
            );

            /*
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
                }//, true, true
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