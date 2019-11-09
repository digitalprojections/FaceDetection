using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers;

namespace FaceDetection
{
    class IRSensor
    {
        private int sensoroffcount = 0;
        private bool checkOK = true;
        System.Windows.Forms.Timer SensorCheckTimer = new System.Windows.Forms.Timer();
        //Mutex mutex = new Mutex();

        public bool CheckOK { get => checkOK; set => checkOK = value; }

        public IRSensor()
        {
            SensorClose();
            //init
            Init_IR_Timer();
            
        }
        public void Destroy()
        {
            Stop_IR_Timer();
            SensorCheckTimer.Dispose();
        }
        private void Init_IR_Timer()
        {
                
           if (Properties.Settings.Default.capture_operator && Properties.Settings.Default.enable_Human_sensor && decimal.ToInt32(Properties.Settings.Default.face_rec_interval)>0)
           {
                SensorCheckTimer.Tick += IR_Timer_Tick;
                //SensorCheckTimer.Elapsed+= IR_Timer_Tick;
                //SensorCheckTimer.AutoReset = true;
                SensorCheckTimer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
                SensorCheckTimer.Start();
            }
            else
            {
                
            }

        }

        private void IR_Timer_Tick(object sender, EventArgs e)
        {
            
            uint rval = CheckSensor();
            if (CheckOK && rval == 0)
            {
                CheckOK = false;
                //heat signature detected, stop timer                
                if (Properties.Settings.Default.capture_method == 0)
                {
                    
                    //initiate RECORD mode
                    if (MainForm.GetMainForm != null && MainForm.GetMainForm.crossbar.PREEVENT_RECORDING)
                    {
                        TaskManager.EventAppeared(RECORD_PATH.EVENT, 
                            1, 
                            decimal.ToInt32(Properties.Settings.Default.seconds_before_event), 
                            decimal.ToInt32(Properties.Settings.Default.seconds_after_event));                        
                    }
                    else
                    {
                        MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.OPERATOR);                                                
                    }
                    MainForm.GetMainForm.crossbar.SET_ICON_TIMER();
                }
             else
                {
                    SNAPSHOT_SAVER.TakeSnapShot(0);
                    
                }
                

                Logger.Add("IR SENSOR: Motion detected");
                if (Properties.Settings.Default.backlight_on_upon_face_rec)
                {
                    MainForm.GetMainForm.BackLight.ON();
                }
                MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
            }
            else
            {
                sensoroffcount++;
                if (sensoroffcount > decimal.ToInt32(Properties.Settings.Default.seconds_after_event))
                {
                    sensoroffcount = 0;
                    
                    //CheckOK = true;
                }
            }
            
            SensorClose();
            
            Console.WriteLine("SENSOR " + CheckOK);
            //mutex.ReleaseMutex();
        }

        public void Start_IR_Timer()
        {
            CheckOK = true;
        }
        

        public void Stop_IR_Timer()
        {
            SensorCheckTimer.Enabled = false;
            //SensorCheckTimer.Stop();
            CheckOK = false;

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
                 }
                catch(Exception e)
                {
                    data[1] = 0;
                    Logger.Add(e.Message + " IRSensor 91");
                }                
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
        static extern bool DispGetSensorRawValue(ref DISP_SENSOR_VALUE x, int y);
        [StructLayout(LayoutKind.Sequential)]
        private struct DISP_SENSOR_VALUE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] iValue;
        }

    }
}
