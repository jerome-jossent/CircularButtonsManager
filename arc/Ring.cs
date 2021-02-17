using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace arc
{
    public class Ring
    {
        public int index;
        public float angle_origine; //degrès par rapport à 12h00, dans le sens horaire
        public float rayon_externe;
        public float rayon_interne;
        public List<Element> elements = new List<Element>();
        public RingButtons ringButtons;
    }
}
