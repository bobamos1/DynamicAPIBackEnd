using DynamicSQLFetcher;
using System.Reflection.Metadata;

namespace DynamicStructureObjects
{
    public class DynamicSQLParamInfo
    {
        internal long id { get; set; }
        internal long ProprietyID { get; set; }
        internal string VarAffected { get; set; }
        internal bool isRequired { get; set; }
        internal List<DynamicValidator> Validators { get; set; }
        internal static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value AS Value, id_ValidatorType AS ValidatorTypeID, message FROM ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @SQLParamInfoID UNION ALL SELECT value AS Value, id_ValidatorType AS ValidatorTypeID, message FROM ValidatorProprietyValues WHERE id_Propriety = @ProprietyID", true);
        internal static readonly Query insertSQLParamInfo = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO SQLParamInfos (id_Propriety, id_RouteQuery, varAffected) VALUES (@PropretyID, @RouteQueryID, @VarAffected)", true);
        internal DynamicSQLParamInfo(long id, string VarAffected, long ProprietyID)
        {
            this.id = id;
            this.VarAffected = VarAffected;
            this.ProprietyID = ProprietyID;
            this.Validators = new List<DynamicValidator>();
            this.isRequired = false;
        }
        internal DynamicSQLParamInfo(DynamicSQLParamInfo paramInfo, bool isRequired)
        {
            this.id = paramInfo.id;
            this.VarAffected = paramInfo.VarAffected;
            this.ProprietyID = paramInfo.ProprietyID;
            this.Validators = paramInfo.Validators;
            this.isRequired = isRequired;
        }
        internal static async Task<DynamicSQLParamInfo> init(DynamicSQLParamInfo paramInfo)
        {
            paramInfo.Validators = (
                await DynamicController.executor.SelectQuery<DynamicValidator>(
                    getValidators
                        .setParam("SQLParamInfoID", paramInfo.id)
                        .setParam("ProprietyID", paramInfo.ProprietyID)
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
        public Task<DynamicSQLParamInfo> addValidator(params ValidatorBundle[] bundles)
        {
            return addValidator(false, bundles);
        }
        public async Task<DynamicSQLParamInfo> addValidator(bool addRequired, params ValidatorBundle[] bundles)
        {
            foreach (var bundle in bundles)
                Validators.Add(await DynamicValidator.addValidator(bundle, id, false));
            if (addRequired)
                Validators.Add(await DynamicValidator.addValidator(ValidatorTypes.REQUIRED.SetValue("true", "needed"), id, false));
            return this;
        }
        internal bool validateParam(object value)
        {
            return Validators.All(validator => validator.validateParam(value));
        }
    }
}
