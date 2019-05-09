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
    /// Interaction logic for MobileVer.xaml
    /// </summary>
    public partial class MobileVer : Window
    {
        ViewMono view = ViewMono.Ins;
        public MobileVer()
        {
            InitializeComponent();
            this.DataContext = view;
            comboBox.SelectedIndex = 0;
            Pstat.Visibility = Visibility.Hidden;

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            view.GameStart(comboBox.SelectionBoxItem.ToString());
            movescreen(0);
        }

        private void NoB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickno();
        }

        private void YesB_Click(object sender, RoutedEventArgs e)
        {
            view.userclickyes();
        }

        private void RollD_Click(object sender, RoutedEventArgs e)
        {
            view.NextTurn();
            movescreen(view.curplayposition());
        }

        private void ReB_Click(object sender, RoutedEventArgs e)
        {
            view.Restart();
        }

        private void P1_Click(object sender, RoutedEventArgs e)
        {
            showP(0);
        }

        private void P2_Click(object sender, RoutedEventArgs e)
        {
            showP(1);
        }

        private void P3_Click(object sender, RoutedEventArgs e)
        {
            showP(2);
        }

        private void P4_Click(object sender, RoutedEventArgs e)
        {
            showP(3);
        }

        private void CloseB_Click(object sender, RoutedEventArgs e)
        {
            Pstat.Visibility = Visibility.Hidden;
            P1.IsEnabled = true;
            P2.IsEnabled = true;
            P3.IsEnabled = true;
            P4.IsEnabled = true;
        }

        private void showP(int i) {
            Pname.Text = "Player" + (i + 1);

            Binding BindC = new Binding("Pcash[" + i +"]");
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
            P1.IsEnabled = false;
            P2.IsEnabled = false;
            P3.IsEnabled = false;
            P4.IsEnabled = false;
            Pstat.Visibility = Visibility.Visible;
        }


        public void movescreen(int position)
        {
            if ((0 <= position && position <= 2) || (35 <= position && position <= 39))
            {
                sview.ScrollToHorizontalOffset(0);
                sview.ScrollToVerticalOffset(400);
                
            }
            else if ((3 <= position && position <= 7))
            {
                sview.ScrollToHorizontalOffset(0);
                sview.ScrollToVerticalOffset(140);
      
            }
            else if (8 <= position && position <= 15)
            {
                sview.ScrollToHorizontalOffset(0);
                sview.ScrollToVerticalOffset(0);
      
            }
            else if (16 <= position && position <= 22)
            {
                sview.ScrollToHorizontalOffset(400);
                sview.ScrollToVerticalOffset(0);
     
            }
            else if (23 <= position && position <= 27)
            {
                sview.ScrollToHorizontalOffset(400);
                sview.ScrollToVerticalOffset(140);
      
            }
            else if (28 <= position && position <= 34)
            {
                sview.ScrollToHorizontalOffset(400);
                sview.ScrollToVerticalOffset(400);
           
            }
            
        }




        //reference : https://sachabarbs.wordpress.com/2008/04/10/creating-a-scrollable-control-surface-in-wpf/
        Point scrollStartPoint;
        Point scrollStartOffset;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e){
         if (sview.IsMouseOver)
         {
             // Save starting point, used later when determining 
             //how much to scroll.
             scrollStartPoint = e.GetPosition(this);
             scrollStartOffset.X = sview.HorizontalOffset;
             scrollStartOffset.Y = sview.VerticalOffset;
               
     
            // Update the cursor if can scroll or not.
            this.Cursor = (sview.ExtentWidth > 
                sview.ViewportWidth) ||
                (sview.ExtentHeight > 
                sview.ViewportHeight) ?
                Cursors.ScrollAll : Cursors.Arrow;
     
            this.CaptureMouse();
        }
     
        base.OnPreviewMouseDown(e);
    }

        


    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        if (this.IsMouseCaptured)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);
       
            // Determine the new amount to scroll.
            Point delta = new Point(
                (point.X > this.scrollStartPoint.X) ?
                 -(point.X - this.scrollStartPoint.X) :
                  (this.scrollStartPoint.X - point.X),
     
                (point.Y > this.scrollStartPoint.Y) ?
                 -(point.Y - this.scrollStartPoint.Y) :
                    (this.scrollStartPoint.Y - point.Y));
       
            // Scroll to the new position.
            sview.ScrollToHorizontalOffset(
                this.scrollStartOffset.X + delta.X);
              sview.ScrollToVerticalOffset(
                  this.scrollStartOffset.Y + delta.Y);
          }
     
        base.OnPreviewMouseMove(e);
    }
     
     
     
    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
        if (this.IsMouseCaptured)
        {
              this.Cursor = Cursors.Arrow;
              this.ReleaseMouseCapture();
          }
     
        base.OnPreviewMouseUp(e);
   }


    }
  
}
