using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace arc
{
    public class RingButtons
    {
        public Dictionary<int, Ring> anneaux;
        public Dictionary<string, Element> elements;
        internal Dictionary<string, Button> boutons;
        public System.Windows.Point centre;
        List<SolidColorBrush> colors;
        public RingButtons(List<string> datas, string[] charSep)
        {
            float epaisseur = 100;
            float angle_origine_deg = 0;

            #region create Elements
            elements = new Dictionary<string, Element>();
            for (int i = 0; i < datas.Count; i++)
            {
                Element element = new Element(datas[i], charSep, this);
                elements.Add(element.name, element);
            }

            //quand on est parent chercher ses enfants
            foreach (Element parent in elements.Values)
                foreach (Element enfant in elements.Values)
                    if (enfant.parent?.name == parent.name)
                    {
                        enfant.ordre = parent.children.Values.Count;
                        parent.children.Add(enfant.ordre, enfant);
                    }
            #endregion

            #region create Rings
            anneaux = new Dictionary<int, Ring>();
            foreach (Element ele in elements.Values)
            {
                if (!anneaux.ContainsKey(ele.anneau_index))
                    anneaux.Add(ele.anneau_index, new Ring()
                    {
                        index = ele.anneau_index,
                        rayon_interne = ele.anneau_index * epaisseur,
                        rayon_externe = (ele.anneau_index + 1) * epaisseur,
                        angle_origine = angle_origine_deg,
                        ringButtons = this
                    });
                anneaux[ele.anneau_index].elements.Add(ele);
                ele.anneau = anneaux[ele.anneau_index];
            }
            #endregion

            #region define Center
            float r = anneaux.Last().Value.rayon_externe;
            centre = new System.Windows.Point(r, r);
            #endregion
        }

        public Canvas CreateRingButtons(MouseEventHandler MouseEnter,
                                        MouseEventHandler MouseLeave,
                                        MouseButtonEventHandler MouseDown)
        {
            colors = new List<SolidColorBrush>() {Brushes.Red, Brushes.Green, Brushes.Yellow, Brushes.Magenta, Brushes.Blue,
                                                Brushes.Cyan, Brushes.Orange, Brushes.Brown, Brushes.Khaki, Brushes.Salmon };
            ColorsSetAlpha(0.5f);

            boutons = new Dictionary<string, Button>();

            Canvas c = new Canvas();

            foreach (Ring anneau in anneaux.Values)
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

            foreach (Ring anneau in anneaux.Values)
                c.Children.Add(DrawSecteurs(anneau, MouseEnter, MouseLeave, MouseDown));

            return c;
        }

        Viewbox DrawSecteurs(Ring anneau, MouseEventHandler MouseEnter,
                                        MouseEventHandler MouseLeave,
                                        MouseButtonEventHandler MouseDown)
        {

            Grid g = new Grid();
            g.Width = anneau.ringButtons.centre.X * 2;
            g.Height = anneau.ringButtons.centre.X * 2;

            int i = 0;
            foreach (Element element in anneau.elements)
            {
                Path p = DrawSecteur(element, MouseEnter, MouseLeave, MouseDown);
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
                boutons.Add(element.button.name, element.button);
                g.Children.Add(p);
                i++;

                Console.WriteLine(element);
            }

            Viewbox v = new Viewbox();
            v.StretchDirection = StretchDirection.Both;
            v.Stretch = Stretch.UniformToFill;
            v.Width = g.Width;
            v.Height = g.Height;
            v.Child = g;
            //v.Margin = new Thickness((r_extMAX - r_ext) / 2);
            return v;
        }

        //text inside :
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/27c9eb93-8a32-40d8-b0dd-b441a2496907/how-to-make-textblocks-appear-inside-an-ellipse-area?forum=wpf

        Path DrawSecteur(Element element, MouseEventHandler MouseEnter,
                                        MouseEventHandler MouseLeave,
                                        MouseButtonEventHandler MouseDown)
        {
            // rayons
            float r_ext = element.anneau.rayon_externe;
            float r_int = element.anneau.rayon_interne;
            Point centre = element.ringButtons.centre;

            // dessins====================
            CombinedGeometry c;
            Path p = new Path();
            // cas origine : doit être le Premier cas !!
            if (element.parent == null || element.parent.children.Count == 1)
                c = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                           new EllipseGeometry(centre, r_ext, r_ext),
                                                           new EllipseGeometry(centre, r_int, r_int));
            else
            {
                CombinedGeometry anneau = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                               new EllipseGeometry(centre, r_ext, r_ext),
                                                               new EllipseGeometry(centre, r_int, r_int));
                
                // triangle - centré !!!!
                float marge = 0;
                //float d = (float)(marge / Math.Cos(element.angle_ouverture_deg / 2 * Math.PI / 180));
                float X = (float)(centre.X - centre.X * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));
                float X_ = (float)(centre.X + centre.X * Math.Tan(element.angle_ouverture_deg / 2 * Math.PI / 180));

                if (X < -100000) X = -100000;
                if (X_ > 100000) X_ = 100000;

                LineSegment A = new LineSegment(new Point(X, 0), true);
                LineSegment B = new LineSegment(new Point(X_, 0), true);

                LineSegment[] s = new LineSegment[] { A, B };
                PathFigure t = new PathFigure(centre, s, true);
                PathGeometry triangle = new PathGeometry(new PathFigure[] { t });

                // Secteur
                c = new CombinedGeometry(GeometryCombineMode.Intersect, anneau, triangle);
                p.RenderTransform = new RotateTransform(element.angle_ouverture_deb + element.angle_ouverture_deg / 2, centre.X, centre.Y);
            }

            p.Data = c;
            p.MouseDown += MouseDown;
            p.MouseEnter += MouseEnter;
            p.MouseLeave += MouseLeave;
            //p.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
            p.Stroke = Brushes.Black;
            p.StrokeThickness = 2;
            return p;
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

        byte ChangeIntensity(byte val, float factor, byte offset)
        {
            int v = (int)val;

            if (factor > 1) v += offset;
            else if (factor < 1) v -= offset;

            v = (int)(v * factor);

            if (v > 255) v = 255;
            if (v < 0) v = 0;

            return (byte)v;
        }
    }
}