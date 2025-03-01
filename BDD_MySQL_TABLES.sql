DROP DATABASE IF EXISTS projet_psi;
CREATE DATABASE Projet_PSI;
USE Projet_PSI;

CREATE TABLE IF NOT EXISTS Utilisateur(
   Id_utilisateur VARCHAR(50),
   Nom VARCHAR(50),
   Prénom VARCHAR(50) NOT NULL,
   Addresse VARCHAR(70) NOT NULL,
   Téléphone INT NOT NULL,
   Email VARCHAR(70) NOT NULL,
   Mot_de_passe VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_utilisateur)
);

CREATE TABLE IF NOT EXISTS Lignemétro(
   Id_ligne VARCHAR(50),
   Num_ligne INT NOT NULL,
   Liste_stations VARCHAR(2000) NOT NULL,
   PRIMARY KEY(Id_ligne)
);

CREATE TABLE IF NOT EXISTS MétroStation(
   Id_station INT NOT NULL,
   Nom_station VARCHAR(50) NOT NULL,
   Latitude VARCHAR(50) NOT NULL,
   Longitude VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_station)
);


CREATE TABLE IF NOT EXISTS Cuisinier(
   Id_cuisinier VARCHAR(50),
   Id_station INT NOT NULL,
   Id_utilisateur VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_cuisinier),
   UNIQUE(Id_utilisateur),
   FOREIGN KEY(Id_station) REFERENCES MétroStation(Id_station),
   FOREIGN KEY(Id_utilisateur) REFERENCES Utilisateur(Id_utilisateur)
);


CREATE TABLE IF NOT EXISTS Client(
   Id_client VARCHAR(50),
   Type VARCHAR(50) NOT NULL,
   Nom_entreprise VARCHAR(50),
   Nom_referent VARCHAR(50),
   Id_station INT NOT NULL,
   Id_utilisateur VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_client),
   UNIQUE(Id_utilisateur),
   FOREIGN KEY(Id_station) REFERENCES MétroStation(Id_station),
   FOREIGN KEY(Id_utilisateur) REFERENCES Utilisateur(Id_utilisateur)
);

CREATE TABLE IF NOT EXISTS Plat(
   Id_plat VARCHAR(50),
   Id_cuisinier VARCHAR(50),
   Nom VARCHAR(50) NOT NULL,
   Type VARCHAR(50) NOT NULL,
   Nombre_pers INT NOT NULL,
   Date_fabrication DATE NOT NULL,
   Date_peremption DATE NOT NULL,
   Prix DECIMAL(15,2) NOT NULL,
   Nationalité VARCHAR(50) NOT NULL,
   Régime VARCHAR(50) NOT NULL,
   Ingrédients VARCHAR(50) NOT NULL,
   Photo VARCHAR(250),
   Id_cuisinier_1 VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_plat),
   FOREIGN KEY(Id_cuisinier_1) REFERENCES Cuisinier(Id_cuisinier)
);

CREATE TABLE Commande(
   Id_commande VARCHAR(50),
   Id_cuisinier VARCHAR(50) NOT NULL,
   Id_client VARCHAR(50) NOT NULL,
   Date__commande VARCHAR(50) NOT NULL,
   Prix DECIMAL(15,2) NOT NULL,
   Statut VARCHAR(50),
   PRIMARY KEY(Id_commande),
   FOREIGN KEY(Id_client) REFERENCES Client(Id_client),
   FOREIGN KEY(Id_cuisinier) REFERENCES Cuisinier(Id_cuisinier)
);

CREATE TABLE LigneCommande(
   Id_lignecommande VARCHAR(50),
   Quantité INT NOT NULL,
   Date_livraison DATE NOT NULL,
   Lieu_livraison_ VARCHAR(50) NOT NULL,
   Id_plat VARCHAR(50) NOT NULL,
   Id_commande VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_lignecommande),
   FOREIGN KEY(Id_plat) REFERENCES Plat(Id_plat),
   FOREIGN KEY(Id_commande) REFERENCES Commande(Id_commande)
);

CREATE TABLE IF NOT EXISTS Livraison(
   Id_livraison VARCHAR(50),
   Id_cuisinier VARCHAR(50),
   Id_commande VARCHAR(50),
   Adresse_départ VARCHAR(50) NOT NULL,
   Adresse_arrivée VARCHAR(50) NOT NULL,
   Date_heure_arrivée DATETIME NOT NULL,
   Distance DECIMAL(15,2) NOT NULL,
   Id_commande_1 VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_livraison),
   UNIQUE(Id_commande_1),
   FOREIGN KEY(Id_commande_1) REFERENCES Commande(Id_commande)
);

CREATE TABLE IF NOT EXISTS HistoriqueTransactions(
   Id_transaction VARCHAR(50),
   Id_cuisinier VARCHAR(50),
   Id_client VARCHAR(50),
   Montant DECIMAL(15,2) NOT NULL,
   Date_transaction DATE NOT NULL,
   Id_cuisinier_1 VARCHAR(50) NOT NULL,
   Id_client_1 VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_transaction),
   UNIQUE(Id_cuisinier_1),
   UNIQUE(Id_client_1),
   FOREIGN KEY(Id_cuisinier_1) REFERENCES Cuisinier(Id_cuisinier),
   FOREIGN KEY(Id_client_1) REFERENCES Client(Id_client)
);

CREATE TABLE IF NOT EXISTS Avis(
   Id_avis VARCHAR(50),
   Id_client VARCHAR(50),
   Id_cuisinier VARCHAR(50),
   Note INT NOT NULL,
   Commentaire VARCHAR(200),
   Date_avis DATE NOT NULL,
   Id_client_1 VARCHAR(50) NOT NULL,
   Id_cuisinier_1 VARCHAR(50) NOT NULL,
   PRIMARY KEY(Id_avis),
   FOREIGN KEY(Id_client_1) REFERENCES Client(Id_client),
   FOREIGN KEY(Id_cuisinier_1) REFERENCES Cuisinier(Id_cuisinier)
);

CREATE TABLE IF NOT EXISTS Contenir(
   Id_plat VARCHAR(50),
   Id_lignecommande VARCHAR(50),
   PRIMARY KEY(Id_plat, Id_lignecommande),
   FOREIGN KEY(Id_plat) REFERENCES Plat(Id_plat),
   FOREIGN KEY(Id_lignecommande) REFERENCES LigneCommande(Id_lignecommande)
);

CREATE TABLE IF NOT EXISTS Effectuer(
   Id_cuisinier VARCHAR(50),
   Id_livraison VARCHAR(50),
   PRIMARY KEY(Id_cuisinier, Id_livraison),
   FOREIGN KEY(Id_cuisinier) REFERENCES Cuisinier(Id_cuisinier),
   FOREIGN KEY(Id_livraison) REFERENCES Livraison(Id_livraison)
);

INSERT INTO MétroStation VALUES (0, "République", "1","1");
INSERT INTO Utilisateur VALUES (0, "Dupond", "Marie", "30 Rue de la République, 75011 Paris", 1234567890, "Mdupond@gmail.com", "MotDePasse");
INSERT INTO Cuisinier VALUES (1, 0, 0);

INSERT INTO MétroStation VALUES (1, "Cardinet", "2","2");
INSERT INTO Utilisateur VALUES (1, "Durand", "Medhy", "15 Rue Cardinet, 75017 Paris", 1234567890, "Mdurand@gmail.com", "MotDePasse");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (1, "Particulier", 1, 1);

INSERT INTO Commande VALUES (1, 1, 1,"01/01/25", 10, "En cours");
INSERT INTO Commande VALUES (2, 1, 1,"02/02/25", 5, "En cours");












