namespace DynamicSQLFetcher
{
    internal class SQLVariableInfo
    {
        internal string ConditionalString { get; set; }
        internal string VarName { get; set; }
        internal bool isSQLText { get; set; }
        internal SQLVariableInfo(string ConditionalString, string VarName, bool isSQLText)
        {
            this.ConditionalString = ConditionalString;
            this.VarName = VarName;
            this.isSQLText = isSQLText;
            if (isSQLText)
            {
                int index = ConditionalString.IndexOf('@');
                this.ConditionalString = string.Format("{0}{{0}}{1}", ConditionalString.Substring(0, index), ConditionalString.Substring(index + VarName.Length + 2));
            }
        }
        internal string validateVar(Dictionary<string, object> paramsUsed)
        {
            if (paramsUsed.ContainsKey(VarName))
                return isSQLText ? string.Format(ConditionalString, paramsUsed[VarName].ToString()) : ConditionalString;
            return "";
        }
        internal void removeVarIdentifier()
        {
            foreach (var varIdentifier in Query.variableType)
                ConditionalString = ConditionalString.Replace($"{'@'}{varIdentifier.Key}", "@");
        }
    }
}
