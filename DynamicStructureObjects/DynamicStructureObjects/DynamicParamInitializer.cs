using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    internal class DynamicParamInitializer
    {
        internal string AssociatedVarName { get; set; }
        internal object Value { get; set; }
        internal bool IsStatic { get; set; }
        internal static readonly Query insertParamInitializer = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ListVars (name, value, id_CSharpType, id_link) VALUES (@Name, @Value, @CSharpTypeID, @LinkID)", true, true);
        internal DynamicParamInitializer(string AssociatedVarName, string Value, long CSharpType)
        {
            this.AssociatedVarName = AssociatedVarName;
            this.Value = SetValue(Value, (CSharpTypes)CSharpType);
        }
        internal object SetValue(string value, CSharpTypes CSharpType)
        {
            if (CSharpType == CSharpTypes.REFERENCE)
            {
                this.IsStatic = false;
                return value;
            }
            this.IsStatic = true;
            return value;
        }
        internal async static Task<DynamicParamInitializer> addParamInitializer(string AssociatedVarName, string Value, CSharpTypes CSharpType, long linkID)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
                insertParamInitializer
                    .setParam("Name", AssociatedVarName)
                    .setParam("Value", Value)
                    .setParam("CSharpTypeID", (long)CSharpType)
                    .setParam("LinkID", linkID)
            );
            return new DynamicParamInitializer(
                  AssociatedVarName
                , Value
                , (long)CSharpType
            );
        }
    }
}
