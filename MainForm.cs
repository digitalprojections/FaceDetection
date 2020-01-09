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
        public CROSSBAR crossbar = null;

        private int timeForDeleteOldFiles;
        //private int freeDiskSpaceLeft;
        //private DiskSpaceWarning warningForm;

        private RecorderCamera cameraMan = null;
        internal bool OPERATOR_CAPTURE_ALLOWED = false;
        internal bool EVENT_RECORDING_IN_PROGRESS = false;
        internal static int SELECTED_CAMERA = 0;

        public Button rec_button;

        private BackLightController backLight;
        /// <summary>
        /// IR Sensor 人感センサー
        /// </summary>
        static IRSensor rSensor;
        internal static IRSensor RSensor { get => rSensor; set => rSensor = value; }

        private static UsbCamera.VideoFormat[] videoFormat = UsbCamera.GetVideoFormat(0);
        /// <summary>
        /// SettingsUI resolutions data, generated each time. But set to the memory value
        /// </summary>
        private static List<string> vf_resolutions = new List<string>();
        private static List<string> vf_fps = new List<string>();
        //User actions end
        static SettingsUI settingUI;
        static Form mainForm;

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
                Logger.Add(vs.Count + "HandleParameters");
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

        internal static bool AtLeastOnePreEventTimeIsNotZero(int cameraindex)
        {
            bool retval = false;

            switch (cameraindex)
            {
                case 0:
                    if ((Properties.Settings.Default.C1_enable_event_recorder && Properties.Settings.Default.C1_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C1_enable_Human_sensor || Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C1_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 1:
                    if ((Properties.Settings.Default.C2_enable_event_recorder && Properties.Settings.Default.C2_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C2_enable_Human_sensor || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C2_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 2:
                    if ((Properties.Settings.Default.C3_enable_event_recorder && Properties.Settings.Default.C3_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C3_enable_Human_sensor || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C3_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 3:
                    if ((Properties.Settings.Default.C4_enable_event_recorder && Properties.Settings.Default.C4_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C4_enable_Human_sensor || Properties.Settings.Default.C4_enable_face_recognition || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C4_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
            }
            return retval;
        }
        
        private void WindowSizeUpdate(object sender, EventArgs e)
        {
            WindowSizeUpdate();
        }

        public void WindowSizeUpdate()
        {
            if (crossbar==null)
            {
                crossbar?.SetWindowPosition(new System.Drawing.Size(this.Width, this.Height));
            }
        }
            
        public void EventRecorderOn(int cameraIndex)
        {
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;

            PARAMETERS.PARAM.Clear();

            switch (cameraIndex)
            {
                case 0:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_after_event);
                    
                    break;
                case 1:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_after_event);
                    
                    break;
                case 2:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_after_event);
                    
                    break;
                case 3:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_after_event);
                    
                    break;
            }

            preeventRecording = MULTI_WINDOW.PreeventRecordingState(cameraIndex);

            if (preeventRecording)
            {
                if (Properties.Settings.Default.capture_method == 0)
                {
                    TaskManager.EventAppeared(RECORD_PATH.EVENT, cameraIndex+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);
                    if (cameraIndex == 0)
                    {
                        MULTI_WINDOW.SET_REC_ICON(cameraIndex);
                        MainForm.GetMainForm.crossbar?.No_Cap_Timer_ON(timeAfterEvent);
                        crossbar?.SetIconTimer(timeAfterEvent);
                    }
                    else
                    {
                        //FormClass.GetSubForm.SetRecordIcon(cameraIndex, timeAfterEvent);
                        MULTI_WINDOW.formList[cameraIndex-1].SetRecordIcon(cameraIndex, timeAfterEvent);
                    }
                }
            }
            else
            {
                //crossbar?.Start(cameraIndex, CAMERA_MODES.EVENT); 
                //Logger.Add("TODO: start event recording now");
            }
        }

        public void EventRecorderOff(int cameraIndex)
        {           
            MULTI_WINDOW.EventRecorderOff(cameraIndex);
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //StopAllTimers(); 
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Instances
            ///////////////////////////////////////
            settingUI = new SettingsUI();
            //RSensor = new IRSensor();
            //FaceDetector = new FaceDetector();
            //backLight = new BackLightController();
            BackLight.Start();
            Mklisteners = new MOUSE_KEYBOARD();
            ////////////////////////////////////////
            #endregion

            //Main window TIMERS
            
            //Object references
            //rec_button = cameraButton;
            //camera_num_txt = camera_number_txt;
            //picbox_recording = pbRecording;            
            mainForm = this;
            //current_date_text = or_dateTimeLabel;
            //controlBut = controlButtons;
            
            this.WindowState = FormWindowState.Minimized;
            
            //crossbar?.PreviewMode();
            AllChangesApply();
            FillResolutionList();
            //ClearCutFileTempFolder();
        }

        

        public static void AllChangesApply()
        {
            int cam_index = settingUI.Camera_index;

            if (Properties.Settings.Default.C1_enable_Human_sensor || Properties.Settings.Default.C2_enable_Human_sensor || Properties.Settings.Default.C3_enable_Human_sensor || Properties.Settings.Default.C4_enable_Human_sensor)
            {
                if (RSensor != null)
                {
                    RSensor.Stop_IR_Timer();
                }
                RSensor.SetInterval();
                RSensor.Start_IR_Timer();
            }
            else
            {
                if (RSensor != null)
                {
                    RSensor.Stop_IR_Timer();
                }
            }

            

            if (Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation)
            {
                Mklisteners.AddMouseAndKeyboardBack();
            }

            
            MULTI_WINDOW.formSettingsChanged();

            //CREATE MORE WINDOWS for more cameras
            MULTI_WINDOW.CreateCameraWindows(decimal.ToInt32(Properties.Settings.Default.camera_count), cam_index);

            

            // Top most
            if (Properties.Settings.Default.window_on_top)
            {
                if (cam_index == 0)
                {
                    MainForm.GetMainForm.TopMost = true;
                    for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                    {
                        MULTI_WINDOW.formList[i].TopMost = false;
                    }
                }
                else
                {
                    if (MULTI_WINDOW.DisplayedCameraCount > 0)
                    {
                        MULTI_WINDOW.formList[cam_index - 1].TopMost = true;
                    }
                    MainForm.GetMainForm.TopMost = false;
                    for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                    {
                        if (i != (cam_index - 1))
                        {
                            MULTI_WINDOW.formList[i].TopMost = false;
                        }
                    }
                }
            }
            else
            {
                MainForm.GetMainForm.TopMost = false;
                for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                {
                    MULTI_WINDOW.formList[i].TopMost = false;
                }

                if (cam_index == 0)
                {
                    MainForm.GetMainForm.Activate();
                }
                else
                {
                    if (MULTI_WINDOW.DisplayedCameraCount > 0)
                    {
                        MULTI_WINDOW.formList[cam_index - 1].Activate();
                    }
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
                    if (cam_index == 0)
                {
                    GetMainForm.WindowState = FormWindowState.Minimized;
                }
                else
                {
                        MULTI_WINDOW.formList[cam_index - 1].WindowState = FormWindowState.Minimized;
                    }
                }
                else
                {
                    if (cam_index == 0)
                    {
                    GetMainForm.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        MULTI_WINDOW.formList[cam_index - 1].WindowState = FormWindowState.Normal;
                    }
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

            
        

        

        

        

        

        /// <summary>
        /// Loop through the camera properties to select all available resolutions and FPS
        /// </summary>
        private void FillResolutionList()
        {
            long FPS;
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////
            //Showing video formats
            for (int k = 0; k < videoFormat.Length; k++)
            {
                if (GetMainForm.UniqueVideoParameter(vf_resolutions, videoFormat[k].Size) != true)
                {
                    vf_resolutions.Add(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);                    
                }
                FPS = 10000000 / videoFormat[k].TimePerFrame;
                if (GetMainForm.UniqueFPS(FPS) != true)
                {
                    vf_fps.Add(FPS.ToString());        
                }
            }
            SettingsUI.SetComboBoxResolutionValues(vf_resolutions);
            SettingsUI.SetComboBoxFPSValues(vf_fps);
            //Logger.Add("UniqueVideoParameter " + vf_resolutions.Count);
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////            
        }

        private bool UniqueFPS(long fps)
        {
            bool retval = false;
            for (int i = 0; i < vf_fps.Count; i++)
            {
                if (UInt32.Parse(vf_fps[i]) == fps)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private bool UniqueVideoParameter(List<string> vf, Size s)
        {
            bool retval = false;
            string temp = s.Width + "x" + s.Height;
            //
            for (int i = 0; i < vf.Count; i++)
            {
                if (vf[i] == temp)
                {
                    retval = true;
                }
            }
            return retval;
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

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
            Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            //Properties.Settings.Default.main_screen_size = new Size(this.Width, this.Height);
            Properties.Settings.Default.Save();
            //WindowSizeUpdate();
        }

        private void BackgroundWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);
            
            Properties.Settings.Default.Save();

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
