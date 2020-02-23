using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FProgram;
using System.Threading;
using System.Net.Cache;

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
            for (int i = 1;i<=count;i++)
                try
                {
                    bgs.Add(new BitmapImage(new Uri("Res/BGs/"+i+".jpg",UriKind.Relative)));                
                } catch (Exception e)
                {
                    Console.Write(e.Message);
                    throw new Exception();
                }
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

                hash.CheckFile(arr[i],true);/// 

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
    }
}
