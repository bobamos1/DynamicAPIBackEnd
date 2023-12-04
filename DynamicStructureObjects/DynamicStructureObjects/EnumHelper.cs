using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ValueAttribute : Attribute
    {
        public readonly string Value;

        public ValueAttribute(string value)
        {
            Value = value;
        }
    }
    public static class EnumHelper
    {
        public static string ValueOf<TEnum>(TEnum? enumEntry) where TEnum : struct, Enum
        {
            if (enumEntry == null)
                throw new Exception("enum not valid");
            return ValueOf<TEnum>((TEnum)enumEntry);
        }
        public static string ValueOf<TEnum>(TEnum enumEntry) where TEnum : Enum
        {
            return ValueOf(enumEntry, typeof(TEnum));
        }
        public static string ValueOf(Enum enumEntry, Type enumType)
        {
            var enumValue = enumEntry.ToString();
            var field = enumType.GetField(enumValue);
            var attribute = field.GetCustomAttributes(typeof(ValueAttribute), false).FirstOrDefault() as ValueAttribute;

            return attribute?.Value;
        }
        public static string Value<TEnum>(this TEnum enumEntry) where TEnum : Enum => ValueOf<TEnum>(enumEntry);
        public static string Value(this Enum enumEntry, Type enumType) => ValueOf(enumEntry, enumType);
        public static long ID<TEnum>(this TEnum enumEntry) where TEnum : Enum => Convert.ToInt64(enumEntry);
        public static KeyValuePair<TEnum, bool> CanModify<TEnum>(this TEnum enumEntry) where TEnum : struct, Enum => new KeyValuePair<TEnum, bool>(enumEntry, true);
        public static KeyValuePair<TEnum, bool> CannotModify<TEnum>(this TEnum enumEntry) where TEnum : struct, Enum => new KeyValuePair<TEnum, bool>(enumEntry, false);

        public static string insertTemplate = "INSERT INTO {0} (id, name) VALUES ({{0}}, '{{1}}')";
        public static Dictionary<string, Type> enumTables = new Dictionary<string, Type>(){ { "BaseRoutes", typeof(BaseRoutes) }, { "CSharpTypes", typeof(CSharpTypes) }, { "QueryTypes", typeof(QueryTypes) }, { "RouteTypes", typeof(RouteTypes) }, { "ShowTypes", typeof(ShowTypes) }, { "ValidatorTypes", typeof(ValidatorTypes) }, { "RouteDisplayTypes", typeof(RouteDisplayTypes) } };
        public static async Task populateBDEnums(SQLExecutor executor)
        {
            var templates = enumTables.ToDictionary(table => table, table => string.Format(insertTemplate, table.Key));
            foreach (var template in templates)
            {
                await DynamicController.InsertEnum(executor, template.Value, template.Key.Key, template.Key.Value);
            }
        }
    }
}
