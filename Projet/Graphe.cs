using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;

namespace Projet
{
    internal class Graphe
    {
        public string titre = null;
        public bool IsOriented;
        public List<Noeud> noeuds = null;
        public List<Lien> liens = null;
        public Dictionary<Noeud, List<Noeud>> Liste_adj;
        public int[,] Matrice_adj = null;
        /// <summary>Constructeur du graphe manuel</summary>
        public Graphe(string titre, bool oriented, List<Noeud> noeuds, List<Lien> liens){
            this.titre = titre;
            this.IsOriented = oriented;
            this.noeuds = noeuds;
            this.liens = liens;
        }

        /// <summary>Constructeur du graphe a partir d'un fichier source</summary>
        /// <param name="filepath">String, Indique le chemin vers le fichier a ouvrir</param>
        public Graphe(string filepath){
            if(filepath.Substring(filepath.Length - 4) == ".mtx"){
                StreamReader SReader = null;
                List<string> File = new List<string>();
                try{
                    SReader = new StreamReader(filepath);
                    string line;
                    while ((line = SReader.ReadLine()) != null){
                        File.Add(line);
                    }
                    Create_mat_adj(File);
                    ConvertirMatriceEnListeAdj();
                }catch(Exception e){
                    Console.WriteLine(e.ToString());
                }finally{
                    if(SReader != null){
                        SReader.Close();
                    }
                }
            }else{
                Console.WriteLine("Extension du fichier invalide");
            }
            
        }

        public void Create_mat_adj(List<string> File){
            for(int i = 0; i < File.Count; i++){
                string ligne = File[i];
                if(ligne[0] == '%'){
                    File.RemoveAt(i);
                    i--;
                }
            }
            Matrice_adj = new int[Convert.ToInt32(File[0].Split(' ')[0]), Convert.ToInt32(File[0].Split(' ')[1])];
            for(int i = 1; i < File.Count; i++){
                string[] data = File[i].Split(' ');
                Matrice_adj[Convert.ToInt32(data[0]) - 1, Convert.ToInt32(data[1]) - 1] = 1;
                Matrice_adj[Convert.ToInt32(data[1]) - 1, Convert.ToInt32(data[0]) - 1] = 1;
            }
        }

        static void AfficherMatrice(int[,] matrice)
        {
            int lignes = matrice.GetLength(0);
            int colonnes = matrice.GetLength(1);

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    Console.Write(matrice[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void ConvertirMatriceEnListeAdj(){
            Liste_adj = new Dictionary<Noeud, List<Noeud>>();
            for(int i = 0; i < Matrice_adj.GetLength(0); i++){
                Noeud noeud = new Noeud(i);
                Liste_adj[noeud] = new List<Noeud>();
                for(int j = 0; j < Matrice_adj.GetLength(1); j++){
                    if(Matrice_adj[i, j] == 1){
                        Liste_adj[noeud].Add(new Noeud(j));
                    }
                }
            }
        }

        public bool IsConnexe(){
            if (Liste_adj.Count == 0) return true;
            HashSet<Noeud> visites = new HashSet<Noeud>();
            DFS(Liste_adj.Keys.First(), visites);
            return visites.Count == Liste_adj.Count;
        }
    
        public void DFS(Noeud noeud, HashSet<Noeud> visites){
            if(visites.Contains(noeud)) return;
            visites.Add(noeud);
            foreach(var voisin in Liste_adj[noeud]){
                DFS(voisin, visites);
            }
        }

        public void BFS(Noeud depart){
            HashSet<Noeud> visites = new HashSet<Noeud>();
            Queue<Noeud> file = new Queue<Noeud>();
            file.Enqueue(depart);
            visites.Add(depart);

            while(file.Count > 0){
                Noeud noeud = file.Dequeue();
                foreach(var voisin in Liste_adj[noeud]){
                    if(!visites.Contains(voisin)){
                        visites.Add(voisin);
                        file.Enqueue(voisin);
                    }
                }
            }
        }

        public bool ContientCircuit(){
            HashSet<Noeud> visites = new HashSet<Noeud>();
            foreach(var noeud in Liste_adj.Keys){
                if (!visites.Contains(noeud) && DetecterCircuit(noeud, null, visites))
                    return true;
            }
            return false;
        }

        public bool DetecterCircuit(Noeud noeud, Noeud parent, HashSet<Noeud> visites){
            visites.Add(noeud);
            foreach(var voisin in Liste_adj[noeud]){
                if(!visites.Contains(voisin)){
                    if(DetecterCircuit(voisin, noeud, visites)) return true;
                }else if(voisin != parent) return true;
            }
            return false;
        }

        /* public void DFS(){

        }

        public void BFS(){

        }

        public void IsConexe(){

        }

        public void Show(){

        } */
    }
}
