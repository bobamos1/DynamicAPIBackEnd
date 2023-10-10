using System.Reflection.Metadata.Ecma335;

namespace DynamicStructureObjects
{
    public enum ShowTypes : long
    {
        [Value("Reference")]
        Ref = 1,
        [Value("CBO")]
        CBO = 2,
        [Value("String")]
        STRING = 3,
        [Value("Int")]
        INT = 4,
        [Value("Float")]
        FLOAT = 5,
        [Value("CBOID")]
        CBOID = 6,
        [Value("ID")]
        ID = 7
    }
    public static class ShowTypesHelper
    {
        public static bool IsCBO(this ShowTypes validatorType) => validatorType == ShowTypes.CBO || validatorType == ShowTypes.CBOID;
        public static bool IsID(this ShowTypes validatorType) => validatorType == ShowTypes.ID || validatorType == ShowTypes.CBOID;
        public static bool IsRef(this ShowTypes validatorType) => validatorType == ShowTypes.Ref;
    }
}
