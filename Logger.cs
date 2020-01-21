using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public class Logger
    {
        public Logger()
        {

        }

        public static void Add(Exception logMessage, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {

            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {

                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine(" :}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace} at line { lineNumber} ({ caller})");
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                //  MessageBox.Show("Cannot access Log.txt file");
            }

        }
        public static void Add(Exception logMessage, string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {


            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {

                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace} at line { lineNumber} ({ caller})");
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                // MessageBox.Show("Cannot access Log.txt file");
            }


        }

        public static void Add(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {


            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message} at line { lineNumber} ({ caller})");
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                //using (StreamWriter w = File.AppendText("log2.txt"))
                //{
                //    w.Write("\r\nLog Entry : ");
                //    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                //    w.WriteLine($"  : {message}");
                //    w.WriteLine("-------------------------------");
                //}
                // MessageBox.Show("Cannot access Log.txt file");
            }

            //Console.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
        }
    }
}
