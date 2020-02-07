using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

namespace FaceDetection
{
    public class BackLightController:IDisposable
    {
                
        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        private readonly System.Timers.Timer backlight_timer = new System.Timers.Timer();
        //Stopwatch stopwatch = new Stopwatch();
        public BackLightController()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
            }
        }

        public void Start()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Elapsed += Backlight_timer_Tick1;
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
                backlight_timer.Enabled = true;
            }
        }

        private void Backlight_timer_Tick1(object sender, ElapsedEventArgs e)
        {
            OFF();
        }

        public void Restart()
        { 
            
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                //backlight_timer.Stop();
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
                if(backlight_timer!=null)
                    backlight_timer.Start();
            }
            else
            {
                if (backlight_timer != null)
                    backlight_timer.Stop();
                //MessageBox.Show("backlight_offset is 0");
            }
        }

        
       
        internal void OFF()
        {
            BacklightOff();
        }
        internal void ON()
        {
            BacklightOn();
            Restart();
        }
        /// <summary>
        /// Turns backlight off
        /// </summary>
        public void BacklightOff()
        {            
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                int hr = SendMessage(0xFFFF, 0x112, 0xF170, (int)MonitorState.MonitorStateOff);

            }
            else
            {
                //MessageBox.Show("backlight_offset is 0");
            }
        }

        public static void BacklightOn()
        {
            try
            {
                mouse_event((int)(MouseEventFlags.MOVE), 0, 0, 0, UIntPtr.Zero);
            }
            catch (Exception x)
            {
                LOGGER.Add(x);
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
        /// <returns></returns>        /// 
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
                IntPtr hWnd,
                uint Msg,
                UIntPtr wParam,
                IntPtr lParam,
                SendMessageTimeoutFlags fuFlags,
                uint uTimeout,
                out UIntPtr lpdwResult);
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);


        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }
        
        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    backlight_timer.Stop();
                    backlight_timer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BackLightController()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }    
}
