using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicSQLParamInfo
    {
        internal long id { get; set; }
        internal long ProprietyID { get; set; }
        internal string VarAffected { get; set; }
        internal bool isRequired { get; set; }
        internal List<DynamicValidator> Validators { get; set; }
        internal static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value, id_ValidatorType FROM ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @SQLParamInfoID", true, true);
        internal static readonly Query insertSQLParamInfo = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO SQLParamInfos (id_Propriety, id_RouteQuery, varAffected) VALUES (@PropretyID, @RouteQueryID, @VarAffected)", true, true);
        internal DynamicSQLParamInfo(long id, string VarAffected, long ProprietyID)
        {
            this.id = id;
            this.VarAffected = VarAffected;
            this.ProprietyID = ProprietyID;
            this.Validators = new List<DynamicValidator>();
        }
        internal static async Task<DynamicSQLParamInfo> init(DynamicSQLParamInfo paramInfo)
        {
            paramInfo.Validators = (
                await DynamicController.executor.SelectQuery<DynamicValidator>(
                    (paramInfo.ProprietyID == 1 ? getValidators : DynamicPropriety.getValidators)
                        .setParam("SQLParamInfoID", (paramInfo.ProprietyID == 1 ? paramInfo.id : paramInfo.ProprietyID))
                    )
                ).ToList();
            foreach (var query in paramInfo.Validators)
                await DynamicValidator.init(query);
            var requiredItem = paramInfo.Validators.FirstOrDefault(item => item.ValidatorType == ValidatorTypes.REQUIRED);

            if (requiredItem != null)
            {
                paramInfo.isRequired = true;
                paramInfo.Validators.Remove(requiredItem);
            }
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
                , ProprietyID
            );
        }
        public async Task<DynamicSQLParamInfo> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorType, false));
            return this;
        }
        internal bool validateParam(object value)
        {
            return Validators.All(validator => validator.validateParam(value));
        }
    }
}
