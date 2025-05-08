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
            /* Interface<string> app = new Interface<string>(Metro); */

            /* Console.WriteLine("Exemple d'affichage du chemin le plus cours entre deux station : ");
            Console.WriteLine("Entre Bastlle et Temple : ");
            Metro.Distance(Metro.Noeuds[13], Metro.Noeuds[58]);
            Console.WriteLine("Appuyer sur entrer pour continuer");
            Console.ReadLine();
            Console.WriteLine("Entre Porte Maillot et Château de Vincenne : ");
            Metro.Distance(Metro.Noeuds[0], Metro.Noeuds[18]);
            Console.WriteLine("Appuyer sur entrer pour continuer");
            Console.ReadLine();
            Console.WriteLine("Entre Corvisart et Oberkampf : ");
            Metro.Distance(Metro.Noeuds[130], Metro.Noeuds[231]);
            Console.WriteLine("Appuyer sur entrer pour continuer");
            Console.ReadLine();
            
            
            app.Run(); */




            /* Dictionary<Noeud<string>, int> couleurs = grapheRelations.WelshPowell();
            Console.WriteLine("Coloration Welsh-Powell :");
            foreach (KeyValuePair<Noeud<string>, int> entry in couleurs)
            {
                Console.WriteLine(entry.Key.ValeurNoeud + " => Couleur " + entry.Value);
            }
            Console.WriteLine("Nombre de couleurs utilisées : " + couleurs.Values.Distinct().Count());


            if (grapheRelations.EstBiparti())
            {
                Console.WriteLine("Le graphe est biparti d apres l algorithme de Welsh Powel");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas biparti d apres l algorithme de Welsh Powel");
            }


            Dictionary<int, List<Noeud<string>>> groupes = grapheRelations.GroupesIndependants();
            Console.WriteLine("Groupes indépendants :");
            foreach (KeyValuePair<int, List<Noeud<string>>> groupe in groupes)
            {
                Console.WriteLine("Couleur " + groupe.Key + " :");
                foreach (Noeud<string> n in groupe.Value)
                {
                    Console.WriteLine(" - " + n.ValeurNoeud);
                }
            } */



            InterfaceV2<string> app = new InterfaceV2<string>(Metro);
        }
    }
}
