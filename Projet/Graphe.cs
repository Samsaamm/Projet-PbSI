using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Projet
{
    internal class Graphe: Control
    {
        public bool IsOriented;
        public List<Noeud> noeuds = new List<Noeud>();
        public List<Lien> liens = new List<Lien>();
        public Dictionary<int, List<int>> liste_adj = new Dictionary<int, List<int>>();
        public int[,] matrice_adj = new int[0,0];



        /// <summary>Constructeur du graphe manuel</summary>/// 
        public Graphe(bool oriented, List<Noeud> noeuds, List<Lien> liens){
            IsOriented = oriented;
            this.noeuds = noeuds;
            this.liens = liens;
            CreatMatAdj(noeuds, liens);
            CreateListAdj();
        }


        /// <summary>Constructeur du graphe a partir d'un fichier source</summary>
        /// <param name="filepath">String, Indique le chemin vers le fichier a ouvrir</param>
        public Graphe(bool oriented, string filepath){
            IsOriented = oriented;
            if(filepath.Substring(filepath.Length - 4) == ".mtx"){
                StreamReader SReader = null;
                List<string> File = new List<string>();
                try{
                    SReader = new StreamReader(filepath);
                    string line;
                    while ((line = SReader.ReadLine()) != null){
                        File.Add(line);
                    }
                    for(int i = 0; i < File.Count; i++){
                        string ligne = File[i];
                        if(ligne[0] == '%'){
                            File.RemoveAt(i);
                            i--;
                        }
                    }

                    for(int i = 0; i < Convert.ToInt32(File[0].Split(' ')[0]); i++){
                        noeuds.Add(new Noeud(i+1));
                    }    

                    for(int i = 1; i < File.Count; i++){
                        string[] data = File[i].Split(' ');
                        liens.Add(new Lien(noeuds[Convert.ToInt32(data[0]) - 1], noeuds[Convert.ToInt32(data[1]) - 1]));
                    }

                    CreatMatAdj(noeuds, liens);
                    CreateListAdj();

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

        public int[,] Matrice_adj{
            get { return matrice_adj; }
        }

        public Dictionary<int, List<int>> Liste_adj{
            get { return liste_adj;}
        }

        public void CreatMatAdj(List<Noeud> node, List<Lien> link){
            matrice_adj = new int[node.Count, node.Count];
            for(int i =0; i < link.Count; i++){
                matrice_adj[link[i].Depart.Numnoeud - 1, link[i].Arrivee.Numnoeud - 1] = 1;
                matrice_adj[link[i].Arrivee.Numnoeud - 1, link[i].Depart.Numnoeud - 1] = 1;
            }
        }

        public void CreateListAdj(){
            for(int i = 0; i < matrice_adj.GetLength(0); i++){
                int noeud = i + 1;
                liste_adj[noeud] = new List<int>();
                for(int j = 0; j < matrice_adj.GetLength(1); j++){
                    if(matrice_adj[i, j] == 1){
                        liste_adj[noeud].Add(j + 1);
                    }
                }
            }
        }

        public HashSet<Noeud> DFS(Noeud noeud, HashSet<Noeud> visites){
            if(visites.Contains(noeud)){
                return null;
            } 
            visites.Add(noeud);
            foreach(var voisin in liste_adj[noeud.Numnoeud]){
                DFS(noeuds[voisin - 1], visites);
            }
            return visites;
        }

        public bool IsConnexe(){
            if (liste_adj.Count == 0) return true;
            HashSet<Noeud> visites = new HashSet<Noeud>();
            DFS(noeuds[liste_adj.Keys.First()], visites);
            return visites.Count == liste_adj.Count;
        }
    
        public HashSet<Noeud> BFS(Noeud depart){
            HashSet<Noeud> visites = new HashSet<Noeud>();
            Queue<Noeud> file = new Queue<Noeud>();
            file.Enqueue(depart);
            visites.Add(depart);

            while(file.Count > 0){
                Noeud noeud = file.Dequeue();
                foreach(var voisin in liste_adj[noeud.Numnoeud]){
                    if(!visites.Contains(noeuds[voisin - 1])){
                        visites.Add(noeuds[voisin - 1]);
                        file.Enqueue(noeuds[voisin - 1]);
                    }
                }
            }
            return visites;
        }

        public bool ContientCircuit(){
            HashSet<Noeud> visites = new HashSet<Noeud>();
            bool res = false;
            foreach(var noeud in liste_adj.Keys){
                if (!visites.Contains(noeuds[noeud - 1]) && DetecterCircuit(noeuds[noeud - 1], null, visites))
                    res = true;
            }
            return res;
        }

        public bool DetecterCircuit(Noeud noeud, Noeud parent, HashSet<Noeud> visites){
            visites.Add(noeud);
            bool res = false;
            foreach(var voisin in liste_adj[noeud.Numnoeud]){
                if(!visites.Contains(noeuds[voisin - 1])){
                    if(DetecterCircuit(noeuds[voisin - 1], noeud, visites)){
                        res = true;
                    }
                }else if(noeuds[voisin - 1] != parent){
                    res = true;
                }
            }
            return res;
        }
    
        public void DrawGraphe(int width = 1920, int height = 1080){
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                Pen pen = new Pen(Color.Black, 2);
                Font font = new Font("Arial", 10);
                Brush brush = Brushes.Blue;
                Random rand = new Random();

                Dictionary<int, Point> positions = new Dictionary<int, Point>();

                foreach (var noeud in liste_adj.Keys){
                    int x = rand.Next(50, width - 50);
                    int y = rand.Next(50, height - 50);
                    positions[noeud] = new Point(x, y);
                }

                foreach (var noeud in liste_adj.Keys){
                    foreach (var voisin in liste_adj[noeud]){
                        if (positions.ContainsKey(voisin)){
                            g.DrawLine(pen, positions[noeud], positions[voisin]);
                        }
                    }
                }

                // Dessin des nœuds
                foreach (var noeud in liste_adj.Keys){
                    Point pos = positions[noeud];
                    g.FillEllipse(brush, pos.X - 10, pos.Y - 10, 20, 20);
                    g.DrawString(noeuds[noeud - 1].Numnoeud.ToString(), font, Brushes.White, pos.X - 5, pos.Y - 5);
                }
            }
            
            string imagePath = "graph.png";
            bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
        }
    }
}
