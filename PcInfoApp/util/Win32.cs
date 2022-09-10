using System;
using System.Runtime.InteropServices;

namespace PcInfoApp.util
{
    public class Win32
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd,
        int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd,
        int index, int newStyle);

        public static void makeTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            Win32.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public static void makeNotTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            Win32.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle ^ WS_EX_TRANSPARENT);
        }
    }
}
