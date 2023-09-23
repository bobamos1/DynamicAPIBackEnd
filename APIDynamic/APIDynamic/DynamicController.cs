﻿using DynamicSQLFetcher;
using Microsoft.AspNetCore.Routing;
using ParserLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace APIDynamic
{
    public class DynamicController
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        internal static SQLExecutor executor { get; set; }
        internal static WebApplication app { get; set; }
        public static readonly Query getControllers = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, name AS Name, isMain AS IsMain FROM Controllers");
        public static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, isReadOnly AS ReadOnly, id_ShowType AS ShowTypeID FROM Proprieties WHERE id_controller = @controllerID");
        public static readonly Query getRoutes = Query.fromQueryString(QueryTypes.SELECT, "SELECT URLRoutes.id AS id, COALESCE(BaseRoutes.name, URLRoutes.name) AS Name FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute WHERE URLRoutes.id_controller = @controllerID");
        public static readonly Query insertController = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Controllers (name, isMain) VALUES (@Name, @IsMain)");
        public List<DynamicRoute> routes { get; set; }
        public List<DynamicPropriety> proprieties { get; set; }
        private DynamicController(long id, string Name, bool IsMain)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
        }
        private static async Task<DynamicController> init(DynamicController controller)
        {
            
            controller.routes = (await executor.SelectQueryTotal<DynamicRoute>(getRoutes.setParam("controllerID", controller.id))).ToList();
            controller.proprieties = (await executor.SelectQueryTotal<DynamicPropriety>(getProprieties.setParam("controllerID", controller.id))).ToList();
            foreach (var route in controller.routes)
                await DynamicRoute.init(route);
            foreach (var propriety in controller.proprieties)
                await DynamicPropriety.init(propriety);
            return controller;
        }
        public static async Task<Dictionary<string, DynamicController>> initControllers(SQLExecutor executor, WebApplication app)
        {
            DynamicController.executor = executor;
            DynamicController.app = app;
            Dictionary<string, DynamicController> controllers = new Dictionary<string, DynamicController>();
            foreach (var controller in await executor.SelectQueryTotal<DynamicController>(getControllers))
                controllers.Add(controller.Name, await init(controller));
            return controllers;
        }
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
            routes.Add(await DynamicRoute.addRoute(id, baseRoute));
            return this;
        }
        public async Task<DynamicController> addRoute(string Name)
        {
            routes.Add(await DynamicRoute.addRoute(id, Name));
            return this;
        }
        public async Task<DynamicController> addRouteQuery(string routeName, string queryString, QueryTypes IDQueryType, bool CompleteCheck)
        {
            await routes.First(route => route.Name == routeName).addRouteQuery(queryString, IDQueryType, CompleteCheck);
            return this;
        }
        public async Task<DynamicController> addSQLParamInfo(string routeName, int index, string varAffected, long ProprietyID)
        {
            await routes.First(route => route.Name == routeName).addSQLParamInfo(index, varAffected, ProprietyID);
            return this;
        }
        public async Task<DynamicController> addValidatorForSQLParam(string routeName, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await routes.First(route => route.Name == routeName).addValidator(indexQuery, VarAffected, Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addFilter(string routeName, int index, string name, ShowTypes showType, string VarAffected)
        {
            await routes.First(route => route.Name == routeName).addFilter(index, name, showType, VarAffected);
            return this;
        }
        public async Task<DynamicController> addPropriety(string Name, bool IsMain, bool IsReadOnly, ShowTypes showType)
        {
            proprieties.Add(await DynamicPropriety.addPropriety(Name, IsMain, IsReadOnly, showType, id));
            return this;
        }
        public async Task<DynamicController> addValidatorForPropriety(string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            await proprieties.First(propriety => propriety.Name == ProprietyName).addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicController> addMapperGenerator(string ProprietyName, string ControllerName, params ParamLinker[] linkers)
        {
            await proprieties.First(propriety => propriety.Name == ProprietyName).addMapperGenerator(ControllerName, linkers);
            return this;
        }
    }
}
