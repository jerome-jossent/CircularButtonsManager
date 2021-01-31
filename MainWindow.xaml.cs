using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace EMGU.CV
{
    public partial class MainWindow : Window
    {
        string nomfichier;
        Image<Gray, UInt16> image_variable;
        bool jeveuxdu16bits = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nomfichier = ofd.FileName;
                Chargement(nomfichier);
            }
        }

        private void Chargement(string nomfichier)
        {
            Title = nomfichier;

            if (jeveuxdu16bits)
            {
                Mat mat = CvInvoke.Imread(nomfichier, Emgu.CV.CvEnum.LoadImageType.AnyDepth);
                image_variable = mat.ToImage<Gray, UInt16>();
            }
            else
                image_variable = new Image<Gray, UInt16>(nomfichier);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            image_variable.Save("D:\\C#\\test0.tiff"); //                            sauve du 16 bits
            image_variable.Mat.Save("D:\\C#\\test1.tiff"); //                        sauve du 16 bits
            image_variable.ToUMat().Save("D:\\C#\\test2.tiff"); //                   sauve du 16 bits
            image_variable.ToBitmap().Save("D:\\C#\\test4.bmp", ImageFormat.Bmp); // sauve du 8 bits
        }

        private static void SaveBmp(Bitmap bmp, string path)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            System.Windows.Media.PixelFormat pixelFormats = ConvertBmpPixelFormat(bmp.PixelFormat);
            pixelFormats = PixelFormats.Gray16;
            BitmapSource source = BitmapSource.Create(bmp.Width,
                                                      bmp.Height,
                                                      bmp.HorizontalResolution,
                                                      bmp.VerticalResolution,
                                                      pixelFormats,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * bmp.Height,
                                                      bitmapData.Stride);

            bmp.UnlockBits(bitmapData);


            FileStream stream = new FileStream(path, FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);

            stream.Close();
        }

        private static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgr32;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }

            return pixelFormats;
        }
    }
}
