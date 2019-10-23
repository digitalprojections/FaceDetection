using System;
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

        internal enum CAMERA_MODES
        {
            PREVIEW,
            CAPTURE,
            NONE,
           
        }

        internal CAMERA_MODES CURRENT_MODE = 0;

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


        private static Label or_testparam;
        private static MainForm or_mainForm;
        private static PictureBox or_pb_recording;
        private static bool or_recording_on;
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
        IRSensor rSensor = new IRSensor();
        //readonly Thread t;        
        bool initrec = false;
        private static UsbCamera.VideoFormat[] videoFormat = UsbCamera.GetVideoFormat(0);
        //SettingsUI resolutions data, generated each time. But set to the memory value
        private static List<string> vf_resolutions = new List<string>();
        private static List<long> vf_fps = new List<long>();
        //User actions end
        static settingsUI settingUI;
        static Form or_mainform;

        /// <summary>
        /// Sets the camera to the Recording mode
        /// </summary>
        internal void RecordMode()
        {
            if (GetCamera()!=null)
                GetCamera().Release();
            if (GetRecorder() == null)
            {
                
                GetCamcorderInstance();
                CURRENT_MODE = CAMERA_MODES.CAPTURE;
            }
            else
            {
                Console.WriteLine("Already in record mode: (1 true, 0 false) -> " + CURRENT_MODE);
            }

            if (Properties.Settings.Default.backlight_on_upon_face_rec)
            {
                BacklightOn();
            }
        }

        
        public static Panel CameraPanel
        {
            get
            {
                return GetMainForm.panelCamera;
            }            
        }
        public static MainForm GetMainForm
        {
            get
            {
                return or_mainForm;
            }
        }

        public static Label Or_testparam { get => or_testparam; set => or_testparam = value; }

        private static UsbCamera camera;

        private static CameraManager camcorder;
        private TaskManager taskManager;
       

        public static UsbCamera GetCamera()
        {           
            return camera;
        }
        internal CameraManager GetRecorder()
        {
            return camcorder;
        }

        private static void SetCamera(UsbCamera value)
        {
            camera = value;
        }

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            taskManager = new TaskManager();
            
            


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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CaptureFace(object sender, EventArgs eventArgs)
        {
            //TODO
            //Console.WriteLine("Face detection timer tick");
        }
        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {

           
                string param = String.Concat(parameters).ToLower();
            
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (param.Contains("uvccameraviewer"))
            {
                Debug.WriteLine(parameters + " at 135");
                if (parameters.Count > 0)
                {
                    //REMOVE THIS PIECE ON PRODUCTION
                    //||||||||||||||||||||||||||
                    try
                    {

                        or_testparam.Text = String.Concat(parameters);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString() + " 117rd line");
                    }
                    //|||||||||||||||||||||||||||
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

                                    MainForm.GetMainForm.TakeSnapShot();
                                
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
                                if (parameters.ElementAt(2) == "1")
                                {
                                    /*
                                 SHOW CONTROL BUTTONS    
                                 */
                                    or_controlBut.Visible = true;
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    /*
                                 HIDE CONTROL BUTTONS    
                                 */
                                    or_controlBut.Visible = false;
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
                                if (parameters.ElementAt(2) == "1")
                                {
                                    //FACE DETECTION
                                    if (parameters.Count > 2)
                                    {
                                        if (parameters.Count == 3)
                                        {
                                            //last one is interval
                                            Properties.Settings.Default.face_rec_interval = int.Parse(parameters.ElementAt(4));
                                            //all cameras whose status is ON must start FACE RECOGNITION
                                            FormChangesApply();
                                        }
                                        else
                                        {
                                            switch (parameters.ElementAt(3))
                                            {
                                                //CAMERA NUMBER
                                                case "1":
                                                    try
                                                    {
                                                        Properties.Settings.Default.face_rec_interval = int.Parse(parameters.ElementAt(4));
                                                        Properties.Settings.Default.enable_face_recognition = true;
                                                        FormChangesApply();
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        System.Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                case "2":
                                                    break;
                                                case "3":
                                                    break;
                                                case "4":

                                                    break;

                                            }
                                        }

                                    }
                                }
                                else if (parameters.ElementAt(2) == "0" && parameters.Count > 2)
                                {
                                    switch (parameters.ElementAt(3))
                                    {
                                        case "1":
                                            break;
                                        case "2":
                                            break;
                                        case "3":
                                            break;
                                        case "4":

                                            break;
                                        default:
                                            Debug.WriteLine(parameters.ElementAt(3) + " Camera number");
                                            break;
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 387");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        //kameyama beginning 20191018  
                        case "-v":
                            try
                            {
                                if (parameters.ElementAt(2) == "1")
                                {
                                    GetMainForm.TopMost = true;
                                    settingUI.TopMost = false;
                                    GetMainForm.Show();
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    GetMainForm.Hide();
                                    FormChangesApply();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        //kameyama End 20191018
                        //kameyama beginning 20191019
                        case "-l":
                            try
                            {
                                if (parameters.ElementAt(2) == "1")
                                {
                                    BacklightOn();

                                } else if (parameters.ElementAt(2) == "0")
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
                            try { }
                            catch { }
                            break;
                        case "-w":
                            try
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
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-q":
                            try {
                                Application.Exit();
                                //ApplicationClose();

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "-r":
                            try
                            {
                                if (parameters.ElementAt(2) == "1")
                                {
                                    GetMainForm.testing_params.Text = String.Concat(parameters);
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
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                     //kameyama End 20191019
                    }


                }

            }
        }

        internal void ResumeSensor()
        {
            rSensor.StartOM_Timer();
        }


        //kameyama comment 20191019 beginning

        private static void ApplicationClose()
        {
            
        }

        private static void WindowPane(bool value)
        {
            MainForm.GetMainForm.ControlBox = value;
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

        private static void BacklightOff()
        {
           
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOff);

        }

        public static void BacklightOn()
        {
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOn);

        }
        //kameyama comment 20191019 end


        public static void HandleParameters(String[] parameters)
        {

        }

        public static void GetCameraInstance()

        {
            if (GetCamera() != null)
                GetCamera().Release();

            

            SetCamera(new UsbCamera(settingUI.Camera_index, new Size(1280, 720), 15, CameraPanel.Handle));
            GetCamera().Start();
            GetMainForm.CURRENT_MODE = CAMERA_MODES.PREVIEW;

        }
        public void GetCamcorderInstance()
        {
            pbRecording.Image = Properties.Resources.Record_Pressed_icon;
            pbRecording.Visible = true;
            CURRENT_MODE = CAMERA_MODES.CAPTURE;

            //GetCamera().Start();
            //GetCamera().GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/movie/" + moviedate + ".mp4");
            //kameyama
            recording_length_timer.Start();
            or_recording_on = true;
            BacklightOn();
            backlight_timer.Stop();
            if (Properties.Settings.Default.backlight_off_timer)
            {
                backlight_timer.Start();
            }
            if (GetRecorder() != null)
                GetRecorder().Release();
            //TODO dynamically generate filename in the right path, for the right camera
            string dstFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".avi";
            //
            try
            {
                if (settingUI.Camera_index == 0)
                {                    
                    Properties.Settings.Default.Save();
                }
                //camcorder = new UsbCamcorder(0, new Size(1280, 720), 15, CameraPanel.Handle, dstFileName);             
                camcorder = new CameraManager(settingUI.Camera_index, new Size(1280, 720), 15, CameraPanel.Handle, dstFileName);
                CURRENT_MODE = CAMERA_MODES.CAPTURE;
            }
            catch(InvalidOperationException iox)
            {
                CURRENT_MODE = CAMERA_MODES.NONE;
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
        }
        public void HoldButton(object sender, MouseEventArgs eventArgs)
        {
            Console.WriteLine("mouse down");
            mouse_down_timer.Enabled = true;
            mouse_down_timer.Interval = 500;//Set it to 3000 for production            
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
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;

                Debug.WriteLine("topmost 223");
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
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Process.Start(Properties.Settings.Default.video_file_location);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }

        public static void FormChangesApply()
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
                    settingsUI.SetComboBoxValues(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);
                    Console.WriteLine(videoFormat[k].Size.Width);
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

            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = (Properties.Settings.Default.current_camera_index+1).ToString();
            
            or_current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            //capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            or_mainForm.TopMost = Properties.Settings.Default.window_on_top;
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
                if (Properties.Settings.Default.show_recording_icon == true && or_recording_on == true)
                {
                    or_pb_recording.Image = Properties.Resources.Record_Pressed_icon;
                    or_pb_recording.Visible = true;
                }
            }
            Debug.WriteLine(or_pb_recording.Visible);

            if (GetMainForm.CURRENT_MODE == CAMERA_MODES.CAPTURE)
            {
                GetMainForm.GetCamcorderInstance();
            }else if(GetMainForm.CURRENT_MODE == CAMERA_MODES.PREVIEW)
            {
                GetCameraInstance();
            }
            GetMainForm.WindowSizeUpdate();
        }

        private void LastPositionUpdate(object sender, EventArgs e)
        {
            Properties.Settings.Default["C"+settingUI.Camera_index + "x" ] = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default["C" + settingUI.Camera_index + "y"] = Convert.ToDecimal(this.Location.Y);
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

                Properties.Settings.Default["C" + settingUI.Camera_index + "w"] = Convert.ToDecimal(this.Width);
                Properties.Settings.Default["C" + settingUI.Camera_index + "h"] = Convert.ToDecimal(this.Height);
                Properties.Settings.Default.Save();
            }

            //Debug.WriteLine(Properties.Camera1.Default.view_width);
            if (GetCamera() != null)
            {
                GetCamera().SetWindowPosition(new Size(this.Width, this.Height));
            }
            else if (camcorder != null)
            {
                camcorder.SetWindowPosition(new Size(this.Width, this.Height));
            }
        }

        private void SnapShot(object sender, EventArgs e)
        {

            //timage.Dispose();
            TakeSnapShot();

        }

        internal void TakeSnapShot()
        {
            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "SNAPSHOT");
            //picloc = Path.Combine(picloc, (CAMERA_INDEX).ToString());
            Directory.CreateDirectory(picloc + "/" + (settingUI.Camera_index + 1).ToString());
            //Bitmap bitmap = camera.GetBitmapImage;
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            //two versions here. Depending on camera mode
            if (CURRENT_MODE==CAMERA_MODES.CAPTURE)
            {
                GetRecorder().GetBitmap().Save(picloc + "/"+ (settingUI.Camera_index + 1).ToString() + "/" + imgdate + ".jpeg");
            }
            else if(CURRENT_MODE == CAMERA_MODES.PREVIEW)
            {
                GetCamera().GetBitmap().Save(picloc + "/" + (settingUI.Camera_index + 1).ToString() + "/" + imgdate + ".jpeg");
            }
            
        }

       
        private void StartVideoRecording(object sender, EventArgs e)
        {
            TakeStartVideoRecording();
        }

        private void TakeStartVideoRecording()
        {
            if (pbRecording.Visible == true && CURRENT_MODE == CAMERA_MODES.CAPTURE)
            {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;
                or_recording_on = false;
                CURRENT_MODE = CAMERA_MODES.NONE;
                initrec = false;
                //kameyama
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/movie");
                var moviedate = DateTime.Now.ToString("yyyyMMddHHmmss");
                camcorder.Release();
                //GetCamera().GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/movie/" + moviedate + ".mp4");
                //kameyama
                GetCameraInstance();
            }
            else if (CURRENT_MODE==CAMERA_MODES.PREVIEW)
                {
                    
                    
                //kameyama
                    GetCamera().Release();
                    GetCamcorderInstance();
                    //GetCamera().Start();
                    //GetCamera().GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/movie/" + moviedate + ".mp4");
                    //kameyama
                    
                }

                initrec = true;
        
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();
            if (GetCamera() != null)
            {
                GetCamera().Release();
            }
                
            if (GetRecorder() != null)
            {
                GetRecorder().Release();
            }
                
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
            CURRENT_MODE = CAMERA_MODES.NONE;
            if (settingUI == null)
            {
                settingUI = new settingsUI();
                mouse_down_timer.Tick += new EventHandler(ShowButtons);//制御ボタンの非/表示用クリックタイマー
                face_timer.Tick += new EventHandler(CaptureFace);
                face_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
                face_timer.Start();
                datetime_ui_updater.Interval = 1000;
                datetime_ui_updater.Start();
                datetime_ui_updater.Tick += new EventHandler(ProcessFrame);

                recording_length_timer.Interval = decimal.ToInt32(Properties.Settings.Default.seconds_after_event)*1000;
                recording_length_timer.Tick += Recording_length_timer_Tick;

                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 1000;
                backlight_timer.Tick += Backlight_timer_Tick;
                if (Properties.Settings.Default.backlight_off_timer)
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
            or_testparam = testing_params;

            //Set this CAMERA Dynamically to the relevant one
            if (settingUI.Camera_index!=0)
            {
                var camx = Properties.Settings.Default["C" + settingUI.Camera_index+1 + "x"].ToString();
                var camy = Properties.Settings.Default["C" + settingUI.Camera_index+1 + "y"].ToString();
                var camw = Properties.Settings.Default["C" + settingUI.Camera_index+1 + "w"].ToString();
                var camh = Properties.Settings.Default["C" + settingUI.Camera_index+1 + "h"].ToString();
                this.Location = new Point(Int32.Parse(camx), Int32.Parse(camy));
                //しばらく　画面サイズをキャプチャーサイズに合わせましょう
                //後で設定サイスに戻す！！！
                //
                this.Size = new Size(Int32.Parse(camw), Int32.Parse(camh));
                //this.Size = new Size(), Properties.Camera1.Default.view_height);
            }

            this.TopMost = Properties.Settings.Default.window_on_top;
            //SET Object references
            or_mainForm = this;
            or_current_date_text = or_dateTimeLabel;
            
            or_controlBut = or_controlButtons;

            //FormChangesApply();

            Debug.WriteLine(Convert.ToInt32(Properties.Settings.Default.backlight_offset_mins) + " WIDTH");
            or_mainform = this;

            //var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/test.mp4";
            //var encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");

            or_mainform.FormClosing += (s, ev) => {
                Debug.WriteLine("recording ended");
                StopAllTimers();
            };

            // Fill camera list combobox with available cameras
            FillCameraList();

            
            // Fill camera list combobox with available resolutions
            FillResolutionList();            
            GetCameraInstance();

            if (Properties.Settings.Default.window_on_top)
            {
                FullScreen(this, null);
            }

            //WindowSizeUpdate();

            ResumeSensor();


            
        }

        private void Backlight_timer_Tick(object sender, EventArgs e)
        {
            //BacklightOff();
        }

        private void Recording_length_timer_Tick(object sender, EventArgs e)
        {
            recording_length_timer.Stop();
            //STOP RECORDING, IF NO MORE TASKS
            taskManager.StatusCheck();
          
        }

        private void StopAllTimers()
        {
            mouse_down_timer.Stop();
            face_timer.Stop();
            datetime_ui_updater.Stop();
            recording_length_timer.Stop();
            backlight_timer.Stop();            
        }

        private void FillCameraList()
        {
            

            //_CameraChoice.UpdateDeviceList();

            //Debug.WriteLine(_CameraChoice.Devices.Count);
        }
        private void FillResolutionList()
        {            

            
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
    }
    
}
