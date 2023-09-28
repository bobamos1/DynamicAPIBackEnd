using DynamicSQLFetcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using System.Data;

namespace DynamicStructureObjects
{
    public class DynamicController
    {
        internal long id { get; set; }
        public string Name { get; internal set; }
        internal bool IsMain { get; set; }
        internal static SQLExecutor executor { get; set; }
        internal static WebApplication app { get; set; }
        internal static Dictionary<string, long> RolesAvailable { get; set; }
        internal List<DynamicRoute> Routes { get; set; }
        internal List<DynamicPropriety> Proprieties { get; set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT name AS Name, id AS id FROM Roles", true, true);
        internal static readonly Query getControllers = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, name AS Name, isMain AS IsMain FROM Controllers", true, true);
        internal static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, isReadOnly AS ReadOnly, id_ShowType AS ShowTypeID FROM Proprieties WHERE id_controller = @controllerID", true, true);
        internal static readonly Query getRoutes = Query.fromQueryString(QueryTypes.SELECT, "SELECT URLRoutes.id AS id, COALESCE(BaseRoutes.name, URLRoutes.name) AS Name FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute WHERE URLRoutes.id_controller = @controllerID", true, true);
        internal static readonly Query insertController = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Controllers (name, isMain) VALUES (@Name, @IsMain)", true, true);
        private DynamicController(long id, string Name, bool IsMain)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.Routes = new List<DynamicRoute>();
            this.Proprieties = new List<DynamicPropriety>();
        }
        private static async Task<DynamicController> init(DynamicController controller)
        {
            
            controller.Routes = (await executor.SelectQuery<DynamicRoute>(getRoutes.setParam("controllerID", controller.id))).ToList();
            controller.Proprieties = (await executor.SelectQuery<DynamicPropriety>(getProprieties.setParam("controllerID", controller.id))).ToList();
            foreach (var route in controller.Routes)
                await DynamicRoute.init(route);
            foreach (var propriety in controller.Proprieties)
                await DynamicPropriety.init(propriety);
            return controller;
        }
        public static async Task<Dictionary<string, DynamicController>> initControllers(SQLExecutor executor)
        {
            DynamicController.executor = executor;
            RolesAvailable = await executor.SelectDictionary<string, long>(getRoles);
            Dictionary<string, DynamicController> controllers = new Dictionary<string, DynamicController>();
            foreach (var controller in await executor.SelectQuery<DynamicController>(getControllers))
                controllers.Add(controller.Name, await init(controller));
            return controllers;
        }
        public static async Task resetStructureData()
        {
            List<KeyValuePair<string, bool>> tablesToDeleteContent = new List<KeyValuePair<string, bool>>()
                .Add("ValidatorProprietyValues", false)
                .Add("ValidatorSQLParamInfoValues", false)
                .Add("Filters", true)
                .Add("SQLParamInfos", true)
                .Add("PermissionRoutes", false)
                .Add("PermissionProprieties", false)
                .Add("RouteQueries", true)
                .Add("URLRoutes", true)
                .Add("ListVars", true)
                .Add("LinkProprietiesControllers", true)
                .Add("Proprieties", true)
                .Add("Controllers", true)
            ;
            List<string> queriesToRun = new List<string>();
            foreach (var table in tablesToDeleteContent)
            {
                queriesToRun.Add($"DELETE {table.Key}");
                if (table.Value)
                    queriesToRun.Add($"DBCC CHECKIDENT ('{table.Key}', RESEED, 0)");
            }
            queriesToRun.Add("INSERT Controllers (name, isMain) VALUES ('NULL', 0)");
            queriesToRun.Add("INSERT Proprieties (name, isMain, id_ShowType, id_controller, isReadOnly) VALUES ('NULL', 0, 1, 1, 1)");
            await executor.ExecuteQueryWithTransaction(queriesToRun.ToArray());
        }
        #region adds
        public async static Task<DynamicController> addController(string Name, bool IsMain)
        {
            return new DynamicController(
                await executor.ExecuteInsertWithLastID(
                    insertController
                        .setParam("Name", Name)
                        .setParam("IsMain", IsMain)
                    )
                , Name
                , IsMain
            );
        }
        public async Task<DynamicController> addRoute(BaseRoutes baseRoute)
        {
            Routes.Add(await DynamicRoute.addRoute(id, baseRoute));
            return this;
        }
        public async Task<DynamicController> addRoute(string Name)
        {
            Routes.Add(await DynamicRoute.addRoute(id, Name));
            return this;
        }
        public async Task<DynamicController> addRouteQuery(string routeName, string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            await Routes.First(route => route.Name == routeName).addRouteQuery(queryString, QueryType, CompleteCheck, CompleteAuth);
            return this;
        }
        public async Task<DynamicController> addRouteQuery(string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            await Routes.Last().addRouteQuery(queryString, QueryType, CompleteCheck, CompleteAuth);
            return this;
        }
        public async Task<DynamicController> addSQLParamInfo(string routeName, int index, string varAffected, string ProprietyName)
        {
            long proprietyID = 1;
            if (ProprietyName is not null)
                proprietyID = Proprieties.First(propriety => propriety.Name == ProprietyName).id;
            await Routes.First(route => route.Name == routeName).addSQLParamInfo(index, varAffected, proprietyID);
            return this;
        }
        public async Task<DynamicController> addSQLParamInfo(string varAffected, string ProprietyName)
        {
            long proprietyID = 1;
            if (ProprietyName is not null)
                proprietyID = Proprieties.First(propriety => propriety.Name == ProprietyName).id;
            await Routes.Last().addSQLParamInfo(varAffected, proprietyID);
            return this;
        }
        public Task<DynamicController> addSQLParamInfo(string varAffected)
        {
            return addSQLParamInfo(varAffected, null);
        }
        public async Task<DynamicController> addValidatorForSQLParam(string routeName, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await Routes.First(route => route.Name == routeName).addValidator(indexQuery, VarAffected, Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addValidatorForSQLParam(string Value, ValidatorTypes ValidatorType)
        {
            await Routes.Last().addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addFilter(string routeName, int index, string name, ShowTypes showType, string VarAffected)
        {
            await Routes.First(route => route.Name == routeName).addFilter(index, name, showType, VarAffected);
            return this;
        }
        public async Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsReadOnly, ShowTypes showType)
        {
            Proprieties.Add(await DynamicPropriety.addPropriety(Name, IsMain, IsReadOnly, showType, id));
            return this;
        }
        public async Task<DynamicController> addValidatorForPropriety(string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addMapperGenerator(string ProprietyName, string ControllerName, params ParamLinker[] linkers)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addMapperGenerator(ControllerName, linkers);
            return this;
        }
        public async Task<DynamicController> addAuthorizedRouteRole(string routeName, long RoleID)
        {
            await Routes.First(route => route.Name == routeName).addAuthorizedRole(RoleID);
            return this;
        }
        public async Task<DynamicController> addAuthorizedProprietyRole(string ProprietyName, long RoleID, bool CanModify)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addAuthorizedRole(RoleID, CanModify);
            return this;
        }
        #endregion
        internal IEnumerable<string> AuthorizedToSee(params long[] roles)
        {
            return Proprieties.Where(propriety => propriety.CanSee(roles)).Select(propriety => propriety.Name);
        }
        internal IEnumerable<string> AuthorizedToModify(params long[] roles)
        {
            return Proprieties.Where(propriety => propriety.CanModify(roles)).Select(propriety => propriety.Name);
        }
        public IEnumerable<DynamicPropriety> getAuthorizedProprieties(params long[] roles)
        {
            return Proprieties.Where(propriety => propriety.CanSee(roles));
        }
        public IEnumerable<DynamicRoute> getAuthorizedRoutes(params long[] roles)
        {
            return Routes.Where(route => route.CanUse(roles) || true);
        }
        public static void initRoutesControllersInfo(WebApplication app, List<DynamicController> controllers)
        {
            DynamicController.app = app;
            foreach (var controller in controllers)
                controller.setBaseInfoRoutes();
        }
        internal void setBaseInfoRoutes()
        {
            app.MapGet($"/{Name}/Info/Propriety", (HttpRequest request) =>
            {
                return Task.FromResult<IResult>(Results.Ok(getAuthorizedProprieties(new long[] { 1, 2 })));
            }).WithName($"{Name}InfoPropriety");
            app.MapGet($"/{Name}/Info/Routes", (HttpRequest request) =>
            {
                return Task.FromResult<IResult>(Results.Ok(getAuthorizedRoutes(new long[] { 1, 2 })));
            }).WithName($"{Name}InfoRoutes");
            foreach (var route in Routes)
            {
                app.MapGet($"/{Name}/Info/RouteFilters/{route.Name}", (HttpRequest request) =>
                {
                    if (!route.CanUse(new long[] { 1, 2 }))
                        return Task.FromResult<IResult>(Results.Forbid());
                    List<DynamicFilter> filters = new List<DynamicFilter>();
                    foreach (var query in route.Queries)
                        filters.AddRange(query.Filters);
                    return Task.FromResult<IResult>(Results.Ok(filters));
                }).WithName($"{Name}Info{route.Name}Filters");
                app.MapGet($"/{Name}/Info/RouteVariables/{route.Name}", (HttpRequest request) =>
                {
                    if (!route.CanUse(new long[] { 1, 2 }))
                        return Task.FromResult<IResult>(Results.Forbid());
                    List<DynamicSQLParamInfo> paramInfo = new List<DynamicSQLParamInfo>();
                    foreach (var query in route.Queries)
                        paramInfo.AddRange(query.ParamsInfos.Values);
                    return Task.FromResult<IResult>(Results.Ok(paramInfo));
                }).WithName($"{Name}Info{route.Name}Variables");
            }
        }
        public long getProperityID(string ProprietyName)
        {
            return Proprieties.First(propriety => propriety.Name == ProprietyName).id;
        }
        public void addRouteAPI(RouteTypes routeType, string routeName, Func<List<Query>, dynamic, IResult> function)
        {
            addRouteAPI(routeType, routeName, async (queries, context) => function(queries, context));
        }

        public void addRouteAPI(RouteTypes routeType, string routeName, Func<List<Query>, dynamic, Task<IResult>> function)
        {
            DynamicRoute route = Routes.First(route => route.Name == routeName);
            List<Query> queries = route.Queries.Select(dynamicQuery => dynamicQuery.query).ToList();

            Func<HttpRequest, Task<IResult>> delegateMethod = async (context) =>
            {
                /*
                if (!route.CanUse( new long[] { 1, 2 })))
                    return Results.Forbid();
                */
                using (var requestBodyStream = new StreamReader(context.Body))
                {
                    string requestBody = await requestBodyStream.ReadToEndAsync();
                    dynamic keyValuePairs = null;
                    if (!string.IsNullOrWhiteSpace(requestBody))
                        keyValuePairs = JsonSerializer.Deserialize<dynamic>(requestBody);

                    return await function(queries, keyValuePairs);
                }
            };
            app.MapRoute(routeType, $"/{Name}/{routeName}", delegateMethod).WithName($"{Name}{routeName}");
        }

        public static bool Cross(IEnumerable<long> arr1, IEnumerable<long> arr2)
        {
            return arr1.Any(item => arr2.Contains(item));
        }
    }
}
