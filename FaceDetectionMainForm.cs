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
        
        private int timeForDeleteOldFiles;
        //private int freeDiskSpaceLeft;
        //private DiskSpaceWarning warningForm;
        public bool recordingInProgress;
        
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
        private static UsbCamera.VideoFormat[] videoFormat = UsbCamera.GetVideoFormat(0);
        /// <summary>
        /// SettingsUI resolutions data, generated each time. But set to the memory value
        /// </summary>
        private static List<string> vf_resolutions = new List<string>();
        private static List<string> vf_fps = new List<string>();
        //User actions end
        static SettingsUI settingUI;
        static Form or_mainform;

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
                Logger.Add(vs.Count + "HandleParameters");
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
        public void SnapShot(object sender, EventArgs e)
        {
            SNAPSHOT_SAVER.TakeSnapShot(0);
        }

        /// <summary>
        /// One second timer to update UI datetime (it also deletes old files)
        /// </summary>
        private void UpdateDateTimeText(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            DateTimeUpdater();

            timeForDeleteOldFiles++;
            if (timeForDeleteOldFiles >= 3600*1) // Check time 3600*1 = 1 hour
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
      
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
            if (mouse_down_timer.Enabled == true)
            {
                mouse_down_timer.Stop();
                mouse_down_timer.Enabled = false;
            }
            
            if (folderButton.Visible == false)
            {
                Or_controlBut.Visible = true;
            }
            else
            {
                Or_controlBut.Visible = false;
            }
        }

        private void FullScreen(object sender, EventArgs eventArgs)
        {
            FullScreen();
        }

        private void FullScreen()
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
                if (Properties.Settings.Default.show_window_pane == true)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                }
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
            }
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
            if (crossbar!=null)
            {
                crossbar.SetWindowPosition(new System.Drawing.Size(this.Width, this.Height));
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
                    preeventRecording = this.crossbar.PREEVENT_RECORDING;
                    break;
                case 1:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_after_event);
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
                case 2:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_after_event);
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
                case 3:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_after_event);
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
            }

            if (preeventRecording)
            {
                if (Properties.Settings.Default.capture_method == 0)
                {
                    TaskManager.EventAppeared(RECORD_PATH.EVENT, cameraIndex+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);
                    if (cameraIndex == 0)
                    {
                        MainForm.GetMainForm.SET_REC_ICON();
                        MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(timeAfterEvent);
                        crossbar.SetIconTimer(timeAfterEvent);
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
                //crossbar.Start(cameraIndex, CAMERA_MODES.EVENT); 
                //Logger.Add("TODO: start event recording now");
            }
        }

        public void EventRecorderOff(int cameraIndex)
        {
            bool preeventRecording = false;

            switch (cameraIndex)
            {
                case 0:
                    preeventRecording = this.crossbar.PREEVENT_RECORDING;
                    break;
                case 1:
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
                case 3:
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
                case 4:
                    preeventRecording = FormClass.GetSubForm.PreeventRecordingState(cameraIndex);
                    break;
            }

            if (!preeventRecording)
            {
                crossbar.Start(cameraIndex, CAMERA_MODES.PREVIEW);  
            }
        }
            
        /// <summary>
        /// Manual Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToggleVideoRecording(object sender, EventArgs e)
        {            
            Or_pb_recording.Image = Properties.Resources.player_record;
            MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                        
            try
            {                
                if ((String)cameraButton.Tag == "play")
                {
                    if (recordingInProgress == false)
                    {
                        SetRecordButtonState("rec");
                        crossbar.Start(0, CAMERA_MODES.MANUAL);                    
                        MainForm.GetMainForm.SET_REC_ICON();
                    }
                }
                else
                {
                    //it really depends if we shoul PREVIEW or PREEVENT
                    //set the deciding factors
                    //for now we can use this value as a test
                    //ONLY 0 index camera or the main camera is the one to be used to the manual recording?
                        
                    Or_pb_recording.Visible = false;                        
                    MainForm.GetMainForm.recordingInProgress = false;
                    SetRecordButtonState("play");
                    SetCameraToDefaultMode(0);
                }
            }
            catch (InvalidOperationException iox)
            {
                Logger.Add(iox);
            }
        }

        public void SetRecordButtonState(string state)
        {
            cameraButton.Tag = state;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers(); 
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
            or_controlBut = controlButtons;
            Or_controlBut.Location = new Point(this.Width-335, this.Height-110);
            
            this.WindowState = FormWindowState.Minimized;
            
            crossbar.PreviewMode();
            AllChangesApply();
            FillResolutionList();
            
            ///SET THE MAIN WINDOW ICONS AND BUTTON POSITIONS MANUALLY
            or_dateTimeLabel.Location = new Point(12, this.Height-80);
            or_camera_num_txt.Location = new Point(this.Width - 90, 10);
            ///////////////////////////////////////////////////////////
            
            ClearCutFileTempFolder();
        }

        public void SET_REC_ICON()
        {
            Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
            recordingInProgress = true;
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

            if (Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C4_enable_face_recognition)
            {
                if (faceDetector != null)
                {
                    FaceDetector.StopFaceTimer();
                }
                faceDetector.SetInterval();
                faceDetector.StartFaceTimer();
            }
            else
            {
                if (faceDetector != null)
                {
                    FaceDetector.StopFaceTimer();
                }
            }

            if (Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation)
            {
                Mklisteners.AddMouseAndKeyboardBack();
            }

            SetMainScreenProperties();
            MULTI_WINDOW.formSettingsChanged();

            //CREATE MORE WINDOWS for more cameras
            MULTI_WINDOW.CreateCameraWindows(decimal.ToInt32(Properties.Settings.Default.camera_count), cam_index);

            //Also must check if the PREEVENT mode is needed
            SetCameraToDefaultMode(cam_index);

            // Top most
            if (Properties.Settings.Default.window_on_top)
            {
                if (cam_index == 0)
                {
                    MainForm.GetMainForm.TopMost = true;
                    for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                    {
                        MULTI_WINDOW.formList[i].TopMost = false;
                    }
                }
                else
                {
                    if (MULTI_WINDOW.subCameraHasBeenDisplayed > 0)
                    {
                        MULTI_WINDOW.formList[cam_index - 1].TopMost = true;
                    }
                    MainForm.GetMainForm.TopMost = false;
                    for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
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
                for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                {
                    MULTI_WINDOW.formList[i].TopMost = false;
                }

                if (cam_index == 0)
                {
                    MainForm.GetMainForm.Activate();
                }
                else
                {
                    if (MULTI_WINDOW.subCameraHasBeenDisplayed > 0)
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
                    GetMainForm.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    GetMainForm.WindowState = FormWindowState.Normal;
                }

                PARAMETERS.PARAM.Clear();
            }

            GC.Collect();
        }

        public static void ParametersChangesApply()
        {
            if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0)
            {
                if (PARAMETERS.isControlButtonVisible)
                {
                    GetMainForm.controlButtons.Visible = true;
                }
                else
                {
                    GetMainForm.controlButtons.Visible = false;
                }

                PARAMETERS.PARAM.Clear();
            }
        }

        private static void SetMainScreenProperties()
        {
            int cameraIndex = Setting_ui.Camera_index;

            if (Setting_ui == null)
            {
                Or_pb_recording.Visible = false;
                MainForm.GetMainForm.recordingInProgress = false;
                Setting_ui = new SettingsUI();
            }

            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            //or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index + 1).ToString();
            //MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            MainForm.GetMainForm.Location = new System.Drawing.Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));
            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;

            if (Properties.Settings.Default.main_window_full_screen)
            {
                if (cameraIndex == 0)
                {
                    MainForm.GetMainForm.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                    {
                        if (i == cameraIndex - 1)
                        {
                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Maximized;
                        }
                    }
                }
            }
            else
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Normal;
                for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                {
                    if (i == cameraIndex - 1)
                    {
                        MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                    }
                }
            }

            if (cameraIndex != 0 && !Properties.Settings.Default.show_all_cams_simulteneously)
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Minimized;
            }

            //Window pane
            if (Properties.Settings.Default.show_window_pane == true)
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                or_mainForm.ControlBox = true;

                for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                {
                    MULTI_WINDOW.formList[i].FormBorderStyle = FormBorderStyle.Sizable;
                }
            }
            else
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.None;

                for (int i = 0; i < MULTI_WINDOW.subCameraHasBeenDisplayed; i++)
                {
                    MULTI_WINDOW.formList[i].FormBorderStyle = FormBorderStyle.None;
                }
            }

            MainForm.GetMainForm.Width = Properties.Settings.Default.main_screen_size.Width;
            MainForm.GetMainForm.Height = Properties.Settings.Default.main_screen_size.Height;
            MainForm.GetMainForm.Location = new Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));
        }

        public static void SetCameraToDefaultMode(int cameraindex)
        {
            if (AtLeastOnePreEventTimeIsNotZero(cameraindex))
            {
                if (cameraindex == 0)
                {
                    MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.PREEVENT);
                }
                else
                {
                    if (MULTI_WINDOW.subCameraHasBeenDisplayed > 0)
                    {
                        FormClass.GetSubForm.SetToPreeventMode(cameraindex);
                    }
                }
            }
            else
            {
                if (cameraindex == 0)
                {
                    MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.PREVIEW);
                }
                else
                {
                    if (MULTI_WINDOW.subCameraHasBeenDisplayed > 0)
                    {
                        FormClass.GetSubForm.SetToPreviewMode(cameraindex);
                    }
                }
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
