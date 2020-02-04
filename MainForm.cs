using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        private delegate void dShowSettingsUI();
        private static MOUSE_KEYBOARD mklisteners = null;
        internal bool OPERATOR_CAPTURE_ALLOWED = false;
        internal bool EVENT_RECORDING_IN_PROGRESS = false;
        internal static int SELECTED_CAMERA = 0;        
        private BackLightController backLight;
        /// <summary>
        /// IR Sensor 人感センサー
        /// </summary>
        static IRSensor rSensor;
        internal static IRSensor RSensor { get => rSensor; set => rSensor = value; }        
        //User actions end
        
        static Form mainForm;
        public static int CAMERA_INDEX = 0;
        static Stopwatch stopwatch = new Stopwatch();
        public static MainForm GetMainForm => (MainForm) mainForm;
        
        //static SettingsUI settingUI;
        public static SettingsUI Settingui { get; set; }        
        public BackLightController BackLight { get => backLight; set => backLight = value; }
        public static MOUSE_KEYBOARD Mklisteners { get => mklisteners; set => mklisteners = value; }
        public static bool AnyRecordingInProgress { get => MULTI_WINDOW.RecordingIsOn();}

        private readonly System.Timers.Timer datetime_timer = new System.Timers.Timer();

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            LOGGER.CreateLoggerPath();


            if (Camera.GetCameraCount().Length > 0)
            {
                if (vs != null && vs.Count() > 0)
                {
                    PARAMETERS.HandleParameters(vs);
                    PARAMETERS.WAKEUPCALL = true;
                    
                    LOGGER.Add(vs.Count + " HandleParameters");
                }

                if (PARAMETERS.PARAM != null)
                {
                    LOGGER.Add(String.Concat(PARAMETERS.PARAM));
                }

                backLight = new BackLightController();

                if(Properties.Settings.Default.enable_delete_old_files)
                {
                    CheckOldFiles.DeleteOldFiles();
                    stopwatch.Start();
                }
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            RSensor?.Destroy();
            Dispose();
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            Camera.SetNumberOfCameras();
            if (Camera.GetCameraCount().Length > 0)
            {

                #region Instances
                ///////////////////////////////////////
                //settingUI = new SettingsUI();
                RSensor = new IRSensor();
                backLight = new BackLightController();
                BackLight.Start();
                Mklisteners = new MOUSE_KEYBOARD();
                
                ////////////////////////////////////////
                if (PINVOKES.IsTouchEnabled())
                {
                    LOGGER.Add("********** You are using a touch enabled device **********");
                }

                #endregion
                //Object references                    
                mainForm = this;
                this.WindowState = FormWindowState.Minimized;

                if (Camera.GetCameraCount().Length > 0 && !PARAMETERS.WrongParameter)
                {
                    AllChangesApply();
                    ClearCutFileTempFolder();
                    ClearTempFolder();

                    datetime_timer.Interval = 1000;
                    datetime_timer.Start();
                    datetime_timer.AutoReset = true;
                    datetime_timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateDateTimeText);
                }else if (PARAMETERS.WrongParameter)
                {
                    Application.Exit();
                }
            }
            
        }

        public static void AllChangesApply()
        {
            if (Settingui == null)
            {
                Settingui = new SettingsUI();
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.culture);
            
            CAMERA_INDEX = Properties.Settings.Default.main_camera_index;//MAIN CAMERA INDEX
            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(CAMERA_INDEX, out bool irsensorOn);
            

            if (irsensorOn)
            {
                RSensor?.Stop_IR_Timer();                
                RSensor?.SetInterval();
                RSensor?.Start_IR_Timer();
            }
            else
            {                
                RSensor?.Stop_IR_Timer();             
            }

            PROPERTY_FUNCTIONS.GetOnOperationStartSwitch(CAMERA_INDEX, out bool spymode);
            if (spymode)
            {
                Mklisteners.AddMouseAndKeyboardBack();
            }
            
            

            PARAMETERS.HandleWakeUpParameters();

            if (!PARAMETERS.WrongParameter)
            {
                //CREATE CAMERA WINDOWS
                MULTI_WINDOW.CreateCameraWindows();
                MULTI_WINDOW.FormSettingsChanged();
            }
            
            if(PARAMETERS.callByParameters)
            {
                if (PARAMETERS.minimizedByParameters)
                {
                    for (int i = 0; i < Properties.Settings.Default.camera_count; i++)
                    {
                        if (PARAMETERS.minimizedByParameter[i])
                        {
                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
                        }
                    }
                    PARAMETERS.minimizedByParameters = false;
                }

                PARAMETERS.callByParameters = false;
            }

            GC.Collect();
        }

        public void ShowSettings(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Button settings_button = (Button)sender;
                ShowSettingsDialogAsync(int.Parse(settings_button.Tag.ToString()));
            }
        }

        private void ShowSettingsDialogAsync(int cameraIndex)
        {
            if (Settingui.InvokeRequired)
            {
                var d = new dShowSettingsUI(() => ShowSettingsDialogAsync(cameraIndex));
                Settingui.Invoke(d);
            }
            else
            {
                if (Settingui.Visible == false)
                {
                    try
                    {
                        Settingui.ShowSettings(cameraIndex);
                        Settingui.DisabledButtonWhenRecording();
                    }
                    catch (InvalidOperationException invx)
                    {
                        Settingui = new SettingsUI();
                    }
                }
            }
        }

        private void ClearCutFileTempFolder()
        {
            string[] listFiles1, listFiles2, listFiles3, listFiles4;
            List<string> listFilesToClear = new List<string>();
            try
            {
                if (Directory.Exists(@"D:\TEMP\1\CutTemp"))
                {
                    listFiles1 = Directory.GetFiles(@"D:\TEMP\1\CutTemp");
                }
                else
                {
                    listFiles1 = Directory.GetFiles(@"C:\TEMP\1\CutTemp");
                }

                listFilesToClear = listFiles1.ToList();
                for (int i = listFilesToClear.Count; i > 0; i--)
                {
                    try { 
                        File.SetAttributes(listFilesToClear.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                    }
                    catch (ArgumentException ax)
                    {
                        /*                         
                        path is empty, contains only white spaces, contains invalid characters, or the file attribute is invalid.
                         */
                    }
                    catch (FileNotFoundException fnfx)
                    {

                    }

                    File.Delete(listFilesToClear.ElementAt(i - 1));
                }
            }
            catch (IOException ex)
            {
                //Console.WriteLine(@" TEMP\1\CutTemp does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\2\CutTemp"))
                {
                    listFiles2 = Directory.GetFiles(@"D:\TEMP\2\CutTemp");
                }
                else
                {
                    listFiles2 = Directory.GetFiles(@"C:\TEMP\2\CutTemp");
                }
                listFilesToClear = listFiles2.ToList();
                for (int i = listFilesToClear.Count; i > 0; i--)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i - 1));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\2\CutTemp does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\3\CutTemp"))
                {
                    listFiles3 = Directory.GetFiles(@"D:\TEMP\3\CutTemp");
                }
                else
                {
                    listFiles3 = Directory.GetFiles(@"C:\TEMP\3\CutTemp");
                }
                listFilesToClear = listFiles3.ToList();
                for (int i = listFilesToClear.Count; i > 0; i--)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i - 1));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\3\CutTemp does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\4\CutTemp"))
                {
                    listFiles4 = Directory.GetFiles(@"D:\TEMP\4\CutTemp");
                }
                else
                {
                    listFiles4 = Directory.GetFiles(@"C:\TEMP\4\CutTemp");
                }
                listFilesToClear = listFiles4.ToList();
                for (int i = listFilesToClear.Count; i > 0; i--)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i - 1));
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine(@" TEMP\4\CutTemp does not exist");
            }
        }

        private void ClearTempFolder()
        {
            string[] listFiles1, listFiles2, listFiles3, listFiles4;
            List<string> listFilesToClear = new List<string>();
            try
            {
                if (Directory.Exists(@"D:\TEMP\1"))
                {
                    listFiles1 = Directory.GetFiles(@"D:\TEMP\1");
                }
                else
                {
                    listFiles1 = Directory.GetFiles(@"C:\TEMP\1");
                }

                listFilesToClear = listFiles1.ToList();
                for (int i = 0; i < listFilesToClear.Count; i++)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\1 does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\2"))
                {
                    listFiles2 = Directory.GetFiles(@"D:\TEMP\2");
                }
                else
                {
                    listFiles2 = Directory.GetFiles(@"C:\TEMP\2");
                }
                listFilesToClear = listFiles2.ToList();
                for (int i = 0; i < listFilesToClear.Count; i++)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\2 does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\3"))
                {
                    listFiles3 = Directory.GetFiles(@"D:\TEMP\3");
                }
                else
                {
                    listFiles3 = Directory.GetFiles(@"C:\TEMP\3");
                }
                listFilesToClear = listFiles3.ToList();
                for (int i = 0; i < listFilesToClear.Count; i++)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\3 does not exist");
            }

            try
            {
                if (Directory.Exists(@"D:\TEMP\4"))
                {
                    listFiles4 = Directory.GetFiles(@"D:\TEMP\4");
                }
                else
                {
                    listFiles4 = Directory.GetFiles(@"C:\TEMP\4");
                }
                listFilesToClear = listFiles4.ToList();
                for (int i = 0; i < listFilesToClear.Count; i++)
                {
                    File.SetAttributes(listFilesToClear.ElementAt(i), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i));
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(@" TEMP\4 does not exist");
            }
        }

        private void UpdateDateTimeText(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            for (int i = 0; i < 4; i++)
            {
                if (MULTI_WINDOW.formList[i] != null)
                {
                    if (MULTI_WINDOW.formList[i].DISPLAYED == true)
                    {
                        MULTI_WINDOW.formList[i].DateTimeUpdater();
                    }
                }
            }

            // Check for delete old files necessary
            if (Properties.Settings.Default.enable_delete_old_files && stopwatch.ElapsedMilliseconds >= 3600000)
            {
                stopwatch.Restart();
                CheckOldFiles.DeleteOldFiles();
            }
        }

        private void BackgroundWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            Application.Exit();
        }

        private new void Dispose()
        {
            datetime_timer?.Dispose();
            backLight?.Dispose();
        }

        public static void ManageDeleteOldFilesTimer(bool checkboxState)
        {
            if(checkboxState)
            {
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
            }
        }
    }

    class PINVOKES
    {

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        public static bool IsTouchEnabled()
        {
            int MAXTOUCHES_INDEX = 0x95;
            int maxTouches = GetSystemMetrics(MAXTOUCHES_INDEX);

            return maxTouches > 0;
        }
        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }
    }
}
