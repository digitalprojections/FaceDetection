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

        private Task openSettingsTask;

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
        /// Sets the camera to the Recording mode
        /// </summary>
        internal void RecordMode()
        {
            this.Update();
            //this will set the backlight ON            
            //backlight.ON();
            //StartTheRecorder();
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
        private void SnapShot(object sender, EventArgs e)
        {
            //timage.Dispose();
            SNAPSHOT_SAVER.TakeSnapShot(0);
        }
        /// <summary>
        /// One second timer to update UI datetime
        /// </summary>
        private void UpdateDateTimeText(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            // ADD Robin
            timeForDeleteOldFiles++;
            if (timeForDeleteOldFiles >= 3600*12) // 3600 = 1 hour
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
            DateTimeUpdater();
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

        public static void HandleParameters(String[] parameters)
        {

        }
      
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
                or_controlButtons.Visible = true;
           }
            else
            {
                or_controlButtons.Visible = false;
            }
            this.TopMost = true;
        }

        

        public void HoldButton(object sender, MouseEventArgs eventArgs)
        {
            Logger.Add("mouse down");
            
            mouse_down_timer.Interval = 1000;//Set it to 3000 for production            
            mouse_down_timer.Enabled = true;
        }
        private void ReleaseButton(object sender, MouseEventArgs e)
        {
            if (mouse_down_timer.Enabled == true)
            {                
                mouse_down_timer.Enabled = false;
            }
        }
        private void FullScreen(object sender, EventArgs eventArgs)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                
                if (!Properties.Settings.Default.show_window_pane)
                    this.FormBorderStyle = FormBorderStyle.None;

                if(Properties.Settings.Default.window_on_top)
                    this.TopMost = true;

                if(Properties.Settings.Default.main_window_full_screen)
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
            
                if (Setting_ui.Visible == false)
                {
                    this.TopMost = false;
                    Setting_ui.ShowDialog();
                }
                else
                {
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
                MessageBox.Show(ioe.Message + " line 840");
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
            
                Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
                Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            if (crossbar!=null)
                crossbar.SetWindowPosition(new System.Drawing.Size(this.Width, this.Height));
            
        }
        public void EventRecorderOn()
        {
            if (crossbar.PREEVENT_RECORDING)
            {
                TaskManager.EventAppeared(RECORD_PATH.EVENT, 1, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
            }
            else
            {
                crossbar.Start(0, CAMERA_MODES.EVENT);
                Logger.Add("TODO: start event recording now");
            }
            crossbar.SET_ICON_TIMER(Properties.Settings.Default.event_record_time_after_event);
        }
        public void EventRecorderOff()
        {
            if(crossbar.PREEVENT_RECORDING)
            {

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
                {
                    if ((String)cameraButton.Tag == "play")
                    {
                        SetRecordButtonState("rec", false);
                        crossbar.Start(0, CAMERA_MODES.MANUAL);                        
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
                    
            }
            catch (InvalidOperationException iox)
            {
                Logger.Add(iox);
            }
        }

        public void SetRecordButtonState(string state, bool camnum_visible)
        {
            or_camera_num_txt.Visible = camnum_visible;
            cameraButton.Tag = state;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();            
            Logger.Add(this.Location.X.ToString());
            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
            Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            Properties.Settings.Default.Save();
            
            Application.Exit();
        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Instances
            ///////////////////////////////////////
            crossbar = new CROSSBAR();
            RSensor = new IRSensor();
            //FaceDetector = new FaceDetector();
            //backLight = new BackLightController();
            BackLight.Start();
            Mklisteners = new MOUSE_KEYBOARD();            
            ////////////////////////////////////////
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
                        
            
            if (Properties.Settings.Default.window_on_top)
            {
                FullScreen(this, null);
            }
            
            AllChangesApply();
            WindowSizeUpdate();
            FillResolutionList();
            this.Width = decimal.ToInt32(Properties.Settings.Default.C1w);
            this.Height = decimal.ToInt32(Properties.Settings.Default.C1h);
            this.Location = new Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));            

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
                    RSensor.Destroy();
                }
                RSensor = new IRSensor();
                RSensor.Start_IR_Timer();
            }
            else
            {
                if (RSensor != null)
                {
                    RSensor.Stop_IR_Timer();
                    RSensor.Destroy();
                }
            }


            if (Properties.Settings.Default.enable_face_recognition)
            {
                if (FaceDetector != null)
                {
                    FaceDetector.Stop_Face_Timer();
                    FaceDetector.Destroy();
                }
                faceDetector = new FaceDetector();
                FaceDetector.Start_Face_Timer();
            }
            else
            {
                if (FaceDetector != null)
                {
                    FaceDetector.Stop_Face_Timer();
                    FaceDetector.Destroy();
                }
            }

            if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {
                Mklisteners.AddMouseAndKeyboardBack();
            }
            if (Setting_ui == null)
            {
                Or_pb_recording.Visible = false;
                Setting_ui = new SettingsUI();                
            }
            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index + 1).ToString();
            MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            

            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            if (Properties.Settings.Default.main_window_full_screen)
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Maximized;                
            }
            else
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Normal;                
            }
            //FULL SCREEN

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

            //Also must check if the PREEVENT mode is needed
            SetCameraToDefaultMode();
            
           
            
            if (PARAMETERS.PARAM!=null && PARAMETERS.PARAM.Count > 0 && !PARAMETERS.PARAM.Contains("uvccameraviewer"))
            {
                PARAMETERS.PARAM.Reverse();
                PARAMETERS.PARAM.Add("uvccameraviewer");
                PARAMETERS.PARAM.Reverse();
                //param = String.Concat(PARAM).ToLower();
                PARAMETERS.HandleParameters(PARAMETERS.PARAM);
                PARAMETERS.PARAM.Clear();

            }



            Debug.WriteLine(Or_pb_recording.Visible);
            GC.Collect();
        }

        private static void SetCameraToDefaultMode()
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
                    vf_resolutions.Add(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);                    
                FPS = 10000000 / videoFormat[k].TimePerFrame;
                if (GetMainForm.UniqueFPS(FPS) != true)
                    vf_fps.Add(FPS.ToString());        
            }
            SettingsUI.SetComboBoxResolutionValues(vf_resolutions);
            SettingsUI.SetComboBoxFPSValues(vf_fps);
            //Logger.Add("UniqueVideoParameter " + vf_resolutions.Count);
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AllChangesApply();
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
    }

}
