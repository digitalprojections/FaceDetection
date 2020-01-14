using System;
using System.Drawing;
using FaceDetectionX;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FaceDetection
{
    public class CROSSBAR
    {
        private delegate void dHideRecIcon();
        private delegate void dStopVideoNow();
        private delegate void dAllowOperCap();
        public bool PREEVENT_RECORDING = false;
        private RecorderCamera recorder = null;
        private UsbCamera camera = null;

        internal int INDEX = 0;
        Form parentwindow = null;
        
        public bool OPER_BAN = false;
        private bool manualRecording;
        bool wait_interval_enabled = false;
        int duration = 0;

        /// <summary>
        /// Recording on. 
        /// PREEVENT or any other actual recording using RecorderCamera.        /// 
        /// </summary>
        bool recording_is_on = false;
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
        //static System.Timers.Timer the_timer = new System.Timers.Timer();
        System.Timers.Timer the_timer;

        /// <summary>
        /// No operator capturing during this period.
        /// <see cref="OPER_BAN"/>
        /// </summary>
        //static System.Timers.Timer no_opcap_timer = new System.Timers.Timer();
        System.Timers.Timer no_opcap_timer;

        //System.Timers.Timer icon_timer = new System.Timers.Timer();
        public System.Timers.Timer icon_timer;

        CameraForm window;

        public bool Recording_is_on { get => recording_is_on; set => recording_is_on = value; }
                        
        public CROSSBAR(int cameraindex, CameraForm window_ptr)
        {
            this.the_timer = new System.Timers.Timer();
            this.no_opcap_timer = new System.Timers.Timer();
            this.icon_timer = new System.Timers.Timer();
            this.INDEX = cameraindex;
            this.parentwindow = window_ptr;
            window = window_ptr;

            int intervalBeforeReinitiating = 0;
            if (cameraindex == 0)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording);
            }
            else if (cameraindex == 1)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording);
            }
            else if (cameraindex == 2)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording);
            }
            else if (cameraindex == 3)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording);
            }

            this.no_opcap_timer.AutoReset = false;
            this.no_opcap_timer.Elapsed += No_opcap_timer_Elapsed;
            this.no_opcap_timer.Enabled = false;

            this.icon_timer.Elapsed += Icon_timer_Tick;
            this.icon_timer.Enabled = false;

            //it is the sum of the 2 values
            int intt = intervalBeforeReinitiating * 1000;
            if (intt > 500)
            {
                this.no_opcap_timer.Interval = intt;
                this.no_opcap_timer.Enabled = false;
            }

            Logger.Add("No operator capture interval : " + this.no_opcap_timer.Interval.ToString());

            this.the_timer.Elapsed += The_timer_Tick;
            this.the_timer.AutoReset = false;
            recorder = new RecorderCamera(this.INDEX, this.parentwindow, this);
        }

        private void Icon_timer_Tick(object sender, ElapsedEventArgs e)
        {
            HideTheRecIcon();           
        }

        private void HideTheRecIcon()
        {
            bool captureOperatorEnabled = false, IRSensorEnabled = false;

            if(this.INDEX == 0)
            {
                captureOperatorEnabled = Properties.Settings.Default.C1_enable_capture_operator;
                IRSensorEnabled = Properties.Settings.Default.C1_enable_Human_sensor;
            }
            else if (this.INDEX == 1)
            {
                captureOperatorEnabled = Properties.Settings.Default.C2_enable_capture_operator;
                IRSensorEnabled = Properties.Settings.Default.C2_enable_Human_sensor;
            }
            else if (this.INDEX == 2)
            {
                captureOperatorEnabled = Properties.Settings.Default.C3_enable_capture_operator;
                IRSensorEnabled = Properties.Settings.Default.C3_enable_Human_sensor;
            }
            else if (this.INDEX == 3)
            {
                captureOperatorEnabled = Properties.Settings.Default.C4_enable_capture_operator;
                IRSensorEnabled = Properties.Settings.Default.C4_enable_Human_sensor;
            }

            if (window.picbox_recording.InvokeRequired)
            {
                var d = new dHideRecIcon(HideTheRecIcon);
                window.picbox_recording.Invoke(d);
            }
            else
            {
                window.picbox_recording.Visible = false;
                window.recordingInProgress = false;
                icon_timer.Enabled = false;
                if (captureOperatorEnabled && IRSensorEnabled)
                {
                    MainForm.RSensor.bIsIRCheckExec = true;
                }
            }
        }
        
        public void SetIconTimer(decimal recording_length)
        {
            window.SET_REC_ICON();
            icon_timer.Interval = decimal.ToInt32(recording_length) * 1000;
            icon_timer.Enabled = true;
            icon_timer.Start();            
        }

        internal bool ANY_CAMERA_ON()
        {
            bool rv = false;
            if (recorder != null)
            {
                try
                {
                    if (recorder.ON)
                        rv = true;                    
                }
                catch (NullReferenceException nrx)
                {
                    Logger.Add(nrx);
                }

            }else if (camera!=null)
            {
                try
                {
                    if (camera.ON)
                        rv = true;
                }
                catch (NullReferenceException nrx)
                {
                    Logger.Add(nrx);
                }
            }
            return rv;
        }

        public void No_Cap_Timer_ON(int vidlen)
        {
            int intervalBeforeReinitiating = 0;
            if(this.INDEX == 0)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording);
            }
            else if (this.INDEX == 1)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording);
            }
            else if (this.INDEX == 2)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording);
            }
            else if (this.INDEX == 3)
            {
                intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording);
            }


            wait_interval_enabled = true;            
            int intt = (intervalBeforeReinitiating + vidlen) * 1000;
            if (intt > 500)
            {
                no_opcap_timer.Interval = intt;
            }
            if (no_opcap_timer != null)
            {
                no_opcap_timer.Enabled = true;
                OPER_BAN = true;
            }
        }

        private void The_timer_Tick(object sender, ElapsedEventArgs e)
        {
            VideoRecordingEnd();
        }

        private void VideoRecordingEnd()
        {
            if (window.InvokeRequired)
            {
                var d = new dStopVideoNow(VideoRecordingEnd);
                window.Invoke(d);
            }
            else
            {
                //We end the recording here
                if (!PREEVENT_RECORDING)
                {
                    window.SetRecordButtonState("play");
                    Recording_is_on = false;
                    recorder.ReleaseInterfaces();
                    the_timer.Enabled = false;
                    if (this != null)
                    {
                        this.PreviewMode();
                    }
                }
                else if (PREEVENT_RECORDING && recorder.CAMERA_MODE==CAMERA_MODES.MANUAL)
                {
                    manualRecording = true;
                    the_timer.Enabled = false;
                    the_timer.Stop();

                    if (the_timer != null)
                    {
                        the_timer.Enabled = true;
                        the_timer.Interval = decimal.ToInt32(Properties.Settings.Default.manual_record_time)*1000;
                        recorder.ReleaseInterfaces();
                        recorder.StartRecorderCamera(this.INDEX); 
                    }
                    recorder.CAMERA_MODE = CAMERA_MODES.PREEVENT;
                }
                else if (PREEVENT_RECORDING)
                {
                    if (manualRecording == true)
                    {
                        manualRecording = false;
                        window.picbox_recording.Visible = false;
                        window.recordingInProgress = false;
                        window.SetRecordButtonState("play");
                    }

                    recorder.CAMERA_MODE = CAMERA_MODES.PREEVENT;
                    if (the_timer != null)
                    {
                        the_timer.Enabled = true;
                        the_timer.Interval = BUFFER_DURATION.BUFFERDURATION;
                    }
                    recorder.RESET_FILE_PATH();
                    try
                    {
                        Task task = Task.Factory.StartNew(() => {
                            if (Directory.Exists(@"D:\TEMP\"))
                            {
                                TaskManager.DeleteOldFiles(@"D:\TEMP\" + (INDEX + 1));
                            }
                            else
                            {
                                TaskManager.DeleteOldFiles(@"C:\TEMP\" + (INDEX + 1));
                            }
                        });
                    }
                    catch (IOException e)
                    {
                        Logger.Add(e.Message + " TaskManager in DeleteOldFiles()");
                    }
                }
                if (wait_interval_enabled)
                {
                    int intt = 0;
                    
                    if (this.INDEX == 0)
                    {
                        intt = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording) * 1000;
                    }
                    else if (this.INDEX == 1)
                    {
                        intt = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording) * 1000;
                    }
                    else if (this.INDEX == 2)
                    {
                        intt = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording) * 1000;
                    }
                    else if(this.INDEX == 3)
                    {
                        intt = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording) * 1000;
                    }

                    //Run the timer
                    if (intt >= 500)
                    {
                        no_opcap_timer.Interval = intt;
                        no_opcap_timer.Enabled = true;
                    }
                }
            }
        }

        private void No_opcap_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (window.recordingInProgress == false)
            {
                AllowOperCap();
            }
        }

        private void AllowOperCap()
        {
            bool recordingWhenOperation = false, IRSensorEnabled = false, faceDetectorEnabled = false;

            if (this.INDEX == 0)
            {
                recordingWhenOperation = Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation;
                IRSensorEnabled = Properties.Settings.Default.C1_enable_Human_sensor;
                faceDetectorEnabled = Properties.Settings.Default.C1_enable_face_recognition;
            }
            else if (this.INDEX == 1)
            {
                recordingWhenOperation = Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation;
                IRSensorEnabled = Properties.Settings.Default.C2_enable_Human_sensor;
                faceDetectorEnabled = Properties.Settings.Default.C2_enable_face_recognition;
            }
            else if (this.INDEX == 2)
            {
                recordingWhenOperation = Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation;
                IRSensorEnabled = Properties.Settings.Default.C3_enable_Human_sensor;
                faceDetectorEnabled = Properties.Settings.Default.C3_enable_face_recognition;
            }
            else if (this.INDEX == 3)
            {
                recordingWhenOperation = Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation;
                IRSensorEnabled = Properties.Settings.Default.C4_enable_Human_sensor;
                faceDetectorEnabled = Properties.Settings.Default.C4_enable_face_recognition;
            }

            OPER_BAN = false;
            Logger.Add("No_opcap_timer_Elapsed, OPER_BAN (operator capture ban) set " + OPER_BAN);
            if (wait_interval_enabled)
            {
                no_opcap_timer.Enabled = false;
                wait_interval_enabled = false;
                if (recordingWhenOperation)
                {
                    MainForm.Mklisteners.AddMouseAndKeyboardBack();
                }

                if (IRSensorEnabled)
                {
                    MainForm.RSensor.Start_IR_Timer();
                }
                else if (faceDetectorEnabled)
                {
                    window.FaceDetector.StartFaceTimer();
                }
            }
        }

        internal Bitmap GetBitmap()
        {
            Bitmap bitmap = null;
            if(recorder!=null)
            {
                try
                {
                    if (recorder.ON || PREEVENT_RECORDING)
                    {
                        bitmap = recorder.GetBitmap();
                    }
                    else
                    {
                        bitmap = camera.GetBitmap();
                    }
                }
                catch (NullReferenceException nrx)
                {
                    Logger.Add(nrx);
                }
            }
            return bitmap;
        }

        internal void PreviewMode()
        {
            //recording_permission = true;
            Recording_is_on = false;
            //window.picbox_recording.Visible = false; // TODO: useless ?
            window.recordingInProgress = false;
            PREEVENT_RECORDING = false;
            if (MainForm.GetMainForm != null)
            {
                Logger.Add("PREVIEW MODE");
                if (recorder != null && recorder.ON)
                {
                    recorder.ReleaseInterfaces();
                }
                if (camera == null || !camera.ON)
                {
                    camera = new UsbCamera(this.INDEX, this.parentwindow);
                    camera.Start();
                }
                else if (camera.Size.Width != PROPERTY_FUNCTIONS.GetCameraSize(0).Width && camera.Size.Height != PROPERTY_FUNCTIONS.GetCameraSize(0).Height)
                {
                    camera.Release();
                    camera = new UsbCamera(this.INDEX, this.parentwindow);
                    camera.Start();
                }
                else
                {
                    camera.Start();
                }
            }
        }

        internal void RecordingMode(int index)
        {
            Recording_is_on = true;
            if (camera != null && camera.ON)
            {
                camera.Release();                    
                recorder.StartRecorderCamera(index);
            }
            else if(recorder!=null && recorder.CAMERA_MODE==CAMERA_MODES.MANUAL)
            {
                Task manual_rec_task = new Task(VideoRecordingEnd);
                manual_rec_task.Start();
            }
            else if (camera != null && camera.Size.Width != PROPERTY_FUNCTIONS.GetCameraSize(index).Width && camera.Size.Height != PROPERTY_FUNCTIONS.GetCameraSize(index).Height)
            {
                recorder.ReleaseInterfaces();
                recorder = new RecorderCamera(index, this.parentwindow, this);
                recorder.StartRecorderCamera(index);
            }
            else
            {
                recorder.StartRecorderCamera(index);
            }
        }

        public bool GetRecordingState()
        {
            return Recording_is_on;
        }
            
        public void StartTimer()
        {
            if (the_timer!=null)
            {
                the_timer.Enabled = true;
                Logger.Add("THE_TIMER started " + the_timer.Interval);
            }            
        }

        public void Start(int index, CAMERA_MODES mode)
        {
            Logger.Add(mode.ToString());
            int duration = 0;
            bool eventRecorderEnabled = false, operatorCaptureEnabled = false;
            int timeAfterEventForEventRecorder = 0, secondsAfterEvent = 0;

            if (index == 0)
            {
                eventRecorderEnabled = Properties.Settings.Default.C1_enable_event_recorder;
                timeAfterEventForEventRecorder = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_after_event);
                operatorCaptureEnabled = Properties.Settings.Default.C1_enable_capture_operator;
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
            }
            else if (index == 1)
            {
                eventRecorderEnabled = Properties.Settings.Default.C2_enable_event_recorder;
                timeAfterEventForEventRecorder = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_after_event);
                operatorCaptureEnabled = Properties.Settings.Default.C2_enable_capture_operator;
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
            }
            else if (index == 2)
            {
                eventRecorderEnabled = Properties.Settings.Default.C3_enable_event_recorder;
                timeAfterEventForEventRecorder = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_after_event);
                operatorCaptureEnabled = Properties.Settings.Default.C3_enable_capture_operator;
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
            }
            else if (index == 3)
            {
                eventRecorderEnabled = Properties.Settings.Default.C4_enable_event_recorder;
                timeAfterEventForEventRecorder = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_after_event);
                operatorCaptureEnabled = Properties.Settings.Default.C4_enable_capture_operator;
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
            }

            switch (mode)
            {
                case CAMERA_MODES.MANUAL:

                    if (Properties.Settings.Default.manual_record_time > 0)
                    {
                        if (index == 0)
                        {
                            window.SET_REC_ICON();
                        }
                        //decimal mrm = Properties.Settings.Default.manual_record_time;
                        //This does not check if the recording is on, as it prioritizes the manual recording
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(this.INDEX, parentwindow, this);
                        recorder.CAMERA_MODE = CAMERA_MODES.MANUAL;
                        recorder.ACTIVE_RECPATH = RECORD_PATH.MANUAL;
                        duration = decimal.ToInt32(Properties.Settings.Default.manual_record_time) * 1000;
                        the_timer.Enabled = true;
                        the_timer.Interval = duration + 2;
                        the_timer.Enabled = false;
                        
                        if (this != null)
                        {
                            this.RecordingMode(INDEX);
                        }
                    }
                    break;
                case CAMERA_MODES.EVENT:
                    //EVENT from parameters
                    if (camera != null && eventRecorderEnabled && timeAfterEventForEventRecorder > 0 && camera.ON)
                    {
                        window.SET_REC_ICON();
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(INDEX, parentwindow, this);
                        recorder.ACTIVE_RECPATH = RECORD_PATH.EVENT;
                        recorder.CAMERA_MODE = CAMERA_MODES.EVENT;
                        duration = timeAfterEventForEventRecorder * 1000;
                        Logger.Add(duration + " is the duration of " + recorder.CAMERA_MODE);

                        the_timer.Enabled = true;
                        the_timer.Interval = duration + 3;
                        the_timer.Enabled = false;
                        if (this != null)
                        {
                            this.RecordingMode(INDEX);
                        }
                    }
                    break;
                case CAMERA_MODES.OPERATOR:
                    //EVENT from parameters                    
                    if (camera != null && operatorCaptureEnabled && secondsAfterEvent > 0 && camera.ON && !no_opcap_timer.Enabled)
                    {
                        window.SET_REC_ICON();
                        duration = secondsAfterEvent * 1000;
                        Logger.Add(duration + "  " + recorder.CAMERA_MODE);
                        the_timer.Enabled = true;
                        the_timer.Interval = duration + 3;
                        the_timer.Enabled = false;
                        Logger.Add(no_opcap_timer.Interval.ToString());
                        if (this != null)
                        {
                            recorder.ACTIVE_RECPATH = RECORD_PATH.EVENT;
                            recorder.CAMERA_MODE = CAMERA_MODES.OPERATOR;
                            wait_interval_enabled = true;
                            this.OPER_BAN = true;
                            Logger.Add("OPERATOR BAN in CAMERA_MODE.OPERATOR : " + OPER_BAN);
                            this.RecordingMode(INDEX);
                        }
                    }
                    break;
                case CAMERA_MODES.PREVIEW:
                    // preview only (no buffer file)
                    if (recorder.CAMERA_MODE != CAMERA_MODES.PREVIEW)
                    {
                        PREEVENT_RECORDING = false;
                        duration = 0;
                        the_timer.Stop();
                        recorder.ACTIVE_RECPATH = "";
                        recorder.ReleaseInterfaces();                        
                    }
                    if (this != null)
                    {
                        this.PreviewMode();
                    }
                    break;
                case CAMERA_MODES.PREEVENT:
                    // cycle with buffer files
                    PREEVENT_RECORDING = true;
                    duration = BUFFER_DURATION.BUFFERDURATION;
                    recorder.ReleaseInterfaces();
                    recorder = new RecorderCamera(INDEX, parentwindow, this);
                    recorder.ACTIVE_RECPATH = Properties.Settings.Default.temp_folder;
                    recorder.CAMERA_MODE = CAMERA_MODES.PREEVENT;
                    the_timer.Enabled = true;
                    the_timer.Interval = duration;
                    //the_timer.Enabled = false;
                    
                    RecordingMode(INDEX);
                    
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
            }
            catch (NullReferenceException nrx)
            {
                Logger.Add(nrx);
            }
        }

        internal void ReleaseCameras()
        {
            Logger.Add("Release camera");
            try
            {
                the_timer.Dispose();
                no_opcap_timer.Dispose();
                icon_timer.Dispose();
                camera.Release();
                recorder.ReleaseInterfaces();
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }
        }

        internal void ReleaseSecondaryCamera()
        {
            Logger.Add("Release secondary camera");
            try
            {
                camera.Release();
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }
        }
    }
}