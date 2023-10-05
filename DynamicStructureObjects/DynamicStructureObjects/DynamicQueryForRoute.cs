using DynamicSQLFetcher;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using System;

namespace DynamicStructureObjects
{
    public class DynamicQueryForRoute
    {
        public long id { get; internal set; }
        public Query query { get; internal set; }
        public bool CompleteAuth { get; internal set; }
        public Dictionary<string, DynamicSQLParamInfo> ParamsInfos { get; internal set; }
        public List<DynamicFilter> Filters { get; internal set; }
        internal static readonly Query getFilters = Query.fromQueryString(QueryTypes.SELECT, "SELECT Filters.id AS id, name AS Name, SQLParamInfos.varAffected AS VarAffected FROM Filters INNER JOIN SQLParamInfos ON SQLParamInfos.id = id_SQLParamInfo WHERE id_RouteQuery = @routeQueryID ORDER BY name, ind", true);
        internal static readonly Query getSQLParamInfos = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, varAffected AS VarAffected, id_Propriety AS ProprietyID FROM SQLParamInfos WHERE id_RouteQuery = @RouteQueryID", true);
        internal static readonly Query insertRouteQuery = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO RouteQueries (ind, SQLString, id_queryType, id_route, CompleteAuth, CompleteCheck) VALUES (@Index, @SQLString, @QueryTypeID, @RouteID, @CompleteAuth, @CompleteCheck)", true);
        internal static readonly Query updateSQLParamInfo = Query.fromQueryString(QueryTypes.UPDATE, "UPDATE SQLParamInfos SET id_Propriety = @ProprietyID WHERE varAffected = @VarAffected AND id_RouteQuery = @RouteQueryID", true);
        private string lastSQLParamAdded;
        internal DynamicQueryForRoute(long id, string queryString, long QueryTypeID, bool CompleteAuth, bool CompleteCheck)
        {
            this.id = id;
            this.query = Query.fromQueryString((QueryTypes)QueryTypeID, queryString, CompleteAuth, CompleteCheck);
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
            this.CompleteAuth = CompleteAuth;
            this.Filters = new List<DynamicFilter>();
        }
        private DynamicQueryForRoute()
        {
            this.id = -1;
            this.query = new Query();
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
            this.CompleteAuth = true;
            this.Filters = new List<DynamicFilter>();
        }
        internal static async Task<DynamicQueryForRoute> init(DynamicQueryForRoute query)
        {
            foreach (var paramInfo in await DynamicController.executor.SelectQuery<DynamicSQLParamInfo>(
                                                getSQLParamInfos
                                                    .setParam("RouteQueryID", query.id)
                                                ))
                query.ParamsInfos.Add(paramInfo.VarAffected, paramInfo);
            query.Filters = 
                (await DynamicController.executor.SelectQuery<DynamicFilter>(
                    getFilters
                        .setParam("routeQueryID", query.id)
                    )
                ).ToList();
            foreach (var filter in query.Filters)
                await DynamicFilter.init(filter);
            if (!query.query.variablesInQuery.All(variable => query.ParamsInfos.ContainsKey(variable.Key)))
                throw new Exception($"There are variables in Query not in ParamInfo for query {query.id}");
            return query;
        }
        public static DynamicQueryForRoute addEmptyQuery()
        {
            return new DynamicQueryForRoute();
        }
        public async static Task<DynamicQueryForRoute> addRouteQuery(int index, string queryString, QueryTypes QueryType, long RouteID, bool CompleteAuth, bool CompleteCheck)
        {
            var dynamicQueryForRoute = new DynamicQueryForRoute(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertRouteQuery
                        .setParam("Index", index)
                        .setParam("SQLString", queryString)
                        .setParam("QueryTypeID", (long)QueryType)
                        .setParam("RouteID", RouteID)
                        .setParam("CompleteAuth", CompleteAuth)
                        .setParam("CompleteCheck", CompleteCheck)

                    )
                , queryString
                , (long)QueryType
                , CompleteAuth
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
            Filters.Add(await DynamicFilter.addFilter(Filters.Count, name, showType, ParamsInfos[VarAffected].id, VarAffected));
            return this;
        }
        public async Task<DynamicQueryForRoute> addSQLParamInfo(string varAffected, long ProprietyID)
        {
            ParamsInfos.Add(varAffected, await DynamicSQLParamInfo.addSQLParamInfo(varAffected, ProprietyID, id));
            lastSQLParamAdded = varAffected;
            return this;
        }
        public Task<DynamicQueryForRoute> addSQLParamInfo(string varAffected)
        {
            return addSQLParamInfo(varAffected, 1);
        }
        public async Task<DynamicQueryForRoute> addValidator(string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await ParamsInfos[VarAffected].addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicQueryForRoute> addValidator(string VarAffected, params ValidatorBundle[] validatorBundles)
        {
            await ParamsInfos[VarAffected].addValidator(validatorBundles);
            return this;
        }
        public Task<DynamicQueryForRoute> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            if (lastSQLParamAdded is null)
                throw new Exception();
            return addValidator(lastSQLParamAdded, Value, ValidatorType);
        }
        public async Task<DynamicQueryForRoute> setValidator(string VarAffected, long ProprietyID, params ValidatorBundle[] ValidatorBundles)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(
                updateSQLParamInfo
                    .setParam("ProprietyID", ProprietyID)
                    .setParam("VarAffected", VarAffected)
                    .setParam("RouteQueryID", id)
            );
            foreach (var bundle in ValidatorBundles)
                await ParamsInfos[VarAffected].addValidator(bundle);
            lastSQLParamAdded = VarAffected;
            return this;
        }
        internal bool validateParams(Dictionary<string, object> bodyData)
        {
            return ParamsInfos.All(param =>
            {
                object val;
                if (!bodyData.TryGetValue(param.Key, out val))
                    return !param.Value.isRequired;
                return param.Value.validateParam(val);
            });
        }
    }
}
