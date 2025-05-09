using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using Spectre.Console;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Bsi;
using Org.BouncyCastle.Crypto.Engines;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Xml.Linq;
using System.Text.Json;

namespace Projet_Sarah_Maxence_Samuel{
    class Interface<T>{
        private MySqlConnection? maConnexion;
        private Graphe<T> Metro;

        /// <summary>
        /// constructeur de la classe
        /// </summary>
        /// <param name="Metro">graphe des métros</param>

        public Interface(Graphe<T> Metro){
            this.Metro = Metro;
            Run();
        }

        /// <summary>
        /// lancement de la console et de l'interface de connexion
        /// </summary>

        public void Run(){
            while(true){
                AnsiConsole.MarkupLine("[bold yellow]=== Connexion à la base de données ===[/]");

                var profil = AnsiConsole.Prompt(
                    new SelectionPrompt<string>().Title("Choisissez votre [green]profil utilisateur[/] :").AddChoices("Client", "Cuisinier", "Administrateur", "[red]Quitter[/]")
                );

                switch (profil){
                    case "Client": 
                        Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=client;PASSWORD=motdepasse_client"); 
                        InsertMetro();
                        MenuClient();
                        profil = null;
                        break;
                    case "Cuisinier":
                        Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=cuisinier;PASSWORD=motdepasse_cuisinier"); 
                        InsertMetro();
                        MenuCuisinier();
                        profil = null;
                        break;
                    case "Administrateur":
                        Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=admin;PASSWORD=root"); 
                        InsertMetro();
                        ConnexionDev();
                        profil = null;
                        break;
                    default: return;
                }
            }
        }

        #region Fonction

        /// <summary>
        /// Insert les metro dans la table metro si elle n'est pas deja remplie
        /// </summary>
        public void InsertMetro(){
            if(CountOfTable("MétroStation") <= 1){
                Console.WriteLine("Insertion des stations de metro");
                foreach(var noeud in Metro.Noeuds){
                    string req = "INSERT INTO MétroStation VALUES (" + noeud.IdNoeud + ", \"" + Convert.ToString(noeud.ValeurNoeud) + "\", \"" + noeud.CoX + "\", \"" + noeud.CoY + "\")";
                    MySqlCommand command = maConnexion.CreateCommand();
                    command.CommandText = req;
                    try{
                        command.ExecuteNonQuery();
                    }catch(MySqlException e){
                        Console.WriteLine("Erreur de connexion : " + e.ToString());
                        Console.ReadLine();
                        return;
                    }
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Renvoie le nombre de tuple dans une table
        /// </summary>
        /// <param name="TableName">Nom de la table</param>
        /// <returns>renvoie le compte</returns>
        public int CountOfTable(string TableName){
            int res = -1;
            string requete = "SELECT count(*) FROM " + TableName;
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            string[] valueString = new string[reader.FieldCount];
            while (reader.Read())
            {
                valueString[0] = reader.GetValue(0).ToString();
                res = Convert.ToInt32(valueString[0]); 
            }
            reader.Close();
            command.Dispose();
            return res;
        }

        /// <summary>
        /// Fait une requete a la base de donné"
        /// </summary>
        /// <param name="req">La requete</param>
        /// <returns>return le resultat sous la forme string[]</returns>
        public string[] Requete(string req){
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = req;
            MySqlDataReader reader = command.ExecuteReader();
            string[] valueString = new string[reader.FieldCount];
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString();
                }
            }
            reader.Close();
            command.Dispose();
            return valueString;
        }

        /// <summary>
        /// Modifie la station la plus porche d'un client ou d'un cuisinier
        /// </summary>
        /// <param name="type">client ou cuisinier</param>
        /// <param name="ID">Id_client ou Id_cuisinier</param>
        public void ChangeStation(string type, int ID)
        {
            var nomStation = AnsiConsole.Ask<string>("[yellow]Veuillez entrer le nom de la station de métro la plus proche de chez vous :[/]");
            int idStation = -1;

            foreach (var noeud in Metro.Noeuds)
            {
                if (Nettoyer(nomStation) == Nettoyer(Convert.ToString(noeud.ValeurNoeud)!))
                {
                    idStation = noeud.IdNoeud;
                    break;
                }
            }

            if (idStation == -1)
            {
                AnsiConsole.MarkupLine("[red]Nom de station invalide. Attention aux fautes d'orthographe.[/]");
                return;
            }

            string req = $"UPDATE {type} SET Id_station = {idStation} WHERE Id_utilisateur = {ID};";
            var command = maConnexion.CreateCommand();
            command.CommandText = req;

            try
            {
                command.ExecuteNonQuery();
                AnsiConsole.MarkupLine("[green]Station mise à jour avec succès.[/]");
            }
            catch (MySqlException e)
            {
                AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Permet a un client de devenir cuisinier et a un cuisinier de devneir client
        /// </summary>
        /// <param name="currenttype">type actuel</param>
        /// <param name="othertype">le type que l'on souhaite devenir</param>
        /// <param name="ID">Id_client ou Id_cuisinier</param>
        public void BecameOtherType(string currenttype, string othertype, int ID)
        {
            string query = $"SELECT * FROM {currenttype} WHERE Id_{currenttype.ToLower()} = {ID};";
            string[] res = Requete(query);

            if (res.Length < 3)
            {
                AnsiConsole.MarkupLine($"[red]Impossible de récupérer les données de l'utilisateur dans {currenttype}.[/]");
                return;
            }

            string insertQuery = $"INSERT INTO {othertype} VALUES ({CountOfTable(othertype)}, {res[1]}, {res[2]});";
            var command = maConnexion.CreateCommand();
            command.CommandText = insertQuery;

            try
            {
                command.ExecuteNonQuery();
                AnsiConsole.MarkupLine($"[green]Utilisateur converti en {othertype.ToLower()} avec succès ![/]");
            }
            catch (MySqlException e)
            {
                AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Affiche le trajet d'une commade
        /// </summary>
        /// <param name="IDClient">int</param>
        /// <param name="IDCuisinier">int</param>
        public void AfficherTrajet(int IDClient, int IDCuisinier){
            string req = "SELECT Id_station FROM Client WHERE Id_client = " + IDClient + ";";
            string req2 = "SELECT Id_station FROM Cuisinier WHERE Id_cuisinier = " + IDCuisinier + ";";
            int res = Convert.ToInt32(Requete(req)[0]);
            int res2 = Convert.ToInt32(Requete(req2)[0]);
            Metro.Distance(Metro.Noeuds[res - 1], Metro.Noeuds[res2 - 1]);
        }

        /// <summary>
        /// savoir si la connexion est réussie ou pas 
        /// </summary>
        /// <param name="connexionString">connexion</param>

        public void Connexion(string connexionString){
            try{
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
                AnsiConsole.MarkupLine("[bold green]Connexion réussie[/]");
            }catch(Exception e){
                Console.WriteLine("Erreur de connexion : " + e.Message);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Menu pour le client
        /// </summary>

        public void MenuClient(){
            while(true){
                AnsiConsole.MarkupLine("[bold yellow]=== Client ===[/]");
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>().Title("").AddChoices("Inscription", "Connexion","[red]Quitter[/]")
                );

                switch(choix){
                    case "Inscription":
                        Inscription(true);
                        break;
                    case "Connexion":
                        Connexion(true);
                        break;
                    case "[red]Quitter[/]":
                        return;
                    default: break;
                }
            }
        }

        /// <summary>
        /// menu pour le cuisinier
        /// </summary>

        public void MenuCuisinier(){
            AnsiConsole.MarkupLine("[bold yellow]=== Cuisinier ===[/]");
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("").AddChoices("Inscription", "Connexion","[red]Quitter[/]")
            );

            switch(choix){
                case "Inscription":
                    Inscription(false);
                    break;
                case "Connexion":
                    Connexion(false);
                    break;
                case "[red]Quitter[/]":
                    return;
                default: break;
            }
        }

        /// <summary>
        /// connexion pour le developpeur
        /// </summary>

        public void ConnexionDev(){
            var essai = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Entrez le mot de passe :[/]")
                    .PromptStyle("green")
                    .Secret()
            );

            if(essai == "root"){
                ModeDev();
            }else{
                AnsiConsole.MarkupLine("[red]Mot de passe invalide[/]");
            }
        }

        /// <summary>
        /// permet de mettre tous les caractères en forme pour comprendre, peu importe ce que la personne rentre en console (accent ou pas)
        /// </summary>
        /// <param name="texte">texte a modifier</param>
        /// <returns>le texte nettoyé</returns>

        private string Nettoyer(string texte) {
            return texte
                .Replace("é", "e").Replace("è", "e").Replace("ê", "e")
                .Replace("à", "a").Replace("â", "a").Replace("ç", "c")
                .Replace("'", " ").Replace("-", " ").Replace(" ", "")
                .ToLower();
        }

        /// <summary>
        /// Permet a un utilisateur de s'inscrire
        /// </summary>
        public void Inscription(bool client){
            var nom = AnsiConsole.Ask<string>("[yellow]Nom :[/]");
            var prenom = AnsiConsole.Ask<string>("[yellow]Prenom :[/]");
            var adresse = AnsiConsole.Ask<string>("[yellow]Adresse :[/]");
            var telephone = AnsiConsole.Ask<int>("[yellow]Téléphone :[/]");
            var email = AnsiConsole.Ask<string>("[yellow]Email :[/]");
            
            var mdp = AnsiConsole.Ask<string>("[yellow]Mot de passe :[/]");

            int numClient = CountOfTable("Utilisateur");

            string insertUtilisateur = "INSERT INTO Utilisateur VALUES (" + numClient + ", \"" + nom + "\", \"" + prenom + "\", \"" + adresse + "\", \"" + telephone + "\", \"" + email + "\", \"" + mdp + "\")";
            var command = maConnexion.CreateCommand();
            command.CommandText = insertUtilisateur;
            try {
                command.ExecuteNonQuery();
            } catch (MySqlException e) {
                AnsiConsole.MarkupLine("[red]Erreur lors de l'insertion de l'utilisateur :[/] " + e);
                return;
            }
            command.Dispose();

            int idStation = -1;
            do{
                var nomStation = AnsiConsole.Ask<string>("[yellow]Nom de la station de métro la plus proche :[/]");
                foreach (var noeud in Metro.Noeuds) {
                    if (Nettoyer(nomStation) == Nettoyer(Convert.ToString(noeud.ValeurNoeud)!)) {
                        idStation = noeud.IdNoeud;
                    }
                }
            }while(idStation == -1);
        
            try{
                string requete = "";
                if(!client){
                    requete = $"INSERT INTO Cuisinier VALUES ({CountOfTable("Cuisinier")}, {idStation}, {numClient});";
                }else{
                    requete = $"INSERT INTO Client VALUES ({CountOfTable("Client")}, {idStation}, {numClient});";
                }
                command = maConnexion.CreateCommand();
                command.CommandText = requete;
                command.ExecuteNonQuery();
                command.Dispose();
            } catch (MySqlException e) {
                AnsiConsole.MarkupLine("[red]Erreur lors de l'insertion dans la table client/cuisinier :[/] " + e);
            }
        }

        /// <summary>
        /// Permet a un client de se connecter
        /// </summary>
        /// <param name="client">client qui veut se connecter</param>
    
        public void Connexion(bool client)
        {
            var email = AnsiConsole.Ask<string>("[yellow]Entrez votre email :[/]");
            string req = $"SELECT Mot_de_passe FROM Utilisateur WHERE Email = \"{email}\";";
            string? mdp = Requete(req).FirstOrDefault();

            if (mdp == null)
            {
                AnsiConsole.MarkupLine("[red]Aucun compte trouvé avec cet email.[/]");
                return;
            }
            
            var essai = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Entrez votre mot de passe :[/]")
                    .PromptStyle("green")
                    .Secret());

            if (essai != mdp)
            {
                AnsiConsole.MarkupLine("[red]Mot de passe incorrect.[/]");
                return;
            }

            string req1 = $"SELECT COUNT(*) FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";";
            int isCuisinier = Convert.ToInt32(Requete(req1)[0]);

            string req2 = $"SELECT COUNT(*) FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";";
            int isClient = Convert.ToInt32(Requete(req2)[0]);

            if (isCuisinier == 1 && isClient == 0)
            {
                int idCuisinier = Convert.ToInt32(Requete($"SELECT Id_cuisinier FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";")[0]);
                ModeCuisinier(idCuisinier);
            }
            else if (isCuisinier == 0 && isClient == 1)
            {
                int idClient = Convert.ToInt32(Requete($"SELECT Id_client FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";")[0]);
                ModeClient(idClient);
            }
            else
            {
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Vous connectez en tant que :[/]")
                        .AddChoices("Client", "Cuisinier"));

                if (choix == "Client")
                {
                    int idClient = Convert.ToInt32(Requete($"SELECT Id_client FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";")[0]);
                    ModeClient(idClient);
                }
                else
                {
                    int idCuisinier = Convert.ToInt32(Requete($"SELECT Id_cuisinier FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"{email}\";")[0]);
                    ModeCuisinier(idCuisinier);
                }
            }
        }

        #endregion





        #region ModeClient

        /// <summary>
        /// permet de voir les plats
        /// </summary>
        /// <param name="ID">identifiant</param>
        public void SeePlat(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Liste des plats disponibles :[/]");

            string req1 = "SELECT Nom, Id_plat FROM Plat WHERE Nombre_pers > 0;";
            string[] res1 = Requete(req1);

            if (res1.Length == 0){
                AnsiConsole.MarkupLine("[red]Aucun plat disponible pour le moment.[/]");
                return;
            }

            var plats = new List<(string nom, string id)>();
            for (int i = 0; i < res1.Length / 2; i++)
                plats.Add((res1[i * 2], res1[i * 2 + 1]));

            var options = plats
                .Select(p => p.nom)
                .Concat(new[] { "[red]Quitter[/]" })
                .ToArray();

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez un plat ou [red]Quitter[/] :[/]")
                    .AddChoices(options)
            );

            if (choixNom == "[red]Quitter[/]")
                return;

            var platChoisi = plats.First(p => p.nom == choixNom);
            string req2 = $"SELECT Nombre_pers, Id_cuisinier, Prix FROM Plat WHERE Id_plat = {platChoisi.id};";
            string[] res2 = Requete(req2);

            int quantDispo = int.Parse(res2[0]);
            int quant;

            do{
                quant = AnsiConsole.Ask<int>($"[yellow]Quelle quantité souhaitez-vous ? Disponible : {quantDispo} (0 pour annuler)[/]");
                if (quant == 0){
                    AnsiConsole.MarkupLine("[grey]Commande annulée.[/]");
                    return;
                }
                if (quant < 0 || quant > quantDispo)
                    AnsiConsole.MarkupLine("[red]Quantité invalide, veuillez réessayer.[/]");
            } while (quant < 0 || quant > quantDispo);

            DateTime dateAujourdhui = DateTime.Today;
            string dateCommande = dateAujourdhui.ToString("dd/MM/yy");
            double prixUnitaire = Convert.ToDouble(res2[2]);
            double total = quant * prixUnitaire;

            string req3 = $"INSERT INTO Commande VALUES ({CountOfTable("Commande")}, {res2[1]}, {ID}, {platChoisi.id}, {quant}, \"{dateCommande}\", {total}, \"active\");";
            string req4 = $"UPDATE Plat SET Nombre_pers = Nombre_pers - {quant} WHERE Id_plat = {platChoisi.id};";

            try{
                using var cmd = maConnexion.CreateCommand();
                cmd.CommandText = req3;
                cmd.ExecuteNonQuery();

                cmd.CommandText = req4;
                cmd.ExecuteNonQuery();

                AnsiConsole.MarkupLine("[green]Commande enregistrée avec succès ![/]");
                AfficherTrajet(ID, Convert.ToInt32(res2[1]));
            }catch (MySqlException e){
                AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
            }
        }

        /// <summary>
        /// permet de voir la commande
        /// </summary>
        /// <param name="ID">identifiant</param>

        public void SeeCommande(int ID)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Vos commandes en cours :[/]");
        
            string req1 = $"SELECT Id_plat, Id_commande FROM Commande WHERE Id_client = {ID} AND Statut = \"active\";";
            string[] res1 = Requete(req1);

            if (res1.Length == 0 || res1[0] == null){
                AnsiConsole.MarkupLine("[red]Aucune commande en cours.[/]");
                return;
            }

            var commandes = new List<(string nom, string id)>();
            for (int i = 0; i < res1.Length / 2; i++){
                string req2 = $"SELECT Nom FROM Plat WHERE Id_plat = {res1[i * 2]};";
                string[] res2 = Requete(req2);
                commandes.Add((res2[0], res1[i * 2 + 1]));
            }

            var options = commandes
                .Select(c => c.nom)
                .Concat(new[] { "[red]Quitter[/]" })
                .ToArray();

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez une commande pour plus d'infos ou [red]Quitter[/] :[/]")
                    .AddChoices(options)
            );

            if (choixNom == "[red]Quitter[/]")
                return;

            var commandeChoisie = commandes.First(c => c.nom == choixNom);
            string req3 = $"SELECT * FROM Commande WHERE Id_commande = {commandeChoisie.id};";
            string[] res3 = Requete(req3);

            AnsiConsole.MarkupLine($@"
                [bold]Nom du plat :[/]    {commandeChoisie.nom}
                [bold]Quantité :[/]       {res3[4]}
                [bold]Date commande :[/] {res3[5]}
                [bold]Prix :[/]           {res3[6]}
            ");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Voir le trajet", "Supprimer la commande", "[red]Retour[/]")
            );

            switch (choixAction) {
                case "Voir le trajet":
                    AfficherTrajet(ID, Convert.ToInt32(res3[1]));
                    break;

                case "Supprimer la commande":
                    string req4 = $"UPDATE Commande SET Statut = \"supprimer\" WHERE Id_commande = {commandeChoisie.id};";
                    string req5 = $"UPDATE Plat SET Nombre_pers = Nombre_pers + {res3[4]} WHERE Id_plat = {res3[0]};";
                    try{
                        using var cmd = maConnexion.CreateCommand();
                        cmd.CommandText = req4;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = req5;
                        cmd.ExecuteNonQuery();

                        AnsiConsole.MarkupLine("[green]Commande supprimée avec succès ![/]");
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;

                case "[red]Retour[/]":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

        /// <summary>
        /// affichage console du module client
        /// </summary>
        /// <param name="IDClient">identifiant du client</param>

        public void ModeClient(int IDClient)
        {
            while (true)
            {
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Client[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Voir les plats",
                            "Voir mes commandes",
                            "Devenir cuisinier",
                            "Changer de station de métro",
                            "Voir la carte",
                            "[red]Quitter[/]")
                );

                switch (choix)
                {
                    case "Voir les plats":
                        SeePlat(IDClient);
                        break;
                    case "Voir mes commandes":
                        SeeCommande(IDClient);
                        break;
                    case "Devenir cuisinier":
                        BecameOtherType("Client", "Cuisinier", IDClient);
                        break;
                    case "Changer de station de métro":
                        ChangeStation("Client", IDClient);
                        break;
                    case "Voir la carte":
                        AfficherTrajet(IDClient, IDClient);
                        break;
                    case "[red]Quitter[/]":
                        AnsiConsole.MarkupLine("[green]Au revoir ![/]");
                        return;
                }
            }
        }
        #endregion





        #region ModeCuisinier

        /// <summary>
        /// inserer un plat
        /// </summary>
        /// <param name="ID">identifiant</param>
        public void InsererPlat(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Ajouter un nouveau plat[/]");

            string nom = AnsiConsole.Ask<string>("[green]Nom du plat :[/]");
            string type = AnsiConsole.Ask<string>("[green]Type de plat :[/]");
            int nbPart = AnsiConsole.Ask<int>("[green]Nombre de parts :[/]");
            string datefab = AnsiConsole.Ask<string>("[green]Date de fabrication (jj/mm/aa) :[/]");
            string dateper = AnsiConsole.Ask<string>("[green]Date de péremption (jj/mm/aa) :[/]");
            double prix = AnsiConsole.Ask<double>("[green]Prix d'une part :[/]");
            string nationalite = AnsiConsole.Ask<string>("[green]Nationalité du plat :[/]");

            int choix;
            do{
                choix = AnsiConsole.Prompt(
                    new SelectionPrompt<int>()
                        .Title("[green]Régime du plat :[/]")
                        .AddChoices(1, 2)
                );
            } while (choix != 1 && choix != 2);

            string regime = choix == 1 ? "vegetarien" : "carnivore";

            string ingredient = AnsiConsole.Ask<string>("[green]Liste des ingrédients du plat (riz, carotte, etc...) :[/]");

            string req = $"INSERT INTO Plat VALUES ({CountOfTable("Plat")}, {ID}, \"{nom}\", \"{type}\", {nbPart}, \"{datefab}\", \"{dateper}\", {prix}, \"{nationalite}\", \"{regime}\", \"{ingredient}\");";

            try{
                MySqlCommand command = maConnexion.CreateCommand();
                command.CommandText = req;
                command.ExecuteNonQuery();
                command.Dispose();

                AnsiConsole.MarkupLine("[green]Plat ajouté avec succès ![/]");
            }catch (MySqlException e){
                AnsiConsole.MarkupLine($"[red]Erreur de connexion :[/] {e.Message}");
            }catch (Exception e){
                AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
            }
        }

        /// <summary>
        /// gerer les plats
        /// </summary>
        /// <param name="ID">identifiants</param>

        public void GererPlat(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Liste des plats disponibles :[/]");

            string req1 = $"SELECT Nom, Id_plat FROM Plat WHERE Nombre_pers > 0 AND Id_cuisinier = {ID};";
            string[] res1 = Requete(req1);

            if (res1.Length == 0){
                AnsiConsole.MarkupLine("[red]Aucun plat disponible à gérer.[/]");
                return;
            }

            var plats = new List<(string nom, string id)>();
            for (int i = 0; i < res1.Length / 2; i++)
                plats.Add((res1[i * 2], res1[i * 2 + 1]));

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez un plat à gérer ou [red]Quitter[/] :[/]")
                    .AddChoices(plats.Select(p => p.nom).Concat(new[] { "[red]Quitter[/]" }))
            );

            if (choixNom == "[red]Quitter[/]")
                return;

            var platChoisi = plats.First(p => p.nom == choixNom);
            string req2 = $"SELECT * FROM Plat WHERE Id_plat = {platChoisi.id};";
            string[] res2 = Requete(req2);

            AnsiConsole.MarkupLine($"[bold]Nom plat :[/] {res2[2]}\n[bold]Quantité disponible :[/] {res2[4]}");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Supprimer le plat", "[red]Retour[/]")
            );

            switch (choixAction){
                case "Supprimer le plat":
                    string req3 = $"UPDATE Plat SET Nombre_pers = 0 WHERE Id_plat = {platChoisi.id};";
                    try{
                        MySqlCommand command = maConnexion.CreateCommand();
                        command.CommandText = req3;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        AnsiConsole.MarkupLine("[green]Plat supprimé avec succès ![/]");
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }catch (Exception e){
                        AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour[/]":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

        /// <summary>
        /// gerer les commmandes 
        /// </summary>
        /// <param name="ID">identifiant</param>

        public void GererCommande(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Vos commandes en cours :[/]");

            string req1 = $"SELECT Id_plat, Id_commande FROM Commande WHERE Id_cuisinier = {ID} AND Statut = \"active\";";
            string[] res1 = Requete(req1);

            var res2 = new List<string[]>();
            for (int i = 0; i < res1.Length / 2; i++){
                string req2 = $"SELECT Nom FROM Plat WHERE Id_plat = {res1[i * 2]};";
                res2.Add(Requete(req2));
            }

            if (res2.Count == 0){
                AnsiConsole.MarkupLine("[red]Aucune commande en cours.[/]");
                return;
            }

            var noms = res2.Select(r => r[0]).Concat(new[] { "[red]Quitter[/]" }).ToArray();
            var choixCommande = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez une commande ou [red]Quitter[/] :[/]")
                    .AddChoices(noms)
            );
            if (choixCommande == "[red]Quitter[/]")
                return;

            int rang = res2.FindIndex(r => r[0] == choixCommande);
            string req3 = $"SELECT * FROM Commande WHERE Id_commande = {res1[rang * 2 + 1]};";
            string[] res3 = Requete(req3);

            AnsiConsole.MarkupLine($"[bold]Nom plat :[/] {res2[rang][0]}\n[bold]Quantité :[/] {res3[4]}\n[bold]Date commande :[/] {res3[5]}\n[bold]Prix :[/] {res3[6]}\n");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Voir le trajet", "Supprimer la commande", "[red]Retour[/]")
            );

            switch (choixAction){
                case "Voir le trajet":
                    AfficherTrajet(ID, Convert.ToInt32(res3[1]));
                    break;
                case "Supprimer la commande":
                    string req4 = $"UPDATE Commande SET Statut = \"supprimer\" WHERE Id_commande = {res1[rang * 2 + 1]};";
                    MySqlCommand command = maConnexion.CreateCommand();
                    command.CommandText = req4;
                    try{
                        command.ExecuteNonQuery();
                        command.Dispose();
                        string req5 = $"UPDATE Plat SET Nombre_pers = Nombre_pers + {res3[4]} WHERE Id_plat = {res1[rang * 2]};";
                        command = maConnexion.CreateCommand();
                        command.CommandText = req5;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        AnsiConsole.MarkupLine("[green]Commande supprimée avec succès ![/]");
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }catch (Exception e){
                        AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour[/]":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

        /// <summary>
        /// affichage console du mode cuisinier
        /// </summary>
        /// <param name="IDCuisinier">identifiant du cuisinier</param>

        public void ModeCuisinier(int IDCuisinier)
        {
            while (true)
            {
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Cuisinier[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Proposer un plat",
                            "Gérer mes plats",
                            "Gérer mes commandes",
                            "Devenir client",
                            "Changer de station de métro",
                            "[red]Quitter[/]")
                );

                switch (choix)
                {
                    case "Proposer un plat":
                        InsererPlat(IDCuisinier);
                        break;
                    case "Gérer mes plats":
                        GererPlat(IDCuisinier);
                        break;
                    case "Gérer mes commandes":
                        GererCommande(IDCuisinier);
                        break;
                    case "Devenir client":
                        BecameOtherType("Cuisinier", "Client", IDCuisinier);
                        break;
                    case "Changer de station de métro":
                        ChangeStation("Cuisinier", IDCuisinier);
                        break;
                    case "[red]Quitter[/]":
                        AnsiConsole.MarkupLine("[green]Au revoir ![/]");
                        return;
                }
            }
        }
        #endregion





        #region ModeDev

        /// <summary>
        /// affichage console du mode developpeur
        /// </summary>
        public void ModeDev(){
            while (true){
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Administrateur[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Ajouter un retard sur le réseaux de transport",
                            "Graphe relationnel",
                            "Gestion des Clients",
                            "Gestion des Cuisiniers",
                            "Gestion des Commandes",
                            "Statistiques",
                            "[red]Quitter[/]")
                );

                switch (choix){
                    case "Gestion des Clients":
                        ModuleClient();
                        break;
                    case "Gestion des Cuisiniers":
                        ModuleCuisinier();
                        break;
                    case "Gestion des Commandes":
                        ModuleCommande();
                        break;
                    case "Statistiques":
                        ModuleStatistiques();
                        break;
                    case "Ajouter un retard sur le réseaux de transport":
                        ModuleReseau();
                        break;
                    case "Graphe relationnel":
                        ModuleGrapheRelationnel();
                        break;
                    case "[red]Quitter[/]":
                        AnsiConsole.MarkupLine("[green]Au revoir ![/]");
                        return;
                    default: break;
                }
            }
        }

        /// <summary>
        /// affichage console pour le graphe relationnel
        /// </summary>

        private void ModuleGrapheRelationnel(){
            while (true){
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Administrateur[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Generer le graphe",
                            "Colorer le graphe",
                            "Generer un fichier xml",
                            "Generer un fichier Json",
                            "[red]Quitter[/]")
                );

                switch (choix){
                    case "Generer le graphe":
                        ConstruireGrapheRelations(false);
                        break;
                    case "Colorer le graphe":
                        ConstruireGrapheRelations(true);
                        break;
                    case "Generer un fichier xml":
                        ExporterGrapheRelationnelEnXml("GrapheXml.xml");
                        break;
                    case "Generer un fichier Json":
                        ExporterGrapheRelationnelEnJson("GrapheJson.json");
                        break;
                    case "[red]Quitter[/]":
                        AnsiConsole.MarkupLine("[green]Au revoir ![/]");
                        return;
                    default: break;
                }
            }
        }

        /// <summary>
        /// affichage console pour le réseau
        /// </summary>

        private void ModuleReseau(){
            var station1 = AnsiConsole.Ask<string>("[yellow]Entrez le nom de la station de départ de la liaison sur laquelle vous souhaitez ajouté un retard[/]");

            int idStation1 = -1;
            foreach (var noeud in Metro.Noeuds){
                if (Nettoyer(station1) == Nettoyer(Convert.ToString(noeud.ValeurNoeud)!)){
                    idStation1 = noeud.IdNoeud;
                    break;
                }
            }

            var station2 = AnsiConsole.Ask<string>("[yellow]Entrez le nom de la station d'arrivé de la liaison sur laquelle vous souhaitez ajouté un retard[/]");
        
            int idStation2 = -1;
            foreach (var noeud in Metro.Noeuds){
                if (Nettoyer(station2) == Nettoyer(Convert.ToString(noeud.ValeurNoeud)!)){
                    idStation2 = noeud.IdNoeud;
                    break;
                }
            }

            var retard = AnsiConsole.Ask<int>("[yellow]Entrez la valeur du retard (en min)[/]");

            Metro.ModifiateAdj(idStation1, idStation2, retard);
        }

        /// <summary>
        /// affichage console pour le module client
        /// </summary>

        private void ModuleClient()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des clients[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Afficher les clients",
                        "Clients par ordre alphabétique",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Clients par ordre alphabétique":
                    string requete = @"SELECT u.Id_utilisateur, u.Nom, u.`Prénom` AS Prenom FROM client c JOIN utilisateur u ON c.Id_utilisateur = u.Id_utilisateur ORDER BY u.Nom ASC;";

                    try{
                        var command = maConnexion.CreateCommand();
                        command.CommandText = requete;

                        using var reader = command.ExecuteReader();
                        var clients = new List<(string IdUtilisateur, string Nom, string Prenom)>();
                        while (reader.Read())
                        {
                            string idUtil  = reader.GetString("Id_utilisateur");
                            string nom     = reader.GetString("Nom");
                            string prenom  = reader.GetString("Prenom");
                            clients.Add((idUtil, nom, prenom));
                        }

                        AnsiConsole.MarkupLine("[bold yellow]Liste des clients par ordre alphabétique :[/]");
                        foreach (var (id, nom, prenom) in clients)
                        {
                            AnsiConsole.MarkupLine($"[green]ID Utilisateur:[/] {id} [bold]Nom:[/] {nom} [bold]Prénom:[/] {prenom}");
                        }
                    }
                    catch (MySqlException e)
                    {
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "Afficher les clients":
                    requete = @"SELECT u.Id_utilisateur, u.Nom, u.`Prénom` AS Prenom FROM client c JOIN utilisateur u ON c.Id_utilisateur = u.Id_utilisateur;";
                    try{
                        var command = maConnexion.CreateCommand();
                        command.CommandText = requete;

                        using var reader = command.ExecuteReader();
                        var clients = new List<(string IdUtilisateur, string Nom, string Prenom)>();
                        while (reader.Read()){
                            string idUtil  = reader.GetString("Id_utilisateur");
                            string nom     = reader.GetString("Nom");
                            string prenom  = reader.GetString("Prenom");
                            clients.Add((idUtil, nom, prenom));
                        }

                        AnsiConsole.MarkupLine("[bold yellow]Liste des clients :[/]");
                        foreach (var (id, nom, prenom) in clients){
                            AnsiConsole.MarkupLine($"[green]ID Utilisateur:[/] {id} [bold]Nom:[/] {nom} [bold]Prénom:[/] {prenom}");
                        }
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        /// <summary>
        /// affichage console pour le module cuisinier
        /// </summary>

        private void ModuleCuisinier()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des cuisiniers[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Afficher les cuisiniers et leurs plats",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Afficher les cuisiniers et leurs plats":
                    string requete = @"SELECT cu.Id_cuisinier, u.Id_utilisateur, u.Nom, u.`Prénom` AS Prenom, p.Id_plat, p.Nom AS PlatNom FROM cuisinier cu JOIN utilisateur u ON cu.Id_utilisateur = u.Id_utilisateur LEFT JOIN plat p ON cu.Id_cuisinier = p.Id_cuisinier ORDER BY u.Nom, u.`Prénom`, p.Nom;";

                    try{
                        using var command = maConnexion.CreateCommand();
                        command.CommandText = requete;

                        using var reader = command.ExecuteReader();
                        var dict = new Dictionary<int, (string Nom, string Prenom, List<(int IdPlat,string PlatNom)> Plats)>();

                        while (reader.Read()){
                            int idCuisinier = reader.GetInt32("Id_cuisinier");
                            string nom  = reader.GetString("Nom");
                            string prenom = reader.GetString("Prenom");
                            
                            int? idPlat = reader.IsDBNull(reader.GetOrdinal("Id_plat")) ? (int?)null : reader.GetInt32("Id_plat");
                            string platNom = reader.IsDBNull(reader.GetOrdinal("PlatNom")) ? null : reader.GetString("PlatNom");

                            if (!dict.ContainsKey(idCuisinier))
                                dict[idCuisinier] = (nom, prenom, new List<(int, string)>());

                            if (idPlat.HasValue)
                                dict[idCuisinier].Plats.Add((idPlat.Value, platNom));
                        }

                        reader.Close();

                        AnsiConsole.MarkupLine("[bold yellow]Liste des cuisiniers et de leurs plats :[/]");
                        foreach (var kv in dict){
                            var (nom, prenom, plats) = kv.Value;
                            AnsiConsole.MarkupLine($"\n[underline]{nom} {prenom} (ID cuisinier: {kv.Key})[/]");
                            if (plats.Count == 0){
                                AnsiConsole.MarkupLine("  [grey]— Aucun plat proposé[/]");
                            }else{
                                foreach (var (idPlat, platNom) in plats)
                                    AnsiConsole.MarkupLine($"  • [green]{platNom}[/] (ID plat: {idPlat})");
                            }
                        }
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        /// <summary>
        /// affichage console pour la gestion des commandes
        /// </summary>

        private void ModuleCommande()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des commandes[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Afficher la liste des commandes en cours",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Afficher la liste des commandes en cours":
                    string requete = @"SELECT co.Id_commande, p.Nom AS PlatNom, ucl.Nom AS ClientNom, ucl.`Prénom` AS ClientPrenom, ucui.Nom AS CuisinierNom, ucui.`Prénom` AS CuisinierPrenom, co.Quantité, co.Date_commande FROM commande co JOIN plat p ON co.Id_plat = p.Id_plat JOIN client cl ON co.Id_client = cl.Id_client JOIN utilisateur ucl ON cl.Id_utilisateur = ucl.Id_utilisateur JOIN cuisinier cui ON co.Id_cuisinier = cui.Id_cuisinier JOIN utilisateur ucui ON cui.Id_utilisateur = ucui.Id_utilisateur WHERE co.Statut = 'active' ORDER BY co.Date_commande DESC;";

                    try{
                        using var cmd    = maConnexion.CreateCommand();
                        cmd.CommandText = requete;

                        using var reader = cmd.ExecuteReader();
                        AnsiConsole.MarkupLine("[bold yellow]Commandes en cours :[/]");
                        while (reader.Read()){
                            int    idCmd      = reader.GetInt32("Id_commande");
                            string platNom    = reader.GetString("PlatNom");
                            string cliNom     = reader.GetString("ClientNom");
                            string cliPrenom  = reader.GetString("ClientPrenom");
                            string cuiNom     = reader.GetString("CuisinierNom");
                            string cuiPrenom  = reader.GetString("CuisinierPrenom");
                            int    quantite   = reader.GetInt32("Quantité");
                            string dateCmd    = reader.GetString("Date_commande");

                            AnsiConsole.MarkupLine(
                                $"• [green]#{idCmd}[/] {platNom} — " +
                                $"Client: {cliNom} {cliPrenom}, " +
                                $"Cuisinier: {cuiNom} {cuiPrenom}, " +
                                $"Quantité: {quantite}, Date: {dateCmd}");
                        }
                        reader.Close();
                    }catch(MySqlException e) {
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        /// <summary>
        /// affichage console pour le module statistiques
        /// </summary>
        private void ModuleStatistiques()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de statistiques[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Moyenne du nombre de commande par cuisinier",
                        "Commandes pour un jour",
                        "Moyenne des prix des commandes",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Moyenne du nombre de commande par cuisinier":
                    string requete = @"SELECT AVG(nb) FROM (SELECT COUNT(*) AS nb FROM commande GROUP BY Id_cuisinier) AS sub;";

                    try{
                        var commande = maConnexion.CreateCommand();
                        commande.CommandText = requete;

                        object result = commande.ExecuteScalar();
                        double moyenne = result == DBNull.Value ? 0 : Convert.ToDouble(result);

                        AnsiConsole.MarkupLine($"[bold yellow]Moyenne de commandes par cuisinier :[/] [green]{moyenne:F2}[/]");
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "Commandes pour un jour":
                    var dateStr = AnsiConsole.Ask<string>("[yellow]Entrez la date des commandes à afficher (jj/mm/aa) :[/]");

                    string req = $"SELECT Id_commande, Id_cuisinier, Id_client, Id_plat, Quantité, Date_commande, Prix, Statut " + $"FROM Commande " + $"WHERE Date_commande = '{dateStr}';";
                
                    var command = maConnexion.CreateCommand();
                    command.CommandText = req;
                
                    try{
                        using var reader = command.ExecuteReader();
                        if (!reader.HasRows){
                            AnsiConsole.MarkupLine("[red]Aucune commande trouvée pour cette date.[/]");
                            return;
                        }
                
                        AnsiConsole.MarkupLine($"[bold green]Commandes du {dateStr} :[/]");
                        while (reader.Read()){
                            var idCom   = reader.GetInt32("Id_commande");
                            var idCu   = reader.GetInt32("Id_cuisinier");
                            var idCl   = reader.GetInt32("Id_client");
                            var idPl   = reader.GetInt32("Id_plat");
                            var qte    = reader.GetInt32("Quantité");
                            var prix   = reader.GetDouble("Prix");
                            var statut = reader.GetString("Statut");
                
                            AnsiConsole.MarkupLine(
                                $"• [blue]Commande #{idCom}[/] – Cuisinier : {idCu}, Client : {idCl}, Plat : {idPl}, Qté : {qte}, Prix : {prix}€, Statut : {statut}"
                            );
                        }
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }finally{
                        command.Dispose();
                    }
                    break;
                case "Moyenne des prix des commandes":
                    requete = @"SELECT AVG(Prix) FROM commande;";

                    try{
                        using var commande = maConnexion.CreateCommand();
                        commande.CommandText = requete;

                        object result = commande.ExecuteScalar();
                        double moyenne = result == DBNull.Value ? 0 : Convert.ToDouble(result);

                        AnsiConsole.MarkupLine($"[bold yellow]Moyenne du prix des commandes :[/] [green]{moyenne:F2}[/] €");
                    }catch (MySqlException e){
                        AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
                    }
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }
        #endregion  

        /// <summary>
        /// construire les graphes des relations
        /// </summary>
        /// <param name="color">couleur du graphe</param>
        public void ConstruireGrapheRelations(bool color){

            List<Noeud<string>> noeuds = new List<Noeud<string>>();
            List<Lien<string>> liens = new List<Lien<string>>();
            Dictionary<string, Noeud<string>> mapNoeuds = new Dictionary<string, Noeud<string>>();
            Dictionary<int, Tuple<double, double>> positionsStations = new Dictionary<int, Tuple<double, double>>();
            
            using (MySqlConnection connexion = maConnexion){
                try{
                    string requeteStations = "SELECT Id_station, Longitude, Latitude FROM MétroStation;";
                    MySqlCommand commandeStation = connexion.CreateCommand();
                    commandeStation.CommandText = requeteStations;
                    MySqlDataReader lecteurStation = commandeStation.ExecuteReader();
                    while (lecteurStation.Read()){
                        int id = Convert.ToInt32(lecteurStation["Id_station"]);

                        double longitude = Convert.ToDouble(lecteurStation["Longitude"].ToString(), CultureInfo.InvariantCulture);
                        double latitude = Convert.ToDouble(lecteurStation["Latitude"].ToString(), CultureInfo.InvariantCulture);

                        positionsStations[id] = new Tuple<double, double>(latitude, longitude);
                    }
                    lecteurStation.Close();
                    commandeStation.Dispose();

                    string requeteRelations = "SELECT Id_client, Id_cuisinier FROM Commande;";
                    MySqlCommand commandeRelations = connexion.CreateCommand();
                    commandeRelations.CommandText = requeteRelations;
                    MySqlDataReader lecteurRelations = commandeRelations.ExecuteReader();

                    List<Tuple<string, string>> relations = new List<Tuple<string, string>>();
                    HashSet<string> listeClients = new HashSet<string>();
                    HashSet<string> listeCuisiniers = new HashSet<string>();

                    while (lecteurRelations.Read()){
                        string idClient = lecteurRelations["Id_client"].ToString();
                        string idCuisinier = lecteurRelations["Id_cuisinier"].ToString();

                        relations.Add(new Tuple<string, string>(idClient, idCuisinier));
                        listeClients.Add(idClient);
                        listeCuisiniers.Add(idCuisinier);
                    }
                    lecteurRelations.Close();
                    commandeRelations.Dispose();

                    Dictionary<string, int> stationClient = new Dictionary<string, int>();
                    if (listeClients.Count > 0){
                        string requeteClients = "SELECT Id_client, Id_station FROM Client;";
                        MySqlCommand commandeClients = connexion.CreateCommand();
                        commandeClients.CommandText = requeteClients;
                        MySqlDataReader lecteurClients = commandeClients.ExecuteReader();

                        while (lecteurClients.Read())
                        {
                            string id = lecteurClients["Id_client"].ToString();
                            int idStation = Convert.ToInt32(lecteurClients["Id_station"]);
                            stationClient[id] = idStation;
                        }

                        lecteurClients.Close();
                        commandeClients.Dispose();
                    }

                    Dictionary<string, int> stationCuisinier = new Dictionary<string, int>();
                    if (listeCuisiniers.Count > 0){
                        string requeteCuisiniers = "SELECT Id_cuisinier, Id_station FROM Cuisinier;";
                        MySqlCommand commandeCuisiniers = connexion.CreateCommand();
                        commandeCuisiniers.CommandText = requeteCuisiniers;
                        MySqlDataReader lecteurCuisiniers = commandeCuisiniers.ExecuteReader();

                        while (lecteurCuisiniers.Read()){
                            string id = lecteurCuisiniers["Id_cuisinier"].ToString();
                            int idStation = Convert.ToInt32(lecteurCuisiniers["Id_station"]);
                            stationCuisinier[id] = idStation;
                        }

                        lecteurCuisiniers.Close();
                        commandeCuisiniers.Dispose();
                    }

                    foreach (Tuple<string, string> relation in relations){
                        string nomClient = "Client_" + relation.Item1;
                        string nomCuisinier = "Cuisinier_" + relation.Item2;

                        if (!mapNoeuds.ContainsKey(nomClient)){
                            double x = 0;
                            double y = 0;
                            if (stationClient.ContainsKey(relation.Item1) && positionsStations.ContainsKey(stationClient[relation.Item1])){
                                x = positionsStations[stationClient[relation.Item1]].Item1;
                                y = positionsStations[stationClient[relation.Item1]].Item2;
                            }
                            Noeud<string> noeudClient = new Noeud<string>(mapNoeuds.Count + 1, nomClient, x, y);

                            noeuds.Add(noeudClient);
                            mapNoeuds[nomClient] = noeudClient;
                        }

                        if (!mapNoeuds.ContainsKey(nomCuisinier)){
                            double x = 0;
                            double y = 0;
                            if (stationCuisinier.ContainsKey(relation.Item2) && positionsStations.ContainsKey(stationCuisinier[relation.Item2])){
                                x = positionsStations[stationCuisinier[relation.Item2]].Item1;
                                y = positionsStations[stationCuisinier[relation.Item2]].Item2;
                            }
                            Noeud<string> noeudCuisinier = new Noeud<string>(mapNoeuds.Count + 1, nomCuisinier, x, y);

                            noeuds.Add(noeudCuisinier);
                            mapNoeuds[nomCuisinier] = noeudCuisinier;
                        }

                        Lien<string> lien = new Lien<string>(mapNoeuds[nomClient], mapNoeuds[nomCuisinier], 1);
                        liens.Add(lien);
                    }
                }catch (MySqlException e){
                    Console.WriteLine("Erreur SQL lors de la construction du graphe : " + e.ToString());
                }
            }
            Graphe<string> grapheRelations = new Graphe<string>(false, noeuds, liens);
            Bitmap img;
            if(color){
                img = grapheRelations.DrawGrapheColored();
            }else{
                img = grapheRelations.DrawGraphe();
            }
            string path = "graphe_relations.png";
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        /// <summary>
        /// exporter le graphe relationnel en xml
        /// </summary>
        /// <param name="filePath">chemin du fichier</param>
    
        public void ExporterGrapheRelationnelEnXml(string filePath){
            var noeuds = new List<Noeud<string>>();
            var liens  = new List<Lien<string>>();
            var map    = new Dictionary<string, Noeud<string>>();
            var positionsStations = new Dictionary<int, (double X, double Y)>();

            using (var cmdPos = maConnexion.CreateCommand()){
                cmdPos.CommandText = "SELECT Id_station, Latitude, Longitude FROM MétroStation;";
                using var rdPos = cmdPos.ExecuteReader();
                while (rdPos.Read()){
                    int id    = rdPos.GetInt32("Id_station");
                    double lat = rdPos.GetDouble("Latitude");
                    double lon = rdPos.GetDouble("Longitude");
                    positionsStations[id] = (lon, lat);
                }
            }

            var relations = new List<(string client, string cuisinier)>();
            using (var cmdRel = maConnexion.CreateCommand()){
                cmdRel.CommandText = "SELECT Id_client, Id_cuisinier FROM commande;";
                using var rdRel = cmdRel.ExecuteReader();
                while (rdRel.Read())
                    relations.Add((
                        rdRel.GetInt32("Id_client").ToString(),
                        rdRel.GetInt32("Id_cuisinier").ToString()
                    ));
            }

            var stationClient = new Dictionary<string,int>();
            using (var cmdC = maConnexion.CreateCommand()){
                cmdC.CommandText = "SELECT Id_client, Id_station FROM client;";
                using var rdC = cmdC.ExecuteReader();
                while (rdC.Read())
                    stationClient[ rdC.GetInt32("Id_client").ToString() ] = rdC.GetInt32("Id_station");
            }

            var stationCui = new Dictionary<string,int>();
            using (var cmdCu = maConnexion.CreateCommand()){
                cmdCu.CommandText = "SELECT Id_cuisinier, Id_station FROM cuisinier;";
                using var rdCu = cmdCu.ExecuteReader();
                while (rdCu.Read())
                    stationCui[ rdCu.GetInt32("Id_cuisinier").ToString() ] = rdCu.GetInt32("Id_station");
            }

            foreach (var (cli, cui) in relations){
                string nomClient = "Client_" + cli;
                if (!map.ContainsKey(nomClient)){
                    var (x,y) = stationClient.TryGetValue(cli, out var s) && positionsStations.TryGetValue(s, out var p)
                               ? p : (0.0,0.0);
                    var n = new Noeud<string>(map.Count+1, nomClient, x, y);
                    noeuds.Add(n);
                    map[nomClient] = n;
                }

                string nomCui = "Cuisinier_" + cui;
                if (!map.ContainsKey(nomCui)){
                    var (x,y) = stationCui.TryGetValue(cui, out var s) && positionsStations.TryGetValue(s, out var p)
                               ? p : (0.0,0.0);
                    var n = new Noeud<string>(map.Count+1, nomCui, x, y);
                    noeuds.Add(n);
                    map[nomCui] = n;
                }

                liens.Add(new Lien<string>(map[nomClient], map[nomCui], 1));
            }

            var doc = new XDocument(
                new XElement("GrapheRelationnel",
                    new XElement("Noeuds",
                        from n in noeuds
                        select new XElement("Noeud",
                            new XAttribute("Id",    n.IdNoeud),
                            new XAttribute("Nom",   n.ValeurNoeud),
                            new XAttribute("X",     n.CoX),
                            new XAttribute("Y",     n.CoY)
                        )
                    ),
                    new XElement("Liens",
                        from l in liens
                        select new XElement("Lien",
                            new XAttribute("Depart",  l.Depart.IdNoeud),
                            new XAttribute("Arrivee", l.Arrivee.IdNoeud),
                            new XAttribute("Poids",   l.Poids)
                        )
                    )
                )
            );

            doc.Save(filePath);
            AnsiConsole.MarkupLine($"[green]Export XML généré : {filePath}[/]");
        }

        /// <summary>
        /// exporter le graphe en Json
        /// </summary>
        /// <param name="filePath">chemin du fichier</param>
        public void ExporterGrapheRelationnelEnJson(string filePath){
            var noeuds = new List<object>();
            var liens  = new List<object>();
            var map    = new Dictionary<string,int>();

            var positionsStations = new Dictionary<int,(double X,double Y)>();
            using (var cmd = maConnexion.CreateCommand()){
                cmd.CommandText = "SELECT Id_station, Latitude, Longitude FROM MétroStation;";
                using var rd = cmd.ExecuteReader();
                while(rd.Read()){
                    int    id  = rd.GetInt32("Id_station");
                    double lat = rd.GetDouble("Latitude");
                    double lon = rd.GetDouble("Longitude");
                    positionsStations[id] = (lon, lat);
                }
            }

            var relations = new List<(string cli,string cui)>();
            using (var cmd = maConnexion.CreateCommand()){
                cmd.CommandText = "SELECT Id_client, Id_cuisinier FROM commande;";
                using var rd = cmd.ExecuteReader();
                while(rd.Read())
                    relations.Add((
                        rd.GetInt32("Id_client").ToString(),
                        rd.GetInt32("Id_cuisinier").ToString()
                    ));
            }

            var stationClient = new Dictionary<string,int>();
            using (var cmd = maConnexion.CreateCommand()){
                cmd.CommandText = "SELECT Id_client, Id_station FROM client;";
                using var rd = cmd.ExecuteReader();
                while(rd.Read())
                    stationClient[ rd.GetInt32("Id_client").ToString() ]
                        = rd.GetInt32("Id_station");
            }

            var stationCuisinier = new Dictionary<string,int>();
            using (var cmd = maConnexion.CreateCommand()){
                cmd.CommandText = "SELECT Id_cuisinier, Id_station FROM cuisinier;";
                using var rd = cmd.ExecuteReader();
                while(rd.Read())
                    stationCuisinier[ rd.GetInt32("Id_cuisinier").ToString() ]
                        = rd.GetInt32("Id_station");
            }

            foreach (var (cli, cui) in relations){
                string nomCli = "Client_" + cli;
                if (!map.ContainsKey(nomCli)){
                    var (x, y) = positionsStations.TryGetValue(
                        stationClient.GetValueOrDefault(cli), out var p) ? p : (0.0, 0.0);
                    int id = map.Count + 1;
                    map[nomCli] = id;
                    noeuds.Add(new { Id = id, Nom = nomCli, X = x, Y = y });
                }

                string nomCui = "Cuisinier_" + cui;
                if (!map.ContainsKey(nomCui)){
                    var (x, y) = positionsStations.TryGetValue(
                        stationCuisinier.GetValueOrDefault(cui), out var p) ? p : (0.0, 0.0);
                    int id = map.Count + 1;
                    map[nomCui] = id;
                    noeuds.Add(new { Id = id, Nom = nomCui, X = x, Y = y });
                }

                liens.Add(new {
                    Depart  = map[nomCli],
                    Arrivee = map[nomCui],
                    Poids   = 1f
                });
            }

            var graphe = new {
                Noeuds = noeuds,
                Liens  = liens
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(graphe, options);
            File.WriteAllText(filePath, json);

            AnsiConsole.MarkupLine($"[green]JSON exporté : {filePath}[/]");
        }

    }
}