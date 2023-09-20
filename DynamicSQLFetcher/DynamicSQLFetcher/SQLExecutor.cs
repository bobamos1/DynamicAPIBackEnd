using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;

namespace DynamicSQLFetcher
{
    public class SQLExecutor
    {
        private static IConfiguration _configuration;
        private string _connectionString;
        internal class SingleValueItem
        {
            internal object item;
        }
        public class KeyValueItem
        {
            internal object key;
            internal object value;
        }
        public SQLExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return SelectQuery(_connectionString, query, completeCheck, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, int page, int step, bool completeCheck, params string[] authorizedColumns)
        {
            return SelectQuery(_connectionString, query, page, step, completeCheck, authorizedColumns);
        }
        public Task<dynamic> DetailedSelectQuerySingle(Query query, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            return DetailedSelectQuerySingle(_connectionString, query, mappers, completeCheck, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            return DetailedSelectQuery(_connectionString, query, mappers, completeCheck, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, int page, int step, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            return DetailedSelectQuery(_connectionString, query, page, step, mappers, completeCheck, authorizedColumns);
        }
        public Task<int> ExecuteQueryWithoutTransaction(Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return ExecuteQueryWithoutTransaction(_connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(completeCheck, authorizedColumns), query.getParameters() } });
        }
        public Task<int> ExecuteQueryWithTransaction(Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return ExecuteQueryWithTransaction(_connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(completeCheck, authorizedColumns), query.getParameters() } });
        }
        public Task<int> ExecuteQueryWithoutTransaction(List<Tuple<Query, string[], bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item3, query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(_connectionString, queriesDict);
        }
        public Task<int> ExecuteQueryWithTransaction(List<Tuple<Query, string[], bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item3, query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithTransaction(_connectionString, queriesDict);
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return SelectQuery(connectionString, query.Parse(completeCheck, authorizedColumns), query.getParameters());
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, int page, int step, bool completeCheck, params string[] authorizedColumns)
        {
            return SelectQuery(connectionString, query.Parse(completeCheck, page, step, authorizedColumns), query.getParameters());
        }
        public Task<object> SelectValue(Query query, bool completeCheck)
        {
            return SelectValue<object>(_connectionString, query, completeCheck);
        }
        public Task<T> SelectValue<T>(Query query, bool completeCheck)
        {
            return SelectValue<T>(_connectionString, query, completeCheck);
        }
        public Task<object> SelectValue(Query query, bool completeCheck, string valueCol)
        {
            return SelectValue<object>(_connectionString, query, completeCheck, valueCol);
        }
        public Task<T> SelectValue<T>(Query query, bool completeCheck, string valueCol)
        {
            return SelectValue<T>(_connectionString, query, completeCheck, valueCol);
        }
        public Task<Dictionary<object, object>> SelectDictionary(Query query, bool completeCheck, string idCol, string valueCol)
        {
            return SelectDictionary(_connectionString, query.Parse(completeCheck, idCol, valueCol), query.getParameters());
        }
        public Task<Dictionary<object, object>> SelectDictionary(Query query, bool completeCheck)
        {
            return SelectDictionary(_connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public Task<IEnumerable<object>> SelectArray(Query query, bool completeCheck)
        {
            return SelectArray(_connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public Task<IEnumerable<object>> SelectArray(Query query, bool completeCheck, string colAuthorized)
        {
            return SelectArray(_connectionString, query.Parse(completeCheck, colAuthorized), query.getParameters());
        }
        public async static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
                return await cnn.QueryAsync(query, parameters);
        }
        private async static Task getDetails(string connectionString, dynamic obj, List<DynamicMapper> mappers)
        {
            using (IDbConnection con = new SqlConnection(connectionString))
            {
                foreach (var mapper in mappers)
                {
                    var dict = obj as IDictionary<string, object>;
                    mapper.queryLinked.clearParams();
                    foreach (var baseParam in mapper.parametersToLink)
                        mapper.queryLinked.addParams(baseParam.Key, dict[baseParam.Value]);
                    foreach (var baseParam in mapper.baseParameters)
                        mapper.queryLinked.addParams(baseParam.Key, baseParam.Value);
                    dict[mapper.propetyName] = await con.QueryAsync(mapper.queryLinked.Parse(mapper.completeCheck), mapper.queryLinked.getParameters());
                }
            }
        }
        public async static Task<dynamic> DetailedSelectQuerySingle(string connectionString, Query query, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            dynamic result = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                result = await connection.QueryFirstOrDefaultAsync(query.Parse(completeCheck, authorizedColumns), query.getParameters());
                await getDetails(connectionString, result, mappers);
            }
            return result;
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, int page, int step, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(completeCheck, page, step, authorizedColumns), query.getParameters(), mappers);
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, List<DynamicMapper> mappers, bool completeCheck, params string[] authorizedColumns)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(completeCheck, authorizedColumns), query.getParameters(), mappers);
        }
        private async static Task<IEnumerable<dynamic>> DetailedSelectQueryString(string connectionString, string query, DynamicParameters parameters, List<DynamicMapper> mappers)
        {
            IEnumerable<dynamic> results = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                results = await connection.QueryAsync(query, parameters);
                foreach (var result in results)
                    await getDetails(connectionString, result, mappers);
            }
            return results;
        }
        public static Task<object> SelectValue(string connectionString, Query query, bool completeCheck)
        {
            return SelectValue<object>(connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public static Task<T> SelectValue<T>(string connectionString, Query query, bool completeCheck)
        {
            return SelectValue<T>(connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public static Task<object> SelectValue(string connectionString, Query query, bool completeCheck, string valueCol)
        {
            return SelectValue<object>(connectionString, query.Parse(completeCheck, valueCol), query.getParameters());
        }
        public static Task<T> SelectValue<T>(string connectionString, Query query, bool completeCheck, string valueCol)
        {
            return SelectValue<T>(connectionString, query.Parse(completeCheck, valueCol), query.getParameters());
        }
        public async static Task<T> SelectValue<T>(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
                return await connection.ExecuteScalarAsync<T>(query, parameters);
        }
        public async static Task<int> ExecuteQueryWithTransaction(string connectionString, Dictionary<string, DynamicParameters> queries)
        {
            int recordsUpdated = 0;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var query in queries)
                            recordsUpdated += await connection.ExecuteAsync(query.Key, query.Value, trans);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }
            return recordsUpdated;
        }
        public async static Task<int> ExecuteQueryWithoutTransaction(string connectionString, Dictionary<string, DynamicParameters> queries)
        {
            int recordsUpdated = 0;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    foreach (var query in queries)
                        recordsUpdated += await connection.ExecuteAsync(query.Key, query.Value);
                }
                catch (Exception ex)
                {
                }
            }
            return recordsUpdated;
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, string query, DynamicParameters parameter)
        {
            return ExecuteQueryWithoutTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query, parameter } });
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, string query, DynamicParameters parameter)
        {
            return ExecuteQueryWithTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query, parameter } });
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return ExecuteQueryWithoutTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(completeCheck, authorizedColumns), query.getParameters() } });
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, Query query, bool completeCheck, params string[] authorizedColumns)
        {
            return ExecuteQueryWithTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(completeCheck, authorizedColumns), query.getParameters() } });
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, List<Tuple<Query, string[], bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item3, query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(connectionString, queriesDict);
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, List<Tuple<Query, string[], bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item3, query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithTransaction(connectionString, queriesDict);
        }
        public static Task<Dictionary<object, object>> SelectDictionary(string connectionString, Query query, bool completeCheck, string idCol, string valueCol)
        {
            return SelectDictionary(connectionString, query.Parse(completeCheck, idCol, valueCol), query.getParameters());
        }
        public static Task<Dictionary<object, object>> SelectDictionary(string connectionString, Query query, bool completeCheck)
        {
            return SelectDictionary(connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public async static Task<Dictionary<object, object>> SelectDictionary(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<KeyValueItem>(query, parameters)).ToDictionary(pair => pair.key, pair => pair.value);
            }
        }
        public static Task<IEnumerable<object>> SelectArray(string connectionString, Query query, bool completeCheck)
        {
            return SelectArray(connectionString, query.Parse(completeCheck, null), query.getParameters());
        }
        public static Task<IEnumerable<object>> SelectArray(string connectionString, Query query, bool completeCheck, string colAuthorized)
        {
            return SelectArray(connectionString, query.Parse(completeCheck, colAuthorized), query.getParameters());
        }
        public async static Task<IEnumerable<object>> SelectArray(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var wtf = (await connection.QueryAsync<SingleValueItem>(query, parameters));
                return wtf.Select(val => val.item);
            }
        }

    }
}
