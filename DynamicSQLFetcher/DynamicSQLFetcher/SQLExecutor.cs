using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;

namespace DynamicSQLFetcher
{
    public static class EnumerableHelper
    {
        public static IEnumerable<KeyValuePair<TK, TV>> toOrderedPairs<T, TK, TV>(this IEnumerable<T> enumerable, Func<T, TK> getKey, Func<T, TV> getValue)
        {
            return enumerable.Select(value => new KeyValuePair<TK, TV>(getKey(value), getValue(value)));
        }
        public static IEnumerable<KeyValuePair<string, DynamicParameters>> toOrderedPairs(this IEnumerable<Query> queries)
        {
            return queries.toOrderedPairs(query => query.Parse(), query => query.getParameters());
        }
        public static IEnumerable<KeyValuePair<string, DynamicParameters>> getDictionaryToRun<TS, TM>(this Query query, string paramNameChanging, string paramNameStatic, TS staticValue, IEnumerable<TM> multiplesValues)
        {
            query.clearParams();
            return multiplesValues.toOrderedPairs(value => query.setParam(paramNameChanging, value).setParam(paramNameStatic, staticValue).Parse(), _ => query.getParameters());
        }
    }
    public class SQLExecutor
    {
        public static readonly string VALUE_FOR_CBO = "VALUE_FOR_CBO";
        private static IConfiguration _configuration;
        private string _connectionString;
        internal class SingleValueItem
        {
            internal object item;
        }
        internal class SingleValueItem<T>
        {
            internal T item;
        }
        public class KeyValueItem
        {
            internal object key;
            internal object value;
        }
        public class KeyValueItem<K, V>
        {
            internal K key;
            internal V value;
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
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, params string[] authorizedColumns)
        {
            return SelectQuery(_connectionString, query, authorizedColumns);
        }
        public Task<IEnumerable<T>> SelectQuery<T>(Query query, params string[] authorizedColumns)
        {
            return SelectQuery<T>(_connectionString, query, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> SelectQuery(Query query, int page, int step, string orderCol, bool isAscending, params string[] authorizedColumns)
        {
            return SelectQuery(_connectionString, query, page, step, orderCol, isAscending, authorizedColumns);
        }
        public Task<IEnumerable<T>> SelectQuery<T>(Query query, int page, int step, string orderCol, bool isAscending, params string[] authorizedColumns)
        {
            return SelectQuery<T>(_connectionString, query, page, step, orderCol, isAscending, authorizedColumns);
        }/*
        public Task<IEnumerable<T>> SelectQueryTotal<T>(Query query)
        {
            return SelectQuery<T>(_connectionString, query.ParseTotalAuth(), query.getParameters());
        }*/
        public Task<dynamic> DetailedSelectQuerySingle(Query query, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            return DetailedSelectQuerySingle(_connectionString, query, mappers, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            return DetailedSelectQuery(_connectionString, query, mappers, authorizedColumns);
        }
        public Task<IEnumerable<dynamic>> DetailedSelectQuery(Query query, int page, int step, string orderCol, bool isAscending, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            return DetailedSelectQuery(_connectionString, query, page, step, orderCol, isAscending, mappers, authorizedColumns);
        }
        public Task<int> ExecuteQueryWithoutTransaction(Query query, params string[] authorizedColumns)
        {
            return ExecuteQueryWithoutTransaction(_connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns), query.getParameters() } });
        }
        public Task<int> ExecuteQueryWithTransaction(Query query, params string[] authorizedColumns)
        {
            return ExecuteQueryWithTransaction(_connectionString, new KeyValuePair<string, DynamicParameters>[] { new KeyValuePair<string, DynamicParameters>(query.Parse(authorizedColumns), query.getParameters()) });
        }
        public Task<int> ExecuteQueryWithoutTransaction(List<Tuple<Query, string[]>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(_connectionString, queriesDict);
        }
        public Task<int> ExecuteQueryWithTransaction(IEnumerable<KeyValuePair<Query, string[]>> queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries.toOrderedPairs(query => query.Key.Parse(query.Value), query => query.Key.getParameters()));
        }
        public Task<int> ExecuteQueryWithTransaction(params Query[] queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries.toOrderedPairs(query => query.Parse(), query => query.getParameters()));
        }
        public Task<int> ExecuteQueryWithTransaction(params KeyValuePair<Query, string[]>[] queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries.toOrderedPairs(query => query.Key.Parse(query.Value), query => query.Key.getParameters()));
        }
        public Task<int> ExecuteQueryWithTransaction(string[] authorizedColumns, params Query[] queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries.toOrderedPairs(query => query.Parse(authorizedColumns), query => query.getParameters()));
        }
        public Task<int> ExecuteQueryWithTransaction(params string[] queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries.toOrderedPairs(query => query, _ => new DynamicParameters()));
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, params string[] authorizedColumns)
        {
            return SelectQuery(connectionString, query.Parse(authorizedColumns), query.getParameters());
        }
        public static Task<IEnumerable<T>> SelectQuery<T>(string connectionString, Query query, params string[] authorizedColumns)
        {
            return SelectQuery<T>(connectionString, query.Parse(authorizedColumns), query.getParameters());
        }
        public static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, Query query, int page, int step, string orderCol, bool isAscending, params string[] authorizedColumns)
        {
            return SelectQuery(connectionString, query.Parse(page, step, orderCol, isAscending, authorizedColumns), query.getParameters());
        }
        public static Task<IEnumerable<T>> SelectQuery<T>(string connectionString, Query query, int page, int step, string orderCol, bool isAscending, params string[] authorizedColumns)
        {
            return SelectQuery<T>(connectionString, query.Parse(page, step, orderCol, isAscending, authorizedColumns), query.getParameters());
        }
        public Task<object> SelectValue(Query query)
        {
            return SelectValue<object>(_connectionString, query);
        }
        public Task<T> SelectValue<T>(Query query)
        {
            return SelectValue<T>(_connectionString, query);
        }
        public Task<object> SelectValue(Query query, string valueCol)
        {
            return SelectValue<object>(_connectionString, query, valueCol);
        }
        public Task<T> SelectValue<T>(Query query, string valueCol)
        {
            return SelectValue<T>(_connectionString, query, valueCol);
        }
        public Task<Dictionary<object, object>> SelectDictionary(Query query, string idCol, string valueCol)
        {
            return SelectDictionary<object, object>(_connectionString, query.Parse(idCol, valueCol), query.getParameters());
        }
        public Task<Dictionary<object, object>> SelectDictionary(Query query)
        {
            return SelectDictionary<object, object>(_connectionString, query.Parse(null), query.getParameters());
        }
        public Task<Dictionary<K, V>> SelectDictionary<K, V>(Query query, string idCol, string valueCol)
        {
            return SelectDictionary<K, V>(_connectionString, query.Parse(idCol, valueCol), query.getParameters());
        }
        public Task<Dictionary<K, V>> SelectDictionary<K, V>(Query query)
        {
            return SelectDictionary<K, V>(_connectionString, query.Parse(null), query.getParameters());
        }
        public Task<IEnumerable<object>> SelectArray(Query query)
        {
            return SelectArray<object>(_connectionString, query.Parse(null), query.getParameters());
        }
        public Task<IEnumerable<object>> SelectArray(Query query, string colAuthorized)
        {
            return SelectArray<object>(_connectionString, query.Parse(colAuthorized), query.getParameters());
        }
        public Task<IEnumerable<T>> SelectArray<T>(Query query)
        {
            return SelectArray<T>(_connectionString, query.Parse(null), query.getParameters());
        }
        public Task<IEnumerable<T>> SelectArray<T>(Query query, string colAuthorized)
        {
            return SelectArray<T>(_connectionString, query.Parse(colAuthorized), query.getParameters());
        }
        public Task<dynamic> SelectSingle(Query query, params string[] authorizedColumns)
        {
            return SelectSingle<dynamic>(_connectionString, query, authorizedColumns);
        }
        public Task<T> SelectSingle<T>(Query query, params string[] authorizedColumns) where T : class
        {
            return SelectSingle<T>(_connectionString, query, authorizedColumns);
        }/*
        public Task<T> SelectSingleTotal<T>(Query query) where T : class
        {
            return SelectSingle<T>(_connectionString, query.ParseTotalAuth(), query.getParameters());
        }*/
        public Task<long> ExecuteInsertWithLastID(Query query)
        {
            return ExecuteInsertWithLastID(_connectionString, query.Parse(), query.getParameters());
        }
        public Task<int> ExecuteQueryWithTransaction(IEnumerable<KeyValuePair<string, DynamicParameters>> queries)
        {
            return ExecuteQueryWithTransaction(_connectionString, queries);
        }
        public async static Task<IEnumerable<T>> SelectQuery<T>(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
                return await cnn.QueryAsync<T>(query, parameters);
        }
        public async static Task<IEnumerable<dynamic>> SelectQuery(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
                return await cnn.QueryAsync(query, parameters);
        }
        private async static Task getDetails(IDbConnection con, IDictionary<string, object> dict, IEnumerable<DynamicMapper> mappers)
        {
            foreach (var mapper in mappers)
            {
                if (mapper.query == null)
                    //dict[mapper.propetyName] = Array.Empty<object>();
                    continue;
                DynamicParameters parameters = mapper.getParameters();
                foreach (var baseParam in mapper.parametersToLink)
                    parameters.Add(baseParam.Key, dict[baseParam.Value]);
                dict[mapper.propetyName] = await con.QueryAsync(mapper.query, parameters);
            }
        }
        public async static Task<dynamic> DetailedSelectQuerySingle(string connectionString, Query query, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            dynamic result = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                result = await connection.QueryFirstOrDefaultAsync(query.Parse(authorizedColumns), query.getParameters());
                if (result is null)
                    return null;
                await getDetails(connection, result as IDictionary<string, object>, mappers);
            }
            return result;
        }
        public static Task<dynamic> SelectSingle(string connectionString, Query query, params string[] authorizedColumns)
        {
            return SelectSingle<dynamic>(connectionString, query, authorizedColumns);
        }
        public static Task<T> SelectSingle<T>(string connectionString, Query query, params string[] authorizedColumns) where T : class
        {
            return SelectSingle<T>(connectionString, query.Parse(authorizedColumns), query.getParameters());
        }
        public async static Task<T> SelectSingle<T>(string connectionString, string query, DynamicParameters parameters) where T : class
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, int page, int step, string orderCol, bool isAscending, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(page, step, orderCol, isAscending, authorizedColumns), query.getParameters(), mappers);
        }
        public static Task<IEnumerable<dynamic>> DetailedSelectQuery(string connectionString, Query query, IEnumerable<DynamicMapper> mappers, params string[] authorizedColumns)
        {
            return DetailedSelectQueryString(connectionString, query.Parse(authorizedColumns), query.getParameters(), mappers);
        }
        private async static Task<IEnumerable<dynamic>> DetailedSelectQueryString(string connectionString, string query, DynamicParameters parameters, IEnumerable<DynamicMapper> mappers)
        {
            IEnumerable<dynamic> results = null;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                results = await connection.QueryAsync(query, parameters);
                var dicts = results as IEnumerable<IDictionary<string, object>>;
                foreach (var result in results)
                    await getDetails(connection, result, mappers);
            }
            return results;
        }
        public static Task<object> SelectValue(string connectionString, Query query)
        {
            return SelectValue<object>(connectionString, query.Parse(null), query.getParameters());
        }
        public static Task<T> SelectValue<T>(string connectionString, Query query)
        {
            return SelectValue<T>(connectionString, query.Parse(null), query.getParameters());
        }
        public static Task<object> SelectValue(string connectionString, Query query, string valueCol)
        {
            return SelectValue<object>(connectionString, query.Parse(valueCol), query.getParameters());
        }
        public static Task<T> SelectValue<T>(string connectionString, Query query, string valueCol)
        {
            return SelectValue<T>(connectionString, query.Parse(valueCol), query.getParameters());
        }
        public async static Task<T> SelectValue<T>(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
                return await connection.ExecuteScalarAsync<T>(query, parameters);
        }
        public async static Task<int> ExecuteQueryWithTransaction(string connectionString, IEnumerable<KeyValuePair<string, DynamicParameters>> queries)
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
                        recordsUpdated = 0;
                        trans.Rollback();
                    }
                }
            }
            return recordsUpdated;
        }
        public static Task<long> ExecuteInsertWithLastID(string connectionString, Query query)
        {
            return ExecuteInsertWithLastID(connectionString, query.Parse(), query.getParameters());
        }
        public async static Task<long> ExecuteInsertWithLastID(string connectionString, string query, DynamicParameters parameters)
        {
            long lastInserted = 0;
            
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())   
                {
                    try
                    {
                        lastInserted = await connection.QuerySingleAsync<long>(query + "; SELECT CAST(SCOPE_IDENTITY() as INT)", parameters, trans);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        lastInserted = 0;
                        trans.Rollback();
                    }
                }
            }
            return lastInserted;
        }
        public Task ExecuteTransaction(Func<IDbConnection, IDbTransaction, Task<bool>> action)
        {
            return ExecuteTransaction(_connectionString, action);
        }
        public async static Task ExecuteTransaction(string connectionString, Func<IDbConnection, IDbTransaction, Task<bool>> action)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        if (await action(connection, trans))
                            trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }
        }
        public static Task<int> InsertWithLastID(IDbConnection connection, IDbTransaction transaction, Query query)
        {
            return connection.QuerySingleAsync<int>(query.Parse(), query.getParameters(), transaction);
        }
        public static Task<int> InsertWithLastID(IDbConnection connection, IDbTransaction transaction, string query, DynamicParameters parameters)
        {
            return connection.QuerySingleAsync<int>(query + "; SELECT CAST(SCOPE_IDENTITY() as INT)", parameters, transaction);
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
            return ExecuteQueryWithTransaction(connectionString, new KeyValuePair<string, DynamicParameters>[] { new KeyValuePair<string, DynamicParameters>(query, parameter) });
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, Query query, params string[] authorizedColumns)
        {
            return ExecuteQueryWithoutTransaction(connectionString, new Dictionary<string, DynamicParameters>() { { query.Parse(authorizedColumns), query.getParameters() } });
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, Query query, params string[] authorizedColumns)
        {
            return ExecuteQueryWithTransaction(connectionString, new KeyValuePair<string, DynamicParameters>[] { new(query.Parse(authorizedColumns), query.getParameters())});
        }
        public static Task<int> ExecuteQueryWithoutTransaction(string connectionString, List<Tuple<Query, string[]>> queries)
        {
            Dictionary<string, DynamicParameters> queriesDict = new Dictionary<string, DynamicParameters>();
            foreach (var query in queries)
                queriesDict.Add(query.Item1.Parse(query.Item2), query.Item1.getParameters());
            return ExecuteQueryWithoutTransaction(connectionString, queriesDict);
        }
        public static Task<int> ExecuteQueryWithTransaction(string connectionString, IEnumerable<KeyValuePair<Query, string[]>> queries)
        {
            return ExecuteQueryWithTransaction(connectionString, queries.toOrderedPairs(query => query.Key.Parse(query.Value), query => query.Key.getParameters()));
        }
        public static Task<Dictionary<object, object>> SelectDictionary(string connectionString, Query query, string idCol, string valueCol)
        {
            return SelectDictionary<object, object>(connectionString, query.Parse(idCol, valueCol), query.getParameters());
        }
        public static Task<Dictionary<object, object>> SelectDictionary(string connectionString, Query query)
        {
            return SelectDictionary<object, object>(connectionString, query.Parse(null), query.getParameters());
        }
        public static Task<Dictionary<K, V>> SelectDictionary<K,V>(string connectionString, Query query, string idCol, string valueCol)
        {
            return SelectDictionary<K, V>(connectionString, query.Parse(idCol, valueCol), query.getParameters());
        }
        public static Task<Dictionary<K, V>> SelectDictionary<K, V>(string connectionString, Query query)
        {
            return SelectDictionary<K, V>(connectionString, query.Parse(null), query.getParameters());
        }
        public async static Task<Dictionary<K, V>> SelectDictionary<K, V>(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<KeyValueItem<K,V>>(query, parameters)).ToDictionary(pair => pair.key, pair => pair.value);
            }
        }
        public static Task<IEnumerable<object>> SelectArray(string connectionString, Query query)
        {
            return SelectArray<object>(connectionString, query.Parse(null), query.getParameters());
        }
        public static Task<IEnumerable<object>> SelectArray(string connectionString, Query query, string colAuthorized)
        {
            return SelectArray<object>(connectionString, query.Parse(colAuthorized), query.getParameters());
        }
        public static Task<IEnumerable<T>> SelectArray<T>(string connectionString, Query query)
        {
            return SelectArray<T>(connectionString, query.Parse(null), query.getParameters());
        }
        public static Task<IEnumerable<T>> SelectArray<T>(string connectionString, Query query, string colAuthorized)
        {
            return SelectArray<T>(connectionString, query.Parse(colAuthorized), query.getParameters());
        }
        public async static Task<IEnumerable<T>> SelectArray<T>(string connectionString, string query, DynamicParameters parameters)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<SingleValueItem<T>>(query, parameters)).Select(val => val.item);
            }
        }
    }
}
