using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    class BackLightController
    {
        static int duration;
        static int wait;

        static Action Run;
        public static Action Destroy;
        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        private static readonly System.Timers.Timer backlight_timer = new System.Timers.Timer();


        public static void Start()
        {            
            Run = () =>
            {
                ON();
                CustomMessage.ShowMessage("Running in " + " mode");
                backlight_timer.Enabled = true;
            };
            Destroy = () =>
            {
                CustomMessage.ShowMessage("Destroyed");
                backlight_timer.Enabled = false;
                backlight_timer.Dispose();
            };

            if (backlight_timer.Enabled)
            {

            }
            else
            {
                //we got the same task as before, while running the camera
                
            }
            //backlight_timer.Enabled = true;
            backlight_timer.AutoReset = false; //prevent from running - true
            backlight_timer.Elapsed += The_timer_Elapsed;
           
            //backlight_timer.Enabled = false;

            Run();
        }

        public static void Restart()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0)
            {
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
                bool  cb = backlight_timer.Enabled;
            }
            else
            {
                MessageBox.Show("backlight_offset is 0");
            }
        }

        public BackLightController()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0)
            {
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
            }
        }
        private void Backlight_timer_Tick(object sender, EventArgs e)
        {
            Trace.WriteLine("back light off called");
            BacklightOff();
        }
        internal static void OFF()
        {
            BacklightOff();
        }
        internal static void ON()
        {
            //backlight_timer.Enabled = Properties.Settings.Default.enable_backlight_off_when_idle;
            BacklightOn();
            Restart();
        }
        /// <summary>
        /// Turns backlight off
        /// </summary>
        public static void BacklightOff()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0)
            {
                SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOff);
            }
            else
            {
                MessageBox.Show("backlight_offset is 0");
            }
        }

        public static void BacklightOn()
        {
            try
            {
                UIntPtr hr = UIntPtr.Zero;
                SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOn);
                //SendMessageTimeout(MainForm.GetMainForm.Handle, 0112, UIntPtr.Zero, (IntPtr)MonitorState.MonitorStateOn, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out hr);
                CustomMessage.ShowMessage(hr + " backlight on request");
            }
            catch (Exception x)
            {
                Logger.Add(x);
                CustomMessage.ShowMessage(x.Message + " 661");
            }
        }

        private static void The_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CustomMessage.ShowMessage(duration + " " + wait + " the_timer.Enabled: " + backlight_timer.Enabled);
            if (wait > 0)
            {
                OFF();
                backlight_timer.Enabled = true;
                backlight_timer.Interval = (int)wait;
                wait = 0;
                //the_timer.Enabled = false;
                //
            }
            else
            {
                OFF();
                backlight_timer.Enabled = false;
                backlight_timer.AutoReset = false;
                //wait also ran or was not set
                //task complete                
                Destroy();
            }

        }
        
        /// <summary>
        /// Backlight OFF
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        /// <summary>
        /// Backlight ON request
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="fuFlags"></param>
        /// <param name="uTimeout"></param>
        /// <param name="lpdwResult"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
                IntPtr hWnd,
                uint Msg,
                UIntPtr wParam,
                IntPtr lParam,
                SendMessageTimeoutFlags fuFlags,
                uint uTimeout,
                out UIntPtr lpdwResult);

        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }
    }    
}
