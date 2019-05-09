using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MonopolyGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        MobileVer mobile = new MobileVer();
        WatchVer watch = new WatchVer();
        PCVer pc = new PCVer();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            pc.ResizeMode = ResizeMode.NoResize;
            mobile.ResizeMode = ResizeMode.NoResize;
            watch.ResizeMode = ResizeMode.NoResize;
            mobile.Show();
           
            watch.Show();
            
            pc.Show();
        }
    }
}
