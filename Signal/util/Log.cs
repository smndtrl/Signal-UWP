using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Util
{
    internal class Log
    {
        public static void Warn(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"Warn: {msg}");
        }

        public static void Error(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {msg}");
        }

        public static void Warn(Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"Warn: Exception -> {e.Message}");
        }

        public static void Debug(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"Debug: {msg}");
        }
    }

    
}
