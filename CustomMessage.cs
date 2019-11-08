using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FaceDetection
{
    class CustomMessage
    {
        public static void Add(string message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {            
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
            


        }
        public static void Add(int message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");

        }
        public static void Add(decimal message,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");

        }
        public static void Add(decimal message, IntPtr x,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {
            Debug.WriteLine(message + " with " + x + " at line " + lineNumber + " (" + caller + ")");

        }
    }
}
