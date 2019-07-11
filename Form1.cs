using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Diagnostics;
using System.Configuration;
using System.IO;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        //User actions
        private Timer timer = new Timer();
        private static Timer capTimer = new Timer();
        private static Timer frameTimer = new Timer();
        private static Label testparam;
        private static MainForm mainForm;
        private static PictureBox pb_recording;
        private static bool recording_on;
        private static Label current_date_text;
        private static Label camera_num;

        //User actions end
        settingsUI settingUI;

        private VideoCapture _capture;

        private CascadeClassifier _cascadeClassifier;
        private Image<Bgr, Byte> imageFrame;
        
        public MainForm()
        {
            Debug.WriteLine(this.WindowState);
            settingUI = new settingsUI();
            Debug.WriteLine(this.Location);
            InitializeComponent();
            _capture = new VideoCapture();
            
            /*
             * img = new Image<Bgr, byte>(Application.StartupPath + "/faces.jpg");
             */
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_alt.xml");
            imgCamUser.SendToBack();

            timer.Tick += new EventHandler(ShowButtons);
            capTimer.Tick += new EventHandler(CaptureFace);
            capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            capTimer.Start();

            frameTimer.Interval = 1000/ Decimal.ToInt32(Properties.Settings.Default.frame_rate_fps);
            frameTimer.Start();
            frameTimer.Tick += new EventHandler(ProcessFrame);

            /*
             * Application.Idle += ProcessFrame;
             */
            pb_recording = pbRecording;
            pbRecording.BackColor = Color.Transparent;
            dateTimeLabel.BackColor = Color.Transparent;
            controlButtons.Parent = imgCamUser;
            testparam = testing_params;
            
            this.Location = new Point(Decimal.ToInt32(Properties.Settings.Default.display_pos_x), Decimal.ToInt32(Properties.Settings.Default.display_pos_y));

            this.Size = new Size(Decimal.ToInt32(Properties.Settings.Default.view_width), Decimal.ToInt32(Properties.Settings.Default.view_height));
            this.TopMost = Properties.Settings.Default.window_on_top;
            mainForm = this;
            windowBorderStyle();
            current_date_text = dateTimeLabel;
            currentDate();
            camera_num = camera_number;
            camera_num.Parent = imgCamUser;
            cameraNoShow();

        }
        private void ProcessFrame(object sender, EventArgs eventArgs)
        {
                imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();
                imgCamUser.Image = imageFrame;
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                dateTimeLabel.Parent = imgCamUser;
                pbRecording.Parent = imgCamUser;
                //Debug.WriteLine(this.Size);            
        }

        private void CaptureFace(object sender, EventArgs eventArgs)
        {
            if (imageFrame != null && Properties.Settings.Default.enable_face_recognition == true)
            {
                var grayframe = imageFrame.Convert<Gray, Byte>();
                var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); 
                //the actual face detection happens here
                foreach (var face in faces)
                {
                    imageFrame.Draw(face, new Bgr(Color.Red), 3); 
                    //the detected face(s) is highlighted here using a box that is drawn around it/them
                    //Debug.WriteLine(imageFrame);
                    imgCamUser.Image = imageFrame;
                }
            }
        }
        public static void handleParameters(IReadOnlyCollection<string> parameters)
        {
            Debug.WriteLine(parameters);
            testparam.Text = String.Concat(parameters);

        }
        public static void handleParameters(String[] parameters)
        {
            //Debug.WriteLine(parameters);
            testparam.Text = String.Concat(parameters);
        }
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
            if(timer.Enabled == true)
            {
                timer.Stop();
            }
            Debug.WriteLine(timer.ToString());
            if(folderButton.Visible==false)
            {
                controlButtons.Visible = true;
                /*
                folderButton.Visible = true;
                settingsButton.Visible = true;
                snapshotButton.Visible = true;
                cameraButton.Visible = true;
                closeButton.Visible = true;
    */
            }
            else
            {
                controlButtons.Visible = false;
      /*          folderButton.Visible = false;
                settingsButton.Visible = false;
                snapshotButton.Visible = false;
                cameraButton.Visible = false;
                closeButton.Visible = false;
                */
            }
        }
        public void holdButton(object sender, MouseEventArgs eventArgs)
        {
            timer.Enabled = true;            
            timer.Interval = 500;//Set it to 3000 for production
            timer.Start();
        }
        private void releaseButton(object sender, MouseEventArgs e)
        {
            if (timer.Enabled == true)
            {
                timer.Stop();
            }
        }
        private void CameraButton_Click(object sender, EventArgs e)
        {
            if (pbRecording.Visible == true)
            {
                pbRecording.Image = Properties.Resources.Pause_Normal_Red_icon;
                pbRecording.Visible = false;
                recording_on = false;
            }else
            {
                if(Properties.Settings.Default.show_recording_icon == true) {
                    pbRecording.Image = Properties.Resources.Record_Pressed_icon;
                    pbRecording.Visible = true;
                    recording_on = true;
                }
                
            }
 
        }
        public static void CameraButton_Click()
        {
            
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
        public static void currentDate()
        {
            current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
        }
        public static void cameraNoShow()
        {
            camera_num.Visible = Properties.Settings.Default.show_camera_no;
        }
        private void fullScreen(object sender, EventArgs eventArgs)
        {
            
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.TopMost = true;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    imgCamUser.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    this.TopMost = false;

                    this.WindowState = FormWindowState.Normal;
                if (Properties.Settings.Default.show_window_pane==true)
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

        private void showSettings(object sender, EventArgs e)
        {
            if(settingUI.Visible==false)
            {                
                settingUI.ShowDialog();
            }
            else
            {
            }
        }

        private void openStoreLocation(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Process.Start(Properties.Settings.Default.video_file_location);

            }catch(IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }
        public static void captureIntervalChange()
        {
            capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
        }
        public static void frameRateChange()
        {
            frameTimer.Interval = 1000/ Decimal.ToInt32(Properties.Settings.Default.frame_rate_fps);
        }
        public static void alwaysOnTop()
        {
            mainForm.TopMost = Properties.Settings.Default.window_on_top;
        }
        public static void windowBorderStyle()
        {
            if (Properties.Settings.Default.show_window_pane == true)
            {
                mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                mainForm.ControlBox = true;
            }
            else
            {
                mainForm.FormBorderStyle = FormBorderStyle.None;
            }
        }
        private void lastPositionUpdate(object sender, EventArgs e)
        {
            Properties.Settings.Default.display_pos_x = Convert.ToDecimal(this.Location.X);
            Properties.Settings.Default.display_pos_y = Convert.ToDecimal(this.Location.Y);
            Properties.Settings.Default.Save();
            Debug.WriteLine(Properties.Settings.Default.display_pos_x);
        }

        private void windowSizeUpdate(object sender, EventArgs e)
        {
            Properties.Settings.Default.view_width = Convert.ToDecimal(this.Width);
            Properties.Settings.Default.view_height = Convert.ToDecimal(this.Height);
            Properties.Settings.Default.Save();
            Debug.WriteLine(Properties.Settings.Default.view_width);
        }

    }
}
