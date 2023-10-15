namespace DynamicStructureObjects
{
    public enum ShowTypes : long
    {
        [Value("None")]
        NONE = 1,
        [Value("Reference")]
        Ref = 2,
        [Value("CBO")]
        CBO = 3,
        [Value("String")]
        STRING = 4,
        [Value("Int")]
        INT = 5,
        [Value("Float")]
        FLOAT = 6,
        [Value("CBOID")]
        CBOID = 7,
        [Value("ID")]
        ID = 8
    }
    public static class ShowTypesHelper
    {
        public static bool IsCBO(this ShowTypes validatorType) => validatorType == ShowTypes.CBO || validatorType == ShowTypes.CBOID;
        public static bool IsID(this ShowTypes validatorType) => validatorType == ShowTypes.ID || validatorType == ShowTypes.CBOID;
        public static bool IsRef(this ShowTypes validatorType) => validatorType == ShowTypes.Ref;
    }
}
