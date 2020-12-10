using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    class Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        Rectangle window = new Rectangle();

        public async Task<object> bounds(int handle)
        {
            RECT rct;

            GetWindowRect(new HandleRef(this, (IntPtr)handle), out rct);

            window.X = rct.Left;
            window.Y = rct.Top;
            window.Width = rct.Right - rct.Left;
            window.Height = rct.Bottom - rct.Top;

            return window;
        }

        // Manage window
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public async Task<object> manage(dynamic input)
        {
            switch(input.type)
            {
                case "close":
                    int processid = 0;
                    GetWindowThreadProcessId((IntPtr)input.handle, out processid);
                    Process tempProc = Process.GetProcessById(processid);
                    tempProc.CloseMainWindow();
                    break;

                case "maximize":
                    WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                    GetWindowPlacement((IntPtr)input.handle, ref placement);

                    switch (placement.showCmd)
                    {
                        case 1:
                            ShowWindow((IntPtr)input.handle, SW_MAXIMIZE);
                            break;
                        case 2:
                            ShowWindow((IntPtr)input.handle, SW_RESTORE);
                            break;
                        case 3:
                            ShowWindow((IntPtr)input.handle, SW_SHOWNORMAL);
                            break;
                    }
                    break;

                case "minimize":
                    ShowWindow((IntPtr)input.handle, SW_MINIMIZE);
                    break;
            }
            return null;
        }
    }
}
