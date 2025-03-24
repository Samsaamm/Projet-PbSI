using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet
{
    public class Noeud<T>
    {
        private int numnoeud;
        private List<T> voisins;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="numnoeud">indique le numéro du noeud</param>
        public Noeud (int numnoeud)
        {
            this.numnoeud = numnoeud;
            this.voisins = new List<T>();
        }


        public int Numnoeud
        {
            get { return numnoeud; }
            set { this.numnoeud = value; }
        }

        public List<T> Voisins
        {
            get { return voisins; }
            set { voisins = value; }
        }

        /// <summary>
        /// Affiche les caractéristiques d'un noeud
        /// </summary>
        /// <returns>les caractéristiques d'un noeud avec sa liste de voisins</returns>
        public string toString()
        {
            string s = "Noeud numéro : " + numnoeud + "\nliste des noeuds voisins : ";
            foreach (T n in voisins)
            {
                s += n.ToString() + "\n";
            }
            return s;
        }
    }
}
