using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    class IRSensor
    {
        Timer OneSecondTimer = new Timer();
        public IRSensor()
        {
            //init
            InitOMTimer();
        }
        private void InitOMTimer()
        {
                
                if (Properties.Settings.Default.capture_operator)
                {
                    OneSecondTimer.Tick += OM_Timer_Tick;
                    OneSecondTimer.Interval = 1000;
                }

        }

        private void OM_Timer_Tick(object sender, EventArgs e)
        {            
            uint rval = CheckSensor();
            if (rval == 1)
            {
                //heat signature detected, stop timer
                StopOM_Timer();
                //initiate RECORD mode
                if (MainForm.GetMainForm != null && RecorderCamera.CAMERA_MODE== RecorderCamera.CAMERA_MODES.PREVIEW)
                {
                    //MainForm.ACTIVE_RECPATH = MainForm.RECPATH.EVENT;
                    MainForm.GetMainForm.RecordMode();
                }
                else
                {
                    TaskManager.EventAppeared("EVENT", decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
                }
            }
            SensorClose();

        }

        public void StartOM_Timer()
        {
            OneSecondTimer.Start();
        }
        

        public void StopOM_Timer()
        {
            OneSecondTimer.Stop();
            //OM_Timer.AutoReset = false;
            //OM_Timer.SynchronizingObject = MainForm.GetMainForm;
        }
        
        public uint CheckSensor()
        {
            bool ret = false;
            int iError = 0;
            uint[] data = new uint[6];
            DISP_SENSOR_VALUE stSensorValue = new DISP_SENSOR_VALUE();
            if (DispDeviceOpen())
            {
                try
                {
                    ret = DispGetSensorRawValue(ref stSensorValue, iError);
                    var ivals = String.Empty;
                    if (stSensorValue.iValue != null)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            ivals += stSensorValue.iValue[i] + ", ";
                        }
                    }
                    data[0] = Convert.ToUInt32((stSensorValue.iValue[0] & 0x00FF) >> 0);     // データ有無・種別 (0:データなし, 1:AK9753, 2:AK9754)
                    data[1] = Convert.ToUInt32((stSensorValue.iValue[0] & 0xFF00) >> 8);        // 検出結果 (0:非検出, 1:検出)
                    data[2] = Convert.ToUInt32((stSensorValue.iValue[1] & 0x00FF) >> 0);     // IR data (L)
                    data[3] = Convert.ToUInt32((stSensorValue.iValue[1] & 0xFF00) >> 8);     // IR data (H)
                    data[4] = Convert.ToUInt32((stSensorValue.iValue[2] & 0x00FF) >> 0);     // TMP data (L)
                    data[5] = Convert.ToUInt32((stSensorValue.iValue[2] & 0xFF00) >> 8);        // TMP data (H)
                    //Console.WriteLine(ret + "\n" + ivals + "\n" + data[0] + "\n" + data[1] + "\n" + data[2] + "\n" + data[3] + "\n" + data[4] + "\n" + data[5] + "\n");
                }
                catch(Exception e)
                {
                    data[1] = 0;
                    Console.WriteLine(e.Message + " IRSensor 91");
                }
                
            }
            else
            {             
                data[1] = 0;                
            }
            //rtb.ScrollToCaret();
            return data[1];
        }
        public void SensorClose()
        {
            DispDeviceClose();
        }


        [DllImport("DispApi.dll")]
        static extern bool DispDeviceOpen();
        [DllImport("DispApi.dll")]
        static extern bool DispDeviceClose();
        [DllImport("DispApi.dll")]
        static extern bool DispGetSensorRawValue(ref DISP_SENSOR_VALUE x, int y);
        [StructLayout(LayoutKind.Sequential)]
        private struct DISP_SENSOR_VALUE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] iValue;
        }

    }
}
