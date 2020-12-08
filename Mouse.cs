using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control
{
    public class Mouse
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] INPUT[] pInput, int cbSize);

        public const int INPUT_MOUSE = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION inputUnion;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
        }

        public const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        public async Task<object> move(dynamic pos)
        {
            INPUT[] mi = new INPUT[3];
            mi[0].type = INPUT_MOUSE;
            mi[0].inputUnion.mi.dx = pos.x;
            mi[0].inputUnion.mi.dy = pos.y;
            mi[0].inputUnion.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;
            //mi[0].inputUnion.mi.dwFlags = MOUSEEVENTF_MOVE;

            SendInput(3, mi, Marshal.SizeOf(mi[0]));

            Point pt = new Point(pos.x, pos.y);
            Cursor.Position = pt;

            return null;
        }

        public async Task<object> currentPos(dynamic input)
        {
            return Cursor.Position;
        }
    }
}
