using System;
using System.IO;
using System.Windows.Forms;

namespace FaceDetection
{
    public class Logger
    {
        public Logger()
        {

        }

        public static void Add(Exception logMessage)
        {

            try
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
            catch (IOException iox)
            {
              //  MessageBox.Show("Cannot access Log.txt file");
            }
            
        }
        public static void Add(Exception logMessage, string message)
        {


            try
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
            catch (IOException iox)
            {
               // MessageBox.Show("Cannot access Log.txt file");
            }
        
            
        }
            
        public static void Add(string message)
            {


            try
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($"  : {message}");
                    w.WriteLine("-------------------------------");
                }
            }
            catch (IOException iox)
            {
               // MessageBox.Show("Cannot access Log.txt file");
            }
            
                


            }
        }
    }
