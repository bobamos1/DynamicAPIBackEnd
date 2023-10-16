using DynamicSQLFetcher;
using DynamicStructureObjects;

namespace APIDynamic
{
    public static class InitializationFile
    {
        public async static Task InitDB(SQLExecutor executor, bool resetRoles)
        {
            await DynamicController.resetStructureData(executor, true);
            if (resetRoles)
            {
                await executor.ExecuteQueryWithTransaction("DELETE UsersRoles");
                await DynamicController.InsertEnum(executor, "INSERT INTO Roles (id, name) VALUES ({0}, '{1}')", "Roles", typeof(Roles));
            }
            await BDInit.InitDB();
        }
        public static Dictionary<string, string> LoadConnectionStrings(IConfiguration configuration)
        {
            SQLExecutor.Initialize(configuration);
            var connectionStringSection = configuration.GetSection("ConnectionStrings");
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            foreach (var child in connectionStringSection.GetChildren())
            {
                var key = child.Key;
                var value = child.Value;

                if (!string.IsNullOrEmpty(value))
                    connectionStrings[key] = value;
            }
            return connectionStrings;
        }
    }

}
