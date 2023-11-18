using DynamicSQLFetcher;
using DynamicStructureObjects;

namespace APIDynamic
{
    public static class BDInit
    {
        public async static Task InitDB()
        {
            var minOrEqualZeroBundle = ValidatorTypes.MINOREQUAL.SetValue("0", "la valeur doit être de 0 ou plus");
            var isEmail = ValidatorTypes.REGEX.SetValue("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", "la valeur doit être un courriel valide");
            var isDate = ValidatorTypes.REGEX.SetValue("^\\d{4}\\-(0[1-9]|1[012])\\-(0[1-9]|[12][0-9]|3[01])$", "");
            var isTelephone = ValidatorTypes.REGEX.SetValue("*", "");

            await DynamicConnection.addRoleToUser(3, Roles.Admin.ID());
            var controllers = new Dictionary<string, DynamicController>();

            #region AddControllers
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
                .addController("FormatsProduits", false)
                .addController("AffectationsPrix", false)
                .addController("AffectationsPrixLorsCommande", false)
                .addController("AffectationsPrixProduits", false)
                .addController("Clients", true)
                .addController("ReseauxSociaux", false)
                .addController("CollaborateursReseauxSociaux", false)
                .addController("Collaborateurs", true)
                .addController("Compagnies", true)
                .addController("Employes", true)
                .addController("ImagesProduits", false)
                //.addController("TypesPreferencesGraphique", false)
                //.addController("Couleurs", false)
                //.addController("PreferencesGraphiques", false)
                .addController("TypesMedias", false)
                .addController("Medias", false)
                .addController("Images", false)
                .addController("TypeFormats", false)
                .addController("TypeValeurs", false)
                .addController("TypeAffectations", false)
                .addController("FormatsProduitsCommandes", false)
                //.addController("Roles", false)
                ;
            #endregion
            #region Categories
            await controllers["Categories"]
                
                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                )
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                    .Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION)
                    .Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("CategorieMereID", true, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    //.Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT a.id AS ID, a.nom AS Nom, a.descriptions AS Description, b.id AS CategorieMereID, b.nom AS CategorieMere FROM categories a LEFT JOIN categories b ON a.id_categorie_mere = b.id WHERE b.id = @_CategorieMereID AND a.id = @_ID AND a.nom LIKE CONCAT('%', @#Nom, '%')", QueryTypes.SELECT)
                
                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO categories (nom, descriptions, id_categorie_mere) VALUES (@Nom, @Description, @CategorieMereID)", QueryTypes.INSERT)   
                    
                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE categories SET nom = @_Nom, descriptions = @_Description, id_categorie_mere = @_CategorieMereID WHERE id = @ID", QueryTypes.UPDATE)
                        
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM categories", QueryTypes.CBO)
                ;

            #endregion
            #region EtatsProduit
            await controllers["EtatsProduit"]
                .addPropriety("ID", false, true, ShowTypes.ID, minOrEqualZeroBundle)
                .Anonymous()
                .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                .Anonymous()
                .Authorize(Roles.Admin.CanModify())
                .addPropriety("Descriptions", false, true, ShowTypes.DESCRIPTION)
                .Anonymous()
                .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_produit WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO etats_produit (nom, descriptions) VALUES (@Nom, @Descriptions)", QueryTypes.INSERT)
                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE etats_produit SET nom = @_Nom, descriptions = @_Descriptions WHERE id = @ID", QueryTypes.UPDATE)


                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_produit", QueryTypes.CBO)
                ;

            #endregion
            #region Produits
            await controllers["Produits"]
                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Descriptions", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Ingrediants", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Prix", true, true, ShowTypes.FLOAT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("QuantiteInventaire", true, true, ShowTypes.INT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("CategorieID", true, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("EtatProduitID", true, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Images", true, true, ShowTypes.Ref,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Formats", true, true, ShowTypes.Ref,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Affectations", true ,true, ShowTypes.Ref)
                    .Anonymous()
                    .Authorize(Roles.Admin.CanModify())


                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT p.id AS ID, p.nom AS Nom, p.descriptions AS Descriptions, p.ingrediants AS Ingrediants, p.prix AS Prix, p.quantite_inventaire AS QuantiteInventaire, p.id_categorie AS CategorieID, c.nom AS Categorie, p.id_etat_produit AS EtatProduitID, ep.nom AS EtatsProduitNom FROM produits AS p LEFT JOIN categories AS c ON c.id = p.id_categorie LEFT JOIN etats_produit AS ep ON ep.id = p.id_etat_produit LEFT JOIN collaborateurs_produits AS cp ON cp.id_produit = p.id WHERE p.id = @_ID AND p.id_categorie = @_CategorieID AND p.id_etat_produit = @_EtatProduitID AND cp.id_collaborateur = @_CollaborateurID", QueryTypes.SELECT)
                    .addFilter("CollaborateurID", "", ShowTypes.INT, 10, "CollaborateurID")

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO produits (nom, descriptions, ingrediants, prix, quantite_inventaire, id_categorie, id_etat_produit) VALUES (@Nom, @Descriptions, @Ingrediants, @Prix, @QuantiteInventaire, @CategorieID, @EtatProduitID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE produits SET nom = @_Nom, descriptions = @_Descriptions, ingrediants = @_Ingrediants, quantite_inventaire = @_QuantiteInventaire, prix = @_Prix, id_categorie = @_CategorieID, id_etat_produit = @_EtatProduitID WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM produits", QueryTypes.CBO)
            ;

            #endregion
            #region Provinces
            await controllers["Provinces"]
                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING)
                    .Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM provinces WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO provinces (nom) VALUES (@Nom)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE provinces SET nom = @_Nom WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM provinces", QueryTypes.CBO)
            ;

            #endregion
            #region Villes
            await controllers["Villes"]

                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ProvinceID", true, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT v.id AS ID, v.nom AS Nom, v.id_province AS ProvinceID FROM villes AS v INNER JOIN provinces AS pro ON pro.id = v.id_province WHERE pro.id = @_ProvinceID AND v.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO villes (nom, id_province) VALUES (@Nom, @ProvinceID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE villes SET nom = @_Nom, id_province = @_ProvinceID WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM villes", QueryTypes.CBO)
            ;

            #endregion
            #region TypeValeurs
            await controllers["TypeValeurs"]

                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Description FROM types_valeur WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO types_valeur (nom, descriptions) VALUES (@Nom, @Description)", QueryTypes.INSERT)
                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE types_valeur SET nom = @_Nom, descriptions = @_Description WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM types_valeur", QueryTypes.CBO)
            ;
            #endregion
            #region TypeAffectations
            await controllers["TypeAffectations"]

                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("EstTaxe", false, true, ShowTypes.BOOLEAN).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Description, facteur_affectation AS EstTaxe FROM types_affectation WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO types_affectation (nom, descriptions, facteur_affectation) VALUES (@Nom, @Description, @EstTaxe)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE types_affectation SET nom = @_Nom, descriptions = @_Description, facteur_affectation = @_EstTaxe WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM types_valeur", QueryTypes.CBO)
            ;
            #endregion
            #region EtatsCommandes
            await controllers["EtatsCommandes"]
                .addPropriety("ID", false, true, ShowTypes.ID, minOrEqualZeroBundle).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Descriptions", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Descriptions FROM etats_commandes WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO etats_commandes (nom, descriptions) VALUES (@Nom, @Descriptions)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE etats_commandes SET nom = @_Nom, descriptions = @_Descriptions WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM etats_commandes", QueryTypes.CBO)
            ;

            #endregion
            #region Employes
            string selectUserInfoStartEmployes = "SELECT emp.id AS userID, emp.adresse_courriel AS username, emp.adresse_courriel AS Email, emp.mdp as passwordHash, emp.sel AS passwordSalt FROM employes AS emp ";
            string updateTokenEmployes = "UPDATE employes SET token = @Token, expiration_token = DATEADD(MINUTE, 15, GETDATE()) WHERE id = @ID";
            string updatePasswordEmployes = "UPDATE employes SET mdp = @PasswordHash, sel = @PasswordSalt WHERE id = @ID";
            await controllers["Employes"]

                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("DateNaissance", false, true, ShowTypes.DATE).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Email", true, true, ShowTypes.STRING, 
                        isEmail
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Actif", false, true, ShowTypes.BOOLEAN).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT emp.id AS ID, emp.nom AS Nom, emp.prenom AS Prenom, emp.date_naissance AS DateNaissance, emp.adresse_courriel AS Email, emp.actif AS Actif FROM employes AS emp WHERE emp.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO employes (nom, prenom, date_naissance, adresse_courriel, actif) VALUES (@Nom, @Prenom, @DateNaissance, @Email, @Actif)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE employes SET nom = @_Nom, prenom = @_Prenom, date_naissance = @_DateNaissance, adresse_courriel = @_Email, actif = @_Actif WHERE id = @ID", QueryTypes.UPDATE)
                        


                .addRoute("ConnexionStepOne", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStartEmployes + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addFilterParam("Password", ShowTypes.STRING)
                    .addRouteQueryNoVar(updateTokenEmployes, QueryTypes.UPDATE, true)

                .addRoute("ConnexionStepTwo", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStartEmployes + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)
                        .addFilter("Token", "", ShowTypes.STRING, 10, "Token")

                .addRoute("InscriptionEmploye", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery("INSERT INTO employes (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@Nom, @Prenom, @DateNaissance, @Email, @MDP, @Token, @Sel, @Actif)", QueryTypes.INSERT)
                        .removeParams("MDP", "Sel", "Token")
                        .addFilterParam("Password", ShowTypes.STRING)


                .addRoute("RecuperationStepOne", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery("SELECT id FROM employes WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)
                    .addRouteQueryNoVar(updateTokenEmployes, QueryTypes.UPDATE, true)

                .addRoute("RecuperationStepTwo", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery(selectUserInfoStartEmployes + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)
                        .addFilterParam("NewPassword", ShowTypes.STRING)
                        .addFilter("Token", "", ShowTypes.STRING, 10, "Token")
                    .addRouteQueryNoVar(updatePasswordEmployes, QueryTypes.UPDATE, true)

                .addRoute("ChangePassword", RouteTypes.PUT, RouteDisplayTypes.MULTIPLE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery(selectUserInfoStartEmployes + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addFilterParam("NewPassword", ShowTypes.STRING)
                        .addFilterParam("Password", ShowTypes.STRING)
                    .addRouteQueryNoVar(updatePasswordEmployes, QueryTypes.UPDATE, true)

                .addRoute("CheckEmail", RouteTypes.GET)
                    .addRouteQuery("SELECT COUNT(*) FROM employes WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, CONCAT(prenom, ' ', nom) FROM employes", QueryTypes.CBO)

            ;
            #endregion
            #region Formats

            await controllers["Formats"]
                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeFormatID", true, true, ShowTypes.CBO).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT fp.id AS ID, fp.nom AS Nom, fp.descriptions AS Description, tfp.id AS TypeFormatID, tfp.nom AS TypeFormat FROM formats_produit fp INNER JOIN types_format_produit tfp ON tfp.id = fp.id_type_format_produit WHERE fp.id = @_ID AND fp.id IN (SELECT id_format_choisi FROM format_produit_produits_commande WHERE id_produit_commande = @#ProduitParCommandeID) AND fp.id IN (SELECT id_format_choisi FROM formats_produit_produits WHERE id_produit = @#ProduitID)", QueryTypes.SELECT)
                    .addFilter("ProduitParCommandeID", "", ShowTypes.STRING, 100, "ProduitParCommandeID")
                    .addFilter("ProduitID", "", ShowTypes.STRING, 100, "ProduitID")

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO formats_produit (nom, descriptions, id_type_format_produit) VALUES (@Nom, @Description, @TypeFormatID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE formats_produit SET nom = @_Nom, descriptions = @_Description, id_type_format_produit = @_TypeFormatID WHERE id = @ID ", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_format_choisi, format_choisi FROM format_produit_produits_commande", QueryTypes.CBO)

            ;
            #endregion
            #region FormatsProduits
            await controllers["FormatsProduits"]
                .addPropriety("ProduitID", false, true, ShowTypes.CBOID,
                    minOrEqualZeroBundle
                )
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("FormatID", true, true, ShowTypes.CBOID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Format", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, false, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeFormat", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT fpp.id_format_produit AS FormatID, fp.nom AS Format, fpp.id_produit AS ProduitID, p.nom AS Produit, fp.descriptions AS Description, tfp.nom AS TypeFormat FROM formats_produit_produits fpp INNER JOIN formats_produit fp ON fp.id = fpp.id_format_produit LEFT JOIN produits AS p ON fpp.id_produit = p.id LEFT JOIN types_format_produit tfp ON tfp.id = fp.id_type_format_produit WHERE fpp.id_produit = @_ProduitID AND fp.id = @_FormatID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO formats_produit_produits (id_format_produit, id_produit) VALUES (@FormatID, @ProduitID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE formats_produit_produits SET id_format_produit = @_FormatIDNew, id_produit = @_ProduitIDNew WHERE id_format_produit = @FormatID AND id_produit = @ProduitID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_format_choisi, format_choisi FROM format_produit_produits_commande", QueryTypes.CBO)

            ;
            #endregion
            #region FormatsProduitsCommandes
            await controllers["FormatsProduitsCommandes"]
                .addPropriety("ProduitParCommandeID", false, true, ShowTypes.CBOID,
                    minOrEqualZeroBundle
                )
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("FormatID", true, true, ShowTypes.CBOID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("Format", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, false, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeFormat", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("format_selected", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("type_format_selected", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT fp.id AS FormatID, fp.nom AS Format, ppc.id_produit AS ProduitParCommandeID, p.nom AS ProduitParCommande, fp.descriptions AS Description, tfp.nom AS TypeFormat, fppc.format_choisi AS format_selected, fppc.type_format AS type_format_selected FROM format_produit_produits_commande fppc INNER JOIN formats_produit fp ON fp.id = fppc.id_format_choisi INNER JOIN produits_par_commande AS ppc ON ppc.id = fppc.id_produit_commande INNER JOIN produits AS p ON ppc.id_produit = p.id LEFT JOIN types_format_produit tfp ON tfp.id = fp.id_type_format_produit WHERE ppc.id = @_ProduitParCommandeID AND fp.id = @_FormatID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO format_produit_produits_commande (id_format_choisi, id_produit_commande, format_choisi, type_format) VALUES (@FormatID, @ProduitParCommandeID, @format_selected, @type_format_selected)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE format_produit_produits_commande SET id_produit_commande = @_ProduitParCommandeIDNew, id_format_choisi = @_FormatIDNew, format_choisi = @_format_selected, type_format AS @_type_format_selected WHERE id_produit_commande = @ProduitParCommandeID AND id_format_choisi = @FormatID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_format_choisi, format_choisi FROM format_produit_produits_commande", QueryTypes.CBO)
            ;
            #endregion
            #region AffectationsPrixProduits
            await controllers["AffectationsPrixProduits"]
                .addPropriety("AffectationPrixID", true, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Montant", true, true, ShowTypes.FLOAT).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ProduitID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeValeur", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeAffectation", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT p.id AS ProduitID, p.nom AS Produit, ap.id AS AffectationPrixID, ap.nom AS Nom, ap.date_debut AS DateDebut, ap.date_fin AS DateFin, ap.descriptions AS Description, ta.nom AS TypeAffectation, tv.nom AS TypeValeur, montant AS Montant FROM affectation_prix_produits apro INNER JOIN affectation_prix ap ON ap.id = apro.id_affectation_prix INNER JOIN produits p ON p.id = apro.id_produit INNER JOIN types_valeur tv ON tv.id = ap.id_types_valeur INNER JOIN types_affectation ta ON ta.id = ap.id_types_affectation WHERE ap.id = @_AffectationPrixID AND p.id = @_ProduitID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO affectation_prix_produits (id_produit, id_affectation_prix, montant) VALUES (@ProduitID, @AffectationPrixID, @Montant)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE affectation_prix_produits SET id_produit = @_ProduitIDNew, id_affectation_prix = @_AffectationPrixIDNew, montant = @_Montant WHERE id_produit = @ProduitID AND id_affectation_prix = @AffectationPrixID", QueryTypes.UPDATE)

                    .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_produit, montant FROM affectation_prix_produits", QueryTypes.CBO)
            ;
            #endregion
            #region AffectationsPrixLorsCommande
            await controllers["AffectationsPrixLorsCommande"]
                .addPropriety("AffectationPrixID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Montant", true, true, ShowTypes.FLOAT).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ProduitParCommandeID", true, true, ShowTypes.CBOID)
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeValeur", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeAffectation", true, false, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ppc.id AS ProduitParCommandeID, p.nom AS ProduitParCommande, ap.id AS AffectationPrixID, ap.nom AS Nom, ap.date_debut AS DateDebut, ap.date_fin AS DateFin, ap.descriptions AS Description, ta.nom AS TypeAffectation, tv.nom AS TypeValeur, montant AS Montant FROM affectation_prix_lors_commande aplc INNER JOIN affectation_prix ap ON ap.id = aplc.id_affectation_prix INNER JOIN produits_par_commande ppc ON ppc.id = aplc.id_produit_par_commande INNER JOIN produits p ON p.id = ppc.id_produit INNER JOIN types_valeur tv ON tv.id = ap.id_types_valeur INNER JOIN types_affectation ta ON ta.id = ap.id_types_affectation WHERE ap.id = @_AffectationPrixID AND ppc.id = @_ProduitParCommandeID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO affectation_prix_commandes (id_commande, id_affectation_prix, montant) VALUES (@ProduitParCommandeID, @AffectationPrixID, @Montant)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE affectation_prix_commandes SET id_commande = @_ProduitParCommandeIDNew, id_affectation_prix = @_AffectationPrixIDNew, montant = @Montant WHERE id_commande = @ProduitParCommandeID AND id_affectation_prix = @AffectationPrixID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_produit_par_commande, id_affectation_prix, montant FROM affectation_prix_lors_commande", QueryTypes.CBO)
            ;
            #endregion
            #region AffectationsPrix
            await controllers["AffectationsPrix"]

                .addPropriety("ID", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("DateDebut", false, true, ShowTypes.DATE,
                    isDate
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("DateFin", false, true, ShowTypes.DATE).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeValeurID", true, true, ShowTypes.CBO).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeAffectationID", true, true, ShowTypes.CBO).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ap.id AS ID, ap.nom AS Nom, ap.date_debut AS DateDebut, ap.date_fin AS DateFin, ap.descriptions AS Description, ta.id AS TypeAffectationID, ta.nom AS TypeAffectation, tv.id AS TypeValeurID, tv.nom AS TypeValeur FROM affectation_prix AS ap INNER JOIN types_valeur tv ON tv.id = ap.id_types_valeur INNER JOIN types_affectation ta ON ta.id = ap.id_types_affectation WHERE ap.id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO affectation_prix(nom, date_debut, date_fin, descriptions, id_types_valeur, id_types_affectation) VALUES(@Nom, @DateDebut, @DateFin, @Description, @TypeValeurID, @TypeAffectationID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE affectation_prix SET nom = @_Nom, date_debut = @_DateDebut, date_fin = @_DateFin, descriptions = @_Description, id_types_valeur = @_TypeValeurID, id_types_affectation = @_TypeAffectationID) WHERE id = @ID", QueryTypes.UPDATE)

                    .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM affectation_prix", QueryTypes.CBO)
            ;

            #endregion
            #region ProduitsParCommande
            var queryInsertPanierDiffEtat = "INSERT INTO produits_par_commande (id_produit, id_commande, quantite, prix_unitaire) SELECT @id_produit, c.id, @quantite, 0 FROM commandes AS c";
            var queryInsertFormat = "INSERT INTO format_produit_produits_commande (id_format_choisi, id_produit_commande, format_choisi, type_format) SELECT fp.id, @ProduitCommandeID, fp.nom, tfp.nom FROM formats_produit AS fp INNER JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit WHERE fp.id @FormatID";
            await controllers["ProduitsParCommande"]

                .addPropriety("id_commande", false, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("id_produit", true, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("id", true, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("quantite", false, true, ShowTypes.INT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("quantite_restante", false, true, ShowTypes.INT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())/*
                .addPropriety("PrixUnitaire", true, true, ShowTypes.FLOAT,
                    minOrEqualZeroBundle
                ).Anonymous()*/
                .addPropriety("format", false, true, ShowTypes.Ref).Anonymous()
                .Authorize(Roles.Client.CanModify())
                .addPropriety("taxes", false, true, ShowTypes.Ref).Anonymous()
                .Authorize(Roles.Client.CanModify())
                .addPropriety("cout", true, true, ShowTypes.FLOAT).Anonymous()
                .Authorize(Roles.Client.CanModify())
                .addPropriety("coutProduit", true, false, ShowTypes.FLOAT).Anonymous()
                .Authorize(Roles.Client.CanModify())
                .addPropriety("Images", false, true, ShowTypes.Ref).Anonymous()
                .Authorize(Roles.Client.CanModify())
                .addPropriety("TaxesProduit", false, true, ShowTypes.Ref).Anonymous()
                .Authorize(Roles.Client.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT pc.id_commande AS id_commande, pro.id AS id_produit, pc.id AS id, pro.nom AS nom, pro.descriptions AS description, pc.quantite AS quantite, pro.quantite_inventaire AS quantite_restante, pc.prix_unitaire AS cout, pro.prix AS coutProduit FROM produits_par_commande AS pc INNER JOIN produits AS pro ON pro.id = pc.id_produit LEFT JOIN format_produit_produits_commande AS fppc ON fppc.id_produit_commande = pc.id LEFT JOIN formats_produit AS fp ON fp.id = fppc.id_format_choisi LEFT JOIN types_format_produit AS tfp ON tfp.id = fp.id_type_format_produit WHERE pc.id = @_id AND pc.id_commande = @_id_commande", QueryTypes.SELECT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE produits_par_commande SET id_produit = @_id_produit, id_commande = @_id_commande, quantite = @_quantite, prix_unitaire = @_cout WHERE id = @id", QueryTypes.UPDATE)

                .addRoute("InsertPanier", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery(queryInsertPanierDiffEtat + " WHERE c.id_client = @id_client AND c.id_etat_commande = 4", QueryTypes.INSERT)
                        .bindParamToUserID("id_client")
                    .addFilter("ClientID", "", ShowTypes.STRING, 10, "id_client")
                    .addRouteQueryNoVar(queryInsertFormat, QueryTypes.INSERT)

                .addRoute("InsertWishList", RouteTypes.POST)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery(queryInsertPanierDiffEtat + " WHERE c.id_client = @id_client AND c.id_etat_commande = 5", QueryTypes.INSERT)
                        .bindParamToUserID("id_client")
                    .addFilter("ClientID", "", ShowTypes.STRING, 10, "id_client")
                    .addRouteQueryNoVar(queryInsertFormat, QueryTypes.INSERT)

                .addRoute("DeletePanier", RouteTypes.DELETE, RouteDisplayTypes.SINGLE)
                    .addRouteQuery("DELETE format_produit_produits_commande WHERE id_produit_commande = @id", QueryTypes.DELETE)
                    .addRouteQuery("DELETE FROM affectation_prix_lors_commande WHERE id_produit_par_commande = @id", QueryTypes.DELETE)
                    .addRouteQuery("DELETE FROM produits_par_commande WHERE id = @id", QueryTypes.DELETE)
                .addRoute("MoveToPanier", RouteTypes.PUT)
                    .addRouteQuery("UPDATE produits_par_commande SET id_commande = (SELECT TOP(1) id FROM commandes WHERE id_client = @ClientID AND id_etat_commande = 4) WHERE id = @id", QueryTypes.UPDATE)
                        .bindParamToUserID("ClientID")
                    .addFilter("ClientID", "", ShowTypes.STRING, 10, "ClientID")

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT ppc.id, p.nom FROM produits_par_commande ppc INNER JOIN produits p ON p.id = ppc.id_produit", QueryTypes.CBO)
            ;

            #endregion
            #region Commandes
            await controllers["Commandes"]
                .addPropriety("id", false, true, ShowTypes.ID,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("MontantBrut", true, true, ShowTypes.FLOAT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("produitsAchetes", false, true, ShowTypes.Ref).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("dateCreation", true, false, ShowTypes.DATE
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("EtatsCommandesID", false, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("no_civique", false, true, ShowTypes.INT).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("rue", false, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("VilleID", false, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("code_postal", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("numero_facture", true, true, ShowTypes.INT,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("EmployeID", false, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("ClientID", false, true, ShowTypes.CBO,
                    minOrEqualZeroBundle
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())
                .addPropriety("Province", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify(), Roles.Admin.CanModify())

             .addRoute(BaseRoutes.GETALL)
                .addRouteQuery("SELECT c.id AS id, c.montant_brut AS MontantBrut, c.date_heure_transaction AS dateCreation, c.id_etat_commande AS EtatsCommandesID, ec.nom AS etat, c.no_civique_livraison AS no_civique, c.rue_livraison AS rue, c.id_ville AS VilleID, v.nom AS ville, c.numero_facture AS numero_facture, c.id_client AS ClientID, CONCAT(cli.prenom, ' ', cli.nom, ' - ', cli.adresse_courriel) AS Client, pro.nom AS Province, c.id_employe AS EmployeID, c.code_postal AS code_postal, CASE WHEN c.id_employe IS NULL THEN NULL ELSE CONCAT(empl.prenom, ' ', empl.nom, ' - ', empl.adresse_courriel) END AS Employe FROM commandes AS c INNER JOIN etats_commandes AS ec ON ec.id = c.id_etat_commande INNER JOIN clients AS cli ON cli.id = c.id_client LEFT JOIN employes AS empl ON empl.id = c.id_employe LEFT JOIN villes AS v ON v.id = c.id_ville LEFT JOIN provinces AS pro ON pro.id = v.id_province WHERE c.id = @_id AND c.id_client = @_ClientID AND c.id_employe = @_EmployeID AND c.no_civique_livraison = @_no_civique AND dateCreation >= @_DateStart AND dateCreation <= @_DateEnd AND c.id_etat_commande = @_EtatsCommandesID AND c.rue_livraison = @_rue ORDER BY @&SortByCol", QueryTypes.SELECT)
                    .bindParamToUserID("ClientID")
                    .setSQLParam("DateStart", ValidatorTypes.MAX.SetValue("1", ""))
                    .setSQLParam("DateEnd", ValidatorTypes.MAX.SetValue("1", ""))
                    .addFilter("DateDeCreation", "", ShowTypes.DATE, 10, "DateStart", "DateEnd")
            
                 .addRoute(BaseRoutes.INSERT)
                     .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                     .addRouteQuery("INSERT INTO commandes (numero_facture, montant_brut, date_heure_transaction, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (@numero_facture, @MontantBrut, @dateCreation, @no_civique, @rue, @ClientID, @EtatsCommandesID, @VilleID, @EmployeID)", QueryTypes.INSERT)

                 .addRoute(BaseRoutes.UPDATE)
                     .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                     .addRouteQuery("UPDATE commandes SET numero_facture = @_numero_facture, date_heure_transaction = @_dateCreation, montant_brut = @_MontantBrut, no_civique_livraison = @_no_civique, rue_livraison = @_rue, id_client = @_ClientID, id_etat_commande = @_EtatsCommandesID, id_ville = @_VilleID, id_employe = @_EmployeID WHERE id = @id", QueryTypes.UPDATE)

                 .addRoute(BaseRoutes.CBO)
                     .addRouteQuery("SELECT c.id, CONCAT(c.NumeroFacture, ' - ', cli.prenom, ' ', cli.nom) FROM commandes INNER JOIN clients AS cli ON cli.id = c.id_clients", QueryTypes.CBO)

                .addRoute("CheckoutPanier", RouteTypes.POST)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("SELECT pc.id AS ProduitParCommande FROM produits_par_commande AS pc INNER JOIN commandes AS c ON c.id = pc.id_commande WHERE c.id_client = @ClientID AND c.id_etat_commande = 4", QueryTypes.ARRAY)
                        .bindParamToUserID("ClientID")
                    .addRouteQueryNoVar("EXEC CheckoutPanier @ClientID, @ProduitParCommandeID", QueryTypes.STOREPROCEDURE)
                    //.addFilter("ProduitParCommandeID", "", ShowTypes.STRING, 100, "ProduitParCommandeID")
                    .addRouteQuery("EXEC FinaliseCommande @ClientID, @no_civique, @rue, @VilleID", QueryTypes.STOREPROCEDURE)

            ;
            #endregion
            #region Clients
            string selectUserInfoStart = "SELECT cli.id AS userID, cli.adresse_courriel AS username, cli.adresse_courriel AS Email, cli.mdp as passwordHash, cli.sel AS passwordSalt FROM clients AS cli ";
            string updateToken = "UPDATE clients SET token = @Token, expiration_token = DATEADD(MINUTE, 15, GETDATE()) WHERE id = @ID";
            string updatePassword = "UPDATE clients SET mdp = @PasswordHash, sel = @PasswordSalt WHERE id = @ID";
            await controllers["Clients"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("DateNaissance", false, true, ShowTypes.DATE).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("Email", true, true, ShowTypes.STRING
                    ,isEmail
                ).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                /*.addPropriety("MDP", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("Token", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())
                .addPropriety("Sel", false, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())*/
                .addPropriety("Actif", false, true, ShowTypes.BOOLEAN).Anonymous()
                    .Authorize(Roles.Client.CanModify())/*
                .addPropriety("ExpirationToken", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Client.CanModify())*/
                .addPropriety("Commandes", false, true, ShowTypes.Ref)//.Anonymous()

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT cli.id AS ID, cli.nom AS Nom, cli.prenom AS Prenom, cli.date_naissance AS DateNaissance, cli.adresse_courriel AS Email, cli.actif AS Actif FROM clients AS cli WHERE cli.id = @_ID", QueryTypes.SELECT)
                        .bindParamToUserID("ID")
                        
                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, actif) VALUES (@ID, @Nom, @Prenom, @DateNaissance, @Email, @Actif)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery("UPDATE clients SET nom = @_Nom, prenom = @_Prenom, date_naissance = @_DateNaissance, adresse_courriel = @_Email, actif = @_Actif WHERE id = @ID", QueryTypes.UPDATE)
                        .bindParamToUserID("ID")


                .addRoute("ConnexionStepOne", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStart + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addFilterParam("Password", ShowTypes.STRING)
                    .addRouteQueryNoVar(updateToken, QueryTypes.UPDATE)

                .addRoute("ConnexionStepTwo", RouteTypes.POST)
                    .addRouteQuery(selectUserInfoStart + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)
                        .addFilter("Token", "", ShowTypes.STRING, 10, "Token")

                .addRoute("InscriptionClient", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery("INSERT INTO clients (nom, prenom, date_naissance, adresse_courriel, mdp, token, sel, actif) VALUES (@Nom, @Prenom, @DateNaissance, @Email, @MDP, @Token, @Sel, @Actif)", QueryTypes.INSERT)
                        .removeParams("MDP", "Sel", "Token")
                        .addFilterParam("Password", ShowTypes.STRING)
                    .addRouteQueryNoVar("INSERT INTO commandes (numero_facture, montant_brut, date_heure_transaction, no_civique_livraison, rue_livraison, id_client, id_etat_commande, id_ville, id_employe) VALUES (NULL, 0, NULL, NULL, NULL, @ClientID, 4, NULL, NULL), (NULL, 0, NULL, NULL, NULL, @ClientID, 5, NULL, NULL)", QueryTypes.INSERT)
                

                .addRoute("RecuperationStepOne", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery("SELECT id FROM clients WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)
                    .addRouteQueryNoVar(updateToken, QueryTypes.UPDATE, true)

                .addRoute("RecuperationStepTwo", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                    .addRouteQuery(selectUserInfoStart + "WHERE token = @Token AND expiration_token > GETDATE()", QueryTypes.ROW, true)
                        .addFilterParam("NewPassword", ShowTypes.STRING)
                        .addFilter("Token", "", ShowTypes.STRING, 10, "Token")
                    .addRouteQueryNoVar(updatePassword, QueryTypes.UPDATE, true)

                .addRoute("ChangePassword", RouteTypes.PUT, RouteDisplayTypes.MULTIPLE)
                    .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addRouteQuery(selectUserInfoStart + "WHERE adresse_courriel = @Email", QueryTypes.ROW, true)
                        .addFilterParam("NewPassword", ShowTypes.STRING)
                        .addFilterParam("Password", ShowTypes.STRING)
                    .addRouteQueryNoVar(updatePassword, QueryTypes.UPDATE, true)

                .addRoute("CheckEmail", RouteTypes.GET)
                    .addRouteQuery("SELECT COUNT(*) FROM Clients WHERE adresse_courriel = @Email", QueryTypes.VALUE, true)

                .addRoute("Contacter", RouteTypes.POST, RouteDisplayTypes.MULTIPLE)
                .Authorize(Roles.Client.ID(), Roles.Admin.ID())
                    .addEmptyQuery()
                        .addFilterParam("Email", ShowTypes.NONE)
                        .addFilterParam("Sujet", ShowTypes.STRING)
                        .addFilterParam("Contenu", ShowTypes.STRING)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, CONCAT(prenom, ' ', nom) FROM clients", QueryTypes.CBO)
            ;

            #endregion
            #region ReseauxSociaux
            await controllers["ReseauxSociaux"]
                
                .addPropriety("ID", false, true, ShowTypes.ID, minOrEqualZeroBundle).Anonymous()
                .Authorize(Roles.Admin.CanModify())

                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM reseaux_sociaux WHERE ID = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO reseaux_sociaux (nom) VALUES (@Nom)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE reseaux_sociaux SET nom = @_Nom WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM reseaux_sociaux", QueryTypes.CBO)
            ;

            #endregion
            #region Collaborateurs
            await controllers["Collaborateurs"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Image", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Prenom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Telephone", true, true, ShowTypes.INT).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Email", true, true, ShowTypes.STRING,
                    isEmail
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("CompagnieID", true, true, ShowTypes.CBO).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Reseau", false, true, ShowTypes.Ref).Anonymous()


                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT coll.id AS ID, coll.nom AS Nom, coll.prenom AS Prenom, coll.telephone AS Telephone, coll.adresse_courriel AS Email,coll.descriptions AS Description, coll.images AS Image, coll.id_compagnie AS CompagnieID, comp.nom AS Compagnie FROM collaborateurs AS coll LEFT JOIN compagnies AS comp ON comp.id = coll.id_compagnie WHERE coll.id = @_ID AND coll.id_compagnie = @_CompagnieID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .addRouteQuery("INSERT INTO collaborateurs (nom, prenom, telephone, adresse_courriel, images, descriptions, id_compagnie) VALUES (@Nom, @Prenom, @Telephone, @Email, @_Image, @_Description, @CompagnieID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .addRouteQuery("UPDATE collaborateurs SET nom = @_Nom, prenom = @_Prenom, telephone = @_Telephone, adresse_courriel = @_Email, images = @_Image,descriptions=@_Description, id_compagnie = @_CompagnieID WHERE id = @ID", QueryTypes.UPDATE)
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, CONCAT(prenom, ' ', nom, ' - ', adresse_courriel) FROM collaborateurs", QueryTypes.CBO)
            ;
            #endregion
            #region Compagnies
            await controllers["Compagnies"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Telephone", true, true, ShowTypes.INT).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("AdresseCourriel", true, true, ShowTypes.STRING,
                    isEmail
                ).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Contact", false, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, telephone AS Telephone, adresse_courriel AS AdresseCourriel, contact AS Contact FROM compagnies WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO compagnies (nom, telephone, adresse_courriel, contact) VALUES (@Nom, @Telephone, @AdresseCourriel, @Contact)", QueryTypes.INSERT)

               .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE compagnies SET nom = @_Nom, telephone = @_Telephone, adresse_courriel = @_AdresseCourriel, contact = @_Contact WHERE id = @ID", QueryTypes.UPDATE)
                
                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM compagnies", QueryTypes.CBO)
            ;

            #endregion
            #region CollaborateursReseauxSociaux
            await controllers["CollaborateursReseauxSociaux"]

                .addPropriety("CollaborateurID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("CollaborateurNom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ReseauxSociauxID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ReseauxSociauxNom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Liens", false, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT crs.id_collaborateur AS CollaborateurID, CONCAT(coll.prenom,' ', coll.nom) AS CollaborateurNom, crs.id_reseaux_sociaux AS ReseauxSociauxID, rs.nom AS ReseauxSociauxNom, crs.liens AS Liens FROM collaborateurs_reseaux_sociaux AS crs LEFT JOIN collaborateurs AS coll ON coll.id = crs.id_collaborateur LEFT JOIN reseaux_sociaux AS rs ON rs.id = crs.id_reseaux_sociaux WHERE crs.id_collaborateur = @_CollaborateurID AND crs.id_reseaux_sociaux = @_ReseauxSociauxID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO collaborateurs_reseaux_sociaux (id_collaborateur, id_reseaux_sociaux, liens) VALUES (@CollaborateurID, @ReseauxSociauxID, @Liens)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE collaborateurs_reseaux_sociaux SET id_collaborateur = @_CollaborateurID, id_reseaux_sociaux = @_ReseauxSociauxID, liens = @Liens WHERE id_collaborateur = @CollaborateurID AND id_reseaux_sociaux = @ReseauxSociauxID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_collaborateur, liens FROM collaborateurs_reseaux_sociaux", QueryTypes.CBO)

            ;
            #endregion
            #region TypeFormats
            await controllers["TypeFormats"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())


                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Description FROM types_format_produit WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO types_format_produit (nom, descriptions) VALUES (@Nom, @Description)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE types_format_produit SET nom = @_Nom, descriptions = @_Description WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom FROM types_format_produit WHERE id = @_ID", QueryTypes.CBO)
            ;
            #endregion
            #region Images
            await controllers["Images"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("URL", true, true, ShowTypes.IMAGE).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ip.id AS ID, ip.url AS URL, ip.descriptions AS Description FROM images_produit ip WHERE id = @_ID AND ip.id IN (SELECT id_image_produit FROM images_produit_produits WHERE id_produit = @#ProduitID)", QueryTypes.SELECT)
                    .addFilter("ProduitID", "", ShowTypes.STRING, 100, "ProduitID")

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO images_produit (url, descriptions) VALUES (@URL, @Description)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE images_produit SET url = @_URL, descriptions = @_Description WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, url FROM images_produit", QueryTypes.CBO)
            ;
            #endregion
            #region ImagesProduits
            await controllers["ImagesProduits"]

                .addPropriety("ImageID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("URL", true, false, ShowTypes.IMAGE).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("ProduitID", false, true, ShowTypes.CBOID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Produit", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT ip.id AS ImageID, ip.url AS URL, p.id AS ProduitID, p.nom AS Produit, ip.descriptions AS Description FROM images_produit_produits AS ipp INNER JOIN images_produit AS ip ON ip.id = ipp.id_image_produit INNER JOIN produits p ON p.id = ipp.id_produit WHERE ip.id = @_ImageID AND ipp.id_produit = @_ProduitID", QueryTypes.SELECT)
            
                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO images_produit (url, descriptions) VALUES (@URL, @Description)", QueryTypes.INSERT)
                    .addRouteQuery("INSERT INTO images_produit_produits (id_image_produit, id_produit) VALUES (@ImageID, @ProduitID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE images_produit_produits SET id_image_produit = @_ImageIDNew, id_produit = @_ProduitIDNew WHERE id_image_produit = @ImageID AND id_produit = @ProduitID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id_image_produit, id_produit FROM images_produit_produits", QueryTypes.CBO)
            ;


            #endregion
            #region Medias
            await controllers["Medias"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Lien", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("TypeMediaID", true, true, ShowTypes.CBO).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT m.id AS ID, m.liens AS Lien, m.Nom AS Nom, tm.id AS TypeMediaID, tm.nom AS TypeMedia FROM media m INNER JOIN types_medias tm ON tm.id = m.id_types_media WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO media (nom, liens, id_types_media) VALUES (@Nom, @Lien, @TypeMediaID)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE media SET nom = @_Nom, liens = @_Lien, id_types_media = @_TypeMediaID WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM media", QueryTypes.CBO)
            ;
            #endregion
            #region TypesMedias
            await controllers["TypesMedias"]
                .addPropriety("ID", false, true, ShowTypes.ID).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()
                    .Authorize(Roles.Admin.CanModify())
                .addPropriety("Description", false, true, ShowTypes.DESCRIPTION).Anonymous()
                    .Authorize(Roles.Admin.CanModify())

                .addRoute(BaseRoutes.GETALL)
                    .addRouteQuery("SELECT id AS ID, nom AS Nom, descriptions AS Description FROM types_medias WHERE id = @_ID", QueryTypes.SELECT)

                .addRoute(BaseRoutes.INSERT)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("INSERT INTO types_medias (nom, descriptions) VALUES (@Nom, @Description)", QueryTypes.INSERT)

                .addRoute(BaseRoutes.UPDATE)
                    .Authorize(Roles.Admin.ID())
                    .addRouteQuery("UPDATE types_medias SET nom = @_Nom, descriptions = @_Description WHERE id = @ID", QueryTypes.UPDATE)

                .addRoute(BaseRoutes.CBO)
                    .addRouteQuery("SELECT id, nom FROM types_medias", QueryTypes.CBO)
            ;
            #endregion
            #region GÉNÉRATION DE CBO ET MAPGENERATORS
            await controllers["Categories"]
                .addCBOInfo("CategorieMereID", "Categories", "CategorieMere")
            ;
            await controllers["Produits"]
                .addCBOInfo("CategorieID", "Categories", "Categorie")
                .addCBOInfo("EtatProduitID", "EtatsProduit", "EtatProduit")
                .addMapperGenerator("Images", "ImagesProduits", CSharpTypes.REFERENCE.Link("ID", "ProduitID"))
                .addMapperGenerator("Formats", "FormatsProduits", CSharpTypes.REFERENCE.Link("ID", "ProduitID"))
                .addMapperGenerator("Affectations", "AffectationsPrixProduits", CSharpTypes.REFERENCE.Link("ID", "ProduitID"))
            ;
            await controllers["Villes"]
                .addCBOInfo("ProvinceID", "Provinces", "Province")
            ;
            
            await controllers["Formats"]
                .addCBOInfo("TypeFormatID", "TypeFormats", "TypeFormat")
            ;

            await controllers["Medias"]
                .addCBOInfo("TypeMediaID", "TypesMedias", "TypeMedia")
            ;
            await controllers["FormatsProduits"]
                .addCBOInfo("ProduitID", "Produits", "Produit")
                .addCBOInfo("FormatID", "Formats", "Format")
            ;
            await controllers["CollaborateursReseauxSociaux"]
                .addCBOInfo("ReseauxSociauxID", "ReseauxSociaux", "ReseauxSociauxNom")
                .addCBOInfo("CollaborateurID", "Collaborateurs", "Collaborateur")
                //.addCBOInfo("ReseauxSociauxNom", "ReseauxSociaux", "Nom")
            ;
            await controllers["FormatsProduitsCommandes"]
                .addCBOInfo("ProduitParCommandeID", "ProduitsParCommande", "ProduitParCommande")
                .addCBOInfo("FormatID", "Formats", "Format")
            ;
            await controllers["ImagesProduits"]
                .addCBOInfo("ProduitID", "Produits", "Produit")
                .addCBOInfo("ImageID", "Images", "URL")
            ;
            await controllers["ProduitsParCommande"]
                .addCBOInfo("id_produit", "Produits", "nom")
                .addCBOInfo("id_commande", "Commandes", "Commande")
                .addMapperGenerator("taxes", "AffectationsPrixLorsCommande", CSharpTypes.REFERENCE.Link("id", "ProduitParCommandeID"))
                .addMapperGenerator("format", "FormatsProduitsCommandes", CSharpTypes.REFERENCE.Link("id", "ProduitParCommandeID"))
                .addMapperGenerator("Images", "ImagesProduits", CSharpTypes.REFERENCE.Link("id_produit", "ProduitID"))
                .addMapperGenerator("TaxesProduit", "AffectationsPrixProduits", CSharpTypes.REFERENCE.Link("id_produit", "ProduitID"))
            ;
            await controllers["AffectationsPrixLorsCommande"]
                .addCBOInfo("ProduitParCommandeID", "ProduitsParCommande", "ProduitParCommande")
                .addCBOInfo("AffectationPrixID", "AffectationsPrix", "AffectationPrix")
            ;
            await controllers["AffectationsPrixProduits"]
                .addCBOInfo("ProduitID", "Produits", "Produit")
                .addCBOInfo("AffectationPrixID", "AffectationsPrix", "AffectationPrix")
            ;
            await controllers["AffectationsPrix"]
                .addCBOInfo("TypeValeurID", "TypeValeurs", "TypeValeur")
                .addCBOInfo("TypeAffectationID", "TypeAffectations", "TypeAffectation")
            ;
            await controllers["Commandes"]
                .addCBOInfo("EmployeID", "Employes", "Employe")
                .addCBOInfo("VilleID", "Villes", "ville")
                .addCBOInfo("EtatsCommandesID", "EtatsCommandes", "etat")
                .addCBOInfo("ClientID", "Clients", "Client")
                .addMapperGenerator("produitsAchetes", "ProduitsParCommande", CSharpTypes.REFERENCE.Link("id", "CommandeID"))
            ;

            await controllers["Clients"]

                .addMapperGenerator("Commandes", "Commandes", CSharpTypes.REFERENCE.Link("ID", "ClientID"))

            ;

            await controllers["Collaborateurs"]
                .addCBOInfo("CompagnieID", "Compagnies", "Compagnie")
                .addMapperGenerator("Reseau", "CollaborateursReseauxSociaux", CSharpTypes.REFERENCE.Link("ID", "CollaborateurID"))
            ;

            #endregion

            //#region Roles
            //await controllers["Roles"]
            //    .addPropriety("ID", true, true, ShowTypes.ID).Anonymous()
            //    .addPropriety("Nom", true, true, ShowTypes.STRING).Anonymous()

            //    .addRoute(BaseRoutes.GETALL)
            //        .addRouteQuery("", QueryTypes.SELECT)

            //    .addRoute(BaseRoutes.INSERT)
            //        .addRouteQuery("", QueryTypes.INSERT)

            //    .addRoute(BaseRoutes.UPDATE)
            //        .addRouteQuery("", QueryTypes.UPDATE)

            //    .addRoute(BaseRoutes.DELETE)
            //        .addRouteQuery("", QueryTypes.DELETE)
            //;
            //#endregion
        }
    }
}
