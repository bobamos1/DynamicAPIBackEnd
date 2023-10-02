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
            controllers["Clients"].addRouteAPI("Connection",
                async (queries, bodyData) =>
                {
                    string username = "bob";
                    string email = "bob@gmail.com";
                    long userID = 1;
                    /*ressortir les données si dessus au lieu qu'ils soient hard codé
                     create token
                    hard code le role client pour le client
                    ou
                    Ressortir le rôle du client*/
                    string token = DynamicConnection.CreateToken(username, email, userID, (long)Roles.Client);
                    return Results.Ok(token);
                    //return Results.Ok(bodyData);
                    //return Results.Ok(await executorData.SelectQuery(queries[0]));
                }, false
            );
            controllers["Commandes"].addRouteAPI("GetClientCommandes",
                async (queries, bodyData) =>
                {
                    var dictionary = (Dictionary<string, object>)bodyData["conds"];
                    object da = dictionary["CurrentUserID"];
                    return Results.Ok(da);
                }, true, true
            );
            //Ajoute les routes de l'API ici
            //Get Liste de souhaits selon le id du client
            var panierGetAll = controllers["Panier"].GetGetAllRoute();
            controllers["Panier"].addRouteAPI("GetListeSouhait",
                async (queries, bodyData) =>
                {
                    return Results.Ok(
                        await executorData.SelectQuery(
                        panierGetAll
                            .clearParams()
                            .setParam("etat_commande", 4)
                            .setParam("id_client", bodyData["CurrentUserID"])
                        , (string[])bodyData["AuthorizedProprieties"]
                        )
                    );
                }, true, true
            );
            //Get Le panier selon le id du client
            controllers["Panier"].addRouteAPI("GetPanier",
                async (queries, bodyData) =>
                {

                    return Results.Ok(
                        await executorData.SelectQuery(
                        panierGetAll
                            .clearParams()
                            .setParam("etat_commande", 5)
                            .setParam("id_client", bodyData["CurrentUserID"])
                        , (string[])bodyData["AuthorizedProprieties"]
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