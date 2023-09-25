using DynamicSQLFetcher;
using ParserLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace APIDynamic
{
    public class DynamicController
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        internal static SQLExecutor executor { get; set; }
        public static Dictionary<string, long> rolesAvailable { get; set; }
        internal static WebApplication app { get; set; }
        public static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT name, id FROM Roles", true, true);
        public static readonly Query getControllers = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, name AS Name, isMain AS IsMain FROM Controllers", true, true);
        public static readonly Query getProprieties = Query.fromQueryString(QueryTypes.SELECT, "SELECT Proprieties.id AS id, Proprieties.name AS Name, isMain AS IsMain, isReadOnly AS ReadOnly, id_ShowType AS ShowTypeID FROM Proprieties WHERE id_controller = @controllerID", true, true);
        public static readonly Query getRoutes = Query.fromQueryString(QueryTypes.SELECT, "SELECT URLRoutes.id AS id, COALESCE(BaseRoutes.name, URLRoutes.name) AS Name FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute WHERE URLRoutes.id_controller = @controllerID", true, true);
        public static readonly Query insertController = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Controllers (name, isMain) VALUES (@Name, @IsMain)", true, true);
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
            
            controller.routes = (await executor.SelectQuery<DynamicRoute>(getRoutes.setParam("controllerID", controller.id))).ToList();
            controller.proprieties = (await executor.SelectQuery<DynamicPropriety>(getProprieties.setParam("controllerID", controller.id))).ToList();
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
            rolesAvailable = await executor.SelectDictionary<string, long>(getRoles);
            Dictionary<string, DynamicController> controllers = new Dictionary<string, DynamicController>();
            foreach (var controller in await executor.SelectQuery<DynamicController>(getControllers))
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
        public async Task<DynamicController> addRouteQuery(string routeName, string queryString, QueryTypes IDQueryType, bool CompleteCheck, bool CompleteAuth)
        {
            await routes.First(route => route.Name == routeName).addRouteQuery(queryString, IDQueryType, CompleteCheck, CompleteAuth);
            return this;
        }
        public Task addFilters(string routeName, int index, string name, long ShowTypeID, string VarAffected)
        {
            return routes.First(route => route.Name == routeName).queries[index].addFilters(name, ShowTypeID, VarAffected);
        }
        public async Task<DynamicController> addAuthorizedRouteRole(string routeName, long RoleID)
        {
            await routes.First(route => route.Name == routeName).addAuthorizedRole(RoleID);
            return this;
        }
        public async Task<DynamicController> addAuthorizedProprietyRole(string ProprietyName, long RoleID, bool CanModify)
        {
            await proprieties.First(propriety => propriety.Name == ProprietyName).addAuthorizedRole(RoleID, CanModify);
            return this;
        }
        public IEnumerable<string> AuthorizedToSee(params long[] roles)
        {
            return proprieties.Where(propriety => propriety.CanSee(roles)).Select(propriety => propriety.Name);
        }
        public IEnumerable<string> AuthorizedToModify(params long[] roles)
        {
            return proprieties.Where(propriety => propriety.CanModify(roles)).Select(propriety => propriety.Name);
        }
    }
}
