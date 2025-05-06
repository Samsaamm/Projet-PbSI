using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using Spectre.Console;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Bsi;
using Org.BouncyCastle.Crypto.Engines;

namespace Projet {
    class InterfaceV2<T>{
        private MySqlConnection? maConnexion;
        private Graphe<T> Metro;

        public InterfaceV2(Graphe<T> Metro){
            this.Metro = Metro;
            AnsiConsole.MarkupLine("[bold yellow]=== Connexion à la base de données ===[/]");

            var profil = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Choisissez votre [green]profil utilisateur[/] :").AddChoices("Client", "Cuisinier", "Administrateur", "[red]Quitter[/]")
            );

            switch (profil){
                case "Client": 
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=client;PASSWORD=motdepasse_client"); 
                    break;
                case "Cuisinier":
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=cuisinier;PASSWORD=motdepasse_cuisinier"); 
                    break;
                case "Administrateur":
                    Connexion("SERVER=localhost;PORT=3306;" + "DATABASE=projet_PSI;" + "UID=admin;PASSWORD=root"); 
                    break;
                default: break;
            }
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
    }
}