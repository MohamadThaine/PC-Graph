using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
using PcInfoApp.PcInfoClasses;
using Process = System.Diagnostics.Process;

namespace PcInfoApp.UserControls
{
    /// <summary>
    /// Interaction logic for NetworkMonitoring.xaml
    /// </summary>
    public partial class NetworkMonitoring : UserControl
    {
        Process[] SelectedProcess;
        public NetworkMonitoring()
        {
            InitializeComponent();
        }
        private void EndProcessBT_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedProcess != null && SelectedProcess.Length > 0)
            {
                {
                    try
                    {
                        foreach (Process process in SelectedProcess)
                        {
                            process.Kill();
                        }
                        SelectedProcess = null;
                    }
                    catch(System.ComponentModel.Win32Exception ex)
                    {
                        MessageBox.Show("you cant end this process!", ex.Message);
                    }
                }
            }
            else
                MessageBox.Show("This app is not running right now!");
        }
        private void NetworkUsageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndProcessBT.IsEnabled = true;
            if(NetworkUsageListView.SelectedItem != null)
            {
                var t = NetworkUsageListView.SelectedItem.GetType();
                System.Reflection.PropertyInfo[] props = t.GetProperties();
                string AppName = props[0].GetValue(NetworkUsageListView.SelectedItem, null).ToString();
                SelectedProcess = Process.GetProcessesByName(AppName);
            }

        }
    }
}
