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
                throw new Exception();
            return ValueOf<TEnum>((TEnum)enumEntry);
        }
        public static string ValueOf<TEnum>(TEnum enumEntry) where TEnum : Enum
        {
            var enumValue = enumEntry.ToString();
            var field = typeof(TEnum).GetField(enumValue);
            var attribute = field.GetCustomAttributes(typeof(ValueAttribute), false).FirstOrDefault() as ValueAttribute;

            return attribute?.Value;
        }
        public static string Value<TEnum>(this TEnum enumEntry) where TEnum : struct, Enum => ValueOf<TEnum>(enumEntry);
        public static KeyValuePair<TEnum, bool> CanModify<TEnum>(this TEnum enumEntry) where TEnum : struct, Enum => new KeyValuePair<TEnum, bool>(enumEntry, true);
        public static KeyValuePair<TEnum, bool> CannotModify<TEnum>(this TEnum enumEntry) where TEnum : struct, Enum => new KeyValuePair<TEnum, bool>(enumEntry, false);
    }
}
