using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

namespace Projet_Sarah_Maxence_Samuel{
    public class Noeud<T>{
        private int idNoeud;
        private T valeurNoeud;
        private double coX;
        private double coY;

        /// <summary>
        /// Constructeur du noeud sans coordonnée predefinie
        /// </summary>
        /// <param name="Id">id du neoud</param>
        /// <param name="valeur">valeur du noeud</param>
        public Noeud(int Id, T valeur){
            this.idNoeud = Id;
            this.valeurNoeud = valeur;
            Random rand = new Random();
            coX = rand.Next(20, 70);
            coY = rand.Next(0, 10);
        }

        
        /// <summary>
        /// Constructeur du noeud avec coordonnée predefinie
        /// </summary>
        /// <param name="Id">id du noeud</param>
        /// <param name="valeur">valeur du noeud</param>
        /// <param name="X">coordonnée X</param>
        /// <param name="Y">coordonnée Y</param>
        public Noeud(int Id, T valeur, double X, double Y){
            this.idNoeud = Id;
            this.valeurNoeud = valeur;
            this.coX = X;
            this.coY = Y;
        }

        /// <summary>
        /// propriété Idnoeud
        /// </summary>
        public int IdNoeud{
            get{ return this.idNoeud;}
            set{ this.idNoeud = value;}
        }

        /// <summary>
        /// propriété
        /// </summary>
        public T ValeurNoeud{
            get{ return this.valeurNoeud;}
        }

        /// <summary>
        /// propriété
        /// </summary>
        public double CoX{
            get{ return this.coX;}
            set{ this.coX = value;}
        }

        /// <summary>
        /// propriété
        /// </summary>

        public double CoY{
            get{ return this.coY;}
            set{ this.coY = value;}
        }

        /// <summary>
        /// méthode pour savoir si deux noeuds sont les mêmes
        /// </summary>
        /// <param name="n2">noeud a vérifier</param>
        /// <returns>si les noeuds sont les mêmes</returns>

        public bool Equals(Noeud<T> n2){
            bool res = false;
            if(Convert.ToString(this.valeurNoeud) == Convert.ToString(n2.valeurNoeud)){
                res = true;
            }
            return res;
        }

        /// <summary>
        /// affichage des caractéristiques du noeud
        /// </summary>
        /// <returns>l'identifiant du noeud</returns>

        public override string ToString()
        {
            return $"Noeud({idNoeud})";
        }
    }
}
