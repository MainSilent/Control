using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;

namespace Control
{
    public class Keyboard
    {
        public async Task<object> press(string key)
        {
            SendKeys.SendWait(key);
            return null;
        }
    }
}
