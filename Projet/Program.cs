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
            InterfaceV2<string> app = new InterfaceV2<string>(Metro);
        }
    }
}
