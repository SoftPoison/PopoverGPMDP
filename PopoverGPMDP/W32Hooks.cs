using System.Runtime.InteropServices;
using System.Windows;

namespace PopoverGPMDP {
    public static class W32Hooks {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point {
            public int X;
            public int Y;
        }
        
        public static Point GetMousePosition() {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}