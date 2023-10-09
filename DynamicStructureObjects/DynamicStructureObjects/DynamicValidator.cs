using DynamicSQLFetcher;
using ParserLib;
using System.Text.RegularExpressions;

namespace DynamicStructureObjects
{
    public class DynamicValidator
    {
        public ValidatorTypes ValidatorType { get; internal set; }
        public object Value { get; internal set; }
        public string Message { get; internal set; }
        internal static readonly Query insertSQLParamInfoValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorSQLParamInfoValues (id_SQLParamInfo, id_ValidatorType, value, message) VALUES (@ParentID, @ValidatorTypeID, @Value, @Message)", true);
        internal static readonly Query insertProprietyValidators = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ValidatorProprietyValues (id_ValidatorType, id_ValidatorType, value, message) VALUES (@ParentID, @ValidatorTypeID, @Value, @Message)", true);
        internal DynamicValidator(string Value, long ValidatorTypeID, string message)
        {
            this.ValidatorType = (ValidatorTypes)ValidatorTypeID;
            this.Value = parseValue(Value);
            this.Message = message;
        }
        internal static async Task<DynamicValidator> init(DynamicValidator validator)
        {
            return validator;
        }
        internal static Task<DynamicValidator> addValidator(string Value, long ParentID, ValidatorTypes ValidatorType, bool forPropriety)
        {
            return addValidator(new ValidatorBundle(ValidatorType, Value, ""), ParentID, forPropriety);
        }
        internal async static Task<DynamicValidator> addValidator(ValidatorBundle bundle, long ParentID, bool forPropriety)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(
                (forPropriety ? insertProprietyValidators : insertSQLParamInfoValidators)
                    .setParam("Value", bundle.value)
                    .setParam("ParentID", ParentID)
                    .setParam("ValidatorTypeID", (long)bundle.validatorType)
                    .setParam("Message", bundle.message)
            );
            return new DynamicValidator(
                  bundle.value
                , (long)bundle.validatorType
                , bundle.message
            );
        }
        internal object parseValue(string value)
        {
            switch (ValidatorType)
            {
                case ValidatorTypes.REQUIRED:
                    return value.To<bool>();
                case ValidatorTypes.MAX:
                    return value.To<double>();
                case ValidatorTypes.MIN:
                    return value.To<double>();
                case ValidatorTypes.REGEX:
                    return new Regex(value.To<string>());
                case ValidatorTypes.MAXOREQUAL:
                    return value.To<double>();
                case ValidatorTypes.MINOREQUAL:
                    return value.To<double>();
                default:
                    return value;
            }
        }
        internal bool validateParam(object value)
        {
            switch (ValidatorType)
            {
                case ValidatorTypes.REQUIRED:
                    return value.To<bool>() == Value.To<bool>();
                case ValidatorTypes.MAX:
                    return value.To<double>() < Value.To<double>();
                case ValidatorTypes.MAXOREQUAL:
                    return value.To<double>() <= Value.To<double>();
                case ValidatorTypes.MIN:
                    return value.To<double>() > Value.To<double>();
                case ValidatorTypes.MINOREQUAL:
                    return value.To<double>() >= Value.To<double>();
                case ValidatorTypes.REGEX:
                    return Value.To<Regex>().IsMatch(value.To<string>());
                default:
                    return false;
            }
        }
    }
}
