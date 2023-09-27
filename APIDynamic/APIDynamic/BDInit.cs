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
                .addController("Clients", true)
                ;

            await controllers["Produits"]
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
            await controllers["Clients"]
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients WHERE p.id = @id", QueryTypes.SELECT, true, true)
                        .addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO clients (id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
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
