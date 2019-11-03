using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DirectShowLib;
using FaceDetectionX;
using System.Configuration;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        //System.Timers.Timer event_timer = new System.Timers.Timer();

        UsbCamcorder usbCamcorder;
        UsbCamera usbCamera;
        private RecorderCamera cameraMan = null;

        private readonly System.Timers.Timer mouse_down_timer = new System.Timers.Timer();
        private readonly Timer datetime_ui_updater = new Timer();
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
        
        //kameyama camera
        //public static Camera CheckCamera;
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
        //public static Panel CameraPanel => GetMainForm.panelCamera;
        public static MainForm GetMainForm => or_mainForm;
        public static SettingsUI SettingUI { get => settingUI; set => settingUI = value; }
        internal static IRSensor RSensor { get => rSensor; set => rSensor = value; }
        public static FlowLayoutPanel Or_controlBut { get => or_controlBut; set => or_controlBut = value; }
        public static PictureBox Or_pb_recording { get => or_pb_recording; set => or_pb_recording = value; }
        

        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            InitializeComponent();
            cameraMan = new RecorderCamera(0);
            //keyboardListener = new KeyboardListener();
            //mouseListener = new MouseListener();
            RSensor = new IRSensor();
            if (vs != null && vs.Count() > 0)
            {
                PARAMETERS.HandleParameters(vs);
            }
            BackLightController.Init();
            /*
            event_timer.Enabled = true;
            event_timer.Interval = 1000;
            event_timer.Elapsed += Event_timer_Tick;
            event_timer.AutoReset = false;
            event_timer.SynchronizingObject = this;
            */
            
        }
        public Size GetResolution(int cam_ind)
        {
            Size retval;
            string[] res;
            switch (cam_ind)
            {
                case 0:
                    res = Properties.Settings.Default.C1res.Split('x');
                    retval = new Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 1:
                    res = Properties.Settings.Default.C2res.Split('x');
                    retval = new Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 2:
                    res = Properties.Settings.Default.C3res.Split('x');
                    retval = new Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                case 3:
                    res = Properties.Settings.Default.C4res.Split('x');
                    retval = new Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    return retval;
                default: return new Size(640, 480);
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
        private void Event_timer_Tick(object sender, EventArgs e)
        {
            //mouseListener.MouseMove += MouseListener_MouseMove;
        }

        //internal Bitmap GetSnapShot()
        //{
        //    //return cameraMan.GetBitmap();
        //}
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
            //==============digital-projects=================\
            //------------------------------------------------\
            //*********** *******  ***   ****   ***  ********||\
            //*********** ******* ** **  ** ** ** ** ********||:\
            //***********   ***   ** **  ** ** ** ** ********||:/
            //***********   ***    ***   ****   ***  ********||/
            //________________________________________________/
            //==============digital-projects=================/
            //Console.WriteLine("Face detection timer tick");
        }
        /// <summary>
        /// Resumes IRsensor check cycle
        /// </summary>
        internal void ResumeSensor()
        {
            if(Properties.Settings.Default.capture_operator)
            {
                RSensor.StartOM_Timer();
            }
        }
        public static void HandleParameters(String[] parameters)
        {

        }
        /// <summary>
        /// PREVIEW MODE CAMERA
        /// </summary>
        public static void StartTheCamera()
        {
            or_camera_num_txt.Visible = true;            
            Or_pb_recording.Visible = false;
            //==============digital-projects=================\
            //------------------------------------------------\
            //*********** *******  ***   ****   ***  ********||\
            //*********** ******* ** **  ** ** ** ** ********||:\
            //***********   ***   ** **  ** ** ** ** ********||:/
            //***********   ***    ***   ****   ***  ********||/
            //________________________________________________/
            //==============digital-projects=================/
            //MainForm.GetMainForm.cameraMan.StartCamera(MainForm.GetMainForm.Handle);
        }
        /// <summary>
        /// RECORDER CAMERA
        /// Dynamically generate filename in the right path, for the right camera
        /// Checking for the active path that will make sure the path is fitting the recording type
        /// </summary>
        public void StartTheRecorder()
        {
            if (!MainForm.GetMainForm.usbCamcorder.ON)
            {
                //if (MainForm.GetMainForm.usbCamera.ON)
                //{
                //    MainForm.GetMainForm.usbCamera.Release();
                //}
                MainForm.GetMainForm.usbCamcorder.Release();

                MainForm.GetMainForm.usbCamcorder = new UsbCamcorder(0);
                MainForm.GetMainForm.usbCamcorder.Start();
            }

            ////CURRENT_MODE = CAMERA_MODES.CAPTURE;
            //or_camera_num_txt.Visible = false;
            //Or_pb_recording.Image = Properties.Resources.Record_Pressed_icon;
            //Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
            //BackLightController.ON();
            //Console.WriteLine("current mode 736");

            ////Initialize the CAPTURE camera by index
            //try
            //{
            //    if((String)cameraButton.Tag == "1")
            //    {
            //        cameraButton.Tag = "0";
            //        //cameraMan.Start(RecorderCamera.CAMERA_MODES.PREVIEW);
            //    }else
            //    {
            //        cameraButton.Tag = "1";
            //        //cameraMan.Start(RecorderCamera.CAMERA_MODES.MANUAL);
            //    }

            //}
            //catch(InvalidOperationException iox)
            //{                
            //    Logger.Add(iox);                
            //}
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
        
        private void LastPositionUpdate(object sender, EventArgs e)
        {
            Console.WriteLine(this.Location.X);
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
            if (SettingUI != null)
            {
                Properties.Settings.Default.C1w = Convert.ToDecimal(this.Width);
                Properties.Settings.Default.C1h = Convert.ToDecimal(this.Height);
                //cameraMan.SetWindowPosition(new Size(this.Width, this.Height));
            }
        }
        private void SnapShot(object sender, EventArgs e)
        {

            //timage.Dispose();
            SNAPSHOT_SAVER.TakeSnapShot();

        }
        /// <summary>
        /// Manual Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartVideoRecording(object sender, EventArgs e)
        {   
            StartVideoRecording();
        }
        private void StartVideoRecording()
        {
             StartTheRecorder();
        }
        private void StopVideoRecording()
        {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;
                StartTheCamera();
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllTimers();
            //cameraMan.ReleaseInterfaces();
            Console.WriteLine(this.Location.X);
            Properties.Settings.Default.C1x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.C1y = Convert.ToDecimal(this.Location.Y);            
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        int tempmode = 0;
        private void KeyDownAllEventHandler(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.KeyCode.ToString());
            //keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            //mouseListener.MouseMove -= MouseListener_MouseMove;
            e.Handled = true;
            //cameraMan.Start(RecorderCamera.CAMERA_MODES.EVENT);

            
        }
        
        
        private void MouseListener_MouseMove(object sender, MouseEventArgs e)
        {
            keyboardListener.KeyDownAll -= new KeyEventHandler(KeyDownAllEventHandler);
            mouseListener.MouseMove -= MouseListener_MouseMove;
            
            //cameraMan.Start(RecorderCamera.CAMERA_MODES.EVENT);
            //BackLightController.Restart();
            if (tempmode == 0)
            {
                //cameraMan.PreviewMode();
                tempmode = 1;                
            }
            else
            {
                tempmode = 0;
                //cameraMan.RecordingMode();
            }
            Console.WriteLine("mouse action");
            //event_timer.Enabled = true;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            //keyboardListener.KeyDownAll += new KeyEventHandler(KeyDownAllEventHandler);
            //mouseListener.MouseMove += MouseListener_MouseMove;

            if (SettingUI == null)
            {
                SettingUI = new SettingsUI();
                mouse_down_timer.Elapsed += ShowButtons;//制御ボタンの非/表示用クリックタイマー
                datetime_ui_updater.Interval = 1000;
                datetime_ui_updater.Start();
                datetime_ui_updater.Tick += new EventHandler(UpdateDateTimeText);
                                
                or_camera_num_txt = camera_number_txt;
                or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
                or_camera_num_txt.Text = "Y";
            }
            
            Or_pb_recording = pbRecording;

            ////////////////////////////////////////////////////
            ////////////////////////////////////////////////////
            //Set this CAMERA Dynamically to the relevant one///
            ////////////////////////////////////////////////////
            var camx = Properties.Settings.Default.C1x.ToString();
            var camy = Properties.Settings.Default.C1y.ToString();
            var camw = Properties.Settings.Default.C1w.ToString();
            var camh = Properties.Settings.Default.C2h.ToString();
            this.Location = new Point(Int32.Parse(camx), Int32.Parse(camy));
            this.Size = new Size(Int32.Parse(camw), Int32.Parse(camh));
            ////////////////////////////////////////////////////
            ////////////////////////////////////////////////////
            ////////////////////////////////////////////////////

            this.TopMost = Properties.Settings.Default.window_on_top;
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
            this.Size = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));

            usbCamcorder = new UsbCamcorder(0);
            usbCamcorder.Start();
            //usbCamera = new UsbCamera(0);
            //usbCamera.Start();

            AllChangesApply();
            WindowSizeUpdate();
            WindowPositionUpdate();
            Camera.SetNumberOfCameras();            
            // Fill camera list combobox with available resolutions
            FillResolutionList();
            //BackLightController.Start(decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 10000, 1000);
            
        }
        public static void AllChangesApply()
        {
            or_camera_num_txt.Visible = Properties.Settings.Default.show_camera_no;
            or_camera_num_txt.Text = "X";
            MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            
            if (Properties.Settings.Default.main_window_full_screen)
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Normal;
            }
            MainForm.GetMainForm.Size = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));
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
                Or_pb_recording.Image = Properties.Resources.Pause_Normal_Red_icon;
                Or_pb_recording.Visible = false;
            }
            
            //Also must check if the Preevent mode is needed
            if (!AtLeastOnePreEventTimeIsNotZero())
            {
                //MainForm.GetMainForm.cameraMan.Start(RecorderCamera.CAMERA_MODES.PREEVENT);

                
                
            }
            else
            {
                //MainForm.GetMainForm.cameraMan.Start(RecorderCamera.CAMERA_MODES.PREVIEW);
                if (Properties.Settings.Default.capture_operator)
                {
                    MainForm.GetMainForm.ResumeSensor();
                    Console.WriteLine("IR sensor ON");

                    //if(!MainForm.GetMainForm.usbCamera.ON)
                    //{
                    //    if (MainForm.GetMainForm.usbCamcorder.ON)
                    //    {
                    //        MainForm.GetMainForm.usbCamcorder.Release();
                    //    }
                    //    MainForm.GetMainForm.usbCamera.Start();
                    //}
                    
                }
            }

            Debug.WriteLine(Or_pb_recording.Visible);
            GC.Collect();
        }
        private void WindowPositionUpdate()
        {
            Location = new Point(Convert.ToInt32(Properties.Settings.Default.C1x), Convert.ToInt32(Properties.Settings.Default.C1y));
            Console.WriteLine(Properties.Settings.Default.C1x);            
        }
        
        public void StopAllTimers()
        {
            mouse_down_timer.Stop();
            datetime_ui_updater.Stop();
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
            //Properties.Settings.Default["C" + settingUI.Camera_number + "res"] = String.Join(",", vf_resolutions.ToArray());
            //Properties.Settings.Default["C" + settingUI.Camera_number + "f"] = String.Join(",", vf_fps.ToArray());
            Console.WriteLine("UniqueVideoParameter " + vf_resolutions.Count);
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

        #region DLL IMPORTS

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);

        #endregion DLL IMPORTS
    }

}
