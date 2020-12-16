using FlaUI.UIA3;
using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Control
{
    class Photoshop
    {
        public static int processId;

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        // take a screenshot of the  document
        public async Task<object> screen(int handle)
        {
            Bitmap final = null;
            GetWindowThreadProcessId((IntPtr)handle, out processId);
            Process Proc = Process.GetProcessById(processId);

            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement((IntPtr)Proc.MainWindowHandle, ref placement);
            var app = new FlaUI.Core.Application(Proc);
            var automation = new UIA3Automation();
            Bitmap capture = app.GetMainWindow(automation).Capture();

            if (placement.showCmd == 1) // 1- normal, 3- maximize
            {
                final = documentArea(capture);
            }
            else
            {
                Bitmap crop = cropImage(capture, new Rectangle(8, 8, capture.Width - 16, capture.Height - 16));
                final = documentArea(crop);
            }
              
            return ImgtoBase64(final);
        }

        public static Bitmap documentArea(Bitmap b)
        {
            int x = 45;
            int y = 87;
            Rectangle area = new Rectangle(x, y, b.Width - x, b.Height - y);
            return cropImage(b, area);
        }

        public static Bitmap cropImage(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(b, -r.X, -r.Y);
                return nb;
            }
        }

        public static string ImgtoBase64(Bitmap img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            return Convert.ToBase64String(byteImage);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
       

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public int X = 0;
        public int Y = 0;
        Rectangle window = new Rectangle();

        // select tools
        public async Task<object> tools(dynamic input)
        {
            RECT rct;
            GetWindowRect(new HandleRef(this, (IntPtr)input.handle), out rct);
            window.X = rct.Left;
            window.Y = rct.Top;
            window.Width = rct.Right - rct.Left;
            window.Height = rct.Bottom - rct.Top;

            int outputX = window.X + 25;
            int outputY = window.Y + Y;

            switch (input.type)
            {
                case "brush":
                    Y = 290;
                    break;
                case "eraser":
                    Y = 370;
                    break;
                case "hand":
                    Y = 585;
                    break;
                case "reset":
                    SetCursorPos(0, 2000);
                    break;
            }

            if (input.type != "reset")
            {
                SetCursorPos(outputX, outputY);
                mouse_event(0x0002, outputX, outputY, 0, 0);
                mouse_event(0x0004, outputX, outputY, 0, 0);
            }

            return null;
        }

        // click on photoshop document
        public async Task<object> click(dynamic input)
        {
            RECT rct;
            GetWindowRect(new HandleRef(this, (IntPtr)input.handle), out rct);
            window.X = rct.Left + 45;
            window.Y = rct.Top + 87;
            window.Width = rct.Right - rct.Left;
            window.Height = rct.Bottom - rct.Top;

            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement((IntPtr)input.handle, ref placement);
            if (placement.showCmd == 3)
            {
                window.X += 8;
                window.Y += 8;
            }

            int outputX = window.X + (int)input.x;
            int outputY = window.Y + (int)input.y;

            SetCursorPos(outputX, outputY);
            mouse_event(0x0002, outputX, outputY, 0, 0);
            mouse_event(0x0004, outputX, outputY, 0, 0);

            return null;
        }

        public async Task<object> hand(string type)
        {
            switch(type)
            {
                case "up":
                    SendKeys.SendWait("{PGUP}");
                    break;
                case "down":
                    SendKeys.SendWait("{PGDN}");
                    break;
                case "left":
                    SendKeys.SendWait("^{PGUP}");
                    break;
                case "right":
                    SendKeys.SendWait("^{PGDN}");
                    break;
                case "zoom-in":
                    SendKeys.SendWait("^{+}");
                    break;
                case "zoom-out":
                    SendKeys.SendWait("^{-}");
                    break;
            }
            return null;
        }
    }
}
