using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    internal class DynamicParamInitializer
    {
        internal string AssociatedVarName { get; set; }
        internal object Value { get; set; }
        internal bool IsStatic { get; set; }
        internal static readonly Query insertParamInitializer = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO ListVars (name, value, id_CSharpType, id_link) VALUES (@Name, @Value, @CSharpTypeID, @LinkID)", true);
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
        internal async static Task<DynamicParamInitializer> addParamInitializer(ParamLinker paramLinker, long linkID)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
                insertParamInitializer
                    .setParam("Name", paramLinker.associatedVarName)
                    .setParam("Value", paramLinker.value)
                    .setParam("CSharpTypeID", (long)paramLinker.cSharpType)
                    .setParam("LinkID", linkID)
            );
            return new DynamicParamInitializer(
                  paramLinker.associatedVarName
                , paramLinker.value
                , (long)paramLinker.cSharpType
            );
        }
    }
}
