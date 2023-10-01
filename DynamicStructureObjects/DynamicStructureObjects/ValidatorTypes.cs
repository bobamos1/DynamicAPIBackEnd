using Microsoft.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace DynamicStructureObjects
{
    public enum ValidatorTypes
    {
        [Value("Required")]
        REQUIRED = 1,
        [Value("Max")]
        MAX = 2,
        [Value("Min")]
        MIN = 3
    }
    public record ValidatorBundle(ValidatorTypes validatorType, string value, string message); 
    public static class ValidatorTypesHelper
    {
        public static ValidatorBundle SetValue(this ValidatorTypes validatorType, string value, string message) => new ValidatorBundle(validatorType, value, message);
    }
}
