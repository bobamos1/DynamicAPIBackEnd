using DynamicSQLFetcher;
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
                .addController("Categories", true)
                .addController("EtatsProduit", false)
                .addController("Produits", true)
                .addController("Commandes", true)
                .addController("produits_par_commande", true)
                .addController("Panier", true)
                .addController("Formats", false)
                .addController("Taxes", false)
                .addController("Clients", true)
                .addController("Collaborateurs", true)
                .addController("Compagnies", true)
                .addController("Villes", false)
                .addController("Provinces", false)
                .addController("Employes", true)
                .addController("EtatsCommandes", false)
                .addController("TypesPreferencesGraphique", false)
                .addController("Couleurs", false)
                .addController("PreferencesGraphiques", false)
                .addController("TypesMedia", false)
                .addController("Media", false)
                .addController("ReseauxSociaux", false)
                .addController("EtatsProduits", false)
                ;
            //CHECK pour l'instant
            /*
            await controllers["Categories"]

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM categories", QueryTypes.CBO, false, false)

                .addPropriety("ID", true, true, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Descriptions", true, true, ShowTypes.INT)
                .addPropriety("CategorieMere", true, true, ShowTypes.CBO)
                    .addCBOInfo("Categories", "CategorieMereID")


                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT a.id AS ID, a.nom AS Nom, a.descriptions AS Description, b.id AS CategorieMereID, b.nom AS CategorieMere FROM categories a INNER JOIN categories b ON a.id_categorie_mere = b.id WHERE b.id = @_id_cat_mere AND a.id = @_ID", QueryTypes.SELECT, true, true)
                        .setSQLParam("ID", "ID")

                .addRoute(BaseRoutes.INSERT)
                    .addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@nom, @descriptions, @id_categorie_mere)", QueryTypes.INSERT, true, true)
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("descriptions", "Descriptions")
                        .setSQLParam("id_categorie_mere", "CategorieMere")
                .addRoute(BaseRoutes.UPDATE)
                    .addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, id_categorie_mere = @_id_categorie_mere WHERE id = @id", QueryTypes.UPDATE, true, false)
                        .setSQLParam("id", "ID")
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("descriptions", "Descriptions")
                        .setSQLParam("id_categorie_mere", "CategorieMere")
                ;

            await controllers["EtatsProduit"]
                .addPropriety("ID", true, true, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Descriptions", true, true, ShowTypes.STRING)
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_produit WHERE id = @_ID", QueryTypes.SELECT, false, true)
                        .setSQLParam("ID", "ID")

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_produit", QueryTypes.CBO, false, false)
                ;
            await controllers["EtatsCommandes"]
                .addPropriety("ID", true, true, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Descriptions", true, true, ShowTypes.STRING)

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_commandes WHERE id = @_ID", QueryTypes.SELECT, false, true)
                        .setSQLParam("ID", "ID")
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_commandes", QueryTypes.CBO, false, false)
                ;

            await controllers["Produits"]
                .addPropriety("ID", true, false, ShowTypes.INT)
                .addPropriety("Nom", true, false, ShowTypes.STRING)
                .addPropriety("Descriptions", true, false, ShowTypes.STRING)
                .addPropriety("Ingrediants", true, false, ShowTypes.STRING)
                .addPropriety("Prix", true, false, ShowTypes.FLOAT)
                .addPropriety("QuantiteInventaire", true, false, ShowTypes.INT)
                .addPropriety("Categorie", true, true, ShowTypes.CBO)
                    .addCBOInfo("Categories", "CategorieID")
                .addPropriety("EtatProduit", true, true, ShowTypes.CBO)
                    .addCBOInfo("EtatsProduit", "EtatProduitID")

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT p.id AS ID, p.nom AS Nom, p.descriptions AS Descriptions, p.ingrediants AS Ingrediants, p.prix AS Prix, p.quantite_inventaire AS QuantiteInventaire, c.id AS CategorieID, c.nom AS CategorieNom, p.id_etat_produit AS EtatProduitID, ep.nom AS EtatProduitNom FROM produits AS p INNER JOIN categories c ON c.id = p.id_categorie INNER JOIN etats_produit ep ON ep.id = p.id_etat_produit WHERE p.id = @_ID", QueryTypes.SELECT, true, true)
                        .setSQLParam("ID", "ID")
                .addRoute(BaseRoutes.INSERT)
                    .addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO produits (nom, descriptions, ingrediants, prix, quantite_inventaire, id_categorie, id_etat_produit) VALUES (@nom, @descriptions, @prix, @id_categorie, @id_etat_produit", QueryTypes.INSERT, true, true)
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("descriptions", "Descriptions")
                        .setSQLParam("prix", "Prix")
                        .setSQLParam("id_categorie", "id_categorie")
                        .setSQLParam("id_etat_produit", "id_etat_produit")
                .addRoute(BaseRoutes.UPDATE)
                    .addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("UPDATE produits SET nom = @_nom, descriptions = @_descriptions, ingrediants = @_ingrediants, quantite_inventaire = @_quantite_inventaire, prix = @_prix, id_categorie = @_id_categorie, id_etat_produit = @_id_etat_produit WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .setSQLParam("id", "ID")
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("descriptions", "Descriptions")
                        .setSQLParam("prix", "Prix")
                        .setSQLParam("id_categorie", "Categorie")
                        .setSQLParam("id_etat_produit", "EtatProduit")

                .addRoute(BaseRoutes.CBO)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT id, nom FROM produits", QueryTypes.CBO, false, true)

                ;

            await controllers["Commandes"]
                .addPropriety("ID", true, false, ShowTypes.INT)
                .addPropriety("Employe",false, true, ShowTypes.CBO)
                    .addCBOInfo("Employes", "EmployeID")
                 .addPropriety("Villes", false, true, ShowTypes.CBO)
                    .addCBOInfo("Villes", "VilleID")
                .addPropriety("Clients", false, true, ShowTypes.CBO)
                    .addCBOInfo("Clients", "ClientID")
                .addPropriety("EtatsCommandes", false, true, ShowTypes.CBO)
                    .addCBOInfo("EtatsCommandes", "EtatsCommandesID")
                .addPropriety("NumeroFacture", true, true, ShowTypes.INT)
                .addPropriety("MontantBrut", true, true, ShowTypes.FLOAT)
                .addPropriety("NumeroCiviqueLivraison", true, true, ShowTypes.STRING)
                .addPropriety("RueLivraison", true, true, ShowTypes.STRING)
                .addPropriety("Produits", false, false, ShowTypes.Ref)
                    .addMapperGenerator("produits_par_commande", CSharpTypes.REFERENCE.Link("ID", "CommandeID"))

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT c.id AS ID, c.numero_facture AS NumeroFacture, c.montant_brut AS MontantBrut, c.no_civique_livraison AS NumeroCiviqueLivraison, c.rue_livraison AS RueLivraison, c.id_client AS ClientID, CONCAT(cli.prenom, ' ', cli.nom, ' - ', cli.adresse_courriel) AS Client, c.id_etat_commande AS EtatsCommandesID, ec.nom AS EtatsCommandes, c.id_ville AS VilleID, v.nom AS Ville, pro.id AS ProvinceID, pro.nom AS Province, c.id_employe AS EmployeID, CASE WHEN c.id_employe IS NULL THEN NULL ELSE CONCAT(empl.prenom, ' ', empl.nom, ' - ', empl.adresse_courriel) END AS Employe FROM commandes AS c INNER JOIN etats_commandes AS ec ON ec.id = c.id_etat_commande INNER JOIN clients AS cli ON cli.id = c.id_client LEFT JOIN employes AS empl ON empl.id = c.id_employe LEFT JOIN villes AS v ON v.id = c.id_ville LEFT JOIN provinces AS pro ON pro.id = v.id_province WHERE c.id = @_ID AND c.id_client = @_ClientID AND c.id_employe = @_EmployeID AND c.no_civique_livraison = @_NumeroCiviqueID AND c.id_etat_commande = @_EtatsCommandesID", QueryTypes.SELECT, true, true)
                        .setSQLParam("ID", "ID")
                        .setSQLParam("ClientID", "ClientsID")
                        .setSQLParam("EmployeID", "EmployeID")
                        .setSQLParam("NumeroCiviqueID", "NumeroCiviqueLivraison")
                        .setSQLParam("EtatsCommandesID", "EtatsCommandesID")

                .addRoute(BaseRoutes.INSERT)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO commandes (numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@numero_facture, @montant_brut, @no_civique_livraison, @rue_livraison, @id_client, @id_etat_commande, @id_ville, @id_employe)", QueryTypes.INSERT, true, true)
                        .setSQLParam("numero_facture", "NumeroFacture")
                        .setSQLParam("montant_brut", "MontantBrut")
                        .setSQLParam("no_civique_livraison", "NumeroCiviqueLivraison")
                        .setSQLParam("rue_livraison", "RueLivraison")
                        .setSQLParam("id_client", "Clients")
                        .setSQLParam("id_etat_commande", "EtatsCommandes")
                        .setSQLParam("id_ville", "Villes")
                        .setSQLParam("id_employe", "Employes")

                .addRoute(BaseRoutes.UPDATE)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, montant_brut = @_montant_brut, no_civique_livraison = @_no_civique_livraison, rue_livraison = @_rue_livraison, id_client = @_id_client, id_etat_commande = @_id_etat_commande, id_ville = @_id_ville, id_employe = @_id_employe WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .setSQLParam("ID", "ID")
                        .setSQLParam("numero_facture", "NumeroFacture")
                        .setSQLParam("montant_brut", "MontantBrut")
                        .setSQLParam("no_civique_livraison", "NumeroCiviqueLivraison")
                        .setSQLParam("rue_livraison", "RueLivraison")
                        .setSQLParam("id_client", "Clients")
                        .setSQLParam("id_etat_commande", "EtatsCommandes")
                        .setSQLParam("id_ville", "Villes")
                        .setSQLParam("id_employe", "Employes")
            ;
            //await controllers["produits_par_commande"]
            //    .addPropriety("ID", true, true, ShowTypes.INT)
            //    .addPropriety("id_produit", true, true, ShowTypes.INT)
            //    .addPropriety("id_commande", true, true, ShowTypes.INT)
            //    .addPropriety("quantite", true, true, ShowTypes.INT)
            //    .addPropriety("prix_unitaire", true, true, ShowTypes.FLOAT)
            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("SELECT id, id_produit, id_commande, quantite, prix_unitaire FROM produits_par_commande WHERE commande_id = @_CommandeID", QueryTypes.SELECT, true, true)
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
            /*
            await controllers["Formats"]
                .addPropriety("ID", true, false, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Produit", true, false, ShowTypes.CBO)
                    .addCBOInfo("Produits", "ProduitID")

                .addRoute("FormatDispoProduits", RouteTypes.GET)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT p.id AS ProduitID, fp.nom AS Nom FROM formats_produit AS fp INNER JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit INNER JOIN formats_produit_produits AS fpp ON fpp.id_format_produit = fp.id INNER JOIN produits AS p ON p.id = fpp.id_produit WHERE id_produit = @_produit_id", QueryTypes.SELECT, true, false)
            ;
            await controllers["Taxes"]
                .addPropriety("ProduitID", true, true, ShowTypes.INT)
                .addPropriety("TypeAffectation", true, true, ShowTypes.STRING)
                .addPropriety("Montant", true, true, ShowTypes.STRING)

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT app.id_produit AS ProduitID, ap.nom AS TypeAffectation, app.montant AS Montant FROM affectation_prix_produits AS app INNER JOIN affectation_prix AS ap ON ap.id = app.id_affectation_prix WHERE ap.id = @_ID", QueryTypes.CBO, true, false)

            ;
            await controllers["Panier"]
                .addPropriety("ID", true, false, ShowTypes.INT)
                .addPropriety("ProduitCommandeID", true, false, ShowTypes.INT)
                .addPropriety("Produits", true, false, ShowTypes.Ref)
                    .addMapperGenerator("Produits", "ProduitID")
                .addPropriety("DescriptionProduit", true, false, ShowTypes.STRING)
                .addPropriety("QuantiteInventaire", true, false, ShowTypes.INT)
                .addPropriety("PrixProduit", true, false, ShowTypes.FLOAT)
                .addPropriety("ImageProduitURL", true, false, ShowTypes.STRING)
                .addPropriety("id_client", true, false, ShowTypes.INT)
                .addPropriety("Formats", false, false, ShowTypes.Ref)
                    .addMapperGenerator("Formats", new ParamLinker("ID", "Formats", CSharpTypes.REFERENCE))
                .addPropriety("Taxes", false, false, ShowTypes.Ref)
                    .addMapperGenerator("Taxes", new ParamLinker("ID", "Taxes", CSharpTypes.REFERENCE))

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT c.id AS ID, pc.id AS ProduitCommandeID, pc.id_produit AS ProduitID, p.nom AS NomProduit, p.descriptions AS DescriptionProduit, p.quantite_inventaire AS QuantiteInventaire, p.prix AS PrixProduit, ip.url AS ImageProduitURL FROM commandes c INNER JOIN produits_par_commande AS pc ON pc.id_commande = c.id INNER JOIN produits AS p ON p.id = pc.id_produit INNER JOIN images_produit_produits AS ipp ON ipp.id_produit = p.id INNER JOIN images_produit AS ip ON ip.id = ipp.id_image_produit WHERE c.id_etat_commande = @_etat_commande AND c.id_client = @_id_client", QueryTypes.SELECT, true, true)
                        .setSQLParam("id_client", "id_client")

                .addRoute("GetListeSouhait", RouteTypes.GET)    //5 est liste de souhaits
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    //COmme il y en avait pas ici je guess que les autres non plus
                .addRoute("GetPanier", RouteTypes.GET)     //4 est Panier
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                        //Add queryempty c'est pour ajouter des params pas comme le id client puisqu'iles tnjsafdhik              
                 .addRoute("DeleteProduitPanier", RouteTypes.DELETE)    //Delete produit du panier
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("DELETE FROM produits_commande WHERE id_produit = @id_produit AND id_client = @id_client AND id_commande = @id_commande", QueryTypes.SELECT, true, true)
                        .setSQLParam("id_produit", "ProduitID")
                        .setSQLParam("id_client", "id_client")
                        .setSQLParam("id_commande", "CommandeID")
                 
            ;
            */
            await controllers["Clients"]
                .addPropriety("ID", true, true, ShowTypes.INT, ValidatorTypes.MAX.SetValue("6", "Eille t'es trop haut"))
                .addPropriety("Nom", true, false, ShowTypes.STRING)
                .addPropriety("Prenom", true, false, ShowTypes.STRING)
                .addPropriety("DateNaissance", true, false, ShowTypes.STRING)
                .addPropriety("AdresseCourriel", true, false, ShowTypes.STRING)
                .addPropriety("MDP", true, false, ShowTypes.STRING)
                .addPropriety("Token", true, false, ShowTypes.STRING)
                .addPropriety("Sel", false, false, ShowTypes.STRING)
                .addPropriety("Actif", true, false, ShowTypes.INT)

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, prenom AS Prenom, date_naissance AS DateNaissance, adresse_courriel AS AdresseCourriel, mdp AS MDP, token AS Token, sel AS Sel, actif AS Actif FROM clients WHERE p.id = @_ID AND token = @_Token AND expiration_token = @_ExpirationToken", QueryTypes.SELECT, false, true)
                        .setSQLParam("ID", "ID")
                        /*
                .addRoute(BaseRoutes.INSERT)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@id, @nom, @prenom, @date_naissance, @adresse_courriel, @mdp, @token, @sel, @actif)", QueryTypes.INSERT, true, true)
                        .setSQLParam("id", "ID")
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("prenom", "Prenom")
                        .setSQLParam("date_naissance", "DateNaissance")
                        .setSQLParam("adresse_courriel", "AdresseCourriel")
                        .setSQLParam("mdp", "MDP")
                        .setSQLParam("token", "Token")
                        .setSQLParam("sel", "Sel")
                        .setSQLParam("actif", "Actif")
                .addRoute(BaseRoutes.UPDATE)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE clients SET nom = @_nom, prenom = @_prenom, date_naissance = @_date_naissance, adresse_courriel = @_adresse_courriel, mdp = @_mdp, token = @_token, sel = @_sel, actif = @_actif WHERE id = @id", QueryTypes.UPDATE, true, true)
                        .setSQLParam("id", "ID")
                        .setSQLParam("nom", "Nom")
                        .setSQLParam("prenom", "Prenom")
                        .setSQLParam("date_naissance", "DateNaissance")
                        .setSQLParam("adresse_courriel", "AdresseCourriel")
                        .setSQLParam("mdp", "MDP")
                        .setSQLParam("token", "Token")
                        .setSQLParam("sel", "Sel")
                        .setSQLParam("actif", "Actif")*/


                .addRoute("ConnexionStepOne", RouteTypes.POST, false)
                    .addRouteQuery("SELECT cli.id AS userID, cli.adresse_courriel AS username, cli.adresse_courriel AS Email, cli.mdp as passwordHash, cli.sel AS passwordSalt FROM clients AS cli WHERE adresse_courriel = @Email", QueryTypes.ROW, false, true)
                        .setSQLParam("Email", "AdresseCourriel")
                        .addSQLParam("Password", ValidatorTypes.REQUIRED.SetValue("true", "Password is missing from request."))
                    .addRouteQuery("UPDATE clients SET token = @_Token, expiration_token = DATEADD(MINUTE, 15, GETDATE()) WHERE id = @_ID", QueryTypes.UPDATE, false, true)
                        .setSQLParam("ID", "ID")
                .addRoute("ConnexionStepTwo", RouteTypes.POST, false)
                    .addRouteQuery("SELECT cli.id AS userID, cli.adresse_courriel AS username, cli.adresse_courriel AS Email, cli.mdp as passwordHash, cli.sel AS passwordSalt FROM clients AS cli WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, false, true)
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
