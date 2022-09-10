using PcInfoApp.util;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PcInfoApp.Overlay
{
    public partial class OverlayWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        public OverlayWindow()
        {
            InitializeComponent();


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            Win32.makeTransparent(hwnd);
        }
    }
}
