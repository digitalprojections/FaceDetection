﻿using System;
using System.Drawing;
using FaceDetectionX;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FaceDetection
{
    public class CROSSBAR
    {
        public bool PREEVENT_RECORDING = false;
        private RecorderCamera recorder = null;
        private UsbCamera camera = null;

        internal int INDEX = 0;
        //BOOLEAN
        public bool OPER_BAN = false;

        bool wait_interval_enabled = false;
        int duration = 0;

        /// <summary>
        /// Recording on. 
        /// PREEVENT or any other actual recording using RecorderCamera.        /// 
        /// </summary>
        static bool recording_is_on = false;
        //TIMERS, 2 timers only
        /// <summary>
        /// If Enabled and AutoReset are both set to false, 
        /// and the timer has previously been enabled, 
        /// setting the Interval property causes the Elapsed event to be raised once, 
        /// as if the Enabled property had been set to true. 
        /// To set the interval without raising the event, 
        /// you can temporarily set the Enabled property to true, 
        /// set the Interval property to the desired time interval, 
        /// and then immediately set the Enabled property back to false.
        /// </summary>
        static Timer the_timer = new Timer();

        
        /// <summary>
        /// No operator capturing during this period.
        /// <see cref="OPER_BAN"/>
        /// </summary>
        static System.Timers.Timer no_opcap_timer = new System.Timers.Timer();
        Timer icon_timer = new Timer();

        public bool Recording_is_on { get => recording_is_on; set => recording_is_on = value; }
                        
        public CROSSBAR()
        {            
            this.INDEX = 0;
            no_opcap_timer.AutoReset = false;
            no_opcap_timer.Elapsed += No_opcap_timer_Elapsed;
            no_opcap_timer.Enabled = true;


            icon_timer.Tick += Icon_timer_Tick;
            icon_timer.Enabled = false;

            //it is the sum of the 2 values
            int intt = decimal.ToInt32(
                Properties.Settings.Default.interval_before_reinitiating_recording)
                * 1000;
            if (intt > 500)
            {
                no_opcap_timer.Interval = intt;
                no_opcap_timer.Enabled = false;
            }

            Logger.Add(no_opcap_timer.Interval.ToString());

            the_timer.Tick += The_timer_Tick;
            recorder = new RecorderCamera(0);
        }

        private void Icon_timer_Tick(object sender, EventArgs e)
        {
            MainForm.Or_pb_recording.Visible = false;            
            icon_timer.Enabled = false;
        }

        private void Icon_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
        }

        public void SET_ICON_TIMER()
        {
            MainForm.Or_pb_recording.Visible = true;
            
            icon_timer.Interval = decimal.ToInt32(Properties.Settings.Default.seconds_after_event) * 1000;
            icon_timer.Start();
            
        }


        public void No_Cap_Timer_ON(int vidlen)
        {
            MainForm.GetMainForm.SET_REC_ICON();
            wait_interval_enabled = true;
            if(no_opcap_timer!=null)
                no_opcap_timer.Enabled = true;
            int intt = decimal.ToInt32(
                Properties.Settings.Default.interval_before_reinitiating_recording + vidlen)
                * 1000;
            if (intt > 500)
            {
                no_opcap_timer.Interval = intt;
            }


        }

        private void The_timer_Tick(object sender, EventArgs e)
        {
            MainForm.GetMainForm.SetRecordButtonState("play", false);
            //We end the recording here
            if (!PREEVENT_RECORDING)
            {
                recorder.ReleaseInterfaces();
                the_timer.Enabled = false;
                if (this != null)
                    this.PreviewMode();
            }
            else if (PREEVENT_RECORDING)
            {
                recorder.CAMERA_MODE = CAMERA_MODES.PREEVENT;
                if(the_timer!=null)
                {
                    the_timer.Enabled = true;
                    the_timer.Interval = BUFFER_DURATION.BUFFERDURATION;
                }                    
                recorder.RESET_FILE_PATH();                
            }
            if (wait_interval_enabled)
            {
                //Run the timer
                int intt = decimal.ToInt32(
                Properties.Settings.Default.interval_before_reinitiating_recording)
                * 1000;
                if (intt >= 500)
                {
                    no_opcap_timer.Interval = intt;
                    no_opcap_timer.Enabled = true;
                }

            }
        }

        private void No_opcap_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OPER_BAN = false;
            Logger.Add("No_opcap_timer_Elapsed, OPER_BAN XXXXXXXXXXXXXXXXXXXXX " + OPER_BAN);
            if (wait_interval_enabled)
            {
                no_opcap_timer.Enabled = false;
                wait_interval_enabled = false;
               
                if (Properties.Settings.Default.Recording_when_at_the_start_of_operation)
                {

                    MOUSE_KEYBOARD.AddMouseAndKeyboardBack();
                }

                if (Properties.Settings.Default.enable_Human_sensor)
                {

                    MainForm.RSensor.Start_IR_Timer();
                } else if (Properties.Settings.Default.enable_face_recognition)
                {
                    
                    MainForm.FaceDetector.Start_Face_Timer();
                }
            }
        }

        internal Bitmap GetBitmap()
        {

            if (Recording_is_on)

                return recorder.GetBitmap();
            else
                return camera.GetBitmap();
        }

        internal void RESTART_CAMERA()
        {
            if (Recording_is_on)
                this.RecordingMode();
            else
                this.PreviewMode();
        }

        internal void PreviewMode()
        {
            //recording_permission = true;
            Recording_is_on = false;
            MainForm.Or_pb_recording.Visible = false;
            PREEVENT_RECORDING = false;
            if (MainForm.GetMainForm != null)
            {
                Logger.Add("DONE: PREVIEW MODE !!!!!!!!!!!!!");
                if (recorder != null && recorder.ON)
                {
                    recorder.ReleaseInterfaces();

                }
                if (camera == null || !camera.ON)
                {
                    camera = new UsbCamera(0);
                    camera.Start();
                }

            }
        }
        internal void RecordingMode()
        {
            Recording_is_on = true;

            if (MainForm.GetMainForm != null)
            {
                if (camera != null && camera.ON)
                {
                    camera.Release();
                }
                recorder.StartRecorderCamera();
            }
        }
        public void StartTimer()
        {
            the_timer.Enabled = true;
            Logger.Add("timer started " + the_timer.Interval);
        }
        public void Start(int index, CAMERA_MODES mode)
        {
            this.INDEX = index;
            Logger.Add(mode.ToString());
            int duration = 0;

            switch (mode)
            {
                case CAMERA_MODES.MANUAL:

                    if (Properties.Settings.Default.manual_record_time > 0)
                    {
                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        //decimal mrm = Properties.Settings.Default.manual_record_time;
                        //This does not check if the recording is on, as it prioritizes the manual recording
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(0);
                        recorder.CAMERA_MODE = CAMERA_MODES.MANUAL;
                        recorder.ACTIVE_RECPATH = RECORD_PATH.MANUAL;
                        //↓20191106 Nagayama added↓
                        //the_timer.Stop();
                        //↑20191106 Nagayama added↑                        
                        //the_timer.Enabled = true;
                        duration = decimal.ToInt32(Properties.Settings.Default.manual_record_time) * 1000;                        
                        the_timer.Interval = duration;
                        the_timer.Enabled = true;
                        if (this != null)
                            this.RecordingMode();
                    }
                    break;
                case CAMERA_MODES.EVENT:
                    //EVENT from parameters
                    if (camera != null && Properties.Settings.Default.enable_event_recorder && Properties.Settings.Default.event_record_time_after_event > 0 && camera.ON)
                    {

                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(0);
                        recorder.ACTIVE_RECPATH = RECORD_PATH.EVENT;
                        recorder.CAMERA_MODE = CAMERA_MODES.EVENT;
                        duration = decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event) * 1000;
                        Logger.Add(duration + "  " + recorder.CAMERA_MODE);

                        the_timer.Enabled = true;
                        the_timer.Interval = duration;                        
                        if (this != null)
                            this.RecordingMode();
                    }
                    break;
                case CAMERA_MODES.OPERATOR:
                    //EVENT from parameters                    
                    if (camera != null && Properties.Settings.Default.capture_operator && Properties.Settings.Default.seconds_after_event > 0 && camera.ON && !no_opcap_timer.Enabled)
                    {
                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        duration = decimal.ToInt32(Properties.Settings.Default.seconds_after_event) * 1000;
                        Logger.Add(duration + "  " + recorder.CAMERA_MODE);
                        the_timer.Enabled = true;
                        the_timer.Interval = duration;
                        Logger.Add(no_opcap_timer.Interval.ToString());
                        if (this != null)
                        {
                            recorder.ACTIVE_RECPATH = RECORD_PATH.EVENT;
                            recorder.CAMERA_MODE = CAMERA_MODES.OPERATOR;
                            wait_interval_enabled = true;
                            this.OPER_BAN = true;
                            this.RecordingMode();
                        }

                    }
                    break;
                case CAMERA_MODES.PREVIEW:
                    //preview
                    PREEVENT_RECORDING = false;
                    duration = 0;
                    the_timer.Stop();
                    recorder.ACTIVE_RECPATH = "";
                    recorder.ReleaseInterfaces();
                    if (this != null)
                        this.PreviewMode();
                    break;
                case CAMERA_MODES.PREEVENT:
                    //permanent cycle
                    PREEVENT_RECORDING = true;
                    duration = BUFFER_DURATION.BUFFERDURATION;
                    //Logger.Add(duration);
                    recorder.ReleaseInterfaces();
                    recorder = new RecorderCamera(0);
                    recorder.ACTIVE_RECPATH = Properties.Settings.Default.temp_folder;
                    recorder.CAMERA_MODE = CAMERA_MODES.PREEVENT;
                    the_timer.Enabled = true;
                    the_timer.Interval = duration;                    
                    if (this != null)
                        this.RecordingMode();
                    break;
            }
        }

        internal void SetWindowPosition(Size size)
        {
            try
            {
                if (camera!=null && camera.ON)
                {
                    camera.SetWindowPosition(size);
                }
                else if (recorder!=null && recorder.ON || PREEVENT_RECORDING)
                {
                    recorder.SetWindowPosition(size);
                }
            } catch (NullReferenceException nrx)
            {
                Logger.Add(nrx);
            }

        }
        internal void ReleaseCameras()
        {
            Logger.Add("TODO: Releasing all cameras here");
            try
            {
                the_timer.Dispose();
                no_opcap_timer.Dispose();
                icon_timer.Dispose();
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }
        }
        

    }

}