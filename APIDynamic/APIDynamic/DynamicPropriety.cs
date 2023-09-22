using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicPropriety
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool ReadOnly { get; set; }
        public IEnumerable<DynamicValidator> validators { get; set; }
        public long IDShowType { get; set; }
        public string ShowTypeName { get; set; }
        public DynamicMapperGenerator mapperGenerator { get; set; }
        public static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT lnk.id AS id, c.id AS controllerID, urlR.id AS RouteID, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck FROM LinkProprietiesControllers lnk INNER JOIN Controllers c ON c.id = lnk.id_controller INNER JOIN URLRoutes urlR ON urlR.id_controller = c.id INNER JOIN RouteQueries rq ON rq.id_route = urlR.id WHERE urlR.id_baseRoute = 1 AND rq.ind = 1 AND lnk.id_propriety = @proprietyID");
        internal DynamicPropriety(long id, string Name, bool IsMain, long IDShowType, bool ReadOnly, string ShowTypeName)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.ReadOnly = ReadOnly;
            this.IDShowType = IDShowType;
            this.ShowTypeName = ShowTypeName;
            this.validators = new List<DynamicValidator>();
        }
        internal static async Task<DynamicPropriety> init(DynamicPropriety propriety)
        {
            if (propriety.IDShowType == 1)
            {
                propriety.mapperGenerator = await DynamicController.executor.SelectSingleTotal<DynamicMapperGenerator>(getMapperGenerator.setParam("proprietyID", propriety.id));
                if (propriety.mapperGenerator is not null)
                    await DynamicMapperGenerator.init(propriety.mapperGenerator, propriety.Name);
            }
            return propriety;
        }
    }
}
