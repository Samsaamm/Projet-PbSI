namespace Projet_Sarah_Maxence_Samuel
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int numero = 5;
            Noeud noeud = new Noeud(numero);
            string attente = "Noeud numéro : " + numero + "\nliste des noeuds voisins : ";

            string res = noeud.toString();

            Assert.AreEqual(attente, res);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Noeud n1 = new Noeud(1);
            Noeud n2 = new Noeud(2);
            Noeud n3 = new Noeud(3);

            n1.Voisins.Add(n2);
            n1.Voisins.Add(n3);

            string attente = "Noeud numéro : 1\nliste des noeuds voisins : ";
            attente += n2.ToString() + "\n";
            attente += n3.ToString() + "\n";

            string res = n1.toString();

            Assert.AreEqual(res, attente);
        }

        [TestMethod]

        public void TestMethod3()
        {
            Noeud n1 = new Noeud(1);
            Noeud n2 = new Noeud(2);
            Lien l = new Lien (n1 , n2);

            string attente = "Lien : Départ du noeud : 1 jusqu'au noeud 2";

            string res = l.toString();

            Assert.AreEqual(res, attente);
        }

    }
}
