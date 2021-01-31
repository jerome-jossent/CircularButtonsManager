using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EMGU.CV
{
    public partial class BoutonsCirculaires : Window, INotifyPropertyChanged
    {
        #region Membres Binding
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public int nbrButtons
        {
            get { return _nbrButtons; }
            set
            {
                if (_nbrButtons != value)
                {
                    _nbrButtons = value;
                    OnPropertyChanged("nbrButtons");
                }
            }
        }
        int _nbrButtons = 15;

        public int valUnitaireMax
        {
            get { return _valUnitaireMax; }
            set
            {
                if (_valUnitaireMax != value)
                {
                    _valUnitaireMax = value;
                    OnPropertyChanged("valUnitaireMax");
                }
            }
        }
        int _valUnitaireMax = 12;

        public int diametre
        {
            get { return _diametre; }
            set
            {
                if (_diametre != value)
                {
                    _diametre = value;
                    OnPropertyChanged("diametre");
                }
            }
        }
        int _diametre = 128;

        public int epaisseur
        {
            get { return _epaisseur; }
            set
            {
                if (_epaisseur != value)
                {
                    _epaisseur = value;
                    OnPropertyChanged("epaisseur");
                }
            }
        }
        int _epaisseur = 2;

        public Color couleur
        {
            get { return _couleur; }
            set
            {
                if (_couleur != value)
                {
                    _couleur = value;
                    OnPropertyChanged("couleur");
                }
            }
        }
        Color _couleur = Colors.Blue;

        public Color couleurBackground
        {
            get { return _couleurBackground; }
            set
            {
                if (_couleurBackground != value)
                {
                    _couleurBackground = value;
                    OnPropertyChanged("couleurBackground");
                }
            }
        }
        Color _couleurBackground = Colors.Transparent;

        public double angleDegres_OrigineParRapportAX
        {
            get { return _angleDegres_OrigineParRapportAX; }
            set
            {
                if (_angleDegres_OrigineParRapportAX != value)
                {
                    _angleDegres_OrigineParRapportAX = value;
                    OnPropertyChanged("angleDegres_OrigineParRapportAX");
                }
            }
        }
        double _angleDegres_OrigineParRapportAX = 90;
        #endregion

        public BoutonsCirculaires()
        {
            InitializeComponent();
            DataContext = this;
            LV.Items.Clear();
        }

        #region COMPUTE (MACRO)
        private void Compute(object sender, RoutedEventArgs e)
        {
            LV.Items.Clear();
            System.Threading.Thread thread = new System.Threading.Thread(Compute);
            thread.Start();
        }

        void Compute()
        {
            Dictionary<int, List<List<int>>> operations = OperationsGenerator(nbrButtons, nbrButtons, valUnitaireMax);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    lv.Items.Clear();
                    foreach (List<List<int>> item in operations.Values)
                    {
                        foreach (List<int> entiers in item)
                        {
                            lv.Items.Add(string.Join(" + ", entiers));
                        }
                    }
                }));





            List<MCvScalar> couleurs = new List<MCvScalar>() { new MCvScalar(couleur.B, couleur.G, couleur.R, couleur.A) };
            Compute(operations, 
                    diametre, 
                    epaisseur, 
                    new MCvScalar(couleurBackground.B, couleurBackground.G, couleurBackground.R, couleurBackground.A), 
                    couleurs);
        }

        void Compute(Dictionary<int, List<List<int>>> operations,
            int diametre, int epaisseur,
            MCvScalar couleurFond, List<MCvScalar> couleurs)
        {
            int couleurindex = -1;
            //draw operations
            foreach (int x in operations.Keys)
            {
                couleurindex++;
                if (couleurindex == couleurs.Count) couleurindex = 0;
                MCvScalar couleur = couleurs[couleurindex];

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    TextBlock tb = new TextBlock()
                    {
                        Text = x.ToString(),
                        Foreground = new SolidColorBrush(Color.FromArgb((byte)couleur.V3,
                                                                       (byte)couleur.V2,
                                                                       (byte)couleur.V1,
                                                                       (byte)couleur.V0))
                    };
                    Viewbox vb = new Viewbox() { Child = tb, Height = diametre, Width = diametre };
                    LV.Items.Add(vb);
                }));

                foreach (List<int> entiers in operations[x])
                {
                    string titre = string.Join(" + ", entiers);
                    Emgu.CV.Image<Rgba, byte> image = DrawRings(diametre,
                       epaisseur,
                       couleur,
                       couleurFond,
                       entiers,
                       angleDegres_OrigineParRapportAX
                       );

                    //ajout de l'image dans l'interface
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() =>
                            {
                                LV.Items.Add(AddImageToIHM(image.Mat, titre));
                            }));
                }
            }
        }
        #endregion

        #region COMPUTE (MICRO)
        Dictionary<int, List<List<int>>> OperationsGenerator(int sommeMin, int sommeMax, int valUnitaireMax)
        {
            //génère toutes les sommes possibles (uniques, valeurs croissantes)
            Dictionary<string, List<int>> val = new Dictionary<string, List<int>>();
            for (int i = 0; i < valUnitaireMax + 1; i++)
            {
                for (int j = 0; j < valUnitaireMax + 1; j++)
                {
                    for (int k = 0; k < valUnitaireMax + 1; k++)
                    {
                        for (int l = 0; l < valUnitaireMax + 1; l++)
                        {
                            if (i <= j && j <= k && k <= l)
                            {
                                string clef = "";
                                if (i > 0) clef += $"{i},";
                                if (j > 0) clef += $"{j},";
                                if (k > 0) clef += $"{k},";
                                if (l > 0) clef += $"{l}";

                                if (i + j + k + l >= sommeMin && i + j + k + l <= sommeMax)
                                    if (!val.ContainsKey(clef))
                                    {
                                        if (i > 0) val.Add(clef, new List<int>() { i, j, k, l });
                                        else if (j > 0) val.Add(clef, new List<int>() { j, k, l });
                                        else if (k > 0) val.Add(clef, new List<int>() { k, l });
                                        else if (l > 0) val.Add(clef, new List<int>() { l });
                                    }
                            }
                        }
                    }
                }
            }

            //rangement par somme
            Dictionary<int, List<List<int>>> operations = new Dictionary<int, List<List<int>>>();
            foreach (List<int> item in val.Values)
            {
                int somme = item.Sum();
                if (!operations.ContainsKey(somme))
                    operations.Add(somme, new List<List<int>>());
                operations[somme].Add(item);
            }
            return operations;
        }

        Image<Rgba, byte> DrawRings(int D_ext, int separation_epaisseur, MCvScalar couleur, MCvScalar couleur_arriereplan,
            List<int> nbrBoutonsParAnneau, double angleDegres_OrigineParRapportAX)
        {
            int nbrAnneaux = nbrBoutonsParAnneau.Count;

            //création de l'image
            Image<Rgba, byte> iMAGE = new Image<Rgba, byte>(D_ext, D_ext);
            int rows = iMAGE.Rows;
            int cols = iMAGE.Cols;

            //centre de l'image
            System.Drawing.Point centre = new System.Drawing.Point(cols / 2, rows / 2);

            //créations des rayons externe/interne de chaque anneau
            List<Tuple<int, int>> rayons = new List<Tuple<int, int>>();
            int rprecedent = 0;
            int epaisseur = (int)(cols / 2 * 1f / nbrAnneaux);
            for (int i = nbrAnneaux - 1; i >= 0; i--)
            {
                int externe = (rayons.Count == 0) ? cols / 2 : rprecedent;
                int interne = (rayons.Count == nbrAnneaux - 1) ? 0 : externe - epaisseur;
                rayons.Add(new Tuple<int, int>(externe, interne));
                rprecedent = interne;
            }

            //dessine les anneaux puis y dessine les séparations 
            for (int i = 0; i < nbrAnneaux; i++)
            {
                //cercle extèrieur
                CvInvoke.Circle(iMAGE, centre, rayons[i].Item1 - separation_epaisseur * 2, couleur, -1);
                //cercle intérieur (/!\ attention écrase tout l'intérieur !)
                CvInvoke.Circle(iMAGE, centre, rayons[i].Item2, couleur_arriereplan, -1);

                //dessine les séparations 
                if (nbrBoutonsParAnneau[nbrAnneaux - 1 - i] > 1)
                    DrawSeparator(iMAGE,
                        nbrBoutonsParAnneau[nbrAnneaux - 1 - i],
                                angleDegres_OrigineParRapportAX,
                                rayons[i].Item2,
                                rayons[i].Item1 - separation_epaisseur * 2,
                                rows / 2,
                                cols / 2,
                                couleur_arriereplan,
                                separation_epaisseur);

                #region ------------vieux code commenté
                //int rayon_0 = cols / 2;
                //int epaisseur_1 = (int)(rayon_0 * 1f / 3);
                //int rayon_1 = rayon_0 - epaisseur_1;

                //int epaisseur_2 = (int)(rayon_0 * 1f / 3);
                //int rayon_2 = rayon_1 - epaisseur_2;


                //double angle_OrigineParRapportAX = -90;

                ////cercle extèrieur
                //CvInvoke.Circle(masque, centre, rayon_0, couleur, -1);
                //CvInvoke.Circle(masque, centre, rayon_1, couleur_arriereplan, -1);
                //DrawSeparator(masque,
                //    10,
                //            angle_OrigineParRapportAX,
                //            rayon_1,
                //            rayon_0,
                //            rayon_0,
                //            rayon_0,
                //            couleur_arriereplan,
                //            separation_epaisseur);

                ////cercle intermédiaire
                //CvInvoke.Circle(masque, centre, rayon_1 - separation_epaisseur * 2, couleur, -1);
                //CvInvoke.Circle(masque, centre, rayon_2, couleur_arriereplan, -1);
                //DrawSeparator(masque,
                //    6,
                //            angle_OrigineParRapportAX,
                //            rayon_2,
                //            rayon_1 - separation_epaisseur * 2,
                //            rayon_0,
                //            rayon_0,
                //            couleur_arriereplan,
                //            separation_epaisseur);

                ////disque central
                //CvInvoke.Circle(masque, centre, rayon_2 - separation_epaisseur * 2, couleur, -1);
                //DrawSeparator(masque,
                //    3,
                //            angle_OrigineParRapportAX,
                //            0,
                //            rayon_2 - separation_epaisseur * 2,
                //            rayon_0,
                //            rayon_0,
                //            couleur_arriereplan,
                //            separation_epaisseur);
                #endregion
            }
            return iMAGE;
        }

        void DrawSeparator(Image<Rgba, byte> masque, int nombre_de_part, double angle_OrigineParRapportAX,
                        int rayon_int, int rayon_ext, int RX, int RY, MCvScalar T, int epaisseur_trait)
        {
            double angle = 360 / nombre_de_part;

            for (int i = 0; i < nombre_de_part; i++)
            {
                double alpha = angle_OrigineParRapportAX + angle * i;
                double alpha_rad = alpha / 180 * Math.PI;
                double X, Y;//repère sur cercle
                int x, y;//repère image

                X = rayon_ext * Math.Cos(alpha_rad);
                Y = rayon_ext * Math.Sin(alpha_rad);
                x = (int)(X + RX);
                y = (int)(Y + RY);
                System.Drawing.Point P2 = new System.Drawing.Point(x, y);

                X = rayon_int * Math.Cos(alpha_rad);
                Y = rayon_int * Math.Sin(alpha_rad);
                x = (int)(X + RX);
                y = (int)(Y + RY);
                System.Drawing.Point P1 = new System.Drawing.Point(x, y);

                CvInvoke.Line(masque, P1, P2, T, epaisseur_trait);
            }
        }
        #endregion

        #region AddImageToIHM
        FrameworkElement AddImageToIHM(Emgu.CV.Image<Rgba, byte> image, string txt = "")
        {
            BitmapSource img = ToBitmapSource(image);
            return AddImageToIHM(img, txt);
        }
        FrameworkElement AddImageToIHM(Mat mat, string txt = "")
        {
            BitmapSource img = ToBitmapSource(mat);
            return AddImageToIHM(img, txt);
        }
        FrameworkElement AddImageToIHM(BitmapSource img, string txt = "")
        {
            Image wpfImage = new Image();
            wpfImage.Width = img.Width;
            wpfImage.Source = img;

            if (txt != "")
            {
                Label lbl = new Label();
                lbl.Content = txt;
                lbl.HorizontalContentAlignment = HorizontalAlignment.Center;

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;

                stackPanel.Children.Add(wpfImage);
                stackPanel.Children.Add(lbl);
                return stackPanel;
            }
            else
                return wpfImage;
        }
        #endregion

        #region CONVERSION
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
        #endregion

        #region CODES EN VRAC
        void SETPixelByCode(int D_ext)
        {
            Emgu.CV.Image<Rgba, byte> image = new Image<Rgba, byte>(D_ext, D_ext);

            int rows = image.Rows;
            int cols = image.Cols;
            Byte[,,] data = image.Data;

            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < cols; ++x)
                {
                    data[x, y, 0] = 255;
                    data[x, y, 1] = 255;
                    data[x, y, 2] = 255;
                    data[x, y, 3] = 255;
                }
            }
        }

        void Draw()
        {
            #region PREMIER JET
            ////coupé en 4 (horizontal, vertical)
            ////diagonale NO
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(0, 0), T, 2);
            ////diagonale NE
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols, 0), T, 2);
            ////diagonale SO
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(0, rows), T, 2);
            ////diagonale SE
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols, rows), T, 2);

            ////coupé en 4 (diagonales)
            ////droit N
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols / 2, 0), T, 2);
            ////droit E
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols, rows / 2), T, 2);
            ////droit S
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols / 2, rows), T, 2);
            ////droit O
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(0, rows / 2), T, 2);


            ////coupé en 3
            //int z1 = (int)(rows / 2 - (float)cols / 2 * Math.Tan(30f / 180 * Math.PI));
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols / 2, rows), T, 2);
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(0, z1), T, 2);
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols, z1), T, 2);

            ////coupé en 6 = coupé en 3 +
            //int z2 = (int)(rows / 2 + (float)cols / 2 * Math.Tan(30f / 180 * Math.PI));
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols / 2, 0), T, 2);
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(0, z2), T, 2);
            //CvInvoke.Line(masque, centre, new System.Drawing.Point(cols, z2), T, 2);

            //coupé en 5 ?

            //coupé en 7 ?

            //coupé en 9 ?

            //coupé en 10 ?

            #endregion

            #region opérations
            //Mat img = masque.Mat;
            //Mat img = image.Mat;
            //Mat logo = masque.Mat;

            //Mat mask = new Mat();//8 bit single channel image
            //CvInvoke.CvtColor(logo, mask, ColorConversion.Bgr2Gray);//color to grayscale
            //CvInvoke.BitwiseNot(mask, mask); //Logical NOT
            //AddImageToIHM(mask, "bitwise_not");
            //CvInvoke.Threshold(mask, mask, 100, 255, ThresholdType.Binary); //Binary
            //AddImageToIHM(mask, "threshold");

            //Mat ROI = new Mat(img, new System.Drawing.Rectangle(0, 0, logo.Cols, logo.Rows));
            //logo.CopyTo(ROI, mask); //Copy the data area of ​​the logo to the ROI
            //AddImageToIHM(ROI, "ROI");

            //AddImageToIHM(img, "result");
            //AddImageToIHM(image, "image");
            //AddImageToIHM(masque, "masque");
            #endregion
        }
        #endregion

    }
}