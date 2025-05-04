using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Projet;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Projet{
    class Program{
        public static void Main()
        {
            Graphe<string> Metro = new Graphe<string>(true, "MetroParis.xlsx");
            Interface<string> app = new Interface<string>(Metro);
            //Console.WriteLine("Exemple d'affichage du chemin le plus cours entre deux station : ");
            //Console.WriteLine("Entre Bastlle et Temple : ");
            //Metro.Distance(Metro.Noeuds[13], Metro.Noeuds[58]);
            //Console.WriteLine("Appuyer sur entrer pour continuer");
            //Console.ReadLine();
            //Console.WriteLine("Entre Porte Maillot et Château de Vincenne : ");
            //Metro.Distance(Metro.Noeuds[0], Metro.Noeuds[18]);
            //Console.WriteLine("Appuyer sur entrer pour continuer");
            //Console.ReadLine();
            //Console.WriteLine("Entre Corvisart et Oberkampf : ");
            //Metro.Distance(Metro.Noeuds[130], Metro.Noeuds[231]);
            //Console.WriteLine("Appuyer sur entrer pour continuer");
            //Console.ReadLine();

            Console.WriteLine("Construction du graphe des relations...");
            Graphe<string> grapheRelations = ConstruireGrapheRelations();
            Bitmap img = grapheRelations.DrawGraphe();
            string path = "graphe_relations.png";
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            Console.WriteLine("Graphe de relations généré et sauvegardé sous 'graphe_relations.png'");
            Console.ReadLine();

            Dictionary<int, int> coloration = grapheRelations.WelshPowellColoration();

            Console.WriteLine("\nRésultats de la coloration (Welsh-Powell) :");
            foreach (KeyValuePair<int, int> paire in coloration)
            {
                Console.WriteLine("Noeud " + grapheRelations.Noeuds[paire.Key - 1].ValeurNoeud + " => Couleur " + paire.Value);
            }
            Console.WriteLine();
            Console.WriteLine("Nombre minimal de couleurs nécessaires : " + coloration.Values.Max());
            Console.WriteLine();

            bool estBiparti = grapheRelations.EstBiparti(coloration);
            if (estBiparti)
            {
                Console.WriteLine("Le graphe est biparti : Oui");
            }
            else
            {
                Console.WriteLine("Le graphe est biparti : Non");
            }

            bool estPlanaire = grapheRelations.EstPlanaire();
            if (estPlanaire)
            {
                Console.WriteLine("Le graphe est planaire (selon heuristique) : Probablement");
            }
            else
            {
                Console.WriteLine("Le graphe est planaire (selon heuristique) : Non");
            }

            Console.WriteLine();
            Console.WriteLine("Groupes indépendants :");
            grapheRelations.AfficherGroupesIndependants(coloration);
            Console.ReadLine();


            app.Run();
        }
        public static Graphe<string> ConstruireGrapheRelations()
        {
            List<Noeud<string>> noeuds = new List<Noeud<string>>();
            List<Lien<string>> liens = new List<Lien<string>>();
            Dictionary<string, Noeud<string>> mapNoeuds = new Dictionary<string, Noeud<string>>();
            Dictionary<int, Tuple<double, double>> positionsStations = new Dictionary<int, Tuple<double, double>>();

            string connexionString = "SERVER=127.0.0.1;PORT=3306;" +
                                     "DATABASE=projet_PSI;" +
                                     "UID=psi_user;PASSWORD=test;";
            using (MySqlConnection connexion = new MySqlConnection(connexionString))
            {
                try
                {
                    connexion.Open();
                    Console.WriteLine("Connexion réussie à la base de données.");
                    string requeteStations = "SELECT Id_station, Longitude, Latitude FROM MétroStation;";
                    MySqlCommand commandeStation = connexion.CreateCommand();
                    commandeStation.CommandText = requeteStations;
                    MySqlDataReader lecteurStation = commandeStation.ExecuteReader();
                    while (lecteurStation.Read())
                    {
                        int id = Convert.ToInt32(lecteurStation["Id_station"]);

                        //Ici j avais fait avec un Replaced pour changer la , en point mais ca marchais pas don chat gpt m a donne ce truc bizarre

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

                    while (lecteurRelations.Read())
                    {
                        string idClient = lecteurRelations["Id_client"].ToString();
                        string idCuisinier = lecteurRelations["Id_cuisinier"].ToString();

                        relations.Add(new Tuple<string, string>(idClient, idCuisinier));
                        listeClients.Add(idClient);
                        listeCuisiniers.Add(idCuisinier);
                    }
                    lecteurRelations.Close();
                    commandeRelations.Dispose();

                    Dictionary<string, int> stationClient = new Dictionary<string, int>();
                    if (listeClients.Count > 0)
                    {
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
                    if (listeCuisiniers.Count > 0)
                    {
                        string requeteCuisiniers = "SELECT Id_cuisinier, Id_station FROM Cuisinier;";
                        MySqlCommand commandeCuisiniers = connexion.CreateCommand();
                        commandeCuisiniers.CommandText = requeteCuisiniers;
                        MySqlDataReader lecteurCuisiniers = commandeCuisiniers.ExecuteReader();

                        while (lecteurCuisiniers.Read())
                        {
                            string id = lecteurCuisiniers["Id_cuisinier"].ToString();
                            int idStation = Convert.ToInt32(lecteurCuisiniers["Id_station"]);
                            stationCuisinier[id] = idStation;
                        }

                        lecteurCuisiniers.Close();
                        commandeCuisiniers.Dispose();
                    }

                    foreach (Tuple<string, string> relation in relations)
                    {
                        string nomClient = "Client_" + relation.Item1;
                        string nomCuisinier = "Cuisinier_" + relation.Item2;

                        if (!mapNoeuds.ContainsKey(nomClient))
                        {
                            double x = 0;
                            double y = 0;
                            if (stationClient.ContainsKey(relation.Item1) &&
                                positionsStations.ContainsKey(stationClient[relation.Item1]))
                            {
                                x = positionsStations[stationClient[relation.Item1]].Item1;
                                y = positionsStations[stationClient[relation.Item1]].Item2;
                            }
                            //Creer un petit cercle autour de la stationj de metro pour pas que les clietns ne se superpose pas
                            double angleClient = (mapNoeuds.Count % 8) * (Math.PI / 4);
                            double rayonClient = 0.0003;
                            double xClient = x + rayonClient * Math.Cos(angleClient);
                            double yClient = y + rayonClient * Math.Sin(angleClient);
                            Noeud<string> noeudClient = new Noeud<string>(mapNoeuds.Count + 1, nomClient, xClient, yClient);

                            noeuds.Add(noeudClient);
                            mapNoeuds[nomClient] = noeudClient;
                        }

                        if (!mapNoeuds.ContainsKey(nomCuisinier))
                        {
                            double x = 0;
                            double y = 0;
                            if (stationCuisinier.ContainsKey(relation.Item2) &&
                                positionsStations.ContainsKey(stationCuisinier[relation.Item2]))
                            {
                                x = positionsStations[stationCuisinier[relation.Item2]].Item1;
                                y = positionsStations[stationCuisinier[relation.Item2]].Item2;
                            }

                            //Creer un petit cercle autour de la stationj de metro pour pas que les cuisinier ne se superpose pas
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
                }
                catch (MySqlException e)
                {
                    Console.WriteLine("Erreur SQL lors de la construction du graphe : " + e.ToString());
                }
            }
            return new Graphe<string>(false, noeuds, liens);
        }
    }
}
