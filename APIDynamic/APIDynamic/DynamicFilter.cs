using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicFilter
    {
        public long id { get; set; }
        public string Name { get; set; }
        public string VarAffected { get; set; }
        public int index { get; set; }
        public long IDShowType { get; set; }
        public string ShowTypeName { get; set; }
        public List<DynamicValidator> validators { get; set; }
        public static readonly string[] ShowTypes = new string[] { null, "Reference", "String", "Int", "Float" };
        public static readonly Query insertFilter = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Filters (ind, name, id_ShowType, id_RouteQuery, varAffected) VALUES (@index, @name, @ShowTypeID, @RouteQueryID, @VarAffected)");
        public static readonly Query selectLastInd = Query.fromQueryString(QueryTypes.VALUE, "SELECT MAX(ind) + 1 FROM Filters WHERE id_RouteQuery = @RouteQueryID");
        internal DynamicFilter(long id, string Name, string VarAffected, int index, long IDShowType, string ShowTypeName)
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
