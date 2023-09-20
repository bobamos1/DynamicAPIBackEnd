using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DynamicSQLFetcher
{
    public class SQLExecutor
    {
        private static IConfiguration _configuration;
        private string _connectionString;

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
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return SelectQuery(_connectionString, query, authorizedColumns, completeCheck);
        }
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, int page, int step, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return SelectQuery(_connectionString, query, page, step, authorizedColumns, completeCheck);
        }
        public Task<dynamic> DetailedSelectQuerySingle(Query query, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return DetailedSelectQuerySingle(_connectionString, query, mappers, authorizedColumns, completeCheck);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return DetailedSelectQuery(_connectionString, query, mappers, authorizedColumns, completeCheck);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, int page, int step, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return DetailedSelectQuery(_connectionString, query, page, step, mappers, authorizedColumns, completeCheck);
        }
        public Task<int> ExecuteQueryWithoutTransaction(Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return ExecuteQueryWithoutTransaction(_connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns, completeCheck), query.getParameters() } });
        }
        public Task<int> ExecuteQueryWithTransaction(Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return ExecuteQueryWithTransaction(_connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns, completeCheck), query.getParameters() } });
        }
        public Task<int> ExecuteQueryWithoutTransaction(List<Tuple<Query, IEnumerable<string>, bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2, query.Item3), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(_connectionString, queriesDict);
        }
        public Task<int> ExecuteQueryWithTransaction(List<Tuple<Query, IEnumerable<string>, bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2, query.Item3), query.Item1.getParameters());
            return ExecuteQueryWithTransaction(_connectionString, queriesDict);
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return SelectQuery(connectionString, query.Parse(authorizedColumns, completeCheck), query.getParameters());
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, int page, int step, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return SelectQuery(connectionString, query.Parse(authorizedColumns, page, step, completeCheck), query.getParameters());
        }
        public Task<object> ValueQuery(Query query, bool completeCheck = false)
        {
            return ValueQuery<object>(_connectionString, query, completeCheck);
        }
        public Task<T> ValueQuery<T>(Query query, bool completeCheck = false)
        {
            return ValueQuery<T>(_connectionString, query, completeCheck);
        }
        public async static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
                return await cnn.QueryAsync(query, parameters);
        }
        private async static Task getDetails(IDbConnection connection, string connectionString, dynamic obj, List<DynamicMapper> mappers)
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
        public async static Task<dynamic> DetailedSelectQuerySingle(string connectionString, Query query, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            dynamic result = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                result = await connection.QueryFirstOrDefaultAsync(query.Parse(authorizedColumns, completeCheck), query.getParameters());
                await getDetails(connection, connectionString, result, mappers);
            }
            return result;
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, int page, int step, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(authorizedColumns, page, step, completeCheck), query.getParameters(), mappers);
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, List<DynamicMapper> mappers, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(authorizedColumns, completeCheck), query.getParameters(), mappers);
        }
        private async static Task<IEnumerable<dynamic>> DetailedSelectQueryString(string connectionString, string query, DynamicParameters parameters, List<DynamicMapper> mappers)
        {
            IEnumerable<dynamic> results = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                results = await connection.QueryAsync(query, parameters);
                foreach (var result in results)
                    await getDetails(connection, connectionString, result, mappers);
            }
            return results;
        }
        public static Task<object> ValueQuery(string connectionString, Query query, bool completeCheck = false)
        {
            return ValueQuery<object>(connectionString, query.Parse(completeCheck), query.getParameters());
        }
        public static Task<T> ValueQuery<T>(string connectionString, Query query, bool completeCheck = false)
        {
            return ValueQuery<T>(connectionString, query.Parse(completeCheck), query.getParameters());
        }
        public async static Task<T> ValueQuery<T>(string connectionString, string query, DynamicParameters parameters)
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
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return ExecuteQueryWithoutTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns, completeCheck), query.getParameters() } });
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, Query query, IEnumerable<string> authorizedColumns, bool completeCheck = false)
        {
            return ExecuteQueryWithTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns, completeCheck), query.getParameters() } });
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, List<Tuple<Query, IEnumerable<string>, bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2, query.Item3), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(connectionString, queriesDict);
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, List<Tuple<Query, IEnumerable<string>, bool>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2, query.Item3), query.Item1.getParameters());
            return ExecuteQueryWithTransaction(connectionString, queriesDict);
        }
    }
}
