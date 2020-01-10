using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace FaceDetection
{
    class IRSensor:IDisposable
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
            int cameraIndex = MainForm.Setting_ui.Camera_index;
            bool operatorCaptureEnabled = false, IRSensorEnabled = false;
            int checkInterval = 0;

            switch (cameraIndex)
            {
                case 0:
                    operatorCaptureEnabled = Properties.Settings.Default.C1_enable_capture_operator;
                    IRSensorEnabled = Properties.Settings.Default.C1_enable_Human_sensor;
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C1_check_interval);
                    break;
                case 1:
                    operatorCaptureEnabled = Properties.Settings.Default.C2_enable_capture_operator;
                    IRSensorEnabled = Properties.Settings.Default.C2_enable_Human_sensor;
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C2_check_interval);
                    break;
                case 2:
                    operatorCaptureEnabled = Properties.Settings.Default.C3_enable_capture_operator;
                    IRSensorEnabled = Properties.Settings.Default.C3_enable_Human_sensor;
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C3_check_interval);
                    break;
                case 3:
                    operatorCaptureEnabled = Properties.Settings.Default.C4_enable_capture_operator;
                    IRSensorEnabled = Properties.Settings.Default.C4_enable_Human_sensor;
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C4_check_interval);
                    break;
            }

            if (operatorCaptureEnabled && IRSensorEnabled && checkInterval > 0)
            {
                SensorCheckTimer.Interval = checkInterval;
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
            int camindex = Properties.Settings.Default.main_camera_index;
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;
            string captureMethod = "";

            switch (camindex)
            {
                case 0:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                    
                    captureMethod = Properties.Settings.Default.C1_capture_type;
                    break;
                case 1:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
                    
                    captureMethod = Properties.Settings.Default.C2_capture_type;
                    break;
                case 2:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
                    
                    captureMethod = Properties.Settings.Default.C3_capture_type;
                    break;
                case 3:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
                    
                    captureMethod = Properties.Settings.Default.C4_capture_type;
                    break;
            }
            preeventRecording = MULTI_WINDOW.formList[camindex].crossbar.PREEVENT_RECORDING;

            if (MainForm.GetMainForm.InvokeRequired)
            {
                var d = new IRTimerTickDelegate(MotionDetected);
                MainForm.GetMainForm.Invoke(d);
            }
            else
            {
                if (MULTI_WINDOW.formList[camindex].crossbar.OPER_BAN == false)
                {
                    if (captureMethod != "Snapshot") // Video
                    {
                        //initiate RECORD mode
                        if (MainForm.GetMainForm != null && preeventRecording)
                        {
                            if (MainForm.GetMainForm.AnyRecordingInProgress == false)
                            {
                                TaskManager.EventAppeared(RECORD_PATH.EVENT, camindex+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);

                                if (camindex == 0)
                                {
                                    MULTI_WINDOW.formList[camindex].crossbar.SetIconTimer(timeAfterEvent);
                                    MULTI_WINDOW.formList[camindex].crossbar.No_Cap_Timer_ON(timeAfterEvent);
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[camindex].SetRecordIcon(camindex, timeAfterEvent);
                                }
                            }
                        }
                        else
                        {
                            MULTI_WINDOW.formList[camindex].crossbar.Start(camindex, CAMERA_MODES.OPERATOR);
                        }
                    }
                    else // Snapshot
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(camindex, "event");

                        MULTI_WINDOW.formList[camindex].crossbar.No_Cap_Timer_ON(0);
                    }

                    Logger.Add("IR SENSOR: Motion detected");
                    if (Properties.Settings.Default.backlight_on_upon_face_rec)
                    {
                        MainForm.GetMainForm.BackLight.ON();
                    }
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
            bIsIRCheckExec = false;
        }

        public void Destroy()
        {
            Stop_IR_Timer();
            Dispose();
        }

        public void SetInterval()
        {
            int camindex = Properties.Settings.Default.main_camera_index;
            int checkInterval = 0;

            switch (camindex)
            {
                case 0:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C1_check_interval);
                    break;
                case 1:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C2_check_interval);
                    break;
                case 2:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C3_check_interval);
                    break;
                case 3:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C4_check_interval);
                    break;
            }

            SensorCheckTimer.Enabled = true;
            SensorCheckTimer.Interval = checkInterval;
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
        
        public void Dispose()
        {
            ((IDisposable)SensorCheckTimer).Dispose();
        }
    }
}
