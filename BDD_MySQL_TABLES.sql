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
   Metro_plus_proche VARCHAR(50) NOT NULL,
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

INSERT INTO MétroStation VALUES (1, "Porte Maillot", "48.87816265269652","2.282583847361549");
INSERT INTO Utilisateur VALUES (1, "Dupond", "Marie", "30 Rue de la République, 75011 Paris", 1234567890, "Mdupond@gmail.com", "DupMar75011", "Porte Maillot");
INSERT INTO Cuisinier VALUES (1, 1, 1);

INSERT INTO MétroStation VALUES (2, "Argentine", "48.87566737565167","2.2894354185422134");
INSERT INTO Utilisateur VALUES (2, "Durand", "Medhy", "15 Rue Cardinet, 75017 Paris", 1234567890, "Mdurand@gmail.com", "DurMed75017","Argentine");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (2, "Particulier", 2, 2);

INSERT INTO MétroStation VALUES (3, "Charles de Gaulle - Etoile", "48.874994575223035", "2.295811775235762");
INSERT INTO Utilisateur VALUES (3, "Martin", "Lucie", "45 Avenue des Champs-Élysées, 75008 Paris", 1234567891, "Lucie.martin@gmail.com", "MarLuc75008","Charles de Gaulle - Etoile");
INSERT INTO Cuisinier VALUES (3, 3, 3);

INSERT INTO MétroStation VALUES (4, "George V", "48.87203776364103", "2.3007597849789168");
INSERT INTO Utilisateur VALUES (4, "Bernard", "Louis", "12 Rue de Rivoli, 75004 Paris", 1234567892, "Louis.bernard@gmail.com", "BerLou75004","George V");
INSERT INTO Client VALUES (4, "Entreprise", "TechCorp", "Jean Dupuis", 4, 4);

INSERT INTO MétroStation VALUES (5, "Franklin D. Roosevelt", "48.868724887050476", "2.309488192337112");
INSERT INTO Utilisateur VALUES (5, "Dubois", "Sophie", "7 Rue Lafayette, 75009 Paris", 1234567893, "Sophie.dubois@gmail.com", "DubSoph75009","Franklin D. Roosevelt");
INSERT INTO Cuisinier VALUES (5, 5, 5);

INSERT INTO MétroStation VALUES (6, "Champs-Elysées - Clemenceau", "48.86765629124574", "2.31446450132278");
INSERT INTO Utilisateur VALUES (6, "Morel", "Pierre", "3 Rue Oberkampf, 75011 Paris", 1234567894, "Pierre.morel@gmail.com", "MorPie75011","Champs-Elysées - Clemenceau");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (6, "Particulier", 6, 6);

INSERT INTO MétroStation VALUES (7, "Concorde", "48.866557992001624", "2.3229614457982586");
INSERT INTO Utilisateur VALUES (7, "Lemoine", "Claire", "10 Avenue Montaigne, 75008 Paris", 1234567895, "Claire.lemoine@gmail.com", "LemCla75008","Concorde");
INSERT INTO Cuisinier VALUES (7, 7, 7);

INSERT INTO MétroStation VALUES (8, "Tuileries", "48.86447783836653", "2.3296780947116407");
INSERT INTO Utilisateur VALUES (8, "Fontaine", "Lucas", "88 Rue de la Pompe, 75016 Paris", 1234567896, "Lucas.fontaine@gmail.com", "FonLuc75016","Tuileries");
INSERT INTO Client VALUES (8, "Entreprise", "Boulangerie Fine", "Elodie Marchand", 8, 8);

INSERT INTO MétroStation VALUES (9, "Palais Royal - Musée du Louvre", "48.86222226462601", "2.3364543677542504");
INSERT INTO Utilisateur VALUES (9, "Perrot", "Julie", "15 Rue Saint-Honoré, 75001 Paris", 1234567897, "Julie.perrot@gmail.com", "PerJul75001","Palais Royal - Musée du Louvre");
INSERT INTO Cuisinier VALUES (9, 9, 9);

INSERT INTO MétroStation VALUES (10, "Louvre - Rivoli", "48.860871211759445", "2.3409696232852686");
INSERT INTO Utilisateur VALUES (10, "Renaud", "Alexandre", "20 Boulevard Haussmann, 75009 Paris", 1234567898, "Alexandre.renaud@gmail.com", "RenAle75009","Louvre - Rivoli");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (10, "Particulier", 10, 10);

INSERT INTO MétroStation VALUES (11, "Châtelet", "48.856953459837165", "2.3481609912345767");
INSERT INTO Utilisateur VALUES (11, "Blanc", "Manon", "5 Quai de la Seine, 75019 Paris", 1234567899, "Manon.blanc@gmail.com", "BlaMan75019","Châtelet");
INSERT INTO Cuisinier VALUES (11, 11, 11);

INSERT INTO MétroStation VALUES (12, "Hôtel de Ville", "48.857352404237716", "2.3520676701390992");
INSERT INTO Utilisateur VALUES (12, "Giraud", "Hugo", "18 Rue de Rennes, 75006 Paris", 1234567800, "Hugo.giraud@gmail.com", "GirHug75006","Hôtel de Ville");
INSERT INTO Client VALUES (12, "Entreprise", "Café du Marché", "Laurent Petit", 12, 12);

INSERT INTO MétroStation VALUES (13, "Saint-Paul (Le Marais)", "48.855187420656264", "2.3608852562751954");
INSERT INTO Utilisateur VALUES (13, "Collet", "Emma", "25 Rue Saint-Antoine, 75004 Paris", 1234567801, "Emma.collet@gmail.com", "ColEmm75004","Saint-Paul (Le Marais)");
INSERT INTO Cuisinier VALUES (13, 13, 13);

INSERT INTO MétroStation VALUES (14, "Bastille", "48.85205429254951", "2.3687189610340877");
INSERT INTO Utilisateur VALUES (14, "Marchand", "Thomas", "30 Avenue Ledru-Rollin, 75011 Paris", 1234567802, "Thomas.marchand@gmail.com", "MarTho75011","Bastille");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (14, "Particulier", 14, 14);

INSERT INTO MétroStation VALUES (15, "Gare de Lyon", "48.845683205787566", "2.373156593789204");
INSERT INTO Utilisateur VALUES (15, "Chauvet", "Camille", "40 Boulevard Diderot, 75012 Paris", 1234567803, "Camille.chauvet@gmail.com", "ChaCam75012","Gare de Lyon");
INSERT INTO Cuisinier VALUES (15, 15, 15);

INSERT INTO MétroStation VALUES (16, "Reuilly - Diderot", "48.84721292574974", "2.3872010704093913");
INSERT INTO Utilisateur VALUES (16, "Robert", "Nicolas", "12 Rue de Charenton, 75012 Paris", 1234567804, "Nicolas.robert@gmail.com", "RobNic75012","Reuilly - Diderot");
INSERT INTO Client VALUES (16, "Entreprise", "Le Petit Déjeuner", "Céline Moreau", 16, 16);

INSERT INTO MétroStation VALUES (17, "Nation", "48.84808428902576", "2.395843988723737");
INSERT INTO Utilisateur VALUES (17, "Garcia", "Alice", "78 Rue des Pyrénées, 75020 Paris", 1234567805, "Alice.garcia@gmail.com", "GarAli75020","Nation");
INSERT INTO Cuisinier VALUES (17, 17, 17);

INSERT INTO MétroStation VALUES (18, "Porte de Vincennes", "48.847007730140405", "2.4108049967014993");
INSERT INTO Utilisateur VALUES (18, "Fernandez", "Julien", "10 Place de la Nation, 75012 Paris", 1234567806, "Julien.fernandez@gmail.com", "FerJul75012","Porte de Vincennes");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (18, "Particulier", 18, 18);

INSERT INTO MétroStation VALUES (19, "Château de Vincennes", "48.844317513365304", "2.4405400954061127");
INSERT INTO Utilisateur VALUES (19, "Leroy", "Pauline", "33 Avenue de Paris, 94300 Vincennes", 1234567807, "Pauline.leroy@gmail.com", "LerPau94300","Château de Vincennes");
INSERT INTO Cuisinier VALUES (19, 19, 19);

INSERT INTO MétroStation VALUES (20, "Porte Dauphine", "48.871396794300246", "2.2776327175452704");
INSERT INTO Utilisateur VALUES (20, "Richard", "Antoine", "2 Boulevard Flandrin, 75016 Paris", 1234567808, "Antoine.richard@gmail.com", "RicAnt75016","Porte Dauphine");
INSERT INTO Client VALUES (20, "Entreprise", "Gourmet Express", "Vincent Dubreuil", 20, 20);

INSERT INTO MétroStation VALUES (21, "Victor Hugo", "48.86992608146256", "2.285828765912979");
INSERT INTO Utilisateur VALUES (21, "Leclerc", "Isabelle", "5 Rue Lauriston, 75016 Paris", 1234567809, "Isabelle.leclerc@gmail.com", "LecIsa75016","Victor Hugo");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (21, "Particulier", 21, 21);

INSERT INTO MétroStation VALUES (22, "Charles de Gaulle - Etoile", "48.874994575223035", "2.295811775235762");
INSERT INTO Utilisateur VALUES (22, "Dupuy", "Arthur", "10 Avenue de Wagram, 75017 Paris", 1234567810, "Arthur.dupuy@gmail.com", "DupArt75017","Charles de Gaulle - Etoile");
INSERT INTO Cuisinier VALUES (22, 22, 22);

INSERT INTO MétroStation VALUES (23, "Ternes", "48.878227729914364", "2.298113288617243");
INSERT INTO Utilisateur VALUES (23, "Gautier", "Emma", "22 Rue Poncelet, 75017 Paris", 1234567811, "Emma.gautier@gmail.com", "GauEmm75017","Ternes");
INSERT INTO Client VALUES (23, "Entreprise", "Bistro Gourmand", "Paul Lambert", 23, 23);

INSERT INTO MétroStation VALUES (24, "Courcelles", "48.8792652531651", "2.303294362425212");
INSERT INTO Utilisateur VALUES (24, "Bertrand", "Lucas", "35 Boulevard de Courcelles, 75008 Paris", 1234567812, "Lucas.bertrand@gmail.com", "BerLuc75008","Courcelles");
INSERT INTO Cuisinier VALUES (24, 24, 24);

INSERT INTO MétroStation VALUES (25, "Monceau", "48.88056876917883", "2.3094129673747017");
INSERT INTO Utilisateur VALUES (25, "Charpentier", "Léa", "12 Avenue Van Dyck, 75008 Paris", 1234567813, "Lea.charpentier@gmail.com", "ChaLea75008","Monceau");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (25, "Particulier", 25, 25);

INSERT INTO MétroStation VALUES (26, "Villiers", "48.88107318212763", "2.3158150926463765");
INSERT INTO Utilisateur VALUES (26, "Renault", "Nicolas", "8 Rue de Levis, 75017 Paris", 1234567814, "Nicolas.renault@gmail.com", "RenNic75017","Villiers");
INSERT INTO Cuisinier VALUES (26, 26, 26);

INSERT INTO MétroStation VALUES (27, "Rome", "48.88234581784814", "2.3213591738229717");
INSERT INTO Utilisateur VALUES (27, "Moulin", "Charlotte", "44 Rue de Rome, 75008 Paris", 1234567815, "Charlotte.moulin@gmail.com", "MouCha75008","Rome");
INSERT INTO Client VALUES (27, "Entreprise", "La Table de Paris", "François Berger", 27, 27);

INSERT INTO MétroStation VALUES (28, "Place de Clichy", "48.88366908732468", "2.327958328010219");
INSERT INTO Utilisateur VALUES (28, "Durant", "Hugo", "18 Boulevard de Clichy, 75009 Paris", 1234567816, "Hugo.durant@gmail.com", "DurHug75009","Place de Clichy");
INSERT INTO Cuisinier VALUES (28, 28, 28);

INSERT INTO MétroStation VALUES (29, "Blanche", "48.88376635352917", "2.332484375434362");
INSERT INTO Utilisateur VALUES (29, "Fabre", "Clémence", "6 Rue Lepic, 75018 Paris", 1234567817, "Clemence.fabre@gmail.com", "FabCle75018","Blanche");
INSERT INTO Client (Id_client, Type, Id_station, Id_utilisateur) VALUES (29, "Particulier", 29, 29);

INSERT INTO MétroStation VALUES (30, "Pigalle", "48.882020931119335", "2.3372111647011273");
INSERT INTO Utilisateur VALUES (30, "Giraudet", "Baptiste", "50 Boulevard de Rochechouart, 75009 Paris", 1234567818, "Baptiste.giraudet@gmail.com", "GirBap75009","Pigalle");
INSERT INTO Cuisinier VALUES (30, 30, 30);

INSERT INTO Commande VALUES (2, 3, 2, "02/01/25", 15, "En cours de livraison");
INSERT INTO Commande VALUES (3, 5, 4, "03/01/25", 20, "Livrée");
INSERT INTO Commande VALUES (4, 7, 6, "04/01/25", 25, "Payée");
INSERT INTO Commande VALUES (5, 9, 8, "05/01/25", 12, "En cours");
INSERT INTO Commande VALUES (6, 11, 10, "06/01/25", 18, "En cours de livraison");
INSERT INTO Commande VALUES (7, 13, 12, "07/01/25", 22, "Livrée");
INSERT INTO Commande VALUES (8, 15, 14, "08/01/25", 30, "Payée");
INSERT INTO Commande VALUES (9, 17, 16, "09/01/25", 14, "En cours");
INSERT INTO Commande VALUES (10, 19, 18, "10/01/25", 16, "En cours de livraison");
INSERT INTO Commande VALUES (11, 22, 20, "11/01/25", 24, "Livrée");
INSERT INTO Commande VALUES (12, 24, 21, "12/01/25", 28, "Payée");
INSERT INTO Commande VALUES (13, 26, 23, "13/01/25", 32, "En cours");
INSERT INTO Commande VALUES (14, 28, 25, "14/01/25", 26, "En cours de livraison");
INSERT INTO Commande VALUES (15, 30, 27, "15/01/25", 35, "Livrée");

















