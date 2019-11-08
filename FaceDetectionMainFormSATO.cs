using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using FaceDetectionX;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        public CROSSBAR crossbar = null;
        private readonly System.Timers.Timer mouse_down_timer = new System.Timers.Timer();
        private readonly System.Windows.Forms.Timer datetime_ui_updater_timer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer face_detection_timer = new System.Windows.Forms.Timer();
        //LL Muse and keyboard
        private readonly KeyboardListener keyboardListener;
        private readonly MouseListener mouseListener;
        private static MainForm or_mainForm;
        private static PictureBox or_pb_recording;
        private static Label or_current_date_text;
        private static Label or_camera_num_txt;
        private static FlowLayoutPanel or_controlBut;
        /// <summary>
        /// IR Sensor 人感センサー
        /// </summary>
        static IRSensor rSensor;
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
        //Cascade
        private static CascadeClassifier cascade = new CascadeClassifier();

        //public static Panel CameraPanel => GetMainForm.panelCamera;
        public static MainForm GetMainForm => or_mainForm;
        public static SettingsUI SettingUI { get => settingUI; set => settingUI = value; }
        internal static IRSensor RSensor { get => rSensor; set => rSensor = value; }
        public static FlowLayoutPanel Or_controlBut { get => or_controlBut; set => or_controlBut = value; }
        public static PictureBox Or_pb_recording { get => or_pb_recording; set => or_pb_recording = value; }

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            crossbar = new CROSSBAR();
            keyboardListener = new KeyboardListener();
            mouseListener = new MouseListener();
            RSensor = new IRSensor();
            if (vs != null && vs.Count() > 0)
            {
                PARAMETERS.HandleParameters(vs);
            }
            BackLightController.Init();
        }        
        
        /// <summary>
        /// Sets the camera to the Recording mode
        /// </summary>
        internal void RecordMode()
        {
            this.Update();
            //this will set the backlight ON            
            //BackLightController.ON();
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
            SNAPSHOT_SAVER.TakeSnapShot();

        }
        /// <summary>
        /// One second timer to update UI datetime
        /// </summary>
        private void UpdateDateTimeText(object sender, EventArgs eventArgs)
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
        /// <summary>
        /// Capture face only works on a PC mode
        /// It uses OpenCV library
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CaptureFace(object sender, EventArgs eventArgs)
        {
            //==============digital-projects=================\
            //------------------------------------------------\
            //********    *******  ***   ****   ***  ********||\
            //*** * *     ******* ** **  ** ** ** ** *** * **||:\
            //*** * *       ***   ** **  ** ** ** ** *** * **||:/
            //********      ***    ***   ****   ***  ********||/
            //________________________________________________/
            //==============digital-projects=================/
            //CustomMessage.ShowMessage("Face detection timer tick");
            try
            {
                Rect[] rectList = cascade.DetectMultiScale(MainForm.GetMainForm.GetSnapShot().ToMat());
                int count = 0;
                foreach (Rect rectFace in rectList)
                {
                    count++;
                    // 見つかった
                    Console.WriteLine("発見" + count);
                }
            }
            catch (NullReferenceException e)
            {
                face_detection_timer.Stop();
            }
            Console.WriteLine(DateTime.Now);
        }
        /// <summary>
        /// Resumes IRsensor check cycle
        /// </summary>
        internal void ResumeSensor()
        {
            if(Properties.Settings.Default.enable_Human_sensor)
            {
                RSensor.StartOM_Timer();
            }else
            {
                RSensor.StopOM_Timer();
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

        internal void AddMouseAndKeyboardBack()
        {
            keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove += MouseListener_MouseMove;
        }

        public void HoldButton(object sender, MouseEventArgs eventArgs)
        {
            CustomMessage.ShowMessage("mouse down");
            mouse_down_timer.Enabled = true;
            mouse_down_timer.Interval = 1000;//Set it to 3000 for production            
            mouse_down_timer.Start();
        }
        private void ReleaseButton(object sender, MouseEventArgs e)
        {
            if (mouse_down_timer.Enabled == true)
            {
                mouse_down_timer.Stop();
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
        private void ShowSettings(object sender, EventArgs e)
        {
            
            if (SettingUI.Visible == false)
            {
                //settingUI.TopMost = true;
                this.TopMost = false;
                Debug.WriteLine("topmost 824");
                SettingUI.ShowDialog();
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
                //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                //folderBrowserDialog.SelectedPath = Properties.Settings.Default.video_file_location;
                //folderBrowserDialog.ShowDialog(this);
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
            //CustomMessage.ShowMessage(settingUI.Camera_number);
            WindowSizeUpdate();
        }
        public void WindowSizeUpdate()
        {
            if (SettingUI != null)
            {
                Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
                Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
                //crossbar.SetWindowPosition(new System.Drawing.Size(this.Width, this.Height));
            }
        }
        public void EventRecorderOn()
        {
            if (crossbar.PREEVENT_RECORDING)
            {
                TaskManager.EventAppeared("EVENT", 1, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
            }
            else
            {
                crossbar.Start(0, CROSSBAR.CAMERA_MODES.EVENT);
                CustomMessage.ShowMessage("TODO: start event recording now");
            }
        }
        public void EventRecorderOff()
        {
            if(crossbar.PREEVENT_RECORDING)
            {

            }
            else
            {
                //CustomMessage.ShowMessage("TODO: STOP event recording now");

            }
            
        }
        /// <summary>
        /// Manual Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleVideoRecording(object sender, EventArgs e)
        {            
            
            Or_pb_recording.Image = Properties.Resources.Record_Pressed_icon;
            Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
            BackLightController.ON();            
            try
            {
                if (crossbar.PREEVENT_RECORDING)
                {
                    if ((String)cameraButton.Tag == "play")
                    {
                        or_camera_num_txt.Visible = false;
                        Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        cameraButton.Tag = "rec";
                        //crossbar.Start(0, CROSSBAR.CAMERA_MODES.MANUAL);
                        TaskManager.EventAppeared("MOVIE", 1, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
                    }
                    else
                    {
                        //it really depends if we shoul PREVIEW ro PREEVENT
                        //set the deciding factors
                        //for now we can use this value as a test
                        //ONLY 0 index camera or the main camera is the one to be used to the manual reording?
                        or_camera_num_txt.Visible = true;
                        Or_pb_recording.Visible = false;
                        cameraButton.Tag = "play";
                        //crossbar.Start(0, CROSSBAR.CAMERA_MODES.PREVIEW);                        
                    }
                }else
                {
                    if ((String)cameraButton.Tag == "play")
                    {
                        or_camera_num_txt.Visible = false;
                        Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        cameraButton.Tag = "rec";
                        crossbar.Start(0, CROSSBAR.CAMERA_MODES.MANUAL);                        
                    }
                    else
                    {
                        //it really depends if we shoul PREVIEW ro PREEVENT
                        //set the deciding factors
                        //for now we can use this value as a test
                        //ONLY 0 index camera or the main camera is the one to be used to the manual reording?
                        or_camera_num_txt.Visible = true;
                        Or_pb_recording.Visible = false;
                        cameraButton.Tag = "play";
                        crossbar.Start(0, CROSSBAR.CAMERA_MODES.PREVIEW);

                    }
                }
                    
            }
            catch (InvalidOperationException iox)
            {
                Logger.Add(iox);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();            
            CustomMessage.ShowMessage(this.Location.X.ToString());
            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
            Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
            Properties.Settings.Default.Save();
            crossbar.ReleaseCameras();
            Application.Exit();
        }
        private void KeyDownAllEventHandler(object sender, KeyEventArgs e)
        {
            //CustomMessage.ShowMessage(e.KeyCode.ToString());
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            e.Handled = true;
            CustomMessage.ShowMessage("TODO: " + CROSSBAR.CAMERA_MODES.EVENT);
        }
        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            CustomMessage.ShowMessage("TODO: " + CROSSBAR.CAMERA_MODES.EVENT);
            BackLightController.Restart();
            
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove += MouseListener_MouseMove;

            mouse_down_timer.Elapsed += ShowButtons;//制御ボタンの非/表示用クリックタイマー

            datetime_ui_updater_timer.Interval = 1000;
            datetime_ui_updater_timer.Start();
            datetime_ui_updater_timer.Tick += new EventHandler(UpdateDateTimeText);

            // Cascadeファイル読み込み
            string cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            cascade.Load(cascade_file);
            // 顔認証
            face_detection_timer.Interval = 1000;  // 更新間隔 (ミリ秒)
            face_detection_timer.Tick += new EventHandler(CaptureFace);
            face_detection_timer.Start();

            or_camera_num_txt = camera_number_txt;

            Or_pb_recording = pbRecording;

            //SET Object references
            or_mainForm = this;
            or_current_date_text = or_dateTimeLabel;

            //==============digital-projects=================\
            //------------------------------------------------\
            //*********** *******  ***   ****   ***  ********||\
            //*********** ******* ** **  ** ** ** ** ********||:\
            //***********   ***   ** **  ** ** ** ** ********||:/
            //***********   ***    ***   ****   ***  ********||/
            //________________________________________________/
            //==============digital-projects=================/
            
            if (Properties.Settings.Default.window_on_top)
            {
                FullScreen(this, null);
            }
            this.Size = new System.Drawing.Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));
            AllChangesApply();
            WindowSizeUpdate();            
            Camera.SetNumberOfCameras();            
            // Fill camera list combobox with available resolutions
            FillResolutionList();
            //BackLightController.Start(decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 10000, 1000);
        }
        public static void AllChangesApply()
        {
            if (SettingUI == null)
            {
                SettingUI = new SettingsUI();
            }
                        
            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index+1).ToString();

            MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;            
            if (Properties.Settings.Default.main_window_full_screen)
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Normal;
            }
            MainForm.GetMainForm.Size = new System.Drawing.Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));
            MainForm.GetMainForm.Location = new System.Drawing.Point(decimal.ToInt32(Properties.Settings.Default.C1x), decimal.ToInt32(Properties.Settings.Default.C1y));
            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            if (Properties.Settings.Default.show_window_pane == true)
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                or_mainForm.ControlBox = true;
            }
            else
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.None;
            }
            if (Or_pb_recording.Visible == true)
            {
                //Or_pb_recording.Image = Properties.Resources.Pause_Normal_Red_icon;
                Or_pb_recording.Visible = false;
            }

            //Also must check if the Preevent mode is needed
            if (AtLeastOnePreEventTimeIsNotZero())
            {
                //CustomMessage.ShowMessage("DOING: " + CROSSBAR.CAMERA_MODES.PREEVENT);                
                MainForm.GetMainForm.crossbar.Start(0, CROSSBAR.CAMERA_MODES.PREEVENT);
            }                
            else
            {                
                MainForm.GetMainForm.crossbar.Start(0, CROSSBAR.CAMERA_MODES.PREVIEW);
                if (Properties.Settings.Default.capture_operator)
                {
                    MainForm.GetMainForm.ResumeSensor();
                    CustomMessage.ShowMessage("IR sensor ON");
                }
            }
            Debug.WriteLine(Or_pb_recording.Visible);
            GC.Collect();
        }
                
        public void StopAllTimers()
        {
            mouse_down_timer.Stop();
            datetime_ui_updater_timer.Stop();
            face_detection_timer.Stop();
            try
            {
                mouse_down_timer.Dispose();
                datetime_ui_updater_timer.Dispose();
                face_detection_timer.Dispose();
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }
            //BackLightController.OFF();
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
            CustomMessage.ShowMessage("UniqueVideoParameter " + vf_resolutions.Count);
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AllChangesApply();
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
        private bool UniqueVideoParameter(List<string> vf, System.Drawing.Size s)
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

        public System.Drawing.Size GetResolution(int cam_ind)
        {
            System.Drawing.Size retval;
            string[] res;
            switch (cam_ind)
            {
                case 0:
                    res = Properties.Settings.Default.C1res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 1:
                    res = Properties.Settings.Default.C2res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 2:
                    res = Properties.Settings.Default.C3res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 3:
                    res = Properties.Settings.Default.C4res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                default: return new System.Drawing.Size(640, 480);
            }
        }
        public int GetFPS(int cam_ind)
        {
            int fps = 15;
            switch (cam_ind)
            {
                case 0:
                    fps = Int32.Parse(Properties.Settings.Default.C1f);
                    break;
                case 1:
                    fps = Int32.Parse(Properties.Settings.Default.C2f);
                    break;
                case 2:
                    fps = Int32.Parse(Properties.Settings.Default.C3f);
                    break;
                case 3:
                    fps = Int32.Parse(Properties.Settings.Default.C4f);
                    break;
            }
            return fps;
        }

        #region DLL IMPORTS

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point p);

        #endregion DLL IMPORTS
    }

}
