using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace arc
{
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
