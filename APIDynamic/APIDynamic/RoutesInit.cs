using Dapper;
using DynamicSQLFetcher;
using DynamicStructureObjects;
using ParserLib;
using Stripe;
using System;
using System.Collections.Immutable;

namespace APIDynamic
{
    public static class RoutesInit
    {
        public static void InitRoutes(Dictionary<string, DynamicController> controllers, WebApplication app, Dictionary<string, string> connectionStrings)
        {
            SQLExecutor executorData = new SQLExecutor(connectionStrings["data"]);
            DynamicController.initRoutesControllersInfo(app, controllers);
            DynamicController.MakeBaseRoutesDefinition(controllers, executorData);
            controllers["Clients"].mapRoute("ConnexionStepOne",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnectionStepOne(executorData, queries[0], queries[1], bodyData.Get<string>("Email"), bodyData.Get<string>("Password"));
                }
            );
            controllers["Clients"].mapRoute("ConnexionStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnectionStepTwo(executorData, queries[0], bodyData.Get<string>("Token"), false, Roles.Client.ID());
                }
            );
            controllers["Clients"].mapRoute("InscriptionClient",
                async (queries, bodyData) =>
                {
                    var nom = bodyData.Get<string>("Nom");
                    var email = bodyData.Get<string>("Email");
                    var userInfo = DynamicConnection.CreatePasswordHash(nom, email, bodyData.Get<string>("Password"));
                    var id = await executorData.ExecuteInsertWithLastID(queries[0]
                        .setParams(bodyData)
                        .setParam("MDP", userInfo.passwordHash)
                        .setParam("Token", "")
                        .setParam("Sel", userInfo.passwordSalt)
                        //.setParam("Password", bodyData.Get<bool>("Password"))
                    );
                    return Results.Ok(DynamicConnection.CreateToken(id, userInfo));
                }
            );
            controllers["Clients"].mapRoute("RecuperationStepOne",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeRecuperationStepOne(executorData, queries[0], queries[1], bodyData.Get<string>("Email"));
                }
            );
            controllers["Clients"].mapRoute("CheckEmail",
                async (queries, bodyData) =>
                {
                    var result = await executorData.SelectValue<int>(queries[0].setParam("Email", bodyData["Email"]));
                    if (result == 0)
                        return Results.Ok();
                    return Results.Problem("", "", 410);
                }
            );
            controllers["Clients"].mapRoute("RecuperationStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeRecuperationStepTwo(executorData, queries[0], queries[1], bodyData.Get<string>("Token"), bodyData.Get<string>("NewPassword"), false, Roles.Client.ID());
                }
            );
            controllers["Clients"].mapRoute("ChangePassword",
                async (queries, bodyData) =>
                {
                    var userInfo = await DynamicConnection.checkUserInfo(bodyData.Get<string>("Email"), bodyData.Get<string>("Password"), queries[0], executorData);
                    if (userInfo is null)
                        return Results.Forbid();
                    if (await DynamicConnection.ChangePassword(executorData, queries[1], userInfo.userID, bodyData.Get<string>("NewPassword")))
                        return Results.Ok();
                    return Results.Forbid();
                }
            );


            //Connection Employes
            controllers["Employes"].mapRoute("ConnexionStepOne",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnectionStepOne(executorData, queries[0], queries[1], bodyData.Get<string>("Email"), bodyData.Get<string>("Password"));
                }
            );
            controllers["Employes"].mapRoute("ConnexionStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeConnectionStepTwo(executorData, queries[0], bodyData.Get<string>("Token"), true);
                }
            );
            controllers["Employes"].mapRoute("InscriptionEmploye",
                async (queries, bodyData) =>
                {
                    var nom = bodyData.Get<string>("Nom");
                    var email = bodyData.Get<string>("Email");
                    var userInfo = DynamicConnection.CreatePasswordHash(nom, email, bodyData.Get<string>("Password"));
                    var id = await executorData.ExecuteInsertWithLastID(queries[0]
                        .setParams(bodyData)
                        .setParam("MDP", userInfo.passwordHash)
                        .setParam("Token", "")
                        .setParam("Sel", userInfo.passwordSalt)
                    //.setParam("Password", bodyData.Get<bool>("Password"))
                    );
                    return Results.Ok(DynamicConnection.CreateToken(id, userInfo));
                }
            );
            controllers["Employes"].mapRoute("RecuperationStepOne",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeRecuperationStepOne(executorData, queries[0], queries[1], bodyData.Get<string>("Email"));
                }
            );
            controllers["Employes"].mapRoute("CheckEmail",
                async (queries, bodyData) =>
                {
                    var result = await executorData.SelectValue<int>(queries[0].setParam("Email", bodyData["Email"]));
                    if (result == 0)
                        return Results.Ok();
                    return Results.Problem("", "", 410);
                }
            );
            controllers["Employes"].mapRoute("RecuperationStepTwo",
                (queries, bodyData) =>
                {
                    return DynamicConnection.makeRecuperationStepTwo(executorData, queries[0], queries[1], bodyData.Get<string>("Token"), bodyData.Get<string>("NewPassword"), true);
                }
            );
            controllers["Employes"].mapRoute("ChangePassword",
                async (queries, bodyData) =>
                {
                    var userInfo = await DynamicConnection.checkUserInfo(bodyData.Get<string>("Email"), bodyData.Get<string>("Password"), queries[0], executorData);
                    if (userInfo is null)
                        return Results.Forbid();
                    if (await DynamicConnection.ChangePassword(executorData, queries[1], userInfo.userID, bodyData.Get<string>("NewPassword")))
                        return Results.Ok();
                    return Results.Forbid();
                }
            );
            //Routes
            controllers["ProduitsParCommande"].mapRoute("InsertPanier",
                (queries, bodyData) =>
                {
                    return insertProduitParCommande(executorData, queries, bodyData);
                }
                );
            controllers["ProduitsParCommande"].mapRoute("InsertWishList",
                (queries, bodyData) =>
                {
                    return insertProduitParCommande(executorData, queries, bodyData);
                }
                );
            controllers["ProduitsParCommande"].mapRoute("DeletePanier",
                async(queries, bodyData) =>
                {
                    var idProduitParCommande = bodyData.Get<int>("id");
                    queries[0].setParam("id", idProduitParCommande);
                    queries[1].setParam("id", idProduitParCommande);
                    queries[2].setParam("id", idProduitParCommande);             

                    if ((await executorData.ExecuteQueryWithTransaction(queries.toOrderedPairs())) == 0)
                        return Results.Forbid();

                    return Results.Ok();
                }

            );
            controllers["ProduitsParCommande"].mapRoute("MoveToPanier",
                async (queries, bodyData) =>
                {
                    queries[0].setParam("ClientID", bodyData.Get<long>("ClientID")).setParam("id", bodyData.Get<long>("id"));
                    if ((await executorData.ExecuteQueryWithTransaction(queries[0])) == 0)
                        return Results.Forbid();
                    return Results.Ok();
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
        public static async Task<IResult> insertProduitParCommande(SQLExecutor executorData, List<Query> queries, Dictionary<string, object> bodyData)
        {
            try
            {
                var produitParCommandeID = await executorData.ExecuteInsertWithLastID(queries[0].setParams(bodyData));
                if (produitParCommandeID == 0)
                    return Results.Forbid();
                var formatChoisis = bodyData.SafeGet<IEnumerable<object>>("FormatID");
                if (formatChoisis.Any())
                {
                    if ((await executorData.ExecuteQueryWithTransaction(queries[1].toOrderedPairs("FormatID", "ProduitCommandeID", produitParCommandeID, formatChoisis))) == 0)
                        return Results.Forbid();
                }
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.Forbid();
            }
        }
    }
}