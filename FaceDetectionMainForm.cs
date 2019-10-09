using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DirectShowLib;
using System.Runtime.InteropServices.ComTypes;


namespace FaceDetection
{
    public partial class MainForm : Form
    {
        //User actions
        private readonly Timer timer = new Timer();
        private readonly static Timer capTimer = new Timer();
        private readonly static Timer frameTimer = new Timer();
        
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

        //User actions end
        static settingsUI settingUI;
                
        //readonly Thread t;
        static Form camform;
        bool initrec = false;
        
        string fileName = string.Format("video_out.mp4");
        int backend_idx = 0; //any backend;

        public static Panel CameraPanel { get => cameraPanel;}
        public static MainForm GetMainForm { get => mainForm;}

        //readonly String fileName = String.Format("c:\video_out.mp4");
        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            //this.cameraControl = new Camera_NET.CameraControl();
            if (settingUI == null)
            {

                settingUI = new settingsUI();
                timer.Tick += new EventHandler(ShowButtons);//制御ボタンの非/表示用クリックタイマー
                capTimer.Tick += new EventHandler(CaptureFace);
                capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
                capTimer.Start();

                frameTimer.Interval = 1000 / Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
                frameTimer.Start();
                frameTimer.Tick += new EventHandler(ProcessFrame);

            }

            cameraPanel = panelCamera;
            

            /*
             * Application.Idle += ProcessFrame;
             */
            pb_recording = pbRecording;
            pbRecording.BackColor = Color.Transparent;
            dateTimeLabel.BackColor = Color.Transparent;
            testparam = testing_params;

            this.Location = new Point(Decimal.ToInt32(Properties.Camera1.Default.pos_x), Decimal.ToInt32(Properties.Camera1.Default.pos_y));
            //しばらく　画面サイズをキャプチャーサイズに合わせましょう
            //後で設定サイスに戻す！！！
            this.Size = new Size(Decimal.ToInt32(Properties.Camera1.Default.view_width), Decimal.ToInt32(Properties.Camera1.Default.view_height));
            //this.Size = new Size(), Properties.Camera1.Default.view_height);

            this.TopMost = Properties.Settings.Default.window_on_top;
            Debug.WriteLine("TOPMOST 80");
            mainForm = this;
            
            current_date_text = dateTimeLabel;
            camera_num = camera_number;
            controlBut = controlButtons;
            FormChangesApply();

            if (vs != null && vs.Count() > 0)
            {
                HandleParameters(vs);
            }

            Debug.WriteLine(Convert.ToInt32(Properties.Camera1.Default.view_width) + " WIDTH");
            camform = this;

            //var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/test.mp4";
            //var encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");

            camform.FormClosing += (s, ev) => {
                Debug.WriteLine("recording ended");
                frameTimer.Stop();
            };
        }

        private void ProcessFrame(object sender, EventArgs eventArgs)
        {

            try
            {
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                

                if(initrec == true)
                {
                   
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
            }
            //Debug.WriteLine(this.Size);     

        }

        private void CaptureFace(object sender, EventArgs eventArgs)
        {
            
        }
        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (parameters.Contains("uvccameraviewer"))
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
                        Debug.WriteLine(e.ToString() + " 243rd line");
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
                                if (parameters.ElementAt(2) == "1")
                                {
                                    /*
                                 SNAPSHOT CODE HERE    
                                 */
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
                                else
                                {
                                    //FACE OFF FOR CAMERA
                                    if (parameters.Count > 2)
                                    {
                                        switch (parameters.ElementAt(3))
                                        {
                                            case "1":
                                                Properties.Settings.Default.enable_face_recognition = false;
                                                //STOP FACE RECOGNITION IF ON
                                                FormChangesApply();
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
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 387");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                    }
                }
            }
            else if (parameters.Count >= 0)//!!!!!!!!!!!! temporary error check, fix it to Count > 0
            {

                switch (parameters.ElementAt(0))
                {
                    case "-c":
                        try
                        {
                            if (parameters.ElementAt(1) == "1")
                            {
                                if (settingUI != null && settingUI.Visible == false)
                                {
                                    settingUI.TopMost = true;
                                    mainForm.TopMost = false;
                                    settingUI.Show();
                                    Debug.WriteLine(settingUI);
                                    Debug.WriteLine(mainForm);
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
                            Debug.WriteLine(e.ToString() + " 421st line");
                            //MessageBox.Show("Incorrect or missing parameters");
                        }
                        break;
                }

            }
        }
        public static void HandleParameters(String[] parameters)
        {
            //Debug.WriteLine(parameters);
            testparam.Text = String.Concat(parameters);
        }
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
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
            Debug.WriteLine("mouse down");
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

            Debug.WriteLine(sender);
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
            capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            mainForm.TopMost = Properties.Settings.Default.window_on_top;
            frameTimer.Interval = 1000 / Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
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
            Properties.Camera1.Default.pos_x = Convert.ToDecimal(this.Location.X);
            Properties.Camera1.Default.pos_y = Convert.ToDecimal(this.Location.Y);
            Properties.Camera1.Default.Save();
            Debug.WriteLine(Properties.Camera1.Default.pos_x);
        }

        private void WindowSizeUpdate(object sender, EventArgs e)
        {
            Properties.Camera1.Default.view_width = Convert.ToDecimal(this.Width);
            Properties.Camera1.Default.view_height = Convert.ToDecimal(this.Height);
            Properties.Camera1.Default.Save();
            Debug.WriteLine(Properties.Camera1.Default.view_width);
        }

        private void SnapShot(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot");
            //var timage = imaMat.ToImage<Bgr, Byte>();
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            //timage.Save(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot/" + imgdate + ".jpeg");
            //timage.Dispose();
        }
        private void StartVideoRecording(object sender, EventArgs e)
        {
            if (pbRecording.Visible == true)
            {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;
                recording_on = false;
                initrec = false;
            }
            else
            {                
                if (Properties.Settings.Default.show_recording_icon == true)
                {
                    pbRecording.Image = Properties.Resources.Record_Pressed_icon;
                    pbRecording.Visible = true;
                    recording_on = true;
                }
                
                initrec = true;
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Camera_number_Click(object sender, EventArgs e)
        {

        }

        private void ImgCamUser_Click()
        {
            //CameraChoice _CameraChoice = new CameraChoice();
            //_CameraChoice.UpdateDeviceList();
            //IMoniker moniker = _CameraChoice.Devices[0].Mon;
            //Debug.WriteLine(cameraControl.Moniker);
            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            //moniker.BindToObject(null, null, ref iid, out source);
            IBaseFilter theDevice = (IBaseFilter)source;
            //SetCamera(moniker, null, null);

            //TEMP code to demo camera properties window
            /*
            if (cameraControl.CameraCreated)
            {
                Camera.DisplayPropertyPage_Device(cameraControl.Moniker, this.Handle);                
            }
            */
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
    }
}
