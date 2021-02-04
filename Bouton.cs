using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMGU.CV
{
    /// <summary>
    /// segment entre A et B
    /// grand arc de cercle entre B et C
    /// segment entre C et D
    /// petit arc de cercle entre D et A
    /// </summary>
    public class Bouton
    {
        public System.Drawing.Point centre; // centre de l'ensemble des boutons
        public double rayon_petit;
        public double rayon_grand;
        public double angle1;
        public double angle2;

        public System.Drawing.Point barycentre;
        public System.Drawing.Point A;
        public System.Drawing.Point B;
        public System.Drawing.Point C;
        public System.Drawing.Point D;

        public Bouton(System.Drawing.Point centre, double rayon_petit, double rayon_grand, double angle1, double angle2)
        {
            this.centre = centre;
            this.rayon_petit = rayon_petit;
            this.rayon_grand = rayon_grand;
            this.angle1 = angle1;
            this.angle2 = angle2;

            A = new System.Drawing.Point(centre.X + (int)(rayon_petit * Math.Cos(angle1 / 180 * Math.PI)),
                                         centre.Y + (int)(rayon_petit * Math.Sin(angle1 / 180 * Math.PI)));

            B = new System.Drawing.Point(centre.X + (int)(rayon_grand * Math.Cos(angle1 / 180 * Math.PI)),
                                         centre.Y + (int)(rayon_grand * Math.Sin(angle1 / 180 * Math.PI)));

            C = new System.Drawing.Point(centre.X + (int)(rayon_grand * Math.Cos(angle2 / 180 * Math.PI)),
                                         centre.Y + (int)(rayon_grand * Math.Sin(angle2 / 180 * Math.PI)));

            D = new System.Drawing.Point(centre.X + (int)(rayon_petit * Math.Cos(angle2 / 180 * Math.PI)),
                                         centre.Y + (int)(rayon_petit * Math.Sin(angle2 / 180 * Math.PI)));

            //centre du bouton : origine de l'icône
            double angle_m = (angle1 + angle2) / 2;
            double rayon_m = (rayon_petit + rayon_grand) / 2;
            int x = (int)(rayon_m * Math.Cos(angle_m / 180 * Math.PI)) + centre.X;
            int y = (int)(rayon_m * Math.Sin(angle_m / 180 * Math.PI)) + centre.Y;
            barycentre = new System.Drawing.Point(x, y);
        }

    }
}
