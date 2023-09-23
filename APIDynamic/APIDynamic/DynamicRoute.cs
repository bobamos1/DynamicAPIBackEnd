using DynamicSQLFetcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace APIDynamic
{
    public class DynamicRoute
    {
        public long id { get; set; }
        public string Name { get; set; }
        public List<DynamicQueryForRoute> queries { get; set; }
        public static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck FROM RouteQueries WHERE id_route = @routeID ORDER BY ind");
        public static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller) VALUES (@Name, @BaseRouteID, @ControllerID)");
        public static readonly Query getBaseRouteName = Query.fromQueryString(QueryTypes.VALUE, "SELECT Name FROM BaseRoutes WHERE id = @BaseRouteID");
        internal DynamicRoute(long id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        internal static async Task<DynamicRoute> init(DynamicRoute route)
        {
            route.queries = (await DynamicController.executor.SelectQueryTotal<DynamicQueryForRoute>(getQueries.setParam("routeID", route.id))).ToList();
            foreach (var query in route.queries)
                await DynamicQueryForRoute.init(query);
            return route;
        }
        public static Task<DynamicRoute> addRoute(long idController, BaseRoutes baseRoute)
        {
            return addRoute(idController, null, baseRoute);
        }
        public static Task<DynamicRoute> addRoute(long idController, string Name)
        {
            return addRoute(idController, Name, BaseRoutes.NONE);
        }
        public async static Task<DynamicRoute> addRoute(long idController, string Name, BaseRoutes baseRoute)
        {
            return new DynamicRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRoute
                        .setParam("Name", Name)
                        .setParam("BaseRouteID", (long)baseRoute)
                        .setParam("ControllerID", idController)
                    )
                , Name is not null ? Name : baseRoute.Value()
            );
        }
        public async Task<DynamicRoute> addRouteQuery(string queryString, QueryTypes IDQueryType, bool CompleteCheck)
        {
            queries.Add(await DynamicQueryForRoute.addRouteQuery(queries.Count, queryString, IDQueryType, id, CompleteCheck));
            return this;
        }
        public async Task<DynamicRoute> addSQLParamInfo(int index, string varAffected, long ProprietyID)
        {
            await queries[index].addSQLParamInfo(varAffected, ProprietyID);
            return this;
        }
        public async Task<DynamicRoute> addValidator(int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await queries[indexQuery].addValidator(VarAffected, Value, ValidatorType);
            return this;
        }
        public async Task<DynamicRoute> addFilter(int index, string name, ShowTypes showType, string VarAffected)
        {
            await queries[index].addFilter(name, showType, VarAffected);
            return this;
        }
    }
}
