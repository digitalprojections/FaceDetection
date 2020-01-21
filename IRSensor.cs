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
            int cameraIndex = MainForm.Settingui.Camera_index;
            PROPERTY_FUNCTIONS.GetCaptureOperatorSwitch(cameraIndex, out bool operatorCaptureEnabled);
            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(cameraIndex, out bool IRSensorEnabled);
            PROPERTY_FUNCTIONS.GetSensorCheckInterval(cameraIndex, out int checkInterval);
            
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
            int INDEX = Properties.Settings.Default.main_camera_index;
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;
            string captureMethod = "";

            PROPERTY_FUNCTIONS.GetSecondsBeforeEvent(INDEX, out timeBeforeEvent);
            PROPERTY_FUNCTIONS.GetSecondsAfterEvent(INDEX, out timeAfterEvent);

            preeventRecording = MULTI_WINDOW.formList[INDEX].crossbar.PREEVENT_RECORDING;

            if (MainForm.GetMainForm.InvokeRequired)
            {
                var d = new IRTimerTickDelegate(MotionDetected);
                MainForm.GetMainForm.Invoke(d);
            }
            else
            {
                if (MULTI_WINDOW.formList[INDEX].crossbar.OPER_BAN == false)
                {
                    if (captureMethod != "Snapshot") // Video
                    {
                        //initiate RECORD mode
                        if (MainForm.GetMainForm != null && preeventRecording)
                        {
                            if (MainForm.AnyRecordingInProgress == false)
                            {
                                TaskManager.EventAppeared(RECORD_PATH.EVENT, INDEX+1, timeBeforeEvent, timeAfterEvent, DateTime.Now);

                                if (INDEX == Properties.Settings.Default.main_camera_index)
                                {
                                    MULTI_WINDOW.formList[INDEX].crossbar.SetIconTimer(timeAfterEvent);
                                    MULTI_WINDOW.formList[INDEX].crossbar.NoCapTimerON(timeAfterEvent);
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[INDEX].SetRecordIcon(INDEX, timeAfterEvent); //SHOULD NOT BE HIT. REMOVE THIS LINE
                                }
                            }
                        }
                        else
                        {
                            MULTI_WINDOW.formList[INDEX].crossbar.Start(INDEX, CAMERA_MODES.OPERATOR);
                            MULTI_WINDOW.formList[INDEX].crossbar.SetIconTimer(timeAfterEvent);
                            MULTI_WINDOW.formList[INDEX].crossbar.NoCapTimerON(timeAfterEvent);
                        }
                    }
                    else // Snapshot
                    {
                        SNAPSHOT_SAVER.TakeSnapShot(INDEX, "event");
                        MULTI_WINDOW.formList[INDEX].crossbar.NoCapTimerON(0);//Only the no-capture switch is enough
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

            PROPERTY_FUNCTIONS.GetSensorCheckInterval(camindex, out int checkInterval);

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
