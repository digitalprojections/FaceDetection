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
        private delegate void dDateTimerUpdater();
        private delegate void dShowSettingsUI();

        private static MOUSE_KEYBOARD mklisteners = null;
        public CROSSBAR crossbar = null;
        private readonly System.Timers.Timer mouse_down_timer = new System.Timers.Timer();
        private readonly System.Timers.Timer datetime_ui_updater_timer = new System.Timers.Timer();
        
        // ADD Robin
        private int timeForDeleteOldFiles;
        //private int freeDiskSpaceLeft;
        //private DiskSpaceWarning warningForm;
        // END Robin
        
        //private readonly KeyboardListener keyboardListener;
        //private readonly MouseListener mouseListener;

        private RecorderCamera cameraMan = null;
        internal bool OPERATOR_CAPTURE_ALLOWED = false;
        internal bool EVENT_RECORDING_IN_PROGRESS = false;
        internal static int SELECTED_CAMERA = 0;

                
        private static MainForm or_mainForm;
        private static PictureBox or_pb_recording;
        public static Label or_current_date_text;
        private static Label or_camera_num_txt;
        private static FlowLayoutPanel or_controlBut;
        public Button rec_button;


        private BackLightController backLight;

        /// <summary>
        /// IR Sensor 人感センサー
        /// </summary>
        static IRSensor rSensor;
        /// <summary>
        /// FACEDETECTOR
        /// </summary>
        static FaceDetector faceDetector;
        //readonly Thread t;                
        private static UsbCamera.VideoFormat[] videoFormat = UsbCamera.GetVideoFormat(0);
        /// <summary>
        /// SettingsUI resolutions data, generated each time. But set to the memory value
        /// </summary>
        private static List<string> vf_resolutions = new List<string>();
        private static List<string> vf_fps = new List<string>();
        //User actions end
        static SettingsUI settingUI;
        static Form or_mainform;
        
        //public static Panel CameraPanel => GetMainForm.panelCamera;
        public static MainForm GetMainForm => or_mainForm;
        public static SettingsUI Setting_ui { get => settingUI; set => settingUI = value; }
        internal static IRSensor RSensor { get => rSensor; set => rSensor = value; }
        internal static FaceDetector FaceDetector { get => faceDetector; set => faceDetector = value; }
        public static FlowLayoutPanel Or_controlBut { get => or_controlBut; set => or_controlBut = value; }
        public static PictureBox Or_pb_recording { get => or_pb_recording; set => or_pb_recording = value; }
        public BackLightController BackLight { get => backLight; set => backLight = value; }
        public static MOUSE_KEYBOARD Mklisteners { get => mklisteners; set => mklisteners = value; }

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            
            if (vs != null && vs.Count() > 0)
            {
                PARAMETERS.HandleParameters(vs);
                Logger.Add(vs.Count + "fff");
            }
            
            if(PARAMETERS.PARAM!=null)
            {
                Logger.Add(String.Concat(PARAMETERS.PARAM));
            }

            backLight = new BackLightController();
        }        
        
        /// <summary>
        /// Get the bitmap from the 
        /// <see cref="CROSSBAR"/>
        /// </summary>
        /// <returns></returns>
        internal Bitmap GetSnapShot()
        {            
            return  crossbar.GetBitmap();
        }

        /// <summary>
        /// Main screen UI button snapshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SnapShot(object sender, EventArgs e)
        {
            SNAPSHOT_SAVER.TakeSnapShot(0);
        }

        /// <summary>
        /// One second timer to update UI datetime (it also deletes old files)
        /// </summary>
        private void UpdateDateTimeText(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            DateTimeUpdater();

            // ADD Robin
            timeForDeleteOldFiles++;
            if (timeForDeleteOldFiles >= 3600*12) // Check time 3600*12 = 12 hours 
            {
                timeForDeleteOldFiles = 0;
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
            // END Robin
        }

        private void DateTimeUpdater()
        {
            if (or_dateTimeLabel!=null && or_dateTimeLabel.InvokeRequired)
            {
                var d = new dDateTimerUpdater(DateTimeUpdater);
                or_dateTimeLabel.Invoke(d);
            }
            else
            {
                try
                {
                    or_dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine(e.Message + " ProcessFrame 225");
                    datetime_ui_updater_timer.Stop();
                }
            }
        }

        //public static void HandleParameters(String[] parameters)
        //{

        //}
      
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
            if (mouse_down_timer.Enabled == true)
            {
                mouse_down_timer.Stop();
                mouse_down_timer.Enabled = false;
            }
            //Debug.WriteLine(timer.ToString());
            if (folderButton.Visible == false)
            {
                Or_controlBut.Visible = true;
            }
            else
            {
                Or_controlBut.Visible = false;
            }
            this.TopMost = true;
        }

        //public void HoldButton(object sender, MouseEventArgs eventArgs)
        //{
        //    Logger.Add("mouse down");
        
        //    mouse_down_timer.Interval = 1000;//Set it to 3000 for production            
        //    mouse_down_timer.Enabled = true;
        //}
        //private void ReleaseButton(object sender, MouseEventArgs e)
        //{
        //    if (mouse_down_timer.Enabled == true)
        //    {                
        //        mouse_down_timer.Enabled = false;
        //    }
        //}

        private void FullScreen(object sender, EventArgs eventArgs)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (!Properties.Settings.Default.show_window_pane)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                }

                if(Properties.Settings.Default.window_on_top)
                {
                    this.TopMost = true;
                }

                
                    this.WindowState = FormWindowState.Maximized;
            
            }
            else
            {
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                if (Properties.Settings.Default.show_window_pane == true)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                }
            }
        }

        private async void ShowSettings(object sender, EventArgs e)
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
                    this.TopMost = false;
                    try
                    {
                        Setting_ui.ShowDialog();
                    }
                   catch(InvalidOperationException invx)
                    {
                        Setting_ui = new SettingsUI();
                    }
                }
            }
            
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Process.Start(Properties.Settings.Default.video_file_location);
                this.TopMost = false;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message + " line OpenStoreLocation");
            }
        }

        internal static bool AtLeastOnePreEventTimeIsNotZero()
        {
            bool retval = false;
            if (Properties.Settings.Default.seconds_before_event>0 || 
                Properties.Settings.Default.event_record_time_before_event>0)
            {
                retval = true;
            }
            return retval;
        }
        
        private void WindowSizeUpdate(object sender, EventArgs e)
        {
            //Logger.Add(settingUI.Camera_number);
            WindowSizeUpdate();
        }

        public void WindowSizeUpdate()
        {            
            //Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            if (crossbar!=null)
            {
                crossbar.SetWindowPosition(new System.Drawing.Size(this.Width, this.Height));
            }
        }
            
        public void EventRecorderOn()
        {
            PARAMETERS.PARAM.Clear();

            if (crossbar.OPER_BAN == false)
            {
                if (crossbar.PREEVENT_RECORDING)
                {
                    if (Properties.Settings.Default.capture_method == 0)
                    {
                        MainForm.GetMainForm.SET_REC_ICON();
                    }
                    TaskManager.EventAppeared(RECORD_PATH.EVENT, 1, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event), DateTime.Now);
                    MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
                }
                else
                {
                    crossbar.Start(0, CAMERA_MODES.EVENT);
                    Logger.Add("TODO: start event recording now");
                }
                crossbar.SetIconTimer(Properties.Settings.Default.event_record_time_after_event);
            }
        }

        public void EventRecorderOff()
        {
            if(crossbar.PREEVENT_RECORDING)
            {
                // do nothing
            }
            else
            {
                Logger.Add("TODO: STOP event recording now");
                crossbar.Start(0, CAMERA_MODES.PREVIEW);
            }
        }
            
        /// <summary>
        /// Manual Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleVideoRecording(object sender, EventArgs e)
        {            
            //↓20191106 Nagayama changed↓
            //Or_pb_recording.Image = Properties.Resources.Pause_Normal_Red_icon;
            Or_pb_recording.Image = Properties.Resources.player_record;
            //↑20191106 Nagayama changed↑            
            MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));
            //BackLight.ON();            
            try
            {                
                    if ((String)cameraButton.Tag == "play")
                    {
                    SetRecordButtonState("rec", false);
                    crossbar.Start(0, CAMERA_MODES.MANUAL);                    
                    MainForm.GetMainForm.SET_REC_ICON();
                }
                    else
                    {
                        //it really depends if we shoul PREVIEW ro PREEVENT
                        //set the deciding factors
                        //for now we can use this value as a test
                        //ONLY 0 index camera or the main camera is the one to be used to the manual reording?
                        
                    Or_pb_recording.Visible = false;                        
                    SetRecordButtonState("play", true);
                    SetCameraToDefaultMode();
                }
            }
            catch (InvalidOperationException iox)
            {
                Logger.Add(iox);
            }
        }

        public void SetRecordButtonState(string state, bool camnum_visible)
        {
            //or_camera_num_txt.Visible = camnum_visible;
            cameraButton.Tag = state;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();
            //backgroundWorkerMain.RunWorkerAsync();
            //if (RSensor != null)
            //{
            //    RSensor.SensorClose();
            //}
            //Console.WriteLine(this.Location.X.ToString());            

            
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Instances
            ///////////////////////////////////////
            settingUI = new SettingsUI();
            crossbar = new CROSSBAR(0, this);
            RSensor = new IRSensor();
            FaceDetector = new FaceDetector();
            //backLight = new BackLightController();
            BackLight.Start();
            Mklisteners = new MOUSE_KEYBOARD();
            ////////////////////////////////////////
            #endregion

            #region PARAMETERS
            if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0 && !PARAMETERS.PARAM.Contains("uvccameraviewer.exe"))
            {
                PARAMETERS.PARAM.Reverse();
                PARAMETERS.PARAM.Add("uvccameraviewer.exe");
                PARAMETERS.PARAM.Reverse();
                PARAMETERS.HandleParameters(PARAMETERS.PARAM);
                PARAMETERS.PARAM.Clear();
            }
            #endregion


            //Main window TIMERS
            mouse_down_timer.Elapsed += ShowButtons;//制御ボタンの非/表示用クリックタイマー
            datetime_ui_updater_timer.Interval = 1000;
            datetime_ui_updater_timer.Start();
            datetime_ui_updater_timer.AutoReset = true;
            datetime_ui_updater_timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateDateTimeText);

            //Object references
            rec_button = cameraButton;
            or_camera_num_txt = camera_number_txt;
            Or_pb_recording = pbRecording;            
            or_mainForm = this;
            or_current_date_text = or_dateTimeLabel;
            or_controlBut = or_controlButtons;
                        
            if (Properties.Settings.Default.window_on_top)
            {
                FullScreen(this, null);
            }
            
            this.WindowState = FormWindowState.Minimized; // Matsuura
            AllChangesApply();
            //WindowSizeUpdate();
            FillResolutionList();
            
            ///SET THE MAIN WINDOW ICONS AND BUTTON POSITIONS MANUALLY
            or_dateTimeLabel.Location = new Point(12, this.Height-80);
            Or_controlBut.Location = new Point(this.Width-320, this.Height-110);
            or_camera_num_txt.Location = new Point(this.Width - 90, 10);
            ///////////////////////////////////////////////////////////
            
        }

        public void SET_REC_ICON()
        {
            Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
        }

        public static void AllChangesApply()
        {
            
            
                if (Properties.Settings.Default.enable_Human_sensor)
                {
                    if (RSensor != null)
                    {
                        RSensor.Stop_IR_Timer();
                        // RSensor.Destroy();
                    }
                    //RSensor = new IRSensor();
                    RSensor.SetInterval();
                    RSensor.Start_IR_Timer();
                }
                else
                {
                    if (RSensor != null)
                    {
                        RSensor.Stop_IR_Timer();
                        //RSensor.SensorClose();
                        // RSensor.Destroy();
                    }
                }

                if (Properties.Settings.Default.enable_face_recognition)
                {
                    if (faceDetector != null)
                    {
                        FaceDetector.StopFaceTimer();
                        //faceDetector.Destroy();
                    }
                    //faceDetector = new FaceDetector();
                    faceDetector.SetInterval();
                    faceDetector.StartFaceTimer();
                }
                else
                {
                    if (faceDetector != null)
                    {
                        FaceDetector.StopFaceTimer();
                        //faceDetector.Destroy();
                    }
                }

                if (Properties.Settings.Default.Recording_when_at_the_start_of_operation)
                {
                    Mklisteners.AddMouseAndKeyboardBack();
                }
            

            //SCREEN PROPS
            SetMainScreenProperties();

            //CREATE MORE WINDOWS for more cameras
            MULTI_WINDOW.CreateCameraWindows(Camera.GetCameraCount().Length);

            //Also must check if the PREEVENT mode is needed
            SetCameraToDefaultMode();
            //Debug.WriteLine(Or_pb_recording.Visible);
            GC.Collect();
            }

        private static void SetMainScreenProperties()
        {
            if (Setting_ui == null)
            {
                Or_pb_recording.Visible = false;
                Setting_ui = new SettingsUI();
            }

            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index + 1).ToString();
            MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            MainForm.GetMainForm.Location = new System.Drawing.Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));
            

            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            if (Properties.Settings.Default.main_window_full_screen)
            {
                if (!PARAMETERS.isHidden)
                    MainForm.GetMainForm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                if (!PARAMETERS.isHidden)
                    MainForm.GetMainForm.WindowState = FormWindowState.Normal;
            }
            //Window pane
            if (Properties.Settings.Default.show_window_pane == true)
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                or_mainForm.ControlBox = true;
            }
            else
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.None;
            }
            //MainForm.GetMainForm.ClientSize = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));
            MainForm.GetMainForm.Width = Properties.Settings.Default.main_screen_size.Width;
            MainForm.GetMainForm.Height = Properties.Settings.Default.main_screen_size.Height;
            //Properties.Settings.Default.C1w = Properties.Settings.Default.main_screen_size.Width;
            //Properties.Settings.Default.C1h = Properties.Settings.Default.main_screen_size.Height;
            MainForm.GetMainForm.Location = new Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));

        }

        //public static void ParametersChangesApply()
        //{
        //    //if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0 && !PARAMETERS.PARAM.Contains("uvccameraviewer.exe"))
        //    //{
        //    //    PARAMETERS.PARAM.Reverse();
        //    //    PARAMETERS.PARAM.Add("uvccameraviewer.exe");
        //    //    PARAMETERS.PARAM.Reverse();
        //    //    PARAMETERS.HandleParameters(PARAMETERS.PARAM);                
        //    //    PARAMETERS.PARAM.Clear();

        //    //}
        //}

        public static void SetCameraToDefaultMode()
        {
            if (AtLeastOnePreEventTimeIsNotZero())
            {
                //Logger.Add("DOING: " + this.CAMERA_MODES.PREEVENT);                
                MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.PREEVENT);
            }
            else
            {
                MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.PREVIEW);
            }
        }

        public void StopAllTimers()
        {
            mouse_down_timer.Stop();
            datetime_ui_updater_timer.Stop();
            if(FaceDetector!=null)
                FaceDetector.Destroy();
            if (RSensor != null)
                RSensor.Destroy();
            try
            {
                mouse_down_timer.Dispose();
                datetime_ui_updater_timer.Dispose();                
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }            
            crossbar.ReleaseCameras();
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

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            WindowSizeUpdate();
        }

        private void BackgroundWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
            Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            Properties.Settings.Default.Save();

            Application.Exit();

        }
    }
}
