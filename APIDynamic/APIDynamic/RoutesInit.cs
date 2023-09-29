﻿using DynamicSQLFetcher;
using DynamicStructureObjects;
using Microsoft.AspNetCore.Mvc;
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
            DynamicController.initRoutesControllersInfo(app, controllers.Values.ToList());
            controllers["Produits"].addRouteAPI(RouteTypes.POST, "GetAll",
                async (queries, bodyData) =>
                {
                    string token = DynamicController.CreateToken("bob", "bob@gmail.com", 1, 1, 2, 3);
                    return Results.Ok(token);
                    return Results.Ok(bodyData);
                    //return Results.Ok(await executorData.SelectQuery(queries[0]));
                }, true
            );
            controllers["Commandes"].addRouteAPI(RouteTypes.POST, "GetClientsCommande",
                async (queries, bodyData) =>
                {
                    var dictionary = (Dictionary<string, object>)bodyData["conds"];
                    long da = (long)dictionary["id_User"];
                    return Results.Ok(bodyData);
                }, true
            );
            //Ajoute les routes de l'API ici
            /*
            controllers["Commandes"].addRouteAPI(RouteTypes.POST, "CommandeCheckout",
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
