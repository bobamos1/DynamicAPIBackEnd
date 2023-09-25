using DynamicSQLFetcher;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Xml.Linq;

namespace APIDynamic
{
    public class DynamicRoute
    {
        public long id { get; set; }
        public string Name { get; set; }
        public List<DynamicQueryForRoute> queries { get; set; }
        public List<long> roles { get; set; }
        public static readonly Query getRoles = Query.fromQueryString(QueryTypes.ARRAY, "SELECT id FROM PermissionRoutes INNER JOIN Roles ON id = id_role WHERE id_route = @RouteID", true, true);
        public static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck, completeAuth AS CompleteAuth FROM RouteQueries WHERE id_route = @RouteID ORDER BY ind", true, true);
        public static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller) VALUES (@Name, @BaseRouteID, @ControllerID)", true, true);
        public static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionProprieties(id_propriety, id_role) VALUES(@BaseRouteID, @RoleID)", true, true);
        public static readonly Query getBaseRouteName = Query.fromQueryString(QueryTypes.VALUE, "SELECT Name FROM BaseRoutes WHERE id = @BaseRouteID", true, true);
        internal DynamicRoute(long id, string Name)
        {
            this.id = id;
            this.Name = Name;
            this.queries = new List<DynamicQueryForRoute>();
        }
        internal static async Task<DynamicRoute> init(DynamicRoute route)
        {
            route.queries = (await DynamicController.executor.SelectQuery<DynamicQueryForRoute>(getQueries.setParam("RouteID", route.id))).ToList(); 
            route.roles = (await DynamicController.executor.SelectArray<long>(getRoles.setParam("RouteID", route.id))).ToList();
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
        public async Task<DynamicRoute> addRouteQuery(string queryString, QueryTypes IDQueryType, bool CompleteCheck, bool CompleteAuth)
        {
            queries.Add(await DynamicQueryForRoute.addRouteQuery(queries.Count, queryString, IDQueryType, id, CompleteCheck, CompleteAuth));
            return this;
        }
        public Task addFilters(int index, string name, long ShowTypeID, string VarAffected)
        {
            return queries[index].addFilters(name, ShowTypeID, VarAffected);
        }
        public async Task<DynamicRoute> addAuthorizedRole(long RoleID)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
                insertRole
                    .setParam("RoleID", RoleID)
                    .setParam("ProprietyID", id)
                );
            roles.Add(RoleID);
            return this;
        }
        public bool CanUse(params long[] rolesUser)
        {
            return rolesUser.Any(role => roles.Contains(role));
        }
    }
}
