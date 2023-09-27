using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    internal class DynamicValidator
    {
        internal ValidatorTypes ValidatorType { get; set; }
        internal string Value { get; set; }
        internal static readonly Query insertSQLParamInfoValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorSQLParamInfoValues (id_SQLParamInfo, id_ValidatorType, value) VALUES (@ParentID, @ValidatorTypeID, @Value)", true, true);
        internal static readonly Query insertProprietyValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorProprietyValues (id_ValidatorType, id_ValidatorType, value) VALUES (@ParentID, @ValidatorTypeID, @Value)", true, true);
        internal DynamicValidator(string Value, long ValidatorTypeID)
        {
            this.ValidatorType = (ValidatorTypes)ValidatorTypeID;
            this.Value = Value;
        }
        internal static async Task<DynamicValidator> init(DynamicValidator validator)
        {
            return validator;
        }
        internal async static Task<DynamicValidator> addValidator(string Value, long ParentID, ValidatorTypes ValidatorType, bool forPropriety)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
                (forPropriety ? insertProprietyValidators : insertSQLParamInfoValidators)
                    .setParam("Value", Value)
                    .setParam("ParentID", ParentID)
                    .setParam("ValidatorTypeID", (long)ValidatorType)
            );
            return new DynamicValidator(
                  Value
                , (long)ValidatorType
            );
        }
    }
}
