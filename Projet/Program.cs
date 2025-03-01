using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Projet;


class Program{
    public static void Main(){
        Noeud n1 = new Noeud(1);
        Noeud n2 = new Noeud(2);
        Noeud n3 = new Noeud(3);
        Graphe Test = new Graphe(false, new List<Noeud>(){n1, n2, n3}, new List<Lien>(){new Lien(n1, n3), new Lien(n2, n3), new Lien(n1, n2)});
        AfficherMatrice(Test.Matrice_adj);
        AfficherListeAdj(Test.Liste_adj);
        Console.WriteLine(Test.IsConnexe());
        Console.WriteLine(Test.ContientCircuit());
        Graphe TestFile = new Graphe(false, "soc-karate.mtx");
        AfficherMatrice(TestFile.Matrice_adj);
        AfficherListeAdj(TestFile.Liste_adj);
        Console.WriteLine(TestFile.IsConnexe());
        Console.WriteLine(TestFile.ContientCircuit());
        TestFile.DrawGraphe();
    }

    public static void AfficherMatrice(int[,] matrice_adj){
        int lignes = matrice_adj.GetLength(0);
        int colonnes = matrice_adj.GetLength(1);
        for (int i = 0; i < lignes; i++){
            for (int j = 0; j < colonnes; j++){
                Console.Write(matrice_adj[i, j]);
            }
            Console.WriteLine();
        }
    }

    public static void AfficherListeAdj(Dictionary<int, List<int>> liste){
        foreach(var noeud in liste){
            Console.WriteLine(noeud.Key + " : " + string.Join(", ", noeud.Value));
        }
    }
}
