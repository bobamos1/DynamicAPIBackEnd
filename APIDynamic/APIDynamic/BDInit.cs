﻿using DynamicSQLFetcher;
using DynamicStructureObjects;
using System.ComponentModel.Design;
using System.Xml.Linq;

namespace APIDynamic
{
    public static class BDInit
    {
        public static string insertRoleString = "INSERT INTO Roles (id, name) VALUES ({0}, '{1}')";
        public static Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Roles (name) VALUES (@Name)", true, true);
        public async static Task InitDB(SQLExecutor executor, bool resetRoles)
        {
            await DynamicController.resetStructureData(executor);
            if (resetRoles)
            {
                List<string> queries = new List<string>() { "DELETE Roles", "DBCC CHECKIDENT ('Roles', RESEED, 0)", "SET IDENTITY_INSERT Roles ON" };
                foreach (Roles role in Enum.GetValues(typeof(Roles)))
                    queries.Add(string.Format(insertRoleString, (long)role, role.Value()));
                queries.Add("SET IDENTITY_INSERT Roles OFF");
                await executor.ExecuteQueryWithTransaction(queries.ToArray());
            }
            await InitDB(new Dictionary<string, DynamicController>());
        }
        public async static Task InitDB(Dictionary<string, DynamicController> controllers)
        {
            await controllers
                .addController("Produits", true)
                .addController("Categories", true)
                .addController("Commandes", true)
                .addController("produits_par_commande", true)
                .addController("Panier", true)
                .addController("Formats", true)
                .addController("Taxes", true)
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

            //await controllers["Produits"]
            //    .addPropriety("ProduitID", true, false, ShowTypes.INT)
            //    .addPropriety("ProduitNom", true, false, ShowTypes.STRING)
            //    .addPropriety("ProduitDescriptions", true, false, ShowTypes.STRING)
            //    .addPropriety("ProduitIngrediants", true, false, ShowTypes.STRING)
            //    .addPropriety("ProduitPrix", true, false, ShowTypes.FLOAT)
            //    .addPropriety("ProduitQuantiteInventaire", true, false, ShowTypes.INT)
            //    .addPropriety("CategorieID", true, false, ShowTypes.INT)
            //    .addPropriety("CategorieNom", true, true, ShowTypes.STRING)
            //        //.addCBOInfo("Categorie", "id_categorie")
            //    .addPropriety("EtatProduitID", true, false, ShowTypes.INT)
            //    .addPropriety("EtatProduit", true, true, ShowTypes.STRING)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addAuthorizedRouteRoles(Roles.Client.ID())
            //        .addRouteQuery("SELECT p.id AS ProduitID, p.nom AS ProduitNom, p.descriptions AS ProduitDescription, p.ingrediants AS ProduitIngrediants, p.prix AS ProduitPrix, p.quantite_inventaire AS ProduitQuantiteInventaire, p.id_categorie AS ProduitCategorie, c.nom AS CategorieNom, p.id_etat_produit AS EtatProduitID, ep.nom AS EtatProduitNom FROM produits AS p INNER JOIN categories c ON c.id = p.id_categorie INNER JOIN etats_produit ep ON ep.id = p.id_etat_produit", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addAuthorizedRouteRoles(Roles.Client.ID())
            //        .addRouteQuery("SELECT p.id AS ProduitID, p.nom AS ProduitNom, p.descriptions AS ProduitDescription, p.ingrediants AS ProduitIngrediants, p.prix AS ProduitPrix, p.quantite_inventaire AS ProduitQuantiteInventaire, p.id_categorie AS ProduitCategorie, c.nom AS CategorieNom, p.id_etat_produit AS EtatProduitID, ep.nom AS EtatProduitNom FROM produits AS p INNER JOIN categories c ON c.id = p.id_categorie INNER JOIN etats_produit ep ON ep.id = p.id_etat_produit WHERE p.id = @id_produit", QueryTypes.SELECT, true, true)
            //            .setSQLParam("ProduitID", ValidatorTypes.REQUIRED.SetValue("id_produit", "Pas de ID passe dans la route"))
            //    .addRoute(BaseRoutes.INSERT)
            //        .addAuthorizedRouteRoles(Roles.Client.ID())
            //        .addRouteQuery("INSERT INTO produits (nom, descriptions, ingrediants, prix, quantite_inventaire, id_categorie, id_etat_produit) VALUES (@nom, @descriptions, @prix, @id_categorie, @id_etat_produit", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom", "nom")
            //            .addSQLParamInfo("descriptions", "descriptions")
            //            .addSQLParamInfo("prix", "prix")
            //            .addSQLParamInfo("id_categorie", "id_categorie")
            //            .addSQLParamInfo("id_etat_produit", "id_etat_produit")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, ingrediants = @_ingrediants, quantite_inventaire = @_quantite_inventaire, prix = @_prix, id_categorie = @_id_categorie, id_etat_produit = @_id_etat_produit WHERE id = @id", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("id", "id")
            //            .addSQLParamInfo("nom", "nom")
            //            .addSQLParamInfo("descriptions", "descriptions")
            //            .addSQLParamInfo("prix", "prix")
            //            .addSQLParamInfo("id_categorie", "id_categorie")
            //            .addSQLParamInfo("id_etat_produit", "id_etat_produit")
            //    ;

            //await controllers["Categories"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("descriptions", true, true, ShowTypes.INT)
            //    .addPropriety("id_categorie_mere", true, true, ShowTypes.STRING)
            //    .addPropriety("Produits",false, false, ShowTypes.Ref)
            //        //.addMapperGenerator("Produits", new ParamLinker("CategorieID", "id", CSharpTypes.REFERENCE))
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT a.id, a.nom, a.descriptions, b.nom FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@nom, @descriptions, @id_categorie_mere)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("descriptions")
            //            .addSQLParamInfo("id_categorie_mere")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addAuthorizedRouteRoles(Roles.Client.ID())
            //        .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, id_categorie_mere = @_id_categorie_mere WHERE id = @id", QueryTypes.UPDATE, true, false)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("descriptions")
            //            .addSQLParamInfo("id_categorie_mere")
            //    //.addRoute(BaseRoutes.CBO)
            //    //    .addRouteQuery("@search", QueryTypes.CBO, false, false)
            //    //        .addSQLParamInfo("search")

            //    ;
            await controllers["Commandes"]
                .addPropriety("numero_facture", true, true, ShowTypes.INT)
                .addPropriety("montant_brut", true, true, ShowTypes.FLOAT)
                .addPropriety("no_civique_livraison", true, true, ShowTypes.STRING)
                .addPropriety("rue_livraison", true, true, ShowTypes.STRING)
                .addPropriety("id_client", true, true, ShowTypes.INT)
                .addPropriety("id_etat_commande", true, true, ShowTypes.INT)
                .addPropriety("id_ville", true, true, ShowTypes.INT)
                .addPropriety("id_employe", true, true, ShowTypes.INT)
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe FROM commandes", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe FROM commandes WHERE id = @id", QueryTypes.SELECT, true, true)
                //.addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO commandes (numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@numero_facture, @montant_brut, @no_civique_livraison, @rue_livraison, @id_client, @id_etat_commande, @id_ville, @id_employe)", QueryTypes.INSERT, true, true)
                /*.addSQLParamInfo("numero_facture")
                .addSQLParamInfo("montant_brut")
                .addSQLParamInfo("no_civique_livraison")
                .addSQLParamInfo("rue_livraison")
                .addSQLParamInfo("id_client")
                .addSQLParamInfo("id_etat_commande")
                .addSQLParamInfo("id_ville")
                .addSQLParamInfo("id_employe")*/
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id = @id", QueryTypes.UPDATE, true, true)
                /*.addSQLParamInfo("id")
                .addSQLParamInfo("numero_facture")
                .addSQLParamInfo("montant_brut")
                .addSQLParamInfo("no_civique_livraison")
                .addSQLParamInfo("rue_livraison")
                .addSQLParamInfo("id_client")
                .addSQLParamInfo("id_etat_commande")
                .addSQLParamInfo("id_ville")
                .addSQLParamInfo("id_employe")*/
                .addRoute("GetClientCommandes", RouteTypes.GET)
                    .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id_client = @id_client", QueryTypes.UPDATE, true, true)
                /*.addSQLParamInfo("numero_facture")
                .addSQLParamInfo("montant_brut")
                .addSQLParamInfo("no_civique_livraison")
                .addSQLParamInfo("rue_livraison")
                .addSQLParamInfo("id_client")
                .addSQLParamInfo("id_etat_commande")
                .addSQLParamInfo("id_ville")
                .addSQLParamInfo("id_employe")*/
                .addRoute("CommandeCheckout", RouteTypes.PUT)
                    .addRouteQuery("UPDATE commandes SET id_etat_commande = 2 WHERE id = @id", QueryTypes.UPDATE, true, true)
                        //.addSQLParamInfo("id")
                ;
            //await controllers["produits_par_commande"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("id_produit", true, true, ShowTypes.INT)
            //    .addPropriety("id_commande", true, true, ShowTypes.INT)
            //    .addPropriety("quantite", true, true, ShowTypes.INT)
            //    .addPropriety("prix_unitaire", true, true, ShowTypes.FLOAT)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute("InsertProduit", RouteTypes.POST)
            //        .addRouteQuery("INSERT INTO produits_par_commande (id_produit, id_commande, quantite, prix_unitaire) VALUES (@id_produit, @id_commande, @quantite, @prix_unitaire)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("id_produit")
            //            .addSQLParamInfo("id_commande")
            //            .addSQLParamInfo("quantite")
            //            .addSQLParamInfo("prix_unitaire")
            //    /*.addRoute("DeleteProduit_commande")
            //        .//addRouteQuery("DELETE FROM produits_par_commande WHERE id = @id", QueryTypes.DELETE, true, true)
            //            .addSQLParamInfo("id")
            //    */
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE produits_par_commande SET id_produit = @_id_produit, id_commande = @_id_commande, quantite = @_quantite, prix_unitaire = @_prix_unitaire", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("id_produit")
            //            .addSQLParamInfo("id_commande")
            //            .addSQLParamInfo("quantite")
            //            .addSQLParamInfo("prix_unitaire")
            //    .addRoute("GetProduitsPanier", RouteTypes.POST)
            //        .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande WHERE id_commande = @id_commande", QueryTypes.SELECT, true, false)
            //            .addSQLParamInfo("id_commande")
            //    ;
            await controllers["Panier"]
                .addPropriety("CommandeID", true, false, ShowTypes.INT)
                .addPropriety("ProduitCommandeID", true, false, ShowTypes.INT)
                .addPropriety("ProduitID", true, false, ShowTypes.INT)
                .addPropriety("NomProduit", true, false, ShowTypes.STRING)
                .addPropriety("DescriptionProduit", true, false, ShowTypes.STRING)
                .addPropriety("QuantiteInventaire", true, false, ShowTypes.INT)
                .addPropriety("PrixProduit", true, false, ShowTypes.FLOAT)
                .addPropriety("ImageProduitURL", true, false, ShowTypes.STRING)
                .addPropriety("id_client", true, false, ShowTypes.INT)
                
                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID())
                    .addRouteQuery("SELECT c.id AS CommandeID, pc.id AS ProduitCommandeID, pc.id_produit AS ProduitID, p.nom AS NomProduit, p.descriptions AS DescriptionProduit, p.quantite_inventaire AS QuantiteInventaire, p.prix AS PrixProduit, ip.url AS ImageProduitURL FROM commandes c INNER JOIN produits_par_commande AS pc ON pc.id_commande = c.id INNER JOIN produits AS p ON p.id = pc.id_produit INNER JOIN images_produit_produits AS ipp ON ipp.id_produit = p.id INNER JOIN images_produit AS ip ON ip.id = ipp.id_image_produit WHERE c.id_etat_commande = @_etat_commande AND c.id_client = @_id_client", QueryTypes.SELECT, true, true)
                        .setSQLParam("id_client", "id_client")
                        .addMapperGenerator("Format")

                .addRoute("GetListeSouhait", RouteTypes.GET)    //5 est liste de souhaits
                    .addAuthorizedRouteRoles(Roles.Client.ID())
                //.addSQLParam("id_client", "id_client", ValidatorTypes.REQUIRED.setValue("true", "Message d'erreur"))
                //.addSQLParam("etat_commande", "etat_commande", ValidatorTypes.REQUIRED.setValue("true", "Message d'erreur"))

                
                .addRoute("GetPanier", RouteTypes.GET)     //4 est Panier
                    .addAuthorizedRouteRoles(Roles.Client.ID())
                 //.addSQLParam("id_client", "id_client", ValidatorTypes.REQUIRED.setValue("true", "Message d'erreur"))
                 //.addSQLParam("etat_commande", "etat_commande", ValidatorTypes.REQUIRED.setValue("true", "Message d'erreur"))

              
                 .addRoute("DeleteProduitPanier", RouteTypes.DELETE)    //Delete produit du panier
                    .addAuthorizedRouteRoles(Roles.Client.ID())
                    .addRouteQuery("DELETE FROM produits_commande WHERE id_produit = @id_produit AND id_client = @id_client AND id_commande = @id_commande", QueryTypes.SELECT, true, true)
                        .setSQLParam("id_produit", "ProduitID")
                        .setSQLParam("id_client", "id_client")
                        .setSQLParam("id_commande", "CommandeID")
                 
            ;
            
            await controllers["Formats"]
                 .addPropriety("Format", true, true, ShowTypes.STRING)

                 .addRoute(BaseRoutes.CBO)
                    .addAuthorizedRouteRoles(Roles.Client.ID())
                    .addRouteQuery("", QueryTypes.CBO, true, false)
                        
            ;

            //await controllers["Taxes"]
            await controllers["Clients"]
                .addPropriety("id", true, true, ShowTypes.INT)
                .addPropriety("nom", true, true, ShowTypes.STRING)
                .addPropriety("prenom", true, true, ShowTypes.STRING)
                .addPropriety("date_naissance", true, true, ShowTypes.STRING)
                .addPropriety("adresse_courriel", true, true, ShowTypes.STRING)
                .addPropriety("mdp", true, true, ShowTypes.STRING)
                .addPropriety("token", true, true, ShowTypes.STRING)
                .addPropriety("sel", true, true, ShowTypes.STRING)
                .addPropriety("actif", true, true, ShowTypes.INT)
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients", QueryTypes.SELECT, true, true)
                .addRoute(BaseRoutes.GET)
                    .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM clients WHERE p.id = @id", QueryTypes.SELECT, true, true)
                //.addSQLParamInfo("id")
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
                /*.addSQLParamInfo("id")
                .addSQLParamInfo("nom")
                .addSQLParamInfo("prenom")
                .addSQLParamInfo("date_naissance")
                .addSQLParamInfo("adresse_courriel")
                .addSQLParamInfo("mdp")
                .addSQLParamInfo("token")
                .addSQLParamInfo("sel")
                .addSQLParamInfo("actif")*/
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE clients SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, true)
                /*.addSQLParamInfo("id")
                .addSQLParamInfo("nom")
                .addSQLParamInfo("prenom")
                .addSQLParamInfo("date_naissance")
                .addSQLParamInfo("adresse_courriel")
                .addSQLParamInfo("mdp")
                .addSQLParamInfo("token")
                .addSQLParamInfo("sel")
                .addSQLParamInfo("actif")*/
                .addRoute("Connection", RouteTypes.POST)
                    .addRouteQuery("SELECT adresse_courriel FROM client WHERE adresse_courriel = @adresse_courriel", QueryTypes.VALUE, true, true)
                    //.addSQLParamInfo("adresse_courriel")
                    .addRouteQuery("SELECT mdp FROM clients WHERE mdp = @mdp", QueryTypes.VALUE, true, true)
                //.addSQLParamInfo("mdp")
                ;
            //await controllers["Collaborateurs"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("prenom", true, true, ShowTypes.STRING)
            //    .addPropriety("telephone", true, true, ShowTypes.INT)
            //    .addPropriety("adresse_courriel", true, true, ShowTypes.STRING)
            //    .addPropriety("id_compagnie", true, true, ShowTypes.INT)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, prenom, telephone, adresse_courriel, id_compagnie FROM collaborateurs", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, prenom, telephone, adresse_courriel, id_compagnie FROM collaborateurs WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO collaborateurs (nom, prenom, telephone, adresse_courriel, id_compagnie) VALUES (@nom, @prenom, @telephone, @adresse_courriel, @id_compagnie)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("prenom")
            //            .addSQLParamInfo("telephone")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("id_compagnie")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE collaborateurs SET nom = @_nom, prenom = @_prenom, telephone = @_telephone, adresse_courriel = @_adresse_courriel, id_compagnie = @_id_compagnie WHERE id = @id", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("prenom")
            //            .addSQLParamInfo("telephone")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("id_compagnie")
            //    ;
            //await controllers["Compagnies"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("prenom", true, true, ShowTypes.STRING)
            //    .addPropriety("telephone", true, true, ShowTypes.INT)
            //    .addPropriety("adresse_courriel", true, true, ShowTypes.STRING)
            //    .addPropriety("contact", true, true, ShowTypes.STRING)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, telephone, adresse_courriel, contact FROM compagnies", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, telephone, adresse_courriel, contact FROM compagnies WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO compagnies (nom, telephone, adresse_courriel, contact) VALUES (@nom, @telephone, @adresse_courriel, @contact)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("telephone")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("contact")
            //   .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE compagnies SET nom = @_nom, telephone = @_telephone, adresse_courriel = @_adresse_courriel, contact = @contact WHERE id = @id", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("telephone")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("contact")
            //    ;
            //await controllers["Villes"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("id_province", true, true, ShowTypes.INT)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, id_province FROM villes", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, id_province WHERE id = @id FROM villes", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    ;
            //await controllers["Provinces"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom FROM provinces", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom WHERE id = @id FROM provinces", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    ;
            //await controllers["Employes"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("prenom", true, true, ShowTypes.STRING)
            //    .addPropriety("date_naissance", true, true, ShowTypes.STRING)
            //    .addPropriety("adresse_courriel", true, true, ShowTypes.STRING)
            //    .addPropriety("mdp", true, true, ShowTypes.STRING)
            //    .addPropriety("token", true, true, ShowTypes.STRING)
            //    .addPropriety("sel", true, true, ShowTypes.STRING)
            //    .addPropriety("actif", true, true, ShowTypes.INT)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM employes", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif FROM employes WHERE p.id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO employes (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("prenom")
            //            .addSQLParamInfo("date_naissance")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("mdp")
            //            .addSQLParamInfo("token")
            //            .addSQLParamInfo("sel")
            //            .addSQLParamInfo("actif")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE employes SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("id")
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("prenom")
            //            .addSQLParamInfo("date_naissance")
            //            .addSQLParamInfo("adresse_courriel")
            //            .addSQLParamInfo("mdp")
            //            .addSQLParamInfo("token")
            //            .addSQLParamInfo("sel")
            //            .addSQLParamInfo("actif")
            //    ;
            //await controllers["EtatsCommandes"]
            //    .addPropriety("id", true, true, ShowTypes.INT)
            //    .addPropriety("nom", true, true, ShowTypes.STRING)
            //    .addPropriety("descriptions", true, true, ShowTypes.STRING)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, descriptions FROM etats_commandes", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, descriptions WHERE id = @id FROM etats_commandes", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    ;
            //await controllers["TypesPreferencesGraphique"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, code_html FROM types_preferences_graphique", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, code_html FROM types_preferences_graphique WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO types_preferences_graphique (nom, code_html) VALUES (@nom, @code_html)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("code_html")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE types_preferences_graphique SET nom = @_nom, code_html = @_code_html", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("code_html")
            //    ;
            //await controllers["Couleurs"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, code_hex FROM couleurs", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, code_hex FROM couleurs WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO couleurs (nom, code_hex) VALUES (@nom, @code_hex)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("code_hex")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE couleurs SET nom = @_nom, code_hex = @_code_hex", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("code_hex")
            //    ;
            //await controllers["PreferencesGraphiques"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, id_couleurs, id_types_preferences FROM preferences_graphiques", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, id_couleurs, id_types_preferences FROM preferences_graphiques WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO preferences_graphiques (nom, id_couleurs, id_types_preferences) VALUES (@nom, @id_couleurs, @id_types_preferences)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("id_couleurs")
            //            .addSQLParamInfo("id_types_preferences")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE preferences_graphiques SET nom = @_nom, id_couleurs = @_id_couleurs, id_types_preferences = @_id_types_preferences", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("id_couleurs")
            //            .addSQLParamInfo("id_types_preferences")
            //    ;
            //await controllers["TypesMedia"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, descriptions FROM types_medias", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, descriptions FROM types_medias WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO types_medias (nom, descriptions) VALUES (@nom, @descriptions)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("descriptions")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE types_medias SET nom = @_nom, descriptions = @_descriptions", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("descriptions")
            //    ;
            //await controllers["Media"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom, descriptions FROM media", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom, liens, id_types_media FROM media WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO media (nom, liens, id_types_media) VALUES (@nom, @liens, @id_types_media)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("liens")
            //            .addSQLParamInfo("id_types_media")
            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE media SET nom = @_nom, liens = @_liens, id_types_media = @_id_types_media", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")
            //            .addSQLParamInfo("liens")
            //            .addSQLParamInfo("id_types_media")
            //    ;
            //await controllers["ReseauxSociaux"]
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, nom FROM reseaux_sociaux", QueryTypes.SELECT, true, true)
            //    .addRoute(BaseRoutes.GET)
            //        .addRouteQuery("SELECT id, nom FROM reseaux_sociaux WHERE id = @id", QueryTypes.SELECT, true, true)
            //            .addSQLParamInfo("id")
            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("INSERT INTO reseaux_sociaux (nom) VALUES (@nom)", QueryTypes.INSERT, true, true)
            //            .addSQLParamInfo("nom")

            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("UPDATE reseaux_sociaux SET nom = @_nom", QueryTypes.UPDATE, true, true)
            //            .addSQLParamInfo("nom")

            //    ;

        }
    }
}
