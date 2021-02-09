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
                    Properties.Settings.Default._nbrButtons = _nbrButtons;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("nbrButtons");
                }
            }
        }
        int _nbrButtons = Properties.Settings.Default._nbrButtons;

        public int valUnitaireMax
        {
            get { return _valUnitaireMax; }
            set
            {
                if (_valUnitaireMax != value)
                {
                    _valUnitaireMax = value;
                    Properties.Settings.Default._valUnitaireMax = _valUnitaireMax;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("valUnitaireMax");
                }
            }
        }
        int _valUnitaireMax = Properties.Settings.Default._valUnitaireMax;

        public int diametre
        {
            get { return _diametre; }
            set
            {
                if (_diametre != value)
                {
                    _diametre = value;
                    Properties.Settings.Default._diametre = _diametre;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("diametre");
                }
            }
        }
        int _diametre = Properties.Settings.Default._diametre;

        public int epaisseur
        {
            get { return _epaisseur; }
            set
            {
                if (_epaisseur != value)
                {
                    _epaisseur = value;
                    Properties.Settings.Default._epaisseur = _epaisseur;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("epaisseur");
                }
            }
        }
        int _epaisseur = Properties.Settings.Default._epaisseur;

        public Color couleur
        {
            get
            {
                byte[] bytes = BitConverter.GetBytes(_couleur);
                return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            set
            {
                int val = BitConverter.ToInt32(new byte[] { value.B, value.G, value.R, value.A }, 0);
                if (_couleur != val)
                {
                    _couleur = val;
                    Properties.Settings.Default._couleur = _couleur;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("couleur");
                }
            }
        }
        int _couleur = Properties.Settings.Default._couleur;

        public Color couleurBackground
        {
            get
            {
                byte[] bytes = BitConverter.GetBytes(_couleurBackground);
                return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            set
            {
                int val = BitConverter.ToInt32(new byte[] { value.B, value.G, value.R, value.A }, 0);
                if (_couleur != val)
                {
                    _couleurBackground = val;
                    Properties.Settings.Default._couleurBackground = _couleurBackground;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("couleurBackground");
                }
            }
        }
        int _couleurBackground = Properties.Settings.Default._couleurBackground;

        public double angleDegres_OrigineParRapportAX
        {
            get { return _angleDegres_OrigineParRapportAX; }
            set
            {
                if (_angleDegres_OrigineParRapportAX != value)
                {
                    _angleDegres_OrigineParRapportAX = value;
                    Properties.Settings.Default._angleDegres_OrigineParRapportAX = _angleDegres_OrigineParRapportAX;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged("angleDegres_OrigineParRapportAX");
                }
            }
        }
        double _angleDegres_OrigineParRapportAX = Properties.Settings.Default._angleDegres_OrigineParRapportAX;
        #endregion

        Dictionary<string, Proposition> propositions;

        public BoutonsCirculaires()
        {
            InitializeComponent();
            DataContext = this;
            LV.Items.Clear();
        }

        #region COMPUTE (MACRO)
        void Compute(object sender, RoutedEventArgs e)
        {
            LV.Items.Clear();
            System.Threading.Thread thread = new System.Threading.Thread(Compute);
            thread.Start();
        }

        void Compute()
        {
            propositions = new Dictionary<string, Proposition>();

            Dictionary<int, List<List<int>>> operations = OperationsGenerator(nbrButtons, nbrButtons, valUnitaireMax);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    lv.Items.Clear();
                    foreach (List<List<int>> item in operations.Values)
                        foreach (List<int> entiers in item)
                            lv.Items.Add(Proposition.SetSignature(entiers));
                }));

            List<MCvScalar> couleurs = new List<MCvScalar>() { new MCvScalar(couleur.B, couleur.G, couleur.R, couleur.A) };
            Compute(operations, diametre, epaisseur,
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
                    string titre = Proposition.SetSignature(entiers);
                    List<Bouton> boutons = new List<Bouton>();

                    Emgu.CV.Image<Rgba, byte> image = DrawRings(diametre,
                       epaisseur,
                       couleur,
                       couleurFond,
                       entiers,
                       angleDegres_OrigineParRapportAX,
                       ref boutons
                       );

                    Image<Gray, byte> gray = new Image<Gray, byte>(image.Width, image.Height);
                    Image<Gray, byte> binaire = new Image<Gray, byte>(image.Width, image.Height);

                    //Alpha to WHITE
                    SETAlphaPixelToColorPixel(image, new MCvScalar(255, 255, 255, 255));

                    CvInvoke.CvtColor(image.Mat, gray, ColorConversion.Bgra2Gray);//color to grayscale
                    CvInvoke.Threshold(gray, binaire, 100, 255, ThresholdType.Binary); //Binary
                    CvInvoke.BitwiseNot(binaire, binaire); //Logical NOT

                    Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                    Mat hierarchy = new Mat();

                    CvInvoke.FindContours(binaire, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                    //CvInvoke.DrawContours(image.Mat, contours, -1, new MCvScalar(0, 255, 0));

                    //WHITE to Alpha
                    SETColorPixelToAlphaPixel(image, new MCvScalar(255, 255, 255, 255));

                    for (int b = 0; b < boutons.Count; b++)
                    {
                        Bouton bouton = boutons[b];

                        CvInvoke.Circle(image.Mat, bouton.barycentre, 2, new MCvScalar(0, 255, 0, 255), -1);

                        //Quel contour est concerné ?
                        int contour_index = 0;
                        for (contour_index = 0; contour_index < contours.Size; contour_index++)
                        {
                            double result = CvInvoke.PointPolygonTest(contours[contour_index], bouton.barycentre, false);
                            if (result > 0) // >0 : inside
                            {
                                break;
                            }
                        }

                        Mat btn = new Mat(image.Size, DepthType.Cv8U, 1);
                        //colorie tout (-1) l'intérieur en vert (0,255,0,255) du contour n°1
                        CvInvoke.DrawContours(btn, contours, contour_index, new MCvScalar(100), -1);

                        //optimisation sur 2 paramètres :
                        //- p position sur l'axe centre du cercle, barycentre (position comprise entre rayon max et rayon min)
                        //- t taille de l'icone
                        //La meilleure solution p aura t maximum

                        // first values
                        // p0 = barycentre (parfait ou trop prêt du centre)
                        // t0 = 1
                        // test : est ce qu'aucun pixels du carrée n'est vide (on a une image vide avec "seulement" le bouton de dessiné)
                        // ==> on augmente t
                        // quand tmax atteint pour p donné, on change de p, avec p compris entre p0 et p sur axe avec D = rayon_ext - t

                        // puis dichtomie pour p en maximisant t à chaque essai.




                        int cote_px = 10;

                        List<int> icon_cote_px_test = new List<int>();
                        System.Drawing.Size icon_size;
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                        System.Drawing.Point icon_center;
                        ////int cote_px_retenu = 100;
                        //while (!icon_cote_px_test.Contains(cote_px))
                        //{
                        //    icon_cote_px_test.Add(cote_px);

                        //    icon_size = new System.Drawing.Size(cote_px, cote_px);
                        //    icon_center = new System.Drawing.Point(btn.barycentre.X - cote_px / 2, btn.barycentre.Y - cote_px / 2);
                        //    rect = new System.Drawing.Rectangle(icon_center, icon_size);

                        //    int area_theo = cote_px * cote_px;
                        //    Mat ROI = new Mat(image.Mat, rect);
                        //    int area_real = CvInvoke.CountNonZero(ROI);
                        //    int pixelvide = area_theo - area_real;
                        //    Console.WriteLine(cote_px + " ==> " + pixelvide);
                        //    if (pixelvide > 0)
                        //        cote_px--;
                        //    if (pixelvide <= 0)
                        //        cote_px++;
                        //}

                        //cote_px = icon_cote_px_test[icon_cote_px_test.Count - 2];
                        //icon_size = new System.Drawing.Size(cote_px, cote_px);
                        //icon_center = new System.Drawing.Point(btn.barycentre.X - cote_px / 2, btn.barycentre.Y - cote_px / 2);
                        //rect = new System.Drawing.Rectangle(icon_center, icon_size);

                        //CvInvoke.Rectangle(image.Mat,
                        //    rect,
                        //    new MCvScalar(0, 0, 0, 255));
                    }

















                    //for (int i = 0; i < contours.Size; i++)
                    //{
                    //    Mat btn = new Mat(image.Size, DepthType.Cv8U, 1);
                    //    //colorie tout (-1) l'intérieur en vert (0,255,0,255) du contour n°1
                    //    CvInvoke.DrawContours(btn, contours, i, new MCvScalar(100), -1);



                    //    //CHANGER : au lieu du centre de masse, prendre l'axe "parfait" = axe entre 2 séparations
                    //    //pourquoi pas commencer par le point sur cet axe le plus prêt du barycentre
                    //    //puis se balader sur l'axe










                    //    //barycentre (centre de masse)
                    //    MCvMoments mom = CvInvoke.Moments(contours[i]);
                    //    System.Drawing.Point center_of_mass = new System.Drawing.Point((int)((float)mom.M10 / mom.M00), (int)((float)mom.M01 / mom.M00));
                    //    CvInvoke.Circle(image.Mat, center_of_mass, 2, new MCvScalar(0, 255, 0, 255), -1);

                    //    int cote_px = 10;

                    //    List<int> icon_cote_px_test = new List<int>();
                    //    System.Drawing.Size icon_size;
                    //    System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                    //    System.Drawing.Point icon_center;
                    //    //int cote_px_retenu = 100;
                    //    while (!icon_cote_px_test.Contains(cote_px))
                    //    {
                    //        icon_cote_px_test.Add(cote_px);

                    //        icon_size = new System.Drawing.Size(cote_px, cote_px);
                    //        icon_center = new System.Drawing.Point(center_of_mass.X - cote_px / 2, center_of_mass.Y - cote_px / 2);
                    //        rect = new System.Drawing.Rectangle(icon_center, icon_size);

                    //        int area_theo = cote_px * cote_px;
                    //        Mat ROI = new Mat(btn, rect);
                    //        int area_real = CvInvoke.CountNonZero(ROI);
                    //        int pixelvide = area_theo - area_real;
                    //        Console.WriteLine(cote_px + " ==> " + pixelvide);
                    //        if (pixelvide > 0)
                    //            cote_px--;
                    //        if (pixelvide <= 0)
                    //            cote_px++;
                    //    }

                    //    cote_px = icon_cote_px_test[icon_cote_px_test.Count - 2];
                    //    icon_size = new System.Drawing.Size(cote_px, cote_px);
                    //    icon_center = new System.Drawing.Point(center_of_mass.X - cote_px / 2, center_of_mass.Y - cote_px / 2);
                    //    rect = new System.Drawing.Rectangle(icon_center, icon_size);

                    //    CvInvoke.Rectangle(image.Mat,
                    //        rect,
                    //        new MCvScalar(0, 0, 0, 255));



                    //    //int nbr = 100000;
                    //    //while (nbr > 50)
                    //    //{
                    //    //    CvInvoke.Erode(btn_prec, btn, element, new System.Drawing.Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                    //    //    nbr = CvInvoke.CountNonZero(btn);
                    //    //    if (nbr > 50)
                    //    //        btn_prec = btn;
                    //    //    //Console.WriteLine(nbr);
                    //    //    //Mat btn_cpy = new Mat();
                    //    //    //btn.CopyTo(btn_cpy);
                    //    //    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    //    //    //    new Action(() =>
                    //    //    //    {
                    //    //    //        Proposition p = new Proposition(entiers, btn_cpy, titre, this);
                    //    //    //        LV.Items.Add(p.pUC);
                    //    //    //    }));
                    //    //}
                    //    //btn = btn_prec;

                    //    //Emgu.CV.Util.VectorOfVectorOfPoint contourBouton = new Emgu.CV.Util.VectorOfVectorOfPoint();
                    //    //Mat hierarchyBouton = new Mat();

                    //    //CvInvoke.FindContours(btn, contourBouton, hierarchyBouton, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                    //    //System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(contourBouton[0]);


                    //    //System.Drawing.Point centre = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    //    //CvInvoke.Circle(image.Mat, geometric_center, 2, new MCvScalar(0, 0, 255, 255), -1);


                    //}




                    #region BoundingBox
                    //Dictionary<int, double> dict = new Dictionary<int, double>();
                    //for (int i = 0; i < contours.Size; i++)
                    //{
                    //    double aera = CvInvoke.ContourArea(contours[i]);
                    //    System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                    //    dict.Add(i, aera);
                    //}

                    //foreach (var it in dict)//item)
                    //{
                    //    int key = int.Parse(it.Key.ToString());
                    //    System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(contours[key]);
                    //    CvInvoke.Rectangle(image.Mat, rect, new MCvScalar(0, 0, 255), 1);
                    //}
                    #endregion

                    //ajout de l'image dans l'interface
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() =>
                            {
                                //Proposition_UC pUC = AddImageToIHM();
                                Proposition p = new Proposition(entiers, image.Mat, titre, this, boutons);
                                propositions.Add(titre, p);
                                LV.Items.Add(p.pUC);
                            }));
                }
            }
        }
        #endregion

        #region DEBUG (MACRO)
        void DEBUG(object sender, MouseButtonEventArgs e)
        {
            LV.Items.Clear();
            System.Threading.Thread thread = new System.Threading.Thread(Debug);
            thread.Start();
        }
        void Debug()
        {
            propositions = new Dictionary<string, Proposition>();

            Dictionary<int, List<List<int>>> operations = OperationsGenerator(nbrButtons, nbrButtons, valUnitaireMax);

            operations.Clear();
            operations.Add(13, new List<List<int>>() { new List<int>() { 2, 4, 7 } });

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    lv.Items.Clear();
                    foreach (List<List<int>> item in operations.Values)
                        foreach (List<int> entiers in item)
                            lv.Items.Add(Proposition.SetSignature(entiers));
                }));

            List<MCvScalar> couleurs = new List<MCvScalar>() { new MCvScalar(couleur.B, couleur.G, couleur.R, couleur.A) };
            Compute(operations, diametre, epaisseur,
                    new MCvScalar(couleurBackground.B, couleurBackground.G, couleurBackground.R, couleurBackground.A),
                    couleurs);
        }
        #endregion

        #region COMPUTE (MICRO)
        Dictionary<int, List<List<int>>> OperationsGenerator(int sommeMin, int sommeMax, int valUnitaireMax)
        {
            //génère toutes les sommes possibles (uniques, valeurs croissantes)
            Dictionary<string, List<int>> val = new Dictionary<string, List<int>>();
            for (int i = 0; i < valUnitaireMax + 1; i++)
                for (int j = 0; j < valUnitaireMax + 1; j++)
                    for (int k = 0; k < valUnitaireMax + 1; k++)
                        for (int l = 0; l < valUnitaireMax + 1; l++)
                            if (i <= j && j <= k && k <= l)
                            {
                                string clef = "";
                                if (i > 0) clef += $"{i},";
                                if (j > 0) clef += $"{j},";
                                if (k > 0) clef += $"{k},";
                                if (l > 0) clef += $"{l}";

                                if (i + j + k + l >= sommeMin && i + j + k + l <= sommeMax)
                                {
                                    List<int> entiers = new List<int>();

                                    if (i > 0) entiers = new List<int>() { i, j, k, l };
                                    else if (j > 0) entiers = new List<int>() { j, k, l };
                                    else if (k > 0) entiers = new List<int>() { k, l };
                                    else if (l > 0) entiers = new List<int>() { l };

                                    if (!val.ContainsKey(clef))
                                    {
                                        // 1 seul fois 1
                                        int nbr1 = 0;
                                        for (int ent = 0; ent < entiers.Count; ent++)
                                        {
                                            if (entiers[ent] == 1)
                                                nbr1++;
                                        }

                                        if (nbr1 < 2)
                                            val.Add(clef, entiers);
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
            List<int> nbrBoutonsParAnneau, double angleDegres_OrigineParRapportAX, ref List<Bouton> boutons)
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
                if (rayons[i].Item2 > 0)
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
                                separation_epaisseur,
                                ref boutons);

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
                        int rayon_int, int rayon_ext, int RX, int RY, MCvScalar T, int epaisseur_trait,
                        ref List<Bouton> boutons)
        {
            double angle = 360 / nombre_de_part;
            Bouton bouton;
            double alpha, alpha_prec = angle_OrigineParRapportAX;
            for (int i = 0; i < nombre_de_part; i++)
            {
                alpha = angle_OrigineParRapportAX + angle * i;
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

                if (i > 0)
                {
                    bouton = new Bouton(new System.Drawing.Point(RX, RY), rayon_int, rayon_ext, alpha, alpha_prec);
                    boutons.Add(bouton);
                }
                alpha_prec = alpha;
            }

            bouton = new Bouton(new System.Drawing.Point(RX, RY), rayon_int, rayon_ext, alpha_prec - 360, angle_OrigineParRapportAX);
            boutons.Add(bouton);

        }
        #endregion

        #region IHM
        void lv_sel_change(object sender, SelectionChangedEventArgs e)
        {
            if (lv.SelectedItem != null)
            {
                string clef = lv.SelectedItem.ToString();
                if (propositions.ContainsKey(clef))
                    LV.SelectedItem = propositions[lv.SelectedItem.ToString()].pUC;
            }
        }

        internal void del(Proposition proposition)
        {
            LV.Items.Remove(proposition.pUC);
            lv.Items.Remove(proposition.signature);
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

        void SETAlphaPixelToColorPixel(Emgu.CV.Image<Rgba, byte> image, MCvScalar couleur)
        {
            for (int y = 0; y < image.Rows; ++y)
                for (int x = 0; x < image.Cols; ++x)
                    if (image.Data[x, y, 3] == 0)
                    {
                        image.Data[x, y, 0] = (byte)couleur.V0;
                        image.Data[x, y, 1] = (byte)couleur.V1;
                        image.Data[x, y, 2] = (byte)couleur.V2;
                        image.Data[x, y, 3] = (byte)couleur.V3;
                    }
        }

        void SETColorPixelToAlphaPixel(Emgu.CV.Image<Rgba, byte> image, MCvScalar couleur)
        {
            for (int y = 0; y < image.Rows; ++y)
                for (int x = 0; x < image.Cols; ++x)
                    if (image.Data[x, y, 0] == (byte)couleur.V0 &&
                        image.Data[x, y, 1] == (byte)couleur.V1 &&
                        image.Data[x, y, 2] == (byte)couleur.V2 &&
                        image.Data[x, y, 3] == (byte)couleur.V3)
                    {

                        image.Data[x, y, 3] = 0;
                    }
        }

        #region CODES EN VRAC
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

            #region Erode
            ////Isole le bouton 1 et cherche le point le plus éloigné des bords
            //var element = CvInvoke.GetStructuringElement(ElementShape.Cross, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));

            //Mat btn_prec = new Mat();
            //btn.CopyTo(btn_prec);

            //int nbr = 100000;
            //while (nbr > 50)
            //{
            //    CvInvoke.Erode(btn_prec, btn, element, new System.Drawing.Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
            //    nbr = CvInvoke.CountNonZero(btn);
            //    if (nbr > 50)
            //        btn_prec = btn;
            //    //Console.WriteLine(nbr);
            //    //Mat btn_cpy = new Mat();
            //    //btn.CopyTo(btn_cpy);
            //    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            //    //    new Action(() =>
            //    //    {
            //    //        Proposition p = new Proposition(entiers, btn_cpy, titre, this);
            //    //        LV.Items.Add(p.pUC);
            //    //    }));
            //}
            //btn = btn_prec;

            //Emgu.CV.Util.VectorOfVectorOfPoint contourBouton = new Emgu.CV.Util.VectorOfVectorOfPoint();
            //Mat hierarchyBouton = new Mat();

            //CvInvoke.FindContours(btn, contourBouton, hierarchyBouton, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            //System.Drawing.Rectangle rect = CvInvoke.BoundingRectangle(contourBouton[0]);


            //System.Drawing.Point centre = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            //CvInvoke.Circle(image.Mat, geometric_center, 2, new MCvScalar(0, 0, 255, 255), -1);
            #endregion
        }
        #endregion
    }
}