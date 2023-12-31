﻿using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicFilter
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public string VarAffected { get; internal set; }
        internal static readonly Query insertFilter = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Filters (ind, name, id_ShowType, id_SQLParamInfo) VALUES (@index, @name, @ShowTypeID, @SQLParamInfoID)", true);
        internal static readonly Query selectSQLParamName = Query.fromQueryString(QueryTypes.VALUE, "SELECT varAffected FROM SQLParamInfos WHERE id = @SQLParamInfoID", true);
        internal DynamicFilter(long id, string Name, string VarAffected)
        {
            this.id = id;
            this.Name = Name;
            this.VarAffected = VarAffected;
        }
        internal static async Task<DynamicFilter> init(DynamicFilter filter)
        {
            return filter;
        }
        internal async static Task<DynamicFilter> addFilter(int index, string name, ShowTypes showType, long SQLParamInfoID, string VarAffected)
        {
            return new DynamicFilter(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertFilter
                        .setParam("index", index)
                        .setParam("name", name)
                        .setParam("ShowTypeID", (long)showType)
                        .setParam("SQLParamInfoID", SQLParamInfoID)
                    )
                , name
                , VarAffected //await DynamicController.executor.SelectValue<string>(selectSQLParamName.setParam("SQLParamInfoID", SQLParamInfoID))
            );
        }
    }
}
