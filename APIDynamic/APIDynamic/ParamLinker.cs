namespace APIDynamic
{
    public class ParamLinker
    {
        public string AssociatedVarName { get; internal set; }
        public string Value { get; internal set; }
        public CSharpTypes CSharpType { get; internal set; }
    }
}