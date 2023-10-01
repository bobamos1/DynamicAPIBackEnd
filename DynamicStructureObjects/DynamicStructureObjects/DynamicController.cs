using DynamicSQLFetcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
//using System.Text.Json;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections;
using System.Runtime.Serialization;
using ParserLib;

namespace DynamicStructureObjects
{
    public class DynamicController
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsMain { get; internal set; }
        internal static SQLExecutor executor { get; set; }
        internal static WebApplication app { get; set; }
        internal static Dictionary<string, long> RolesAvailable { get; set; }
        public List<DynamicRoute> Routes { get; internal set; }
        public List<DynamicPropriety> Proprieties { get; internal set; }
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT name AS Name, id AS id FROM Roles");
        internal static readonly Query getControllers = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, name AS Name, isMain AS IsMain FROM Controllers", true, true);
        internal static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, isReadOnly AS ReadOnly, id_ShowType AS ShowTypeID FROM Proprieties WHERE id_controller = @controllerID", true, true);
        internal static readonly Query getRoutes = Query.fromQueryString(QueryTypes.SELECT, "SELECT URLRoutes.id AS id, COALESCE(BaseRoutes.name, URLRoutes.name) AS Name, id_routeType AS RouteTypeID FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute WHERE URLRoutes.id_controller = @controllerID", true, true);
        internal static readonly Query insertController = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Controllers (name, isMain) VALUES (@Name, @IsMain)", true, true);
        private DynamicController(long id, string Name, bool IsMain)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.Routes = new List<DynamicRoute>();
            this.Proprieties = new List<DynamicPropriety>();
        }
        public Query GetGetAllRoute()
        {
            return Routes.FirstOrDefault(route => route.Name == BaseRoutes.GETALL.Value())?.Queries.First().query;
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
        public static async Task<Dictionary<string, DynamicController>> initControllers(SQLExecutor executor, string apiKey)
        {
            DynamicConnection.apiKey = apiKey;
            DynamicController.executor = executor;
            RolesAvailable = await executor.SelectDictionary<string, long>(getRoles);
            Dictionary<string, DynamicController> controllers = new Dictionary<string, DynamicController>();
            foreach (var controller in await executor.SelectQuery<DynamicController>(getControllers))
                controllers.Add(controller.Name, await init(controller));
            controllers.Remove("NULL");
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
        public static async Task resetStructureData(SQLExecutor executor)
        {
            DynamicController.executor = executor;
            await resetStructureData();
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
        public async Task<DynamicController> addRoute(string Name, RouteTypes routeType)
        {
            Routes.Add(await DynamicRoute.addRoute(id, Name, routeType));
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
        public async Task<DynamicController> addCBOInfo(string ProprietyName, string ControllerName, string key, params ParamLinker[] linkers)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addCBOInfo(ControllerName, key, linkers);
            return this;
        }
        public async Task<DynamicController> addMapperGenerator(string ControllerName, params ParamLinker[] linkers)
        {
            await Proprieties.Last().addMapperGenerator(ControllerName, linkers);
            return this;
        }
        public async Task<DynamicController> addCBOInfo(string ControllerName, string key, params ParamLinker[] linkers)
        {
            await Proprieties.Last().addCBOInfo(ControllerName, key, linkers);
            return this;
        }
        public async Task<DynamicController> addAuthorizedRouteRole(string routeName, long RoleID)
        {
            await Routes.First(route => route.Name == routeName).addAuthorizedRole(RoleID);
            return this;
        }
        public async Task<DynamicController> addAuthorizedRouteRoles(params long[] roles)
        {
            DynamicRoute route = Routes.Last();
            foreach (var role in roles)
                await route.addAuthorizedRole(role);
            return this;
        }
        public async Task<DynamicController> addAuthorizedProprietyRole(string ProprietyName, long RoleID, bool CanModify)
        {
            await Proprieties.First(propriety => propriety.Name == ProprietyName).addAuthorizedRole(RoleID, CanModify);
            return this;
        }
        public async Task<DynamicController> addAuthorizedProprietyRoles(params KeyValuePair<long, bool>[] roles)
        {
            DynamicPropriety propriety = Proprieties.Last();
            foreach (var role in roles)
                await propriety.addAuthorizedRole(role.Key, role.Value);
            return this;
        }
        #endregion
        public IEnumerable<DynamicPropriety> getAuthorizedProprieties(bool onlyModify, params long[] roles)
        {
            return getAuthorizedProprieties(onlyModify, (IEnumerable<long>)roles);
        }
        public IEnumerable<string> getCBOKeyValues(IEnumerable<DynamicPropriety> proprieties)
        {
            return proprieties.Where(prop => prop.ShowType == ShowTypes.CBO).Select(prop => prop.MapperGenerator.parametersToLink[SQLExecutor.KEY_FOR_CBO]);
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
        public static void initRoutesControllersInfo(WebApplication app, Dictionary<string, DynamicController> controllers)
        {
            DynamicController.app = app;
            foreach (var controller in controllers)
                controller.Value.setBaseInfoRoutes();
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
        internal void setBaseInfoRoutes()
        {
            app.MapGet($"/{Name}/Info/Propriety", ([FromHeader(Name = "Authorization")] string JWT) =>
            {
                return Task.FromResult<IResult>(Results.Ok(getAuthorizedProprieties(false, DynamicConnection.ParseRoles(DynamicConnection.ParseClaim(JWT)))));
            }).WithName($"{Name}InfoPropriety");
            app.MapGet($"/{Name}/Info/Routes", ([FromHeader(Name = "Authorization")] string JWT) =>
            {
                return Task.FromResult<IResult>(Results.Ok(getAuthorizedRoutes(DynamicConnection.ParseRoles(DynamicConnection.ParseClaim(JWT)))));
            }).WithName($"{Name}InfoRoutes");
            foreach (var route in Routes)
            {
                app.MapGet($"/{Name}/Info/RouteFilters/{route.Name}", ([FromHeader(Name = "Authorization")] string JWT) =>
                {
                    if (!route.CanUse(DynamicConnection.ParseRoles(DynamicConnection.ParseClaim(JWT))))
                        return Task.FromResult<IResult>(Results.Forbid());
                    List<DynamicFilter> filters = new List<DynamicFilter>();
                    foreach (var query in route.Queries)
                        filters.AddRange(query.Filters);
                    return Task.FromResult<IResult>(Results.Ok(filters));
                }).WithName($"{Name}Info{route.Name}Filters");
                app.MapGet($"/{Name}/Info/RouteVariables/{route.Name}", ([FromHeader(Name = "Authorization")] string JWT) =>
                {
                    if (!route.CanUse(DynamicConnection.ParseRoles(DynamicConnection.ParseClaim(JWT))))
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
        public void addRouteAPI(BaseRoutes routeName, Func<List<Query>, Dictionary<string, object>, Task<IResult>> function, bool requireAuthorization = true, bool getAuthorizedCols = false, bool onlyModify = false, bool getRoles = false)
        {
            addRouteAPI(routeName.Value(), function, requireAuthorization, getAuthorizedCols, onlyModify, getRoles);
        }

        public void addRouteAPI(string routeName, Func<List<Query>, Dictionary<string, object>, Task<IResult>> function, bool requireAuthorization = true, bool getAuthorizedCols = false, bool onlyModify = false, bool getRoles = false)
        {
            DynamicRoute route = getRoute(routeName);
            if (route is null)
                throw new Exception();
            List<Query> queries = route.Queries.Select(dynamicQuery => dynamicQuery.query).ToList();
            Func<dynamic, string, Task<IResult>> delegateMethod = async ([FromBody] dynamic request, [FromHeader(Name = "Authorization")] string JWT) =>
            {
                Dictionary<string, object> bodyData = JObjectToDictionary(JObject.Parse(request.ToString()));
                if (requireAuthorization)
                {
                    var jsonToken = DynamicConnection.ParseClaim(JWT);
                    var roles = DynamicConnection.ParseRoles(jsonToken);
                    if (jsonToken is null || !route.CanUse(roles) && false)
                        return Results.Forbid();
                    if (getAuthorizedCols)
                    {
                        var authorizedProprieties = getAuthorizedProprieties(onlyModify, roles);
                        bodyData["AuthorizedProprieties"] = authorizedProprieties.Select(prop => prop.Name).Concat(getCBOKeyValues(authorizedProprieties));
                    }
                    bodyData["CurrentUserID"] = DynamicConnection.ParseUserID(jsonToken);
                    if (bodyData["CurrentUserID"].Equals(-1))
                        return Results.Forbid();
                    if (getRoles)
                        bodyData["CurrentUserRoles"] = roles;
                }
                if (!route.validateParams(bodyData))
                    return Results.Forbid();
                return await function(queries, bodyData);
            };
            var routeBuilder = app.MapRoute(route.RouteType, $"/{Name}/{routeName}", delegateMethod).WithName($"{Name}{routeName}").WithGroupName(Name);
            /*
            if (requireAuthorization)
                routeBuilder.RequireAuthorization($"{Name}_{route.Name}_Policy");*/
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
                    controller.Value.addRouteAPI(BaseRoutes.GETALL, async (queries, bodyData) =>
                    {
                        queries[1].setParams(bodyData);
                        return Results.Ok(await executorData.SelectQuery(queries[1], (string[])bodyData["AuthorizedProprieties"]));
                    }, true, true, false);
                if (controller.Value.hasRoute(BaseRoutes.CBO.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.CBO, async (queries, bodyData) =>
                    {
                        queries[1].setParams(bodyData);
                        return Results.Ok(await executorData.SelectDictionary(queries[1]));
                    });
                if (controller.Value.hasRoute(BaseRoutes.GET.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.GET, async (queries, bodyData) =>
                    {
                        queries[1].setParams(bodyData);
                        return Results.Ok(await executorData.SelectSingle(queries[1], (string[])bodyData["AuthorizedProprieties"]));
                    }, true, true, false);
                if (controller.Value.hasRoute(BaseRoutes.INSERT.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.INSERT, async (queries, bodyData) =>
                    {
                        foreach (var query in queries)
                            query.setParams(bodyData);
                        int nbAffected = await executorData.ExecuteQueryWithTransaction(queries.ToArray());
                        if (nbAffected > 0)
                            return Results.Ok(nbAffected);
                        return Results.Problem();
                    }, true, true, false);
                if (controller.Value.hasRoute(BaseRoutes.UPDATE.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.UPDATE, async (queries, bodyData) =>
                    {
                        foreach (var query in queries)
                            query.setParams(bodyData);
                        int nbAffected = await executorData.ExecuteQueryWithTransaction((string[])bodyData["AuthorizedProprieties"], queries.ToArray());
                        if (nbAffected > 0)
                            return Results.Ok(nbAffected);
                        return Results.Problem();
                    }, true, true, true);
                if (controller.Value.hasRoute(BaseRoutes.GETALLDETAILED.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.GETALLDETAILED, async (queries, bodyData) =>
                    {
                        string[] authorizedProprieties = (string[])bodyData["AuthorizedProprieties"];
                        List<DynamicMapper> mappers = controller.Value.getMappersGenerated(controllers, (IEnumerable<long>)bodyData["AuthorizedProprieties"], authorizedProprieties);
                        return Results.Ok(await executorData.DetailedSelectQuery(queries[1], mappers, authorizedProprieties));
                    }, true, true, false, true);
                if (controller.Value.hasRoute(BaseRoutes.GETDETAILED.Value()))
                    controller.Value.addRouteAPI(BaseRoutes.GETDETAILED, async (queries, bodyData) =>
                    {
                        string[] authorizedProprieties = (string[])bodyData["AuthorizedProprieties"];
                        List<DynamicMapper> mappers = controller.Value.getMappersGenerated(controllers, (IEnumerable<long>)bodyData["AuthorizedProprieties"], authorizedProprieties);
                        return Results.Ok(await executorData.DetailedSelectQuerySingle(queries[1], mappers, authorizedProprieties));
                    }, true, true, false, true);
            }
        }
        public List<DynamicMapper> getMappersGenerated(Dictionary<string, DynamicController> controllers, IEnumerable<long> roles, params string[] authorizedColumns)
        {
            return Proprieties.Where(propriety => authorizedColumns.Contains(propriety.Name) && propriety.MapperGenerator is not null).Select(propriety => propriety.MapperGenerator.updateMapper(controllers[propriety.MapperGenerator.controllerName].getAuthorizedProprieties(false, roles).Select(prop => prop.Name).ToArray())).ToList();
        }
        internal bool hasRoute(string routeName)
        {
            return Routes.Any(route => route.Name == routeName);
        }
        internal DynamicRoute getRoute(string routeName)
        {
            return Routes.FirstOrDefault(route => route.Name == routeName);
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
