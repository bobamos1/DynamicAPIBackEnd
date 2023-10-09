using DynamicSQLFetcher;
using DynamicStructureObjects;

namespace APIDynamic
{
    public static class BDInit
    {
        public async static Task InitDB()
        {
            var controllers = new Dictionary<string, DynamicController>();
            await controllers
                .addController("Categories", true)
                .addController("EtatsProduit", false)
                .addController("Produits", true)
                .addController("Provinces", false)
                .addController("Villes", false)
                .addController("EtatsCommandes", false)
                .addController("Commandes", true)
                .addController("ProduitsParCommande", true)
                .addController("Formats", false)
                .addController("Taxes", false)
                .addController("AffectationsPrix", false)
                .addController("Clients", true)
                .addController("ReseauxSociaux", false)
                .addController("CollaborateursReseauxSociaux", false)
                .addController("Collaborateurs", true)
                .addController("Compagnies", true)
                .addController("Employes", true)
                .addController("ImagesProduit", false)
                .addController("TypesPreferencesGraphique", false)
                .addController("Couleurs", false)
                .addController("PreferencesGraphiques", false)
                .addController("TypesMedia", false)
                .addController("Media", false)
                ;
            
            await controllers["Categories"]
                
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Descriptions", true, true, ShowTypes.INT).Anonymous()
                      .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("CategorieMereID", true, true, ShowTypes.CBO).Anonymous()
                      .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                      .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)

                .addRoute(BaseRoutes.GETALL)
                    //.addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT a.id AS ID, a.nom AS Nom, a.descriptions AS Description, b.id AS CategorieMereID, b.nom AS CategorieMere FROM categories a LEFT JOIN categories b ON a.id_categorie_mere = b.id WHERE b.id = @_CategorieMereID AND a.id = @_ID", QueryTypes.SELECT)
                
                .addRoute(BaseRoutes.INSERT)
                    .addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@Nom, @Descriptions, @CategorieMereID)", QueryTypes.INSERT)             
                .addRoute(BaseRoutes.UPDATE)
                    //.addAuthorizedRouteRoles(Roles.Admin.ID())
                    .addRouteQuery("UPDATE categpories SET nom = @_Nom, descriptions = @_Descriptions, id_categorie_mere = @_CategorieMereID WHERE id = @ID", QueryTypes.UPDATE)
                        
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM categories", QueryTypes.CBO)
                ;
            
            await controllers["EtatsProduit"]
                .addPropriety("ID", true, true, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Descriptions", true, true, ShowTypes.STRING)
                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_produit WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_produit", QueryTypes.CBO)
                ;

            await controllers["Produits"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("Descriptions", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("Ingrediants", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("Prix", true, true, ShowTypes.FLOAT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("QuantiteInventaire", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("CategorieID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("EtatProduitID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("ImageID", true, true, ShowTypes.Ref).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)


                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT p.id AS ID, p.nom AS Nom, p.descriptions AS Descriptions, p.ingrediants AS Ingrediants, p.prix AS Prix, p.quantite_inventaire AS QuantiteInventaire, p.id_categorie AS CategorieID, c.nom AS CategorieNom, p.id_etat_produit AS EtatProduitID, ep.nom AS EtatsProduitNom FROM produits AS p LEFT JOIN categories AS c ON c.id = p.id_categorie LEFT JOIN etats_produit AS ep ON ep.id = p.id_etat_produit WHERE p.id = @_ID AND p.id_categorie = @_CategorieID AND p.id_etat_produit = @_EtatProduitID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO produits (nom, descriptions, ingrediants, prix, quantite_inventaire, id_categorie, id_etat_produit) VALUES (@Nom, @Descriptions, @Prix, @CategorieID, @EtatProduitID", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE produits SET nom = @_Nom, descriptions = @_Descriptions, ingrediants = @_Ingrediants, quantite_inventaire = @_QuantiteInventaire, prix = @_Prix, id_categorie = @_CategorieID, id_etat_produit = @_EtatProduitID WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM produits", QueryTypes.CBO)
            ;

            await controllers["Provinces"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM provinces WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM provinces", QueryTypes.CBO)
            ;

            await controllers["Villes"]

                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("ProvinceID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MINOREQUAL)

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT v.id AS ID, v.nom AS Ville, v.id_province AS ProvinceID FROM villes AS v INNER JOIN provinces AS pro ON pro.id = v.id_province WHERE pro.id = @_ProvinceID AND v.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM villes", QueryTypes.CBO)
            ;

            await controllers["EtatsCommandes"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("Descriptions", true, true, ShowTypes.STRING).Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_commandes WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_commandes", QueryTypes.CBO)
                ;

            await controllers["Employes"]
                .addPropriety("ID", true, true, ShowTypes.INT)
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .addPropriety("Prenom", true, true, ShowTypes.STRING)
                .addPropriety("DateNaissance", true, true, ShowTypes.STRING)
                .addPropriety("Email", true, true, ShowTypes.STRING)
                .addPropriety("MDP", true, true, ShowTypes.STRING)
                .addPropriety("Token", true, true, ShowTypes.STRING)
                .addPropriety("Sel", true, true, ShowTypes.STRING)
                .addPropriety("Actif", true, true, ShowTypes.INT)

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS, prenom AS Prenom, date_naissance AS DateNaissance, adresse_courriel AS Email, actif AS Actif FROM employes WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO employes (nom, prenom, date_naissance, adresse_courriel, actif) VALUES (@Nom, @Prenom, @DateNaissance, @Email,  @Actif)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE employes SET nom = @_Nom, prenom = @_Prenom, date_naissance = @_DateNaissance, adresse_courriel = @_Email, actif = @_Actif WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM employes", QueryTypes.CBO)
                ;

            await controllers["Formats"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("ProduitID", true, true, ShowTypes.CBO).Anonymous()

                .addRoute("FormatDispoProduits", RouteTypes.GET)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT p.id AS ProduitID, fp.nom AS Nom FROM formats_produit AS fp INNER JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit INNER JOIN formats_produit_produits AS fpp ON fpp.id_format_produit = fp.id INNER JOIN produits AS p ON p.id = fpp.id_produit WHERE fpp.id_produit = @_ProduitID AND fp.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_format_choisi, format_choisi FROM format_produit_produits_commande", QueryTypes.CBO)
                    
            ;

            await controllers["Taxes"]
                .addPropriety("ProduitID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("AffectationPrixID", true, true, ShowTypes.CBO).Anonymous()
                .addPropriety("Taxe", true, false, ShowTypes.STRING).Anonymous()
                .addPropriety("Descriptions", true, false, ShowTypes.STRING).Anonymous()
                .addPropriety("Montant", true, false, ShowTypes.STRING).Anonymous()
                .addPropriety("TypeAffectation", true, false, ShowTypes.STRING).Anonymous()
                .addPropriety("Montant", true, true, ShowTypes.FLOAT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("TypeAffectation", true, false, ShowTypes.STRING).Anonymous()
                .addPropriety("FacteurAffectation", true, false, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("TypeAffectationDescriptions", true, false, ShowTypes.STRING).Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())   //Avec Formats et Taxes vérifier si l'id du format et de la taxes est sur la bonne table (genre est-ce app ou ap)
                    .addRouteQuery("SELECT app.id_produit AS ProduitID, app.id_affectation_prix AS AffectationPrixID, ap.nom AS Taxe, ap.descriptions AS Descriptions, app.montant AS Montant, ta.nom AS TypeAffectation, ta.facteur_affectation AS FacteurAffectation, ta.descriptions AS TypeAffectationDescriptions FROM affectation_prix AS ap INNER JOIN affectation_prix_produits AS app ON app.id_affectation_prix = ap.id INNER JOIN types_affectation AS ta ON ta.id = ap.id_types_affectation WHERE app.id_produit = @_ProduitID", QueryTypes.SELECT)

                //.addRoute(BaseRoutes.GET) ->Si j'en veux une pcq y'a pas de ID unique à la table on utilise un "combo id"
            ;

            await controllers["AffectationsPrix"]

                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("DateDebut", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("DateFin", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("Descriptions", true, true, ShowTypes.STRING).Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ap.id AS ID, ap.nom AS Nom, ap.date_debut AS DateDebut, ap.date_fin AS DateFin, ap.descriptions AS Descriptions FROM affectation_prix AS ap WHERE ap.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM affectation_prix", QueryTypes.CBO)
            ;

            await controllers["ProduitsParCommande"]

                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("ProduitID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("ProduitDescriptions", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("QuantiteRestante", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("CommandeID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("Quantite", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("PrixUnitaire", true, true, ShowTypes.FLOAT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                .addPropriety("FormatChoisiString", true, true, ShowTypes.STRING).Anonymous()
                .addPropriety("FormatChoisiID", true, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                //.addPropriety("Taxes", true, false, ShowTypes.Ref)
                //.addMapperGenerator("Taxes", CSharpTypes.REFERENCE.Link("TaxeID", "ID"))
                /*.addPropriety("Images", true, false, ShowTypes.Ref)
                    .addMapperGenerator("Images", CSharpTypes.REFERENCE.Link("ImageID", "ID"))
                ->Ajouter les images qui va être également le même mapGenerator que celui pour les images, Par contre faire un CBO pour Afficher que les TOUS les produits (parce que je ne veux que le première image)
                */

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT pc.id AS ID, pro.id AS ProduitID, pro.nom AS Produit, pro.descriptions AS ProduitDescriptions, pro.prix AS PrixCourant, pro.quantite_inventaire AS QuantiteRestante, pc.id_commande AS CommandeID, pc.quantite AS Quantite, pc.prix_unitaire AS PrixPaye, fppc.id_format_choisi AS FormatChoisiID, fppc.format_choisi AS FormatChoisiString, fp.nom AS FormatChoisi, fppc.type_format AS TypeFormat FROM produits_par_commande AS pc INNER JOIN produits AS pro ON pro.id = pc.id_produit LEFT JOIN format_produit_produits_commande AS fppc ON fppc.id_produit_commande = pc.id INNER JOIN formats_produit AS fp ON fp.id = fppc.id_format_choisi INNER JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit", QueryTypes.SELECT)

                .addRoute("InsertProduit", RouteTypes.POST)
                    .addRouteQuery("INSERT INTO produits_par_commande (id_produit, id_commande, quantite, prix_unitaire) VALUES (@ProduitID, @CommandeID, @Quantite, @PrixUnitaire)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO produits_par_commande (id_produit, id_commande, quantite, prix_unitaire) SELECT 1, @prod, 44, p.prix FROM produits AS p WHERE p.id = @ProduitID", QueryTypes.INSERT)
                    .addRouteQuery("INSERT INTO format_produit_produits_commande (id_format_choisi, id_produit_commande, format_choisi, type_format) SELECT @FormatChoisiID, @ProduitCommandeID, fp.nom, tfp.nom FROM formats_produit AS fp INNER JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit WHERE fp.id = @FormatChoisiID", QueryTypes.INSERT)
                        .addSQLParam("FormatID")

                .addRoute(BaseRoutes.DELETE)
                    .addRouteQuery("DELETE FROM produits_par_commande WHERE id = @ID", QueryTypes.DELETE)

                /*
                 * Checker ce qui peut être modifié dans cette table là, dans le fonctionnement, tu ne peux pas changer un produit, mais au lieu l'enlever de la commande et ajouter / DOnc 1 delete et 1 insert au lieu d'un update
                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE produits_par_commande SET id_produit = @_ProduitID, id_commande = @_CommandeID, quantite = @_Quantite, prix_unitaire = @_PrixUnitaire WHERE id = @ID", QueryTypes.UPDATE)
                */
                ;

            await controllers["Commandes"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("EmployeID", false, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("VilleID", false, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("EtatsCommandesID", false, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("NumeroFacture", true, true, ShowTypes.INT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("MontantBrut", true, true, ShowTypes.FLOAT).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("NumeroCiviqueLivraison", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("RueLivraison", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("Produits", false, true, ShowTypes.Ref).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("ClientID", false, true, ShowTypes.CBO).Anonymous()
                    .addValidatorForPropriety("0", ValidatorTypes.MIN)
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify(), Roles.Admin.CanModify())

             .addRoute(BaseRoutes.GETALL)
                 .addRouteQuery("SELECT c.id AS ID, c.numero_facture AS NumeroFacture, c.montant_brut AS MontantBrut, c.no_civique_livraison AS NumeroCiviqueLivraison, c.rue_livraison AS RueLivraison, c.id_client AS ClientID, CONCAT(cli.prenom, ' ', cli.nom, ' - ', cli.adresse_courriel) AS Client, c.id_etat_commande AS EtatsCommandesID, ec.nom AS EtatsCommandes, c.id_ville AS VilleID, v.nom AS Ville, pro.id AS ProvinceID, pro.nom AS Province, c.id_employe AS EmployeID, CASE WHEN c.id_employe IS NULL THEN NULL ELSE CONCAT(empl.prenom, ' ', empl.nom, ' - ', empl.adresse_courriel) END AS Employe FROM commandes AS c INNER JOIN etats_commandes AS ec ON ec.id = c.id_etat_commande INNER JOIN clients AS cli ON cli.id = c.id_client LEFT JOIN employes AS empl ON empl.id = c.id_employe LEFT JOIN villes AS v ON v.id = c.id_ville LEFT JOIN provinces AS pro ON pro.id = v.id_province WHERE c.id = @_ID AND c.id_client = @_ClientID AND c.id_employe = @_EmployeID AND c.no_civique_livraison = @_NumeroCiviqueLivraison AND c.id_etat_commande = @_EtatsCommandesID ORDER BY @&SortByCol", QueryTypes.SELECT)
            
             .addRoute(BaseRoutes.INSERT)
                 .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                 .addRouteQuery("INSERT INTO commandes (numero_facture, montant_brut, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@NumeroFacture, @MontantBrut, @NumeroCiviqueLivraison, @RueLivraison, @ClientID, @EtatsCommandesID, @VilleID, @EmployeID)", QueryTypes.INSERT)

             .addRoute(BaseRoutes.UPDATE)
                 .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                 .addRouteQuery("UPDATE commandes SET numero_facture = @_NumeroFacture, montant_brut = @_MontantBrut, no_civique_livraison = @_NumeroCiviqueLivraison, rue_livraison = @_RueLivraison, id_client = @_ClientID, id_etat_commande = @_EtatsCommandesID, id_ville = @_VilleID, id_employe = @_EmployeID WHERE id = @ID", QueryTypes.UPDATE)

             .addRoute(BaseRoutes.CBO)
                 .addRouteQuery("SELECT c.id, CONCAT(c.NumeroFacture, ' - ', cli.prenom, ' ', cli.nom) FROM commandes INNER JOIN clients AS cli ON cli.id = c.id_clients", QueryTypes.CBO)


            ;
            string selectUserInfoStart = "SELECT cli.id AS userID, cli.adresse_courriel AS username, cli.adresse_courriel AS Email, cli.mdp as passwordHash, cli.sel AS passwordSalt FROM clients AS cli ";
            string updateToken = "UPDATE clients SET token = @Token, expiration_token = DATEADD(MINUTE, 15, GETDATE()) WHERE id = @ID";
            string updatePassword = "UPDATE clients SET mdp = @PasswordHash, sel = @PasswordSalt WHERE id = @ID";
            await controllers["Clients"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("DateNaissance", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Email", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("MDP", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Token", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Sel", false, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Actif", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("ExpirationToken", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanModify())
                .addPropriety("Commandes", true, true, ShowTypes.Ref)//.Anonymous()

                .addRoute(BaseRoutes.GETALL, "ID")
                    .addRouteQuery("SELECT cli.id AS ID, cli.nom AS Nom, cli.prenom AS Prenom, cli.date_naissance AS DateNaissance, cli.adresse_courriel AS Email, cli.actif AS Actif FROM clients AS cli WHERE cli.id = @_ID", QueryTypes.SELECT)
                        
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, actif) VALUES (@ID, @Nom, @Prenom, @DateNaissance, @Email, @Actif)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE, "ID")
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE clients SET nom = @_Nom, prenom = @_Prenom, date_naissance = @_DateNaissance, adresse_courriel = @_Email, actif = @_Actif WHERE id = @ID", QueryTypes.UPDATE)


                .addRoute("ConnexionStepOne", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStart + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addParam("Password")
                    .addRouteQueryNoVar(updateToken, QueryTypes.UPDATE, true)

                .addRoute("ConnexionStepTwo", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStart + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)

                .addRoute("InscriptionClient", RouteTypes.POST)
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@Nom, @Prenom, @DateNaissance, @Email, @MDP, @Token, @Sel, @Actif)", QueryTypes.INSERT)
                        .setNotRequired("MDP", "Sel", "Token")
                        .addParam("Password")
                

                .addRoute("RecuperationStepOne", RouteTypes.POST)
                    .addRouteQuery("SELECT id FROM clients WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)
                    .addRouteQueryNoVar(updateToken, QueryTypes.UPDATE, true)

                .addRoute("RecuperationStepTwo", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStart + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)
                        .addParam("NewPassword")
                    .addRouteQueryNoVar(updatePassword, QueryTypes.UPDATE, true)

                .addRoute("ChangePassword", RouteTypes.PUT)
                    .addAuthorizedRouteRoles(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery(selectUserInfoStart + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addParam("NewPassword")
                        .addParam("Password")
                    .addRouteQueryNoVar(updatePassword, QueryTypes.UPDATE, true)

                .addRoute("CheckEmail", RouteTypes.GET)
                    .addRouteQuery("SELECT COUNT(*) FROM Clients WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, CONCAT(prenom, ' ', nom) FROM clients", QueryTypes.CBO)
            ;

            await controllers["ReseauxSociaux"]
                
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM reseaux_sociaux WHERE ID = @_ID", QueryTypes.SELECT)
                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO reseaux_sociaux (nom) VALUES (@Nom)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE reseaux_sociaux SET nom = @_Nom WHERE id = @_ID", QueryTypes.UPDATE)
                ;

            await controllers["Collaborateurs"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Telephone", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Email", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("CompagnieID", true, true, ShowTypes.CBO).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())


                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT coll.id AS ID, coll.nom AS Nom, coll.prenom AS Prenom, coll.telephone AS Telephone, coll.adresse_courriel AS AdresseCourriel, coll.id_compagnie AS CompagnieID, comp.nom AS CompagnieNom FROM collaborateurs AS coll LEFT JOIN compagnies AS comp ON comp.id = coll.id_compagnie WHERE coll.id = @_ID AND coll.id_compagnie = @_CompagnieID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO collaborateurs (nom, prenom, telephone, adresse_courriel, id_compagnie) VALUES (@Nom, @Prenom, @Telephone, @Email, @CompagnieID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE collaborateurs SET nom = @_Nom, prenom = @_Prenom, telephone = @_Telephone, adresse_courriel = @_Email, id_compagnie = @_CompagnieID WHERE id = @ID", QueryTypes.UPDATE)
                ;
            await controllers["Compagnies"]
                .addPropriety("ID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Telephone", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("AdresseCourriel", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Contact", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, telephone AS Telephone, adresse_courriel AS AdresseCourriel, contact AS Contact FROM compagnies WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO compagnies (nom, telephone, adresse_courriel, contact) VALUES (@Nom, @Telephone, @Email, @Contact)", QueryTypes.INSERT)

               .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE compagnies SET nom = @_Nom, telephone = @_Telephone, adresse_courriel = @_Email, contact = @_Contact WHERE id = @ID", QueryTypes.UPDATE)
                
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM compagnies", QueryTypes.CBO)
                ;

            await controllers["CollaborateursReseauxSociaux"]

                .addPropriety("ReseauxSociauxID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("ReseauxSociaux", true, true, ShowTypes.Ref).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("CollaborateurID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("Collaborateur", true, true, ShowTypes.Ref).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT crs.id_collaborateur AS CollaborateurID, CONCAT(coll.prenom,' ', coll.nom) AS CollaborateurNom, crs.id_reseaux_sociaux AS ReseauxSociauxID, rs.nom AS ReseauxSociauxNom FROM collaborateurs_reseaux_sociaux AS crs LEFT JOIN collaborateurs AS coll ON coll.id = crs.id_collaborateur LEFT JOIN reseaux_sociaux AS rs ON rs.id = crs.id_reseaux_sociaux WHERE crs.id_collaborateur = @_CollaborateurID AND crs.id_reseaux_sociaux = @_ReseauxSociauxID", QueryTypes.SELECT)

            ;

            await controllers["ImagesProduit"]

                .addPropriety("ImageID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("URL", true, true, ShowTypes.Ref).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("ProduitID", true, true, ShowTypes.INT).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())
                .addPropriety("ImageDescriptions", true, true, ShowTypes.STRING).Anonymous()
                    .addAuthorizedProprietyRoles(Roles.Client.CanNotModify(), Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ip.id AS ID, ip.url AS URL FROM images_produit_produits AS ipp INNER JOIN images_produit AS ip ON ip.id = ipp.id_image_produit WHERE ipp.id_produit = @ProduitID", QueryTypes.SELECT)
            /*
            .addRoute(BaseRoutes.INSERT)
                .addRouteQuery("INSERT INTO ")*/
            ;

            /*GÉNÉRATION DE CBO ET MAPGENERATORS*/

            await controllers["Categories"]
                .addCBOInfo("CategorieMereID", "Categories", "CategorieMere")
            ;
            await controllers["Produits"]
                .addCBOInfo("CategorieID", "Categories", "Categorie")
                .addCBOInfo("EtatProduitID", "EtatsProduit", "EtatProduit")
                //.addMapperGenerator("Images", "ImagesProduit", CSharpTypes.REFERENCE.Link("ID", "ImageID"))
            ;
            await controllers["Villes"]
                .addCBOInfo("ProvinceID", "Provinces", "Province")
            ;
            /*
            await controllers["Formats"]
               .addCBOInfo("ProduitID", "Produits", "Produit")
            ;*/
            await controllers["Taxes"]
                .addCBOInfo("AffectationPrixID", "AffectationsPrix", "Taxe")
            ;
            await controllers["ProduitsParCommande"]
                .addCBOInfo("ProduitID", "Produits", "Produit")
                .addCBOInfo("FormatChoisiID", "Formats", "FormatChoisi")
            ;
            await controllers["Commandes"]
                .addCBOInfo("EmployeID", "Employes", "Employe")
                .addCBOInfo("VilleID", "Villes", "Ville")
                .addCBOInfo("EtatsCommandesID", "EtatsCommandes", "EtatsCommandes")
                .addCBOInfo("ClientID", "Clients", "Client")
            ;

            await controllers["Clients"]

                .addMapperGenerator("Commandes", "Commandes", CSharpTypes.REFERENCE.Link("ID", "ClientID"))

            ;
            await controllers["Commandes"]
                .addMapperGenerator("Produits", "ProduitsParCommande", CSharpTypes.REFERENCE.Link("ID", "CommandeID"))

            ;

            await controllers["ProduitsParCommande"]

                .addCBOInfo("CommandeID", "Commandes", "Commande")
            ;
            await controllers["Collaborateurs"]
                .addCBOInfo("CompagnieID", "Compagnies", "Compagnie")
            ;
            await controllers["CollaborateursReseauxSociaux"]
                .addMapperGenerator("Collaborateur", "Collaborateurs", CSharpTypes.REFERENCE.Link("CollaborateurID", "ID"))
                .addMapperGenerator("ReseauxSociaux", "ReseauxSociaux", CSharpTypes.REFERENCE.Link("ReseauxSociauxID", "ID"))
            ;

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


        }
    }
}
