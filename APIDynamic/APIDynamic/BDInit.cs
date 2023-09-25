using DynamicSQLFetcher;

namespace APIDynamic
{
    public class BDInit
    {
        public async static Task InitDB(Dictionary<string, DynamicController> controllers)
        {
            await controllers
                .addController("Produits", true)
                    .addPropriety(<nom en string>, true, true, <EnumShowType>)
                    .addRoute("Produits", BaseRoutes.GETALL)
                        .addRouteQuery("Produits", BaseRoutes.GETALL.Value(), "SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id", QueryTypes.SELECT, true, false)
                    .addRoute("Produits", BaseRoutes.GET)
                        .addRouteQuery("Produit", BaseRoutes.GET.Value(), "SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id WHERE p.id = @id", QueryTypes.SELECT, true, false)
                            .addSQLParamInfo("id", null)
                    .addRoute("Produits", BaseRoutes.INSERT)
                        .addRouteQuery("Produits", BaseRoutes.INSERT.Value(), "INSERT INTO produits (nom, descriptions, prix, id_categorie, id_etat_produit) VALUES (@nom, @descriptions, @prix, @id_categorie, @id_etat_produit", QueryTypes.INSERT, true, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("descriptions", null)
                            .addSQLParamInfo("prix", null)
                            .addSQLParamInfo("id_categorie", null)
                            .addSQLParamInfo("id_etat_produit", null)
                    .addRoute("Produits", BaseRoutes.UPDATE)
                        .addRouteQuery("Produits", BaseRoutes.UPDATE.Value(), "UPDATE produits SET nom = @_value WHERE id = @id", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("table", null)
                            .addSQLParamInfo("col", null)
                            .addSQLParamInfo("value", null)
                            .addSQLParamInfo("id", null)
                .addController("Categories", true)
                    .addRoute("Categories", BaseRoutes.GETALL)
                        .addRouteQuery("Categories", BaseRoutes.GETALL.Value(), "SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id", QueryTypes.SELECT, true, false)
                    .addRoute("Categorie", BaseRoute.GET)
                        .addRouteQuery("Categories", BaseRoutes.GET.Value(), "SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id WHERE id = @id", QueryTypes.SELECT, true, false)
                            .addSQLParamInfo("id", null)
                    .addRoute("Categories", BaseRoute.INSERT)
                        .addRouteQuery("Categories", BaseRoutes.INSERT.Value(), "INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@nom, @descriptions, @id_categorie_mere)", QueryTypes.INSERT, true, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("descriptions", null)
                            .addSQLParamInfo("id_categorie_mere", null)

                .addController("Commandes", true)
                    .addRoute("Commandes", BaseRoute.GETALL)
                        .addRouteQuery("Commandes", BaseRoutes.GETALL.Value(), "SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe FROM commandes", QueryTypes.SELECT ,true ,false )
                    .addRoute("Commandes", BaseRoute.GET)
                        .addRouteQuery("Commandes", BaseRoutes.GET.Value(), "SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe WHERE id = @id FROM commandes", QueryTypes.SELECT ,true ,false )
                        .addSQLParamInfo("id", null)
                    .addRoute("Commandes", BaseRoute.INSERT)
                        .addRouteQuery("Commandes", BaseRoutes.INSERT.Value(), "INSERT INTO commandes (numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@numero_facture, @montant_brut, @no_civique_livraison, @rue_livraison, @id_client, @id_etat_commande, @id_ville, @id_employe)", QueryTypes.INSERT, true, false)
                            .addSQLParamInfo("numero_facture", null)
                            .addSQLParamInfo("montant_brut", null)
                            .addSQLParamInfo("no_civique_livraison", null)
                            .addSQLParamInfo("rue_livraison", null)
                            .addSQLParamInfo("id_client", null)
                            .addSQLParamInfo("id_etat_commande", null)
                            .addSQLParamInfo("id_ville", null)
                            .addSQLParamInfo("id_employe", null) 


                ;
        }
    }
}
