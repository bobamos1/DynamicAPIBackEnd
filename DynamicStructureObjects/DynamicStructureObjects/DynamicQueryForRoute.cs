using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicQueryForRoute
    {
        public long id { get; internal set; }
        public Query query { get; internal set; }
        public bool CompleteAuth { get; internal set; }
        public Dictionary<string, DynamicSQLParamInfo> ParamsInfos { get; internal set; }
        internal static readonly Query getSQLParamInfos = Query.fromQueryString(QueryTypes.SELECT, "SELECT id AS id, varAffected AS VarAffected, id_Propriety AS ProprietyID FROM SQLParamInfos WHERE id_RouteQuery = @RouteQueryID", true);
        internal static readonly Query insertRouteQuery = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO RouteQueries (ind, SQLString, id_queryType, id_route, CompleteAuth, CompleteCheck) VALUES (@Index, @SQLString, @QueryTypeID, @RouteID, @CompleteAuth, @CompleteCheck)", true);
        internal static readonly Query updateSQLParamInfo = Query.fromQueryString(QueryTypes.UPDATE, "UPDATE SQLParamInfos SET id_Propriety = @ProprietyID WHERE varAffected = @VarAffected AND id_RouteQuery = @RouteQueryID", true, false);
        internal static readonly Query deleteSQLParamInfoRequired = Query.fromQueryString(QueryTypes.DELETE, "DELETE ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @ID AND id_ValidatorType = 1", true);
        internal static readonly Query deleteSQLParamInfoValidators = Query.fromQueryString(QueryTypes.DELETE, "DELETE ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @ID", true);
        internal static readonly Query deleteSQLParamInfo = Query.fromQueryString(QueryTypes.DELETE, "DELETE SQLParamInfos WHERE id = @ID", true);
        private string lastSQLParamAdded;
        internal DynamicQueryForRoute(long id, string queryString, long QueryTypeID, bool CompleteAuth, bool CompleteCheck)
        {
            this.id = id;
            this.query = Query.fromQueryString((QueryTypes)QueryTypeID, queryString, CompleteAuth, CompleteCheck);
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
            this.CompleteAuth = CompleteAuth;
        }
        private DynamicQueryForRoute()
        {
            this.id = -1;
            this.query = new Query();
            this.ParamsInfos = new Dictionary<string, DynamicSQLParamInfo>();
            this.CompleteAuth = true;
        }
        internal DynamicQueryForRoute(DynamicQueryForRoute dynamicQuery, bool requiredID)
        {
            this.id = dynamicQuery.id;
            this.query = dynamicQuery.query;
            this.ParamsInfos = dynamicQuery.ParamsInfos.ToDictionary(param => param.Key, param => param.Value);
            this.CompleteAuth = dynamicQuery.CompleteAuth;
            DynamicSQLParamInfo paramInfo;
            if (requiredID && ParamsInfos.TryGetValue("ID", out paramInfo))
                ParamsInfos["ID"] = new DynamicSQLParamInfo(paramInfo, true);
        }
        internal static async Task<DynamicQueryForRoute> init(DynamicQueryForRoute query)
        {
            foreach (var paramInfo in await DynamicController.executor.SelectQuery<DynamicSQLParamInfo>(
                                                getSQLParamInfos
                                                    .setParam("RouteQueryID", query.id)
                                                ))
                query.ParamsInfos.Add(paramInfo.VarAffected, await DynamicSQLParamInfo.init(paramInfo));
            return query;
        }
        public static DynamicQueryForRoute addEmptyQuery()
        {
            return new DynamicQueryForRoute();
        }
        public async static Task<DynamicQueryForRoute> addRouteQuery(int index, string queryString, QueryTypes QueryType, long RouteID, bool CompleteAuth, bool CompleteCheck, bool withVar)
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
            if (withVar)
            {
                foreach (var variable in dynamicQueryForRoute.query.variablesInQuery)
                {
                    dynamicQueryForRoute.ParamsInfos.Add(variable.Key, await DynamicSQLParamInfo.addSQLParam(variable.Key, 1, dynamicQueryForRoute.id));
                    if (!variable.Value)
                        await dynamicQueryForRoute.ParamsInfos[variable.Key].addValidator("true", ValidatorTypes.REQUIRED);
                }
            }
            return dynamicQueryForRoute;
            
        }
        public async Task<DynamicQueryForRoute> addSQLParam(string varAffected, long ProprietyID)
        {
            ParamsInfos.Add(varAffected, await DynamicSQLParamInfo.addSQLParam(varAffected, ProprietyID, id));
            lastSQLParamAdded = varAffected;
            return this;
        }
        public Task<DynamicQueryForRoute> addSQLParam(string varAffected)
        {
            return addSQLParam(varAffected, 1);
        }
        public async Task<DynamicQueryForRoute> addValidator(string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await ParamsInfos[VarAffected].addValidator(Value, ValidatorType);
            return this;
        }
        public async Task<DynamicQueryForRoute> addValidator(string VarAffected, bool addRequired, params ValidatorBundle[] validatorBundles)
        {
            await ParamsInfos[VarAffected].addValidator(addRequired, validatorBundles);
            return this;
        }
        public Task<DynamicQueryForRoute> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            if (lastSQLParamAdded is null)
                throw new Exception();
            return addValidator(lastSQLParamAdded, Value, ValidatorType);
        }
        public async Task<DynamicQueryForRoute> setNotRequired(params string[] VarsAffected)
        {
            foreach (var VarAffected in VarsAffected)
            {
                var modifiedParam = ParamsInfos[VarAffected];
                await DynamicController.executor.ExecuteQueryWithTransaction(deleteSQLParamInfoRequired.setParam("ID", modifiedParam.id));
                modifiedParam.isRequired = false;
            }
            return this;
        }
        public async Task<DynamicQueryForRoute> removeParams(params string[] VarsAffected)
        {
            foreach (var VarAffected in VarsAffected)
            {
                var modifiedParam = ParamsInfos[VarAffected];
                await DynamicController.executor.ExecuteQueryWithTransaction(deleteSQLParamInfoValidators.setParam("ID", modifiedParam.id));
                await DynamicController.executor.ExecuteQueryWithTransaction(deleteSQLParamInfo.setParam("ID", modifiedParam.id));
                ParamsInfos.Remove(VarAffected);
            }
            return this;
        }
        public async Task<DynamicQueryForRoute> setSQLParam(string VarAffected, long ProprietyID, bool updatePropriety, params ValidatorBundle[] ValidatorBundles)
        {
            if (updatePropriety)
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
