using DynamicSQLFetcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicStructureObjects
{
    public static class NewExtensions
    {
        public static RouteHandlerBuilder MapRoute(this WebApplication app, RouteTypes routeType, string routePath, Func<HttpRequest, Task<IResult>> delegateMethod)
        {
            switch (routeType)
            {
                case RouteTypes.GET:
                    return app.MapGet(routePath, delegateMethod);
                case RouteTypes.POST:
                    return app.MapPost(routePath, delegateMethod);
                case RouteTypes.PUT:
                    return app.MapPut(routePath, delegateMethod);
                case RouteTypes.DELETE:
                    return app.MapDelete(routePath, delegateMethod);
                default:
                    throw new ArgumentException("Invalid routeType");
            }
        }
        public static List<KeyValuePair<TK, TV>> Add<TK, TV>(this List<KeyValuePair<TK, TV>> dict, TK key, TV value)
        {
            dict.Add(new KeyValuePair<TK, TV>(key, value));
            return dict;
        }
        public static long UserID(this Dictionary<string, object> data)
        {
            object result;
            if (data.TryGetValue(DynamicController.USERIDKEY, out result))
                return (long)result;
            return DynamicPropriety.AnonymousRoleID;
        }
        public static long[] UserRoles(this Dictionary<string, object> data)
        {
            object result;
            if (data.TryGetValue(DynamicController.ROLESKEY, out result))
                return (long[])result;
            return new long[0];
        }
        public static string[] AuthProprieties(this Dictionary<string, object> data)
        {
            object result;
            if (data.TryGetValue(DynamicController.PROPRETYKEY, out result))
                return (string[])result;
            return new string[0];
        }
        public static T Get<T>(this Dictionary<string, object> data, string key)
        {
            return (T)data[key];
        }
        public static T SafeGet<T>(this Dictionary<string, object> data, string key, T defaultVal = default(T))
        {
            object result;
            if (data.TryGetValue(key, out result))
                return (T)result;
            return defaultVal;
        }
        public static readonly QueryTypes[] authQueries = new[] { QueryTypes.INSERT, QueryTypes.DELETE }; 
        public static bool CompleteAuth(this QueryTypes queryType)
        {
            return authQueries.Contains(queryType);
        }
    }
}
