﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Projet{
    public class Lien<T>{
        private Noeud<T> depart;
        private Noeud<T> arrivee;
        private float poids;

        /// <summary>
        /// Constructeur d'un lien
        /// </summary>
        /// <param name="depart">noeud de depart</param>
        /// <param name="arrivee">noeud d'arriver</param>
        /// <param name="poid">poid du lien</param>
        public Lien(Noeud<T> depart, Noeud<T> arrivee, float poid){
            this.depart = depart;
            this.arrivee = arrivee;
            this.poids = poid;
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

        public float Poids
        {
            get { return poids; }
            set { poids = value; }
        }
    }
}