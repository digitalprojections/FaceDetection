using System;
using System.IO;

namespace FaceDetection
{
    public class Logger
    {
        public Logger()
        {
            
        }

        public static void Add(Exception logMessage)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine(" :}");
                w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace}");
                w.WriteLine("-------------------------------");
            }            
        }
        public static void Add(Exception logMessage, string message)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine($"  : {message}");
                w.WriteLine($"  :{logMessage.Message}{logMessage.InnerException}");
                w.WriteLine($"  :{logMessage.Message}{logMessage.StackTrace}");
                w.WriteLine("-------------------------------");
            }

        }
        public static void Add(string message)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine($"  : {message}");                
                w.WriteLine("-------------------------------");
            }
            
        }
    }
}
