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

namespace Projet {
    class InterfaceV2<T>{
        private MySqlConnection? maConnexion;
        private Graphe<T> Metro;

        public InterfaceV2(Graphe<T> Metro){
            this.Metro = Metro;
            Run();
        }

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

        public void SeePlat(int ID)
        {
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

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez un plat pour commander ou [red]Échap[/] pour quitter :[/]")
                    .AddChoices(plats.Select(p => p.nom))
            );

            var platChoisi = plats.First(p => p.nom == choixNom);
            string req2 = $"SELECT Nombre_pers, Id_cuisinier, Prix FROM Plat WHERE Id_plat = {platChoisi.id};";
            string[] res2 = Requete(req2);

            int quantDispo = int.Parse(res2[0]);
            int quant;

            do{
                quant = AnsiConsole.Ask<int>($"[yellow]Quelle quantité souhaitez-vous ? Disponible : {quantDispo}[/]");
                if (quant <= 0 || quant > quantDispo)
                    AnsiConsole.MarkupLine("[red]Quantité invalide, veuillez réessayer.[/]");
            }while (quant <= 0 || quant > quantDispo);

            DateTime dateAujourdhui = DateTime.Today;
            string dateCommande = dateAujourdhui.ToString("dd/MM/yy");
            double prixUnitaire = Convert.ToDouble(res2[2]);
            double total = quant * prixUnitaire;

            string req3 = $"INSERT INTO Commande VALUES ({CountOfTable("Commande")}, {res2[1]}, {ID}, {platChoisi.id}, {quant}, \"{dateCommande}\", {total}, \"active\");";
            string req4 = $"UPDATE Plat SET Nombre_pers = Nombre_pers - {quant} WHERE Id_plat = {platChoisi.id};";

            try{
                var command = maConnexion.CreateCommand();
                command.CommandText = req3;
                command.ExecuteNonQuery();
                command.Dispose();

                command = maConnexion.CreateCommand();
                command.CommandText = req4;
                command.ExecuteNonQuery();
                command.Dispose();

                AnsiConsole.MarkupLine("[green]Commande enregistrée avec succès ![/]");
                AfficherTrajet(ID, Convert.ToInt32(res2[1]));
            }
            catch (MySqlException e){
                AnsiConsole.MarkupLine($"[red]Erreur SQL :[/] {e.Message}");
            }
            catch (Exception e){
                AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
            }
        }

        public void SeeCommande(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Vos commandes en cours :[/]");

            string req1 = "SELECT Id_plat, Id_commande FROM Commande WHERE Id_client = " + ID + " AND Statut = \"active\";";
            string[] res1 = Requete(req1);
            Console.WriteLine(res1.Length);
            if (res1[0] == null){
                AnsiConsole.MarkupLine("[red]Aucune commande en cours.[/]");
                return;
            }

            var commandes = new List<(string nom, string id)>();
            for (int i = 0; i < res1.Length / 2; i++){
                string req2 = $"SELECT Nom FROM Plat WHERE Id_plat = {res1[i * 2]};";
                string[] res2 = Requete(req2);
                commandes.Add((res2[0], res1[i * 2 + 1]));
            }

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez une commande pour plus d'infos ou [red]Échap[/] pour quitter :[/]")
                    .AddChoices(commandes.Select(c => c.nom))
            );

            var commandeChoisie = commandes.First(c => c.nom == choixNom);
            string req3 = $"SELECT * FROM Commande WHERE Id_commande = {commandeChoisie.id};";
            string[] res3 = Requete(req3);

            AnsiConsole.MarkupLine($"[bold]Nom plat :[/] {commandeChoisie.nom}\n[bold]Quantité :[/] {res3[4]}\n[bold]Date commande :[/] {res3[5]}\n[bold]Prix :[/] {res3[6]}\n");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Voir le trajet", "Supprimer la commande", "Quitter")
            );

            switch (choixAction){
                case "Voir le trajet":
                    AfficherTrajet(ID, Convert.ToInt32(res3[1]));
                    break;
                case "Supprimer la commande":
                    string req4 = $"UPDATE Commande SET Statut = \"supprimer\" WHERE Id_commande = {commandeChoisie.id};";
                    string req5 = $"UPDATE Plat SET Nombre_pers = Nombre_pers + {res3[4]} WHERE Id_plat = {res3[0]};";

                    try{
                        var command = maConnexion.CreateCommand();
                        command.CommandText = req4;
                        command.ExecuteNonQuery();
                        command.Dispose();

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
                case "Quitter":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

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

            for (int i = 0; i < plats.Count; i++){
                AnsiConsole.MarkupLine($"{i + 1}. {plats[i].nom}");
            }

            var choixNom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sélectionnez un plat à gérer ou [red]Échap[/] pour quitter :[/]")
                    .AddChoices(plats.Select(p => p.nom))
            );

            var platChoisi = plats.First(p => p.nom == choixNom);
            string req2 = $"SELECT * FROM Plat WHERE Id_plat = {platChoisi.id};";
            string[] res2 = Requete(req2);

            AnsiConsole.MarkupLine($"[bold]Nom plat :[/] {res2[2]}\n[bold]Quantité disponible :[/] {res2[4]}");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Supprimer le plat", "Quitter")
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
                        AnsiConsole.MarkupLine($"[red]Erreur de connexion :[/] {e.Message}");
                    }catch (Exception e){
                        AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
                    }
                    break;
                case "Quitter":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

        public void GererCommande(int ID){
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Vos commandes en cours :[/]");

            string req1 = $"SELECT Id_plat, Id_commande FROM Commande WHERE Id_cuisinier = {ID} AND Statut = \"active\";";
            string[] res1 = Requete(req1);

            List<string[]> res2 = new List<string[]>();
            for (int i = 0; i < res1.Length / 2; i++){
                string req2 = $"SELECT Nom FROM Plat WHERE Id_plat = {res1[i * 2]};";
                res2.Add(Requete(req2));
            }

            if (res2.Count == 0){
                AnsiConsole.MarkupLine("[red]Aucune commande en cours.[/]");
                return;
            }

            for (int i = 0; i < res2.Count; i++){
                AnsiConsole.MarkupLine($"{i + 1}. {res2[i][0]}");
            }

            var choixCommande = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Entrez le numéro de la commande pour laquelle vous souhaitez des informations ou [red]Échap[/] pour quitter :[/]")
                    .AddChoices(res2.Select(r => r[0]))
            );

            int rang = res2.FindIndex(r => r[0] == choixCommande);
            string req3 = $"SELECT * FROM Commande WHERE Id_commande = {res1[rang * 2 + 1]};";
            string[] res3 = Requete(req3);

            AnsiConsole.MarkupLine($"[bold]Nom plat :[/] {res2[rang][0]}\n[bold]Quantité :[/] {res3[4]}\n[bold]Date commande :[/] {res3[5]}\n[bold]Prix :[/] {res3[6]}\n");

            var choixAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Que souhaitez-vous faire ?[/]")
                    .AddChoices("Voir le trajet", "Supprimer la commande", "Quitter")
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
                        AnsiConsole.MarkupLine($"[red]Erreur de connexion :[/] {e.Message}");
                    }catch (Exception e){
                        AnsiConsole.MarkupLine($"[red]Erreur inattendue :[/] {e.Message}");
                    }
                    break;
                case "Quitter":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Choix invalide, veuillez réessayer.[/]");
                    break;
            }
        }

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
        public void ModeDev(){
            while (true){
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Administrateur[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Ajouter un retard sur le réseaux de transport",
                            "Gestion des Clients",
                            "Gestion des Cuisiniers",
                            "Gestion des Commandes",
                            "Statistiques",
                            "Graphe relationnel",
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

        private void ModuleGrapheRelationnel(){
            while (true){
                var choix = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Menu Administrateur[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Generer le graphe",
                            "[red]Quitter[/]")
                );

                switch (choix){
                    case "Generer le graphe":
                        ConstruireGrapheRelations();
                        break;
                    case "[red]Quitter[/]":
                        AnsiConsole.MarkupLine("[green]Au revoir ![/]");
                        return;
                    default: break;
                }
            }
        }

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

        private void ModuleClient()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des clients[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Clients par ordre alphabétique",
                        "Modifier un client",
                        "Supprimer un client",
                        "Afficher les clients",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Clients par ordre alphabétique":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Tri des clients par ordre alphabétique...");
                    break;
                case "Modifier un client":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Modification d'un client...");
                    break;
                case "Supprimer un client":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Suppression d'un client...");
                    break;
                case "Afficher les clients":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Affichage des clients...");
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        private void ModuleCuisinier()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des cuisiniers[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Ajouter un cuisinier",
                        "Modifier un cuisinier",
                        "Supprimer un cuisinier",
                        "Afficher les cuisiniers et leurs plats",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Ajouter un cuisinier":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Ajout d'un cuisinier...");
                    break;
                case "Modifier un cuisinier":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Modification d'un cuisinier...");
                    break;
                case "Supprimer un cuisinier":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Suppression d'un cuisinier...");
                    break;
                case "Afficher les cuisiniers et leurs plats":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Affichage des cuisiniers et de leurs plats...");
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        private void ModuleCommande()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de gestion des commandes[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Créer une commande",
                        "Modifier une commande",
                        "Supprimer une commande",
                        "Calculer le prix d'une commande",
                        "Déterminer le chemin optimal de livraison",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Créer une commande":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Création d'une commande...");
                    break;
                case "Modifier une commande":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Modification d'une commande...");
                    break;
                case "Supprimer une commande":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Suppression d'une commande...");
                    break;
                case "Calculer le prix d'une commande":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Calcul du prix d'une commande...");
                    break;
                case "Déterminer le chemin optimal de livraison":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Calcul du chemin optimal de livraison...");
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }

        private void ModuleStatistiques()
        {
            var choix = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Module de statistiques[/]")
                    .PageSize(10)
                    .AddChoices(
                        "Nombre de livraisons par cuisinier",
                        "Commandes par période",
                        "Moyenne des prix des commandes",
                        "Moyenne des comptes clients",
                        "Liste des commandes par nationalité des plats",
                        "[red]Retour au menu principal[/]")
            );

            switch (choix)
            {
                case "Nombre de livraisons par cuisinier":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Nombre de livraisons par cuisinier...");
                    break;
                case "Commandes par période":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Affichage des commandes par période...");
                    break;
                case "Moyenne des prix des commandes":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Calcul de la moyenne des prix des commandes...");
                    break;
                case "Moyenne des comptes clients":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Calcul de la moyenne des comptes clients...");
                    break;
                case "Liste des commandes par nationalité des plats":
                    AnsiConsole.MarkupLine("[grey][REQUÊTE SQL][/] Liste des commandes par nationalité des plats...");
                    break;
                case "[red]Retour au menu principal[/]":
                    break;
                default: break;
            }
        }
        #endregion  

        public void ConstruireGrapheRelations(){

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

                        positionsStations[id] = new Tuple<double, double>(longitude, latitude);
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
                            double angleClient = (mapNoeuds.Count % 8) * (Math.PI / 4);
                            double rayonClient = 0.0003;
                            double xClient = x + rayonClient * Math.Cos(angleClient);
                            double yClient = y + rayonClient * Math.Sin(angleClient);
                            Noeud<string> noeudClient = new Noeud<string>(mapNoeuds.Count + 1, nomClient, xClient, yClient);

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

                            double angleCuisinier = (mapNoeuds.Count % 8) * (Math.PI / 4);
                            double rayonCuisinier = 0.0003;
                            double xCuisinier = x + rayonCuisinier * Math.Cos(angleCuisinier);
                            double yCuisinier = y + rayonCuisinier * Math.Sin(angleCuisinier);
                            Noeud<string> noeudCuisinier = new Noeud<string>(mapNoeuds.Count + 1, nomCuisinier, xCuisinier, yCuisinier);

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
            Bitmap img = grapheRelations.DrawGraphe();
            string path = "graphe_relations.png";
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}