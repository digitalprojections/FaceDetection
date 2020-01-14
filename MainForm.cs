using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using FaceDetectionX;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        private delegate void dShowSettingsUI();

        private static MOUSE_KEYBOARD mklisteners = null;
                
        //private int timeForDeleteOldFiles;
        //private int freeDiskSpaceLeft;
        //private DiskSpaceWarning warningForm;

        
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
        static SettingsUI settingUI;
        static Form mainForm;
        public int CAMERA_INDEX = 0;

        static Stopwatch stopwatch = new Stopwatch();

        public static MainForm GetMainForm => (MainForm) mainForm;
        public static SettingsUI Setting_ui { get => settingUI; set => settingUI = value; }
        
        public BackLightController BackLight { get => backLight; set => backLight = value; }
        public static MOUSE_KEYBOARD Mklisteners { get => mklisteners; set => mklisteners = value; }
        public bool AnyRecordingInProgress { get => MULTI_WINDOW.RecordingIsOn();}

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            
            if (vs != null && vs.Count() > 0)
            {
                PARAMETERS.HandleParameters(vs);
                Logger.Add(vs.Count + " HandleParameters");
            }
            
            if(PARAMETERS.PARAM!=null)
            {
                Logger.Add(String.Concat(PARAMETERS.PARAM));
            }

            backLight = new BackLightController();
            
            stopwatch.Start();
        }        
        
        
        public async void ShowSettings(object sender, EventArgs e)
        {
            ShowSettingsDialogAsync();
        }

        private void ShowSettingsDialogAsync()
        {
            if (Setting_ui.InvokeRequired)
            {
                var d = new dShowSettingsUI(ShowSettingsDialogAsync);
                Setting_ui.Invoke(d);
            }
            else
            {
                if (Setting_ui.Visible == false)
                {
                    try
                    {
                        Setting_ui.Show();
                    }
                    catch (InvalidOperationException invx)
                    {
                        Setting_ui = new SettingsUI();
                    }
                }
            }
        }

        //private void OpenStoreLocation(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
        //        Process.Start(Properties.Settings.Default.video_file_location);
        //        this.TopMost = false;
        //    }
        //    catch (IOException ioe)
        //    {
        //        MessageBox.Show(ioe.Message + " line OpenStoreLocation");
        //    }
        //}
            
        public void EventRecorderOn(int cameraIndex)
        {
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;

            PARAMETERS.PARAM.Clear();

            PROPERTY_FUNCTIONS.GetPreAndPostEventTimes(cameraIndex, out timeBeforeEvent, out timeAfterEvent);
            
            preeventRecording = MULTI_WINDOW.PreeventRecordingState(cameraIndex);

            if (preeventRecording)
            {
                if (Properties.Settings.Default.capture_method == 0)
                {
                    TaskManager.EventAppeared(RECORD_PATH.EVENT, cameraIndex+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);
                    
                    MULTI_WINDOW.SET_REC_ICON(cameraIndex);                                        
                    MULTI_WINDOW.formList[cameraIndex].SetRecordIcon(cameraIndex, timeAfterEvent);                    
                }
            }
            else
            {
                MULTI_WINDOW.formList[cameraIndex].crossbar?.Start(cameraIndex, CAMERA_MODES.EVENT); 
                Logger.Add("EVENT RECORDING STARTS (console call using parameters)");
            }
        }

        public void EventRecorderOff(int cameraIndex)
        {           
            MULTI_WINDOW.EventRecorderOff(cameraIndex);
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //StopAll();             
            RSensor?.Destroy();
            Application.Exit();
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Instances
            ///////////////////////////////////////
            settingUI = new SettingsUI();
            //RSensor = new IRSensor();            
            backLight = new BackLightController();
            BackLight.Start();
            Mklisteners = new MOUSE_KEYBOARD();
            ////////////////////////////////////////
            #endregion
            //Object references                    
            mainForm = this;            
            this.WindowState = FormWindowState.Minimized;            
            AllChangesApply();            
            ClearCutFileTempFolder();
            ClearTempFolder();
        }

        public static void AllChangesApply()
        {
            int cam_index = settingUI.Camera_index;//MAIN CAMERA INDEX

            if (PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(cam_index))
            {
                RSensor?.Stop_IR_Timer();                
                RSensor?.SetInterval();
                RSensor?.Start_IR_Timer();
            }
            else
            {                
                RSensor?.Stop_IR_Timer();             
            }
            
            if (Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation)
            {
                Mklisteners.AddMouseAndKeyboardBack();
            }
            
            MULTI_WINDOW.formSettingsChanged();

            //CREATE CAMERA WINDOWS
            MULTI_WINDOW.CreateCameraWindows();

            // Top most
            if (Properties.Settings.Default.window_on_top)
            {

                    if (MULTI_WINDOW.displayedCameraCount > 0)
                    {
                    MULTI_WINDOW.formList[cam_index].Activate();
                    }
            }
            else
            {
                //MainForm.GetMainForm.TopMost = false;
                //for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                //{
                //    MULTI_WINDOW.formList[i].TopMost = false;                    
                //}
                if (MULTI_WINDOW.displayedCameraCount > 0)
                {
                    MULTI_WINDOW.formList[cam_index].Activate();//MAIN CAMERA active
                }
            }

            if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0 && !PARAMETERS.PARAM.Contains("uvccameraviewer.exe"))
            {
                PARAMETERS.PARAM.Reverse();
                PARAMETERS.PARAM.Add("uvccameraviewer.exe");
                PARAMETERS.PARAM.Reverse();
                PARAMETERS.HandleParameters(PARAMETERS.PARAM);

                if (PARAMETERS.isMinimized)
                {
                    MULTI_WINDOW.formList[PARAMETERS.CameraIndex].WindowState = FormWindowState.Minimized;
                }
                else
                {
                    MULTI_WINDOW.formList[PARAMETERS.CameraIndex].WindowState = FormWindowState.Normal;
                }

                PARAMETERS.PARAM.Clear();
            }

            GC.Collect();

            if (stopwatch.ElapsedMilliseconds>=3600000)
            {
                stopwatch.Restart();
                CheckDiskSpace.DeleteOldFiles();
                //    freeDiskSpaceLeft = CheckDiskSpace.CheckDisk();
                //    if (freeDiskSpaceLeft < 2) // 2 Go
                //    {
                //        try
                //        {
                //            warningForm.Select(); // If the form already exist, put it on the front
                //        }
                //        catch (Exception ex) // If the form doesn't exist yet, create it
                //        {
                //            warningForm = new DiskSpaceWarning();
                //            warningForm.Show();
                //        }
                //    }
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
                    File.SetAttributes(listFilesToClear.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                    File.Delete(listFilesToClear.ElementAt(i - 1));
                }
            }
            catch (Exception ex)
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

        //private void MainForm_ResizeEnd(object sender, EventArgs e)
        //{
        //    Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
        //    Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
        //    //Properties.Settings.Default.main_screen_size = new Size(this.Width, this.Height);
        //    Properties.Settings.Default.Save();
        //    //WindowSizeUpdate();
        //}

        private void BackgroundWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            //Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);            
            //Properties.Settings.Default.Save();

            Application.Exit();
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

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
