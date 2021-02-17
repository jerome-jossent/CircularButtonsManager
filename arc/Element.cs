using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace arc
{
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


}
