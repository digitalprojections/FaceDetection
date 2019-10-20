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
        private readonly Timer timer = new Timer();
        private readonly Timer face_timer = new Timer();
        private readonly Timer datetime_ui_updater = new Timer();

        // Camera choice
        //private CameraChoice _CameraChoice = new CameraChoice();

        private static Label testparam;
        private static MainForm mainForm;
        private static PictureBox pb_recording;
        private static bool recording_on;
        private static Label current_date_text;
        private static Label camera_num;
        private static FlowLayoutPanel controlBut;
        private static Panel cameraPanel;
        //WebCam camera;
        UsbCamera usbCamera;

        //PROPERTY

        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }

        //User actions end
        static settingsUI settingUI;

        //readonly Thread t;
        static Form camform;
        bool initrec = false;

        public static Panel CameraPanel { get => cameraPanel; }
        public static MainForm GetMainForm { get => mainForm; }
        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();



            GitHub.secile.Video.UsbCamera.VideoFormat[] videoFormat = GitHub.secile.Video.UsbCamera.GetVideoFormat(0);

            //Showing video formats
            for (int k = 0; k < videoFormat.Length; k++)
            {
                Console.WriteLine(videoFormat[k].Caps.Guid);
            }

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
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
            }
            //Debug.WriteLine(this.Size);     

        }
        /// <summary>
        /// Capture face only works on a PC mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CaptureFace(object sender, EventArgs eventArgs)
        {

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

                        testparam.Text = String.Concat(parameters);

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
                                        mainForm.TopMost = false;
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
                               

                                    TakeSnapShot();
                                
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
                                    controlBut.Visible = true;
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    /*
                                 HIDE CONTROL BUTTONS    
                                 */
                                    controlBut.Visible = false;
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
                                    mainForm.TopMost = true;
                                    settingUI.TopMost = false;
                                    mainForm.Show();
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    mainForm.Hide();
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
                                    mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                                      
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
                                    testparam.Text = String.Concat(parameters);
                                    ManualRecordingOn();
                                }
                                else if (parameters.ElementAt(2) == "0")
                                {
                                    ManualRecordingOff();
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



        //kameyama comment 20191019 beginning

        private static void ApplicationClose()
        {
            
        }

        private static void WindowPane()
        {
            
        }

        private static void ManualRecordingOff()
        {
            
        }

        private static void ManualRecordingOn()
        {
            
        }

        private static void EventRecorder()
        {
            
        }

        private static void BacklightOff()
        {
           
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOff);

        }

        private static void BacklightOn()
        {
            SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOn);

        }
        //kameyama comment 20191019 end


        public static void HandleParameters(String[] parameters)
        {
            //Debug.WriteLine(parameters);
            testparam.Text = String.Concat(parameters);
        }
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
            //camera = new WebCam(panelCamera, Decimal.ToInt32(Properties.Camera1.Default.view_width), Decimal.ToInt32(Properties.Camera1.Default.view_height));

            usbCamera = new UsbCamera(0, new Size(1280, 720), 15, cameraPanel.Handle);
            usbCamera.Start();
            
            if (timer.Enabled == true)
            {
                timer.Stop();
            }
            //Debug.WriteLine(timer.ToString());
            if (folderButton.Visible == false)
            {
                controlButtons.Visible = true;
           }
            else
            {
                controlButtons.Visible = false;
            }
            ImgCamUser_Click();
        }
        public void HoldButton(object sender, MouseEventArgs eventArgs)
        {
            Console.WriteLine("mouse down");
            timer.Enabled = true;
            timer.Interval = 500;//Set it to 3000 for production
            timer.Start();
        }
        private void ReleaseButton(object sender, MouseEventArgs e)
        {
            if (timer.Enabled == true)
            {
                timer.Stop();
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
                Debug.WriteLine("topmost 414");
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
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
                Process.Start(Properties.Settings.Default.video_file_location);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }

        public static void FormChangesApply()
        {
            camera_num.Visible = Properties.Settings.Default.show_camera_no;
            current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            //capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            mainForm.TopMost = Properties.Settings.Default.window_on_top;
            //frameTimer.Interval = 1000 / Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
            if (Properties.Settings.Default.show_window_pane == true)
            {
                mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                mainForm.ControlBox = true;
            }
            else
            {
                mainForm.FormBorderStyle = FormBorderStyle.None;
            }
            if (pb_recording.Visible == true)
            {
                pb_recording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pb_recording.Visible = false;
            }
            else
            {
                //設定ウィンドウからの変更なので、recording_on かどうかによる表示する
                //recording_onがfalseの場合は表示する必要はない
                if (Properties.Settings.Default.show_recording_icon == true && recording_on == true)
                {
                    pb_recording.Image = Properties.Resources.Record_Pressed_icon;
                    pb_recording.Visible = true;
                }
            }
            Debug.WriteLine(pb_recording.Visible);
        }

        private void LastPositionUpdate(object sender, EventArgs e)
        {
            Properties.Settings.Default["C"+settingUI.Camera_number + "x" ] = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default["C" + settingUI.Camera_number + "y"] = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.Save();
                        
            
        }

        private void WindowSizeUpdate(object sender, EventArgs e)
        {
            Console.WriteLine(settingUI.Camera_number);
            if(settingUI.Camera_number != 0)
            {
                Console.WriteLine(Properties.Settings.Default["C" + settingUI.Camera_number + "w"]);
                Properties.Settings.Default["C" + settingUI.Camera_number + "w"] = Convert.ToDecimal(this.Width);
                Properties.Settings.Default["C" + settingUI.Camera_number + "h"] = Convert.ToDecimal(this.Height);
                Properties.Settings.Default.Save();
            }
            
            //Debug.WriteLine(Properties.Camera1.Default.view_width);
            if (usbCamera!=null)
            {
                usbCamera.SetWindowPosition(new Size(this.Width, this.Height));
            }
            
        }

        private void SnapShot(object sender, EventArgs e)
        {

            //timage.Dispose();
            TakeSnapShot();

        }

        private static void TakeSnapShot()
        {
            Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot");
            //Bitmap bitmap = camera.GetBitmapImage;
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            mainForm.usbCamera.GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot/" + imgdate + ".jpeg");
        }

       
        private void StartVideoRecording(object sender, EventArgs e)
        {
                if (pbRecording.Visible == true)
            {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;
                recording_on = false;
                initrec = false;
                //kameyama
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/movie");
                var moviedate = DateTime.Now.ToString("yyyyMMddHHmmss");
                mainForm.usbCamera.GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/movie/" + moviedate + ".mp4");
                //kameyama
            }
            else
            {                
                if (Properties.Settings.Default.show_recording_icon == true)
                {
                    pbRecording.Image = Properties.Resources.Record_Pressed_icon;
                    pbRecording.Visible = true;
                    recording_on = true;
                    //kameyama
                    Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/movie");
                    var moviedate = DateTime.Now.ToString("YYYYMMDDhhmmss");
                    mainForm.usbCamera.GetBitmap().Save(Properties.Settings.Default.video_file_location + "/Camera/1/movie/" + moviedate + ".mp4");
                    //kameyama
                }
                
                initrec = true;
            }

        }

        private void TakeStartVideoRecording()
        { }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Camera_number_Click(object sender, EventArgs e)
        {

        }

        private void ImgCamUser_Click()
        {
            
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

            UpdateCameraBitmap();

            // gui update
            UpdateGUIButtons();
        }
        private void Camera_OutputVideoSizeChanged(object sender, EventArgs e)
        {
            // Update camera's bitmap (new size needed)
            UpdateCameraBitmap();
            
        }
        private void UpdateCameraBitmap()
        {
            
        }
        private void UpdateUnzoomButton()
        {
           
        }
        private void UpdateGUIButtons()
        {
           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (settingUI == null)
            {

                settingUI = new settingsUI();
                timer.Tick += new EventHandler(ShowButtons);//制御ボタンの非/表示用クリックタイマー
                face_timer.Tick += new EventHandler(CaptureFace);
                face_timer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
                face_timer.Start();

                datetime_ui_updater.Interval = 1000;
                datetime_ui_updater.Start();
                datetime_ui_updater.Tick += new EventHandler(ProcessFrame);

            }

            cameraPanel = panelCamera;

            

            /*
             * Application.Idle += ProcessFrame;
             */
            pb_recording = pbRecording;
            pbRecording.BackColor = Color.Transparent;
            dateTimeLabel.BackColor = Color.Transparent;
            testparam = testing_params;

            //Set this CAMERA Dynamically to the relevant one
            if (settingUI.Camera_number!=0)
            {
                var camx = Properties.Settings.Default["C" + settingUI.Camera_number + "x"].ToString();
                var camy = Properties.Settings.Default["C" + settingUI.Camera_number + "y"].ToString();
                var camw = Properties.Settings.Default["C" + settingUI.Camera_number + "w"].ToString();
                var camh = Properties.Settings.Default["C" + settingUI.Camera_number + "h"].ToString();
                this.Location = new Point(Int32.Parse(camx), Int32.Parse(camy));
                //しばらく　画面サイズをキャプチャーサイズに合わせましょう
                //後で設定サイスに戻す！！！
                //
                this.Size = new Size(Int32.Parse(camw), Int32.Parse(camh));
                //this.Size = new Size(), Properties.Camera1.Default.view_height);
            }

            this.TopMost = Properties.Settings.Default.window_on_top;
            mainForm = this;
            current_date_text = dateTimeLabel;
            camera_num = camera_number;
            controlBut = controlButtons;

            //FormChangesApply();

            Debug.WriteLine(Convert.ToInt32(Properties.Settings.Default.backlight_offset_mins) + " WIDTH");
            camform = this;

            //var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/test.mp4";
            //var encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");

            camform.FormClosing += (s, ev) => {
                Debug.WriteLine("recording ended");
                //frameTimer.Stop();
            };

            // Fill camera list combobox with available cameras
            FillCameraList();

            
            // Fill camera list combobox with available resolutions
            FillResolutionList();
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
    }

}
