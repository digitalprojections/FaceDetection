﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public class BackLightController
    {
        int duration;
        int wait;
        Action Run;
        public static Action Destroy;
        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        private readonly System.Timers.Timer backlight_timer = new System.Timers.Timer();


        public void Start()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Elapsed += Backlight_timer_Tick1;
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
                backlight_timer.Enabled = true;
            }
        }

        private void Backlight_timer_Tick1(object sender, EventArgs e)
        {   
           OFF();
        }

        public void Restart()
        {
            
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Stop();
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
                backlight_timer.Start();
            }
            else
            {
                MessageBox.Show("backlight_offset is 0");
            }
        }

        public BackLightController()
        {
            if (Properties.Settings.Default.backlight_offset_mins > 0 && Properties.Settings.Default.enable_backlight_off_when_idle)
            {
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60 * 1000;
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
                mouse_event((int)(MouseEventFlags.MOVE), 0, 0, 0, UIntPtr.Zero);
            }
            catch (Exception x)
            {
                Logger.Add(x);
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
    }    
}
