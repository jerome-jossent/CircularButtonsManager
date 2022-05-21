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

        public string parent_name = "";

        //public Element(string name, string[] charSep, RingButtons ringButtons)
        //{
        //    this.name = name;
        //    this.display = name;
        //    this.ringButtons = ringButtons;

        //    deg = name.Split(charSep, StringSplitOptions.RemoveEmptyEntries);

        //    /////anneau_index = deg.Length - 1;

        //    //trouver le nom du parent dans le nom
        //    for (int i = 0; i < deg.Length - 1; i++)
        //    {
        //        if (parent_name.Length > 0)
        //            parent_name += charSep[0];
        //        parent_name += deg[i];
        //    }
        //    isOrigin = parent_name == "";
        //    if (isOrigin)
        //        ringButtons.origine = this;

        //    if (ringButtons.elements.ContainsKey(parent_name))
        //        parent = ringButtons.elements[parent_name];
        //}

        public Element(string name, string[] charSep, string displayname, RingButtons ringButtons)
        {
            this.name = name;
            this.display = displayname;
            this.ringButtons = ringButtons;

            deg = name.Split(charSep, StringSplitOptions.RemoveEmptyEntries);

            /////anneau_index = deg.Length - 1;

            //trouver le nom du parent dans le nom
            for (int i = 0; i < deg.Length - 1; i++)
            {
                if (parent_name.Length > 0)
                    parent_name += charSep[0];
                parent_name += deg[i];
            }
            isOrigin = parent_name == "";
        }

        public Element(string name, string displayname, RingButtons ringButtons, bool isOrigin)
        {
            this.name = name;
            this.display = displayname;
            this.ringButtons = ringButtons;
            this.isOrigin = isOrigin;
        }

        public static Dictionary<string, Element> CreateElements(List<string> datas, string[] charSep, RingButtons ringButtons)
        {
            Dictionary<string, Element> elements = new Dictionary<string, Element>();
            for (int i = 0; i < datas.Count; i++)
            {
                Element element = new Element(datas[i], charSep, datas[i].Replace(charSep[0], ""), ringButtons);
                if (elements.ContainsKey(element.parent_name))
                    element.parent = elements[element.parent_name];
                elements.Add(element.name, element);
            }
            return elements;
        }

        public static void CompleteElements(ref Dictionary<string, Element> elements, RingButtons ringButtons, string OrigineDisplayName, string stringToRemoveInNameForDisplayName,
            out Element origine, out int nbr_anneaux)
        {
            //Dictionary<string, Element> orphelins = new Dictionary<string, Element>();
            Dictionary<string, Element> parents_manquant = new Dictionary<string, Element>();

            origine = new Element("OrigineDisplayName", OrigineDisplayName, ringButtons, true);
            foreach (Element elem in elements.Values)
            {
                if (elem.parent == null)
                {
                    if (!parents_manquant.ContainsKey(elem.parent_name))
                    {
                        Element newParent = new Element(elem.parent_name, elem.parent_name.Replace(stringToRemoveInNameForDisplayName, ""), ringButtons, false);
                        newParent.parent = origine;
                        parents_manquant.Add(elem.parent_name, newParent);
                    }
                    elem.parent = parents_manquant[elem.parent_name];

                    //elem.parent.children.Add(elem.parent.children.Count, elem);

                    //elem.parent = origine;
                    //origine.children.Add(origine.children.Count, elem);
                }
            }

            elements.Add(origine.name, origine);
            foreach (var item in parents_manquant.Values)
                elements.Add(item.name, item);


            //quand on est parent chercher ses enfants
            foreach (Element parent in elements.Values)
                foreach (Element enfant in elements.Values)
                    if (enfant.parent?.name == parent.name)
                    {
                        enfant.ordre = parent.children.Values.Count;
                        parent.children.Add(enfant.ordre, enfant);
                    }

            //set anneau_index
            nbr_anneaux = 0;
            foreach (Element e in elements.Values)
            {
                e.anneau_index = 0;
                Element p = e.parent;
                while (p != null)
                {
                    e.anneau_index++;
                    p = p.parent;
                }
                if (nbr_anneaux < e.anneau_index)
                    nbr_anneaux = e.anneau_index;
            }

            //order elements by anneau_index (then by name ?)
            Dictionary<string, Element> elements_ordered = new Dictionary<string, Element>();
            for (int i = 0; i < nbr_anneaux + 1; i++)
            {
                foreach (var ele in elements)
                {
                    if (ele.Value.anneau_index == i)
                    {
                        elements_ordered.Add(ele.Key, ele.Value);
                    }
                }
            }

            elements = elements_ordered;
        }

        public override string ToString()
        {
            return $"{name} anneau {anneau.index} {angle_ouverture_deg} {angle_ouverture_deb} {angle_ouverture_fin}";
        }
    }
}