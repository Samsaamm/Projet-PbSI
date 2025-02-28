using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet
{
    internal class Graphe
    {
        public string titre;
        public bool IsOriented;
        public List<Noeud> noeuds;
        public List<Lien> liens;
        public Dictionary<Noeud, List<Noeud>> Liste_adj;
        public int[,] Matrice_adj;

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

                    for(int i = 0; i < File.Count; i++){
                        string ligne = File[i];
                        if(ligne[0] != '%'){
                            Console.WriteLine(ligne);
                            /* for (int j = 0; j < ligne.Length; ++){
                                
                            } */
                        }
                    }
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
