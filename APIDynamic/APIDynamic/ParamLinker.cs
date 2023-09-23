namespace APIDynamic
{
    public struct ParamLinker
    {
        public ParamLinker(string associatedVarName, string value, CSharpTypes cSharpType)
        {
            AssociatedVarName = associatedVarName;
            Value = value;
            CSharpType = cSharpType;
        }

        public string AssociatedVarName { get; set; }
        public string Value { get; set; }
        public CSharpTypes CSharpType { get; set; }
    }
}
