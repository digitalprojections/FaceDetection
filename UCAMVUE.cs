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
using System.Threading;

namespace FaceDetection
{    
    public partial class MainForm : Form
    {
        //User actions
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private static System.Windows.Forms.Timer capTimer = new System.Windows.Forms.Timer();
        private static System.Windows.Forms.Timer frameTimer = new System.Windows.Forms.Timer();
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
        private VideoWriter videoWriter;

        private CascadeClassifier _cascadeClassifier;
        private CascadeClassifier _cascadeClassifierEyes;
        private CascadeClassifier _cascadeClassifierBody;
        private static Image<Bgr, Byte> imageFrame;
        private Mat imaMat;
        
        int index = 0;
        static GitHub.secile.Video.UsbCamera camera;
        Thread t;
        static Form camform;

        //private OpenH264Lib.Encoder encoder;

        String fileName = String.Format("c:\video_out.mp4");
        public MainForm(IReadOnlyCollection<string> vs = null)
        {
            //Debug.WriteLine(this.WindowState);
            //Debug.WriteLine(this.Location);
            InitializeComponent();
            
            if (settingUI == null)
            {
                settingUI = new settingsUI();
                _capture = new VideoCapture();
                
                _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_alt2.xml");
                _cascadeClassifierEyes = new CascadeClassifier(Application.StartupPath + "/haarcascade_righteye_2splits.xml");
                _cascadeClassifierBody = new CascadeClassifier(Application.StartupPath + "/haarcascade_fullbody.xml");
                timer.Tick += new EventHandler(ShowButtons);
                //capTimer.Tick += new EventHandler(CaptureFace);
                capTimer.Interval = Decimal.ToInt32(Properties.Settings.Default.face_rec_interval);//milliseconds
                capTimer.Start();

                frameTimer.Interval = 1000 / Decimal.ToInt32(Properties.Camera1.Default.frame_rate);
                frameTimer.Start();
                frameTimer.Tick += new EventHandler(ProcessFrame);
            }

            /*
             * img = new Image<Bgr, byte>(Application.StartupPath + "/faces.jpg");
             */
            imgCamUser.SendToBack();

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
            if(vs !=null && vs.Count()>0)
            {
                handleParameters(vs);
            }

            

           
            
            //videoWriter = new VideoWriter(fileName, backend_idx, fourcc, 30, new Size(480, 640), true);
            Debug.WriteLine(Convert.ToInt32(Properties.Camera1.Default.view_width) + " WIDTH");
            camform = this;
            //camera = new GitHub.secile.Video.UsbCamera(0, new Size(640, 480));
            //camera.Start();
            //t = new Thread(new ThreadStart(VideoRecording));
            //t.Start();
         
            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/test.mp4";
            //var encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");
            var fps = 10.0f;
            //var writer = new GitHub.secile.Avi.AviWriter(System.IO.File.OpenWrite(path), "H264", camera.Size.Width, camera.Size.Height, fps);

            /*OpenH264Lib.Encoder.OnEncodeCallback onEncode = (data, length, frameType) =>
            {
                
                var keyFrame = (frameType == OpenH264Lib.Encoder.FrameType.IDR) || (frameType == OpenH264Lib.Encoder.FrameType.I);
                writer.AddImage(data);
            };
            */
            int bps = 1000000;
            //encoder.Setup(camera.Size.Width, camera.Size.Height, bps, fps, 2.0f, onEncode);

            camform.FormClosing += (s, ev) => {
                Debug.WriteLine("recording ended");
                //camera.Release();
                frameTimer.Stop();
                //writer.Close();
                //encoder.Dispose();
            };
        }
        private void ProcessFrame(object sender, EventArgs eventArgs)
        {

            try
            {
                //imaMat = _capture.QueryFrame();                
                //imageFrame = imaMat.ToImage<Bgr, Byte>();
                var bmp = camera.GetBitmap();
                imageFrame = new Image<Bgr, Byte>(bmp);
                imgCamUser.Image = imageFrame;
                dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                dateTimeLabel.Parent = imgCamUser;
                pbRecording.Parent = imgCamUser;

                //videoWriter.Write(imaMat);
                //var bmp = camera.GetBitmap();
                
                //encoder.Encode(bmp);

            }
            catch(NullReferenceException e)
            {
                ////Debug.WriteLine(e.Message);
            }
            //Debug.WriteLine(this.Size);     
            
        }
        private static void VideoRecording()
        {

            
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
                    
                }
                var eyes = _cascadeClassifierEyes.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                //the actual face detection happens here
                foreach (var eye in eyes)
                {
                    imageFrame.Draw(eye, new Bgr(Color.Red), 3);
                    //the detected face(s) is highlighted here using a box that is drawn around it/them
                    //Debug.WriteLine(imageFrame);

                }
                var people = _cascadeClassifierBody.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                //the actual face detection happens here
                foreach (var person in people)
                {
                    imageFrame.Draw(person, new Bgr(Color.Red), 3);
                    //the detected face(s) is highlighted here using a box that is drawn around it/them
                    //Debug.WriteLine(imageFrame);

                }
            }
            imgCamUser.Image = imageFrame;
            


        }

        public static void handleParameters(IReadOnlyCollection<string> parameters)
        {
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (parameters!=null && parameters.Contains("uvccameraviewer"))
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
                        ////Debug.WriteLine(e.Message);
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
                                    formChangesApply();
                                    
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                //Debug.WriteLine(e.Message);
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
                                //Debug.WriteLine(e.Message);
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
                                //Debug.WriteLine(e.Message);
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
                                //Debug.WriteLine(e.Message);
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                    }
                }
            }
            else if(parameters.Count>=0)//!!!!!!!!!!!! temporary error check, fix it to Count > 0
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
                                    formChangesApply();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                //Debug.WriteLine(e.Message);
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
            //Debug.WriteLine(timer.ToString());
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
                settingUI.Show();
                
                
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
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");

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

        private void SnapShot(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot");
            var timage = imaMat.ToImage<Bgr, Byte>();
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            timage.Save(Properties.Settings.Default.video_file_location + "/Camera/1/snapshot/" + imgdate+".jpeg");
            timage.Dispose();
        }
    }
}