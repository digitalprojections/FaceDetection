using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace FaceDetection
{
    static class Program
    {        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]      

        static void Main(string[] vs)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IReadOnlyCollection<string> vs1 = (IReadOnlyCollection<string>) vs;

            //
            FormCollection forms = Application.OpenForms;
            if (forms["MainForm"] !=null)
            {
                PARAMETERS.HandleParameters(vs1);
            }
            else
            {
                try
                {
                    /*
                    uvc_param.Reverse();
                    uvc_param.Add("uvccameraviewer");
                    uvc_param.Reverse();
                    var uvc_param_final = (IReadOnlyCollection<string>) uvc_param;
                     */
                    Logger.Add(vs1.Count + " PARAMETER COUNT at line 45");
                    SingleInstanceApplication.Run(new MainForm(vs1), NewInstanceHandler);
                    //PARAMETERS.HandleParameters(vs1);
                }
                catch (Exception e)
                {
                    Logger.Add(e.ToString());
                }
            }
        }                

        public static void NewInstanceHandler(object sender, StartupNextInstanceEventArgs e)
        {            
            var uvc_param = e.CommandLine.ToList<string>();         
            e.BringToForeground = false;
            PARAMETERS.HandleParameters(uvc_param);            
        }
        public class SingleInstanceApplication : WindowsFormsApplicationBase
        {
            private SingleInstanceApplication()
            {
                base.IsSingleInstance = true;
            }

            public static void Run(Form f, StartupNextInstanceEventHandler startupHandler)
            {
                SingleInstanceApplication app = new SingleInstanceApplication
                {
                    MainForm = f
                };
                app.StartupNextInstance += startupHandler;
                app.Run(Environment.GetCommandLineArgs());
            }
        }
    }
}
