using DynamicSQLFetcher;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace APIDynamic
{
    public class DynamicQueryForRoute
    {
        public long id { get; set; }
        public Query query { get; set; }
<<<<<<< Updated upstream
        public List<DynamicFilter> filters { get; set; }
        public static readonly Query getFilters = Query.fromQueryString(QueryTypes.SELECT, "SELECT Filters.id AS id, Filters.name AS Name, varAffected AS VarAffected, ind AS [Index], id_ShowType AS IDShowType, ShowTypes.name AS ShowTypeName FROM Filters INNER JOIN ShowTypes ON ShowTypes.id = id_ShowType WHERE id_RouteQuery = @routeQueryID ORDER BY Filters.name, ind");
        public static readonly Query insertRouteQuery = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO RouteQueries (ind, SQLString, id_queryType, id_route, completeCheck) SELECT MAX(ind) + 1, @SQLString, @QueryTypeID, @RouteID, @CompleteCheck FROM RouteQueries WHERE id_route = @RouteID");
        internal DynamicQueryForRoute(long id, string queryString, long IDQueryType, bool CompleteCheck)
        {
            this.id = id;
            this.query = Query.fromQueryString((QueryTypes)IDQueryType, queryString, CompleteCheck);
        }
        internal static async Task<DynamicQueryForRoute> init(DynamicQueryForRoute query)
        {
            query.filters = (await DynamicController.executor.SelectQueryTotal<DynamicFilter>(getFilters.setParam("routeQueryID", query.id))).ToList();
            return query;
        }
        public async static Task addRouteQuery(List<DynamicQueryForRoute> routeQueries, string queryString, QueryTypes IDQueryType, long RouteID, bool CompleteCheck)
        {
            routeQueries.Add(new DynamicQueryForRoute(await DynamicController.executor.ExecuteInsertWithLastID(insertRouteQuery.setParam("SQLString", queryString).setParam("QueryTypeID", (long)IDQueryType).setParam("RouteID", RouteID).setParam("CompleteCheck", CompleteCheck)), queryString, (long)IDQueryType, CompleteCheck));
=======
        public bool CompleteAuth { get; set; }
        public Dictionary<string, DynamicSQLParamInfo> ParamsInfos { get; set; }
        public List<DynamicFilter> filters { get; set; }
        public static readonly Query getFilters = Query.fromQueryString(QueryTypes.SELECT, "SELECT Filters.id AS id, name AS Name, SQLParamInfos.varAffected AS VarAffected FROM Filters INNER JOIN SQLParamInfos ON SQLParamInfos.id = id_SQLParamInfo WHERE id_RouteQuery = @routeQueryID ORDER BY name, ind", true, true);
        public static readonly Query getSQLParamInfos = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, varAffected AS varAffected, id_Propriety AS ProprietyID FROM SQLParamInfos WHERE id_RouteQuery = @RouteQueryID", true, true);
        public static readonly Query insertRouteQuery = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO RouteQueries (ind, SQLString, id_queryType, id_route, completeCheck, completeAuth) VALUES (@Index, @SQLString, @QueryTypeID, @RouteID, @CompleteCheck, @CompleteAuth)", true, true);
        internal DynamicQueryForRoute(long id, string queryString, long IDQueryType, bool CompleteCheck, bool CompleteAuth)
        {
            this.id = id;
            this.query = Query.fromQueryString((QueryTypes)IDQueryType, queryString, CompleteCheck, CompleteAuth);
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
            this.CompleteAuth = CompleteAuth;
        }
        internal static async Task<DynamicQueryForRoute> init(DynamicQueryForRoute query)
        {
            foreach (var paramInfo in await DynamicController.executor.SelectQuery<DynamicSQLParamInfo>(getSQLParamInfos.setParam("RouteQueryID", query.id)))
                query.ParamsInfos.Add(paramInfo.varAffected, paramInfo);
            query.filters = (await DynamicController.executor.SelectQuery<DynamicFilter>(getFilters.setParam("routeQueryID", query.id))).ToList();
            foreach (var filter in query.filters)
                await DynamicFilter.init(filter);
            return query;
        }
        public async static Task<DynamicQueryForRoute> addRouteQuery(int index, string queryString, QueryTypes IDQueryType, long RouteID, bool CompleteCheck, bool CompleteAuth)
        {
            DynamicQueryForRoute dynamicQueryForRoute = new DynamicQueryForRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRouteQuery
                        .setParam("Index", index)
                        .setParam("SQLString", queryString)
                        .setParam("QueryTypeID", (long)IDQueryType)
                        .setParam("RouteID", RouteID)
                        .setParam("CompleteCheck", CompleteCheck)
                        .setParam("CompleteAuth", CompleteAuth)
                    )
                , queryString
                , (long)IDQueryType
                , CompleteCheck
                , CompleteAuth
            );
            foreach (var variable in dynamicQueryForRoute.query.variablesInQuery)
            {
                dynamicQueryForRoute.ParamsInfos.Add(variable.Key, await DynamicSQLParamInfo.addSQLParamInfo(variable.Key, 1, dynamicQueryForRoute.id));
                await dynamicQueryForRoute.ParamsInfos[variable.Key].addValidator((variable.Value ? 0 : 1).ToString(), ValidatorTypes.REQUIRED);
            }

            return dynamicQueryForRoute;
>>>>>>> Stashed changes
        }
        public Task addFilters(string name, long ShowTypeID, string VarAffected)
        {
            return DynamicFilter.addFilters(filters, name, ShowTypeID, id, VarAffected);
        }
    }
}
