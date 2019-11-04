using System;
using System.Drawing;
using FaceDetectionX;
using System.Windows.Forms;

namespace FaceDetection
{
    public class CROSSBAR
    {
        private static CROSSBAR crossbar = null;
        public bool PREEVENT_RECORDING = false;
        private RecorderCamera recorder = null;
        private UsbCamera camera = null;
        /// <summary>
        /// Final path combination of all path parameters
        /// </summary>
        string targetPath = String.Empty;
        // This is the synchronization point that prevents events
        // from running concurrently, and prevents the main thread 
        // from executing code after the Stop method until any 
        // event handlers are done executing.
        private static int syncPoint = 0;
        // Count the number of times the thread that calls Stop
        // has to wait for an Elapsed event to finish.
        private static int numWaits = 0;
        // Count the number of times the event handler is called,
        // is executed, is skipped, or is called after Stop.
        private static int numEvents = 0;
        private static int numExecuted = 0;
        private static int numSkipped = 0;
        private static int numLate = 0;

        internal int INDEX = 0;
        //BOOLEAN
        bool OPER_BAN = false;
        
        static bool wait_interval_enabled = false;
        static int duration;
        //static int wait;

        /// <summary>
        /// Recording is in process
        /// </summary>
        static bool recording_is_on = false;
        /// <summary>
        /// Recording allowed. Overrides allowed
        /// </summary>
        //static bool recording_permission = false;
        
        //TIMERS, 2 timers only
        static Timer the_timer = new Timer();
        /// <summary>
        /// No operator capturing during this period.
        /// <see cref="OPER_BAN"/>
        /// </summary>
        static System.Timers.Timer no_opcap_timer = new System.Timers.Timer();

        
        /// <summary>
        ///All cameras have the same modes,
        ///but only the Main camera supports operator capture.         
        ///(各カメラには異なるモードがあります メインカメラのみがオペレータキャプチャをサポートしています)
        /// Camera modes to assist identify current status 
        /// of the application during operation.
        /// Manage it properly, carefully, 
        /// so there are no confusions or contradictions.
        /// Available choices
        /// </summary>
        public enum CAMERA_MODES
        {
            PREVIEW,
            HIDDEN,
            EVENT,
            OPERATOR,
            MANUAL,
            PREEVENT
        }
        internal class RECPATH
        {
            public const string PHOTO = "snapshot";
            public const string MANUAL = "movie";
            public const string EVENT = "event";
            public const string TEMP = "temp";
        }
        

        public CROSSBAR()
        {
            crossbar = this;
            this.INDEX = 0;
            no_opcap_timer.AutoReset = false;
            no_opcap_timer.Elapsed += No_opcap_timer_Elapsed;
            no_opcap_timer.Enabled = true;
            //it is the sum of the 2 values
            no_opcap_timer.Interval = decimal.ToInt32(
                Properties.Settings.Default.interval_before_reinitiating_recording + 
                Properties.Settings.Default.seconds_after_event) 
                *1000;
            no_opcap_timer.Enabled = false;

            the_timer.Tick += The_timer_Tick;

            recorder = new RecorderCamera(0);
        }

        private void The_timer_Tick(object sender, EventArgs e)
        {
            CustomMessage.ShowMessage("The_timer_Elapsed " + duration + " is duration and " + "the_timer.Enabled: " + the_timer.Enabled);

            //recording_permission = true;

            //We end the recording here
            if (!PREEVENT_RECORDING)
            {
                recorder.ReleaseInterfaces();                
                CustomMessage.ShowMessage("not ======>>> PREEVENT*PREEVENT*PREEVENT mode");
                the_timer.Enabled = false;
                MainForm.Or_pb_recording.Visible = false;
                if (CROSSBAR.crossbar != null)
                    CROSSBAR.crossbar.PreviewMode();                
            }
            else if (PREEVENT_RECORDING)
            {
                //loop
                recorder.ReleaseInterfaces();
                recorder = new RecorderCamera(0);
                recorder.ACTIVE_RECPATH = Properties.Settings.Default.temp_folder;
                recorder.CAMERA_MODE = RecorderCamera.CAMERA_MODES.PREEVENT;
                CROSSBAR.crossbar.RecordingMode();
            }
            if (wait_interval_enabled)
            {
                //Run the timer
                
                no_opcap_timer.Interval = decimal.ToInt32(
                Properties.Settings.Default.interval_before_reinitiating_recording +
                Properties.Settings.Default.seconds_after_event)
                * 1000;
                no_opcap_timer.Enabled = true;
            }
        }

        private void No_opcap_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OPER_BAN = false;
            CustomMessage.ShowMessage("No_opcap_timer_Elapsed, OPER_BAN " + OPER_BAN);
            if (wait_interval_enabled)
            {
                no_opcap_timer.Enabled = false;
                wait_interval_enabled = false;
                CustomMessage.ShowMessage(wait_interval_enabled + "wait_interval_enabled");
                MainForm.GetMainForm.AddMouseAndKeyboardBack();
            }
        }

        internal Bitmap GetBitmap()
        {
            if (recording_is_on)
                return recorder.GetBitmap();
            else
                return camera.GetBitmap();
        }
                
        internal void PreviewMode()
        {
            //recording_permission = true;
            recording_is_on = false;
            MainForm.Or_pb_recording.Visible = false;
            PREEVENT_RECORDING = false;
            if (MainForm.GetMainForm != null)
            {             
                CustomMessage.ShowMessage("TODO: PREVIEW MODE");
                if(recorder!=null && recorder.ON)
                {
                    recorder.ReleaseInterfaces();
                    
                }
                if(camera==null)
                {
                    camera = new UsbCamera(0);
                    camera.Start();
                }else if (!camera.ON)
                {
                    camera = new UsbCamera(0);
                    camera.Start();
                }
                
            }
        }
        internal void RecordingMode()
        {            
            recording_is_on = true;
            
            if (MainForm.GetMainForm != null)
            {                
                if(camera!=null && camera.ON)
                {
                    camera.Release();
                }
                recorder.StartRecorderCamera();
            }
        }
        public void Start(int index, CAMERA_MODES mode)
        {
            this.INDEX = index;
            CustomMessage.ShowMessage(mode.ToString());
            int duration = 0;

            switch (mode)
            {
                case CAMERA_MODES.MANUAL:
                    
                    if (Properties.Settings.Default.manual_record_maxtime > 0)
                    {
                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        CustomMessage.ShowMessage(camera.ON + " camera ON?");
                        //This does not check if the recor5ding is on, as it prioritizes the manual recording
                        recorder.ACTIVE_RECPATH = RECPATH.MANUAL;
                        duration = decimal.ToInt32(Properties.Settings.Default.manual_record_maxtime) * 1000;
                        //the_timer = new Timer();
                        the_timer.Interval = duration;
                        the_timer.Start();
                        if (CROSSBAR.crossbar != null)
                            CROSSBAR.crossbar.RecordingMode();
                    }                    
                    break;
                case CAMERA_MODES.EVENT:
                    
                    //EVENT from parameters
                    if (Properties.Settings.Default.enable_event_recorder && Properties.Settings.Default.event_record_time_after_event > 0 && camera.ON)
                    {
                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(0);
                        recorder.ACTIVE_RECPATH = RECPATH.EVENT;
                        duration = decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event) * 1000;
                        the_timer.Interval = duration;
                        the_timer.Enabled = true;
                        if (CROSSBAR.crossbar != null)
                            CROSSBAR.crossbar.RecordingMode();
                    }                    
                    break;
                case CAMERA_MODES.OPERATOR:
                    //EVENT from parameters                    
                    if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.seconds_after_event > 0 && camera.ON && !no_opcap_timer.Enabled)
                    {
                        MainForm.Or_pb_recording.Visible = Properties.Settings.Default.show_recording_icon;
                        recorder.ReleaseInterfaces();
                        recorder = new RecorderCamera(0);
                        duration = decimal.ToInt32(Properties.Settings.Default.seconds_after_event) * 1000;
                        the_timer.Interval = duration;
                        the_timer.Start();
                        if (CROSSBAR.crossbar != null)
                        {
                            CROSSBAR.crossbar.RecordingMode();
                            wait_interval_enabled = true;
                            CROSSBAR.crossbar.OPER_BAN = true;
                        }                            
                        recorder.ACTIVE_RECPATH = RECPATH.EVENT;                        
                    }                    
                    break;
                case CAMERA_MODES.PREVIEW:
                    //preview
                    PREEVENT_RECORDING = false;
                    duration = 0;
                    the_timer.Stop();
                    //the_timer.Enabled = false;
                    
                    recorder.ACTIVE_RECPATH = "";
                    recorder.ReleaseInterfaces();
                    if (CROSSBAR.crossbar != null)
                        CROSSBAR.crossbar.PreviewMode();
                    break;
                case CAMERA_MODES.PREEVENT:
                    //permanent cycle
                    PREEVENT_RECORDING = true;
                    //recording_permission = false;
                    duration = 1 * 10 * 1000;
                    recorder.ReleaseInterfaces();
                    recorder = new RecorderCamera(0);
                    recorder.ACTIVE_RECPATH = Properties.Settings.Default.temp_folder;
                    recorder.CAMERA_MODE = RecorderCamera.CAMERA_MODES.PREEVENT;
                    //CustomMessage.ShowMessage(recorder.CAMERA_MODE + " mode. Active path: " + recorder.ACTIVE_RECPATH);
                    the_timer.Interval = duration;
                    the_timer.Start();
                    if (CROSSBAR.crossbar != null)
                        CROSSBAR.crossbar.RecordingMode();
                    break;
            }
            //the_timer.Enabled = false;
        }

        internal void SetWindowPosition(Size size)
        {
            if (camera.ON)
            {
                camera.SetWindowPosition(size);
            }else if (recorder.ON)
            {
                recorder.SetWindowPosition(size);
            }
        }



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
        private void The_timer_Elapsed(object sender, EventHandler e)
        {
            
        }

        internal void ReleaseCameras()
        {
            CustomMessage.ShowMessage("TODO: Releasing all cameras here");
            try
            {
                the_timer.Dispose();
                no_opcap_timer.Dispose();
            }
            catch(Exception x)
            {
                Logger.Add(x);
            }
        }
    }
}