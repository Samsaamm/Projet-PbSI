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
        public Dictionary<Noeud, List<Noeud>> mat_adj;

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
            try{

            }catch(exception E){
        
            }finally{

            }
        }

        public Dictionary<Noeud, List<Noeud>> Matrice_adj(){
            
        }

        public void DFS(){

        }

        public void BFS(){
            
        }

        public void Show(){

        }
    }
}
