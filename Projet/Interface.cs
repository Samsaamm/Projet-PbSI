using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Bsi;
using Org.BouncyCastle.Crypto.Engines;

namespace Projet {


    class Interface<T>{
        private MySqlConnection? maConnexion;
        private Graphe<T> Metro;

        public Interface(Graphe<T> metro) {
            this.Metro = metro;
            try {
                string connexionString = "SERVER=127.0.0.1;PORT=3306;" +
                         "DATABASE=projet_PSI;" +
                         "UID=psi_user;PASSWORD=test;";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
                Console.WriteLine("Connexion réussie à la base de données.");
                InsertMetro();
            } catch (MySqlException e) {
                Console.WriteLine("Erreur de connexion : " + e.Message);
                Environment.Exit(1);
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
        /// Modifie la station la plus porche d'un client ou d'un cuisinier
        /// </summary>
        /// <param name="type">client ou cuisinier</param>
        /// <param name="ID">Id_client ou Id_cuisinier</param>
        public void ChangeStation(string type, int ID){
            Console.WriteLine("Veuillez entrer le nom de la station de metro la plus proche de chez vous :");
            string? NomStation = Console.ReadLine();
            int idStation = -1;
            foreach(var noeud in Metro.Noeuds){
                if(NomStation.Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("à", "a").Replace("â", "a").Replace("'", " ").Replace("-", " ").Replace(" ", "").Replace("ç", "c").ToLower() == Convert.ToString(noeud.ValeurNoeud).Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("à", "a").Replace("â", "a").Replace("'", " ").Replace("-", " ").Replace(" ", "").Replace("ç", "c").ToLower()){
                    idStation = noeud.IdNoeud;
                }
            }

            if(idStation == -1){
                Console.WriteLine("Nom de station invalide attention au faute d'orthographe");
            }else{
                string req = "UPDATE " + type + " SET Id_station = " + idStation + " WHERE Id_utilisateur = " + ID + ";";
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

        /// <summary>
        /// Permet a un client de devenir cuisinier et a un cuisinier de devneir client
        /// </summary>
        /// <param name="currenttype">type actuel</param>
        /// <param name="othertype">le type que l'on souhaite devenir</param>
        /// <param name="ID">Id_client ou Id_cuisinier</param>
        public void BecameOtherType(string currenttype, string othertype, int ID){
            string[] res = Requete("SELECT * FROM " + currenttype + " WHERE Id_" + currenttype.ToLower() + " = " + ID + ";");
            string req = "INSERT INTO " + othertype + " VALUES (" + CountOfTable(othertype) + ", " + res[1] + ", " + res[2] + ");";
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
        /// Permet de creer un plat pour un cuisinier
        /// </summary>
        /// <param name="ID">Id_cuisinier</param>
        public void InsererPlat(int ID){
            Console.WriteLine("Nom du plat : ");
            string? nom = Console.ReadLine();
            Console.WriteLine("Type de plat : ");
            string? type = Console.ReadLine();
            Console.WriteLine("Nombre de part : ");
            int nbPart = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Date fabrication : jj/mm/aa");
            string? datefab = Console.ReadLine();
            Console.WriteLine("Date peremption : jj/mm/aa");
            string? dateper = Console.ReadLine();
            Console.WriteLine("Prix d'une part : ");
            double prix = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Nationalité du plat : ");
            string? nationalite = Console.ReadLine();
            int choix;
            do{
                Console.WriteLine("Régime du plat : 1. Vegetarien   2. Autre");
                choix = Convert.ToInt32(Console.ReadLine());
            }while(choix != 1 && choix != 2);
            string regime = null;
            if(choix == 1){
                regime = "vegetarien";
            }else{
                regime = "carnivore";
            }
            Console.WriteLine("Liste des ingrédient du plat : (riz, carotte, etc...)");
            string? ingredient = Console.ReadLine();

            string req = "INSERT INTO Plat VALUES (" + CountOfTable("Plat") + ", " + ID + ", \"" + nom + "\", \"" + type + "\", " + nbPart + ", \"" + datefab + "\", \"" + dateper + "\", " + prix + ", \"" + nationalite + "\", \"" + regime + "\", \"" + ingredient + "\");";
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

        /// <summary>
        /// Permet a un cuisinier de gerer ses plat
        /// </summary>
        /// <param name="ID">Id_cuisinier</param>
        public void GererPlat(int ID){
            Console.Clear();
            Console.WriteLine("Liste des plats disponible : ");
            string req1 = "SELECT Nom, Id_plat FROM Plat WHERE Nombre_pers > 0 AND Id_cuisinier = " + ID + ";";
            string[] res1 = Requete(req1);
            for(int i = 0; i < res1.Length / 2; i++){
                Console.WriteLine((i+1) + ". " + res1[i*2] + ";");
            }

            Console.WriteLine("Entrez le numero d'un plat pour le gerer ou entrez q pour quitter : ");
            string? choix = Console.ReadLine();
            if(choix =="q"){
                return;
            }else{
                try{
                    int rang = Convert.ToInt32(choix) - 1;
                    string req2 = "SELECT * FROM Plat WHERE Id_plat = " + res1[rang*2 + 1] + ";";
                    string[] res2 = Requete(req2);
                    Console.WriteLine("Nom plat : " + res2[2] + "\nQuantite dispo : " + res2[4] + "\n\n");

                    Console.WriteLine("1. Supprimer le plat");
                    Console.WriteLine("2. Quitter");

                    string? choice = Console.ReadLine();
                    switch (choice){
                        case "1": 
                            string req3 = "UPDATE Plat SET Nombre_pers = 0 WHERE Id_plat = " + res2[0] + ";";
                            MySqlCommand command = maConnexion.CreateCommand();
                            command.CommandText = req3;
                            try{
                                command.ExecuteNonQuery();
                            }catch(MySqlException e){
                                Console.WriteLine("Erreur de connexion : " + e.ToString());
                                Console.ReadLine();
                                return;
                            }
                            command.Dispose();
                            break;
                        case "2": return;
                        default : Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                    }
                }catch(Exception e){
                    Console.Clear();
                    Console.WriteLine("Format invalide");
                }
            }
        }

        /// <summary>
        /// Permet a un cuisinier de gerer ses commandes
        /// </summary>
        /// <param name="ID">Id_cuisinier</param>
        public void GererCommande(int ID){
            Console.Clear();
            Console.WriteLine("Vos commandes en cours : ");
            string req1 = "SELECT Id_plat, Id_commande FROM Commande WHERE Id_cuisinier = " + ID + "AND Statut = \"active\";";
            string[] res1 = Requete(req1);

            List<string[]> res2 = new List<string[]>();
            for(int i = 0; i < res1.Length / 2; i++){
                string req2 = "SELECT Nom FROM Plat WHERE Id_plat = " + res1[i*2] + ";";
                res2.Add(Requete(req2));
            }

            for(int i = 0; i < res2.Count; i++){
                Console.WriteLine((i+1) + ". " + res2[i][0]);
            }

            Console.WriteLine("Entrez le numero de la commande pour laquelle vous souhaiter des infos ou entrez q pour quitter : ");
            string? choix = Console.ReadLine();
            if(choix =="q"){
                return;
            }else{
                try{
                    int rang = Convert.ToInt32(choix) - 1;
                    string req3 = "SELECT * FROM Commande WHERE Id_commande = " + res1[rang*2 + 1] + ";";
                    string[] res3 = Requete(req3);
                    Console.WriteLine("Nom plat : " + res2[rang] + "\nQuantité : " + res3[4] + "\nDate commande : " + res3[5] + "\nPrix : " + res3[6] + "\n\n");

                    
                    Console.WriteLine("1. Voir le trajet");
                    Console.WriteLine("2. Supprimer la commande");
                    Console.WriteLine("3. Quitter");

                    string? choice = Console.ReadLine();
                    switch (choice){
                        case "1": AfficherTrajet(ID, Convert.ToInt32(res3[1])); break;
                        case "2": 
                            string req4 = "UPDATE Commande SET Statut = \"supprimer\" WHERE Id_commande = " + res1[rang*2 + 1] + ";";
                            MySqlCommand command = maConnexion.CreateCommand();
                            command.CommandText = req4;
                            try{
                                command.ExecuteNonQuery();
                            }catch(MySqlException e){
                                Console.WriteLine("Erreur de connexion : " + e.ToString());
                                Console.ReadLine();
                                return;
                            }
                            command.Dispose();
                            string req5 = "UPDATE Plat SET Nombre_pers = Nombre_pers + " + res3[4] + " WHERE Id_plat = " + res1[rang*2] + ";";
                            command = maConnexion.CreateCommand();
                            command.CommandText = req5;
                            try{
                                command.ExecuteNonQuery();
                            }catch(MySqlException e){
                                Console.WriteLine("Erreur de connexion : " + e.ToString());
                                Console.ReadLine();
                                return;
                            }
                            command.Dispose();
                            break;
                        case "3": return;
                        default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                    }
                }catch(Exception e){
                    Console.Clear();
                    Console.WriteLine("Format invalide");
                }
            }
        }

        /// <summary>
        /// Permet a un client de voir la liste des plats disponible
        /// </summary>
        /// <param name="ID">Id_client</param>
        public void SeePlat(int ID){
            Console.Clear();
            Console.WriteLine("Liste des plats disponible : ");
            string req1 = "SELECT Nom, Id_plat FROM Plat WHERE Nombre_pers > 0;";
            string[] res1 = Requete(req1);
            for(int i = 0; i < res1.Length / 2; i++){
                Console.WriteLine((i+1) + ". " + res1[i*2] + ";");
            }

            Console.WriteLine("Entrez le numero d'un plat pour commander ou entrez q pour quitter : ");
            string? choix = Console.ReadLine();
            if(choix =="q"){
                return;
            }else{
                try{
                    int rang = Convert.ToInt32(choix);
                    DateTime dateAujourdhui = DateTime.Today;
                    string datecommande = dateAujourdhui.ToString("dd/MM/yy");

                    string req2 = "SELECT Nombre_pers, Id_cuisinier, Prix FROM Plat WHERE Id_plat = " + res1[(rang - 1)*2 + 1] + ";";
                    string[] res2 = Requete(req2);

                    int quant;
                    do{
                        Console.WriteLine("Quelle quantité souhaiter vous ? Quantité dispo : " + res2[0]);
                        quant = Convert.ToInt32(Console.ReadLine());
                    }while(quant < 0 || quant > Convert.ToInt32(res2[0]));

                    string req3 = "INSERT INTO Commande VALUES (" + CountOfTable("Commande") + ", " + res2[1] + ", " + ID + ", " + res1[(rang-1)* 2 + 1] + ", " + quant + ", \"" + datecommande + "\", " + quant * Convert.ToDouble(res2[2]) + ", \"active\");";
                    string req4 = "UPDATE Plat SET Nombre_pers = Nombre_pers - " + quant + " WHERE Id_plat = " + res1[(rang-1)* 2 + 1] + ";";
                    MySqlCommand command = maConnexion.CreateCommand();
                    command.CommandText = req3;
                    try{
                        command.ExecuteNonQuery();
                    }catch(MySqlException e){
                        Console.WriteLine("Erreur de connexion : " + e.ToString());
                        Console.ReadLine();
                        return;
                    }
                    command.Dispose();

                    command = maConnexion.CreateCommand();
                    command.CommandText = req4;
                    try{
                        command.ExecuteNonQuery();
                    }catch(MySqlException e){
                        Console.WriteLine("Erreur de connexion : " + e.ToString());
                        Console.ReadLine();
                        return;
                    }
                    command.Dispose();

                    AfficherTrajet(ID, Convert.ToInt32(res2[1]));

                }catch(Exception e){
                    Console.Clear();
                    Console.WriteLine("Erreur de connexion : " + e.ToString());
                    Console.WriteLine("Format incorect");
                }
                
            }
        }   

        /// <summary>
        /// Permet a un client de voir ses commande en cours
        /// </summary>
        /// <param name="ID">Id_client</param>
        public void SeeCommande(int ID){
            Console.Clear();
            Console.WriteLine("Vos commandes en cours : ");
            string req1 = "SELECT Id_plat, Id_commande FROM Commande WHERE Id_client = " + ID + "AND Statut = \"active\";";
            string[] res1 = Requete(req1);

            List<string[]> res2 = new List<string[]>();
            for(int i = 0; i < res1.Length / 2; i++){
                string req2 = "SELECT Nom FROM Plat WHERE Id_plat = " + res1[i*2] + ";";
                res2.Add(Requete(req2));
            }

            for(int i = 0; i < res2.Count; i++){
                Console.WriteLine((i+1) + ". " + res2[i][0]);
            }

            Console.WriteLine("Entrez le numero de la commande pour laquelle vous souhaiter des infos ou entrez q pour quitter : ");
            string? choix = Console.ReadLine();
            if(choix =="q"){
                return;
            }else{
                try{
                    int rang = Convert.ToInt32(choix) - 1;
                    string req3 = "SELECT * FROM Commande WHERE Id_commande = " + res1[rang*2 + 1] + ";";
                    string[] res3 = Requete(req3);
                    Console.WriteLine("Nom plat : " + res2[rang] + "\nQuantité : " + res3[4] + "\nDate commande : " + res3[5] + "\nPrix : " + res3[6] + "\n\n");

                    
                    Console.WriteLine("1. Voir le trajet");
                    Console.WriteLine("2. Supprimer la commande");
                    Console.WriteLine("3. Quitter");

                    string? choice = Console.ReadLine();
                    switch (choice){
                        case "1": AfficherTrajet(ID, Convert.ToInt32(res3[1])); break;
                        case "2": 
                            string req4 = "UPDATE Commande SET Statut = \"supprimer\" WHERE Id_commande = " + res1[rang*2 + 1] + ";";
                            MySqlCommand command = maConnexion.CreateCommand();
                            command.CommandText = req4;
                            try{
                                command.ExecuteNonQuery();
                            }catch(MySqlException e){
                                Console.WriteLine("Erreur de connexion : " + e.ToString());
                                Console.ReadLine();
                                return;
                            }
                            command.Dispose();
                            string req5 = "UPDATE Plat SET Nombre_pers = Nombre_pers + " + res3[4] + " WHERE Id_plat = " + res1[rang*2] + ";";
                            command = maConnexion.CreateCommand();
                            command.CommandText = req5;
                            try{
                                command.ExecuteNonQuery();
                            }catch(MySqlException e){
                                Console.WriteLine("Erreur de connexion : " + e.ToString());
                                Console.ReadLine();
                                return;
                            }
                            command.Dispose();
                            break;
                        case "3": return;
                        default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                    }
                }catch(Exception e){
                    Console.Clear();
                    Console.WriteLine("Format invalide");
                }
            }
        }

        /// <summary>
        /// Fonction pour faire tournee l'app
        /// </summary>
        public void Run(){
            while(true){
                Console.WriteLine("\nMenu Principal :");
                Console.WriteLine("1. Mode développeur");
                Console.WriteLine("2. S'inscrire");
                Console.WriteLine("3. Se connecter");
                Console.WriteLine("4. Quitter");
                Console.Write("Votre choix : ");

                string? choix = Console.ReadLine();
                switch (choix) {
                    case "1": ModeDev(); break;
                    case "2": Inscription(); break;
                    case "3": Connexion(); break;
                    case "4": Console.WriteLine("Au revoir !"); return;
                    default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                }
            }
        }

        /// <summary>
        /// Permet a un utilisateur de s'inscrire
        /// </summary>
        public void Inscription(){
            Console.WriteLine("Nom : ");
            string? nom = Console.ReadLine();
            Console.WriteLine("Prenom : ");
            string? prenom = Console.ReadLine();
            Console.WriteLine("Adresse : ");
            string? adresse = Console.ReadLine();
            Console.WriteLine("Telephone : ");
            int telephone = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Email : ");
            string? email = Console.ReadLine();
            int choix;
            do{
                Console.WriteLine("Cuisinier ? : (1 pour oui et 0 pour non)");
                if (int.TryParse(Console.ReadLine(), out choix) && (choix == 0 || choix == 1)){
                    break;
                }
                Console.WriteLine("Entrée invalide. Veuillez entrer 1 pour oui ou 0 pour non.");
            } while (true);
            Console.WriteLine("Mot de passe : ");
            string? mdp = Console.ReadLine();

            int numClient = CountOfTable("Utilisateur");

            string insertUtilisateur = "INSERT INTO Utilisateur VALUES (" + numClient + ", \"" + nom + "\", \"" + prenom + "\", \"" + adresse + "\", \"" + telephone + "\", \"" + email + "\", \"" + mdp + "\")";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = insertUtilisateur;
            try{
                command.ExecuteNonQuery();
            }catch(MySqlException e){
                Console.WriteLine("Erreur de connexion : " + e.ToString());
                Console.ReadLine();
                return;
            }
            command.Dispose();

            Console.WriteLine("Veuillez entrer le nom de la station de metro la plus proche de chez vous :");
            string? NomStation = Console.ReadLine();
            int idStation = -1;
            foreach(var noeud in Metro.Noeuds){
                if(NomStation.Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("à", "a").Replace("â", "a").Replace("'", " ").Replace("-", " ").Replace(" ", "").Replace("ç", "c").ToLower() == Convert.ToString(noeud.ValeurNoeud).Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("à", "a").Replace("â", "a").Replace("'", " ").Replace("-", " ").Replace(" ", "").Replace("ç", "c").ToLower()){
                    idStation = noeud.IdNoeud;
                }
            }

            if(idStation == -1){
                Console.WriteLine("Nom de station invalide attention au faute d'orthographe");
            }else{
                if(choix == 1){
                    string InsertCuisinier = "INSERT INTO Cuisinier VALUES (" + CountOfTable("Cuisinier") + ", " + idStation + ", " + numClient + ");";
                    command = maConnexion.CreateCommand();
                    command.CommandText = InsertCuisinier;
                    try{
                        command.ExecuteNonQuery();
                    }catch(MySqlException e){
                        Console.WriteLine("Erreur de connexion : " + e.ToString());
                        Console.ReadLine();
                        return;
                    }
                    command.Dispose();
                }else if(choix == 0){
                    string InsertClient= "INSERT INTO Client VALUES (" + CountOfTable("Client") + ", " + idStation + ", " + numClient + ");";
                    command = maConnexion.CreateCommand();
                    command.CommandText = InsertClient;
                    try{
                        command.ExecuteNonQuery();
                    }catch(MySqlException e){
                        Console.WriteLine("Erreur de connexion : " + e.ToString());
                        Console.ReadLine();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Permet a un utilisateur de se connecter
        /// </summary>
        public void Connexion(){
            Console.WriteLine("Entrez votre email");
            string? email = Console.ReadLine();
            string req = "SELECT Mot_de_passe FROM Utilisateur WHERE Email = \"" + email + "\";";
            string mdp = Requete(req)[0];

            if(mdp == null){
                Console.WriteLine("Aucun compte trouver avec cet email");
            }else{
                Console.WriteLine("Entrez votre mot de passe");
                string? essai = Console.ReadLine();
                if(essai == mdp){
                    string req1 = "SELECT COUNT(*) FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ";
                    int res1 = Convert.ToInt32(Requete(req1)[0]);
                    string req2 = "SELECT COUNT(*) FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ";
                    int res2 = Convert.ToInt32(Requete(req2)[0]);

                    if(res1 == 1 && res2 == 0){
                        ModeCuisinier(Convert.ToInt32(Requete("SELECT Id_cuisinier FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ")[0]));
                    }else if(res1 == 0 && res2 == 1){
                        ModeClient(Convert.ToInt32(Requete("SELECT Id_client FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ")[0]));
                    }else{
                        int session;
                        do{
                            Console.WriteLine("Vous connectez en tant que : 1. Client    2. Cuisinier");
                            session = Convert.ToInt32(Console.ReadLine());
                        }while(session != 1 && session != 2);

                        if(session == 1){
                            ModeClient(Convert.ToInt32(Requete("SELECT Id_client FROM Client c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ")[0]));
                        }else{
                            ModeCuisinier(Convert.ToInt32(Requete("SELECT Id_cuisinier FROM Cuisinier c JOIN Utilisateur u ON c.Id_utilisateur = u.Id_utilisateur WHERE u.Email = \"" + email + "\"; ")[0]));
                        }
                        
                    }
                }
            }

            
            //Requete pour trouver le mdp de l'utilisateur avec ce nom
            // => acces au mode client ou cuisinier en fonction de ce qu'il est 
            // => si les deux demande sous quel type il souhaite se connecter
        }

        /// <summary>
        /// Mode de l'app pour un client
        /// </summary>
        /// <param name="IDClient">int</param>
        public void ModeClient(int IDClient){
            while(true){
                Console.WriteLine("1. Voir les plats");
                Console.WriteLine("2. Voir mes commandes");
                Console.WriteLine("3. Devenir cuisinier");
                Console.WriteLine("4. Changer de metro");
                Console.WriteLine("5. Quitter");
                Console.Write("Votre choix : ");

                string? choix = Console.ReadLine();
                switch (choix) {
                    case "1": SeePlat(IDClient); break;
                    case "2": SeeCommande(IDClient); break;
                    case "3": BecameOtherType("Client", "Cuisinier", IDClient); break;
                    case "4": ChangeStation("Client", IDClient); break;
                    case "5": Console.WriteLine("Au revoir !"); return;
                    default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                }
            }
        }

        /// <summary>
        /// Mode de l'app pour un cuisinier
        /// </summary>
        /// <param name="IDCuisinier">int</param>
        public void ModeCuisinier(int IDCuisinier){
            while(true){
                Console.WriteLine("1. Proposer un plat");
                Console.WriteLine("2. Gerer mes plats");
                Console.WriteLine("3. Gerer mes commandes");
                Console.WriteLine("4. Devenir client");
                Console.WriteLine("5. Changer de metro");
                Console.WriteLine("6. Quitter");
                Console.Write("Votre choix : ");

                string? choix = Console.ReadLine();
                switch (choix) {
                    case "1": InsererPlat(IDCuisinier); break;
                    case "2": GererPlat(IDCuisinier); break;
                    case "3": GererCommande(IDCuisinier); break;
                    case "4": BecameOtherType("Cuisinier", "Client", IDCuisinier); break;
                    case "5": ChangeStation("Cuisinier", IDCuisinier); break;
                    case "6": Console.WriteLine("Au revoir !"); return;
                    default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                }
            }
        }

        /// <summary>
        /// Mode de l'app pour un développeur
        /// </summary>
        public void ModeDev(){
            while (true) {
                Console.WriteLine("\nMenu Principal :");
                Console.WriteLine("1. Gestion des Clients");
                Console.WriteLine("2. Gestion des Cuisiniers");
                Console.WriteLine("3. Gestion des Commandes");
                Console.WriteLine("4. Statistiques");
                Console.WriteLine("5. Quitter");
                Console.Write("Votre choix : ");

                string? choix = Console.ReadLine();
                switch (choix) {
                    case "1": ModuleClient(); break;
                    case "2": ModuleCuisinier(); break;
                    case "3": ModuleCommande(); break;
                    case "4": ModuleStatistiques(); break;
                    case "5": Console.WriteLine("Au revoir !"); return;
                    default: Console.WriteLine("Choix invalide, veuillez réessayer."); break;
                }
            }
        }

        private void ModuleClient(){
            Console.WriteLine("1. Client par ordre alphabétique");
            Console.WriteLine("2. Modifier un client");
            Console.WriteLine("3. Supprimer un client");
            Console.WriteLine("4. Afficher les clients");
            Console.WriteLine("5. Retour au menu principal");
            Console.Write("Votre choix : ");
            string? choix = Console.ReadLine();
            switch (choix) {
                case "1": Console.WriteLine("[REQUÊTE SQL] Ajout d'un client"); break;
                case "2": Console.WriteLine("[REQUÊTE SQL] Modification d'un client"); break;
                case "3": Console.WriteLine("[REQUÊTE SQL] Suppression d'un client"); break;
                case "4": Console.WriteLine("[REQUÊTE SQL] Affichage des clients"); break;
                case "5": return;
                default: Console.WriteLine("Choix invalide"); break;
            }
        }

        private void ModuleCuisinier(){
            Console.WriteLine("1. Ajouter un cuisinier");
            Console.WriteLine("2. Modifier un cuisinier");
            Console.WriteLine("3. Supprimer un cuisinier");
            Console.WriteLine("4. Afficher les cuisiniers et leurs plats");
            Console.WriteLine("5. Retour au menu principal");
            Console.Write("Votre choix : ");
            string? choix = Console.ReadLine();
            switch (choix) {
                case "1": Console.WriteLine("[REQUÊTE SQL] Ajout d'un cuisinier"); break;
                case "2": Console.WriteLine("[REQUÊTE SQL] Modification d'un cuisinier"); break;
                case "3": Console.WriteLine("[REQUÊTE SQL] Suppression d'un cuisinier"); break;
                case "4": Console.WriteLine("[REQUÊTE SQL] Affichage des cuisiniers et de leurs plats"); break;
                case "5": return;
                default: Console.WriteLine("Choix invalide"); break;
            }
        }

        private void ModuleCommande(){
            Console.WriteLine("1. Créer une commande");
            Console.WriteLine("2. Modifier une commande");
            Console.WriteLine("3. Supprimer une commande");
            Console.WriteLine("4. Calculer le prix d'une commande");
            Console.WriteLine("5. Déterminer le chemin optimal de livraison");
            Console.WriteLine("6. Retour au menu principal");
            Console.Write("Votre choix : ");
            string? choix = Console.ReadLine();
            switch (choix) {
                case "1": Console.WriteLine("[REQUÊTE SQL] Création d'une commande"); break;
                case "2": Console.WriteLine("[REQUÊTE SQL] Modification d'une commande"); break;
                case "3": Console.WriteLine("[REQUÊTE SQL] Suppression d'une commande"); break;
                case "4": Console.WriteLine("[REQUÊTE SQL] Calcul du prix"); break;
                case "5": Console.WriteLine("[REQUÊTE SQL] Calcul du chemin optimal"); break;
                case "6": return;
                default: Console.WriteLine("Choix invalide"); break;
            }
        }

        private void ModuleStatistiques(){
            Console.WriteLine("1. Nombre de livraisons par cuisinier");
            Console.WriteLine("2. Commandes par période");
            Console.WriteLine("3. Moyenne des prix des commandes");
            Console.WriteLine("4. Moyenne des comptes clients");
            Console.WriteLine("5. Liste des commandes par nationalité des plats");
            Console.WriteLine("6. Retour au menu principal");
            Console.Write("Votre choix : ");
            string? choix = Console.ReadLine();
            switch (choix) {
                case "1": Console.WriteLine("[REQUÊTE SQL] Nombre de livraisons par cuisinier"); break;
                case "2": Console.WriteLine("[REQUÊTE SQL] Affichage des commandes par période"); break;
                case "3": Console.WriteLine("[REQUÊTE SQL] Moyenne des prix"); break;
                case "4": Console.WriteLine("[REQUÊTE SQL] Moyenne des comptes clients"); break;
                case "5": Console.WriteLine("[REQUÊTE SQL] Liste des commandes par nationalité"); break;
                case "6": return;
                default: Console.WriteLine("Choix invalide"); break;
            }
        }

    }
}