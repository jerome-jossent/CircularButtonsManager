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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        //http://csharphelper.com/blog/2019/05/make-an-intuitive-extension-method-to-draw-an-elliptical-arc-in-wpf-and-c/
        //http://csharphelper.com/blog/2014/12/find-the-area-where-circles-overlap-in-c/

        class bouton
        {
            public int ring_index;
            public int btn_index;
            public string name;
            public Brush couleur;
            public Brush couleurfonce;
        }

        List<SolidColorBrush> colors;

        Dictionary<string, bouton> dico;
        Dictionary<int, Standard_UC_JJO.Slider_INT_JJO> sliders;
        RingButtons ABC;

        bool pause;

        public int intvalue
        {
            get { return _intvalue; }
            set
            {
                if (value == _intvalue) return;

                if (sliders == null)
                {
                    sliders = new Dictionary<int, Standard_UC_JJO.Slider_INT_JJO>();
                    grd.Children.Clear();
                }

                if (value > _intvalue)
                {
                    //add
                    int val_init = sliders.Count;
                    for (int i = val_init; i < value; i++)
                    {
                        Standard_UC_JJO.Slider_INT_JJO slider = new Standard_UC_JJO.Slider_INT_JJO();
                        slider._label_title = $"Anneau {i} : nombre de boutons";
                        slider._index = i;
                        slider._value_min = 1;
                        slider._value_max = 20;
                        slider._ValueChanged += Rings_ButtonsNumberChanged;
                        sliders.Add(i, slider);
                        sp_rings_sliders.Children.Add(slider);
                    }
                }
                else
                {
                    while (sliders.Count > value)
                    {
                        int clef = sliders.Last().Key;
                        Standard_UC_JJO.Slider_INT_JJO slider = sliders[clef];
                        slider._ValueChanged -= Rings_ButtonsNumberChanged;
                        sp_rings_sliders.Children.Remove(slider);
                        sliders.Remove(clef);
                    }
                }

                _intvalue = value;
                OnPropertyChanged("intvalue");
            }
        }
        int _intvalue;

        void Rings_ButtonsNumberChanged(object sender, EventArgs e)
        {
            Slider s = (Slider)sender;
            Grid g = (Grid)(s.Parent);
            Standard_UC_JJO.Slider_INT_JJO slider = (Standard_UC_JJO.Slider_INT_JJO)(g.Parent);
            CreateButtons_FromSliders();
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            GO();
        }

        void GO()
        {
            int mode = 2;
            switch (mode)
            {
                case 1:
                    CreateButtons();
                    break;

                case 2:
                    List<string> abc = new List<string>() {
                                                            "ABC",
                                                            "ABC,A", "ABC,B", "ABC,C", "ABC,D",
                                                            "ABC,A,A","ABC,A,B",
                                                            "ABC,A,B,A","ABC,A,B,B",
                                                            "ABC,A,A,A","ABC,A,A,B",
                                                            "ABC,C,A","ABC,C,B","ABC,C,C",
                                                            "ABC,D,A","ABC,D,B","ABC,D,C","ABC,D,D",
                                                            "ABC,D,A,A","ABC,D,A,B","ABC,D,A,C","ABC,D,A,D",
                                                            "ABC,D,A,A,A","ABC,D,A,A,B","ABC,D,A,A,C","ABC,D,A,A,D",
                                                            "ABC,A,B,B,A","ABC,A,B,B,B",
                                                        };


                    //List<string> abc = new List<string>() {
                    //                                        "ABC",
                    //                                        "ABC,A"//, "ABC,B",
                    //                                        //"ABC,A,A","ABC,A,B",
                    //                                        //"ABC,A,A,A","ABC,A,A,B"
                    //                                    };

                    //List<string> abc_light = new List<string>() {
                    //                                        "ABC,B",
                    //                                        "ABC,A,B",
                    //                                        "ABC,A,A,A","ABC,A,A,B",
                    //                                        "ABC,C,A","ABC,C,B","ABC,C,C",
                    //                                        "ABC,D,A","ABC,D,B","ABC,D,C","ABC,D,D"
                    //                                    };

                    ABC = new RingButtons(abc, new string[1] { "," });

                    CreateRingButtons(ABC);

                    break;
                case 3:
                    //Dictionary<int, List<List<int>>> operations = Operation.OperationsGenerator(10, 20, 2);
                    break;
            }
        }

        void CreateRingButtons(RingButtons rb)
        {
            colors = new List<SolidColorBrush>() {Brushes.Red, Brushes.Green, Brushes.Yellow, Brushes.Magenta, Brushes.Blue,
                                                Brushes.Cyan, Brushes.Orange, Brushes.Brown, Brushes.Khaki, Brushes.Salmon };
            ColorsSetAlpha(0.5f);

            rb.boutons = new Dictionary<string, Button>();

            dico = new Dictionary<string, bouton>();
            Canvas c = new Canvas();

            foreach (Ring anneau in rb.anneaux.Values)
                foreach (Element element in anneau.elements)
                {
                    Element eleParent = element.parent;

                    // angles, dépendent de :
                    // - parent : quels sont les angles du parent
                    // - combien d'enfant (dont cet élément) a ce parent ?
                    if (element.isOrigin)
                    {
                        //pas de découpage :                //je suis au centre et j'utilise toute la place 0->359,99°
                        element.angle_ouverture_deb = 0;
                        element.angle_ouverture_fin = 360;
                        element.angle_ouverture_deg = 360;
                    }
                    else
                    {
                        eleParent.angle_ouverture_deg = eleParent.angle_ouverture_fin - eleParent.angle_ouverture_deb;

                        element.angle_ouverture_deg = eleParent.angle_ouverture_deg / eleParent.children.Count;

                        element.angle_ouverture_deb = eleParent.angle_ouverture_deb + element.angle_ouverture_deg * element.ordre;
                        element.angle_ouverture_fin = element.angle_ouverture_deb + element.angle_ouverture_deg;
                    }
                }

            foreach (Ring anneau in rb.anneaux.Values)
                c.Children.Add(DrawSecteurs(anneau));

            //List<Viewbox> vbxs = new List<Viewbox>();
            //foreach (Ring anneau in rb.anneaux.Values)
            //    vbxs.Add(DrawSecteurs(anneau));
            //for (int i = vbxs.Count-1; i >= 0; i--)           
            //    c.Children.Add(vbxs[i]);

            grd.Children.Clear();
            grd.Children.Add(c);

            //ListBox lb = new ListBox();
            //foreach (Ring anneau in rb.anneaux.Values)
            //{
            //    ListBoxItem lbi = new ListBoxItem();
            //    lbi.Content = DrawSecteurs(anneau);
            //    lb.Items.Add(lbi);
            //}

            //grd.Children.Clear();
            //grd.Children.Add(lb);
        }

        void ColorsSetAlpha(double alpha0a1)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                colors[i] = new SolidColorBrush(Color.FromArgb((byte)(255 * alpha0a1),
                                                                colors[i].Color.R,
                                                                colors[i].Color.G,
                                                                colors[i].Color.B));
            }
        }

        Viewbox DrawSecteurs(Ring anneau)
        {
            //Point c = new Point(anneau.rayon_externe, anneau.rayon_externe);
            Grid g = new Grid();
            g.Width = anneau.ringButtons.anneaux.Last().Value.rayon_externe; // anneau.rayon_externe * 2;
            g.Height = anneau.ringButtons.anneaux.Last().Value.rayon_externe; //anneau.rayon_externe * 2;
            //g.Width = anneau.rayon_externe * 2;
            //g.Height = anneau.rayon_externe * 2;

            int i = 0;
            foreach (Element element in anneau.elements)
            {
                Path p = DrawSecteur(element);
                p.Name = "ring_" + anneau.index + "_btn_" + i;

                int j = i;
                while (j >= colors.Count)
                    j -= colors.Count;
                Brush couleur = colors[j];
                p.Fill = couleur;

                //initialisation bouton
                element.button = new Button();
                element.button.ring = element.anneau;
                element.button.element = element;
                element.button.couleur = couleur;
                Color color = ((SolidColorBrush)couleur).Color;
                float fctr = 0.8f;
                byte offest = 50;
                element.button.couleurfonce = new SolidColorBrush(Color.FromRgb(ChangeIntensity(color.R, fctr, offest),
                                                                    ChangeIntensity(color.G, fctr, offest),
                                                                    ChangeIntensity(color.B, fctr, offest)));
                element.button.btn_index = i;
                element.button.name = p.Name;
                ABC.boutons.Add(element.button.name, element.button);
                g.Children.Add(p);
                i++;

                Console.WriteLine(element);
            }

            Viewbox v = new Viewbox();
            v.StretchDirection = StretchDirection.Both;
            v.Stretch = Stretch.UniformToFill;
            //v.Width = anneau.rayon_externe;
            //v.Height = anneau.rayon_externe;
            v.Width = g.Width;
            v.Height = g.Height;
            v.Child = g;
            //v.Margin = new Thickness((r_extMAX - r_ext) / 2);
            return v;
        }

        //text inside :
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/27c9eb93-8a32-40d8-b0dd-b441a2496907/how-to-make-textblocks-appear-inside-an-ellipse-area?forum=wpf

        Path DrawSecteur(Element element)
        {
            // rayons
            float r_ext = element.anneau.rayon_externe;
            float r_int = element.anneau.rayon_interne;
            Point centre = element.ringButtons.centre;

            //dessins====================
            CombinedGeometry c;
            Path p = new Path();
            //cas origine : doit être le Premier cas !!
            if (element.parent == null || element.parent.children.Count == 1)
                c = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                           new EllipseGeometry(centre, r_ext, r_ext),
                                                           new EllipseGeometry(centre, r_int, r_int));
            else
            {
                CombinedGeometry anneau = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                               new EllipseGeometry(centre, r_ext, r_ext),
                                                               new EllipseGeometry(centre, r_int, r_int));
                float marge = 0;

                //triangle - centré !!!!
                float d = (float)(marge / Math.Cos(element.angle_ouverture_deg / 2 * Math.PI / 180));
                float X = (float)(centre.X - centre.X * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));
                float X_ = (float)(centre.X + centre.X * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));
                //float X = r_ext - (float)(r_ext * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));
                //float X_ = r_ext + (float)(r_ext * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));

                if (X < -100000) X = -100000;
                if (X_ > 100000) X_ = 100000;

                //Point c_ = new Point(r_ext, r_ext - d);
                LineSegment x = new LineSegment(new Point(X, -d), true);
                LineSegment x_ = new LineSegment(new Point(X_, -d), true);

                LineSegment[] s = new LineSegment[] { x, x_ };
                //PathFigure t = new PathFigure(c_, s, true);
                PathFigure t = new PathFigure(centre, s, true);
                PathGeometry triangle = new PathGeometry(new PathFigure[] { t });
                //secteur
                c = new CombinedGeometry(GeometryCombineMode.Intersect, anneau, triangle);
                p.RenderTransform = new RotateTransform(element.angle_ouverture_deb + element.angle_ouverture_deg / 2, centre.X, centre.Y);
            }

            p.Data = c;
            p.MouseDown += MD2;
            p.MouseEnter += ME2;
            p.MouseLeave += ML2;
            //p.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
            p.Stroke = Brushes.Black;
            p.StrokeThickness = 2;
            return p;
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


        void CreateButtons()
        {
            dico = new Dictionary<string, bouton>();

            int marge = 0;
            float r_max = 300;
            float r_1 = r_max * 1 / 6;
            float r_2 = r_max * 1 / 3;

            Canvas c = new Canvas();
            c.Children.Add(DrawSecteurs(0, r_1, r_max, r_1, 3, marge));
            c.Children.Add(DrawSecteurs(1, r_2 + r_1, r_max, r_2, 9, marge));
            c.Children.Add(DrawSecteurs(2, r_max, r_max, r_max - r_2 - r_1, 18, marge));
            grd.Children.Add(c);
        }

        void CreateButtons_FromSliders()
        {
            dico = new Dictionary<string, bouton>();
            int marge = 0;
            float r_max = 300;
            float epaisseur = r_max / sliders.Count;
            float r_1 = r_max * 1 / 6;
            float r_2 = r_max * 1 / 3;

            Canvas c = new Canvas();

            for (int i = 0; i < sliders.Count; i++)
                c.Children.Add(DrawSecteurs(i, r_max * (i + 1) / sliders.Count, r_max, epaisseur, (int)sliders[i]._sld.Value, marge));

            grd.Children.Clear();
            grd.Children.Add(c);
        }

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
                bouton b = new bouton();
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
                b.ring_index = ring_index;
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

    }
}