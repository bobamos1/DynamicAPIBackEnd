﻿using DynamicSQLFetcher;
using DynamicStructureObjects;
using Microsoft.Extensions.Configuration;

namespace APIDynamic
{
    public static class RoutesInit
    {
        public static void InitRoutes(Dictionary<string, DynamicController> controllers, WebApplication app, Dictionary<string, string> connectionStrings)
        {
            SQLExecutor executorData = new SQLExecutor(connectionStrings["data"]);
            DynamicController.setApp(app);
            controllers["Produits"].addRouteAPI(RouteTypes.GET, "GetAll",
                async (queries, context) => Results.Ok(await executorData.SelectQuery(queries[0]))
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
