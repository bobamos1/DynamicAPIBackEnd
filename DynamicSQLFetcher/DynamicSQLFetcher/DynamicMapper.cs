using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQLFetcher
{
    public class DynamicMapper
    {
        internal string propetyName { get; set; }
        internal string query { get; set; }
        internal Dictionary<string, object> baseParameters { get; set; }
        internal Dictionary<string, string> parametersToLink { get; set; }
        public DynamicMapper(string propetyName, string query, Dictionary<string, string> parametersToLink, Dictionary<string, object> baseParameters)
        {
            this.propetyName = propetyName;
            this.query = query;
            this.baseParameters = baseParameters is null ? new Dictionary<string, object>() : baseParameters;
            this.parametersToLink = parametersToLink is null ? new Dictionary<string, string>() : parametersToLink;
        }
        public DynamicMapper(string propetyName, Query queryLinked, Dictionary<string, string> parametersToLink, Dictionary<string, object> baseParameters, params string[] authorizedColumns) 
            :this (propetyName, queryLinked.Parse(authorizedColumns), parametersToLink, baseParameters)
        { }
        public DynamicMapper(string propetyName, Dictionary<string, string> parametersToLink, Dictionary<string, object> baseParameters)
            : this(propetyName, (string)null, parametersToLink, baseParameters)
        { }
        public DynamicMapper updateQuery(string newQuery)
        {
            this.query = newQuery;
            return this;
        }
        public DynamicMapper(string propetyName, Query queryLinked, params string[] authorizedColumns)
            : this(propetyName, queryLinked.Parse(authorizedColumns), new Dictionary<string, string>(), new Dictionary<string, object>())
        { }
        public DynamicMapper addBaseParams(string varName, object value)
        {
            this.baseParameters[varName] = value;
            return this;
        }
        public DynamicMapper addLinkParams(string varName, string parentField)
        {
            this.parametersToLink[varName] = parentField;
            return this;
        }
        public DynamicParameters getParameters()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            if (baseParameters.Any())
                foreach (var parameter in baseParameters)
                    dynamicParameters.Add(parameter.Key, parameter.Value);
            return dynamicParameters;
        }
    }
}
