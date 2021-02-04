using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMGU.CV
{
    // pour une liste d'entiers donnée, résultat de la création d'un ensemble circulaire de boutons
    public class Proposition
    {
        BoutonsCirculaires bc;
        public Proposition_UC pUC;
        public int somme;
        public List<int> entiers;
        public string signature;
        public List<Bouton> boutons;
        public Proposition(List<int> entiers, Emgu.CV.Mat mat, string txt, BoutonsCirculaires bc, List<Bouton> boutons)
        {
            System.Windows.Media.Imaging.BitmapSource img = BoutonsCirculaires.ToBitmapSource(mat);
            this.entiers = entiers;
            pUC = new Proposition_UC();
            pUC.LINK(this);
            somme = entiers.Sum();
            signature = SetSignature(entiers);

            pUC.wpfImage.Width = img.Width;
            pUC.wpfImage.Source = img;
            pUC.lbl.Content = txt;

            this.boutons = boutons;
            this.bc = bc;
        }

        public static string SetSignature(List<int> entiers)
        {
            return string.Join(" + ", entiers);
        }

        internal void del()
        {
            bc.del(this);
        }
    }
}
