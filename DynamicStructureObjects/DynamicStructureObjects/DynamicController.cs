using DynamicSQLFetcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace DynamicStructureObjects
{
    public class DynamicController
    {
        public static readonly string ISASCENDINGKEY = "IsAscending";
        public readonly static string STEPKEY = "Step";
        public readonly static int DEFAULTSTEP = 25;
        public readonly static int DEFAULTPAGE = 1;
        public readonly static string PAGEKEY = "Page";
        public readonly static string USERIDKEY = "CurrentUserID";
        public readonly static string PROPRETYKEY = "AuthorizedProprieties";
        public readonly static string IDParamsKey = "IDParams";
        public readonly static string ROLESKEY = "CurrentUserRoles";
        public string CBOString => BaseRouteString(BaseRoutes.CBO);
        public string GetAllString => BaseRouteString(BaseRoutes.GETALL);
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsMain { get; internal set; }
        public IEnumerable<long> Roles { get; internal set; }
        internal static SQLExecutor executor { get; set; }
        internal static WebApplication app { get; set; }
        internal static Dictionary<string, long> RolesAvailable { get; set; }
        public List<DynamicRoute> Routes { get; internal set; }
        public List<DynamicPropriety> Proprieties { get; internal set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT name AS Name, id AS id FROM Roles");
        internal static readonly Query getControllers = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, name AS Name, isMain AS IsMain FROM Controllers", true);
        internal static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, isUpdatable AS IsUpdatable, id_ShowType AS ShowTypeID, ind AS ind, description AS description, displayName AS displayName, placeholder AS placeholder FROM Proprieties WHERE id_controller = @controllerID", true);
        internal static readonly Query getRoutes = Query.fromQueryString(QueryTypes.SELECT, "SELECT URLRoutes.id AS id, CASE WHEN URLRoutes.id_baseRoute = 1 THEN URLRoutes.name ELSE BaseRoutes.name END AS Name, id_routeType AS RouteTypeID, requireAuthorization AS requireAuthorization, getAuthorizedCols AS getAuthorizedCols, onlyModify AS onlyModify, paramForUserID AS paramForUserID, id_routeDisplayType AS routeDisplayTypeID FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute WHERE URLRoutes.id_controller = @controllerID", true);
        internal static readonly Query insertController = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Controllers (name, isMain) VALUES (@Name, @IsMain)", true);
        private DynamicController(long id, string Name, bool IsMain)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.Routes = new List<DynamicRoute>();
            this.Proprieties = new List<DynamicPropriety>();
            this.Roles = new long[0];
        }
        public Query GetGetAllRoute()
        {
            return Routes.FirstOrDefault(route => route.Name == BaseRoutes.GETALL.Value())?.Queries.First().query;
        }
        private static async Task<DynamicController> init(DynamicController controller)
        {

            if (controller.id == 1)
                return controller;

            controller.Proprieties = (await executor.SelectQuery<DynamicPropriety>(getProprieties.setParam("controllerID", controller.id))).ToList();
            controller.Routes = (await executor.SelectQuery<DynamicRoute>(getRoutes.setParam("controllerID", controller.id))).ToList();
            foreach (var propriety in controller.Proprieties)
                await DynamicPropriety.init(propriety);
            foreach (var route in controller.Routes)
                await DynamicRoute.init(route, controller.Proprieties);

            var getAllRoute = controller.Routes.FirstOrDefault(route => route.Name == BaseRoutes.GETALL.Value());
            var ids = controller.GetIDProprieties();
            controller.Roles = controller.Proprieties.SelectMany(prop => prop.roles.Select(role => role.Key)).Distinct();
            if (getAllRoute is null)
                throw new Exception($"Need getAll for controller {controller.Name}");
            if (!controller.hasRoute(BaseRoutes.GETALLDETAILED.Value()))
                controller.Routes.Add(new DynamicRoute(getAllRoute, BaseRoutes.GETALLDETAILED, new string[0]));
            if (!controller.hasRoute(BaseRoutes.GET.Value()))
            {
                var paramInfos = getAllRoute.Queries.First().ParamsInfos;
                if (!ids.Any() || ids.Any(id => !paramInfos.ContainsKey(id)))
                    throw new Exception($"Need all ids in route GetAll {controller.Name}");
                controller.Routes.Add(new DynamicRoute(getAllRoute, BaseRoutes.GET, ids));
                if (!controller.hasRoute(BaseRoutes.GETDETAILED.Value()))
                    controller.Routes.Add(new DynamicRoute(getAllRoute, BaseRoutes.GETDETAILED, ids));
            }
            else if (!controller.hasRoute(BaseRoutes.GETDETAILED.Value()))
                controller.Routes.Add(new DynamicRoute(getAllRoute, BaseRoutes.GETDETAILED, new string[0]));
            if (!controller.hasRoute(BaseRoutes.CBO.Value()))
                throw new Exception($"Need CBO route for controller {controller.Name}");
            var allParams = getAllRoute.Queries.SelectMany(query => query.query.selectColumns.Keys);
            if (!controller.Proprieties.All(prop => (prop.ShowType.IsRef() || allParams.Contains(prop.Name)) /*!prop.ShowType.IsRef() && allParams.Contains(prop.Name)*/))
                throw new Exception($"Need all prop for getAll route for controller {controller.Name}");
            return controller;
        }
        public string BaseRouteString(BaseRoutes baseRoute)
        {
            return getRoute(baseRoute.Value()).FirstQuery.originalQuery;
        }
        public Query BaseRouteQuery(BaseRoutes baseRoute)
        {
            return getRoute(baseRoute.Value()).FirstQuery;
        }
        public static async Task<Dictionary<string, DynamicController>> initControllers(SQLExecutor executor, string apiKey)
        {
            if (DynamicConnection.CourrielTokenBody is null || DynamicConnection.CourrielTokenSubject is null)
                throw new Exception("CourrielTokenBody ou CourrielTokenSubject ne peut pas être null");
            if (DynamicConnection.CourrielTokenSubjectRecovery is null || DynamicConnection.CourrielTokenBodyRecovery is null)
                throw new Exception("CourrielTokenSubjectRecovery ou CourrielTokenBodyRecovery ne peut pas être null");
            DynamicConnection.apiKey = apiKey;
            DynamicController.executor = executor;
            RolesAvailable = await executor.SelectDictionary<string, long>(getRoles);
            Dictionary<string, DynamicController> controllers = new Dictionary<string, DynamicController>();
            foreach (var controller in await executor.SelectQuery<DynamicController>(getControllers))
                controllers.Add(controller.Name, await init(controller));
            controllers.Remove("NULL");
            return controllers;
        }
        internal IEnumerable<string> GetIDProprieties()
        {
            return Proprieties.Where(p => p.ShowType.IsID()).Select(p => p.Name);
        }
        public static async Task resetStructureData(bool resetEnum)
        {
            List<KeyValuePair<string, bool>> tablesToDeleteContent = new List<KeyValuePair<string, bool>>()
                .Add("ValidatorProprietyValues", false)
                .Add("ValidatorSQLParamInfoValues", false)
                .Add("FiltersSQLParamsInfo", false)
                .Add("SQLParamInfos", true)
                .Add("Filters", true)
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
            await executor.ExecuteQueryWithTransaction(queriesToRun.ToArray());
            if (resetEnum)
                await EnumHelper.populateBDEnums(executor);
            queriesToRun.Clear();
            queriesToRun.Add("INSERT Controllers (name, isMain) VALUES ('NULL', 0)");
            queriesToRun.Add("INSERT Proprieties (name, isMain, id_ShowType, id_controller, isUpdatable) VALUES ('NULL', 0, 1, 1, 0)");
            await executor.ExecuteQueryWithTransaction(queriesToRun.ToArray());
        }
        public static async Task resetStructureData(SQLExecutor executor, bool resetEnum)
        {
            DynamicController.executor = executor;
            await resetStructureData(resetEnum);
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
        internal long GetProprietyID(string proprietyName)
        {
            if (proprietyName is null)
                return 1;
            var bindedPropriety = Proprieties.FirstOrDefault(propriety => propriety.Name == proprietyName || (propriety.ShowType.IsID() && $"{propriety.Name}New" == proprietyName));
            if (bindedPropriety is not null)
                return bindedPropriety.id;
            return 1;
        }
        public async Task<DynamicController> addRoute(BaseRoutes baseRoute)
        {
            Routes.Add(await DynamicRoute.addRoute(id, baseRoute));
            return this;
        }
        public async Task<DynamicController> addRoute(string Name, RouteTypes routeType, RouteDisplayTypes routeDisplayType = RouteDisplayTypes.NONE, bool getAuthorizedCols = false, bool onlyModify = false, bool requireAuthorization = false)
        {
            Routes.Add(await DynamicRoute.addRoute(id, Name, routeType, routeDisplayType, getAuthorizedCols, onlyModify, requireAuthorization));
            return this;
        }
        public async Task<DynamicController> addEmptyQuery()
        {
            await Routes.Last().addEmptyQuery();
            return this;
        }
        public async Task<DynamicController> addRouteQuery(string routeName, string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            await Routes.First(route => route.Name == routeName).addRouteQuery(queryString, QueryType, CompleteAuth, CompleteCheck);
            return this;
        }
        public async Task<DynamicController> bindParamToUserID(string paramName)
        {
            if (!Routes.Any(route => route.Queries.Any(query => query.ParamsInfos.Any(param => param.Key == paramName))))
                throw new Exception();
            Routes.Last().bindParamToUserID(paramName);
            return this;
        }
        public async Task<DynamicController> addRouteQuery(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            await Routes.Last().addRouteQuery(queryString, QueryType, CompleteAuth, CompleteCheck);
            var lastRouteQuery = Routes.Last().Queries.Last();
            var ind = 0;
            foreach (var paramInfo in lastRouteQuery.ParamsInfos)
            {
                var bindedProprietyId = GetProprietyID(paramInfo.Key);
                if (bindedProprietyId > -1)
                    await lastRouteQuery.setSQLParam(paramInfo.Key, bindedProprietyId, true);
                ind++;
            }
            return this;
        }
        public async Task<DynamicController> addRouteQueryNoMapping(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            await Routes.Last().addRouteQuery(queryString, QueryType, CompleteAuth, CompleteCheck);
            return this;
        }
        public async Task<DynamicController> addRouteQueryNoVar(string queryString, QueryTypes QueryType, bool? CompleteAuth = null, bool CompleteCheck = true)
        {
            await Routes.Last().addRouteQueryNoVar(queryString, QueryType, CompleteAuth, CompleteCheck);
            return this;
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
        public async Task<DynamicController> addSQLParam(string ParamName, params ValidatorBundle[] ValidatorBundles)
        {
            return await addParam(ParamName, false, ValidatorBundles);
        }
        public async Task<DynamicController> addParam(string ParamName, params ValidatorBundle[] ValidatorBundles)
        {
            return await addParam(ParamName, true, ValidatorBundles);
        }
        public async Task<DynamicController> addParam(string ParamName, bool addRequired, params ValidatorBundle[] ValidatorBundles)
        {
            await Routes.Last().addSQLParam(ParamName, 1, addRequired, ValidatorBundles);
            return this;
        }
        public Task<DynamicController> addFilterParamOptional(string varAffected, string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, DisplayName, Description, placeholder, showType, refController, ind, false, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, ShowTypes showType, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, "", "", "", showType, "", true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string placeholder, ShowTypes showType, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, "", "", placeholder, showType, "", true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string Description, string placeholder, ShowTypes showType, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, "", Description, placeholder, showType, "", true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string placeholder, ShowTypes showType, int ind, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, "", "", placeholder, showType, "", ind, true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string Description, string placeholder, ShowTypes showType, string refController, int ind, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, "", Description, placeholder, showType, refController, ind, true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, DisplayName, Description, placeholder, showType, refController, ind, true, ValidatorBundles);
        }
        public Task<DynamicController> addFilterParam(string varAffected, string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, bool addRequired, params ValidatorBundle[] ValidatorBundles)
        {
            return addFilterParam(varAffected, 1, DisplayName, Description, placeholder, showType, refController, ind, addRequired, ValidatorBundles);
        }
        public async Task<DynamicController> addFilterParam(string varAffected, long ProprietyID, string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, bool addRequired, params ValidatorBundle[] ValidatorBundles)
        {
            await Routes.Last().addFilterParam(varAffected, ProprietyID, DisplayName, Description, placeholder, showType, refController, ind, addRequired, ValidatorBundles);
            return this;
        }
        public async Task<DynamicController> addFilterParam(string varAffected, long ProprietyID, string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, bool addRequired, params ValidatorBundle[] ValidatorBundles)
        {
            await Routes.Last().addFilterParam(varAffected, ProprietyID, DisplayName, Description, placeholder, showType, refController, addRequired, ValidatorBundles);
            return this;
        }
        public async Task<DynamicController> addFilter(string DisplayName, string Description, ShowTypes showType, int ind, params string[] SQLVariables)
        {
            await Routes.Last().addFilter(DisplayName, Description, "", showType, "", ind, SQLVariables);
            return this;
        }
        public async Task<DynamicController> addFilter(string DisplayName, string Description, ShowTypes showType, string refController, int ind, params string[] SQLVariables)
        {
            await Routes.Last().addFilter(DisplayName, Description, "", showType, refController, ind, SQLVariables);
            return this;
        }
        public async Task<DynamicController> addFilter(string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, params string[] SQLVariables)
        {
            await Routes.Last().addFilter(DisplayName, Description, placeholder, showType, refController, ind, SQLVariables);
            return this;
        }
        public async Task<DynamicController> setSQLParam(string VarAffected, string ProprietyName, params ValidatorBundle[] ValidatorBundles)
        {
            long proprietyID = 1;
            if (ProprietyName is not null)
                proprietyID = Proprieties.First(propriety => propriety.Name == ProprietyName).id;
            await Routes.Last().setSQLParam(VarAffected, proprietyID, ValidatorBundles);
            return this;
        }
        public async Task<DynamicController> setSQLParam(string VarAffected, params ValidatorBundle[] ValidatorBundles)
        {
            await Routes.Last().setSQLParam(VarAffected, ValidatorBundles);
            return this;
        }

        public async Task<DynamicController> setNotRequired(params string[] VarsAffected)
        {
            await Routes.Last().setNotRequired(VarsAffected);
            return this;
        }
        public async Task<DynamicController> removeParams(params string[] VarsAffected)
        {
            await Routes.Last().removeParams(VarsAffected);
            return this;
        }
        public Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, params ValidatorBundle[] validatorBundles)
        {
            return addPropriety(Name, IsMain, IsUpdatable, showType, "", "", "", Proprieties.Count, validatorBundles);
        }
        public Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, string placeholder, params ValidatorBundle[] validatorBundles)
        {
            return addPropriety(Name, IsMain, IsUpdatable, showType, "", "", placeholder, Proprieties.Count, validatorBundles);
        }
        public Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, string placeholder, int ind, params ValidatorBundle[] validatorBundles)
        {
            return addPropriety(Name, IsMain, IsUpdatable, showType, "", "", placeholder, ind, validatorBundles);
        }
        public Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, string description, string placeholder, int ind, params ValidatorBundle[] validatorBundles)
        {
            return addPropriety(Name, IsMain, IsUpdatable, showType, "", description, placeholder, ind, validatorBundles);
        }
        public async Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, string displayName, string description, string placeholder, int ind, params ValidatorBundle[] validatorBundles)
        {
            Proprieties.Add(await DynamicPropriety.addPropriety(Name, IsMain, IsUpdatable, showType, displayName, description, placeholder, ind, id, validatorBundles));
            return this;
        }
        public async Task<DynamicController> addValidatorForPropriety(string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addValidatorForPropriety(string Value, ValidatorTypes ValidatorType)
        {
            await Proprieties.Last().addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addMapperGenerator(string ProprietyName, string ControllerName, params ParamLinker[] linkers)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addMapperGenerator(ControllerName, linkers);
            return this;
        }
        public async Task<DynamicController> addCBOInfo(string ProprietyName, string ControllerName, string value, params ParamLinker[] linkers)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addCBOInfo(ControllerName, value, linkers);
            return this;
        }
        public async Task<DynamicController> addMapperGenerator(string ControllerName, params ParamLinker[] linkers)
        {
            await Proprieties.Last().addMapperGenerator(ControllerName, linkers);
            return this;
        }
        public async Task<DynamicController> addCBOInfo(string ControllerName, string value, params ParamLinker[] linkers)
        {
            await Proprieties.Last().addCBOInfo(ControllerName, value, linkers);
            return this;
        }
        public async Task<DynamicController> Authorize(string routeName, long RoleID)
        {
            await Routes.First(route => route.Name == routeName).addAuthorizedRole(RoleID);
            return this;
        }
        public async Task<DynamicController> Authorize(params long[] roles)
        {
            await Routes.Last().addAuthorizedRoles(roles);
            return this;
        }
        public async Task<DynamicController> Authorize(string ProprietyName, long RoleID, bool CanModify)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addAuthorizedRole(RoleID, CanModify);
            return this;
        }
        public async Task<DynamicController> Anonymous(string ProprietyName)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).Anonymous();
            return this;
        }
        public async Task<DynamicController> Anonymous()
        {
            await Proprieties.Last().Anonymous();
            return this;
        }
        public async Task<DynamicController> Authorize(params KeyValuePair<long, bool>[] roles)
        {
            await Proprieties.Last().addAuthorizedRoles(roles);
            return this;
        }
        #endregion
        public IEnumerable<DynamicPropriety> getAuthorizedProprieties(bool onlyModify, params long[] roles)
        {
            return getAuthorizedProprieties(onlyModify, (IEnumerable<long>)roles);
        }
        public IEnumerable<string> getCBOKeyValues(IEnumerable<DynamicPropriety> proprieties)
        {
            return proprieties.Where(prop => prop.ShowType.IsCBO()).Select(prop => prop.MapperGenerator.parametersToLink[SQLExecutor.VALUE_FOR_CBO]);
        }
        public IEnumerable<DynamicPropriety> getAuthorizedProprieties(bool onlyModify, IEnumerable<long> roles)
        {
            if (onlyModify)
                return Proprieties.Where(propriety => propriety.CanModify(roles));
            return Proprieties.Where(propriety => propriety.CanSee(roles));
        }
        public IEnumerable<DynamicRoute> getAuthorizedRoutes(IEnumerable<long> roles)
        {
            return Routes.Where(route => route.CanUse(roles));
        }
        public object InfoObject()
        {
            return new { id = this.id, name = this.Name, isMain = this.IsMain };
        }
        public ParamAffectedResume[] InfoParamAffectedPropriety(DynamicPropriety propriety)
        {

            var paramAffecteds = new ParamAffectedResume[] { new ParamAffectedResume(propriety.Name, false, propriety.Validators.Select(validator => new ValidatorResume(validator.Value, (long)validator.ValidatorType, validator.Message)).ToArray()) };
            if (propriety.ShowType.IsCBO())
                paramAffecteds = paramAffecteds.Append(new ParamAffectedResume(propriety.MapperGenerator.parametersToLink[SQLExecutor.VALUE_FOR_CBO].ToString(), false)).ToArray();
            return paramAffecteds;
        }
        public IEnumerable<ParamInfoResume> InfoObjectPropreties(IEnumerable<DynamicPropriety> proprieties)
        {
            return proprieties.Select(prop => new ParamInfoResume(prop.displayName, prop.IsMain, prop.description, prop.placeholder, (long)prop.ShowType, prop.MapperGenerator?.toResume(), prop.ind, InfoParamAffectedPropriety(prop)));
        }
        public IEnumerable<object> InfoObjectRoutes(IEnumerable<DynamicRoute> routes)
        {
            return routes.Select(route => new {name = route.Name, routeDisplay = (long)route.routeDisplayType});
        }
        public IEnumerable<object> InfoObjectFiltres(IEnumerable<DynamicFilter> filters)
        {
            return filters.Select(filter => new ParamInfoResume(filter.Name, true, filter.Description, filter.Placeholder, (long)filter.ShowType, new MapperResume(filter.RefController, null, null), filter.ind, filter.AffectedVars.Select(affected => new ParamAffectedResume(affected.VarAffected, affected.isRequired, affected.Validators.Select(validator => new ValidatorResume(validator.Value, (long)validator.ValidatorType, validator.Message)).ToArray())).ToArray()));
        }
        public IEnumerable<ParamInfoResume> InfoObjectSQLParam(DynamicRoute route)
        {
            foreach (var paramInfo in route.Queries.SelectMany(query => query.ParamsInfos.Values))
            {
                var paramAffected = new ParamAffectedResume(paramInfo.VarAffected, paramInfo.isRequired, paramInfo.Validators.Select(validator => new ValidatorResume(validator.ValidatorType == ValidatorTypes.REGEX? validator.Value.ToString() : validator.Value, (long)validator.ValidatorType, validator.Message)).ToArray());
                if (paramInfo.ProprietyID != 1)
                {
                    var propriety = Proprieties.First(prop => prop.id == paramInfo.ProprietyID);
                    yield return new ParamInfoResume(propriety.displayName, propriety.IsMain, propriety.description, propriety.placeholder, (long)propriety.ShowType, propriety.MapperGenerator?.toResume(), propriety.ind, paramAffected);
                }
                else if (paramInfo.VarAffected == Query.ORDERBYVARKEY)
                {
                    continue;
                }
                else
                {
                    var filter = route.Filters.First(filter => filter.AffectedVars.Any(affectedVar => affectedVar.VarAffected == paramInfo.VarAffected));
                    yield return new ParamInfoResume(filter.Name, true, filter.Description, filter.Placeholder, (long)filter.ShowType, new MapperResume(filter.RefController, null, null), filter.ind, paramAffected);
                }
            }
        }
        public bool CanUse(IEnumerable<long> roles)
        {
            return Roles.Contains(0) || Roles.Any(role => roles.Contains(role));
        }
        public static void initRoutesControllersInfo(WebApplication app, Dictionary<string, DynamicController> controllers)
        {
            DynamicController.app = app;
            app.MapGet("RefreshToken", ([FromHeader(Name = "Authorization")] string? JWT) =>
            {
                var result = DynamicConnection.RefreshToken(JWT);
                if (result is null)
                    return Results.Forbid();
                return Results.Ok(result);
            });
            app.MapGet("Info/Controllers", ([FromHeader(Name = "Authorization")] string? JWT) =>
            {
                var roles = getRolesInfo(JWT);
                if (!roles.Any())
                    return Results.Forbid();
                return Results.Ok(controllers.Where(controller => controller.Value.CanUse(roles)).Select(controller => controller.Value.InfoObject()));
            });
            foreach (var controller in controllers.Where(controller => controller.Value.hasRoute(BaseRoutes.GETALL.Value())))
                controller.Value.setBaseInfoRoutes();
            app.MapPost("AddRoleToUser", async ([FromHeader(Name = "Authorization")] string? JWT, RequestForRoleID requestForRole) =>
            {
                try
                {
                    var roles = getRolesInfo(JWT);
                    if (!roles.Any() || !roles.Contains(2))
                        return Results.Forbid();

                    if (!(await DynamicConnection.addRoleToUser(requestForRole.UserID, requestForRole.RolesIDS)))
                        return Results.Problem();
                    return Results.Ok();
                }
                catch
                {
                    return Results.Forbid();
                }
            });
        }
        public static void addPolicies(Dictionary<string, DynamicController> controllers, AuthorizationOptions options)
        {
            foreach (var controller in controllers)
                foreach (var route in controller.Value.Routes)
                {
                    options.AddPolicy($"{controller.Key}_{route.Name}_Policy", policy =>
                    {
                        policy.Requirements.Add(new HasRoleRequirement(route.Roles));
                    });
                }
        }
        internal static IEnumerable<long> getRolesInfo(string JWT)
        {
            var claim = DynamicConnection.ParseClaim(JWT);
            if (claim is null || claim.ValidTo < DateTime.UtcNow)
                return new long[0];
            return DynamicConnection.ParseRoles(claim);
        }
        internal void setBaseInfoRoutes()
        {

            var getAllRoute = getRoute(BaseRoutes.GETALL.Value());
            var infoObjectFilters = InfoObjectFiltres(getAllRoute.Filters).ToArray();
            app.MapGet($"/{Name}/Info/Filters", () =>
            {
                return Results.Ok(infoObjectFilters);
            }).WithName($"{Name}InfoFilters");
            app.MapGet($"/{Name}/Info/Proprieties", ([FromHeader(Name = "Authorization")] string? JWT) =>
            {
                var roles = getRolesInfo(JWT);
                if (!roles.Any())
                    return Results.Forbid();
                return Results.Ok(InfoObjectPropreties(getAuthorizedProprieties(false, roles)));
            }).WithName($"{Name}InfoProprieties");

            var infoRoutesObjectSQLParam = Routes.Select(route => new { route = new RouteResume(route.Name, route.routeDisplayType, route.RouteType, route.requireAuthorization, route.Roles), paramsInfo = InfoObjectSQLParam(route) });
            app.MapGet($"/{Name}/Info/Routes", ([FromHeader(Name = "Authorization")] string? JWT) =>
            {
                var roles = getRolesInfo(JWT);
                if (!roles.Any())
                    return Results.Forbid();
                //return Results.Ok(getAuthorizedRoutes(roles));
                return Results.Ok(infoRoutesObjectSQLParam.Where(info => info.route.CanUse(roles)));
            }).WithName($"{Name}InfoRoutes");
            foreach (var route in Routes)
            {
                var infoObjectSQLParam = InfoObjectSQLParam(route).ToArray();
                app.MapGet($"/{Name}/InfoRoute/{route.Name}", ([FromHeader(Name = "Authorization")] string? JWT) =>
                {
                    var roles = getRolesInfo(JWT);
                    if (!roles.Any() || !route.CanUse(roles))
                        return Results.Forbid();
                    return Results.Ok(infoObjectSQLParam);
                }).WithName($"{Name}Info{route.Name}Variables");
            }
        }
        public long getProperityID(string ProprietyName)
        {
            return Proprieties.First(propriety => propriety.Name == ProprietyName).id;
        }
        public void mapRoute(BaseRoutes routeName, Func<List<Query>, Dictionary<string, object>, Task<IResult>> function)
        {
            mapRoute(routeName.Value(), function);
        }

        public void mapRoute(string routeName, Func<List<Query>, Dictionary<string, object>, Task<IResult>> function)
        {
            DynamicRoute route = getRoute(routeName);
            var idsParams = Proprieties.Where(propriety => propriety.ShowType.IsID()).Select(propriety => propriety.Name).ToArray();
            if (route is null)
                throw new Exception();
            List<Query> queries = route.Queries.Select(dynamicQuery => dynamicQuery.query).ToList();
            Func<HttpRequest, Task<IResult>> delegateMethod = async (request) =>
            {
                var bodyData = JObjectToDictionary(await StreamToJObject(request.Body));
                QueryCollectionToDictionary(request.Query, bodyData);
                StringValues JWT;
                request.Headers.TryGetValue("Authorization", out JWT);
                if (!fillBodyData(bodyData, DynamicConnection.ParseClaim(JWT.Any() ? JWT[0] : null), route))
                    return Results.Forbid();
                if (!route.validateParams(bodyData))
                    return Results.Forbid();
                if (route.onlyModify)
                    bodyData[IDParamsKey] = idsParams;
                return await function(queries, bodyData);
            };
            var routeBuilder = app.MapRoute(route.RouteType, $"/{Name}/{routeName}", delegateMethod).WithName($"{Name}{routeName}").WithGroupName(Name);
            /*
            if (requireAuthorization)
                routeBuilder.RequireAuthorization($"{Name}_{route.Name}_Policy");*/
        }
        internal async Task<string> StreamToJObject(Stream stream)
        {
            if (stream is null)
                return string.Empty; 
            using (var reader = new StreamReader(stream,
                      encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
                return await reader.ReadToEndAsync();
        }
        internal bool fillBodyData(Dictionary<string, object> bodyData, JwtSecurityToken token, DynamicRoute route)
        {
            IEnumerable<DynamicPropriety> authorizedProprieties;
            if (token is null || token.ValidTo < DateTime.UtcNow)
            {
                if (route.requireAuthorization)
                    return false;
                setAuthorizedProprieties(bodyData, route.onlyModify);
                return true;
            }
            var roles = DynamicConnection.ParseRoles(token).ToArray();
            var userID = DynamicConnection.ParseUserID(token);
            bodyData[USERIDKEY] = userID;
            bodyData[ROLESKEY] = roles;
            if (route.paramForUserID is not null && roles.Length <= 1 && roles[0] == 1)
                bodyData[route.paramForUserID] = userID;
            if (route.getAuthorizedCols)
                setAuthorizedProprieties(bodyData, route.onlyModify, roles);
            if (route.requireAuthorization)
                return route.CanUse(roles);
            return true;
        }
        public void setAuthorizedProprieties(Dictionary<string, object> bodyData, bool forModify, params long[] roles)
        {
            IEnumerable<DynamicPropriety> authorizedProprieties;
            if (roles.Any())
                authorizedProprieties = getAuthorizedProprieties(forModify, roles);
            else
                authorizedProprieties = getAuthorizedProprieties(forModify);
            bodyData[PROPRETYKEY] = authorizedProprieties.Select(prop => forModify && prop.ShowType.IsID() ? $"{prop.Name}New" : prop.Name).Concat(getCBOKeyValues(authorizedProprieties)).ToArray();
        }
        public static void QueryCollectionToDictionary(IQueryCollection queryParameters, Dictionary<string, object> dictionary)
        {
            foreach (var (key, values) in queryParameters)
                if (values.Count == 1)
                {
                    if (int.TryParse(values[0], out var intValue))
                        dictionary[key] = intValue;
                    else if (bool.TryParse(values[0], out var boolValue))
                        dictionary[key] = boolValue;
                    else if (DateTime.TryParse(values[0], out var dateTimeValue))
                        dictionary[key] = dateTimeValue;
                    else
                        dictionary[key] = values[0];
                }
                else
                    dictionary[key] = values.ToArray();
        }
        public static Dictionary<string, object> JObjectToDictionary(string json)
        {
            if (json.IsNullOrEmpty())
                return new Dictionary<string, object>();
            return JObjectToDictionary(JObject.Parse(json));
        }
        public static Dictionary<string, object> JObjectToDictionary(JObject jObject)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (jObject is null)
                return dictionary;
            foreach (var property in jObject.Properties())
                dictionary[property.Name] = dispatchJItem(property.Value);
            return dictionary;
        }
        public static IEnumerable<object> JArrayToList(JArray jArray)
        {
            List<object> list = new List<object>();
            if (jArray is null)
                return list;
            foreach (var item in jArray)
                list.Add(dispatchJItem(item));
            return list;
        }
        public static object dispatchJItem(JToken item)
        {
            switch (item.Type)
            {
                case JTokenType.Object:
                    return JObjectToDictionary((JObject)item);
                case JTokenType.Array:
                    return JArrayToList((JArray)item);
                default:
                    return ((JValue)item).Value;
            }
        }

        public static bool Cross(IEnumerable<long> arr1, IEnumerable<long> arr2)
        {
            return arr1.Any(item => arr2.Contains(item));
        }

        public static void MakeBaseRoutesDefinition(Dictionary<string, DynamicController> controllers, SQLExecutor executorData)
        {
            foreach (var controller in controllers)
            {
                if (controller.Value.hasRoute(BaseRoutes.GETALL.Value()))
                    controller.Value.mapRoute(BaseRoutes.GETALL, async (queries, bodyData) =>
                    {
                        if (queries[0].setParams(bodyData).containOrderVar)
                            return Results.Ok(await executorData.SelectQuery(queries[0],
                                bodyData.SafeGet<int>(PAGEKEY, DEFAULTPAGE),
                                bodyData.SafeGet<int>(STEPKEY, DEFAULTSTEP),
                                bodyData.SafeGet<string>(Query.ORDERBYVARKEY, null),
                                bodyData.SafeGet<bool>(ISASCENDINGKEY, true),
                                bodyData.AuthProprieties()));
                        return Results.Ok(await executorData.SelectQuery(queries[0], bodyData.AuthProprieties()));
                    });//, true, true, false
                if (controller.Value.hasRoute(BaseRoutes.CBO.Value()))
                    controller.Value.mapRoute(BaseRoutes.CBO, async (queries, bodyData) =>
                    {
                        return Results.Ok(await executorData.SelectDictionary(queries[0].setParams(bodyData)));
                    });
                if (controller.Value.hasRoute(BaseRoutes.GET.Value()))
                    controller.Value.mapRoute(BaseRoutes.GET, async (queries, bodyData) =>
                    {
                        return Results.Ok(await executorData.SelectSingle(queries[0].setParams(bodyData), bodyData.AuthProprieties()));
                    });//, true, true, false
                if (controller.Value.hasRoute(BaseRoutes.INSERT.Value()))
                    controller.Value.mapRoute(BaseRoutes.INSERT, async (queries, bodyData) =>
                    {
                        foreach (var query in queries)
                            query.setParams(bodyData);
                        int nbAffected = await executorData.ExecuteQueryWithTransaction(queries.ToArray());
                        if (nbAffected <= 0)
                            return Results.Problem();
                        return Results.Ok(nbAffected);
                    });//, true, true, false
                if (controller.Value.hasRoute(BaseRoutes.UPDATE.Value()))
                    controller.Value.mapRoute(BaseRoutes.UPDATE, async (queries, bodyData) =>
                    {

                        var authorizedCols = bodyData.AuthProprieties().Concat((string[])bodyData[IDParamsKey]);
                        var authorizedVariables = bodyData
                            .Where(kv => authorizedCols.Contains(kv.Key));
                        foreach (var query in queries)
                            query.setParams(authorizedVariables);
                        int nbAffected = await executorData.ExecuteQueryWithTransaction(queries.ToArray());
                        if (nbAffected <= 0)
                            return Results.Problem();
                        return Results.Ok(nbAffected);
                    });//, true, true, true
                if (controller.Value.hasRoute(BaseRoutes.GETALLDETAILED.Value()))
                    controller.Value.mapRoute(BaseRoutes.GETALLDETAILED, async (queries, bodyData) =>
                    {
                        var authorizedProprieties = bodyData.AuthProprieties();
                        var mappers = controller.Value.getMappersGenerated(controllers, bodyData.UserRoles(), authorizedProprieties);
                        if (queries[0].setParams(bodyData).containOrderVar)
                            return Results.Ok(await executorData.DetailedSelectQuery(queries[0],
                                bodyData.SafeGet<int>(PAGEKEY, DEFAULTPAGE),
                                bodyData.SafeGet<int>(STEPKEY, DEFAULTSTEP),
                                bodyData.SafeGet<string>(Query.ORDERBYVARKEY, null),
                                bodyData.SafeGet<bool>(ISASCENDINGKEY, true),
                                mappers,
                                authorizedProprieties));
                        return Results.Ok(await executorData.DetailedSelectQuery(queries[0], mappers, authorizedProprieties));
                    });//, true, true, false
                if (controller.Value.hasRoute(BaseRoutes.GETDETAILED.Value()))
                    controller.Value.mapRoute(BaseRoutes.GETDETAILED, async (queries, bodyData) =>
                    {
                        var authorizedProprieties = bodyData.AuthProprieties();
                        var mappers = controller.Value.getMappersGenerated(controllers, bodyData.UserRoles(), authorizedProprieties);
                        return Results.Ok(await executorData.DetailedSelectQuerySingle(queries[0].setParams(bodyData), mappers, authorizedProprieties));
                    });//, true, true, false
            }
        }
        public IEnumerable<DynamicMapper> getMappersGenerated(Dictionary<string, DynamicController> controllers, IEnumerable<long> roles, params string[] authorizedColumns)
        {
            return Proprieties.Where(propriety => authorizedColumns.Contains(propriety.Name) && propriety.ShowType.IsRef()).Select(propriety => propriety.MapperGenerator.updateMapper(controllers[propriety.MapperGenerator.controllerName].getAuthorizedProprieties(false, roles).Select(prop => prop.Name).ToArray()));
        }
        internal bool hasRoute(string routeName)
        {
            return Routes.Any(route => route.Name == routeName);
        }
        internal DynamicRoute getRoute(string routeName)
        {
            return Routes.FirstOrDefault(route => route.Name == routeName);
        }
        public async static Task InsertEnum(SQLExecutor executor, string insertTemplate, string tableName, Type classType)
        {
            List<string> queries = new List<string>() { $"DELETE {tableName}", $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)", $"SET IDENTITY_INSERT {tableName} ON" };
            foreach (Enum entry in Enum.GetValues(classType))
                queries.Add(string.Format(insertTemplate, entry.ID(), entry.Value(classType)));
            queries.Add($"SET IDENTITY_INSERT {tableName} OFF");
            await executor.ExecuteQueryWithTransaction(queries.ToArray());
        }
        public static Task InsertEnum<T>(SQLExecutor executor, string insertTemplate, string tableName) where T : Enum
        {
            return InsertEnum(executor, insertTemplate, tableName, typeof(T));
        }


        /*
        public static async Task<JObject> ParseBody(HttpRequest request)
        {
            using (var requestBodyStream = new StreamReader(request.Body))
            {
                string requestBody = await requestBodyStream.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(requestBody))
                    return JObject.Parse(requestBody);
                return new JObject();
            }
        }
        */
    }
}
