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
using System.Linq;

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

        List<Brush> colors = new List<Brush>() {Brushes.Red, Brushes.Green, Brushes.Yellow, Brushes.Magenta, Brushes.Blue,
                                                Brushes.Cyan, Brushes.Orange, Brushes.Brown, Brushes.Khaki, Brushes.Salmon };

        Dictionary<string, bouton> dico;
        Dictionary<int, Standard_UC_JJO.Slider_INT_JJO> sliders;

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
                    for (int i = sliders.Count; i < value + 1; i++)
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
                        sliders[clef]._ValueChanged -= Rings_ButtonsNumberChanged;
                        sliders.Remove(clef);
                    }
                }

                _intvalue = value;
                OnPropertyChanged("intvalue");
            }
        }
        int _intvalue;

        private void Rings_ButtonsNumberChanged(object sender, EventArgs e)
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

            //Dictionary<int, List<List<int>>> operations = Operation.OperationsGenerator(10, 20, 2);
            int mode = 1;

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
                                                            "ABC,A,A,A","ABC,A,A,B",
                                                            "ABC,C,A","ABC,C,B","ABC,C,C",
                                                            "ABC,D,A","ABC,D,B","ABC,D,C","ABC,D,D"
                                                        };

                    RingButtons ABC = new RingButtons(abc);
                    CreateRingButtons(ABC);

                    break;
            }
        }

        void CreateRingButtons(RingButtons rb)
        {

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

        private void CreateButtons_FromSliders()
        {
            grd.Children.Clear();
            dico = new Dictionary<string, bouton>();
            int marge = 0;
            float r_max = 300;
            float epaisseur = r_max / sliders.Count;
            float r_1 = r_max * 1 / 6;
            float r_2 = r_max * 1 / 3;

            Canvas c = new Canvas();
            //c.Children.Add(DrawSecteurs(0, r_1, r_max, r_1, 3, marge));
            //c.Children.Add(DrawSecteurs(1, r_2 + r_1, r_max, r_2, 9, marge));
            //c.Children.Add(DrawSecteurs(2, r_max, r_max, r_max - r_2 - r_1, 18, marge));

            for (int i = 0; i < sliders.Count; i++)
            {
                c.Children.Add(DrawSecteurs(i, r_max * (i + 1) / sliders.Count, r_max, epaisseur, (int)sliders[i]._sld.Value, marge));
            }
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
                b.couleurfonce = new SolidColorBrush(Color.FromRgb(ChangeIntensity(color.R, fctr),
                                                                    ChangeIntensity(color.G, fctr),
                                                                    ChangeIntensity(color.B, fctr)));
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

        byte ChangeIntensity(byte val, float factor)
        {
            int v = (int)val;
            if (factor > 1)
                v += 50;
            else if (factor < 1)
                v -= 50;

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
            Path cercle_ext = new Path();
            Path cercle_int = new Path();
            cercle_ext.Data = new EllipseGeometry(centre, r_ext, r_ext);
            cercle_int.Data = new EllipseGeometry(centre, r_int, r_int);
            CombinedGeometry anneau = new CombinedGeometry(GeometryCombineMode.Exclude, cercle_ext.Data, cercle_int.Data);

            //triangle
            float d = (float)(marge / Math.Cos(angle_ouverture_deg / 2 * Math.PI / 180));
            float X = r_ext - (float)(r_ext * Math.Tan(angle_ouverture_deg / 2 * Math.PI / 180));
            float X_ = r_ext + (float)(r_ext * Math.Tan(angle_ouverture_deg / 2 * Math.PI / 180));

            //Point c_ = new Point(r_ext, r_ext - d);
            //LineSegment x = new LineSegment(new Point(X, r_ext - r_extMAX - d), true);
            //LineSegment x_ = new LineSegment(new Point(X_, r_ext - r_extMAX - d), true);
            Point c_ = new Point(r_ext, r_ext - d);
            LineSegment x = new LineSegment(new Point(X, -d), true);
            LineSegment x_ = new LineSegment(new Point(X_, -d), true);

            LineSegment[] s = new LineSegment[] { x, x_ };
            PathFigure t = new PathFigure(c_, s, true);
            PathGeometry triangle = new PathGeometry(new PathFigure[] { t });

            //secteur
            CombinedGeometry c = new CombinedGeometry(GeometryCombineMode.Intersect, anneau, triangle);

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

        private void ME(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " ENTER";
            p.Fill = dico[p.Name].couleurfonce;
        }

        private void MD(object sender, MouseButtonEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " DOWN";
            p.Fill = dico[p.Name].couleur;
        }

        private void ML(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " LEAVE";
            p.Fill = dico[p.Name].couleur;
        }

    }
}
