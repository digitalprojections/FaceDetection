using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace FaceDetection
{
    class FaceDetector
    {
        private bool first = true;
        private delegate void dGetTheBMPImage();
        private delegate void dSetTheIcons();

        //Cascade
        private CascadeClassifier face_cascade = new CascadeClassifier();
        private CascadeClassifier eye_cascade = new CascadeClassifier();
        private CascadeClassifier body_cascade = new CascadeClassifier();
        System.Timers.Timer face_check_timer = new System.Timers.Timer();
        System.Timers.Timer face_display_end_timer = new System.Timers.Timer();
        private bool checkOK = false;
        private Rect[] rectList;
        private int i = 0;

        Task faceTask;

        public FaceDetector()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            int checkInterval = 0;
            bool faceRecognitionEnabled = false;

            switch (camindex)
            {
                case 0:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C1_check_interval);
                    faceRecognitionEnabled = Properties.Settings.Default.C1_enable_face_recognition;
                    break;
                case 1:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C2_check_interval);
                    faceRecognitionEnabled = Properties.Settings.Default.C2_enable_face_recognition;
                    break;
                case 2:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C3_check_interval);
                    faceRecognitionEnabled = Properties.Settings.Default.C3_enable_face_recognition;
                    break;
                case 3:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C4_check_interval);
                    faceRecognitionEnabled = Properties.Settings.Default.C4_enable_face_recognition;
                    break;
            }

            // Cascadeファイル読み込み
            string face_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string eye_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string body_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            face_cascade.Load(face_cascade_file);
            eye_cascade.Load(eye_cascade_file);
            body_cascade.Load(body_cascade_file);
            face_check_timer.Enabled = true;
            face_check_timer.Interval = checkInterval;
            face_check_timer.Elapsed += Face_check_timer_Tick;
            face_check_timer.AutoReset = true;
            face_check_timer.Enabled = false;

            Task.Run(() => {
                Thread.Sleep(5000);
                try
                {
                    if (faceRecognitionEnabled)
                    {
                        face_check_timer.Start();
                    }
                }
                catch(ObjectDisposedException odx)
                {
                    Logger.Add(odx);
                }
              });
        }

        private void Face_check_timer_Tick(object sender, ElapsedEventArgs e)
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            bool faceRecognitionEnabled = false;

            switch (camindex)
            {
                case 0:
                    faceRecognitionEnabled = Properties.Settings.Default.C1_enable_face_recognition;
                    break;
                case 1:
                    faceRecognitionEnabled = Properties.Settings.Default.C2_enable_face_recognition;
                    break;
                case 2:
                    faceRecognitionEnabled = Properties.Settings.Default.C3_enable_face_recognition;
                    break;
                case 3:
                    faceRecognitionEnabled = Properties.Settings.Default.C4_enable_face_recognition;
                    break;
            }

            Console.WriteLine("FACE " + checkOK);
            try
            {
                if (faceRecognitionEnabled && checkOK)
                {
                    GetTheBMPForFaceCheck();                    
                }
                else
                {
                    face_check_timer.Stop();
                }
            }
            catch (NullReferenceException ex)
            {
                Logger.Add(ex);
                //Stop_Face_Timer();
                //Destroy();
            }         
        }

        private async void GetTheBMPForFaceCheck()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            Bitmap bitmap;

            if (MainForm.GetMainForm.InvokeRequired)
            {
                var d = new dGetTheBMPImage(GetTheBMPForFaceCheck);
                if(MainForm.GetMainForm!=null)
                {
                    MainForm.GetMainForm.Invoke(d);
                }
            }
            else
            {
                if (camindex == 0)
                {
                    bitmap = MULTI_WINDOW.formList[camindex].crossbar.GetBitmap();
                }
                else
                {
                    //bitmap = CameraForm.crossbar.GetBitmap();
                    //Access through MultiWindow
                    bitmap = null;
                }

                if (bitmap != null)
                {
                   faceTask = new Task(() => {
                        Mat mat = bitmap.ToMat();
                       //Rect[] rectList = face_cascade.DetectMultiScale(mat)
                       rectList = face_cascade.DetectMultiScale(mat);
                       //if (rectList.Length == 0)
                       //{
                       //    rectList = eye_cascade.DetectMultiScale(mat);
                       //}
                       //if (rectList.Length == 0)
                       //{
                       //    rectList = body_cascade.DetectMultiScale(mat);
                       //}
                       if (rectList.Length > 0) // Face signature detected
                        {
                            checkOK = false;
                           Logger.Add("Face detected");

                           // Launch timer to display rectangle around the detected face
                           face_display_end_timer.Enabled = true;
                           face_display_end_timer.Interval = 50; // ms
                           face_display_end_timer.Elapsed += Face_display_end_timer_Tick;
                           face_display_end_timer.AutoReset = true;

                            FaceDetectedAction();
                        }
                    });                    
                    faceTask.Start();
                    //faceTask.Wait();
                }
            }
        }

        private void Face_display_end_timer_Tick(object sender, ElapsedEventArgs e)
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            System.Drawing.Size size;

            if (camindex == 0)
            {
                size = MainForm.GetMainForm.Size;
            }
            else
            {
                size = MULTI_WINDOW.formList[camindex - 1].Size;
            }

            int formWidth = size.Width;
            int formHeight = size.Height;
            var screen = Screen.PrimaryScreen;
            int screenResWidth = screen.Bounds.Width;
            int screenResHeight = screen.Bounds.Height;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int rectPosX = Convert.ToInt32((((float)rectList[0].Location.X / screenWidth) * formWidth) * ((float)screenResWidth / 1280));
            int rectPosY = Convert.ToInt32((((float)rectList[0].Location.Y / screenHeight) * formHeight) * ((float)screenResHeight / 720));
            int rectWidth = Convert.ToInt32((((float)rectList[0].Width / screenWidth) * formWidth) * ((float)screenResWidth / 1280));
            int rectHeight = Convert.ToInt32((((float)rectList[0].Height / screenHeight) * formHeight) * ((float)screenResHeight / 720));

            i++;

            if (rectList.Length > 0) // Face signature detected
            {
                Rectangle rectangle = new Rectangle(rectPosX, rectPosY, rectWidth, rectHeight);
                Pen redPen = new Pen(Color.Red, 3);

                // Draw rectangle to screen
                if (camindex == 0)
                {
                    Graphics gr = MainForm.GetMainForm.CreateGraphics();
                    gr.DrawRectangle(redPen, rectangle);
                }
                else
                {
                    Graphics gr = MULTI_WINDOW.formList[camindex - 1].CreateGraphics();
                    gr.DrawRectangle(redPen, rectangle);
                }
            }
            if (i == 25) // To display the rectangle enough long time to be visible
            {
                face_display_end_timer.Enabled = false;
                i = 0;
            }
        }

        private void FaceDetectedAction()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;
            string captureMethod = "";

            PROPERTY_FUNCTIONS.GetPreAndPostEventTimes(camindex, out timeBeforeEvent, out timeAfterEvent);

            preeventRecording = MULTI_WINDOW.formList[camindex].crossbar.PREEVENT_RECORDING;
            PROPERTY_FUNCTIONS.GetCaptureMethod(camindex, out captureMethod);

            if (MainForm.GetMainForm!=null && MainForm.GetMainForm.InvokeRequired)
            {
                var d = new dSetTheIcons(FaceDetectedAction);
                MainForm.GetMainForm.Invoke(d);
            }
            else
            {
                if (MULTI_WINDOW.formList[camindex].crossbar.OPER_BAN == false)
                {
                    if (captureMethod != "Snapshot") // Video
                    {
                        //initiate RECORD mode
                        if (MainForm.GetMainForm != null && preeventRecording)
                        {
                            if (MainForm.AnyRecordingInProgress == false)
                            {
                                TaskManager.EventAppeared(RECORD_PATH.EVENT, camindex+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);
                                                                
                                MULTI_WINDOW.formList[camindex].SetRecordIcon(camindex, timeAfterEvent);
                                
                            }
                        }
                        else
                        {
                            //Direct recording
                            MULTI_WINDOW.formList[camindex].crossbar.Start(camindex, CAMERA_MODES.OPERATOR);                            
                        }
                    }
                    else // Snapshot
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(Properties.Settings.Default.main_camera_index, "event");
                        MULTI_WINDOW.formList[camindex].crossbar.No_Cap_Timer_ON(camindex);
                    }  

                    if (Properties.Settings.Default.backlight_on_upon_face_rec)
                    {
                        MainForm.GetMainForm.BackLight.ON();
                    }
                }
            }
        }

        public void SetInterval()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            int checkInterval = 0;

            PROPERTY_FUNCTIONS.GetInterval(camindex, out checkInterval);

            face_check_timer.Enabled = true;
            face_check_timer.Interval = checkInterval;
            face_check_timer.Enabled = false;
        }

          public void Destroy()
        {
            StopFaceTimer();
            face_check_timer.Dispose();
        }
        
        public void StartFaceTimer()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            int checkInterval = 0;
            bool faceRecognitionEnabled = false;

            PROPERTY_FUNCTIONS.GetInterval(camindex, out checkInterval);
            PROPERTY_FUNCTIONS.GetFaceRecognitionSwitch(camindex, out faceRecognitionEnabled);

            Task task = new Task(() => {
                if (first)
                    Thread.Sleep(7000);
                first = false;
                checkOK = true;
                face_check_timer.Interval = checkInterval;
                face_check_timer.Start();                                
            });
            if (faceRecognitionEnabled)
            {
                task.Start();
            }
        }

        public void StopFaceTimer()
        {
            face_check_timer.Enabled = false;
            checkOK = false;
        }
    }
}
