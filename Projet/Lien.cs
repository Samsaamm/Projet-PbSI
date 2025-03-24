using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet
{
    public class Lien<T>
    {
        private Noeud<T> depart;
        private Noeud<T> arrivee;
        private int poids;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="depart">noeud de départ</param>
        /// <param name="arrivee">noeud d'arrivée</param>
        public Lien (Noeud<T> depart, Noeud<T> arrivee)
        {
            this.depart = depart;
            this.arrivee = arrivee;
        }

        public Noeud<T> Depart
        {
            get { return depart; }
            set { this.depart = value; }
        }

        public Noeud<T> Arrivee
        {
            get { return arrivee; }
            set { arrivee = value; }
        }
        public int Poids
        {
            get { return poids; }
        }

        /// <summary>
        /// Affichage des caractéristiques d'un lien
        /// </summary>
        /// <returns>le début et la fin du lien</returns>

        public string toString()
        {
            string s = "Lien : Départ du noeud : " + depart.Numnoeud + " jusqu'au noeud " + arrivee.Numnoeud;
            return s;
        }
    }
}
