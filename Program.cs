using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new mainForm(vs));
            IReadOnlyCollection<string> vs1 = (IReadOnlyCollection<string>) vs;

            MainForm.handleParameters(vs1);

            SingleInstanceApplication.Run(new MainForm(), NewInstanceHandler);
        }
        public static void NewInstanceHandler(object sender, StartupNextInstanceEventArgs e)
        {
            var uvc_param = e.CommandLine;
            //MessageBox.Show(uvc_param);
            e.BringToForeground = false;
            MainForm.handleParameters(uvc_param);
            
        }
        public class SingleInstanceApplication : WindowsFormsApplicationBase
        {
            private SingleInstanceApplication()
            {
                base.IsSingleInstance = true;
            }

            public static void Run(Form f, StartupNextInstanceEventHandler startupHandler)
            {
                SingleInstanceApplication app = new SingleInstanceApplication();
                app.MainForm = f;
                app.StartupNextInstance += startupHandler;
                app.Run(Environment.GetCommandLineArgs());
            }
        }
    }
}
