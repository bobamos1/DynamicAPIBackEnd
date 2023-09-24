using DynamicSQLFetcher;

namespace APIDynamic
{
    public class BDInit
    {
        public async static Task InitDB(Dictionary<string, DynamicController> controllers)
        {
            await controllers
                .addController("Produits", true)
                    .addRoute("Produits", BaseRoutes.GETALL)
                        .addRouteQuery("Produits", BaseRoutes.GETALL.Value(), "SELECT id FROM produits", QueryTypes.SELECT, false, false)
                        .addRouteQuery("Produits", BaseRoutes.GETALL.Value(), "SELECT nom FROM produits WHERE id = @id", QueryTypes.SELECT, false, false)
                .addController("Categories", true)
                ;
        }
    }
}
