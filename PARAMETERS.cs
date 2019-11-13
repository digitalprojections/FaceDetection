using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    class PARAMETERS
    {
        public static List<string> PARAM;
        static string param;
        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            
            PARAM = parameters.ToList<string>();
           
            Logger.Add(param + "sdfsdfdsdgf");

            param = String.Concat(parameters).ToLower();
            //↓20191108 Nagayama added↓
            string elem;
            string method = "";
            bool switchOnOff = false;
            int cameraIndex = -1;
            int time = 0;
            //↑20191108 Nagayama added↑
            /*
             Handle the initial start up CL parameters, if exist
             */
            if (param.Contains("uvccameraviewer"))
            {
                if (parameters.Count > 0)
                {
                    //↓20191108 Nagayama added↓
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        elem = parameters.ElementAt(i).ToLower();
                        switch (elem.Substring(0, 1))
                        {
                            case "m":
                                method = elem.Substring(2);
                                break;
                            case "s":
                                switchOnOff = (elem.Substring(2) != "0");
                                break;
                            case "c":
                                cameraIndex = Int32.Parse(elem.Substring(2)) - 1;
                                break;
                            case "t":
                                time = Int32.Parse(elem.Substring(2));
                                break;
                        }
                    }
                    if(cameraIndex == -1)//カメラ番号が未入力の場合
                    {
                        if(MainForm.GetMainForm == null)
                        {
                            cameraIndex = 0;
                        }
                        else if(method == "n")
                        {
                            cameraIndex = GetNextCameraIndex(MainForm.SELECTED_CAMERA);
                        }
                        else
                        {
                            cameraIndex = MainForm.SELECTED_CAMERA;
                        }
                    }
                    //↑20191108 Nagayama added↑
                    switch (method)
                    {
                        case "":
                            if (CheckCameraIndex(cameraIndex))
                            {
                                if(cameraIndex != 8)
                                {


                                }else
                                {
                                    //the value is 9. Start all cameras
                                }
                            }
                            break;
                        case "c":
                            try
                            {
                                if (switchOnOff)
                                {


                                    if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                    {
                                        //settingUI.TopMost = true;
                                        MainForm.GetMainForm.TopMost = false;
                                        MainForm.Setting_ui.ShowDialog();
                                    }

                                }
                                else
                                {
                                    MainForm.Setting_ui.Hide();
                                    MainForm.AllChangesApply();

                                }
                            }

                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Trace.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "s":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    //Properties.Settings.Default.main_camera_index = cameraIndex;
                                    SNAPSHOT_SAVER.TakeSnapShot(cameraIndex);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.Message + " in line 286");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "b":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    Logger.Add("テスト" + parameters.ElementAt(0) + "×" + method + "×" + (switchOnOff ? "1" : "0") + "×" + (cameraIndex + 1));

                                    if (switchOnOff)
                                    {
                                        /*
                                        SHOW CONTROL BUTTONS    
                                        */
                                        MainForm.Or_controlBut.Visible = true;
                                    }
                                    else
                                    {
                                        /*
                                        HIDE CONTROL BUTTONS    
                                        */
                                        MainForm.Or_controlBut.Visible = false;
                                    }

                                }

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        case "d":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    if (switchOnOff)
                                    {
                                        if (MainForm.FaceDetector != null)
                                        {
                                            MainForm.FaceDetector.Start_Face_Timer();
                                        }
                                        Properties.Settings.Default.enable_face_recognition = true;
                                        Properties.Settings.Default.enable_Human_sensor = false;
                                        Properties.Settings.Default.capture_operator = true;
                                    }
                                    else
                                    {
                                        if (MainForm.FaceDetector != null && MainForm.GetMainForm.crossbar.Recording_is_on)
                                        {
                                            MainForm.GetMainForm.crossbar.PreviewMode();
                                            MainForm.FaceDetector.Stop_Face_Timer();
                                        }
                                        Properties.Settings.Default.enable_face_recognition = false;
                                    }

                                    CycleTime(time);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 387");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;
                        //kameyama beginning 20191018  
                        case "h":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    if (switchOnOff)
                                    {
                                        if (MainForm.RSensor != null) {
                                            MainForm.RSensor.Start_IR_Timer();
                                        }
                                        Properties.Settings.Default.enable_Human_sensor = true;
                                        Properties.Settings.Default.enable_face_recognition = false;
                                        Properties.Settings.Default.capture_operator = true;
                                    }
                                    else
                                    {
                                        if (MainForm.RSensor != null)
                                        {
                                            MainForm.RSensor.Stop_IR_Timer();
                                        }
                                        Properties.Settings.Default.enable_Human_sensor = false;
                                    }

                                    CycleTime(time);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " paramameters 399");
                            }
                            break;

                        case "v":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {

                                    if (switchOnOff)
                                    {
                                        MainForm.GetMainForm.TopMost = true;
                                        //settingUI.TopMost = false;
                                        MainForm.GetMainForm.Show();
                                    }
                                    else
                                    {
                                        MainForm.GetMainForm.Hide();
                                        MainForm.AllChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters");
                            }
                            break;

                        case "l":
                            try
                            {

                                if (switchOnOff)
                                {
                                    MainForm.GetMainForm.BackLight.ON();

                                }
                                else
                                {

                                    MainForm.GetMainForm.BackLight.OFF();
                                }

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                //MessageBox.Show("Incorrect or missing parameters");
                                Debug.WriteLine(e.ToString() + " in line 271");
                            }
                            break;
                        case "n":
                            try
                            {
                                //if (CheckCameraIndex(parameters))
                                //{
                                //    if (parameters.ElementAt(3) == "1")
                                //    {

                                //    }
                                //    else if (parameters.ElementAt(3) == "0")
                                //    {

                                //    }
                                //}
                            }
                            catch (Exception e)
                            {
                                Logger.Add(e.Message + " paramameters 466");
                            }
                            break;
                        case "w":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    if (switchOnOff)
                                    {

                                        //kameyama comennt 20191020
                                        //Properties.Settings.Default.show_window_pane = true;
                                        //FormChangesApply();
                                        MainForm.GetMainForm.FormBorderStyle = FormBorderStyle.Sizable;
                                        Properties.Settings.Default.show_window_pane = true;
                                        Properties.Settings.Default.Save();

                                        if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                        {
                                            //settingUI.TopMost = true;
                                            MainForm.GetMainForm.TopMost = false;
                                            //MainForm.Setting_ui.ShowDialog();
                                            Logger.Add("Properties.Settings.Default.show_window_pane " + Properties.Settings.Default.show_window_pane);
                                        }

                                    }
                                    else
                                    {
                                        Properties.Settings.Default.show_window_pane = false;
                                        Properties.Settings.Default.Save();
                                        MainForm.AllChangesApply();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "q":
                            try
                            {
                                //if (CheckCameraIndex(parameters))
                                //{
                                //    ManualRecordingOn(parameters.ElementAt(1));
                                //}
                                //else if (parameters.ElementAt(2) == "0")
                                //{
                                //    ManualRecordingOff(parameters.ElementAt(1));
                                //}



                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;
                        case "e":
                            try
                            {
                                /////////////////////////////////////
                                ///    dont CheckCameraIndex()    ///
                                /////////////////////////////////////
                                    if (switchOnOff)
                                    {
                                        MainForm.GetMainForm.EventRecorderOn();
                                    }
                                    else
                                    {
                                        MainForm.GetMainForm.EventRecorderOff();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in line 310");
                                //MessageBox.Show("Incorrect or missing parameters"); } 
                            }
                            break;

                    }
                }

            }
        }
        /// <summary>
        /// Camera number is correct if one of 1,2,3,4 or 9
        /// </summary>
        /// <param name="cameraIndex"></param>
        /// <returns></returns>
        private static bool CheckCameraIndex(int cameraIndex)
        {
            bool Retval = false;
            //cameraIndexは0から始まるので注意
            switch (cameraIndex)
            {
                case 0:
                    Retval = true;
                    Logger.Add("カメラ番号まで来た");
                    //Camera.CheckCamera(parameters.ElementAt());
                    break;
                case 1:
                    Retval = true;
                    break;
                case 2:
                    Retval = true;
                    break;
                case 3:
                    Retval = true;
                    break;
                case 8:
                    Retval = true;
                    break;
            }
            return Retval;
        }
        private static void SetWindowPane(bool value)
        {
            MainForm.GetMainForm.ControlBox = value;
        }
        private static void ManualRecordingOn(string camnum)
        {
            //StartVideoRecording();
        }

        private static void ManualRecordingOff(string camnum)
        {
            //StopVideoRecording();
        }
        /// <summary>
        /// No parameters
        /// </summary>
        public static void HandleParameters()
        {

        }
        
        //kameyama comment 20191019 end

        private static void CycleTime(int time)
        {
            if (time >= 500 && time <= 1000)
            {
                Properties.Settings.Default.face_rec_interval = time;
                Logger.Add("できた");
            }
        }
        private static int GetNextCameraIndex(int cameraIndex)
        {
            if(cameraIndex == 8)
            {
                //すべてのカメラ指定時
                //1から始まって9なので、0から始まると8になる
                return cameraIndex;
            }
            else if(cameraIndex >= 3)
            {
                return 0;
            }
            else
            {
                return cameraIndex + 1;
            }
        }
        #region DLL IMPORTS

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

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
        #endregion DLL IMPORTS
    }
}
