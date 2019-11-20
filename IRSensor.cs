using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace FaceDetection
{
    class IRSensor
    {
        private delegate void IRTimerTickDelegate();
        System.Timers.Timer SensorCheckTimer = new System.Timers.Timer();

        static bool bIsIRCheck = true;

        public bool bIsIRCheckExec { get => bIsIRCheck; set => bIsIRCheck = value; }

        public IRSensor()
        {
            bIsIRCheckExec = true;
            //DispDeviceOpen();
            //SensorClose();
            //init
            Init_IR_Timer();
            SensorCheckTimer.Elapsed += IR_Timer_Tick;
            SensorCheckTimer.AutoReset = true;
        }
        
        private void Init_IR_Timer()
        {
           if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.enable_Human_sensor && decimal.ToInt32(Properties.Settings.Default.face_rec_interval)>0)
           {
                //SensorCheckTimer.Tick += IR_Timer_Tick;
                
                SensorCheckTimer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            }
            else
            {
                Stop_IR_Timer();
            }
        }

        private void IR_Timer_Tick(object sender, ElapsedEventArgs e)
        {
            uint rval = CheckSensor();
            if (bIsIRCheckExec)
            {
                if ( rval == 1 )
                {
                    bIsIRCheckExec = false;
                    Stop_IR_Timer();
                    //heat signature detected, stop timer
                    MotionDetected();
                }
            }            
            //Logger.Add("SENSOR " + bIsIRCheckExec);
        }

        private void MotionDetected()
        {
            if (MainForm.GetMainForm.InvokeRequired)
            {
                var d = new IRTimerTickDelegate(MotionDetected);
                MainForm.GetMainForm.Invoke(d);
            }
                else
            {
                if (!MainForm.GetMainForm.crossbar.OPER_BAN)
                {
                    if (Properties.Settings.Default.capture_method <= 0)
                    {
                        //initiate RECORD mode
                        if (MainForm.GetMainForm != null && MainForm.GetMainForm.crossbar.PREEVENT_RECORDING)
                        {
                            TaskManager.EventAppeared(RECORD_PATH.EVENT,
                                1,
                                decimal.ToInt32(Properties.Settings.Default.seconds_before_event),
                                decimal.ToInt32(Properties.Settings.Default.seconds_after_event),
                                DateTime.Now);
                            if (Properties.Settings.Default.capture_method == 0)
                            {
                                MainForm.GetMainForm.SET_REC_ICON();
                            }
                        }
                        else
                        {
                            MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);
                        }
                        MainForm.GetMainForm.crossbar.SetIconTimer(Properties.Settings.Default.seconds_after_event);
                    }
                    else
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(0, "event");
                    }

                    Logger.Add("IR SENSOR: Motion detected");
                    if (Properties.Settings.Default.backlight_on_upon_face_rec)
                    {
                        MainForm.GetMainForm.BackLight.ON();
                    }
                    MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                }                
            }
        }

        public void Start_IR_Timer()
        {
            bIsIRCheckExec = true;
            SensorCheckTimer.Start();
        }
        

        public void Stop_IR_Timer()
        {            
            //SensorCheckTimer.Enabled = false;
            //SensorCheckTimer.Stop();
            bIsIRCheckExec = false;

        }
        public void Destroy()
        {
            Stop_IR_Timer();
            SensorCheckTimer.Dispose();
        }

        public void SetInterval()
        {
            SensorCheckTimer.Enabled = true;
            SensorCheckTimer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            SensorCheckTimer.Enabled = false;
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
                    ret = DispGetSensorRawValue(ref stSensorValue, ref iError);
                    var ivals = String.Empty;
                    if (ret == true)
                    {
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

                        Logger.Add("IRSENSOR VALUE DETECTED: 0 -> " + data[0] + ";\n 1 -> " + data[1] + ";\n 2 -> " + data[2] + ";\n 3-> " + data[3] + ";\n 4-> " + data[4] + ";\n 5-> " + data[5] + ";");
                    }
                    else
                    {
                        data[1] = 0;
                    }
                }
                catch (Exception e)
                {
                    data[1] = 0;
                    //Logger.Add(e.Message + " ******** IRSensor ERROR");
                }
                SensorClose();
            }
            else
            {             
                data[1] = 0;                
            }            

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
        static extern bool DispGetSensorRawValue(ref DISP_SENSOR_VALUE stSensor, ref int iErrors);

        [StructLayout(LayoutKind.Sequential)]
        private struct DISP_SENSOR_VALUE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] iValue;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static bool IsDllLoaded(string path)
        {
            return GetModuleHandle(path) != IntPtr.Zero;
        }

        
    }
}
