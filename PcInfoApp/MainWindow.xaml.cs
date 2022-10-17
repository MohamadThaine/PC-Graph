using PCGraph.UserControls;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Forms = System.Windows.Forms;
namespace PCGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
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
                var btn = sender as Button;
                var lvi = FindAncestor<ListViewItem>(btn);
                if (lvi != null)
                {
                    var listview = FindAncestor<ListView>(lvi);
                    listview.SelectedItem = lvi;
                    lvi.IsSelected = true;
                }
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
                var btn = sender as Button;
                var lvi = FindAncestor<ListViewItem>(btn);
                if (lvi != null)
                {
                    var listview = FindAncestor<ListView>(lvi);
                    listview.SelectedItem = lvi;
                    lvi.IsSelected = true;
                }
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
                var btn = sender as Button;
                var lvi = FindAncestor<ListViewItem>(btn);
                if (lvi != null)
                {
                    var listview = FindAncestor<ListView>(lvi);
                    listview.SelectedItem = lvi;
                    lvi.IsSelected = true;
                }
            }
        }
        private void OverLayBT_Click(object sender, RoutedEventArgs e)
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
            var btn = sender as Button;
            var lvi = FindAncestor<ListViewItem>(btn);
            if (lvi != null)
            {
                lvi.IsSelected = false;
            }
        }
        private void SystemSpecsBT_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlEnabled != 4)
            {
                ViewGrid.Children.Clear();
                UserControlEnabled = 4;
                SystemInfo SystemInfo = new SystemInfo();
                ViewGrid.Children.Add(SystemInfo);
                var btn = sender as Button;
                var lvi = FindAncestor<ListViewItem>(btn);
                if (lvi != null)
                {
                    var listview = FindAncestor<ListView>(lvi);
                    listview.SelectedItem = lvi;
                    lvi.IsSelected = true;
                }
            }
        }
        private void PrepareTrayIcon()
        {
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("imgs/systemtray.ico");
            notifyIcon.Visible = false;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Open PC Graph", null, NotifyIcon_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Show/Hide Overlay", null, ShowOverlay);
            notifyIcon.ContextMenuStrip.Items.Add("Close PC Graph", null, ShutdownApp);
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
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
            this.ShowActivated = false;
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private static T FindAncestor<T>(FrameworkElement fe) where T : FrameworkElement
        {
            if (fe == null)
            {
                return null;
            }
            var p = fe.Parent as FrameworkElement ?? fe.TemplatedParent as FrameworkElement;
            if (p == null)
            {
                return null;
            }
            var match = p as T;
            return match ?? FindAncestor<T>(p);
        }
    }
}
