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


namespace FaceDetection
{
    public partial class mainForm : Form
    {
        //User actions
        private Timer timer = new Timer();

        //User actions end
        settingsUI settingUI;

        private VideoCapture _capture;

        private CascadeClassifier _cascadeClassifier;
        
        
        public mainForm(string[] vs)
        {
            settingUI = new settingsUI();
            Debug.WriteLine(vs + " run parameters");
            InitializeComponent();
            _capture = new VideoCapture();
            //img = new Image<Bgr, byte>(Application.StartupPath + "/faces.jpg");
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_alt.xml");
            imgCamUser.SendToBack();
            timer.Tick += new EventHandler(ShowButtons);

            Application.Idle += ProcessFrame;
            pbRecording.BackColor = Color.Transparent;
            dateTimeLabel.BackColor = Color.Transparent;

            if (vs.Length > 0)
            {
                label1.Text =vs[1];
            }   
        }
        private void ProcessFrame(object sender, EventArgs eventArgs)
        {
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray,Byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                    foreach (var face in faces)
                    {
                        imageFrame.Draw(face, new Bgr(Color.Red), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                    }
                }
                imgCamUser.Image = imageFrame;
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                dateTimeLabel.Parent = imgCamUser;
                pbRecording.Parent = imgCamUser;
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
            
            timer.Interval = 3000;
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
                
                settingUI.Show();
            }
            else
            {
            }
        }
    }
}
