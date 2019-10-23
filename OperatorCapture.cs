using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class OperatorCapture
    {
        private static Timer record_length_timer = new Timer();
        private static Timer interval_timer = new Timer();

        private static bool timeOut = false;
        private static bool recording = false;

        public static void Start()
        {
            //Initiate a recording camera or simply build a view mode to capture snapshots
            //at given intervals

            if (!timeOut || !recording)
            {
                SetupTimer();
                if(Properties.Settings.Default.capture_type == "Video")
                {
                    //Initiate video

                }else
                {
                    //Snapshot
                }
            }
        }

        public static void SetupTimer()
        {
            record_length_timer.Interval = Convert.ToInt32(Properties.Settings.Default.recording_length_seconds);
            interval_timer.Interval = Convert.ToInt32(Properties.Settings.Default.interval_before_reinitiating_recording);

            record_length_timer.Tick += Record_length_timer_Tick;
            interval_timer.Tick += Interval_timer_Tick;

            record_length_timer.Start();
            recording = true;
        }

        private static void Interval_timer_Tick(object sender, EventArgs e)
        {
            timeOut = false;
            interval_timer.Stop();
        }

        private static void Record_length_timer_Tick(object sender, EventArgs e)
        {            
            recording = false;
            timeOut = true;
            record_length_timer.Stop();
            interval_timer.Start();
        }
    }
}
