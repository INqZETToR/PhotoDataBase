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

namespace PDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ImageSource> bgs = new List<ImageSource>();

        public MainWindow()
        {
            //Properties.Resources.ResourceManager;
            
            InitializeComponent();
            Check_BGs();
            BG.Source = bgs[0];
        }

        private void Check_BGs ()
        {
            int count = 12;
            for (int i = 1;i<=count;i++)
            bgs.Add(new BitmapImage(new Uri("Res/BGs/"+i+".jpg",UriKind.Relative)));
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


    }
}
