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
    public class FaceDetectorX
    {
        private bool first = true;
        private delegate void dGetTheBMPImage();
        private delegate void dSetTheIcons();

        //Cascade
        private CascadeClassifier fase_cascade = new CascadeClassifier();
        private CascadeClassifier eye_cascade = new CascadeClassifier();
        private CascadeClassifier body_cascade = new CascadeClassifier();
        System.Timers.Timer face_check_timer = new System.Timers.Timer();
        
        bool checkOK = false;

        Task faceTask;

        FormClass xform;

        public FaceDetectorX(FormClass form)
        {
            xform = form;
            // Cascadeファイル読み込み
            string fase_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string eye_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string body_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            fase_cascade.Load(fase_cascade_file);
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
            Console.WriteLine("FACE camera index: " + xform.CAMERA_INDEX + " - " + checkOK);
            try
            {
                if (Properties.Settings.Default.enable_face_recognition && checkOK)
                {
                    GetTheBMPForFaceCheck();                    
                }else
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

        private void GetTheBMPForFaceCheck()
        {
            if (xform.InvokeRequired)
            {
                var d = new dGetTheBMPImage(GetTheBMPForFaceCheck);
                if(xform != null)
                    xform.Invoke(d);
            }
            else
            {
                Bitmap bitmap = xform.crossbar.GetBitmap();
                if (bitmap != null)
                {
                   faceTask = new Task(() => {

                        Mat mat = bitmap.ToMat();
                        Rect[] rectList = fase_cascade.DetectMultiScale(mat);
                        if (rectList.Length == 0)
                            rectList = eye_cascade.DetectMultiScale(mat);

                        if (rectList.Length == 0)
                            rectList = body_cascade.DetectMultiScale(mat);

                        if (rectList.Length > 0)
                        {
                            checkOK = false;
                            //heat signature detected, stop timer

                            //↓20191107 Nagayama added↓
                            FaceDetectedAction();
                        }
                    });                    
                    faceTask.Start();
                    //faceTask.Wait();
                }
            }
        }

        private void FaceDetectedAction()
        {
            if (xform != null && xform.InvokeRequired)
            {
                var d = new dSetTheIcons(FaceDetectedAction);
                xform.Invoke(d);
            }
            else
            {
                if (Properties.Settings.Default.capture_method <= 0)
                {
                    //↑20191107 Nagayama added↑
                    //initiate RECORD mode
                    if (xform != null && xform.crossbar.PREEVENT_RECORDING)
                    {
                        TaskManager.EventAppeared(RECORD_PATH.EVENT,
                            xform.CAMERA_INDEX+1,
                            decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                            decimal.ToInt32(Properties.Settings.Default.seconds_after_event),
                            DateTime.Now);

                        xform.SET_REC_ICON_STATE_ON_VIEW();
                        
                    }
                    else
                    {
                        //Direct recording
                        xform.crossbar.Start(0, CAMERA_MODES.OPERATOR);
                    }
                    xform.crossbar.SetIconTimer(Properties.Settings.Default.seconds_after_event);
                    xform.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                }
                //↓20191107 Nagayama added↓
                else
                {
                    SNAPSHOT_SAVER.TakeSnapShot(xform.CAMERA_INDEX + 1, "event");
                    xform.crossbar.No_Cap_Timer_ON(0);
                }
                //↑20191107 Nagayama added↑    

                if (Properties.Settings.Default.backlight_on_upon_face_rec)
                    MainForm.GetMainForm.BackLight.ON();
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
