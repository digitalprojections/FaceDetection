using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FaceDetection
{
    class CustomMessage
    {
        public static void ShowMessage(string message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {            
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")\n");
            


        }
        public static void ShowMessage(int message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")\n");

        }
        public static void ShowMessage(decimal message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")\n");

        }
        public static void ShowMessage(decimal message, IntPtr x,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " with " + x + " at line " + lineNumber + " (" + caller + ")\n");

        }
    }
}
