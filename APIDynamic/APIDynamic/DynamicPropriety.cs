namespace APIDynamic
{
    public class DynamicPropriety
    {
        public long id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public List<DynamicFilter> filters { get; set; }
        public long IDDataType { get; set; }
    }
}
