using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMGU.CV
{
    public partial class Proposition_UC : UserControl
    {
        Proposition p;

        public Proposition_UC()
        {
            InitializeComponent();
        }

        public void LINK(Proposition prop)
        {
            p = prop;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            icone_del.Visibility = Visibility.Visible;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            icone_del.Visibility = Visibility.Hidden;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                p.del();
        }
    }
}