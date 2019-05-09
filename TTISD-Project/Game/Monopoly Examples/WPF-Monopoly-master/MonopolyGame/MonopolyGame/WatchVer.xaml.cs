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
using System.Windows.Shapes;
using MonopolyGame.source;
namespace MonopolyGame
{
    /// <summary>
    /// Interaction logic for WatchVer.xaml
    /// </summary>
    public partial class WatchVer : Window
    {
        ViewMono view = ViewMono.Ins;
        public WatchVer()
        {
            InitializeComponent();
            this.DataContext = view;
            comboBox.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            StatDisplay.Visibility = Visibility.Hidden;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string x = comboBox.SelectionBoxItem.ToString();
            view.GameStart(x);
            if (x == "2") { P3.Visibility = Visibility.Collapsed; P4.Visibility = Visibility.Collapsed; }
            else if (x == "3") { P4.Visibility = Visibility.Collapsed; }
           
            
        }

        private void YesB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickyes();
        }

        private void NoB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickno();
        }

        private void RollB_Click(object sender, RoutedEventArgs e)
        {
            view.NextTurn();
        }

        private void ReB_Click(object sender, RoutedEventArgs e)
        {
            view.Restart();
        }

        private void StatB_Click(object sender, RoutedEventArgs e)
        {
            StatDisplay.Visibility = Visibility.Visible;
            StatB.IsEnabled = false;

        }

        private void CloseB_Click(object sender, RoutedEventArgs e)
        {
            StatDisplay.Visibility = Visibility.Hidden;
            StatB.IsEnabled = true;
        }

        private void showP(int i)
        {
           

            Binding BindC = new Binding("Pcash[" + i + "]");
            BindC.Source = view;
            PcashT.SetBinding(TextBlock.TextProperty, BindC);

            Binding BindW = new Binding("Pwealth[" + i + "]");
            BindW.Source = view;
            PwealthT.SetBinding(TextBlock.TextProperty, BindW);

            Binding BindP = new Binding("Pprop[" + i + "]");
            BindP.Source = view;
            PpropT.SetBinding(TextBlock.TextProperty, BindP);



            //PcashT.Text = view.Pcash[i].ToString();
            //PwealthT.Text = view.Pwealth[i].ToString();
            //PpropT.Text = view.Pprop[i];
        
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showP(comboBox1.SelectedIndex);
        }
    }

    



}
