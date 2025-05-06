using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using Spectre.Console;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Bsi;
using Org.BouncyCastle.Crypto.Engines;
using Spectre.Console.Rendering;

namespace Projet {
    class InterfaceV2<T>{
        private MySqlConnection? maConnexion;
        private Graphe<T> Metro;

        public InterfaceV2(Graphe<T> Metro){
            this.Metro = Metro;
            InsertMetro();
            AnsiConsole.MarkupLine("[bold yellow]=== Connexion à la base de données ===[/]");

            var profil = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Choisissez votre [green]profil utilisateur[/] :").AddChoices("Client", "Cuisinier", "Administrateur", "[red]Quitter[/]")
            );

            switch (profil){
                case "Client": 
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=client;PASSWORD=motdepasse_client"); 
                    MenuClient();
                    break;
                case "Cuisinier":
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=cuisinier;PASSWORD=motdepasse_cuisinier"); 
                    MenuCuisinier();
                    break;
                case "Administrateur":
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=admin;PASSWORD=root"); 
                    break;
                default: break;
            }
        }

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

        public void MenuClient(){
            AnsiConsole.MarkupLine("[bold yellow]=== Client ===[/]");
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("").AddChoices("Inscription", "Connexion","[red]Quitter[/]")
            );

            switch(choix){
                case "Inscription":
                    Inscription(true);
                    break;
                case "Connexion":
                    break;
                case "Quitter":
                    break;
                default: break;
            }

        }

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
                    break;
                case "Quitter":
                    break;
                default: break;
            }
        }

        public void ConnexionDev(){

        }

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
    
        public void Connexion()
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
    
    
    }
}