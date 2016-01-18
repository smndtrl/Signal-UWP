using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Util
{
    class Log
    {
        public static void Warn(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"Warn: {msg}");
        }

        public static void Debug(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"Debug: {msg}");
        }
    }

    
}
