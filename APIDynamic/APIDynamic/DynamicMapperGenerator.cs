using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicMapperGenerator
    {
        public long id { get; set; }
        public long controllerID { get; set; }
        public long routeID { get; set; }
        public Query query { get; set; }
        public DynamicMapper mapper { get; set; }
        public string PropretyName { get; set; }
        internal Dictionary<string, object> baseParameters { get; set; }
        internal Dictionary<string, string> parametersToLink { get; set; }
        public static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT name AS AssociatedVarName, value AS Value, id_CSharpType AS CSharpType FROM ListVars WHERE id_link = @link");
        public DynamicMapperGenerator(long id, long controllerID, long routeID, string queryString, long IDQueryType, bool CompleteCheck)
        {
            this.id = id;
            this.controllerID = controllerID;
            this.routeID = routeID; 
            this.query = Query.fromQueryString((QueryTypes)IDQueryType, queryString, CompleteCheck);
        }
        internal static async Task<DynamicMapperGenerator> init(DynamicMapperGenerator mapperGenerator, string proprietyName)
        {
            IEnumerable<DynamicParamInitializer> parameters = await DynamicController.executor.SelectQueryTotal<DynamicParamInitializer>(getMapperGenerator.setParam("link", mapperGenerator.id));
            mapperGenerator.baseParameters = parameters.Where(param => param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => item.Value);
            mapperGenerator.parametersToLink = parameters.Where(param => !param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => item.Value.ToString());
            mapperGenerator.mapper = new DynamicMapper(proprietyName, mapperGenerator.parametersToLink, mapperGenerator.baseParameters);
            return mapperGenerator;
        }
        public DynamicMapper updateMapper()
        {
            return this.mapper.updateQuery(query.Parse("id"));//faudra gerer les authorizations
        }
    }
}
