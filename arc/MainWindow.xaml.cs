using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //http://csharphelper.com/blog/2019/05/make-an-intuitive-extension-method-to-draw-an-elliptical-arc-in-wpf-and-c/
        //http://csharphelper.com/blog/2014/12/find-the-area-where-circles-overlap-in-c/

        public MainWindow()
        {
            InitializeComponent();
            DrawSecteurs();
        }

        void DrawSecteurs()
        {
            //grd.Children.Clear();

            float r_ext = 50;
            float r_int = 25;
            float marge = 5;

            int nbrboutons = 7;

            float angle_ouverture_deg = 360 / nbrboutons;
            float angle_position_deg_init = 0;

            Point c = new Point(50, 50);

            for (int i = 0; i < nbrboutons; i++)
            {
                float angle_position_deg = angle_position_deg_init + i * angle_ouverture_deg;

                grd.Children.Add(DrawSecteur("btn_" + i, c, r_ext, r_int, marge, angle_ouverture_deg, angle_position_deg));
            }
        }

        Path DrawSecteur(string name, Point centre, float r_ext, float r_int, float marge, float angle_ouverture_deg, float angle_position_deg)
        {
            //disques
            Path c1 = new Path();
            Path c2 = new Path();
            c1.Data = new EllipseGeometry(centre, r_ext, r_ext);
            c2.Data = new EllipseGeometry(centre, r_int, r_int);
            CombinedGeometry cg1 = new CombinedGeometry(GeometryCombineMode.Exclude, c1.Data, c2.Data);

            //triangle
            Point c_ = new Point(50, 45);
            LineSegment x = new LineSegment(new Point(30, 0), true);
            LineSegment x_ = new LineSegment(new Point(70, 0), true);
            LineSegment[] s = new LineSegment[] { x, x_ };
            PathFigure t = new PathFigure(c_, s, true);
            PathGeometry pg = new PathGeometry(new PathFigure[] { t });

            //secteur
            CombinedGeometry c = new CombinedGeometry(GeometryCombineMode.Intersect, cg1, pg);
            Path p = new Path();
            p.RenderTransform = new RotateTransform(angle_position_deg, centre.X, centre.Y);
            p.Data = c;
            p.Name = name;
            p.MouseDown += MD;
            p.MouseEnter += ME;
            p.MouseLeave += ML;
            p.Fill = Brushes.Red;
            //p.Stroke = Brushes.Black;
            //p.StrokeThickness = 1;
            p.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            return p;
        }

        void DrawArc()
        {
            Canvas cnv = new Canvas();
            Path pth = new Path();
            pth.Fill = Brushes.BlueViolet;
            pth.Stroke = Brushes.OrangeRed;

            PathGeometry pg = new PathGeometry();
            PathFigureCollection pfc = new PathFigureCollection();

            PathFigure pf = new PathFigure();

            ArcSegment a = new ArcSegment(new Point(200, 100), new Size(300, 300), 45, true, SweepDirection.Clockwise, true);

            cnv.Children.Add(pth);
            pth.Data = pg;
            pfc.Add(pf);
            pg.Figures = pfc;
            pf.Segments.Add(a);
            grd.Children.Insert(0, cnv);
        }

        void DrawArc2()
        {
            Arc _arc = new Arc();
            _arc.StartAngle = 0;
            _arc.EndAngle = 45;

            grd.Children.Insert(0, _arc);
        }

        private void ME(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " ENTER";
        }

        private void MD(object sender, MouseButtonEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " DOWN";
        }

        private void ML(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            Title = p.Name + " LEAVE";
        }
    }
}
