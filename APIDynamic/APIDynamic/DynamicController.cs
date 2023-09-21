using DynamicSQLFetcher;
using System.ComponentModel;

namespace APIDynamic
{
    public class DynamicController
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public static SQLExecutor executor { get; set; }
        public static WebApplication app { get; set; }
        public static Query getRoutes = Query.fromQueryString(QueryType.CBO, "SELECT COALESCE(br.name, r.name) AS routeName, r.id FROM URLRoutes r LEFT JOIN BaseRoutes br ON br.id = r.id_baseRoute WHERE r.id_controller = @controllerID");
        public Dictionary<string, DynamicRoute> routes { get; set; }
        public Dictionary<string, DynamicPropriety> proprieties { get; set; }
        private DynamicController(long id, string Name, bool IsMain)
        {
            if (executor is null)
                throw new ArgumentNullException(nameof(executor));
            if (app is null)
                throw new ArgumentNullException(nameof(app));
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
        }
        public async Task<DynamicController> init(long id, string Name, bool IsMain)
        {
            DynamicController dynamicController = new DynamicController(id, Name, IsMain);
            Dictionary<object, object> routes = await executor.SelectDictionary(getRoutes.setParam("controllerID", id));

            return dynamicController;
        }
    }
}
