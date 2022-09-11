using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace PCGraph
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            String PCGraphProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == PCGraphProcessName) > 1)
            {
                MessageBox.Show("PC Graph alreadly runnig in the system please close it first", "you cant run it twice", MessageBoxButton.OK);
                App.Current.Shutdown();
            }
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk1ODQ1QDMyMzAyZTMyMmUzMEpiclZxTFRydzVOUkQybUl2WVdoMUV5V3JwbUIyS2VJeGZrMll6YjNRaUU9");
        }
    }
}
