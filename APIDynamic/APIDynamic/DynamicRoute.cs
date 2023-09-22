using DynamicSQLFetcher;
using Microsoft.AspNetCore.Mvc;

namespace APIDynamic
{
    public class DynamicRoute
    {
        public long id { get; set; }
        public string Name { get; set; }
        public List<DynamicQueryForRoute> queries { get; set; }
        public static readonly string[] BaseRoutes = new string[] { null, null, "GetAll", "Get" };
        public static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck FROM RouteQueries WHERE id_route = @routeID ORDER BY ind");
        public static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller) VALUES (@Name, @BaseRouteID, @ControllerID)");
        internal DynamicRoute(long id, string Name)
        {
            this.id = id;
            this.Name = Name;
            this.queries = new List<DynamicQueryForRoute>();
        }
        internal static async Task<DynamicRoute> init(DynamicRoute route)
        {
            route.queries = (await DynamicController.executor.SelectQueryTotal<DynamicQueryForRoute>(getQueries.setParam("routeID", route.id))).ToList();
            foreach (var query in route.queries)
                await DynamicQueryForRoute.init(query);
            return route;
        }
        public static Task addRoute(List<DynamicRoute> routes, long idController, long baseRoute)
        {
            return addRoute(routes, idController, null, baseRoute);
        }
        public static Task addRoute(List<DynamicRoute> routes, long idController, string Name)
        {
            long baseRoute = Array.IndexOf(BaseRoutes, Name);
            if (baseRoute == -1)
                return addRoute(routes, idController, Name, 1);
            return addRoute(routes, idController, null, baseRoute);
        }
        public async static Task addRoute(List<DynamicRoute> routes, long idController, string Name, long baseRoute)
        {
            routes.Add(new DynamicRoute(await DynamicController.executor.ExecuteInsertWithLastID(insertRoute.setParam("Name", Name).setParam("BaseRouteID", baseRoute).setParam("ControllerID", idController)), Name));
        }
        public Task addRouteQuery(string queryString, QueryTypes IDQueryType, bool CompleteCheck)
        {
            return DynamicQueryForRoute.addRouteQuery(queries, queryString, IDQueryType, id, CompleteCheck);
        }
        public Task addFilters(int index, string name, long ShowTypeID, string VarAffected)
        {
            return queries[index].addFilters(name, ShowTypeID, VarAffected);
        }
    }
}
