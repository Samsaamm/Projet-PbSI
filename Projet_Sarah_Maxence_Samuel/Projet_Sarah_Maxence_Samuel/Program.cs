using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Projet_Sarah_Maxence_Samuel;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Projet_Sarah_Maxence_Samuel{
    class Program{
        public static void Main()
        {
            Graphe<string> Metro = new Graphe<string>(true, "MetroParis.xlsx");
            Interface<string> app = new Interface<string>(Metro);
        }
    }
}
