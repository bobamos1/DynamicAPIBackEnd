using DynamicSQLFetcher;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

namespace DynamicStructureObjects
{
    public class DynamicRoute
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public RouteTypes RouteType { get; internal set; }
        public List<DynamicQueryForRoute> Queries { get; internal set; }
        public List<long> Roles { get; internal set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.ARRAY, "SELECT id FROM PermissionRoutes INNER JOIN Roles ON id = id_role WHERE id_route = @RouteID", true, true);
        internal static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS QueryTypeID, completeCheck AS CompleteCheck, completeAuth AS CompleteAuth FROM RouteQueries WHERE id_route = @RouteID ORDER BY ind", true, true);
        internal static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller, id_routeType) VALUES (@Name, @BaseRouteID, @ControllerID, @RouteTypeID)", true, true);
        internal static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionProprieties(id_propriety, id_role) VALUES(@BaseRouteID, @RoleID)", true, true);
        internal static readonly Query getBaseRouteName = Query.fromQueryString(QueryTypes.VALUE, "SELECT Name FROM BaseRoutes WHERE id = @BaseRouteID", true, true);
        internal DynamicRoute(long id, string Name, long RouteTypeID)
        {
            this.id = id;
            this.Name = Name;
            this.RouteType = (RouteTypes)RouteTypeID;
            this.Queries = new List<DynamicQueryForRoute>();
            this.Roles = new List<long>();
        }
        internal static async Task<DynamicRoute> init(DynamicRoute route)
        {
            route.Queries = new List<DynamicQueryForRoute>();
            route.Queries = (await DynamicController.executor.SelectQuery<DynamicQueryForRoute>(getQueries.setParam("RouteID", route.id))).ToList(); 
            route.Roles = (await DynamicController.executor.SelectArray<long>(getRoles.setParam("RouteID", route.id))).ToList();
            route.Roles.Add(1);
            foreach (var query in route.Queries)
                await DynamicQueryForRoute.init(query);
            return route;
        }
        public static Task<DynamicRoute> addRoute(long idController, BaseRoutes baseRoute)
        {
            return addRoute(idController, null, baseRoute, baseRoute.Type());
        }
        public static Task<DynamicRoute> addRoute(long idController, string Name, RouteTypes routeType)
        {
            return addRoute(idController, Name, BaseRoutes.NONE, routeType);
        }
        public async static Task<DynamicRoute> addRoute(long idController, string Name, BaseRoutes baseRoute, RouteTypes routeType)
        {
            return new DynamicRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRoute
                        .setParam("Name", Name)
                        .setParam("BaseRouteID", (long)baseRoute)
                        .setParam("ControllerID", idController)
                        .setParam("RouteTypeID", (long)routeType)
                    )
                , Name is not null ? Name : baseRoute.Value()
                , (long)routeType
            );
        }
        public async Task<DynamicRoute> addRouteQuery(string queryString, QueryTypes QueryType, bool CompleteAuth, bool CompleteCheck)
        {
            Queries.Add(await DynamicQueryForRoute.addRouteQuery(Queries.Count + 1, queryString, QueryType, id, CompleteAuth, CompleteCheck));
            return this;
        }
        public Task<DynamicRoute> addSQLParamInfo(int indexQuery, string varAffected)
        {
            return addSQLParamInfo(indexQuery, varAffected, 1);
        }
        public async Task<DynamicRoute> addSQLParamInfo(int indexQuery, string varAffected, long ProprietyID)
        {
            await Queries[indexQuery].addSQLParamInfo(varAffected, ProprietyID);
            return this;
        }
        public Task<DynamicRoute> addSQLParamInfo(string varAffected)
        {
            return addSQLParamInfo(varAffected, 1);
        }
        public async Task<DynamicRoute> addSQLParamInfo(string varAffected, long ProprietyID)
        {
            await Queries.Last().addSQLParamInfo(varAffected, ProprietyID);
            return this;
        }

        public async Task<DynamicRoute> setValidator(string VarAffected, long ProprietyID, params ValidatorBundle[] ValidatorBundles)
        {
            await Queries.Last().setValidator(VarAffected, ProprietyID, ValidatorBundles);
            return this;
        }
        public async Task<DynamicRoute> setValidator(string VarAffected, params ValidatorBundle[] ValidatorBundles)
        {
            await Queries.Last().setValidator(VarAffected, 1, ValidatorBundles);
            return this;
        }
        public async Task<DynamicRoute> addValidator(int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await Queries[indexQuery].addValidator(VarAffected, Value, ValidatorType);
            return this;
        }
        public async Task<DynamicRoute> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            await Queries.Last().addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicRoute> addValidator(string varAffected, params ValidatorBundle[] validatorBundles)
        {
            await Queries.Last().addValidator(varAffected, validatorBundles);
            return this;
        }
        public async Task<DynamicRoute> addFilter(int indexQuery, string name, ShowTypes showType, string VarAffected)
        {
            await Queries[indexQuery].addFilter(name, showType, VarAffected);
            return this;
        }
        public async Task<DynamicRoute> addAuthorizedRole(long RoleID)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
                insertRole
                    .setParam("RoleID", RoleID)
                    .setParam("ProprietyID", id)
                );
            Roles.Add(RoleID);
            return this;
        }
        internal bool CanUse(params long[] rolesUser)
        {
            return CanUse((IEnumerable<long>)rolesUser);
        }

        internal bool CanUse(IEnumerable<long> rolesUser)
        {
            return rolesUser.Any(role => Roles.Contains(role));
        }
        internal bool validateParams(Dictionary<string, object> bodyData)
        {
            return Queries.All(query => query.validateParams(bodyData));
        }
    }
}
