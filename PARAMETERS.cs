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
        private static bool snapshotRequested;
        public static bool wakeUpCall;

        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            PARAM = parameters.ToList<string>();
           
            Logger.Add(param);

            param = String.Concat(parameters).ToLower();
            string elem;
            string method = "";
            bool parameterOnOffSwitch = false;
            int cameraIndex = -1;
            int parameterTime = 0;
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
                                    parameterOnOffSwitch = (elem.Substring(2) != "0");                                    
                                    break;
                                case "c":
                                    cameraIndex = Int32.Parse(elem.Substring(2)) - 1;
                                    CameraIndex = cameraIndex;
                                    break;
                                case "t":
                                    parameterTime = Int32.Parse(elem.Substring(2));
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
                        case ""://No method?
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

                        case "c"://Show Settings window
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        if (MainForm.Settingui != null && MainForm.Settingui.Visible == false)
                                        {
                                            //MainForm.GetMainForm.TopMost = false;
                                            MainForm.Settingui.ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        MainForm.Settingui.Hide();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Trace.WriteLine(e.ToString() + " in method c");
                            }
                            break;

                        case "n":
                            if (CheckCameraIndex(cameraIndex) && (cameraIndex == Properties.Settings.Default.main_camera_index))
                            {
                                //cameraIndex = GetNextCameraIndex(cameraIndex);
                                MULTI_WINDOW.formList[cameraIndex].Text = $"UVC Camera Viewer - MAIN CAMERA {(cameraIndex + 1)}";
                                Properties.Settings.Default.main_camera_index = cameraIndex;

                                MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                MULTI_WINDOW.formList[cameraIndex].Activate();
                                if (!Properties.Settings.Default.show_all_cams_simulteneously)
                                {
                                    MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                }
                                PARAMETERS.PARAM.Clear();                                
                            }
                            else if(CheckCameraIndex(cameraIndex) && cameraIndex!=8)
                            {
                                MULTI_WINDOW.formList[cameraIndex].Text = "UVC Camera Viewer -  camera " + (cameraIndex + 1);
                            }
                            break;

                        case "s"://SNAPSHOT
                            try
                            {
                                if (wakeUpCall)
                                {
                                    snapshotRequested = true;
                                    wakeUpCall = false;
                                    if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                    {
                                        SNAPSHOT_SAVER.TakeAsyncSnapShot(true, cameraIndex, "event");
                                    }
                                    else if (CheckCameraIndex(cameraIndex) && cameraIndex < 4)
                                    {
                                        SNAPSHOT_SAVER.TakeAsyncSnapShot(false, cameraIndex, "event");
                                    }
                                }
                                else
                                {
                                    if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                    {
                                        SNAPSHOT_SAVER.TakeSnapShotAll();

                                    }
                                    else if (CheckCameraIndex(cameraIndex) && cameraIndex < 4)
                                    {
                                        SNAPSHOT_SAVER.TakeSnapShot(cameraIndex, "event");
                                        //SNAPSHOT_SAVER.TakeAsyncSnapShot();

                                    }
                                }

                                
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.Message + " in method s");
                            }
                            break;

                        case "b"://SHOW / HIDE CONTROL BUTTONS
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        isControlButtonVisible = true;
                                        MULTI_WINDOW.formList[cameraIndex].SettingChangesApply(cameraIndex);
                                    }
                                    else
                                    {
                                        isControlButtonVisible = false;
                                        MULTI_WINDOW.formList[cameraIndex].SettingChangesApply(cameraIndex);
                                    }                                                                       
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method b");
                            }
                            break;

                        case "d"://Enable/Disable Face detection
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {                                        
                                        PROPERTY_FUNCTIONS.Set_Face_Switch(cameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(cameraIndex, true);
                                        if (MULTI_WINDOW.formList[cameraIndex].FaceDetector != null)
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].FaceDetector.StartFaceTimer();
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

                                    PROPERTY_FUNCTIONS.SetCycleTime(cameraIndex, parameterTime);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method d");
                            }
                            break;

                        case "h"://IR Sensor
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        PROPERTY_FUNCTIONS.Set_Human_Sensor(cameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(cameraIndex, true);
                                    }
                                    else
                                    {
                                        if (MainForm.RSensor != null)
                                        {
                                            MainForm.RSensor.Stop_IR_Timer();
                                        }

                                        PROPERTY_FUNCTIONS.Set_Human_Sensor(cameraIndex, false);
                                    }
                                    MainForm.AllChangesApply();
                                    PROPERTY_FUNCTIONS.SetCycleTime(cameraIndex, parameterTime);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " in method h");
                            }
                            break;

                        case "v"://Visible - Active
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        isMinimized = false;
                                        MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                        MULTI_WINDOW.formList[cameraIndex].Show();
                                        MULTI_WINDOW.formList[cameraIndex].Activate();
                                    }
                                    else
                                    {
                                        isMinimized = true;
                                        MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                    }
                                }
                                else if (CheckCameraIndex(cameraIndex) && cameraIndex==8)
                                {
                                    for (int i=0;i<MULTI_WINDOW.displayedCameraCount;i++)
                                    {
                                        if (parameterOnOffSwitch)
                                        {
                                            isMinimized = false;
                                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                                            MULTI_WINDOW.formList[i].Show();
                                            MULTI_WINDOW.formList[i].Activate();
                                        }
                                        else
                                        {
                                            isMinimized = true;
                                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
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

                        case "l"://Backlight
                            try
                            {
                                if (parameterOnOffSwitch)
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

                        case "w"://Window pane
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == MainForm.Settingui.Camera_index))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        Properties.Settings.Default.show_window_pane = true;
                                    }
                                    else
                                    {
                                        Properties.Settings.Default.show_window_pane = false;                                        
                                    }
                                    Properties.Settings.Default.Save();
                                    MainForm.AllChangesApply();
                                }
                                PARAMETERS.PARAM.Clear();
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method w");
                            }
                            break;

                        case "q"://Close a window or all (exit app)
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
                                {
                                    if (cameraIndex >= 0 && cameraIndex<4)
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].Close();
                                    }
                                    else if (cameraIndex == 8)
                                    {                                        
                                        Application.Exit();
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method q");
                            }
                            break;

                        case "e"://Event recorder
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex<4))
                                {
                                    if (parameterOnOffSwitch && MainForm.AnyRecordingInProgress == false)
                                    {
                                        MULTI_WINDOW.EventRecorderOn(cameraIndex);
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.EventRecorderOff(cameraIndex);
                                    }
                                }
                                else if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)                                
                                {
                                    //All camera event recorder needed?
                                    if (parameterOnOffSwitch)
                                    {
                                        MULTI_WINDOW.EventRecorderOnOFFAll(true);
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.EventRecorderOnOFFAll(false);
                                    }
                                }

                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method e");
                            }
                            break;

                        case "r"://Manual recording
                            try
                            {
                                if (cameraIndex == MainForm.Settingui.Camera_index)
                                {
                                    if (parameterOnOffSwitch)
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

        internal static void HandleWakeUpParameters()
        {
            if (PARAM != null && PARAM.Count > 0 && !PARAM.Contains("uvccameraviewer.exe"))
            {
                PARAM.Reverse();
                PARAM.Add("uvccameraviewer.exe");
                PARAM.Reverse();
                wakeUpCall = true;
                HandleParameters(PARAM);

                if (isMinimized)
                {
                    if (CameraIndex >= 0 && CameraIndex < 4)
                        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
                }
                else
                {
                    if (CameraIndex >= 0 && CameraIndex < 4)
                        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Normal;
                }

                PARAM.Clear();
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
