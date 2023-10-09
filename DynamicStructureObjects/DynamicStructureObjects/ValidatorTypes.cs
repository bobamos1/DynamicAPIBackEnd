namespace DynamicStructureObjects
{
    public enum ValidatorTypes
    {
        [Value("Required")]
        REQUIRED = 1,
        [Value("Max")]
        MAX = 2,
        [Value("Min")]
        MIN = 3,
        [Value("Regex")]
        REGEX = 4,
        [Value("MaxOrEqual")]
        MAXOREQUAL = 5,
        [Value("MinOrEqual")]
        MINOREQUAL = 6,
        [Value("Boolean")]
        BOOLEAN = 7
    }
    public record ValidatorBundle(ValidatorTypes validatorType, string value, string message); 
    public static class ValidatorTypesHelper
    {
        public static ValidatorBundle SetValue(this ValidatorTypes validatorType, string value, string message) => new ValidatorBundle(validatorType, value, message);
    }
}
