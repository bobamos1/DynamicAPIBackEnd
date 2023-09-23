using DynamicSQLFetcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace APIDynamic
{
    public class DynamicQueryForRoute
    {
        public long id { get; set; }
        public Query query { get; set; }
        public Dictionary<string, DynamicSQLParamInfo> ParamsInfos { get; set; }
        public List<DynamicFilter> filters { get; set; }
        public static readonly Query getFilters = Query.fromQueryString(QueryTypes.SELECT, "SELECT Filters.id AS id, name AS Name, SQLParamInfos.varAffected AS VarAffected FROM Filters INNER JOIN SQLParamInfos ON SQLParamInfos.id = id_SQLParamInfo WHERE id_RouteQuery = @routeQueryID ORDER BY name, ind");
        public static readonly Query getSQLParamInfos = Query.fromQueryString(QueryTypes.SELECT, "SELECT id, varAffected FROM SQLParamInfos WHERE id_RouteQuery = @RouteQueryID");
        public static readonly Query insertRouteQuery = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO RouteQueries (ind, SQLString, id_queryType, id_route, completeCheck) @Index, @SQLString, @QueryTypeID, @RouteID, @CompleteCheck");
        internal DynamicQueryForRoute(long id, string queryString, long IDQueryType, bool CompleteCheck)
        {
            this.id = id;
            this.query = Query.fromQueryString((QueryTypes)IDQueryType, queryString, CompleteCheck);
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
        }
        internal static async Task<DynamicQueryForRoute> init(DynamicQueryForRoute query)
        {
            foreach (var paramInfo in await DynamicController.executor.SelectQueryTotal<DynamicSQLParamInfo>(getSQLParamInfos.setParam("RouteQueryID", query.id)))
                query.ParamsInfos.Add(paramInfo.varAffected, paramInfo);
            query.filters = (await DynamicController.executor.SelectQueryTotal<DynamicFilter>(getFilters.setParam("routeQueryID", query.id))).ToList();
            foreach (var filter in query.filters)
                await DynamicFilter.init(filter);
            return query;
        }
        public async static Task<DynamicQueryForRoute> addRouteQuery(int index, string queryString, QueryTypes IDQueryType, long RouteID, bool CompleteCheck)
        {
            DynamicQueryForRoute dynamicQueryForRoute = new DynamicQueryForRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRouteQuery
                        .setParam("Index", index)
                        .setParam("SQLString", queryString)
                        .setParam("QueryTypeID", (long)IDQueryType)
                        .setParam("RouteID", RouteID)
                        .setParam("CompleteCheck", CompleteCheck)
                    )
                , queryString
                , (long)IDQueryType
                , CompleteCheck
            );
            foreach (var variable in dynamicQueryForRoute.query.variablesInQuery)
            {
                dynamicQueryForRoute.ParamsInfos.Add(variable.Key, await DynamicSQLParamInfo.addSQLParamInfo(variable.Key, 1, dynamicQueryForRoute.id));
                await dynamicQueryForRoute.ParamsInfos[variable.Key].addValidator((variable.Value ? 0 : 1).ToString(), ValidatorTypes.REQUIRED);
            }

            return dynamicQueryForRoute;
        }
        public async Task<DynamicQueryForRoute> addFilter(string name, ShowTypes showType, string VarAffected)
        {
            filters.Add(await DynamicFilter.addFilter(filters.Count, name, showType, ParamsInfos[VarAffected].id));
            return this;
        }
        public async Task<DynamicQueryForRoute> addSQLParamInfo(string varAffected, long ProprietyID)
        {
            ParamsInfos.Add(varAffected, await DynamicSQLParamInfo.addSQLParamInfo(varAffected, ProprietyID, id));
            return this;
        }
        public async Task<DynamicQueryForRoute> addValidator(string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await ParamsInfos[VarAffected].addValidator(Value, ValidatorType);
            return this; 
        }
    }
}
