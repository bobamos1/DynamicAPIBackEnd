﻿using DynamicSQLFetcher;
using DynamicStructureObjects;

namespace APIDynamic
{
    public class BDInit
    {
        public async static Task InitDB(Dictionary<string, DynamicController> controllers)
        {
            await controllers
                .addController("Produits", true)
                .addController("Categories", true)
                .addController("Commandes", true)
                .addController("produits_par_commande", true)
                .addController("Clients", true)
                .addController("Collaborateurs", true)
                .addController("Compagnies", true)
                .addController("Villes", true)
                .addController("Provinces", true)
                .addController("Employes", true)
                .addController("EtatsCommandes", true)
                .addController("TypesPreferencesGraphique", true)
                .addController("Couleurs", true)
                .addController("PreferencesGraphiques", true)
                .addController("TypesMedia", true)
                .addController("Media", true)
                .addController("ReseauxSociaux", true)
                .addController("EtatsProduits", true)
                ;

            await controllers["Produits"]
                .addPropriety("nom", true, true, ShowTypes.STRING)
                .addPropriety("descriptions", true, true, ShowTypes.STRING)
                .addPropriety("prix", true, true, ShowTypes.FLOAT)
                .addPropriety("c.nom", true, true, ShowTypes.STRING)
                .addPropriety("ep.nom", true, true, ShowTypes.STRING)
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, descriptions, prix, c.nom as categorie, ep.nom FROM produits p INNER JOIN categorie c ON p.id_categorie = c.id INNER JOIN etats_produit ep ON p.id_etat_produit = ep.id WHERE p.id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO produits (nom, descriptions, prix, id_categorie, id_etat_produit) VALUES (@nom, @descriptions, @prix, @id_categorie, @id_etat_produit", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                        .addSQLParamInfo("prix")
                        .addSQLParamInfo("id_categorie")
                        .addSQLParamInfo("id_etat_produit")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, prix = @_prix, id_categorie = @_id_categorie, id_etat_produit = @_id_etat_produit WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                        .addSQLParamInfo("prix")
                        .addSQLParamInfo("id_categorie")
                        .addSQLParamInfo("id_etat_produit")
                ;
            await controllers["Categories"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@nom, @descriptions, @id_categorie_mere)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                        .addSQLParamInfo("id_categorie_mere")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, id_categorie_mere = @_id_categorie_mere WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("id")
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                        .addSQLParamInfo("id_categorie_mere")
                ;
            await controllers["Commandes"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe FROM commandes", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe WHERE id = @id FROM commandes", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO commandes (numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@numero_facture, @montant_brut, @no_civique_livraison, @rue_livraison, @id_client, @id_etat_commande, @id_ville, @id_employe)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("numero_facture")
                        .addSQLParamInfo("montant_brut")
                        .addSQLParamInfo("no_civique_livraison")
                        .addSQLParamInfo("rue_livraison")
                        .addSQLParamInfo("id_client")
                        .addSQLParamInfo("id_etat_commande")
                        .addSQLParamInfo("id_ville")
                        .addSQLParamInfo("id_employe")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("id")
                        .addSQLParamInfo("numero_facture")
                        .addSQLParamInfo("montant_brut")
                        .addSQLParamInfo("no_civique_livraison")
                        .addSQLParamInfo("rue_livraison")
                        .addSQLParamInfo("id_client")
                        .addSQLParamInfo("id_etat_commande")
                        .addSQLParamInfo("id_ville")
                        .addSQLParamInfo("id_employe")
                .addRoute("GetClientsCommande")
                    .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id_client = @id_client", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("numero_facture")
                        .addSQLParamInfo("montant_brut")
                        .addSQLParamInfo("no_civique_livraison")
                        .addSQLParamInfo("rue_livraison")
                        .addSQLParamInfo("id_client")
                        .addSQLParamInfo("id_etat_commande")
                        .addSQLParamInfo("id_ville")
                        .addSQLParamInfo("id_employe")
                ;
            await controllers["produits_par_commande"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute("InsertProduit")
                    .addRouteQuery("INSERT INTO produits_par_commande (id_produit, id_commande, quantite, prix_unitaire) VALUES (@id_produit, @id_commande, @quantite, @prix_unitaire)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("id_produit")
                        .addSQLParamInfo("id_commande")
                        .addSQLParamInfo("quantite")
                        .addSQLParamInfo("prix_unitaire")
                /*.addRoute("DeleteProduit_commande")
                    .//addRouteQuery("DELETE FROM produits_par_commande WHERE id = @id", QueryTypes.DELETE, true, true)
                        .addSQLParamInfo("id")
                */
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPADATE produits_par_commande SET id_produit = @_id_produit, id_commande = @_id_commande, quantite = @_quantite, prix_unitaire = @_prix_unitaire", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("id_produit")
                        .addSQLParamInfo("id_commande")
                        .addSQLParamInfo("quantite")
                        .addSQLParamInfo("prix_unitaire")
                ;
            await controllers["Clients"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients WHERE p.id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("prenom")
                        .addSQLParamInfo("date_naissance")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("mdp")
                        .addSQLParamInfo("token")
                        .addSQLParamInfo("sel")
                        .addSQLParamInfo("actif")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE clients SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("prenom")
                        .addSQLParamInfo("date_naissance")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("mdp")
                        .addSQLParamInfo("token")
                        .addSQLParamInfo("sel")
                        .addSQLParamInfo("actif")
                ;
            await controllers["Collaborateurs"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, prenom, telephone, adresse_courriel, id_compagnie FROM collaborateurs", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, prenom, telephone, adresse_courriel, id_compagnie FROM collaborateurs WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO collaborateurs (nom, prenom, telephone, adresse_courriel, id_compagnie) VALUES (@nom, @prenom, @telephone, @adresse_courriel, @id_compagnie)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("prenom")
                        .addSQLParamInfo("telephone")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("id_compagnie")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE collaborateurs SET nom = @_nom, prenom = @_prenom, telephone = @_telephone, adresse_courriel = @_adresse_courriel, id_compagnie = @_id_compagnie WHERE id = @id", QueryTypes.UPDATE, true, true)
                            .addSQLParamInfo("id")
                            .addSQLParamInfo("nom")
                            .addSQLParamInfo("prenom")
                            .addSQLParamInfo("telephone")
                            .addSQLParamInfo("adresse_courriel")
                            .addSQLParamInfo("id_compagnie")
                ;
            await controllers["Compagnies"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, telephone, adresse_courriel, contact FROM compagnies", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, telephone, adresse_courriel, contact FROM compagnies WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO compagnies (nom, telephone, adresse_courriel, contact) VALUES (@nom, @telephone, @adresse_courriel, @contact)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("id")
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("telephone")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("contact")
               .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE compagnies SET nom = @_nom, telephone = @_telephone, adresse_courriel = @_adresse_courriel, contact = @contact WHERE id = @id", QueryTypes.UPDATE, true, true)
                            .addSQLParamInfo("id")
                            .addSQLParamInfo("nom")
                            .addSQLParamInfo("telephone")
                            .addSQLParamInfo("adresse_courriel")
                            .addSQLParamInfo("contact")
                ;
            await controllers["Villes"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, id_province FROM villes", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, id_province WHERE id = @id FROM villes", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                ;
            await controllers["Provinces"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom FROM provinces", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom WHERE id = @id FROM provinces", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                ;
            await controllers["Employes"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM employes", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM employes WHERE p.id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO employes (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("prenom")
                        .addSQLParamInfo("date_naissance")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("mdp")
                        .addSQLParamInfo("token")
                        .addSQLParamInfo("sel")
                        .addSQLParamInfo("actif")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE employes SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("prenom")
                        .addSQLParamInfo("date_naissance")
                        .addSQLParamInfo("adresse_courriel")
                        .addSQLParamInfo("mdp")
                        .addSQLParamInfo("token")
                        .addSQLParamInfo("sel")
                        .addSQLParamInfo("actif")
                ;
            await controllers["EtatsCommandes"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, descriptions FROM etats_commandes", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, descriptions WHERE id = @id FROM etats_commandes", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                ;
            await controllers["TypesPreferencesGraphique"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, code_html FROM types_preferences_graphique", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes .GET)
                    .addRouteQuery("SELECT id, nom, code_html FROM types_preferences_graphique WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO types_preferences_graphique (nom, code_html) VALUES (@nom, @code_html)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("code_html")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE types_preferences_graphique SET nom = @_nom, code_html = @_code_html", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("code_html")
                ;
            await controllers["Couleurs"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, code_hex FROM couleurs", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, code_hex FROM couleurs WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO couleurs (nom, code_hex) VALUES (@nom, @code_hex)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("code_hex")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE couleurs SET nom = @_nom, code_hex = @_code_hex", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("code_hex")
                ;
            await controllers["PreferencesGraphiques"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, id_couleurs, id_types_preferences FROM preferences_graphiques", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, id_couleurs, id_types_preferences FROM preferences_graphiques WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO preferences_graphiques (nom, id_couleurs, id_types_preferences) VALUES (@nom, @id_couleurs, @id_types_preferences)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("id_couleurs")
                        .addSQLParamInfo("id_types_preferences")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE preferences_graphiques SET nom = @_nom, id_couleurs = @_id_couleurs, id_types_preferences = @_id_types_preferences", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("id_couleurs")
                        .addSQLParamInfo("id_types_preferences")
                ;
            await controllers["TypesMedia"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, descriptions FROM types_medias", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, descriptions FROM types_medias WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO types_medias (nom, descriptions) VALUES (@nom, @descriptions)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE types_medias SET nom = @_nom, descriptions = @_descriptions", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("descriptions")
                ;
            await controllers["Media"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, descriptions FROM media", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, liens, id_types_media FROM media WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO media (nom, liens, id_types_media) VALUES (@nom, @liens, @id_types_media)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("liens")
                        .addSQLParamInfo("id_types_media")
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE media SET nom = @_nom, liens = @_liens, id_types_media = @_id_types_media", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")
                        .addSQLParamInfo("liens")
                        .addSQLParamInfo("id_types_media")
                ;
            await controllers["ReseauxSociaux"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom FROM reseaux_sociaux", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom FROM reseaux_sociaux WHERE id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO reseaux_sociaux (nom) VALUES (@nom)", QueryTypes.INSERT, true, true)
                        .addSQLParamInfo("nom")

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE reseaux_sociaux SET nom = @_nom", QueryTypes.UPDATE, true, true)
                        .addSQLParamInfo("nom")

                ;
        }
    }
}
