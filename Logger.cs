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

        public static void Add(Exception logMessage,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string caller = null)
        {

            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {

                    w.Write("\r\nLog Entry Exception thrown: ");
                    w.WriteLine($"  : at line {lineNumber} in {caller}");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine(" :}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace}");                    
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                //  MessageBox.Show("Cannot access Log.txt file");
                Add(iox);
            }

        }
        public static void Add(Exception logMessage, string message)
        {
            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {

                    w.Write("\r\nLog Entry EXCEPTION thrown: ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                    w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace}");
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                // MessageBox.Show("Cannot access Log.txt file");
                Add(iox);
            }


        }

        public static void Add(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {


            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {

                    w.Write("\r\nLog Entry from Message: ");
                    w.WriteLine($"  : at line {lineNumber} in {caller}");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message}");                    
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
                using (StreamWriter w = File.AppendText("log2.txt"))
                {

                    w.Write("\r\nLog Entry : " + iox.Message);
                    w.WriteLine($"  : at line {lineNumber} in {caller}");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message}");                    
                    w.WriteLine("-------------------------------");
                }
                // MessageBox.Show("Cannot access Log.txt file");
            }

            Console.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
        }
    }
}
