USE Projet_PSI;

SELECT * FROM Utilisateur; -- récupère tous les utilisateurs

SELECT Nom, Email FROM Utilisateur; -- on récupère juste le nom et l'email des utilisateurs

SELECT Id_cuisinier, Plat FROM Cuisinier; -- liste tous les cuisiniers et leurs plats 

SELECT * FROM Plat WHERE Prix < 20; -- on récupère les plats avec un prix inférieur à 20€

SELECT * FROM Commande WHERE Date__commande < '2024-12-06' ; -- on récupère les commandes passées après 2024

SELECT * FROM Avis WHERE Note = 5; -- on récupère les avis avec la note de 5

SELECT * FROM Plat WHERE Régime = 'Végétarien'; -- on récupère la liste des plats végétariens;

SELECT * FROM Commande WHERE Statut = 'En attente'; -- on récupère les commandes avec le statut en attente
