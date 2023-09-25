using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicFilter
    {
        public long id { get; set; }
        public string Name { get; set; }
        public string VarAffected { get; set; }
        public static readonly Query insertFilter = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Filters (ind, name, id_ShowType, id_SQLParamInfo) VALUES (@index, @name, @ShowTypeID, @SQLParamInfoID)", true, true);
        public static readonly Query selectSQLParamName = Query.fromQueryString(QueryTypes.VALUE, "SELECT varAffected FROM SQLParamInfos WHERE id = @SQLParamInfoID", true, true);
        internal DynamicFilter(long id, string Name, string VarAffected)
        {
            this.id = id;
            this.Name = Name;
            this.VarAffected = VarAffected;
            this.index = index;
            this.IDShowType = IDShowType;
            this.ShowTypeName = ShowTypeName;
            this.validators = new List<DynamicValidator>();
        }
        public async static Task addFilters(List<DynamicFilter> filters, string name, long ShowTypeID, long RouteQueryID, string VarAffected)
        {
            int index = await DynamicController.executor.SelectValue<int>(selectLastInd);
            filters.Add(new DynamicFilter(await DynamicController.executor.ExecuteInsertWithLastID(insertFilter.setParam("index", index).setParam("name", name).setParam("ShowTypeID", ShowTypes[ShowTypeID]).setParam("RouteQueryID", RouteQueryID).setParam("VarAffected", VarAffected)), name, VarAffected, index, (int)ShowTypeID, ShowTypes[ShowTypeID]));
        }
    }
}
