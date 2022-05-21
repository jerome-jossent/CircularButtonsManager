using Advise_IHM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace arc
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //http://csharphelper.com/blog/2019/05/make-an-intuitive-extension-method-to-draw-an-elliptical-arc-in-wpf-and-c/
        //http://csharphelper.com/blog/2014/12/find-the-area-where-circles-overlap-in-c/
        List<SolidColorBrush> colors;
        Dictionary<string, Button> dico;
        RingButtons ABC;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            MenuDynamiqueHide();
            colors = new List<SolidColorBrush>() {Brushes.Red, Brushes.Green, Brushes.Yellow, Brushes.Magenta, Brushes.Blue,
                                                Brushes.Cyan, Brushes.Orange, Brushes.Brown, Brushes.Khaki, Brushes.Salmon };
        }

        #region LAUNCHERS=========================================================
        private void Mode_Manu(object sender, RoutedEventArgs e)
        {
            MenuDynamiqueHide();
            CreateButtons();
        }

        void Mode_Dynamique(object sender, RoutedEventArgs e)
        {
            if (slider_dynamique.Height == new GridLength(0))
            {
                slider_dynamique.Height = new GridLength(1, GridUnitType.Auto);
                CreateButtons_FromSliders2();
            }
            else
                MenuDynamiqueHide();
        }

        void MenuDynamiqueHide()
        {
            slider_dynamique.Height = new GridLength(0);
        }

        void Mode_Source(object sender, RoutedEventArgs e)
        {
            MenuDynamiqueHide();
            CreateButtonsModeSource();
        }

        //From Operations...
        //Dictionary<int, List<List<int>>> operations = Operation.OperationsGenerator(10, 20, 2);
        #endregion

        #region Mode_Manu=========================================================
        void CreateButtons()
        {
            dico = new Dictionary<string, Button>();

            int marge = 0;
            float r_max = 300;
            float r_1 = r_max * 1 / 6;
            float r_2 = r_max * 1 / 3;

            Canvas c = new Canvas();
            c.Children.Add(DrawSecteurs(0, r_1, r_max, r_1, 3, marge));
            c.Children.Add(DrawSecteurs(1, r_2 + r_1, r_max, r_2, 9, marge));
            c.Children.Add(DrawSecteurs(2, r_max, r_max, r_max - r_2 - r_1, 18, marge));

            grd.Children.Clear();
            grd.Children.Add(c);
        }
        #endregion

        #region Mode_Dynamique====================================================
        void CreateButtons_FromSliders2()
        {
            int marge = 0;
            float r_max = R0_R + R1_R + R2_R + R3_R + R4_R;
            //float epaisseur = r_max / sliders.Count;
            dico = new Dictionary<string, Button>();

            Viewbox v = new Viewbox();
            Canvas c = new Canvas();
            c.Children.Add(DrawSecteurs(0, R0_R, r_max, R0_R, R0_B, marge));
            c.Children.Add(DrawSecteurs(1, R0_R + R1_R, r_max, R1_R, R1_B, marge));
            c.Children.Add(DrawSecteurs(2, R0_R + R1_R + R2_R, r_max, R2_R, R2_B, marge));
            c.Children.Add(DrawSecteurs(3, R0_R + R1_R + R2_R + R3_R, r_max, R3_R, R3_B, marge));
            c.Children.Add(DrawSecteurs(4, R0_R + R1_R + R2_R + R3_R + R4_R, r_max, R4_R, R4_B, marge));

            lbl_nbr_boutons.Content = dico.Count + " boutons";

            c.Width = r_max;
            c.Height = r_max;
            c.HorizontalAlignment = HorizontalAlignment.Left;
            c.VerticalAlignment = VerticalAlignment.Top;
            v.Child = c;
            v.HorizontalAlignment = HorizontalAlignment.Left;
            v.VerticalAlignment = VerticalAlignment.Top;

            grd.Children.Clear();

            //v.Stretch = Stretch.UniformToFill;
            grd.Children.Add(v);
        }

        void ME2(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = ABC.boutons[p.Name].element.display + " ENTER";
            p.Fill = ABC.boutons[p.Name].couleurfonce;
        }

        void MD2(object sender, MouseButtonEventArgs e)
        {
            Path p = (Path)sender;
            Title = ABC.boutons[p.Name].element.display + " DOWN";
            p.Fill = ABC.boutons[p.Name].couleur;
        }

        void ML2(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = ABC.boutons[p.Name].element.display + " LEAVE";
            p.Fill = ABC.boutons[p.Name].couleur;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region Binding
        public int R0_B
        {
            get { return _R0_B; }
            set { _R0_B = value; OnPropertyChanged("R0_B"); CreateButtons_FromSliders2(); }
        }
        int _R0_B = 3;
        public int R1_B
        {
            get { return _R1_B; }
            set { _R1_B = value; OnPropertyChanged("R1_B"); CreateButtons_FromSliders2(); }
        }
        int _R1_B = 7;
        public int R2_B
        {
            get { return _R2_B; }
            set { _R2_B = value; OnPropertyChanged("R2_B"); CreateButtons_FromSliders2(); }
        }
        int _R2_B = 12;
        public int R3_B
        {
            get { return _R3_B; }
            set { _R3_B = value; OnPropertyChanged("R3_B"); CreateButtons_FromSliders2(); }
        }
        int _R3_B = 15;
        public int R4_B
        {
            get { return _R4_B; }
            set { _R4_B = value; OnPropertyChanged("R4_B"); CreateButtons_FromSliders2(); }
        }
        int _R4_B = 20;
        //------------------------
        public int R0_R
        {
            get { return _R0_R; }
            set { _R0_R = value; OnPropertyChanged("R0_R"); CreateButtons_FromSliders2(); }
        }
        int _R0_R = 150;
        public int R1_R
        {
            get { return _R1_R; }
            set { _R1_R = value; OnPropertyChanged("R1_R"); CreateButtons_FromSliders2(); }
        }
        int _R1_R = 120;
        public int R2_R
        {
            get { return _R2_R; }
            set { _R2_R = value; OnPropertyChanged("R2_R"); CreateButtons_FromSliders2(); }
        }
        int _R2_R = 100;
        public int R3_R
        {
            get { return _R3_R; }
            set { _R3_R = value; OnPropertyChanged("R3_R"); CreateButtons_FromSliders2(); }
        }
        int _R3_R = 90;
        public int R4_R
        {
            get { return _R4_R; }
            set { _R4_R = value; OnPropertyChanged("R4_R"); CreateButtons_FromSliders2(); }
        }
        int _R4_R = 80;
        #endregion
        #endregion

        #region Mode_Manu & Mode_Dynamique========================================
        Viewbox DrawSecteurs(int ring_index, float r_ext, float r_extMAX, float epaisseur, int nbrboutons, float marge)
        {
            float r_int = r_ext - epaisseur;

            float angle_ouverture_deg = (float)360 / nbrboutons;
            float angle_position_deg_init = angle_ouverture_deg / 2;

            Point c = new Point(r_ext, r_ext);
            Grid g = new Grid();
            g.Width = r_ext * 2;
            g.Height = r_ext * 2;
            for (int i = 0; i < nbrboutons; i++)
            {
                Button b = new Button();
                float angle_position_deg = angle_position_deg_init + i * angle_ouverture_deg;
                Path p = DrawSecteur(c,
                                    r_ext,
                                    r_extMAX,
                                    r_int,
                                    marge,
                                    angle_ouverture_deg,
                                    angle_position_deg);

                p.Name = "ring_" + ring_index + "_btn_" + i;

                int j = i;
                while (j >= colors.Count)
                    j -= colors.Count;
                p.Fill = colors[j];

                b.couleur = p.Fill;
                Color color = ((SolidColorBrush)b.couleur).Color;
                float fctr = 0.8f;
                byte offest = 50;
                b.couleurfonce = new SolidColorBrush(Color.FromRgb(ChangeIntensity(color.R, fctr, offest),
                                                                    ChangeIntensity(color.G, fctr, offest),
                                                                    ChangeIntensity(color.B, fctr, offest)));
                b.btn_index = i;
                b.name = p.Name;

                dico.Add(b.name, b);
                g.Children.Add(p);
            }

            Viewbox v = new Viewbox();
            v.StretchDirection = StretchDirection.Both;
            v.Stretch = Stretch.UniformToFill;
            v.Width = r_ext;
            v.Height = r_ext;
            v.Child = g;
            v.Margin = new Thickness((r_extMAX - r_ext) / 2);
            return v;
        }

        byte ChangeIntensity(byte val, float factor, byte offset)
        {
            int v = (int)val;
            if (factor > 1)
                v += offset;
            else if (factor < 1)
                v -= offset;

            v = (int)(v * factor);

            if (v > 255)
                v = 255;

            if (v < 0)
                v = 0;
            return (byte)v;
        }

        Path DrawSecteur(Point centre, float r_ext, float r_extMAX, float r_int, float marge, float angle_ouverture_deg, float angle_position_deg)
        {
            //disques
            CombinedGeometry anneau = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                           new EllipseGeometry(centre, r_ext, r_ext),
                                                           new EllipseGeometry(centre, r_int, r_int));
            CombinedGeometry c;
            if (angle_ouverture_deg == 360)
                c = anneau;
            else
            {
                //triangle - centré !!!!
                float d = (float)(marge / Math.Cos(angle_ouverture_deg / 2 * Math.PI / 180));
                float X = r_ext - (float)(r_ext * Math.Tan(angle_ouverture_deg / 2 * Math.PI / 180));
                float X_ = r_ext + (float)(r_ext * Math.Tan(angle_ouverture_deg / 2 * Math.PI / 180));

                if (X < -100000) X = -100000;
                if (X_ > 100000) X_ = 100000;

                Point c_ = new Point(r_ext, r_ext - d);
                LineSegment x = new LineSegment(new Point(X, -d), true);
                LineSegment x_ = new LineSegment(new Point(X_, -d), true);

                LineSegment[] s = new LineSegment[] { x, x_ };
                PathFigure t = new PathFigure(c_, s, true);
                PathGeometry triangle = new PathGeometry(new PathFigure[] { t });
                //secteur
                c = new CombinedGeometry(GeometryCombineMode.Intersect, anneau, triangle);
            }

            Path p = new Path();
            p.RenderTransform = new RotateTransform(angle_position_deg, centre.X, centre.Y);
            p.Data = c;
            p.MouseDown += MD;
            p.MouseEnter += ME;
            p.MouseLeave += ML;
            //p.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
            p.Stroke = Brushes.Black;
            p.StrokeThickness = 2;
            return p;
        }

        void ME(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " ENTER";
            p.Fill = dico[p.Name].couleurfonce;
        }

        void MD(object sender, MouseButtonEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " DOWN";
            p.Fill = dico[p.Name].couleur;
        }

        void ML(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " LEAVE";
            p.Fill = dico[p.Name].couleur;
        }
        #endregion

        #region ModeSource========================================================
        void CreateButtonsModeSource()
        {
            string pathAdviseProgramData = AppDomain.CurrentDomain.BaseDirectory; // @"C:\advise\Data\";
            string defConfigurationEdition = pathAdviseProgramData + @"BaseDefs13508_V2.fr";
            Dictionary<string, CodeObservable> observablesNorme = null; // Base des observables
            ObservablesReader observablesReader = new ObservablesReader(defConfigurationEdition, ref observablesNorme);

            List<string> abc = new List<string>();
            abc.Add("BAF");
            foreach (var item in observablesNorme)
            {
                string key = item.Key;
                if (key.IndexOf(abc[0]) < 0 || key == abc[0])
                    continue;

                string clef = abc[0];
                for (int i = abc[0].Length; i < key.Length; i++)
                {
                    clef += ";" + key[i];
                }
                abc.Add(clef);
            }

            //abc.Add("A");
            //abc.Add("B");
            //abc.Add("C");
            //abc.Add("D");

            //foreach (var item in observablesNorme)
            //{
            //    string key = item.Key;
            //    if (key.IndexOf(abc[0]) < 0 || key == abc[0])
            //        continue;

            //    string clef = abc[0];
            //    for (int i = abc[0].Length; i < key.Length; i++)
            //    {
            //        clef += ";" + key[i];
            //    }
            //    abc.Add(clef);
            //}

            #region commenté
            //List<string> abc = new List<string>() {
            //                                        "ABC",
            //                                        "ABC;A", "ABC;B", "ABC;C", "ABC;D",
            //                                        "ABC;A;A","ABC;A;B",
            //                                        "ABC;A;B;A","ABC;A;B;B",
            //                                        "ABC;A;A;A","ABC;A;A;B",
            //                                        "ABC;C;A","ABC;C;B","ABC;C;C",
            //                                        "ABC;D;A","ABC;D;B","ABC;D;C","ABC;D;D",
            //                                        "ABC;D;A;A","ABC;D;A;B","ABC;D;A;C","ABC;D;A;D",
            //                                        "ABC;D;A;A;A","ABC;D;A;A;B","ABC;D;A;A;C","ABC;D;A;A;D",
            //                                        "ABC;A;B;B;A","ABC;A;B;B;B",
            //                                    };

            //List<string> abc = new List<string>() {
            //                                        "ABC",
            //                                        "ABC;A", "ABC;B",
            //                                        "ABC;A;A","ABC;A;B",
            //                                        "ABC;A;A;A","ABC;A;A;B"
            //                                    };

            //List<string> abc_light = new List<string>() {
            //                                        "ABC;B",
            //                                        "ABC;A;B",
            //                                        "ABC;A;A;A","ABC;A;A;B",
            //                                        "ABC;C;A","ABC;C;B","ABC;C;C",
            //                                        "ABC;D;A","ABC;D;B","ABC;D;C","ABC;D;D"
            //                                    };
            #endregion

            ABC = new RingButtons(abc, new string[1] { ";" });
            grd.Children.Clear();
            Viewbox v = new Viewbox();
            Canvas c = ABC.CreateRingButtons(ME2, ML2, MD2);
            c.Width = 1000;
            c.Height = 1000;
            c.HorizontalAlignment = HorizontalAlignment.Left;
            c.VerticalAlignment = VerticalAlignment.Top;
            v.Child = c;
            v.HorizontalAlignment = HorizontalAlignment.Left;
            v.VerticalAlignment = VerticalAlignment.Top;
            //v.Stretch = Stretch.UniformToFill;
            grd.Children.Add(v);
        }
        #endregion
    }
}