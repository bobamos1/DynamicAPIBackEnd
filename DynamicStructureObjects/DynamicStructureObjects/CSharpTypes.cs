namespace DynamicStructureObjects
{
    public enum CSharpTypes
    {
        [Value("Reference")]
        REFERENCE = 1,
        [Value("String")]
        STRING = 2,
        [Value("Int")]
        Int = 3,
    }

    public static class CSharpTypeHelper
    {
        public static ParamLinker Link(this CSharpTypes CSharpType, string value, string varInRefQuery)
        {
            return new ParamLinker(varInRefQuery, value, CSharpType);
        }
    }
}
