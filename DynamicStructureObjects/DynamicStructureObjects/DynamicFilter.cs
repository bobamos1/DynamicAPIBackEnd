using Dapper;
using DynamicSQLFetcher;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicStructureObjects
{
    public class DynamicFilter
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string Placeholder { get; internal set; }
        public ShowTypes ShowType { get; internal set; }
        public string RefController { get; internal set; }
        public int ind { get; internal set; }
        public IEnumerable<DynamicValidator> Validators { get; internal set; }
        public IEnumerable<DynamicSQLParamInfo> AffectedVars { get; internal set; }
        internal static readonly Query insertFilter = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Filters (displayName, description, ind, id_showType, id_route, placeholder, refController) VALUES (@DisplayName, @Description, @ind, @ShowTypeID, @RouteID, @placeholder, @refController)");
        internal static readonly Query insertFilterBinder = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO FiltersSQLParamsInfo (id_filter, id_SQLParamInfo, ind) VALUES (@FilterID, @SQLParamInfoID, @ind)");
        public DynamicFilter(long id, string DisplayName, string Description, long ShowTypeID, int ind, string placeholder, string refController)
        {
            this.id = id;
            this.ind = ind;
            this.Name = DisplayName;
            this.Description = Description;
            this.ShowType = (ShowTypes)ShowTypeID;
            this.AffectedVars = new List<DynamicSQLParamInfo>();
            this.Placeholder = placeholder;
            this.Validators = new DynamicValidator[0];
            this.RefController = refController;
        }
        public DynamicFilter(string DisplayName, string Description, string placeholder, ShowTypes ShowType, string refController, int ind, params DynamicSQLParamInfo[] AffectedVars) : this(DisplayName, Description, placeholder, ShowType, refController, ind, (IEnumerable<DynamicSQLParamInfo>)AffectedVars)
        { }
        public DynamicFilter(string DisplayName, string Description, string placeholder, ShowTypes ShowType, string refController, int ind, IEnumerable<DynamicSQLParamInfo> AffectedVars)
        {
            this.ind = ind;
            this.Name = DisplayName;
            this.Description = Description;
            this.ShowType = ShowType;
            this.AffectedVars = AffectedVars;
            this.Placeholder = placeholder;
            this.Validators = new DynamicValidator[0];
        }
        public static Task<DynamicFilter> addFilter(string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, long RouteID, params DynamicSQLParamInfo[] SQLVariables)
        {
            return addFilter(DisplayName, Description, placeholder, showType, refController, ind, RouteID, (IEnumerable<DynamicSQLParamInfo>)SQLVariables);
        }
        public async static Task<DynamicFilter> addFilter(string DisplayName, string Description, string placeholder, ShowTypes showType, string refController, int ind, long RouteID, IEnumerable<DynamicSQLParamInfo> SQLVariables)
        {
            var idFilter = await DynamicController.executor.ExecuteInsertWithLastID(
                insertFilter
                    .setParam("DisplayName", DisplayName)
                    .setParam("Description", Description)
                    .setParam("ShowTypeID", (long)showType)
                    .setParam("ind", ind)
                    .setParam("RouteID", RouteID)
                    .setParam("placeholder", placeholder)
                    .setParam("refController", refController)
            );
            if (!SQLVariables.Any())
                throw new Exception();
            await DynamicController.executor.ExecuteQueryWithTransaction(insertFilterBinder.toOrderedPairs("SQLParamInfoID", "FilterID", idFilter, SQLVariables.Select(param => param.id), (query, i) => query.setParam("ind", i)));
            return new DynamicFilter(DisplayName, Description, placeholder, showType, "", ind, SQLVariables);
        }
    }
}
