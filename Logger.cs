using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public static class LOGGER
    {
        static string filepath = string.Empty;
         public static void CreateLoggerPath()
        {
            filepath = Path.Combine(Properties.Settings.Default.temp_folder, "log.txt");
        }

        public static void Add(Exception logMessage, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                filepath = Path.Combine(Properties.Settings.Default.temp_folder, "log.txt");
            }

            try
            {
                using (StreamWriter w = File.AppendText(filepath))
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
            if (string.IsNullOrEmpty(filepath))
            {
                filepath = Path.Combine(Properties.Settings.Default.temp_folder, "log.txt");
            }

            try
            {
                using (StreamWriter w = File.AppendText(filepath))
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
        /// <summary>
        /// Log text will automatically include the linenumber and function name
        /// </summary>
        /// <param name="message"></param>
        /// <param name="lineNumber"></param>
        /// <param name="caller"></param>
        public static void Add(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                filepath = Path.Combine(Properties.Settings.Default.temp_folder, "log.txt");
            }

            try
            {
                using (StreamWriter w = File.AppendText(filepath))
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
