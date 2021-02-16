using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace arc
{
    class Objects { }

    public class RingButtons
    {
        public Dictionary<int, Ring> anneaux;
        public Dictionary<string, Element> elements;
        internal Dictionary<string, Button> boutons;
        public System.Windows.Point centre;

        public RingButtons(List<string> abc, string[] charSep)
        {
            float epaisseur = 100;
            float angle_origine_deg = 0;

            #region create Elements
            elements = new Dictionary<string, Element>();
            for (int i = 0; i < abc.Count; i++)
            {
                Element element = new Element(abc[i], charSep, this);
                elements.Add(element.name, element);
            }

            //quand on est parent chercher ses enfants
            foreach (Element parent in elements.Values)
                foreach (Element enfant in elements.Values)
                    if (enfant.parent?.name == parent.name)
                    {
                        enfant.ordre = parent.children.Values.Count;
                        parent.children.Add(enfant.ordre, enfant);
                    }
            #endregion

            #region create Rings
            anneaux = new Dictionary<int, Ring>();
            foreach (Element ele in elements.Values)
            {
                if (!anneaux.ContainsKey(ele.anneau_index))
                    anneaux.Add(ele.anneau_index, new Ring()
                    {
                        index = ele.anneau_index,
                        rayon_interne = ele.anneau_index * epaisseur,
                        rayon_externe = (ele.anneau_index + 1) * epaisseur,
                        angle_origine = angle_origine_deg,
                        ringButtons = this
                    });
                anneaux[ele.anneau_index].elements.Add(ele);
                ele.anneau = anneaux[ele.anneau_index];
            }
            #endregion

            #region define centre
            float r = anneaux.Last().Value.rayon_externe;
            centre = new System.Windows.Point(r, r);
            #endregion
        }
    }

    public class Element
    {
        public string name;
        public string display;
        public Dictionary<int, Element> children = new Dictionary<int, Element>();
        public int ordre;
        public Element parent = null;
        internal Button button;
        public Ring anneau;
        public int anneau_index;
        string[] deg;
        public RingButtons ringButtons;
        public bool isOrigin;

        public float angle_ouverture_deb;
        public float angle_ouverture_fin;
        public float angle_ouverture_deg;

        public Element(string name, string[] charSep, RingButtons ringButtons)
        {
            this.name = name;
            this.display = name;
            this.ringButtons = ringButtons;

            deg = name.Split(charSep, StringSplitOptions.RemoveEmptyEntries);

            anneau_index = deg.Length - 1;

            //trouver le parent
            string parent_name = "";
            for (int i = 0; i < deg.Length - 1; i++)
            {
                if (parent_name.Length > 0)
                    parent_name += charSep[0];
                parent_name += deg[i];
            }
            isOrigin = parent_name == "";

            if (ringButtons.elements.ContainsKey(parent_name))
                parent = ringButtons.elements[parent_name];

        }

        public override string ToString()
        {
            return $"{name} anneau {anneau.index} {angle_ouverture_deg} {angle_ouverture_deb} {angle_ouverture_fin}";
        }
    }

    public class Ring
    {
        public int index;
        public float angle_origine; //degrès par rapport à 12h00, dans le sens horaire
        public float rayon_externe;
        public float rayon_interne;
        public List<Element> elements = new List<Element>();
        public RingButtons ringButtons;
    }
    internal class Button
    {
        internal Ring ring;
        internal float angle_deb; //par rapport à anneau
        internal float angle_fin; //par rapport à anneau

        //public int ring_index;
        internal int btn_index;
        internal string name;
        internal Brush couleur;
        internal Brush couleurfonce;
        internal Element element;
    }
}
