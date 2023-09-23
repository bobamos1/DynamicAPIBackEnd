using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicSQLParamInfo
    {
        public long id { get; set; }
        public string varAffected { get; set; }
        public List<DynamicValidator> Validators { get; set; }
        public static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value, id_ValidatorType FROM ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @SQLParamInfoID");
        public static readonly Query insertSQLParamInfo = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO SQLParamInfos (id_Propriety, id_RouteQuery, varAffected) VALUES (@PropretyID, @RouteQueryID, @VarAffected)");
        public DynamicSQLParamInfo(long id, string varAffected)
        {
            this.id = id;
            this.varAffected = varAffected;
        }
        internal static async Task<DynamicSQLParamInfo> init(DynamicSQLParamInfo paramInfo)
        {
            paramInfo.Validators = (await DynamicController.executor.SelectQueryTotal<DynamicValidator>(getValidators.setParam("SQLParamInfoID", paramInfo.id))).ToList();
            //foreach (var query in paramInfo.dynamicValidators)
                //await DynamicValidator.init(query);
            return paramInfo;
        }
        public async static Task<DynamicSQLParamInfo> addSQLParamInfo(string VarAffected, long ProprietyID, long RouteQueryID)
        {
            return new DynamicSQLParamInfo(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertSQLParamInfo
                        .setParam("PropretyID", ProprietyID)
                        .setParam("RouteQueryID", RouteQueryID)
                        .setParam("VarAffected", VarAffected)
                    )
                , VarAffected
            );
        }
        public async Task<DynamicSQLParamInfo> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorType, false));
            return this;
        }
    }
}
