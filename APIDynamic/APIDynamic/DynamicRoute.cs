namespace APIDynamic
{
    public class DynamicRoute
    {
        public long id { get; set; }
        public string Name { get; set; }
        public List<DynamicRouteAndFilter> queries { get; set; }
    }
}
