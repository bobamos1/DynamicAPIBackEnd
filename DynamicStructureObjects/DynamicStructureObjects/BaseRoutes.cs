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
        public static BaseRoutes[] requireAuthorizationRoutes = new BaseRoutes[] { BaseRoutes.INSERT, BaseRoutes.UPDATE, BaseRoutes.DELETE };
        public static bool requireAuthorization(this BaseRoutes baseRoute)
        {
            return requireAuthorizationRoutes.Contains(baseRoute);
        }
        public static BaseRoutes[] displaySingle = new BaseRoutes[] { BaseRoutes.UPDATE, BaseRoutes.DELETE };
        public static BaseRoutes[] displayMultiple = new BaseRoutes[] { BaseRoutes.INSERT };
        public static RouteDisplayTypes DisplayType(this BaseRoutes baseRoute)
        {
            if (displaySingle.Contains(baseRoute))
                return RouteDisplayTypes.SINGLE;
            if (displayMultiple.Contains(baseRoute))
                return RouteDisplayTypes.MULTIPLE;
            return RouteDisplayTypes.GET;
        }
        public static BaseRoutes[] notgetAuthorizedColsRoutes = new BaseRoutes[] { BaseRoutes.CBO, BaseRoutes.INSERT };
        public static bool getAuthorizedCols(this BaseRoutes baseRoute)
        {
            return !notgetAuthorizedColsRoutes.Contains(baseRoute);
        }
        public static BaseRoutes[] onlyModifyRoutes = new BaseRoutes[] { BaseRoutes.UPDATE };
        public static bool onlyModify(this BaseRoutes baseRoute)
        {
            return onlyModifyRoutes.Contains(baseRoute);
        }
    }
}
