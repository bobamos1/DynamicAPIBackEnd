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
                        .addRouteQuery("Produits", BaseRoutes.UPDATE.Value(), "UPDATE produits SET nom = @_nom, descriptions = @_descriptions, prix = @_prix, id_categorie = @_id_categorie, id_etat_produit = @_id_etat_produit WHERE id = @id", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("descriptions", null)
                            .addSQLParamInfo("prix", null)
                            .addSQLParamInfo("id_categorie", null)
                            .addSQLParamInfo("id_etat_produit", null)

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
                    .addRoute("Categories", BaseRoutes.UPDATE)
                        .addRouteQuery("Categories", BaseRoutes.UPDATE.Value(), "UPDATE produits SET nom = @_nom, descriptions = @_descriptions, id_categorie_mere = @_id_categorie_mere WHERE id = @id", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("id", null)
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
                    .addRoute("Commandes", BaseRoute.UPDATE)
                        .addRouteQuery("Commandes", BaseRoutes.UPDATE.Value(), "UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id = @id", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("id", null)
                            .addSQLParamInfo("numero_facture", null)
                            .addSQLParamInfo("montant_brut", null)
                            .addSQLParamInfo("no_civique_livraison", null)
                            .addSQLParamInfo("rue_livraison", null)
                            .addSQLParamInfo("id_client", null)
                            .addSQLParamInfo("id_etat_commande", null)
                            .addSQLParamInfo("id_ville", null)
                            .addSQLParamInfo("id_employe", null)
                    .addRoute("Commandes", "GetClientsCommande")
                        .addRouteQuery("Commandes", BaseRoutes.UPDATE.Value(), "UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id_client = @id_client", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("numero_facture", null)
                            .addSQLParamInfo("montant_brut", null)
                            .addSQLParamInfo("no_civique_livraison", null)
                            .addSQLParamInfo("rue_livraison", null)
                            .addSQLParamInfo("id_client", null)
                            .addSQLParamInfo("id_etat_commande", null)
                            .addSQLParamInfo("id_ville", null)
                            .addSQLParamInfo("id_employe", null)

                .addController("Clients", true)
                    .addRoute("Clients", BaseRoute)
                        .addRouteQuery("Clients", BaseRoutes.GETALL.Value(), "SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients", QueryTypes.SELECT, true, false)
                    .addRoute("Clients", BaseRoutes.GET)
                        .addRouteQuery("Clients", BaseRoutes.GET.Value(), "SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients WHERE p.id = @id", QueryTypes.SELECT, true, false)
                            .addSQLParamInfo("id", null)
                    .addRoute("Clients", BaseRoutes.INSERT)
                        .addRouteQuery("Clients", BaseRoutes.INSERT.Value(), "INSERT INTO clients (id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("prenom", null)
                            .addSQLParamInfo("date_naissance", null)
                            .addSQLParamInfo("adresse_courriel", null)
                            .addSQLParamInfo("mdp", null)
                            .addSQLParamInfo("token", null)
                            .addSQLParamInfo("sel", null)
                            .addSQLParamInfo("actif", null)
                    .addRoute("Clients", BaseRoutes.UPDATE)
                        .addRouteQuery("Clients", BaseRoutes.UPDATE.Value(), "UPDATE clients SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, false)
                            .addSQLParamInfo("nom", null)
                            .addSQLParamInfo("prenom", null)
                            .addSQLParamInfo("date_naissance", null)
                            .addSQLParamInfo("adresse_courriel", null)
                            .addSQLParamInfo("mdp", null)
                            .addSQLParamInfo("token", null)
                            .addSQLParamInfo("sel", null)
                            .addSQLParamInfo("actif", null)

                ;





                      ,[nom]
      ,[prenom]
      ,[date_naissance]
      ,[adresse_courriel]
      ,[mdp]
      ,[token]
      ,[sel]
      ,[actif]
        }
    }
}
