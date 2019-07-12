﻿using System;
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
        private static FlowLayoutPanel controlBut;

        //User actions end
        

        static settingsUI settingUI;

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

            frameTimer.Interval = 1000/ Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
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
            
            this.Location = new Point(Decimal.ToInt32(Properties.Camera1.Default.pos_x), Decimal.ToInt32(Properties.Camera1.Default.pos_y));

            this.Size = new Size(Decimal.ToInt32(Properties.Camera1.Default.view_width), Decimal.ToInt32(Properties.Camera1.Default.view_height));
            this.TopMost = Properties.Settings.Default.window_on_top;
            Debug.WriteLine("TOPMOST 78");
            mainForm = this;
            
            current_date_text = dateTimeLabel;
            camera_num = camera_number;
            camera_num.Parent = imgCamUser;
            controlBut = controlButtons;
            formChangesApply();
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
            //Debug.WriteLine(parameters);
            
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

                }
                //|||||||||||||||||||||||||||
                    switch (parameters.ElementAt(1))
                {
                    case "-c":
                        try
                        {
                            if (parameters.ElementAt(2) == "1")
                            {
                                if (settingUI.Visible == false)
                                {
                                    settingUI.TopMost = true; mainForm.TopMost = false; settingUI.ShowDialog();
                                }                                
                            }
                            else
                            {
                                settingUI.Close();
                                formChangesApply();
                            }
                        }catch(ArgumentOutOfRangeException e)
                        {
                            //MessageBox.Show("Incorrect or missing parameters");
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
                            //MessageBox.Show("Incorrect or missing parameters");
                        }
                        break;
                    case "-d":
                        try
                        {
                            if (parameters.ElementAt(2) == "1")
                            {
                                //FACE DETECTION
                                if (parameters.Count>2)
                                {
                                    if (parameters.Count == 3)
                                    {
                                        //last one is interval
                                        Properties.Settings.Default.face_rec_interval = int.Parse(parameters.ElementAt(4));
                                        //all cameras whose status is ON must start FACE RECOGNITION
                                        formChangesApply();
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
                                                    formChangesApply();
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
                                            formChangesApply();
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
                            //MessageBox.Show("Incorrect or missing parameters");
                        }
                        break;
                }
            }

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
        
        
        private void fullScreen(object sender, EventArgs eventArgs)
        {
            
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.TopMost = true;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    imgCamUser.SizeMode = PictureBoxSizeMode.Zoom;
                Debug.WriteLine("topmost 223");
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
                settingUI.TopMost = true;
                this.TopMost = false;
                Debug.WriteLine("topmost 251");
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
        
        public static void formChangesApply()
        {
            camera_num.Visible = Properties.Settings.Default.show_camera_no;
            current_date_text.Visible = Properties.Settings.Default.show_current_datetime;
            capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
            mainForm.TopMost = Properties.Settings.Default.window_on_top;
            frameTimer.Interval = 1000/ Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
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
        
        private void lastPositionUpdate(object sender, EventArgs e)
        {
            Properties.Camera1.Default.pos_x = Convert.ToDecimal(this.Location.X);
            Properties.Camera1.Default.pos_y = Convert.ToDecimal(this.Location.Y);
            Properties.Camera1.Default.Save();
            Debug.WriteLine(Properties.Camera1.Default.pos_x);
        }

        private void windowSizeUpdate(object sender, EventArgs e)
        {
            Properties.Camera1.Default.view_width = Convert.ToDecimal(this.Width);
            Properties.Camera1.Default.view_height = Convert.ToDecimal(this.Height);
            Properties.Camera1.Default.Save();
            Debug.WriteLine(Properties.Camera1.Default.view_width);
        }

    }
}
