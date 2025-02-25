using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet
{
    internal class Lien
    {
        private Noeud depart;
        private Noeud arrivee;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="depart">noeud de départ</param>
        /// <param name="arrivee">noeud d'arrivée</param>
        public Lien (Noeud depart, Noeud arrivee)
        {
            this.depart = depart;
            this.arrivee = arrivee;
        }

        public Noeud Depart
        {
            get { return depart; }
            set { this.depart = value; }
        }

        public Noeud Arrivee
        {
            get { return arrivee; }
            set { arrivee = value; }
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
