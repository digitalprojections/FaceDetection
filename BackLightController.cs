﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FaceDetection
{
    class BackLightController
    {
        //private FormSettings settingsBase = Properties.Camera1.Default;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        private static readonly System.Timers.Timer backlight_timer = new System.Timers.Timer();
        
        public BackLightController()
        {            
            backlight_timer.Elapsed += Backlight_timer_Tick;
        }
        internal static void Init()
        {
            
            if (Properties.Settings.Default.backlight_offset_mins > 0)
            {
                backlight_timer.Interval = decimal.ToInt32(Properties.Settings.Default.backlight_offset_mins) * 60000;
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
            backlight_timer.Enabled = Properties.Settings.Default.enable_backlight_off_when_idle;
            BacklightOn();
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
        }

        public static void BacklightOn()
        {
            try
            {
                UIntPtr hr = UIntPtr.Zero;
                SendMessageTimeout(MainForm.GetMainForm.Handle, 0x112, UIntPtr.Zero, (IntPtr)MonitorState.MonitorStateOn, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out hr);
                Console.WriteLine(hr + " backlight on request");
            }
            catch (Exception x)
            {
                Logger.Add(x);
                Console.WriteLine(x.Message + " 661");
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