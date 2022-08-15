using PcInfoApp.UserControls;
using System.Windows;

namespace PcInfoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Pcspecsbtn_Click(object sender, RoutedEventArgs e)
        {
            PcSpecs pcSpecs = new PcSpecs();
            ViewGrid.Children.Add(pcSpecs);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private string DoubleToString(double value)
        {
            return value.ToString();
        }
    }
}
