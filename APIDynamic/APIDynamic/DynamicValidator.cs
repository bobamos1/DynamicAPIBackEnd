using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicValidator
    {
        public ValidatorTypes ValidatorType { get; set; }
        public string Value { get; set; }
        public static readonly Query insertSQLParamInfoValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorSQLParamInfoValues (id_SQLParamInfo, id_ValidatorType, value) VALUES (@ParentID, @ValidatorTypeID, @Value)", true, true);
        public static readonly Query insertProprietyValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorProprietyValues (id_ValidatorType, id_ValidatorType, value) VALUES (@ParentID, @ValidatorTypeID, @Value)", true, true);
        public DynamicValidator(string Value, long ValidatorTypeID)
        {
            this.ValidatorType = (ValidatorTypes)ValidatorTypeID;
            this.Value = Value;
        }
        public async static Task<DynamicValidator> addValidator(string Value, long ParentID, ValidatorTypes ValidatorType, bool forPropriety)
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
