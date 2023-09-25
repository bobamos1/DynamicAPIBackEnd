using DynamicSQLFetcher;
using DynamicStructureObjects;

namespace APIDynamic
{
    public class BDInit
    {
        public async static Task InitDB(Dictionary<string, DynamicController> controllers)
        {
            /*
            await controllers
                .addController("Produits", true)
                    .addRoute("Produits", BaseRoutes.GETALL)
                        .addRouteQuery("Produits", BaseRoutes.GETALL.Value(), "SELECT id FROM produits", QueryTypes.SELECT, false, false)
                        .addRouteQuery("Produits", BaseRoutes.GETALL.Value(), "SELECT nom FROM produits WHERE id = @id", QueryTypes.SELECT, false, false)
                .addController("Categories", true)
                ;
            */
            string controllerProduitName = "LesProduits";
            await controllers
                .addController(controllerProduitName, true)
                    //.addPropriety(< nom en string >, true, true, < EnumShowType >)
                    .addRoute(controllerProduitName, BaseRoutes.GETALL)
                        .addRouteQuery(controllerProduitName, BaseRoutes.GETALL.Value(), "SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id", QueryTypes.SELECT, false, false)
                    .addRoute(controllerProduitName, BaseRoutes.GET)
                        .addRouteQuery(controllerProduitName, BaseRoutes.GET.Value(), "SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id WHERE p.id = @id", QueryTypes.SELECT, true, false)
                            .addSQLParamInfo(controllerProduitName, BaseRoutes.GET.Value(), 1, "id", null)
                    .addRoute(controllerProduitName, BaseRoutes.INSERT)
                        .addRouteQuery("InsertProduit", BaseRoutes.INSERT.Value(), "INSERT INTO produits (nom, descriptions, prix, id_categorie, id_etat_produit) VALUES (@nom, @descriptions, @prix, @id_categorie, @id_etat_produit", QueryTypes.INSERT, false, false)
                            /*.addSQLParamInfo("nom", null)
                            .addSQLParamInfo("descriptions", null)
                            .addSQLParamInfo("prix", null)
                            .addSQLParamInfo("id_categorie", null)
                            .addSQLParamInfo("id_etat_produit", null)
                    .addRoute("UpdateSingleCol", BaseRoutes.UPDATE)
                        .addRouteQuery("UpdateProduit", BaseRoutes.UPDATE.Value(), "UPDATE Produits SET nom2 = @_value2, nom3 = @_value3, nom4 = @_value4 WHERE id = @id", QueryTypes.UPDATE, false, false)
                            .addSQLParamInfo("table", null)
                            .addSQLParamInfo("col", null)
                            .addSQLParamInfo("value", null)
                            .addSQLParamInfo("id", null)
                .addController("Categories", true)
                    .addRoute("Categories", BaseRoutes.GETALL)
                        .addRouteQuery("Categories", BaseRoutes.GETALL.Value(), "SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id", QueryTypes.SELECT, false, false)
                    .addRoute("Categorie", BaseRoutes.GET)
                        .addRouteQuery("Categorie", BaseRoutes.GET.Value(), "SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id WHERE id = @id", QueryTypes.SELECT, false, false)
                            .addSQLParamInfo("id", null)
                    .addRoute("InsertCategorie", BaseRoutes.INSERT)
                        .addRouteQuery("InsertCategorie", BaseRoutes.INSERT.Value(), "INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@nom, @descriptions, @id_categorie_mere)", QueryTypes.INSERT, false, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("descriptions", null)
                            .addSQLParamInfo("id_categorie_mere", null)

                //.addController("Commandes", true)
                            */

                ;
        }
    }
}
