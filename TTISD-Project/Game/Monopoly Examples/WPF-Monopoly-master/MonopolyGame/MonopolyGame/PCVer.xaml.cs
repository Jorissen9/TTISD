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
using MonopolyGame.source;

namespace MonopolyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PCVer : Window
    {
        ViewMono view = ViewMono.Ins;
        public PCVer()
        {
            InitializeComponent();
            this.DataContext = view;
            comboBox.SelectedIndex = 0;
           this.myGrid.ItemsSource = view.history;
         
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            view.GameStart(comboBox.SelectionBoxItem.ToString());
            this.myGrid.Items.Refresh();
        }

        private void YesB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickyes();
            this.myGrid.Items.Refresh();
        }

        private void NoB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickno();
            this.myGrid.Items.Refresh();
        }

        private void RollB_Click(object sender, RoutedEventArgs e)
        {
            view.NextTurn();
        }

        private void ConfirmB_Click(object sender, RoutedEventArgs e)  //useful for expansion
        {

        }

        private void ReB_Click(object sender, RoutedEventArgs e)
        {
            view.Restart();
        }
    }

    

}
