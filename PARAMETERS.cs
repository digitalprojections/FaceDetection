using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    class PARAMETERS
    {
        public static List<string> PARAM;
        static string param;
        public static bool isMinimized = false;
        public static bool isControlButtonVisible = true;
        public static int CameraIndex = 0;

        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            PARAM = parameters.ToList<string>();
           
            Logger.Add(param);

            param = String.Concat(parameters).ToLower();
            string elem;
            string method = "";
            bool switchOnOff = false;
            int cameraIndex = -1;
            int time = 0;
            /*
             Handle the initial start up CL parameters, if exist
            */
            if (param.Contains("uvccameraviewer"))
            {
                if (parameters.Count > 0)
                {
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        elem = parameters.ElementAt(i).ToLower();
                        try
                        {
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
                                    if (cameraIndex == 8)
                                    {
                                        cameraIndex = Properties.Settings.Default.main_camera_index;
                                    }
                                    CameraIndex = cameraIndex;
                                    break;
                                case "t":
                                    time = Int32.Parse(elem.Substring(2));
                                    break;
                            }
                        }
                        catch (Exception e) 
                        {
                            Debug.WriteLine(e.Message + " parameters in the command were sent with unexpected values");
                        }
                    }
                    if(cameraIndex == -1)//カメラ番号が未入力の場合
                    {
                        if(MainForm.GetMainForm == null)
                        {
                            cameraIndex = 0;
                        }
                        //else if(method == "n")
                        //{
                        //    cameraIndex = GetNextCameraIndex(MainForm.SELECTED_CAMERA);
                        //}
                        else
                        {
                            cameraIndex = Properties.Settings.Default.main_camera_index;
                        }
                    }

                    // METHOD
                    switch (method)
                    {
                        case "":
                            if (CheckCameraIndex(cameraIndex))
                            {
                                if(cameraIndex == 8)
                                {
                                    //Start all cameras
                                }
                                else
                                {

                                }
                            }
                            break;

                        case "c":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                        {
                                            MainForm.GetMainForm.TopMost = false;
                                            MainForm.Setting_ui.ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        MainForm.Setting_ui.Hide();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Trace.WriteLine(e.ToString() + " in method c");
                            }
                            break;

                        case "n":
                            if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                            {
                                cameraIndex = GetNextCameraIndex(cameraIndex);

                                //if (cameraIndex == 0)
                                //{
                                //    MainForm.GetMainForm.WindowState = FormWindowState.Normal;
                                //    MainForm.GetMainForm.Width = Decimal.ToInt32(Properties.Settings.Default.C1w);
                                //    MainForm.GetMainForm.Height = Decimal.ToInt32(Properties.Settings.Default.C1h);
                                //    MainForm.GetMainForm.Activate();
                                //    if(!Properties.Settings.Default.show_all_cams_simulteneously && MULTI_WINDOW.DisplayedCameraCount >= 1)
                                //    {
                                //        MULTI_WINDOW.formList[MULTI_WINDOW.DisplayedCameraCount].WindowState = FormWindowState.Minimized;
                                //    }
                                //}
                                //else
                                //{
                                    MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                    MULTI_WINDOW.formList[cameraIndex].Activate();
                                    if (!Properties.Settings.Default.show_all_cams_simulteneously)
                                    {
                                        
                                        MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                        
                                    }
                                //}
                                PARAMETERS.PARAM.Clear();
                            }
                            break;

                        case "s":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    SNAPSHOT_SAVER.TakeAsyncSnapShot();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.Message + " in method s");
                            }
                            break;

                        case "b":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        //SHOW CONTROL BUTTONS
                                        isControlButtonVisible = true;
                                        MULTI_WINDOW.formList[cameraIndex].ParametersChangesApply(cameraIndex);

                                    }
                                    else
                                    {
                                        //HIDE CONTROL BUTTONS
                                        isControlButtonVisible = false;
                                        
                                        MULTI_WINDOW.formList[cameraIndex].ParametersChangesApply(cameraIndex);
                                        
                                    }                                                                       
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method b");
                            }
                            break;

                        case "d":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        if (MULTI_WINDOW.formList[cameraIndex].FaceDetector != null)
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].FaceDetector.StartFaceTimer();
                                        }

                                        if (cameraIndex == 0)
                                        {
                                            Properties.Settings.Default.C1_enable_face_recognition = true;
                                            Properties.Settings.Default.C1_enable_Human_sensor = false;
                                            Properties.Settings.Default.C1_enable_capture_operator = true;
                                        }
                                        else if (cameraIndex == 1)
                                        {
                                            Properties.Settings.Default.C2_enable_face_recognition = true;
                                            Properties.Settings.Default.C2_enable_Human_sensor = false;
                                            Properties.Settings.Default.C2_enable_capture_operator = true;
                                        }
                                        else if (cameraIndex == 2)
                                        {
                                            Properties.Settings.Default.C3_enable_face_recognition = true;
                                            Properties.Settings.Default.C3_enable_Human_sensor = false;
                                            Properties.Settings.Default.C3_enable_capture_operator = true;
                                        }
                                        else if (cameraIndex == 3)
                                        {
                                            Properties.Settings.Default.C4_enable_face_recognition = true;
                                            Properties.Settings.Default.C4_enable_Human_sensor = false;
                                            Properties.Settings.Default.C4_enable_capture_operator = true;
                                        }
                                    }
                                    else
                                    {
                                        if (cameraIndex >= 0 && cameraIndex <4)
                                        {
                                            if (MULTI_WINDOW.formList[cameraIndex].FaceDetector != null && MULTI_WINDOW.formList[cameraIndex].crossbar.Recording_is_on)
                                            {
                                                MULTI_WINDOW.formList[cameraIndex].crossbar.PreviewMode();
                                                MULTI_WINDOW.formList[cameraIndex].FaceDetector.StopFaceTimer();
                                            }
                                            Properties.Settings.Default.C4_enable_face_recognition = false;
                                        }
                                    }

                                    CycleTime(cameraIndex, time);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method d");
                            }
                            break;

                        case "h":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        if (cameraIndex == 0)
                                        {
                                            Properties.Settings.Default.C1_enable_capture_operator = true;
                                            Properties.Settings.Default.C1_enable_Human_sensor = true;
                                            Properties.Settings.Default.C1_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 1)
                                        {
                                            Properties.Settings.Default.C2_enable_capture_operator = true;
                                            Properties.Settings.Default.C2_enable_Human_sensor = true;
                                            Properties.Settings.Default.C2_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 2)
                                        {
                                            Properties.Settings.Default.C3_enable_capture_operator = true;
                                            Properties.Settings.Default.C3_enable_Human_sensor = true;
                                            Properties.Settings.Default.C3_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 3)
                                        {
                                            Properties.Settings.Default.C4_enable_capture_operator = true;
                                            Properties.Settings.Default.C4_enable_Human_sensor = true;
                                            Properties.Settings.Default.C4_enable_face_recognition = false;
                                        }
                                    }
                                    else
                                    {
                                        if (MainForm.RSensor != null)
                                        {
                                            MainForm.RSensor.Stop_IR_Timer();
                                        }

                                        if (cameraIndex == 0)
                                        {
                                            Properties.Settings.Default.C1_enable_Human_sensor = false;
                                        } 
                                        //IS THIS PART EVENT VALID??? THERE IS ONLY 1 HUMAN SENSOR
                                        else if (cameraIndex == 1)
                                        {
                                            Properties.Settings.Default.C2_enable_Human_sensor = false;
                                        }
                                        else if (cameraIndex == 2)
                                        {
                                            Properties.Settings.Default.C3_enable_Human_sensor = false;
                                        }
                                        else if (cameraIndex == 3)
                                        {
                                            Properties.Settings.Default.C4_enable_Human_sensor = false;
                                        }
                                    }
                                    MainForm.AllChangesApply();
                                    CycleTime(cameraIndex, time);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " in method h");
                            }
                            break;

                        case "v":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        isMinimized = false;
                                        if (cameraIndex == 0)
                                        {
                                            MainForm.GetMainForm.WindowState = FormWindowState.Normal;
                                            MainForm.GetMainForm.Show();
                                            MainForm.GetMainForm.Activate();
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                            MULTI_WINDOW.formList[cameraIndex].Show();
                                            MULTI_WINDOW.formList[cameraIndex].Activate();
                                        }
                                    }
                                    else
                                    {
                                        isMinimized = true;
                                        if (cameraIndex == 0)
                                        {
                                            MainForm.GetMainForm.WindowState = FormWindowState.Minimized;
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                        }
                                    }
                                }
                                PARAMETERS.PARAM.Clear();
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method v");
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
                                Debug.WriteLine(e.ToString() + " in method l");
                            }
                            break;

                        case "w":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff)
                                    {
                                        MainForm.GetMainForm.FormBorderStyle = FormBorderStyle.Sizable;
                                        Properties.Settings.Default.show_window_pane = true;
                                        Properties.Settings.Default.Save();

                                        if (MainForm.Setting_ui != null && MainForm.Setting_ui.Visible == false)
                                        {
                                            MainForm.GetMainForm.TopMost = false;
                                        }
                                    }
                                    else
                                    {
                                        Properties.Settings.Default.show_window_pane = false;
                                        Properties.Settings.Default.Save();
                                    }
                                    MainForm.AllChangesApply();
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method w");
                            }
                            break;

                        case "q":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    //if (cameraIndex == 0)
                                    //{
                                    //    MULTI_WINDOW.formList[0].Close();
                                    //}
                                    //else if (cameraIndex == 1)
                                    //{
                                    //    MULTI_WINDOW.formList[1].Close();
                                    //}
                                    //else if (cameraIndex == 2)
                                    //{
                                    //    
                                    //}
                                    //else
                                    //{
                                    MULTI_WINDOW.formList[cameraIndex].Close();
                                    //Application.Exit();
                                    //}
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method q");
                            }
                            break;

                        case "e":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Setting_ui.Camera_index))
                                {
                                    if (switchOnOff && MainForm.GetMainForm.AnyRecordingInProgress == false)
                                    {
                                        MainForm.GetMainForm.EventRecorderOn(cameraIndex);
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.EventRecorderOff(cameraIndex);
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method e");
                            }
                            break;

                        case "r":
                            try
                            {
                                if (cameraIndex == MainForm.Setting_ui.Camera_index)
                                {
                                    if (switchOnOff)
                                    {
                                        
                                        MULTI_WINDOW.formList[cameraIndex].SetRecordIcon(cameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.Start(cameraIndex, CAMERA_MODES.MANUAL);
                                        
                                    }
                                    else
                                    {
                                        
                                            MULTI_WINDOW.formList[cameraIndex].HideIcon();
                                            if (PROPERTY_FUNCTIONS.CheckPreEventTimes(cameraIndex))
                                            {
                                                if (MULTI_WINDOW.displayedCameraCount > 0)
                                                {
                                                MULTI_WINDOW.formList[cameraIndex].SetToPreeventMode();
                                                }
                                            }
                                            else
                                            {
                                                if (MULTI_WINDOW.displayedCameraCount > 0)
                                                {
                                                MULTI_WINDOW.formList[cameraIndex].SetToPreviewMode();
                                                }
                                            }
                                        
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method r");
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
            bool retval = false;

            if (cameraIndex ==8 || cameraIndex >=0 && cameraIndex<4)
            {                
                retval = true;                
            }
            return retval;
        }

        private static void CycleTime(int cameraIndex, int time)
        {
            if (time >= 500 && time <= 1000)
            {
                if (cameraIndex == 0)
                {
                Properties.Settings.Default.C1_check_interval = time;
            }
                else if (cameraIndex == 1)
                {
                    Properties.Settings.Default.C2_check_interval = time;
                }
                else if (cameraIndex == 2)
                {
                    Properties.Settings.Default.C3_check_interval = time;
                }
                else if (cameraIndex == 3)
                {
                    Properties.Settings.Default.C4_check_interval = time;
                }
            }
        }

        private static int GetNextCameraIndex(int cameraIndex)
        {
            //if(cameraIndex == 8)
            //{
            //    //すべてのカメラ指定時
            //    //1から始まって9なので、0から始まると8になる
            //    return cameraIndex;
            //}
            //else if(cameraIndex >= 3)
            if (cameraIndex >= MULTI_WINDOW.displayedCameraCount)
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
