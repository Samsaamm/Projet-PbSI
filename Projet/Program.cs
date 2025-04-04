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
            Interface<string> app = new Interface<string>(Metro);
            app.Run();
        
            /* Metro.Distance(Metro.Noeuds[0], Metro.Noeuds[18]); */
        }
    }
}
