﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DirectShowLib;
using System.Runtime.InteropServices.ComTypes;
using GitHub.secile.Video;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        //User actions
        private readonly Timer recording_length_timer = new Timer();
        private readonly Timer backlight_timer = new Timer();
        private readonly Timer mouse_down_timer = new Timer();
        private readonly Timer face_timer = new Timer();
        private readonly Timer datetime_ui_updater = new Timer();

        private RecorderCamera cameraMan = null;
        /// <summary>
        /// Camera modes to assist identify current status 
        /// of the application during operation
        /// Manage it properly, carefully, 
        /// so there are no confusion or contradictions
        /// Available choices
        /// <para>NONE</para>
        /// <para>PREVIEW</para>
        /// <para>CAPTURE</para>
        /// <para>FACE (recommended use on PC)</para>
        /// <para>HIDDEN (recording mode without preview)</para>
        /// </summary>
        internal enum CAMERA_MODES
        {
            NONE,
            PREVIEW,
            CAPTURE,
            FACE,
            HIDDEN,
            ERROR
        }

        internal static class RECPATH
        {
            public static string NORMAL = "";
            public static string MANUAL = "MOVIE";
            public static string EVENT = "EVENT";            
        }

        internal static string ACTIVE_RECPATH = RECPATH.MANUAL;
        internal static int SELECTED_CAMERA = 0;

        /*
         All cameras have the same modes
         But only the Main camera supports operator capture         
         (各カメラには異なるモードがあります メインカメラのみがオペレータキャプチャをサポートしています)
             */

        internal static CAMERA_MODES CURRENT_MODE = 0;
               
        internal CAMERA_MODES CAMERA_MODE {
            get
            {
                return CURRENT_MODE;
            }
            set
            {
                CURRENT_MODE = value;
            }
            }


        // Camera choice
        //private CameraChoice _CameraChoice = new CameraChoice();


        
        private static MainForm or_mainForm;
        private static PictureBox or_pb_recording;
        
        private static Label or_current_date_text;
        private static Label or_camera_num_txt;
        private static FlowLayoutPanel or_controlBut;   


        //PROPERTY

        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        //IRSensor
        static IRSensor rSensor;
        //readonly Thread t;                
        private static UsbCamera.VideoFormat[] videoFormat = UsbCamera.GetVideoFormat(0);
                
        /// <summary>
        /// SettingsUI resolutions data, generated each time. But set to the memory value
        /// </summary>
        private static List<string> vf_resolutions = new List<string>();
        private static List<long> vf_fps = new List<long>();
        //User actions end
        static SettingsUI settingUI;
        static Form or_mainform;
        //kameyama camera
        //public static Camera CheckCamera;

        /// <summary>
        /// Sets the camera to the Recording mode
        /// </summary>
        internal void RecordMode()
        {
           
            if (Properties.Settings.Default.backlight_on_upon_face_rec)
            {
                BacklightOn();
            }
        }


        public static Panel CameraPanel => GetMainForm.panelCamera;
        public static MainForm GetMainForm => or_mainForm;

       // private static UsbCamera camera;
        //private static RecorderCamera camcorder;
        private TaskManager taskManager;
       
        /*
        public static UsbCamera GetCamera()
        {           
            return camera;
        }
        internal RecorderCamera GetRecorder()
        {
            return camcorder;
        }*/

        private static void SetCamera(UsbCamera value)
        {
            //camera = value;
        }

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            taskManager = new TaskManager();
            cameraMan = new RecorderCamera();
            

            rSensor = new IRSensor();

            if (vs != null && vs.Count() > 0)
            {
                HandleParameters(vs);
            }
        }
        

        /// <summary>
        /// One second timer to update UI datetime
        /// </summary>        
        private void ProcessFrame(object sender, EventArgs eventArgs)
        {
            try
            {
                or_dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
                datetime_ui_updater.Enabled = false;
                datetime_ui_updater.Stop();
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
            //TODO
            //Console.WriteLine("Face detection timer tick");
        }

        /// <summary>
        /// No parameters
        /// </summary>
        public static void HandleParameters()
        {

        }

        #region HANDLEPAMETERS
        /// <summary>
        /// Accepts parameters to control the app work flow and settings
        /// </summary>
        /// <param name="parameters"></param>
        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            Console.WriteLine("Parameters: " + String.Concat(parameters));
            string param = String.Concat(parameters).ToLower();            
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (param.Contains("uvccameraviewer"))
            {
                
                if (parameters.Count > 0)
                {
                    //Only 2 parameter elements
                    if (parameters.Count==2)
                    {                     
                         CURRENT_MODE = CAMERA_MODES.HIDDEN;
                        //There is only one parameter with 1 element???
                        //
                        return;
                    }


                        switch (parameters.ElementAt(1))
                    {
                        case "-c":
                            try
                            {
                                    if (parameters.ElementAt(2) == "1")
                                    {


                                        if (settingUI != null && settingUI.Visible == false)
                                        {
                                            settingUI.TopMost = true;
                                            or_mainForm.TopMost = false;
                                            settingUI.Show();
                                        }

                                    }
                                    else
                                    {
                                        settingUI.Hide();
                                        FormChangesApply();

                                    }
                            }
                            
                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Debug.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "-s":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    //Properties.Settings.Default.main_camera_index = Int32.Parse(parameters.ElementAt(2));
                                    MainForm.GetMainForm.TakeSnapShot();
                                }

                                

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.Message + " in line 286");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "-b":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    Console.WriteLine("テスト" + parameters.ElementAt(0) + "×" + parameters.ElementAt(1) + "×" + parameters.ElementAt(2) + "×" + parameters.ElementAt(3));

                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        /*
                                     SHOW CONTROL BUTTONS    
                                     */
                                        or_controlBut.Visible = true;
                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {
                                        /*
                                     HIDE CONTROL BUTTONS    
                                     */
                                        or_controlBut.Visible = false;
                                    }

                                }
                               
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "-d":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(3) == "1")
                                    {

                                    }
                                    if (parameters.ElementAt(3) == "0")
                                    {

                                    }

                                    CycleTime(parameters);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 387");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        //kameyama beginning 20191018  
                        case "-h":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(2) == "1")
                                    {
                                        rSensor.StartOM_Timer();
                                    }
                                    else
                                    {
                                        rSensor.StopOM_Timer();
                                    }

                                    CycleTime(parameters);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {

                            }
                            break;
      
                        case "-v":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {

                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        GetMainForm.TopMost = true;
                                        settingUI.TopMost = false;
                                        GetMainForm.Show();
                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {
                                        GetMainForm.Hide();
                                        FormChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        
                        case "-l":
                            try
                            {
                              
                                    if (parameters.ElementAt(3) == "1")
                                    {
                                        BacklightOn();

                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {

                                        BacklightOff();
                                    }
                                
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Debug.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "-n":
                            try {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(3) == "1")
                                    {

                                    }
                                    else if (parameters.ElementAt(3) == "0")
                                    {

                                    }
                                }
                            }
                            catch { }
                            break;
                        case "-w":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(2) == "1")
                                    {

                                        //kameyama comennt 20191020
                                        //Properties.Settings.Default.show_window_pane = true;
                                        //FormChangesApply();
                                        GetMainForm.FormBorderStyle = FormBorderStyle.Sizable;


                                        if (settingUI != null && settingUI.Visible == false)
                                        {
                                            settingUI.TopMost = true;
                                            or_mainForm.TopMost = false;
                                            settingUI.Show();
                                        }

                                    }
                                    else
                                    {
                                        settingUI.Hide();
                                        FormChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-q":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                       {                                            
                                            ManualRecordingOn(parameters.ElementAt(1));
                                        }
                                    else if (parameters.ElementAt(2) == "0")
                                        {
                                            ManualRecordingOff(parameters.ElementAt(1));
                                        }

                                
                                
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-e":
                            try
                            {
                                if (CheckCameraIndex(parameters))
                                {
                                    if (parameters.ElementAt(2) == "1")
                                    {
                                        if (settingUI != null && settingUI.Visible == false)
                                        {
                                            settingUI.TopMost = true;
                                            or_mainForm.TopMost = false;
                                            settingUI.Show();
                                        }

                                    }
                                    else if (parameters.ElementAt(2) == "0")
                                    {
                                        settingUI.Hide();
                                        FormChangesApply();


                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                     
                    }
                }

            }
        }
        #endregion HANDLEPAMETERS
        /// <summary>
        /// Camera number is correct if one of 1,2,3,4 or 9
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool CheckCameraIndex(IReadOnlyCollection<string> parameters)
        {
            bool Retval = false;

            switch (Int32.Parse(parameters.ElementAt(2)))
            {
                case 1:
                    Retval = true;
                    Console.WriteLine("カメラ番号まで来た");
                    //Camera.CheckCamera(parameters.ElementAt());
                    break;
                case 2:
                    Retval = true;
                    break;
                case 3:
                    Retval = true;
                    break;
                case 4:
                    Retval = true;
                    break;
                case 9:
                    Retval = true;
                    break;

              
            }
            return Retval;
        }

         private static void CycleTime(IReadOnlyCollection<string> parameters)
        {
            if(Int32.Parse(parameters.ElementAt(4))>=0 && Int32.Parse(parameters.ElementAt(4))<=1000)
            {
                Console.WriteLine("できた");
            }           
        }


        /// <summary>
        /// Resumes IRsensor check cycle
        /// </summary>
        internal void ResumeSensor()
        {
            if(Properties.Settings.Default.capture_operator)
            {
                rSensor.StartOM_Timer();
            }            
        }

        private static void SetWindowPane(bool value)
        {
            
        }

        private static void ManualRecordingOff(string camnum)
        {
            
            MainForm.GetMainForm.TakeStartVideoRecording();
        }

        private static void ManualRecordingOn(string camnum)
        {
            MainForm.GetMainForm.TakeStartVideoRecording();
        }

        private static void EventRecorder()
        {
            
        }

        /// <summary>
        /// Turns backlight off
        /// </summary>
        private static void BacklightOff()
        {
           
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOff);

        }

        public static void BacklightOn()
        {
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOn);
            MainForm.GetMainForm.backlight_timer.Stop();
            if (Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                MainForm.GetMainForm.backlight_timer.Start();
            }

        }
        //kameyama comment 20191019 end


        public static void HandleParameters(String[] parameters)
        {

        }
        /// <summary>
        /// PREVIEW MODE CAMERA
        /// </summary>
        public static void GetCameraInstance()

        {
            //CURRENT_MODE = CAMERA_MODES.PREVIEW;
            MainForm.GetMainForm.cameraMan.ReleaseInterfaces();
            //MainForm.GetMainForm.cameraMan.GetInterfaces();
            MainForm.GetMainForm.cameraMan.StartCamera(0, new Size(1280, 720), 15, CameraPanel.Handle);

            

        }
        /// <summary>
        /// RECORDER CAMERA
        /// </summary>
        public void GetCamcorderInstance()
        {
            CURRENT_MODE = CAMERA_MODES.CAPTURE;
            or_camera_num_txt.Visible = false;
            pbRecording.Image = Properties.Resources.Record_Pressed_icon;
            pbRecording.Visible = true;
            
            //kameyama

            //MANAGE TIMERS            
            recording_length_timer.Start();    
            BacklightOn();

           
            //Dynamically generate filename in the right path, for the right camera
            //Checking for the active path that will make sure the path is fitting the recording type
            string dstFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi";
                
            //
            //Initialize the CAPTURE camera by index
            try
            {
                cameraMan.ReleaseInterfaces();

                cameraMan.StartRecorderCamera(0, new Size(1280, 720), 15, CameraPanel.Handle, dstFileName) ;
                CURRENT_MODE = CAMERA_MODES.CAPTURE;
            }
            catch(InvalidOperationException iox)
            {

                CURRENT_MODE = CAMERA_MODES.NONE;
                Logger.Add(iox.Message);
                Console.WriteLine(dstFileName + " dest file 560" + CameraPanel.Handle);
                //camcorder = new UsbCamcorder(0, new Size(1280, 720), 15, or_cameraPanel.Handle, dstFileName);
            }
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
        public void HoldButton(object sender, MouseEventArgs eventArgs)
        {
            Console.WriteLine("mouse down");
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
            
            if (settingUI.Visible == false)
            {
                settingUI.TopMost = true;
                this.TopMost = false;
                Debug.WriteLine("topmost 824");
                settingUI.Show();
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
                MessageBox.Show(ioe.Message);
            }
        }

        public static void FormChangesApply()
        {
            MainForm.GetMainForm.ControlBox = Properties.Settings.Default.show_window_pane;
            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = (SELECTED_CAMERA+1).ToString();
            
            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            //capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            //or_mainForm.TopMost = Properties.Settings.Default.window_on_top;
            //frameTimer.Interval = 1000 / Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
            if (Properties.Settings.Default.show_window_pane == true)
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                or_mainForm.ControlBox = true;
            }
            else
            {
                or_mainForm.FormBorderStyle = FormBorderStyle.None;
            }
            if (or_pb_recording.Visible == true)
            {
                or_pb_recording.Image = Properties.Resources.Pause_Normal_Red_icon;
                or_pb_recording.Visible = false;
            }
            else
            {
                //設定ウィンドウからの変更なので、recording_on かどうかによる表示する
                //recording_onがfalseの場合は表示する必要はない
                if (Properties.Settings.Default.show_recording_icon == true && CURRENT_MODE == CAMERA_MODES.CAPTURE)
                {
                    or_pb_recording.Image = Properties.Resources.Record_Pressed_icon;
                    or_pb_recording.Visible = true;
                }
            }
            Debug.WriteLine(or_pb_recording.Visible);

            if (CURRENT_MODE == CAMERA_MODES.CAPTURE)
            {
                GetMainForm.GetCamcorderInstance();
            }else if(CURRENT_MODE == CAMERA_MODES.PREVIEW)
            {
                GetCameraInstance();
            }
            
            GC.Collect();
        }

        private void LastPositionUpdate(object sender, EventArgs e)
        {

            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.Save();
        }

        private void WindowSizeUpdate(object sender, EventArgs e)
        {
            //Console.WriteLine(settingUI.Camera_number);

            WindowSizeUpdate();
        }

        public void WindowSizeUpdate()
        {
            if (settingUI != null && settingUI.Camera_index != 0)
            {
                Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
                Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
                Properties.Settings.Default.Save();
            }

            //Debug.WriteLine(Properties.Camera1.Default.view_width);
            try
            {
                cameraMan.SetWindowPosition(new Size(this.Width, this.Height));
            }catch(InvalidComObjectException icom)
            {
                Console.WriteLine(icom.Message + icom.StackTrace);
            }
                
        }

        private void SnapShot(object sender, EventArgs e)
        {

            //timage.Dispose();
            TakeSnapShot();

        }

        internal void TakeSnapShot()
        {
            //set it, but not use it here
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;            
            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);
                        
            // Save the bitmap as a JPEG file with quality level 80.
            myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            

            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "SNAPSHOT");            
            Directory.CreateDirectory(picloc);            
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            //two versions here. Depending on camera mode
            
            //pic from regular camera
            cameraMan.GetBitmap().Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
            
            
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }


        private void StartVideoRecording(object sender, EventArgs e)
        {
            ACTIVE_RECPATH = RECPATH.MANUAL;
            TakeStartVideoRecording();
        }

        private void TakeStartVideoRecording()
        {
            if (pbRecording.Visible == true && CURRENT_MODE == CAMERA_MODES.CAPTURE)
            {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;                
                CURRENT_MODE = CAMERA_MODES.PREVIEW;
                
                                
                GetCameraInstance();
            }
            else 
                {   
                    
                    GetCamcorderInstance();
                }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();
            cameraMan.ReleaseInterfaces();
            
            Application.Exit();
        }
        
        private void SetCamera(IMoniker camera_moniker)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Error while running camera");
            }            
        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {
            //CURRENT_MODE = CAMERA_MODES.HIDDEN;
            if (settingUI == null)
            {
                settingUI = new SettingsUI();
                mouse_down_timer.Tick += new EventHandler(ShowButtons);//制御ボタンの非/表示用クリックタイマー

                if (Properties.Settings.Default.enable_face_recognition)
                {
                    face_timer.Tick += new EventHandler(CaptureFace);
                    face_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
                    face_timer.Start();
                }

                datetime_ui_updater.Interval = 1000;
                datetime_ui_updater.Start();
                datetime_ui_updater.Tick += new EventHandler(ProcessFrame);

                if(Properties.Settings.Default.recording_length_seconds>0)
                {
                    recording_length_timer.Interval = decimal.ToInt32(Properties.Settings.Default.seconds_after_event) * 1000;
                    recording_length_timer.Tick += Recording_length_timer_Tick;
                }

                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60000;
                backlight_timer.Tick += Backlight_timer_Tick;

                if (Properties.Settings.Default.enable_backlight_off_when_idle)
                {
                    backlight_timer.Start();
                }
                or_camera_num_txt = camera_number_txt;
                or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
                or_camera_num_txt.Text = (Properties.Settings.Default.current_camera_index+1).ToString();
            }
            Console.WriteLine("Line 876");

            //Console.WriteLine(panelCamera.Handle);
            /*
             * Application.Idle += ProcessFrame;
             */
            or_pb_recording = pbRecording;
            pbRecording.BackColor = Color.Transparent;
            //dateTimeLabel.BackColor = Color.Transparent;
            

            //Set this CAMERA Dynamically to the relevant one
            if (settingUI.Camera_index != 0)
            {
                var camx = Properties.Settings.Default.C1x.ToString();
                var camy = Properties.Settings.Default.C1y.ToString();
                var camw = Properties.Settings.Default.C1w.ToString();
                var camh = Properties.Settings.Default.C2h.ToString();
                this.Location = new Point(Int32.Parse(camx), Int32.Parse(camy));
                
                this.Size = new Size(Int32.Parse(camw), Int32.Parse(camh));                
            }

            this.TopMost = Properties.Settings.Default.window_on_top;
            //SET Object references
            or_mainForm = this;
            or_current_date_text = or_dateTimeLabel;            
            
            or_controlBut = or_controlButtons;

            //FormChangesApply();

            //Debug.WriteLine(Convert.ToInt32(Properties.Settings.Default.backlight_offset_mins) + " WIDTH");
            or_mainform = this;

            //var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/test.mp4";
            //var encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");

            or_mainform.FormClosing += (s, ev) => {
                Debug.WriteLine("recording ended");
                StopAllTimers();
            };
                                 
            // Fill camera list combobox with available resolutions
            
            
            //FillResolutionList();
            
            
            //TODO
            //Also must check if the 
            
            if (Properties.Settings.Default.seconds_before_event>0)
            {
                GetCamcorderInstance();
            }
            else
            {
                
                GetCameraInstance();

                if (Properties.Settings.Default.capture_operator)
                {
                    
                    ResumeSensor();
                }
            }
            

            if (Properties.Settings.Default.window_on_top)
            {
                FullScreen(this, null);
            }

            WindowSizeUpdate();
            WindowPositionUpdate();

            Camera.SetNumberOfCameras();

        }

        private void WindowPositionUpdate()
        {
            Location = new Point(Convert.ToInt32(Properties.Settings.Default.C1x), Convert.ToInt32(Properties.Settings.Default.C1y));
            Console.WriteLine(Properties.Settings.Default.C1x);
            Properties.Settings.Default.Save();
        }

        private void Backlight_timer_Tick(object sender, EventArgs e)
        {
            BacklightOff();
        }

        private void Recording_length_timer_Tick(object sender, EventArgs e)
        {
            recording_length_timer.Stop();
            //STOP RECORDING, IF NO MORE TASKS            
            if (CURRENT_MODE == CAMERA_MODES.NONE)
            {
                cameraMan.ReleaseInterfaces();
                Console.WriteLine("CAMERA_MODES.NONE at 1128");
                
            }else if (CURRENT_MODE==CAMERA_MODES.CAPTURE)
            {
                cameraMan.ReleaseInterfaces();
                ResumeSensor();
                pbRecording.Visible = false;
            }
            GetCameraInstance();
        }

        private void StopAllTimers()
        {
            mouse_down_timer.Stop();
            face_timer.Stop();
            datetime_ui_updater.Stop();
            recording_length_timer.Stop();
            backlight_timer.Stop();            
        }
                
        private void FillResolutionList()
        {
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////
            //Showing video formats
            for (int k = 0; k < videoFormat.Length; k++)
            {
                if (GetMainForm.UniqueVideoParameter(vf_resolutions, videoFormat[k].Size) != true)
                {
                    vf_resolutions.Add(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);
                    SettingsUI.SetComboBoxValues(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);                    
                }
                if (GetMainForm.UniqueFPS(videoFormat[k].TimePerFrame) == false)
                {
                    vf_fps.Add(videoFormat[k].TimePerFrame);
                }
            }

            //Properties.Settings.Default["C" + settingUI.Camera_number + "res"] = String.Join(",", vf_resolutions.ToArray());
            //Properties.Settings.Default["C" + settingUI.Camera_number + "f"] = String.Join(",", vf_fps.ToArray());
            Console.WriteLine("UniqueVideoParameter " + vf_resolutions.Count);
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////            
        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            
        }
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private bool UniqueFPS(long fps)
        {
            bool retval = false;
            for (int i = 0; i < vf_fps.Count; i++)
            {
                if (vf_fps[i] == fps)
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Event KEYBOARD touched
            if (Properties.Settings.Default.enable_event_recorder)
            {
                //turn on the firat camera and capture the user now
                OperatorCapture.Start();
            }
                        
            if (Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Stop();
                backlight_timer.Start();
            }
            BacklightOn();  

        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Stop();
                backlight_timer.Start();
            }
            BacklightOn();
        }
    }
    
}
