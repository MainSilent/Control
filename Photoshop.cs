using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}
