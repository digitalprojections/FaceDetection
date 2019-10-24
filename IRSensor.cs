using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    class IRSensor
    {
        Timer OM_Timer = new Timer();
        public IRSensor()
        {
            //init
            InitOMTimer();
        }
        private void InitOMTimer()
        {
                DialogResult dialogResult = MessageBox.Show("Does this PC have an IR Sensor?", "Please, choose operation mode", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    OM_Timer.Tick += OM_Timer_Tick;
                    OM_Timer.Interval = 1000;
                }
                else if (dialogResult == DialogResult.No)
                {
                //do something else
                Properties.Settings.Default.use_ir_sensor = false;
                Properties.Settings.Default.Save();
                }
            
            
        }

        private void OM_Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("IR Sensor TIMER tick: " + e.ToString());
            if (MainForm.Or_testparam.Text != "sensor check")
                MainForm.Or_testparam.Text = "sensor check";
            else
                MainForm.Or_testparam.Text = "";
            uint rval = CheckSensor();
            if (rval == 1)
            {
                //heat signature detected, stop timer
                StopOM_Timer();
                
                //initiate RECORD mode
                if (MainForm.GetMainForm != null)
                {                    
                    MainForm.GetMainForm.RecordMode();
                    
                }  
            }

        }

        public void StartOM_Timer()
        {
            OM_Timer.Start();
        }
        

        public void StopOM_Timer()
        {
            OM_Timer.Stop();
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
                catch
                {
                    data[1] = 0;
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
