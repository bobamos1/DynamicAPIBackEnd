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
        }
        public Task addFilters(string name, long ShowTypeID, string VarAffected)
        {
            return DynamicFilter.addFilters(filters, name, ShowTypeID, id, VarAffected);
        }
    }
}
