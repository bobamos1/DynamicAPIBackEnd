namespace DynamicStructureObjects
{
    public enum BaseRoutes
    {
        [Value("NULL")]
        NONE = 1,
        [Value("GetAll")]
        GETALL = 2,
        [Value("Get")]
        GET = 3,
        [Value("INSERT")]
        INSERT = 4,
        [Value("UPDATE")]
        UPDATE = 5,
        [Value("GetAllDetailed")]
        GETALLDETAILED = 6,
        [Value("GetDetailed")]
        GETDETAILED = 7,
        [Value("CBO")]
        CBO = 8,
        [Value("DELETE")]
        DELETE = 9,
    }
    public static class BaseRoutesHelper
    {
        public static RouteTypes GetRouteType(BaseRoutes baseRoute)
        {
            switch (baseRoute)
            {
                case BaseRoutes.GETALL:
                    return RouteTypes.GET;
                case BaseRoutes.GET:
                    return RouteTypes.GET;
                case BaseRoutes.INSERT:
                    return RouteTypes.POST;
                case BaseRoutes.UPDATE:
                    return RouteTypes.PUT;
                case BaseRoutes.GETALLDETAILED:
                    return RouteTypes.GET;
                case BaseRoutes.GETDETAILED:
                    return RouteTypes.GET;
                case BaseRoutes.CBO:
                    return RouteTypes.GET;
                case BaseRoutes.DELETE:
                    return RouteTypes.DELETE;
                default:
                    throw new NotImplementedException();
            }
        }
        public static RouteTypes Type(this BaseRoutes baseRoute)
        {
            return GetRouteType(baseRoute);
        }
    }
}
