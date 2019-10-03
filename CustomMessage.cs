using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CapTest
{
    class CustomMessage
    {
        public static void ShowMessage(string message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
        }
    }
}
