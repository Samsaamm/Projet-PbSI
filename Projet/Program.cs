using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Projet;
using System.Drawing;

namespace Projet{
    class Program{
        public static void Main()
        {
            Graphe<string> Metro = new Graphe<string>(true, "MetroParis.xlsx");
            /* Interface<string> app = new Interface<string>(Metro);
            Console.WriteLine("Exemple d'affichage du chemin le plus cours entre deux station : ");
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

            InterfaceV2<string> app = new InterfaceV2<string>(Metro);
        }
    }
}
