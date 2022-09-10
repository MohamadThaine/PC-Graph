using PcInfoApp.UserControls;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;
namespace PcInfoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int UserControlEnabled = 0; // to know which user control is used now , 1 = PcSpecs UserControl
        Overlay.OverlayWindow overlayWindow;
        bool IsoverlayEnabled = false;
        System.Windows.Forms.NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            PrepareTrayIcon();
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
        private void OverLayBT_Click(object sender, RoutedEventArgs e)
        {
            if(IsoverlayEnabled)
            {
                overlayWindow.Close();
                IsoverlayEnabled = false;
            }
            else
            {
                overlayWindow = new Overlay.OverlayWindow();
                overlayWindow.Show();
                IsoverlayEnabled = true;
            }
        }
        private void PrepareTrayIcon()
        {
            /*Uri iconuri = new Uri("/imgs/systemtray.ico");
            Stream icon = new Stream(iconuri);*/
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("imgs/systemtray.ico");
            notifyIcon.Visible = false;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Open App", null, NotifyIcon_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Close App", null, ShutdownApp);
            notifyIcon.ContextMenuStrip.Items.Add("Show/Hide Overlay", null, ShowOverlay);
            notifyIcon.DoubleClick += NotifyIcon_Click;
        }

        private void ShowOverlay(object? sender, EventArgs e)
        {
            if (IsoverlayEnabled)
            {
                overlayWindow.Close();
                IsoverlayEnabled = false;
            }
            else
            {
                overlayWindow = new Overlay.OverlayWindow();
                overlayWindow.Show();
                IsoverlayEnabled = true;
            }
        }

        private void ShutdownApp(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NotifyIcon_Click(object? sender, System.EventArgs e)
        {
            notifyIcon.Visible = false;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000, "Minimzed", "App minimized to tray", Forms.ToolTipIcon.Info);
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
