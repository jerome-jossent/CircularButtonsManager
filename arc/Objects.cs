using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace arc
{
    class Objects { }

    public class RingButtons
    {
        Dictionary<string, Element> elements;
        private List<string> abc;

        public RingButtons(List<string> abc)
        {
            this.abc = abc;

            Dictionary<string, Element> ABC_Dico = new Dictionary<string, Element>();
            Dictionary<int, Ring> Anneau_Dico = new Dictionary<int, Ring>();

            string[] charSep = new string[1] { "," };

            float epaisseur = 100;
            float angle_origine_deg = 0;

            for (int i = 0; i < abc.Count; i++)
            {
                string[] deg = abc[i].Split(charSep, StringSplitOptions.RemoveEmptyEntries);

                int Anneau_index = deg.Length - 1;
                if (deg.Length == 1)
                {
                    //niveau 0
                    if (!Anneau_Dico.ContainsKey(Anneau_index))
                        Anneau_Dico.Add(Anneau_index, new Ring() { index = Anneau_index, rayon_interne = 0, rayon_externe = epaisseur, angle_origine = angle_origine_deg });
                    Button b_origine = new Button() { ring = Anneau_Dico[Anneau_index], angle_deb = 0, angle_fin = 360 };
                    Element origine = new Element() { parent = null, display = deg[i], name = abc[i], button = b_origine };
                    ABC_Dico.Add(origine.name, origine);
                }
                else
                {
                    if (!Anneau_Dico.ContainsKey(Anneau_index))
                        Anneau_Dico.Add(Anneau_index, new Ring()
                        {
                            index = Anneau_index,
                            rayon_interne = Anneau_Dico[Anneau_index - 1].rayon_externe,
                            rayon_externe = Anneau_Dico[Anneau_index - 1].rayon_externe + epaisseur,
                            angle_origine = angle_origine_deg
                        });

                    if (deg.Length == 2)
                    {
                        //répartition équitable des niveaux 1

                    }
                    else
                    {

                    }
                }
            }



            Ring r1 = new Ring() { index = 1, rayon_interne = 100, rayon_externe = 200, angle_origine = 0 };
            Ring r2 = new Ring() { index = 2, rayon_interne = 200, rayon_externe = 300, angle_origine = 0 };




        }
    }

    public class Element
    {
        public string name;
        public string display;
        public List<Element> children = new List<Element>();
        public Element parent;
        public Button button;
    }

    public class Ring
    {
        public int index;
        public float angle_origine; //degrès par rapport à 12h00, dans le sens horaire
        public float rayon_externe;
        public float rayon_interne;
    }
    public class Button
    {
        public Ring ring;
        public float angle_deb; //par rapport à anneau
        public float angle_fin; //par rapport à anneau
    }
}
