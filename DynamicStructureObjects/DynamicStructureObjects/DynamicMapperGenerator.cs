﻿using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicMapperGenerator
    {
        public long id { get; internal set; }
        public long controllerID { get; internal set; }
        public string controllerName { get; internal set; }
        public long routeID { get; internal set; }
        public Query query { get; internal set; }
        public DynamicMapper Mapper { get; internal set; }
        public string ProprietyName { get; internal set; }
        public Dictionary<string, object> baseParameters { get; set; }
        public Dictionary<string, string> parametersToLink { get; set; }
        internal static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT name AS AssociatedVarName, value AS Value, id_CSharpType AS CSharpType FROM ListVars WHERE id_link = @link", true, true);
        internal static readonly Query insertMapperGenerator = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO LinkProprietiesControllers (id_propriety, id_controller) VALUES (@PropretyID, @ControllerID)", true, true);
        internal static readonly Query getMapperGeneratorSingleInfo = Query.fromQueryString(QueryTypes.ROW, "SELECT TOP (1) @LinkID AS id, @ControllerID AS controllerID, c.Name AS controllerName, urlR.id AS RouteID, SQLString AS queryString, id_queryType AS QueryTypeID, completeCheck AS CompleteCheck, p.name AS ProprietyName FROM URLRoutes urlR INNER JOIN RouteQueries rq ON rq.id_route = urlR.id INNER JOIN Proprieties p ON p.id = @PropertyID INNER JOIN Controllers c ON c.id = @ControllerID WHERE urlR.id_baseRoute = 1 AND rq.ind = 1 AND urlR.id_controller = @ControllerID", true, true);
        internal static readonly Query getControllerID = Query.fromQueryString(QueryTypes.VALUE, "SELECT id FROM Controllers WHERE name = @ControllerName", true, true);
        internal DynamicMapperGenerator(long id, long controllerID, string controllerName, long routeID, string queryString, long QueryTypeID, bool CompleteCheck, bool CompleteAuth, string ProprietyName)
        {
            this.id = id;
            this.controllerID = controllerID;
            this.controllerName = controllerName;
            this.routeID = routeID; 
            this.query = Query.fromQueryString((QueryTypes)QueryTypeID, queryString, CompleteCheck, CompleteAuth);
            this.ProprietyName = ProprietyName;
            this.Mapper = null;
            this.baseParameters = new Dictionary<string, object>();
            this.parametersToLink = new Dictionary<string, string>();
        }
        internal static async Task<DynamicMapperGenerator> init(DynamicMapperGenerator mapperGenerator)
        {
            IEnumerable<DynamicParamInitializer> parameters = 
                await DynamicController.executor.SelectQuery<DynamicParamInitializer>(
                    getMapperGenerator
                        .setParam("link", mapperGenerator.id)
                    );
            mapperGenerator.baseParameters = parameters.Where(param => param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => item.Value);
            mapperGenerator.parametersToLink = parameters.Where(param => !param.IsStatic).ToDictionary(item => item.AssociatedVarName, item => ParserLib.Parser.to<string>(item.Value));
            mapperGenerator.Mapper = new DynamicMapper(mapperGenerator.ProprietyName, mapperGenerator.parametersToLink, mapperGenerator.baseParameters);
            return mapperGenerator;
        }
        internal DynamicMapper updateMapper(params string[] authorizedColumns)
        {
            return this.Mapper.updateQuery(query.Parse(authorizedColumns));
        }
        internal async static Task<DynamicMapperGenerator> addMapperGenerator(string ControllerName, long ProprietyID, params ParamLinker[] linkers)
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
            DynamicMapperGenerator mapperGenerator =  await DynamicController.executor.SelectSingle<DynamicMapperGenerator>(
                getMapperGeneratorSingleInfo
                .setParam("LinkID", id)
                .setParam("ControllerID", ControllerID)
            );
            foreach (var linker in linkers)
                await mapperGenerator.addParamInitializer(linker.AssociatedVarName, linker.Value, linker.CSharpType);
            await init(mapperGenerator);
            return mapperGenerator;
        }
        internal async Task<DynamicMapperGenerator> addParamInitializer(string AssociatedVarName, string Value, CSharpTypes CSharpType)
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
