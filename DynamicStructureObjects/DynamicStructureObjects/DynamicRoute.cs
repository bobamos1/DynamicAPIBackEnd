﻿using DynamicSQLFetcher;

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
        public string paramForUserID { get; internal set; }
        public RouteDisplayTypes routeDisplayType { get; internal set; }

        public List<DynamicQueryForRoute> Queries { get; internal set; }
        public List<long> Roles { get; internal set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.ARRAY, "SELECT id FROM PermissionRoutes INNER JOIN Roles ON id = id_role WHERE id_route = @RouteID");
        internal static readonly Query getQueries = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, SQLString AS queryString, id_queryType AS QueryTypeID, completeAuth AS CompleteAuth, completeCheck AS CompleteCheck FROM RouteQueries WHERE id_route = @RouteID ORDER BY ind", true);
        internal static readonly Query insertRoute = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO URLRoutes (name, id_baseRoute, id_controller, id_routeType, requireAuthorization, getAuthorizedCols, onlyModify, id_routeDisplayType) VALUES (@Name, @BaseRouteID, @ControllerID, @RouteTypeID, @requireAuthorization, @getAuthorizedCols, @onlyModify, @RouteDisplayTypeID)", true);
        internal static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionRoutes (id_route, id_role) VALUES (@RouteID, @RoleID)", true);
        internal static readonly Query getBaseRouteName = Query.fromQueryString(QueryTypes.VALUE, "SELECT Name FROM BaseRoutes WHERE id = @BaseRouteID");
        internal static readonly Query updateRequired = Query.fromQueryString(QueryTypes.UPDATE, "UPDATE URLRoutes SET requireAuthorization = 1 WHERE @ID = id", true);
        internal static readonly Query updateParamForUserID = Query.fromQueryString(QueryTypes.UPDATE, "UPDATE URLRoutes SET paramForUserID = @paramForUserID WHERE id = @ID", true);
        internal DynamicRoute(long id, string Name, long RouteTypeID, bool requireAuthorization, bool getAuthorizedCols, bool onlyModify, string paramForUserID, long routeDisplayTypeID)
        {
            this.id = id;
            this.Name = Name;
            this.RouteType = (RouteTypes)RouteTypeID;
            this.Queries = new List<DynamicQueryForRoute>();
            this.Roles = new List<long>();
            this.requireAuthorization = requireAuthorization;
            this.getAuthorizedCols = getAuthorizedCols;
            this.onlyModify = onlyModify;
            this.paramForUserID = paramForUserID;
            this.routeDisplayType = (RouteDisplayTypes)routeDisplayTypeID;
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
            this.paramForUserID = dynamicRoute.paramForUserID;
            this.routeDisplayType = dynamicRoute.routeDisplayType;
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
            return addRoute(idController, null, baseRoute, baseRoute.Type(), baseRoute.DisplayType(), baseRoute.getAuthorizedCols(), baseRoute.onlyModify(), baseRoute.requireAuthorization());
        }
        public static Task<DynamicRoute> addRoute(long idController, string Name, RouteTypes routeType, RouteDisplayTypes routeDisplayType = RouteDisplayTypes.NONE, bool getAuthorizedCols = false, bool onlyModify = false, bool requireAuthorization = false)
        {
            return addRoute(idController, Name, BaseRoutes.NONE, routeType, routeDisplayType, getAuthorizedCols, onlyModify, requireAuthorization);
        }
        public async static Task<DynamicRoute> addRoute(long idController, string Name, BaseRoutes baseRoute, RouteTypes routeType, RouteDisplayTypes routeDisplayType, bool getAuthorizedCols, bool onlyModify, bool requireAuthorization)
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
                        .setParam("RouteDisplayTypeID", (long)routeDisplayType)
                    )
                , Name is not null ? Name : baseRoute.Value()
                , (long)routeType
                , requireAuthorization
                , getAuthorizedCols
                , onlyModify
                , null
                , (long)routeDisplayType
            );
        }
        public async Task<DynamicRoute> bindParamToUserID(string paramName)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(updateParamForUserID.setParam("paramForUserID", paramName).setParam("ID", id));
            return this;
        }
        public async Task<DynamicRoute> addRouteQuery(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true, bool withVar = true)
        {
            if (CompleteAuth is null)
                CompleteAuth = QueryType.CompleteAuth();
            Queries.Add(await DynamicQueryForRoute.addRouteQuery(Queries.Count + 1, queryString, QueryType, id, (bool)CompleteAuth, CompleteCheck, withVar));
            return this;
        }
        public Task<DynamicRoute> addRouteQueryNoVar(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            return addRouteQuery(queryString, QueryType, CompleteAuth, CompleteCheck, false);
        }
        public DynamicRoute addEmptyQuery()
        {
            Queries.Add(DynamicQueryForRoute.addEmptyQuery());
            return this;
        }
        
        public Task<DynamicRoute> addSQLParam(int indexQuery, string varAffected, ShowTypes? showType, int ind)
        {
            return addSQLParam(indexQuery, varAffected, 1, showType, ind);
        }
        public async Task<DynamicRoute> addSQLParam(int indexQuery, string varAffected, long ProprietyID, ShowTypes? showType, int ind)
        {
            await Queries[indexQuery].addSQLParam(varAffected, ProprietyID, showType, ind);
            return this;
        }
        public Task<DynamicRoute> addSQLParam(string varAffected, ShowTypes? showType, int ind)
        {
            return addSQLParam(varAffected, 1, showType, ind);
        }
        public async Task<DynamicRoute> addSQLParam(string varAffected, long ProprietyID, ShowTypes? showType, int ind)
        {
            await Queries.Last().addSQLParam(varAffected, ProprietyID, showType, ind);
            return this;
        }

        public async Task<DynamicRoute> setSQLParam(string VarAffected, ShowTypes? showType, int? ind, params ValidatorBundle[] ValidatorBundles)
        {
            await Queries.Last().setSQLParam(VarAffected, 1, false, showType, ind, ValidatorBundles);
            return this;
        }
        public async Task<DynamicRoute> setSQLParam(string VarAffected, long ProprietyID, ShowTypes? showType, int? ind, params ValidatorBundle[] ValidatorBundles)
        {
            await Queries.Last().setSQLParam(VarAffected, ProprietyID, false, showType, ind, ValidatorBundles);
            return this;
        }
        public async Task<DynamicRoute> setNotRequired(params string[] VarsAffected)
        {
            await Queries.Last().setNotRequired(VarsAffected);
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
        public async Task<DynamicRoute> addValidator(string varAffected, bool addRequired, params ValidatorBundle[] validatorBundles)
        {
            await Queries.Last().addValidator(varAffected, addRequired, validatorBundles);
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
    }
}
