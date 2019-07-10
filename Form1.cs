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
    public partial class mainForm : Form
    {
        //User actions
        private Timer timer = new Timer();
        private static Timer capTimer = new Timer();
        private static Timer frameTimer = new Timer();

        //User actions end
        settingsUI settingUI;

        private VideoCapture _capture;

        private CascadeClassifier _cascadeClassifier;
        private Image<Bgr, Byte> imageFrame;

        public mainForm(string[] vs)
        {
            this.Location = new Point(int.Parse(Properties.Settings.Default.display_pos_x), int.Parse(Properties.Settings.Default.display_pos_y));
            this.Width = int.Parse(Properties.Settings.Default.view_width);
            this.Height = int.Parse(Properties.Settings.Default.display_pos_y);

            settingUI = new settingsUI();
            Debug.WriteLine(vs + " run parameters");
            InitializeComponent();
            _capture = new VideoCapture();
            
            //img = new Image<Bgr, byte>(Application.StartupPath + "/faces.jpg");
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_alt.xml");
            imgCamUser.SendToBack();
            timer.Tick += new EventHandler(ShowButtons);
            capTimer.Tick += new EventHandler(CaptureFace);
            capTimer.Interval = int.Parse(Properties.Settings.Default.face_rec_interval);//milliseconds
            capTimer.Start();
            frameTimer.Interval = 1000/int.Parse(Properties.Settings.Default.frame_rate_fps);
            frameTimer.Start();
            frameTimer.Tick += new EventHandler(ProcessFrame);
            //Application.Idle += ProcessFrame;
            pbRecording.BackColor = Color.Transparent;
            dateTimeLabel.BackColor = Color.Transparent;

            if (vs.Length > 0)
            {
                label1.Text =vs[1];
            }   
        }
        private void ProcessFrame(object sender, EventArgs eventArgs)
        {
            imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();
            
                
                imgCamUser.Image = imageFrame;
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                dateTimeLabel.Parent = imgCamUser;
                pbRecording.Parent = imgCamUser;
            



        }

        private void CaptureFace(object sender, EventArgs eventArgs)
        {
            if (imageFrame != null && Properties.Settings.Default.enable_face_recognition == true)
            {
                var grayframe = imageFrame.Convert<Gray, Byte>();
                var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                foreach (var face in faces)
                {
                    imageFrame.Draw(face, new Bgr(Color.Red), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                }
            }
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
                folderButton.Visible = true;
                settingsButton.Visible = true;
                snapshotButton.Visible = true;
                cameraButton.Visible = true;
                closeButton.Visible = true;
            }
            else
            {
                folderButton.Visible = false;
                settingsButton.Visible = false;
                snapshotButton.Visible = false;
                cameraButton.Visible = false;
                closeButton.Visible = false;

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
            }else
            {
                pbRecording.Image = Properties.Resources.Record_Pressed_icon;
                pbRecording.Visible = true;
            }
 
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
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
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
            capTimer.Interval = int.Parse(Properties.Settings.Default.face_rec_interval);//milliseconds
        }
        public static void frameRateChange()
        {
            frameTimer.Interval = 1000/int.Parse(Properties.Settings.Default.frame_rate_fps);
        }
    }
}
