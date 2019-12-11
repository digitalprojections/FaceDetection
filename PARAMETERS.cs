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
                        else if(method == "n")
                        {
                            cameraIndex = GetNextCameraIndex(MainForm.SELECTED_CAMERA);
                        }
                        else
                        {
                            cameraIndex = MainForm.SELECTED_CAMERA;
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
                                        if (cameraIndex == 0)
                                        {
                                            MainForm.ParametersChangesApply();
                                        }
                                        else
                                        {
                                            FormClass.ParametersChangesApply(cameraIndex-1);
                                        }
                                    }
                                    else
                                    {
                                        //HIDE CONTROL BUTTONS
                                        isControlButtonVisible = false;
                                        if (cameraIndex == 0)
                                        {
                                            MainForm.ParametersChangesApply();
                                        }
                                        else
                                        {
                                            FormClass.ParametersChangesApply(cameraIndex-1);
                                        }
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
                                        if (MainForm.FaceDetector != null)
                                        {
                                            MainForm.FaceDetector.StartFaceTimer();
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
                                        if (cameraIndex == 0)
                                        {
                                            if (MainForm.FaceDetector != null && MainForm.GetMainForm.crossbar.Recording_is_on)
                                            {
                                                MainForm.GetMainForm.crossbar.PreviewMode();
                                                MainForm.FaceDetector.StopFaceTimer();
                                            }
                                            Properties.Settings.Default.C1_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 1)
                                        {
                                            if (MainForm.FaceDetector != null && FormClass.crossbarList[0].Recording_is_on)
                                            {
                                                MainForm.GetMainForm.crossbar.PreviewMode();
                                                MainForm.FaceDetector.StopFaceTimer();
                                            }
                                            Properties.Settings.Default.C2_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 2)
                                        {
                                            if (MainForm.FaceDetector != null && FormClass.crossbarList[1].Recording_is_on)
                                            {
                                                MainForm.GetMainForm.crossbar.PreviewMode();
                                                MainForm.FaceDetector.StopFaceTimer();
                                            }
                                            Properties.Settings.Default.C3_enable_face_recognition = false;
                                        }
                                        else if (cameraIndex == 3)
                                        {
                                            if (MainForm.FaceDetector != null && FormClass.crossbarList[2].Recording_is_on)
                                            {
                                                MainForm.GetMainForm.crossbar.PreviewMode();
                                                MainForm.FaceDetector.StopFaceTimer();
                                            }
                                            Properties.Settings.Default.C4_enable_face_recognition = false;
                                        }
                                    }

                                    CycleTime(time);
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
                                    CycleTime(time);
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
                                            MULTI_WINDOW.formList[cameraIndex - 1].WindowState = FormWindowState.Normal;
                                            MULTI_WINDOW.formList[cameraIndex - 1].Show();
                                            MULTI_WINDOW.formList[cameraIndex - 1].Activate();
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
                                            MULTI_WINDOW.formList[cameraIndex - 1].WindowState = FormWindowState.Minimized;
                                        }
                                    }
                                }
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
                                            //Logger.Add("Properties.Settings.Default.show_window_pane " + Properties.Settings.Default.show_window_pane);
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
                                if (cameraIndex == 1)
                                {
                                    MULTI_WINDOW.formList[0].Close();
                                }
                                else if (cameraIndex == 2)
                                {
                                    MULTI_WINDOW.formList[1].Close();
                                }
                                else if (cameraIndex == 3)
                                {
                                    MULTI_WINDOW.formList[2].Close();
                                }
                                else
                                {
                                    Application.Exit();
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
                                    if (switchOnOff && MainForm.GetMainForm.recordingInProgress == false)
                                    {
                                        MainForm.GetMainForm.EventRecorderOn(cameraIndex);
                                    }
                                    else
                                    {
                                        MainForm.GetMainForm.EventRecorderOff(cameraIndex);
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
                                        MainForm.Or_pb_recording.Image = Properties.Resources.player_record;
                                        MainForm.GetMainForm.crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                        MainForm.GetMainForm.crossbar.Start(0, CAMERA_MODES.MANUAL);
                                        if (Properties.Settings.Default.capture_method == 0)
                                        {
                                            MainForm.GetMainForm.SET_REC_ICON();
                                        }
                                    }
                                    else
                                    {
                                        MainForm.Or_pb_recording.Visible = false;
                                        MainForm.GetMainForm.recordingInProgress = false;
                                        MainForm.SetCameraToDefaultMode(cameraIndex);
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
            bool Retval = false;

            switch (cameraIndex)
            {
                case 0:
                    Retval = true;
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

        private static void CycleTime(int time)
        {
            if (time >= 500 && time <= 1000)
            {
                Properties.Settings.Default.C1_check_interval = time;
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
