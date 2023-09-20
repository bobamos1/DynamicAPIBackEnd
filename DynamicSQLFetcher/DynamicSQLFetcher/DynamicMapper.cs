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
        internal Query queryLinked { get; set; }
        internal bool completeCheck { get; set; }
        internal IEnumerable<string> authorizedColumns { get; set; }
        internal Dictionary<string, object> baseParameters { get; set; }
        internal Dictionary<string, string> parametersToLink { get; set; }
        public DynamicMapper(string propetyName, Query queryLinked, bool completeCheck, Dictionary<string, string> parametersToLink, Dictionary<string, object> baseParameters, params string[] authorizedColumns) 
        {
            this.propetyName = propetyName;
            this.queryLinked = queryLinked;
            this.authorizedColumns = authorizedColumns;
            this.baseParameters = baseParameters is null ? new Dictionary<string, object>() : baseParameters;
            this.parametersToLink = parametersToLink is null ? new Dictionary<string, string>() : parametersToLink;
            this.completeCheck = completeCheck;
            queryLinked.setQueryWithCols(authorizedColumns);
        }
        public DynamicMapper(string propetyName, Query queryLinked, bool completeCheck, params string[] authorizedColumns)
        {
            this.propetyName = propetyName;
            this.queryLinked = queryLinked;
            this.authorizedColumns = authorizedColumns;
            this.baseParameters = new Dictionary<string, object>();
            this.parametersToLink = new Dictionary<string, string>();
            this.completeCheck = completeCheck;
            queryLinked.setQueryWithCols(authorizedColumns);
        }
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
    }
}
