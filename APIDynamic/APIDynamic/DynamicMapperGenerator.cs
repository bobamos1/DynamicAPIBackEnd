using DynamicSQLFetcher;
using System.ComponentModel.DataAnnotations;

namespace APIDynamic
{
    public class DynamicMapperGenerator
    {
        public long id { get; set; }
        public long controllerID { get; set; }
        public long routeID { get; set; }
        public Query query { get; set; }
        public DynamicMapper mapper { get; set; }
        public string ProprietyName { get; set; }
        internal Dictionary<string, object> baseParameters { get; set; }
        internal Dictionary<string, string> parametersToLink { get; set; }
        public static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT name AS AssociatedVarName, value AS Value, id_CSharpType AS CSharpType FROM ListVars WHERE id_link = @link");
        public static readonly Query insertMapperGenerator = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO LinkProprietiesControllers (id_propriety, id_controller) VALUES (@PropretyID, @ControllerID)");
        public static readonly Query getMapperGeneratorSingleInfo = Query.fromQueryString(QueryTypes.ROW, "SELECT TOP (1) @LinkID AS id, @ControllerID AS controllerID, urlR.id AS RouteID, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck, p.name AS ProprietyName FROM URLRoutes urlR INNER JOIN RouteQueries rq ON rq.id_route = urlR.id INNER JOIN Proprieties p ON p.id = @PropertyID WHERE urlR.id_baseRoute = 1 AND rq.ind = 1 AND urlR.id_controller = @ControllerID");
        public static readonly Query getControllerID = Query.fromQueryString(QueryTypes.VALUE, "SELECT id FROM Controllers WHERE name = @ControllerName");
        public DynamicMapperGenerator(long id, long controllerID, long routeID, string queryString, long IDQueryType, bool CompleteCheck, string ProprietyName)
        {
            this.id = id;
            this.controllerID = controllerID;
            this.routeID = routeID; 
            this.query = Query.fromQueryString((QueryTypes)IDQueryType, queryString, CompleteCheck);
            this.ProprietyName = ProprietyName;
        }
        internal static async Task<DynamicMapperGenerator> init(DynamicMapperGenerator mapperGenerator)
        {
            IEnumerable<DynamicParamInitializer> parameters = await DynamicController.executor.SelectQueryTotal<DynamicParamInitializer>(getMapperGenerator.setParam("link", mapperGenerator.id));
            mapperGenerator.baseParameters = parameters.Where(param => param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => item.Value);
            mapperGenerator.parametersToLink = parameters.Where(param => !param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => ParserLib.Parser.to<string>(item.Value));
            mapperGenerator.mapper = new DynamicMapper(mapperGenerator.ProprietyName, mapperGenerator.parametersToLink, mapperGenerator.baseParameters);
            return mapperGenerator;
        }
        public DynamicMapper updateMapper()
        {
            return this.mapper.updateQuery(query.Parse("id"));//faudra gerer les authorizations
        }
        public async static Task<DynamicMapperGenerator> addMapperGenerator(string ControllerName, long ProprietyID, params ParamLinker[] linkers)
        {
            long ControllerID = await DynamicController.executor.SelectValue<long>(
                getControllerID
                    .setParam("ControllerName", ControllerName)
            );
            long id = await DynamicController.executor.ExecuteInsertWithLastID(
                insertMapperGenerator
                    .setParam("PropretyID", ProprietyID)
                    .setParam("ControllerID", ControllerID)
            );
            DynamicMapperGenerator mapperGenerator =  await DynamicController.executor.SelectSingleTotal<DynamicMapperGenerator>(
                getMapperGeneratorSingleInfo
                .setParam("LinkID", id)
                .setParam("ControllerID", ControllerID)
            );
            foreach (var linker in linkers)
                await mapperGenerator.addParamInitializer(linker.AssociatedVarName, linker.Value, linker.CSharpType);
            await init(mapperGenerator);
            return mapperGenerator;
        }
        public async Task<DynamicMapperGenerator> addParamInitializer(string AssociatedVarName, string Value, CSharpTypes CSharpType)
        {
            DynamicParamInitializer initializer = await DynamicParamInitializer.addParamInitializer(AssociatedVarName, Value, CSharpType, id);
            if (initializer.IsStatic)
                baseParameters.Add(initializer.AssociatedVarName, initializer.Value);
            else
                parametersToLink.Add(initializer.AssociatedVarName, Value);
            return this;
        }

    }
}
