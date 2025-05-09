using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Prng;
using Spectre.Console;

namespace Projet{
    public class Graphe<T>{
        private bool isOriented;
        private List<Noeud<T>> noeuds = new List<Noeud<T>>();
        private List<Lien<T>> liens = new List<Lien<T>>();
        private float[,] matrice_adj;
        private Dictionary<int, List<int>> liste_adj;
        private Bitmap? map;

        /// <summary>
        /// Constructeur par liste
        /// </summary>
        /// <param name="oriented">Est ce que le graphe est orienté</param>
        /// <param name="noeuds">Liste de noeuds</param>
        /// <param name="liens">Liste de liens</param>
        public Graphe(bool oriented, List<Noeud<T>> noeuds, List<Lien<T>> liens){
            this.isOriented = oriented;
            this.noeuds = noeuds;
            this.liens = liens;
            AvoidDoubleNoeud();
            this.matrice_adj = CreateMatriceAdj(noeuds, liens, oriented);
            this.liste_adj = CreateListeAdj(this.matrice_adj);
            
        }

        /// <summary>
        /// Constructeur par fichier. Accepte le format .mtx et .xlsx
        /// </summary>
        /// <param name="oriented">Est ce que le graphe est orienté</param>
        /// <param name="filepath">Chemin vers le fichier</param>
        public Graphe(bool oriented, string filepath){
            this.isOriented = oriented;
            FileReader<T> reader = new FileReader<T>(filepath);
            this.noeuds = reader.Noeuds;
            this.liens = reader.Liens;
            AvoidDoubleNoeud();
            this.matrice_adj = CreateMatriceAdj(this.noeuds, this.liens, oriented);
            this.liste_adj = CreateListeAdj(this.matrice_adj);
            
        }

        /// <summary>
        /// propriété
        /// </summary>

        public List<Noeud<T>> Noeuds{
            get{ return this.noeuds;}
        }

        /// <summary>
        /// propriété
        /// </summary>

        public List<Lien<T>> Liens{
            get{ return this.liens;}
        }

        /// <summary>
        /// propriété
        /// </summary>

        public float[,] Matrice_adj{
            get{ return this.matrice_adj;}
        }

        /// <summary>
        /// propriété
        /// </summary>

        public Dictionary<int, List<int>> Liste_adj{
            get { return this.liste_adj;}
        }

        /// <summary>
        /// Crée un lien représneter les liaisions pour chaque station qui apparait plusieurs fois
        /// </summary>
        public void AvoidDoubleNoeud(){
            for(int i = 0; i < this.noeuds.Count; i++){
                for(int j = 0; j < this.noeuds.Count; j++){
                    if(this.noeuds[i].Equals(this.noeuds[j]) && i != j){
                        liens.Add(new Lien<T>(this.noeuds[i], this.noeuds[j], 1));
                    }
                }
            }
        }

        /// <summary>
        /// Creer la matrice d'adjacence du graphe
        /// </summary>
        /// <param name="n">Liste de noeuds</param>
        /// <param name="l">Liste de liens</param>
        /// <param name="IsOriented">Est ce que le graphe est orienté</param>
        /// <returns>Renvoie une matrice de taille nxn contenant des floats</returns>
        public float[,] CreateMatriceAdj(List<Noeud<T>> n, List<Lien<T>> l, bool IsOriented){
            float[,] mat = new float[n.Count, n.Count];
            for(int i =0; i < l.Count; i++){
                mat[l[i].Depart.IdNoeud - 1, l[i].Arrivee.IdNoeud - 1] = l[i].Poids;
                if(!IsOriented){
                    mat[l[i].Arrivee.IdNoeud - 1, l[i].Depart.IdNoeud - 1] = l[i].Poids;
                }
            }
            return mat;
        }

        /// <summary>
        /// Creer la liste d'adjacence du graphe
        /// </summary>
        /// <param name="matrice_adj">La matrice d'adjacence du graphe</param>
        /// <returns>Un dictionnaire ou les clés représente les id des noeuds du graphe et les valeur, la liste des id des noeuds liée</returns>
        public Dictionary<int, List<int>> CreateListeAdj(float[,] matrice_adj){
            Dictionary<int, List<int>> res = new Dictionary<int, List<int>>();
            for(int i = 0; i < matrice_adj.GetLength(0); i++){
                int noeud = i + 1;
                res[noeud] = new List<int>();
                for(int j = 0; j < matrice_adj.GetLength(1); j++){
                    if(matrice_adj[i, j] != 0){
                        res[noeud].Add(j + 1);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// modification possible de la matrice d'adj si par exemple l'administrateur veut ajouter un retard entre deux stations
        /// </summary>
        /// <param name="idnoeud1">station de départ pour le retard</param>
        /// <param name="idnoeud2">station d'arrivée pour le retard</param>
        /// <param name="poids">poids entre les deux stations</param>

        public void ModifiateAdj(int idnoeud1, int idnoeud2, int poids){
            bool update = false;
            Console.WriteLine(idnoeud1 + "   " + idnoeud2);
            foreach(Lien<T> l in this.liens){
                if((l.Depart.IdNoeud == idnoeud1 && l.Arrivee.IdNoeud == idnoeud2) || (l.Depart.IdNoeud == idnoeud2 && l.Arrivee.IdNoeud == idnoeud1)){
                    l.Poids = poids;
                    this.matrice_adj = CreateMatriceAdj(this.noeuds, this.liens, this.isOriented);
                    this.liste_adj = CreateListeAdj(this.matrice_adj);
                    update = true;
                }
            }

            if(!update){
                AnsiConsole.MarkupLine("[red]Aucune liaison n'a été trouver avec cette station de départ et d'arrivé[/]");
            }
        }

        /// <summary>
        /// Algo de parcours en profondeur
        /// </summary>
        /// <param name="noeud">noeud de depart</param>
        /// <param name="visites">HashSet nécéssaire pour les appel récurssif</param>
        /// <returns>Le hashset des visites</returns>
        public HashSet<Noeud<T>>? DFS(Noeud<T> noeud, HashSet<Noeud<T>> visites){
            if(visites == null){
                visites = new HashSet<Noeud<T>>();
            }
            if(visites.Contains(noeud)){
                return null;
            } 
            visites.Add(noeud);
            foreach(var voisin in liste_adj[noeud.IdNoeud]){
                DFS(noeuds[voisin - 1], visites);
            }
            return visites;
        }

        /// <summary>
        /// Parcours en largeur
        /// </summary>
        /// <param name="depart">noeud de depart</param>
        /// <returns>Hashset des visites</returns>
        public HashSet<Noeud<T>> BFS(Noeud<T> depart){
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            Queue<Noeud<T>> file = new Queue<Noeud<T>>();
            file.Enqueue(depart);
            visites.Add(depart);

            while(file.Count > 0){
                Noeud<T> noeud = file.Dequeue();
                foreach(var voisin in liste_adj[noeud.IdNoeud]){
                    if(!visites.Contains(noeuds[voisin - 1])){
                        visites.Add(noeuds[voisin - 1]);
                        file.Enqueue(noeuds[voisin - 1]);
                    }
                }
            }
            return visites;
        }

        /// <summary>
        /// Est ce que le graphe est connexe
        /// </summary>
        /// <returns>Un bool true : graphe connexe, false : graphe non connexe</returns>
        public bool IsConnexe(){
            if (liste_adj.Count == 0) return true;
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            DFS(noeuds[liste_adj.Keys.First()], visites);
            return visites.Count == liste_adj.Count;
        }

        /// <summary>
        ///  Est ce que le graphe contient en circuit
        /// </summary>
        /// <returns>un bool caractérisant le resultat</returns>
        public bool ContientCircuit(){
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            bool res = false;
            foreach(var noeud in liste_adj.Keys){
                if (!visites.Contains(noeuds[noeud - 1]) && DetecterCircuit(noeuds[noeud - 1], null, visites)){
                    res = true;
                }
            }
            return res;
        }

        /// <summary>
        /// Cherhce l'existence d'un circuit entre 2 noeuds
        /// </summary>
        /// <param name="noeud">depart</param>
        /// <param name="parent">parent du noeuds</param>
        /// <param name="visites">HashSet nécéssaire pour les appel récurssif</param>
        /// <returns>Un bool caractérisant le resultat</returns>
        public bool DetecterCircuit(Noeud<T> noeud, Noeud<T>? parent, HashSet<Noeud<T>> visites){
            visites.Add(noeud);
            bool res = false;
            foreach(var voisin in liste_adj[noeud.IdNoeud]){
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

        /// <summary>
        /// Calcul la distance entre 2 noeuds en utilisant les algo vu en cours
        /// </summary>
        /// <param name="source">noeud de depart</param>
        /// <param name="cible">noeud d'arriver</param>
        /// <param name="algo">algo utilisé </param>
        /// <returns>Une liste de liens caractérisant le chemin utilisé</returns>
        public List<Lien<T>?> Distance(Noeud<T> source, Noeud<T> cible, string algo = "dijk"){
            List<Lien<T>?> chemin = new List<Lien<T>?>();

            bool neg = false;
            foreach(var lien in liens){
                if(lien.Poids < 0){
                    neg = true;
                }
            }
            if(!neg){
                Dictionary<Noeud<T>, Noeud<T>> precedents = null;
                if(algo == "dijk"){
                    precedents = Dijkstra(source);
                }else if(algo == "bell"){
                    precedents = BellmanFord(source);
                }else if(algo == "floy"){
                    Dictionary<Noeud<T>, Dictionary<Noeud<T>, Noeud<T>?>> values = FloydWarshall();
                    precedents = values[source];
                }else{
                    Console.WriteLine("Erreur type d'algo invalide");
                }
                
                Noeud<T>? prec = cible;
                while(prec != source){
                    foreach(var lien in liens){
                        if(lien.Depart == precedents[prec] && lien.Arrivee == prec){
                            chemin.Add(lien);
                        }
                    }
                    prec = precedents[prec];
                }
                Bitmap bitmap = DrawChemin(chemin);
                string imagePath = "graph.png";
                bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
            }

            return chemin;
        }

        /// <summary>
        /// Algo de Dijkstar
        /// </summary>
        /// <param name="source">noeuds source</param>
        /// <returns>Dictionnaire des précendents</returns>
        public Dictionary<Noeud<T>, Noeud<T>?> Dijkstra(Noeud<T> source){
            Dictionary<Noeud<T>, float> distances = new Dictionary<Noeud<T>, float>();
            Dictionary<Noeud<T>, Noeud<T>?> precedents = new Dictionary<Noeud<T>, Noeud<T>?>();
            HashSet<Noeud<T>> non_visites = new HashSet<Noeud<T>>(noeuds);

            for(int i = 0; i < noeuds.Count; i++){
                distances[noeuds[i]] = float.MaxValue;
                precedents[noeuds[i]] = null;
            }
            distances[source] = 0;
            
            while(non_visites.Count > 0){
                Noeud<T> u = null;
                float minDist = float.MaxValue;
                foreach(var noeud in non_visites){
                    if(distances[noeud] < minDist){
                        minDist = distances[noeud];
                        u = noeud;
                    }
                }
                if(u != null){
                    non_visites.Remove(u);
                    for(int i = 0; i < noeuds.Count; i++){
                        if(matrice_adj[u.IdNoeud - 1, i] > 0 && non_visites.Contains(noeuds[i])){
                            float newDist = distances[u] + matrice_adj[u.IdNoeud - 1, i];
                            if(newDist < distances[noeuds[i]]){
                                distances[noeuds[i]] = newDist;
                                precedents[noeuds[i]] = u;
                            }
                        }
                    }
                }else{
                    break;
                }
            }
            return precedents;
        }

        /// <summary>
        /// Algo de Bellman Ford
        /// </summary>
        /// <param name="source">noeud source</param>
        /// <returns>Dictionnaire des précédent</returns>
        public Dictionary<Noeud<T>, Noeud<T>?> BellmanFord(Noeud<T> source)
        {
            Dictionary<Noeud<T>, float> distances = new Dictionary<Noeud<T>, float>();
            Dictionary<Noeud<T>, Noeud<T>?> precedents = new Dictionary<Noeud<T>, Noeud<T>?>();
            for (int i = 0; i < noeuds.Count; i++){
                distances[noeuds[i]] = float.MaxValue;
                precedents[noeuds[i]] = null;
            }
            distances[source] = 0;

            int n = noeuds.Count;
            for (int iter = 0; iter < n - 1; iter++){
                for (int u = 0; u < n; u++){
                    for (int v = 0; v < n; v++){
                        if (matrice_adj[u, v] > 0 && distances[noeuds[u]] != float.MaxValue){
                            float newDist = distances[noeuds[u]] + matrice_adj[u, v];

                            if (newDist < distances[noeuds[v]]){
                                distances[noeuds[v]] = newDist;
                                precedents[noeuds[v]] = noeuds[u];
                            }
                        }
                    }
                }
            }

            for (int u = 0; u < n; u++){
                for (int v = 0; v < n; v++){
                    if (matrice_adj[u, v] > 0 && distances[noeuds[u]] != float.MaxValue){
                        if (distances[noeuds[u]] + matrice_adj[u, v] < distances[noeuds[v]]){
                            throw new Exception("Cycle absorbant détecté !");
                        }
                    }
                }
            }

            return precedents;
        }

        /// <summary>
        /// Algo de Floyd Warshall
        /// </summary>
        /// <returns>Dictionnaire des Dictionnaire des précédent pour chaque noeuds du graphe</returns>
        public Dictionary<Noeud<T>, Dictionary<Noeud<T>, Noeud<T>?>> FloydWarshall()
        {
            int n = noeuds.Count;
            float[,] distances = new float[n, n];
            Dictionary<Noeud<T>, Dictionary<Noeud<T>, Noeud<T>?>> predecesseurs = new();

            for (int i = 0; i < n; i++){
                predecesseurs[noeuds[i]] = new Dictionary<Noeud<T>, Noeud<T>?>();
                for (int j = 0; j < n; j++){
                    if (i == j){
                        distances[i, j] = 0;
                        predecesseurs[noeuds[i]][noeuds[j]] = null;
                    }else if (matrice_adj[i, j] > 0){
                        distances[i, j] = matrice_adj[i, j];
                        predecesseurs[noeuds[i]][noeuds[j]] = noeuds[i];
                    }else{
                        distances[i, j] = float.MaxValue;
                        predecesseurs[noeuds[i]][noeuds[j]] = null;
                    }
                }
            }
            for (int k = 0; k < n; k++){
                for (int i = 0; i < n; i++){
                    for (int j = 0; j < n; j++){
                        if (distances[i, k] < float.MaxValue && distances[k, j] < float.MaxValue){
                            float newDist = distances[i, k] + distances[k, j];
                            if (newDist < distances[i, j]){
                                distances[i, j] = newDist;
                                predecesseurs[noeuds[i]][noeuds[j]] = predecesseurs[noeuds[k]][noeuds[j]];
                            }
                        }
                    }
                }
            }
            return predecesseurs;
        }

        /// <summary>
        /// Dessine le chemin le plus rapide sur le plans du graphe
        /// </summary>
        /// <param name="chemin">liste des lien catégorisant le chemin a dessiner</param>
        /// <param name="map">Le plans du graphe sous forme de Bitmap</param>
        /// <returns>Renvoie une Bitmap avec le chemin dessiner</returns>
        public Bitmap DrawChemin(List<Lien<T>> chemin, Bitmap? map = null){
            if(map == null){
                map = DrawGraphe();
            }
            using(Graphics g = Graphics.FromImage(map)){
                Pen pen = new Pen(System.Drawing.Color.Red, 3);
                Font font = new Font("Arial", 10);

                Dictionary<int, double[]> positions = new Dictionary<int, double[]>();
                foreach(var noeud in noeuds){
                    double x = noeud.CoX;
                    double y = noeud.CoY;
                    positions[noeud.IdNoeud] = [x, y];
                }
                Dictionary<int, Point> CartPositions = CooCartesienne(positions, map.Width - 50, map.Height - 50);
                foreach(var lien in chemin){
                    Point d = CartPositions[lien.Depart.IdNoeud - 1];
                    d.Y = map.Height - d.Y;
                    Point a = CartPositions[lien.Arrivee.IdNoeud - 1];
                    a.Y = map.Height - a.Y;
                    if(isOriented){
                        DrawArrow(g, pen, d, a);
                    }else{
                        g.DrawLine(pen, d, a);
                    }
                }
            }
            return map;
        }


        /// <summary>
        /// Dessine le plans du graphe
        /// </summary>
        /// <param name="width">largeur de l'image</param>
        /// <param name="height">hauteur de l'image</param>
        /// <returns>renvoie le plans sous forme de Bitmap</returns>
        public Bitmap DrawGraphe(int width = 1920*2, int height = 1080*2){
            Bitmap bitmap= new Bitmap(width, height);
            using(Graphics g = Graphics.FromImage(bitmap)){
                g.Clear(System.Drawing.Color.White);
                Pen pen = new Pen(System.Drawing.Color.Black, 2);
                Font font = new Font("Arial", 10);
                Brush brush = Brushes.Black;
                Dictionary<int, double[]> positions = new Dictionary<int, double[]>();
                foreach(var noeud in noeuds){
                    double x = noeud.CoX;
                    double y = noeud.CoY;
                    positions[noeud.IdNoeud] = [x, y];
                }
                Dictionary<int, Point> CartPositions = CooCartesienne(positions, width - 50, height - 50);
                foreach(var noeud in noeuds){
                    Point pos = CartPositions[noeud.IdNoeud - 1];
                    pos.Y = height - pos.Y;
                    g.FillEllipse(brush, pos.X - 10, pos.Y - 10, 20, 20);
                    if(noeud.ValeurNoeud != null){
                        g.DrawString(noeud.ValeurNoeud.ToString(), font, brush, pos.X, pos.Y - 25);
                    }else{
                        g.DrawString("Null", font, Brushes.Red, pos.X, pos.Y - 15);
                    }
                }
                foreach(var lien in liens){
                    Point d = CartPositions[lien.Depart.IdNoeud - 1];
                    d.Y = height - d.Y;
                    Point a = CartPositions[lien.Arrivee.IdNoeud - 1];
                    a.Y = height - a.Y;
                    if(isOriented){
                        DrawArrow(g, pen, d, a);
                    }else{
                        g.DrawLine(pen, d, a);
                    }
                }
            }
            this.map = bitmap;
            return bitmap;
        }

        /// <summary>
        /// dessin du graphe coloré
        /// </summary>
        /// <param name="width">largeur</param>
        /// <param name="height">hauteur</param>
        /// <returns>le graphe coloré</returns>

        public Bitmap DrawGrapheColored(int width = 1920 * 2, int height = 1080 * 2)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Dictionary<Noeud<T>, int> couleurs = WelshPowell();

            using (Graphics g = Graphics.FromImage(bitmap)){
                g.Clear(System.Drawing.Color.White);
                Pen pen = new Pen(System.Drawing.Color.Black, 2);
                Font font = new Font("Arial", 10);
                Brush brushTexte = Brushes.Black;

                Dictionary<int, double[]> positions = new Dictionary<int, double[]>();
                foreach (Noeud<T> noeud in noeuds){
                    double x = noeud.CoX;
                    double y = noeud.CoY;
                    positions[noeud.IdNoeud] = new double[] { x, y };
                }

                Dictionary<int, Point> cartPositions = CooCartesienne(positions, width - 50, height - 50);

                foreach (Lien<T> lien in liens){
                    Point d = cartPositions[lien.Depart.IdNoeud - 1];
                    d.Y = height - d.Y;
                    Point a = cartPositions[lien.Arrivee.IdNoeud - 1];
                    a.Y = height - a.Y;

                    if (isOriented){
                        DrawArrow(g, pen, d, a);
                    }else{
                        g.DrawLine(pen, d, a);
                    }
                }

                foreach (Noeud<T> noeud in noeuds){
                    Point pos = cartPositions[noeud.IdNoeud - 1];
                    pos.Y = height - pos.Y;

                    Brush brush = Brushes.Gray;
                    if (couleurs.ContainsKey(noeud)){
                        brush = GetColorFromInt(couleurs[noeud]);
                    }

                    g.FillEllipse(brush, pos.X - 10, pos.Y - 10, 20, 20);
                    g.DrawEllipse(Pens.Black, pos.X - 10, pos.Y - 10, 20, 20);

                    if (noeud.ValeurNoeud != null){
                        g.DrawString(noeud.ValeurNoeud.ToString(), font, brushTexte, pos.X + 12, pos.Y - 10);
                    }else{
                        g.DrawString("Null", font, Brushes.Red, pos.X, pos.Y - 15);
                    }
                }
            }

            this.map = bitmap;
            return bitmap;
        }

        /// <summary>
        /// tableau pour le pinceau de couleur
        /// </summary>
        /// <param name="i">indice de la couleur dans le tableau</param>
        /// <returns>un pinceau avec la couleur choisie </returns>
        public Brush GetColorFromInt(int i)
        {
            System.Drawing.Color[] couleurs = new System.Drawing.Color[]
            {
                System.Drawing.Color.Blue, System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Orange, System.Drawing.Color.Purple,
                System.Drawing.Color.Cyan, System.Drawing.Color.Magenta, System.Drawing.Color.Brown, System.Drawing.Color.Teal, System.Drawing.Color.DarkGray
            };

            return new SolidBrush(couleurs[i % couleurs.Length]);
        }



        /// <summary>
        /// Transforme des coordonnée au format longitude latite en coordonnée cartésienne
        /// </summary>
        /// <param name="positions">Liste des coordonnée a transformer</param>
        /// <param name="largeur">intervalle possible pour la coordonnée x</param>
        /// <param name="hauteur">intervalle possible pour la coordonnée y</param>
        /// <returns>renvoie la liste des positiions transformer</returns>
        public Dictionary<int, Point> CooCartesienne(Dictionary<int, double[]> positions, int largeur, int hauteur){
            Dictionary<int, Point> result = new Dictionary<int, Point>();
            List<double[]> coos = new List<double[]>();
            int R = 6371;
            int xMax = int.MinValue;
            int xMin = int.MaxValue;
            int yMax = int.MinValue;
            int yMin = int.MaxValue;
            foreach(var (i, pos) in positions){
                double x = R * pos[0] * Math.PI / 180 % 100 * 10;
                double y = R * Math.Log(Math.Tan(Math.PI / 4 + pos[1] * Math.PI / 180 / 2)) % 100 * 10;
                if(x > xMax){
                    xMax = (int)x;
                }
                if(x < xMin){
                    xMin = (int)x;
                }
                if(y > yMax){
                    yMax = (int)y;
                }
                if(y < yMin){
                    yMin = (int)y;
                }
                coos.Add([x, y]);
            }

            for(int i = 0; i < coos.Count; i++){
                result.Add(i, new Point((int)(50 + (coos[i][0] - xMin) * (largeur - 50) / (xMax - xMin)), (int)(50 + (coos[i][1] - yMin) * (hauteur - 50) / (yMax - yMin))));
            }

            return result;
        }

        /// <summary>
        /// Dessine une fleche sur le plans
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="pen">Pen</param>
        /// <param name="start">Point de depart</param>
        /// <param name="end">Point d'arriver</param>
        private void DrawArrow(Graphics g, Pen pen, Point start, Point end){
            AdjustableArrowCap arrowCap = new AdjustableArrowCap(5, 10);
            pen.CustomEndCap = arrowCap;
            g.DrawLine(pen, start, end);
        }

        /// <summary>
        /// algorithme de welsh powell
        /// </summary>
        /// <returns>un dictionnaire avec chaque couleur pour chaque noeud</returns>
        public Dictionary<Noeud<T>, int> WelshPowell()
        {
            Dictionary<Noeud<T>, int> couleurs = new Dictionary<Noeud<T>, int>();
            List<Noeud<T>> listeTriee = new List<Noeud<T>>(noeuds);
            listeTriee.Sort((n1, n2) => liste_adj[n2.IdNoeud].Count.CompareTo(liste_adj[n1.IdNoeud].Count));

            int couleur = 0;

            for (int i = 0; i < listeTriee.Count; i++)
            {
                Noeud<T> noeud = listeTriee[i];

                if (!couleurs.ContainsKey(noeud))
                {
                    couleurs[noeud] = couleur;

                    for (int j = i + 1; j < listeTriee.Count; j++)
                    {
                        Noeud<T> autre = listeTriee[j];

                        if (!couleurs.ContainsKey(autre))
                        {
                            bool estAdjoint = false;

                            foreach (int voisinId in liste_adj[autre.IdNoeud])
                            {
                                Noeud<T> voisin = noeuds[voisinId - 1];

                                if (couleurs.ContainsKey(voisin) && couleurs[voisin] == couleur)
                                {
                                    estAdjoint = true;
                                    break;
                                }
                            }

                            if (!estAdjoint)
                            {
                                couleurs[autre] = couleur;
                            }
                        }
                    }

                    couleur++;
                }
            }

            return couleurs;
        }

        /// <summary>
        /// savoir si un graphe est biparti
        /// </summary>
        /// <returns>si le graphe est biparti</returns>
        public bool EstBiparti()
        {
            Dictionary<Noeud<T>, int> couleurs = WelshPowell();
            int nombreCouleurs = couleurs.Values.Distinct().Count();
            if (nombreCouleurs == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// les groupes de noeuds par couleurs
        /// </summary>
        /// <returns>un dictionnaire avec pour chaque couleur, tous les noeuds à l'intérieur</returns>
        public Dictionary<int, List<Noeud<T>>> GroupesIndependants()
        {
            Dictionary<Noeud<T>, int> couleurs = WelshPowell();
            Dictionary<int, List<Noeud<T>>> groupes = new Dictionary<int, List<Noeud<T>>>();

            foreach (KeyValuePair<Noeud<T>, int> element in couleurs)
            {
                int couleur = element.Value;
                Noeud<T> noeud = element.Key;

                if (!groupes.ContainsKey(couleur))
                {
                    groupes[couleur] = new List<Noeud<T>>();
                }

                groupes[couleur].Add(noeud);
            }

            return groupes;
        }        
    }
}