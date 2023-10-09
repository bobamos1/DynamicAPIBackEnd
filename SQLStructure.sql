
use master;
GO
DROP DATABASE structure;
GO

CREATE DATABASE structure;
GO
use structure;
GO
CREATE TABLE Tables (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    isView BIT
)
CREATE TABLE Colonnes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    id_table BIGINT,
    id_datatype BIGINT
)
CREATE TABLE DataTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE Tables_Colonnes (
    id_table BIGINT NOT NULL,
    id_colonne BIGINT NOT NULL
)
CREATE TABLE Controllers (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    isMain BIT
)
CREATE TABLE Proprieties (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    isMain BIT,
    isUpdatable BIT DEFAULT 0,
    id_ShowType BIGINT,
    id_controller BIGINT
)
CREATE TABLE CSharpTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(50)
)
CREATE TABLE LinkProprietiesControllers (
    id BIGINT IDENTITY(1,1),
    id_propriety BIGINT,
    id_controller BIGINT
)
CREATE TABLE ListVars (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    value VARCHAR(100),
    id_CSharpType BIGINT,
    id_link BIGINT
)
CREATE TABLE QueryTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE RouteTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE BaseRoutes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE URLRoutes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100),
    id_baseRoute BIGINT,
    id_controller BIGINT,
    id_routeType BIGINT,
    requireAuthorization BIT,
    getAuthorizedCols BIT,
    onlyModify BIT,
    id_proprietyForUserID BIGINT
)
CREATE TABLE RouteQueries (
    id BIGINT IDENTITY(1,1),
    ind INT,
    SQLString VARCHAR(MAX),
    id_queryType BIGINT,
    id_route BIGINT,
    completeCheck BIT DEFAULT 1,
    completeAuth BIT
)
CREATE TABLE Roles (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE UsersRoles (
    id_user BIGINT NOT NULL,
    id_role BIGINT NOT NULL
)
CREATE TABLE PermissionProprieties (
    id_propriety BIGINT NOT NULL,
    canModify BIT,
    id_role BIGINT NOT NULL
)
CREATE TABLE PermissionRoutes (
    id_route BIGINT NOT NULL,
    id_role BIGINT NOT NULL
)
CREATE TABLE ShowTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE SQLParamInfos (
    id BIGINT IDENTITY(1,1),
    id_Propriety BIGINT,
    id_RouteQuery BIGINT,
    varAffected VARCHAR(100)
)
CREATE TABLE Filters (
    id BIGINT IDENTITY(1,1),
    id_SQLParamInfo BIGINT,
    id_ShowType BIGINT,
    ind INT,
    name VARCHAR(100)
)
CREATE TABLE ValidatorTypes (
    id BIGINT IDENTITY(1,1),
    name VARCHAR(100)
)
CREATE TABLE ValidatorSQLParamInfoValues (
    id_SQLParamInfo BIGINT NOT NULL,
    id_ValidatorType BIGINT NOT NULL,
    message VARCHAR(MAX),
    value VARCHAR(MAX)
)
CREATE TABLE ValidatorProprietyValues (
    id_Propriety BIGINT NOT NULL,
    id_ValidatorType BIGINT NOT NULL,
    message VARCHAR(MAX),
    value VARCHAR(MAX)
)
GO
-- Add primary keys

-- Tables Table
ALTER TABLE Tables
ADD CONSTRAINT PK_Tables PRIMARY KEY (id);

-- Colonnes Table
ALTER TABLE Colonnes
ADD CONSTRAINT PK_Colonnes PRIMARY KEY (id);

-- DataTypes Table
ALTER TABLE DataTypes
ADD CONSTRAINT PK_DataTypes PRIMARY KEY (id);

-- Tables_Colonnes Table
ALTER TABLE Tables_Colonnes
ADD CONSTRAINT PK_Tables_Colonnes PRIMARY KEY (id_table, id_colonne);

-- Controllers Table
ALTER TABLE Controllers
ADD CONSTRAINT PK_Controllers PRIMARY KEY (id);

-- Properties Table
ALTER TABLE Proprieties
ADD CONSTRAINT PK_Properties PRIMARY KEY (id);

-- CSharpTypes Table
ALTER TABLE CSharpTypes
ADD CONSTRAINT PK_CSharpTypes PRIMARY KEY (id);

-- LinkProprietiesControllers Table
ALTER TABLE LinkProprietiesControllers
ADD CONSTRAINT PK_LinkProprietiesControllers PRIMARY KEY (id);

-- ListVars Table
ALTER TABLE ListVars
ADD CONSTRAINT PK_ListVars PRIMARY KEY (id);

-- QueryType Table
ALTER TABLE QueryTypes
ADD CONSTRAINT PK_QueryType PRIMARY KEY (id);

-- RouteTypes Table
ALTER TABLE RouteTypes
ADD CONSTRAINT PK_RouteTypes PRIMARY KEY (id);

-- BaseRoutes Table
ALTER TABLE BaseRoutes
ADD CONSTRAINT PK_BaseRoutes PRIMARY KEY (id);

-- URLRoutes Table
ALTER TABLE URLRoutes
ADD CONSTRAINT PK_Routes PRIMARY KEY (id);

-- RouteQueries Table
ALTER TABLE RouteQueries
ADD CONSTRAINT PK_RouteQueries PRIMARY KEY (id);

-- Roles Table
ALTER TABLE Roles
ADD CONSTRAINT PK_Roles PRIMARY KEY (id);

-- UsersRoles Table
ALTER TABLE UsersRoles
ADD CONSTRAINT PK_UsersRoles PRIMARY KEY (id_user, id_role);

-- PermissionProprieties Table
ALTER TABLE PermissionProprieties
ADD CONSTRAINT PK_PermissionProprieties PRIMARY KEY (id_propriety, id_role);

-- PermissionRoutes Table
ALTER TABLE PermissionRoutes
ADD CONSTRAINT PK_PermissionRoutes PRIMARY KEY (id_route, id_role);

-- ShowTypes Table
ALTER TABLE ShowTypes
ADD CONSTRAINT PK_ShowTypes PRIMARY KEY (id);

-- SQLParamInfos Table
ALTER TABLE SQLParamInfos
ADD CONSTRAINT PK_SQLParamInfos PRIMARY KEY (id);

-- Filters Table
ALTER TABLE Filters
ADD CONSTRAINT PK_Filters PRIMARY KEY (id);

-- ValidatorTypes Table
ALTER TABLE ValidatorTypes
ADD CONSTRAINT PK_ValidatorTypes PRIMARY KEY (id);

-- ValidatorSQLParamInfoValues Table
ALTER TABLE ValidatorSQLParamInfoValues
ADD CONSTRAINT PK_ValidatorSQLParamInfoValues PRIMARY KEY (id_SQLParamInfo, id_ValidatorType);

-- ValidatorProprietyValues Table
ALTER TABLE ValidatorProprietyValues
ADD CONSTRAINT PK_ValidatorProprietyValues PRIMARY KEY (id_Propriety, id_ValidatorType);

-- Add foreign keys

-- Colonnes Table
ALTER TABLE Colonnes
ADD CONSTRAINT FK_Colonnes_DataTypes
FOREIGN KEY (id_datatype) REFERENCES DataTypes(id);

-- Tables_Colonnes Table
ALTER TABLE Tables_Colonnes
ADD CONSTRAINT FK_Tables_Colonnes_Tables
FOREIGN KEY (id_table) REFERENCES Tables(id);

ALTER TABLE Tables_Colonnes
ADD CONSTRAINT FK_Tables_Colonnes_Colonnes
FOREIGN KEY (id_colonne) REFERENCES Colonnes(id);

-- Properties Table
ALTER TABLE Proprieties
ADD CONSTRAINT FK_Properties_ShowTypes
FOREIGN KEY (id_ShowType) REFERENCES ShowTypes(id);

ALTER TABLE Proprieties
ADD CONSTRAINT FK_Properties_Controllers
FOREIGN KEY (id_controller) REFERENCES Controllers(id);

-- LinkProprietiesControllers Table
ALTER TABLE LinkProprietiesControllers
ADD CONSTRAINT FK_LinkProprietiesControllers_Properties
FOREIGN KEY (id_propriety) REFERENCES Proprieties(id);

ALTER TABLE LinkProprietiesControllers
ADD CONSTRAINT FK_LinkProprietiesControllers_Controllers
FOREIGN KEY (id_controller) REFERENCES Controllers(id);

-- ListVars Table
ALTER TABLE ListVars
ADD CONSTRAINT FK_ListVars_CSharpTypes
FOREIGN KEY (id_CSharpType) REFERENCES CSharpTypes(id);

ALTER TABLE ListVars
ADD CONSTRAINT FK_ListVars_LinkProprietiesControllers
FOREIGN KEY (id_link) REFERENCES LinkProprietiesControllers(id);

-- URLRoutes Table
ALTER TABLE URLRoutes
ADD CONSTRAINT FK_Routes_BaseRoutes
FOREIGN KEY (id_baseRoute) REFERENCES BaseRoutes(id);

ALTER TABLE URLRoutes
ADD CONSTRAINT FK_Routes_Controller
FOREIGN KEY (id_controller) REFERENCES Controllers(id);

ALTER TABLE URLRoutes
ADD CONSTRAINT FK_Routes_RouteTypes
FOREIGN KEY (id_routeType) REFERENCES RouteTypes(id);

ALTER TABLE URLRoutes
ADD CONSTRAINT FK_Routes_Proprieties
FOREIGN KEY (id_proprietyForUserID) REFERENCES Proprieties(id);

-- RouteQueries Table
ALTER TABLE RouteQueries
ADD CONSTRAINT FK_RouteQueries_QueryTypes
FOREIGN KEY (id_queryType) REFERENCES QueryTypes(id);

ALTER TABLE RouteQueries
ADD CONSTRAINT FK_RouteQueries_Routes
FOREIGN KEY (id_route) REFERENCES URLRoutes(id);
/*
-- UsersRoles Table
ALTER TABLE UsersRoles
ADD CONSTRAINT FK_UsersRoles_User
FOREIGN KEY (id_user) REFERENCES Users(id);
*/

ALTER TABLE UsersRoles
ADD CONSTRAINT FK_UsersRoles_Roles
FOREIGN KEY (id_role) REFERENCES Roles(id);

-- PermissionProprieties Table
ALTER TABLE PermissionProprieties
ADD CONSTRAINT FK_PermissionProprieties_Proprieties
FOREIGN KEY (id_propriety) REFERENCES Proprieties(id);

ALTER TABLE PermissionProprieties
ADD CONSTRAINT FK_PermissionProprieties_Roles
FOREIGN KEY (id_role) REFERENCES Roles(id);

-- PermissionRoutes Table
ALTER TABLE PermissionRoutes
ADD CONSTRAINT FK_PermissionRoutes_Routes
FOREIGN KEY (id_route) REFERENCES URLRoutes(id);

ALTER TABLE PermissionRoutes
ADD CONSTRAINT FK_PermissionRoutes_Roles
FOREIGN KEY (id_role) REFERENCES Roles(id);

-- SQLParamInfos Table
ALTER TABLE SQLParamInfos
ADD CONSTRAINT FK_SQLParamInfos_RouteQuery
FOREIGN KEY (id_RouteQuery) REFERENCES RouteQueries(id);

ALTER TABLE SQLParamInfos
ADD CONSTRAINT FK_SQLParamInfos_Proprieties
FOREIGN KEY (id_Propriety) REFERENCES Proprieties(id);

-- Filters Table
ALTER TABLE Filters
ADD CONSTRAINT FK_Filters_ShowTypes
FOREIGN KEY (id_ShowType) REFERENCES ShowTypes(id);

ALTER TABLE Filters
ADD CONSTRAINT FK_Filters_SQLParamInfos
FOREIGN KEY (id_SQLParamInfo) REFERENCES SQLParamInfos(id);

-- ValidatorSQLParamInfoValues Table
ALTER TABLE ValidatorSQLParamInfoValues
ADD CONSTRAINT FK_ValidatorSQLParamInfoValues_ValidatorTypes
FOREIGN KEY (id_ValidatorType) REFERENCES ValidatorTypes(id);

ALTER TABLE ValidatorSQLParamInfoValues
ADD CONSTRAINT FK_ValidatorSQLParamInfoValues_SQLParamInfos
FOREIGN KEY (id_SQLParamInfo) REFERENCES SQLParamInfos(id);

-- ValidatorProprietyValues Table
ALTER TABLE ValidatorProprietyValues
ADD CONSTRAINT FK_ValidatorProprietyValues_ValidatorTypes
FOREIGN KEY (id_ValidatorType) REFERENCES ValidatorTypes(id);

ALTER TABLE ValidatorProprietyValues
ADD CONSTRAINT FK_ValidatorProprietyValues_Proprieties
FOREIGN KEY (id_Propriety) REFERENCES Proprieties(id);
GO
-- Populating DataTypes
INSERT INTO DataTypes (name)
VALUES 
('INT'), ('VARCHAR'), ('DATE'), ('TEXT'), ('BIT'), 
('FLOAT');
GO
-- Populating Tables
INSERT INTO Tables (name, isView)
VALUES ('clients', 0), ('provinces', 0), ('villes', 0), 
       ('employes', 0), ('etats_commandes', 0), ('types_valeur', 0), 
       ('types_affectation', 0), ('types_preferences_graphique', 0), 
       ('couleurs', 0), ('preferences_graphiques', 0), 
       ('types_medias', 0), ('media', 0), ('reseaux_sociaux', 0), 
       ('compagnies', 0), ('collaborateurs', 0), ('etats_produit', 0), 
       ('categories', 0), ('types_format_produit', 0), 
       ('formats_produit', 0), ('fournisseurs', 0), 
       ('images_produit', 0), ('affectation_prix', 0), 
       ('produits', 0), ('commandes', 0), 
       ('produits_par_commande', 0), ('affectation_prix_lors_commande', 0), 
       ('affectation_prix_produits', 0), ('affectation_prix_commandes', 0), 
       ('affectation_prix_categorie', 0), 
       ('collaborateurs_reseaux_sociaux', 0), 
       ('collaborateurs_produits', 0), ('formats_produit_produits', 0), 
       ('fournisseurs_produits', 0), ('images_produit_produits', 0);
GO
-- Populating Colonnes
INSERT INTO Colonnes (name, id_table, id_datatype)
VALUES
-- clients table columns
('id', 1, 1), ('nom', 1, 2), ('prenom', 1, 2), ('date_naissance', 1, 3),
('adresse_courriel', 1, 2), ('mdp', 1, 4), ('token', 1, 4), ('sel', 1, 2),
('actif', 1, 5),

-- provinces table columns
('id', 2, 1), ('nom', 2, 2),

-- villes table columns
('id', 3, 1), ('nom', 3, 2), ('id_province', 3, 1),

-- employes table columns
('id', 4, 1), ('nom', 4, 2), ('prenom', 4, 2), ('date_naissance', 4, 3),
('adresse_courriel', 4, 2), ('mdp', 4, 4), ('token', 4, 4), ('sel', 4, 2),
('actif', 4, 5),

-- etats_commandes table columns
('id', 5, 1), ('nom', 5, 2), ('descriptions', 5, 4),

-- types_valeur table columns
('id', 6, 1), ('nom', 6, 2), ('descriptions', 6, 4),

-- types_affectation table columns
('id', 7, 1), ('nom', 7, 2), ('descriptions', 7, 4), ('facteur_affectation', 7, 5),

-- types_preferences_graphique table columns
('id', 8, 1), ('nom', 8, 2), ('code_html', 8, 2),

-- couleurs table columns
('id', 9, 1), ('nom', 9, 2), ('code_hex', 9, 2),

-- preferences_graphiques table columns
('id', 10, 1), ('nom', 10, 2), ('id_couleurs', 10, 1), ('id_types_preferences', 10, 1),

-- types_medias table columns
('id', 11, 1), ('nom', 11, 2), ('descriptions', 11, 4),

-- media table columns
('id', 12, 1), ('nom', 12, 2), ('liens', 12, 4), ('id_types_media', 12, 1),

-- reseaux_sociaux table columns
('id', 13, 1), ('nom', 13, 2),

-- compagnies table columns
('id', 14, 1), ('nom', 14, 2), ('telephone', 14, 2), ('adresse_courriel', 14, 2),
('contact', 14, 2),

-- collaborateurs table columns
('id', 15, 1), ('nom', 15, 2), ('prenom', 15, 2), ('telephone', 15, 2),
('adresse_courriel', 15, 2), ('id_compagnie', 15, 1),

-- etats_produit table columns
('id', 16, 1), ('nom', 16, 2), ('descriptions', 16, 4),

-- categories table columns
('id', 17, 1), ('nom', 17, 2), ('descriptions', 17, 4), ('id_categorie_mere', 17, 1),

-- types_format_produit table columns
('id', 18, 1), ('nom', 18, 2), ('descriptions', 18, 4),

-- formats_produit table columns
('id', 19, 1), ('nom', 19, 2), ('descriptions', 19, 4), ('id_type_format_produit', 19, 1),

-- fournisseurs table columns
('id', 20, 1), ('nom', 20, 2), ('telephone', 20, 2), ('adresse_courriel', 20, 2),
('contact', 20, 2),

-- images_produit table columns
('id', 21, 1), ('url', 21, 4), ('descriptions', 21, 4),

-- affectation_prix table columns
('id', 22, 1), ('nom', 22, 2), ('date_debut', 22, 3), ('date_fin', 22, 3),
('descriptions', 22, 4), ('id_types_valeur', 22, 1), ('id_types_affectation', 22, 1),

-- produits table columns
('id', 23, 1), ('nom', 23, 2), ('descriptions', 23, 4), ('prix', 23, 6),
('quantite_inventaire', 23, 1), ('id_categorie', 23, 1), ('id_etat_produit', 23, 1),

-- commandes table columns
('id', 24, 1), ('numero_facture', 24, 2), ('montant_brut', 24, 6),
('no_civique_livraison', 24, 1), ('rue_livraison', 24, 2), ('id_client', 24, 1),
('id_etat_commande', 24, 1), ('id_ville', 24, 1), ('id_employe', 24, 1),

-- produits_par_commande table columns
('id', 25, 1), ('id_produit', 25, 1), ('id_commande', 25, 1),
('quantite', 25, 1), ('prix_unitaire', 25, 6),

-- affectation_prix_lors_commande table columns
('id', 26, 1), ('id_produit_par_commande', 26, 1), ('id_affectation_prix', 26, 1),
('montant', 26, 6),

-- affectation_prix_produits table columns
('id', 27, 1), ('id_produit', 27, 1), ('id_affectation_prix', 27, 1), ('montant', 27, 6),

-- affectation_prix_commandes table columns
('id', 28, 1), ('id_commande', 28, 1), ('id_affectation_prix', 28, 1),

-- affectation_prix_categorie table columns
('id', 29, 1), ('id_categorie', 29, 1), ('id_affectation_prix', 29, 1),

-- collaborateurs_reseaux_sociaux table columns
('id', 30, 1), ('id_collaborateur', 30, 1), ('id_reseaux_sociaux', 30, 1), ('liens', 30, 4),

-- collaborateurs_produits table columns
('id', 31, 1), ('id_collaborateur', 31, 1), ('id_produit', 31, 1),

-- formats_produit_produits table columns
('id', 32, 1), ('id_format_produit', 32, 1), ('id_produit', 32, 1),

-- fournisseurs_produits table columns
('id', 33, 1), ('id_fournisseur', 33, 1), ('id_produit', 33, 1),

-- images_produit_produits table columns
('id', 34, 1), ('id_image_produit', 34, 1), ('id_produit', 34, 1);

GO
-- Insert data for foreign keys and their corresponding columns into Tables_Colonnes
INSERT INTO Tables_Colonnes (id_colonne, id_table)
VALUES
-- FK_villes_provinces
(14, 2), -- id_province in villes

-- FK_commandes_villes
(107, 2), -- id_ville in commandes

-- FK_commandes_clients
(105, 1), -- id_client in commandes

-- FK_commandes_employes
(108, 4), -- id_employe in commandes

-- FK_commandes_etats_commandes
(106, 5), -- id_etat_commande in commandes

-- FK_produits_par_commande_commandes
(111, 24), -- id_commande in produits_par_commande

-- FK_produits_par_commande_produits
(110, 23), -- id_produit in produits_par_commande

-- FK_affectation_prix_lors_commande_produits_par_commande
(115, 25), -- id_produit_par_commande in affectation_prix_lors_commande

-- FK_affectation_prix_lors_commande_affectation_prix
(116, 22), -- id_affectation_prix in affectation_prix_lors_commande

-- FK_affectation_prix_commandes_commandes
(123, 24), -- id_commande in affectation_prix_commandes

-- FK_affectation_prix_commandes_affectation_prix
(124, 22), -- id_affectation_prix in affectation_prix_commandes

-- FK_affectation_prix_produits_produits
(119, 23), -- id_produit in affectation_prix_produits

-- FK_affectation_prix_produits_affectation_commande
(120, 22), -- id_affectation_prix in affectation_prix_produits

-- FK_affectation_prix_categorie_categories
(126, 17), -- id_categorie in affectation_prix_categorie

-- FK_affectation_prix_categorie_affectation_prix
(127, 22), -- id_affectation_prix in affectation_prix_categorie

-- FK_affectation_prix_types_valeur
(91, 6), -- id_types_valeur in affectation_prix

-- FK_affectation_prix_types_affectation
(92, 7), -- id_types_affectation in affectation_prix

-- FK_preferences_graphiques_types_preferences_graphiques
(43, 8), -- id_types_preferences in preferences_graphiques

-- FK_preferences_graphiques_couleurs
(42, 9), -- id_couleurs in preferences_graphiques

-- FK_medias_types_media
(50, 11), -- id_types_media in media

-- FK_categories_sous_categories
(70, 14), -- id_categorie_mere in categories

-- FK_collaborateurs_reseaux_sociaux_collaborateurs
(129, 15), -- id_collaborateur in collaborateurs_reseaux_sociaux

-- FK_collaborateurs_reseaux_sociaux_reseaux_sociaux
(130, 13), -- id_reseaux_sociaux in collaborateurs_reseaux_sociaux

-- FK_collaborateurs_compagnies
(63, 14), -- id_compagnie in collaborateurs

-- FK_collaborateurs_produits_collaborateurs
(133, 15), -- id_collaborateur in collaborateurs_produits

-- FK_collaborateurs_produits_produits
(134, 23), -- id_produit in collaborateurs_produits

-- FK_produits_categories
(98, 17), -- id_categorie in produits

-- FK_produits_etats_produit
(99, 16), -- id_etat_produit in produits

-- FK_images_produit_produits_images_produit
(142, 21), -- id_image_produit in images_produit_produits

-- FK_images_produit_produits_produits
(143, 23), -- id_produit in images_produit_produits

-- FK_fournisseurs_produits_produits
(140, 23), -- id_produit in fournisseurs_produits

-- FK_fournisseurs_produits_fournisseurs
(139, 20), -- id_fournisseur in fournisseurs_produits

-- FK_formats_produit_produits_produits
(137, 23), -- id_produit in formats_produit_produits

-- FK_formats_produit_produits_formats_produit
(136, 19), -- id_format_produit in formats_produit_produits

-- FK_formats_produit_types_format_produit
(77, 18); -- id_type_format_produit in formats_produit

GO
INSERT BaseRoutes (name) 
VALUES 
(NULL), ('GetAll'), ('Get'), ('INSERT'), ('UPDATE'), ('GetAllDetailed'), ('GetDetailed'), ('CBO'), ('DELETE')
GO
INSERT CSharpTypes (name) 
VALUES 
('Reference'), ('String'), ('Int')
GO
INSERT RouteTypes (name) 
VALUES 
('GET'), ('POST'), ('PUT'), ('DELETE')
GO
INSERT ShowTypes (name) 
VALUES 
('Reference'), ('CBO'), ('String'), ('Int'), ('Float')
GO
INSERT ValidatorTypes (name) 
VALUES 
('Required'), ('Max'), ('Min')
GO
INSERT QueryTypes (name) 
VALUES 
('UPDATE'), ('SELECT'), ('ARRAY'), ('VALUE'), ('ROW'), ('CBO'), ('INSERT'), ('DELETE')
GO
INSERT Controllers (name, isMain)
VALUES 
('NULL', 0)
GO
INSERT Proprieties (name, isMain, id_ShowType, id_controller, isUpdatable)
VALUES 
('NULL', 0, 1, 1, 1)
GO
/*
INSERT URLRoutes (name, id_baseRoute, id_controller) VALUES (NULL, 1, 1)
GO
INSERT RouteQueries (ind, SQLString, id_queryType, id_route, completeCheck) 
VALUES 
(1, 'SELECT id FROM produits', 2, 1, 1),
(2, 'SELECT nom FROM produits WHERE id = @id', 2, 1, 1)
GO
*/
SELECT c.name, t1.name, d.name, t2.name FROM Colonnes c INNER JOIN DataTypes d ON d.id = c.id_datatype INNER JOIN Tables t1 ON t1.id = c.id_table  LEFT JOIN Tables_Colonnes tc ON tc.id_colonne = c.id LEFT JOIN Tables t2 ON t2.id = tc.id_table




SELECT * FROM Controllers
SELECT COALESCE(BaseRoutes.name, URLRoutes.name) AS Name, * FROM URLRoutes LEFT JOIN BaseRoutes ON BaseRoutes.id = URLRoutes.id_baseRoute
SELECT * FROM RouteQueries ORDER BY ind
SELECT * FROM SQLParamInfos