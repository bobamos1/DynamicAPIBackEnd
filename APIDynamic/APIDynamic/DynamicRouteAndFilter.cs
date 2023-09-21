using DynamicSQLFetcher;

namespace APIDynamic
{
    public class DynamicRouteAndFilter
    {
        public long id { get; set; }
        public Query query { get; set; }
        public List<DynamicFilter> filters { get; set; }
    }
}
