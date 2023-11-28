using Dapper;
using DynamicSQLFetcher;
using DynamicStructureObjects;
using ParserLib;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System;
using System.Collections.Immutable;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

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
                (queries, bodyData) =>  DynamicConnection.makeConnectionStepOne(executorData, queries[0], queries[1], bodyData.Get<string>("Email"), bodyData.Get<string>("Password"))
            );
            controllers["Clients"].mapRoute("ConnexionStepTwo",
                (queries, bodyData) => DynamicConnection.makeConnectionStepTwo(executorData, queries[0], bodyData.Get<string>("Token"), false, false, Roles.Client.ID())
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
                    if ((await executorData.ExecuteQueryWithTransaction(queries[1].setParam("ClientID", id))) == 0)
                        return Results.Problem();
                    return Results.Ok(DynamicConnection.CreateToken(id, userInfo, Roles.Client.ID()));
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
                (queries, bodyData) => DynamicConnection.makeRecuperationStepTwo(executorData, queries[0], queries[1], bodyData.Get<string>("Token"), bodyData.Get<string>("NewPassword"), false, Roles.Client.ID())
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
                    return DynamicConnection.makeConnectionStepTwo(executorData, queries[0], bodyData.Get<string>("Token"), true, true);
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

            var stripeApiKey = "sk_test_51O1boKAHfZleTlSeTjEqgkKrxHVsyKDs5R0fUpfGTpvtqGwkhxkjVNVA11waQoRLfFTvHej7Nq6t1apDKGFMqkhB00mDMWqPQl";
            StripeConfiguration.ApiKey = stripeApiKey;

            controllers["Commandes"].mapRoute("CheckoutPanier",
                async (queries, bodyData) =>
                {

                    var idClient = bodyData.UserID();
                    var ProduitsParCommande = await executorData.SelectArray<long>(queries[0].setParam("ClientID", idClient));

                    var montantTotal = 0;
                    var itemMontant = 0;


                    foreach (long idProduitParCommande in ProduitsParCommande)
                    {

                        //ExecuteQueryWithTransaction c,est le npombre de row affected
                        if (await executorData.ExecuteQueryWithTransaction(queries[1].setParam("ClientID", idClient).setParam("ProduitParCommandeID", idProduitParCommande)) == 0)
                                return Results.Forbid();
                        else
                        {
                            itemMontant = await executorData.SelectSingle(queries[2].setParam("ProduitParCommandeID", idProduitParCommande));
                            montantTotal += itemMontant;
                        }
                    }

                    if ((await executorData.ExecuteQueryWithTransaction(queries[3].setParam("ClientID", idClient).setParam("no_civique", bodyData.SafeGet<int>("no_civique")).setParam("rue", bodyData.SafeGet<string>("rue")).setParam("VilleID", bodyData.SafeGet<string>("VilleID"))) == 0))
                        return Results.Forbid();

                    string CommandeNom = "Commande";
                    montantTotal = montantTotal * 100;      //Mettre la valeur en cenne pour Stripe


                    StripeConfiguration.ApiKey = stripeApiKey;  //Défini avant l'appel de la route

                    var options = new Stripe.Checkout.SessionCreateOptions
                    {
                        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                        {
                            new Stripe.Checkout.SessionLineItemOptions
                            {
                                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                                {
                                    Currency = "cad",
                                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = CommandeNom,
                                    },
                                    UnitAmount = montantTotal,
                                    TaxBehavior = "unspecified",
                                },
                                AdjustableQuantity = new Stripe.Checkout.SessionLineItemAdjustableQuantityOptions
                                {
                                    Enabled = true,
                                    Minimum = 1,
                                    Maximum = 10,
                                },
                                Quantity = 1,
                            },
                        },
                        AutomaticTax = new Stripe.Checkout.SessionAutomaticTaxOptions { Enabled = false },
                        Mode = "payment",
                        SuccessUrl = "http://localhost:4200/success",
                        CancelUrl = "http://localhost:4200/cancel",
                    };
                    var service = new Stripe.Checkout.SessionService();
                    

                    return Results.Ok(service.Create(options));
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

            controllers["Couleurs"].mapRoute("GetValues", 
                async (queries, bodyData) =>
                {
                    return Results.Ok(await executorData.SelectDictionary(queries[0]));
                });

            controllers["Clients"].mapRoute("Contacter",
                async (queries, bodyData) =>
                {

                    string Message = bodyData.Get<string>("Email") + " : " + bodyData.Get<string>("Contenu");


                    return Results.Ok(DynamicConnection.emailSender.SendEmail(app.Configuration["Email:EmailHost"], bodyData.Get<string>("Sujet"), Message, null));
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