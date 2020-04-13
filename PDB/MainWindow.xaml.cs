using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FProgram;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace PDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ImageSource> bgs = new List<ImageSource>();
        private Program hash;
        Thread SecThr;

        public MainWindow()
        {            
            InitializeComponent();
            
            Check_BGs();
            BG.Source = bgs[0];
            ///Сделать добавление в обои всех фоток
            try
            {
                var bi = new BitmapImage(new Uri(Environment.CurrentDirectory + "/13.jpeg"));
                Resources.Add("13", bi);
                
            } catch
            {

            }
            if (this.TryFindResource("13") != null)
            bgs.Add((BitmapImage)this.TryFindResource("13"));
            
            ///
            hash = new Program();
            UpdateProgBar(0);
        }

        private void Check_BGs ()
        {
            int count = 12;
            BGs_Container.Resources.BeginInit();
            for (int i = 1;i<=count;i++)
                try
                {
                    var bb = new BitmapImage(new Uri("Res/BGs/" + i + ".jpg", UriKind.Relative));
                    bgs.Add(bb);
                    var dd = new System.Windows.Controls.Image();
                    dd.Source = bb;
                    dd.Stretch = Stretch.Uniform;
                    dd.Height = 100;
                    Effect eff = new DropShadowEffect();
                    dd.Opacity = 60;
                    //BGs_Container.ScrollOwner.Content = dd;
                    BGs_Container.Children.Add(dd);
                    dd.MouseDown += Dd_MouseDown;
                } catch (Exception e)
                {
                    Console.Write(e.Message);
                    throw new Exception();
                }
            BGs_Container.Resources.EndInit();
            
        }

        private void Dd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BG.Source = (sender as System.Windows.Controls.Image).Source;
        }

        private void Next_Button(object sender, RoutedEventArgs e)
        {
            //Debug//MessageBox.Show(bgs.IndexOf(BG.Source).ToString());
            if (bgs.IndexOf(BG.Source) + 1 < bgs.Count)
                BG.Source = bgs[bgs.IndexOf(BG.Source) + 1];
            else BG.Source = bgs[0];
        }

        private void Prew_Button(object sender, RoutedEventArgs e)
        {
            if (bgs.IndexOf(BG.Source) - 1 >= 0)
                BG.Source = bgs[bgs.IndexOf(BG.Source) - 1];
            else BG.Source = bgs.Last();
        }
        
        private void Check_All_Files(object sender, RoutedEventArgs e)
        {
            SecThr = new Thread(new ThreadStart(CAF));
            SecThr.Start();
            Check_All_Files_Button.IsEnabled = false;
            OpenDB_Button.IsEnabled = false;
        }

        private void CAF()
        {
            float koef = 0.00F;
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                koef = (float)koeff.Value+80;
            }));
            MemoryStream lastStream = null;
            var arr = hash.GenerateFileArray("");
            
            for (int i = 0; i < arr.Length; i++)
            {           
                
                byte[] byteArr = File.ReadAllBytes(arr[i]);                
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    BitmapImage ty = new BitmapImage();
                    var stream = new MemoryStream(byteArr);
                    {                        
                        var tmp_img = stream;
                        ty.BeginInit();
                        ty.StreamSource = tmp_img;
                        ty.EndInit();
                    }
                    if (lastStream != null)
                    {
                        PrewImg.Source = null;
                        lastStream.Dispose();
                    }
                    PrewImg.Source = ty;
                }));

                hash.CheckFile(arr[i],true, koef);/// 

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {                    
                    Progr_Bar.Value = ((double)(i + 1) * 10000 / arr.Length);
                }));

                Console.Title = ((i + 1) * 100 / arr.Length).ToString() + "%";
                
            }
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                
                Check_All_Files_Button.IsEnabled = true;
                OpenDB_Button.IsEnabled = true;
                Progr_Bar.Value = 0;
                PrewImg.Source = null;
            }));
            hash.SaveChanges();
            SecThr.Abort();            
        }

        public void UpdateProgBar(double d)
        {
            Progr_Bar.Value = d;
        }

        private void OpenDB_Button_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text =="")
            {
                NameBox.Text = "PDB";
            }
            hash.ReloadDBFile(NameBox.Text);
        }

        private void OpenGalery_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenCloseGalAnimation();
        }

        private void OpenCloseGalAnimation()
        {
            ThicknessAnimation Slideanim = new ThicknessAnimation();
            if (OpenGallery_Button.FlowDirection == FlowDirection.RightToLeft)
            {
                Slideanim.From = GalleryComplex_Container.Margin;
                Slideanim.To = new Thickness(0, GalleryComplex_Container.Margin.Top, -126, GalleryComplex_Container.Margin.Bottom);
                Slideanim.Duration = TimeSpan.FromSeconds(0.5);
                Slideanim.AccelerationRatio = 0.7;
                OpenGallery_Button.FlowDirection = FlowDirection.LeftToRight;
            }
            else
            {                
                Slideanim.From = GalleryComplex_Container.Margin;
                Slideanim.To = new Thickness(0, GalleryComplex_Container.Margin.Top, 0, GalleryComplex_Container.Margin.Bottom);
                Slideanim.Duration = TimeSpan.FromSeconds(0.5);
                Slideanim.AccelerationRatio = 0.7;
                OpenGallery_Button.FlowDirection = FlowDirection.RightToLeft;
            }
            GalleryComplex_Container.BeginAnimation(Button.MarginProperty, Slideanim);
        }
    }
}
