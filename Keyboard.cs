using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control
{
    public class Keyboard
    {
        public async Task<object> press(string key)
        {
            SendKeys.SendWait("{"+ key + "}");
            return null;
        }
    }
}
