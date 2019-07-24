using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection;

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
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //Application.Run(new mainForm(vs));
            IReadOnlyCollection<string> vs1 = (IReadOnlyCollection<string>) vs;

            //
            FormCollection forms = Application.OpenForms;
            if (forms["MainForm"]!=null)
            {
                MainForm.handleParameters(vs1);
            }
            else
            {
                try
                {
                    SingleInstanceApplication.Run(new MainForm(vs1), NewInstanceHandler);
                    MainForm.handleParameters(vs1);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("FaceDetection.OpenH264Lib.dll"))
            {
                byte[] assemblyData = new byte[str.Length];
                str.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
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
