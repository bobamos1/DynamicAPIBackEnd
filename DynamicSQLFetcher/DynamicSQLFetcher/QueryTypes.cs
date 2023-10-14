namespace DynamicSQLFetcher
{
    public enum QueryTypes
    {
        NONE = -1,
        UPDATE = 1,
        SELECT = 2,
        ARRAY = 3,
        VALUE = 4,
        ROW = 5,
        CBO = 6,
        INSERT = 7,
        DELETE = 8,
        STOREPROCEDURE = 9,
    }
}
