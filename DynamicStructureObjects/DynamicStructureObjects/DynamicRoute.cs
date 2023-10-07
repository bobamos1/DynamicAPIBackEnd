using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicRoute
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public RouteTypes RouteType { get; internal set; }
        public bool requireAuthorization { get; internal set; }
        public bool getAuthorizedCols { get; internal set; }
        public bool onlyModify { get; internal set; }
        public List<DynamicQueryForRoute> Queries { get; internal set; }
        public List<long> Roles { get; internal set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.ARRAY, "SELECT id FROM PermissionRoutes INNER JOIN Roles ON id = id_role WHERE id_route = @RouteID", true);
        internal static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS QueryTypeID, completeAuth AS CompleteAuth, completeCheck AS CompleteCheck FROM RouteQueries WHERE id_route = @RouteID ORDER BY ind", true);
        internal static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller, id_routeType, requireAuthorization, getAuthorizedCols, onlyModify) VALUES (@Name, @BaseRouteID, @ControllerID, @RouteTypeID, @requireAuthorization, @getAuthorizedCols, @onlyModify)", true);
        internal static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionRoutes (id_route, id_role) VALUES (@RouteID, @RoleID)", true);
        internal static readonly Query getBaseRouteName = Query.fromQueryString(QueryTypes.VALUE, "SELECT Name FROM BaseRoutes WHERE id = @BaseRouteID", true);
        internal static readonly Query updateRequired = Query.fromQueryString(QueryTypes.UPDATE, "UPDATE URLRoutes SET requireAuthorization = 1 WHERE @ID = id", true);
        internal DynamicRoute(long id, string Name, long RouteTypeID, bool requireAuthorization, bool getAuthorizedCols, bool onlyModify)
        {
            this.id = id;
            this.Name = Name;
            this.RouteType = (RouteTypes)RouteTypeID;
            this.Queries = new List<DynamicQueryForRoute>();
            this.Roles = new List<long>();
            this.requireAuthorization = requireAuthorization;
            this.getAuthorizedCols = getAuthorizedCols;
            this.onlyModify = onlyModify;
        }
        internal DynamicRoute(DynamicRoute dynamicRoute, BaseRoutes baseRoute, bool requiredID = false)
        {
            this.id = dynamicRoute.id;
            this.Name = baseRoute.Value();
            this.RouteType = dynamicRoute.RouteType;
            this.Queries = dynamicRoute.Queries.ToList();
            this.Roles = dynamicRoute.Roles;
            this.requireAuthorization = baseRoute.requireAuthorization();
            this.getAuthorizedCols = baseRoute.getAuthorizedCols();
            this.onlyModify = baseRoute.onlyModify();
            if (requiredID)
                Queries[0] = new DynamicQueryForRoute(Queries[0], true);
        }
        internal static async Task<DynamicRoute> init(DynamicRoute route)
        {
            route.Queries = (await DynamicController.executor.SelectQuery<DynamicQueryForRoute>(getQueries.setParam("RouteID", route.id))).ToList(); 
            route.Roles = (await DynamicController.executor.SelectArray<long>(getRoles.setParam("RouteID", route.id))).ToList();
            //route.Roles.Add(1);
            foreach (var query in route.Queries)
                await DynamicQueryForRoute.init(query);
            return route;
        }
        public static Task<DynamicRoute> addRoute(long idController, BaseRoutes baseRoute)
        {
            return addRoute(idController, null, baseRoute, baseRoute.Type(), baseRoute.getAuthorizedCols(), baseRoute.onlyModify(), baseRoute.requireAuthorization());
        }
        public static Task<DynamicRoute> addRoute(long idController, string Name, RouteTypes routeType, bool getAuthorizedCols = false, bool onlyModify = false, bool requireAuthorization = false)
        {
            return addRoute(idController, Name, BaseRoutes.NONE, routeType, getAuthorizedCols, onlyModify, requireAuthorization);
        }
        public async static Task<DynamicRoute> addRoute(long idController, string Name, BaseRoutes baseRoute, RouteTypes routeType, bool getAuthorizedCols, bool onlyModify, bool requireAuthorization)
        {
            return new DynamicRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRoute
                        .setParam("Name", Name)
                        .setParam("BaseRouteID", (long)baseRoute)
                        .setParam("ControllerID", idController)
                        .setParam("RouteTypeID", (long)routeType)
                        .setParam("requireAuthorization", requireAuthorization)
                        .setParam("getAuthorizedCols", getAuthorizedCols)
                        .setParam("onlyModify", onlyModify)
                    )
                , Name is not null ? Name : baseRoute.Value()
                , (long)routeType
                , requireAuthorization
                , getAuthorizedCols
                , onlyModify
            );
        }
        public async Task<DynamicRoute> addRouteQuery(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            if (CompleteAuth is null)
                CompleteAuth = QueryType.CompleteAuth();
            Queries.Add(await DynamicQueryForRoute.addRouteQuery(Queries.Count + 1, queryString, QueryType, id, (bool)CompleteAuth, CompleteCheck));
            return this;
        }
        public DynamicRoute addEmptyQuery()
        {
            Queries.Add(DynamicQueryForRoute.addEmptyQuery());
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
        public async Task<DynamicRoute> addAuthorizedRoles(params long[] RolesID)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(updateRequired.setParam("ID", id));
            foreach (var RoleID in RolesID)
                await addAuthorizedRole(RoleID);
            return this;
        }
        public async Task<DynamicRoute> addAuthorizedRole(long RoleID)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(
                insertRole
                    .setParam("RoleID", RoleID)
                    .setParam("RouteID", id)
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
            return !requireAuthorization || Roles.Any(role => rolesUser.Contains(role));
        }
        internal bool validateParams(Dictionary<string, object> bodyData)
        {
            return Queries.All(query => query.validateParams(bodyData));
        }

        internal static Task<DynamicRoute> addRoute(long id, BaseRoutes baseRoute, bool v, object value1, object value2)
        {
            throw new NotImplementedException();
        }
    }
}
