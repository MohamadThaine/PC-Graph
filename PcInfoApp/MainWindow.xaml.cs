using PcInfoApp.UserControls;
using System.Windows;

namespace PcInfoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int UserControlEnabled = 0; // to know which user control is used now , 1 = PcSpecs UserControl
        public MainWindow()
        {
            InitializeComponent();
            PcSpecs pcSpecs = new PcSpecs();
            UserControlEnabled = 1;
            ViewGrid.Children.Add(pcSpecs);
        }
        private void Pcspecsbtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlEnabled != 1)
            {
                ViewGrid.Children.Clear();
                UserControlEnabled = 1;
                PcSpecs pcSpecs = new PcSpecs();
                ViewGrid.Children.Add(pcSpecs);
            }
        }
        private void FileSizeBT_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlEnabled != 2)
            {
                ViewGrid.Children.Clear();
                UserControlEnabled = 2;
                FilesSize filesSize = new FilesSize();
                ViewGrid.Children.Add(filesSize);
            }
        }
        private void NetworkBt_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlEnabled != 3)
            {
                ViewGrid.Children.Clear();
                UserControlEnabled = 3;
                NetworkMonitoring networkMonitoring = new NetworkMonitoring();
                ViewGrid.Children.Add(networkMonitoring);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
