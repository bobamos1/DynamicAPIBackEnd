using DynamicSQLFetcher;
using System;

namespace APIDynamic
{
    public class DynamicPropriety
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool ReadOnly { get; set; }
        public List<DynamicValidator> Validators { get; set; }
        public ShowTypes ShowType { get; set; }
        public DynamicMapperGenerator mapperGenerator { get; set; }
        public static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value AS Value, id_ValidatorType AS ValidatorTypeID FROM ValidatorProprietyValues WHERE id_Propriety = @ProprietyID");
        public static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT lnk.id AS id, c.id AS controllerID, urlR.id AS RouteID, SQLString AS queryString, id_queryType AS IDQueryType, completeCheck AS CompleteCheck, p.name AS ProprietyName FROM LinkProprietiesControllers lnk INNER JOIN Controllers c ON c.id = lnk.id_controller INNER JOIN URLRoutes urlR ON urlR.id_controller = c.id INNER JOIN RouteQueries rq ON rq.id_route = urlR.id INNER JOIN Proprieties p ON p.id = lnk.id_propriety WHERE urlR.id_baseRoute = 1 AND rq.ind = 1 AND lnk.id_propriety = @ProprietyID");
        public static readonly Query insertPropriety = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Proprieties (name, isMain, isReadOnly,id_ShowType, id_controller) VALUES (@Name, @IsMain, @IsReadOnly, @ShowTypeID, @ControllerID)");
        internal DynamicPropriety(long id, string Name, bool IsMain, bool ReadOnly, long ShowTypeID)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.ReadOnly = ReadOnly;
            this.ShowType = (ShowTypes)ShowTypeID;
            this.Validators = new List<DynamicValidator>();
        }
        internal static async Task<DynamicPropriety> init(DynamicPropriety propriety)
        {
            propriety.Validators = (await DynamicController.executor.SelectQueryTotal<DynamicValidator>(getValidators.setParam("ProprietyID", propriety.id))).ToList();
            if (propriety.ShowType == ShowTypes.Ref)
            {
                propriety.mapperGenerator = await DynamicController.executor.SelectSingleTotal<DynamicMapperGenerator>(getMapperGenerator.setParam("ProprietyID", propriety.id));
                if (propriety.mapperGenerator is not null)
                    await DynamicMapperGenerator.init(propriety.mapperGenerator);
            }
            return propriety;
        }
        public async static Task<DynamicPropriety> addPropriety(string Name, bool IsMain, bool IsReadOnly, ShowTypes showType, long ControllerID)
        {
            return new DynamicPropriety(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertPropriety
                        .setParam("Name", Name)
                        .setParam("IsMain", IsMain)
                        .setParam("IsReadOnly", IsReadOnly)
                        .setParam("ShowTypeID", (long)showType)
                        .setParam("ControllerID", ControllerID)
                    )
                , Name
                , IsMain
                , IsReadOnly
                , (long)showType
            );
        }
        public async Task<DynamicPropriety> addMapperGenerator(string ControllerName, params ParamLinker[] linkers)
        {
            if (ShowType != ShowTypes.Ref)
                throw new Exception("cannot add mapper on not ref proprety");
            mapperGenerator = await DynamicMapperGenerator.addMapperGenerator(ControllerName, id, linkers);
            return this;
        }
        public async Task<DynamicPropriety> addValidator(string Value, ValidatorTypes ValidatorTypeID)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorTypeID, true));
            return this;
        }
    }
}
