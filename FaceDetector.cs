﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    class FaceDetector
    {
        //Cascade
        private CascadeClassifier fase_cascade = new CascadeClassifier();
        private CascadeClassifier eye_cascade = new CascadeClassifier();
        private CascadeClassifier body_cascade = new CascadeClassifier();
        Timer face_check_timer = new Timer();
        
        bool checkOK = false;

        public FaceDetector()
        {
            // Cascadeファイル読み込み
            string fase_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string eye_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            string body_cascade_file = ".\\HAARCASCADES\\haarcascade_frontalface_default.xml";
            fase_cascade.Load(fase_cascade_file);
            eye_cascade.Load(eye_cascade_file);
            body_cascade.Load(body_cascade_file);
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            face_check_timer.Tick += Face_check_timer_Tick;
            //face_check_timer.AutoReset = true;
            //face_check_timer.Elapsed += Face_check_timer_Tick;
            face_check_timer.Start();
        }

        private void Face_check_timer_Tick(object sender, EventArgs e)
        {
            
            Console.WriteLine("FACE " + checkOK);
            try
            {
                if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.enable_face_recognition && checkOK)
                {
                    Bitmap bitmap = MainForm.GetMainForm.crossbar.GetBitmap();                    
                    if (bitmap != null)
                    {
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
                            if (Properties.Settings.Default.capture_method == 0)
                            {
                                //↑20191107 Nagayama added↑
                                //initiate RECORD mode
                                if (MainForm.GetMainForm != null && MainForm.GetMainForm.crossbar.PREEVENT_RECORDING)
                                {                                    
                                    TaskManager.EventAppeared(RECORD_PATH.EVENT, 
                                        1, 
                                        decimal.ToInt32(Properties.Settings.Default.seconds_before_event), 
                                        decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                                }
                                else
                                {
                                    //Direct recording
                                    MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.EVENT);
                                }                                
                                MainForm.GetMainForm.crossbar.SET_ICON_TIMER();
                            }
                            //↓20191107 Nagayama added↓
                            else
                            {
                                SNAPSHOT_SAVER.TakeSnapShot(0);                                
                            }
                            //↑20191107 Nagayama added↑    

                            if (Properties.Settings.Default.backlight_on_upon_face_rec)
                                MainForm.GetMainForm.BackLight.ON();

                            MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                        }
                    }                    
                }

            }
            
            catch (NullReferenceException ex)
            {
                Logger.Add(ex);
                //Stop_Face_Timer();
                //Destroy();
            }         
        }

        public void SetInterval()
        {
            face_check_timer.Enabled = true;
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
        }

          public void Destroy()
        {
            Stop_Face_Timer();
            face_check_timer.Dispose();
        }
        
        public void Start_Face_Timer()
        {
            checkOK = true;
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            face_check_timer.Enabled = true;
        }


        public void Stop_Face_Timer()
        {
            face_check_timer.Enabled = false;
            //face_check_timer.Stop();
            checkOK = false;
        }
    }
}
