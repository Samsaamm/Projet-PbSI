using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Projet
{
    internal class Graphe<T>
    {
        public bool IsOriented;
        public List<Noeud<T>> noeuds = new List<Noeud<T>>();
        public List<Lien<T>> liens = new List<Lien<T>>();
        public Dictionary<int, List<int>> liste_adj = new Dictionary<int, List<int>>();
        public int[,] matrice_adj = new int[0,0];


        /// <summary>Constructeur du graphe manuel</summary>/// 
        public Graphe(bool oriented, List<Noeud<T>> noeuds, List<Lien<T>> liens){
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
                        noeuds.Add(new Noeud<T>(i+1));
                    }    

                    for(int i = 1; i < File.Count; i++){
                        string[] data = File[i].Split(' ');
                        liens.Add(new Lien<T>(noeuds[Convert.ToInt32(data[0]) - 1], noeuds[Convert.ToInt32(data[1]) - 1]));
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
            }else if(filepath.Substring(filepath.Length - 5) == ".xlsx"){

            }else{
                Console.WriteLine("Extension du fichier invalide");
            }
        }

        #region Attribut
        public int[,] Matrice_adj{
            get { return matrice_adj; }
        }

        public Dictionary<int, List<int>> Liste_adj{
            get { return liste_adj;}
        }
        #endregion

        public void CreatMatAdj(List<Noeud<T>> node, List<Lien<T>> link){
            matrice_adj = new int[node.Count, node.Count];
            for(int i =0; i < link.Count; i++){
                matrice_adj[link[i].Depart.Numnoeud - 1, link[i].Arrivee.Numnoeud - 1] = 1;
                if(!IsOriented){
                    matrice_adj[link[i].Arrivee.Numnoeud - 1, link[i].Depart.Numnoeud - 1] = 1;
                }
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

        public HashSet<Noeud<T>> DFS(Noeud<T> noeud, HashSet<Noeud<T>> visites){
            if(visites.Contains(noeud)){
                return null;
            } 
            visites.Add(noeud);
            foreach(var voisin in liste_adj[noeud.Numnoeud]){
                DFS(noeuds[voisin - 1], visites);
            }
            return visites;
        }

        public HashSet<Noeud<T>> BFS(Noeud<T> depart){
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            Queue<Noeud<T>> file = new Queue<Noeud<T>>();
            file.Enqueue(depart);
            visites.Add(depart);

            while(file.Count > 0){
                Noeud<T> noeud = file.Dequeue();
                foreach(var voisin in liste_adj[noeud.Numnoeud]){
                    if(!visites.Contains(noeuds[voisin - 1])){
                        visites.Add(noeuds[voisin - 1]);
                        file.Enqueue(noeuds[voisin - 1]);
                    }
                }
            }
            return visites;
        }

        public bool IsConnexe(){
            if (liste_adj.Count == 0) return true;
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            DFS(noeuds[liste_adj.Keys.First()], visites);
            return visites.Count == liste_adj.Count;
        }
    

        public bool ContientCircuit(){
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            bool res = false;
            foreach(var noeud in liste_adj.Keys){
                if (!visites.Contains(noeuds[noeud - 1]) && DetecterCircuit(noeuds[noeud - 1], null, visites))
                    res = true;
            }
            return res;
        }

        public bool DetecterCircuit(Noeud<T> noeud, Noeud<T> parent, HashSet<Noeud<T>> visites){
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
    

        #region Graphique
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
                    int x = rand.Next(100, width - 100);
                    int y = rand.Next(100, height - 100);
                    positions[noeud] = new Point(x, y);
                }

                foreach (var noeud in liste_adj.Keys){
                    foreach (var voisin in liste_adj[noeud]){
                        if (positions.ContainsKey(voisin)){
                            if(IsOriented){
                                DrawArrow(g, pen, positions[noeud], positions[voisin]);
                            }else{
                                g.DrawLine(pen, positions[noeud], positions[voisin]);
                            }
                        }
                    }
                }

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

        /// <summary>
        /// Dessine une flèche entre deux points
        /// </summary>
        private void DrawArrow(Graphics g, Pen pen, Point start, Point end)
        {
            AdjustableArrowCap arrowCap = new AdjustableArrowCap(5, 10);
            pen.CustomEndCap = arrowCap;
            g.DrawLine(pen, start, end);
        }
        #endregion
    }
}
