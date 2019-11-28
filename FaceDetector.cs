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
            // Cascadeファイル読み込み
            string face_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string eye_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string body_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            face_cascade.Load(face_cascade_file);
            eye_cascade.Load(eye_cascade_file);
            body_cascade.Load(body_cascade_file);
            face_check_timer.Enabled = true;
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            face_check_timer.Elapsed += Face_check_timer_Tick;
            face_check_timer.AutoReset = true;
            face_check_timer.Enabled = false;

            Task.Run(() => {
                Thread.Sleep(5000);
                try
                {
                    if (Properties.Settings.Default.enable_face_recognition)
                        face_check_timer.Start();
                }
                catch(ObjectDisposedException odx)
                {
                    Logger.Add(odx);
                }
              });
        }

        private void Face_check_timer_Tick(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("FACE " + checkOK);
            try
            {
                if (Properties.Settings.Default.enable_face_recognition && checkOK)
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
            if (MainForm.GetMainForm.InvokeRequired)
            {
                var d = new dGetTheBMPImage(GetTheBMPForFaceCheck);
                if (MainForm.GetMainForm != null)
                {
                    MainForm.GetMainForm.Invoke(d);
                }
            }
            else
            {
                Bitmap bitmap = MainForm.GetMainForm.crossbar.GetBitmap();
                if (bitmap != null)
                {
                   faceTask = new Task(() => {

                       Mat mat = bitmap.ToMat();
                       //Rect[] rectList = face_cascade.DetectMultiScale(mat)
                       rectList = face_cascade.DetectMultiScale(mat);
                       if (rectList.Length == 0)
                       {
                           rectList = eye_cascade.DetectMultiScale(mat);
                       }
                       if (rectList.Length == 0)
                       {
                           rectList = body_cascade.DetectMultiScale(mat);
                       }
                       if (rectList.Length > 0) // Face signature detected
                       {
                           checkOK = false;

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
            System.Drawing.Size size = MainForm.GetMainForm.Size;
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
                Graphics gr = MainForm.GetMainForm.CreateGraphics();
                gr.DrawRectangle(redPen, rectangle);
            }
            if (i == 25) // To display the rectangle enough long time to be visible
            {
                face_display_end_timer.Enabled = false;
                i = 0;
            }
        }

        private void FaceDetectedAction()
        {
            if (MainForm.GetMainForm!=null && MainForm.GetMainForm.InvokeRequired)
            {
                var d = new dSetTheIcons(FaceDetectedAction);
                MainForm.GetMainForm.Invoke(d);
            }
            else
            {
                if (MainForm.GetMainForm.crossbar.OPER_BAN == false)
                {
                    if (Properties.Settings.Default.capture_method <= 0)
                    {
                        //initiate RECORD mode
                        if (MainForm.GetMainForm != null && MainForm.GetMainForm.crossbar.PREEVENT_RECORDING)
                        {
                            if (MainForm.GetMainForm.recordingInProgress == false)
                            {
                                TaskManager.EventAppeared(RECORD_PATH.EVENT,
                                    1,
                                    decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                                    decimal.ToInt32(Properties.Settings.Default.seconds_after_event),
                                    DateTime.Now);

                                MainForm.GetMainForm.SET_REC_ICON();
                                MainForm.GetMainForm.crossbar.SetIconTimer(Properties.Settings.Default.seconds_after_event);
                                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                            }
                        }
                        else
                        {
                            //Direct recording
                            MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);
                        }
                        if (MainForm.GetMainForm.recordingInProgress == false)
                        {
                            MainForm.GetMainForm.crossbar.SetIconTimer(Properties.Settings.Default.seconds_after_event);
                            MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                        }
                    }
                    //↓20191107 Nagayama added↓
                    else
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(0, "event");
                        MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(0);
                    }
                    //↑20191107 Nagayama added↑    

                    if (Properties.Settings.Default.backlight_on_upon_face_rec)
                    {
                        MainForm.GetMainForm.BackLight.ON();
                    }
                }
            }
        }

        public void SetInterval()
        {
            face_check_timer.Enabled = true;
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            face_check_timer.Enabled = false;
        }

          public void Destroy()
        {
            StopFaceTimer();
            face_check_timer.Dispose();
        }
        
        public void StartFaceTimer()
        {
            Task task = new Task(() => {
                if (first)
                    Thread.Sleep(7000);
                first = false;
                checkOK = true;
                face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
                face_check_timer.Start();                                
            });
            if (Properties.Settings.Default.enable_face_recognition)
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
