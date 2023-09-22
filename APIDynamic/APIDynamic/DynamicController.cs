using DynamicSQLFetcher;
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
        public static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, id_ShowType AS IDShowType, isReadOnly AS ReadOnly, ShowTypes.name AS ShowTypeName FROM Proprieties INNER JOIN ShowTypes ON ShowTypes.id = id_ShowType WHERE id_controller = @controllerID");
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
        public async static Task addController(Dictionary<string, DynamicController> controllers, string Name, bool IsMain)
        {
            controllers.Add(Name, new DynamicController(await executor.ExecuteInsertWithLastID(insertController.setParam("Name", Name).setParam("IsMain", IsMain)), Name, IsMain));
        }
        public Task addRoute(long baseRoute)
        {
            return DynamicRoute.addRoute(routes, id, baseRoute);
        }
        public Task addRoute(string Name)
        {
            return DynamicRoute.addRoute(routes, id, Name);
        }
        public Task addRouteQuery(string routeName, string queryString, QueryTypes IDQueryType, bool CompleteCheck)
        {
            return routes.First(route => route.Name == routeName).addRouteQuery(queryString, IDQueryType, CompleteCheck);
        }
        public Task addFilters(string routeName, int index, string name, long ShowTypeID, string VarAffected)
        {
            return routes.First(route => route.Name == routeName).queries[index].addFilters(name, ShowTypeID, VarAffected);
        }
    }
}
