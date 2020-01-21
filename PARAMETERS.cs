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
                            Properties.Settings.Default.main_camera_index = 0;
                            Properties.Settings.Default.camera_count = 1;
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
                        //No method
                        case "": 
                            break;

                        // Show Settings window
                        case "c":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
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

                        // Change main camera
                        case "n":
                            if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                            {
                                if (MULTI_WINDOW.formList[cameraIndex]?.DISPLAYED == true && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                {
                                    //MULTI_WINDOW.formList[Properties.Settings.Default.main_camera_index].Text = "UVC Camera Viewer -  camera " + (Properties.Settings.Default.main_camera_index + 1);
                                    //MULTI_WINDOW.formList[cameraIndex].Text = $"UVC Camera Viewer - MAIN CAMERA {(cameraIndex + 1)}";
                                    //Properties.Settings.Default.main_camera_index = cameraIndex;

                                    //MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                    //MULTI_WINDOW.formList[cameraIndex].Activate();
                                    //if (!Properties.Settings.Default.show_all_cams_simulteneously)
                                    //{
                                    //    MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                    //}
                                    Properties.Settings.Default.main_camera_index = cameraIndex;
                                    Properties.Settings.Default.Save();
                                    MULTI_WINDOW.formSettingsChanged();
                                }
                                PARAMETERS.PARAM.Clear();
                            }
                            break;

                        // SNAPSHOT
                        case "s":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                                {
                                    if (wakeUpCall)
                                    {
                                        wakeUpCall = false;
                                        //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                        //{
                                        //    SNAPSHOT_SAVER.TakeAsyncSnapShot(true, cameraIndex, "event");
                                        //}
                                        //else 

                                        SNAPSHOT_SAVER.TakeAsyncSnapShot(false, cameraIndex, "event");

                                    }
                                }
                                else
                                {
                                    //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                    //{
                                    //    SNAPSHOT_SAVER.TakeSnapShotAll();
                                    //}
                                    //else 
                                    if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                    {
                                        //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                        //{
                                        //    SNAPSHOT_SAVER.TakeSnapShotAll();
                                        //}
                                        //else 

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

                        // SHOW / HIDE CONTROL BUTTONS
                        case "b":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
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

                        // Enable/Disable Face detection
                        //case "d":
                        //    try
                        //    {
                                
                        //    }
                        //    catch (ArgumentOutOfRangeException e)
                        //    {
                        //        Debug.WriteLine(e.ToString() + " in method d");
                        //    }
                        //    break;

                        // IR Sensor
                        case "h":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
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

                        // Visible - Active
                        case "v":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
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

                        // Backlight
                        case "l":
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

                        // Window pane
                        case "w":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex))
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

                        // Close a window or all (exit app)
                        case "q":
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

                        // Event recorder
                        case "e":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex == Properties.Settings.Default.main_camera_index) && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false) // Main camera
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        MULTI_WINDOW.EventRecorderOn(cameraIndex);
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.EventRecorderOff(cameraIndex);
                                    }
                                }
                                else if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)  // Not main camera                              
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.recordFromParamNotMain = true;
                                        MULTI_WINDOW.formList[cameraIndex].SetRecordIcon(cameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.Start(cameraIndex, CAMERA_MODES.MANUAL);
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].HideIcon();
                                        MULTI_WINDOW.formList[cameraIndex].SetToPreviewMode();
                                    }
                                }
                                PARAMETERS.PARAM.Clear();
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method e");
                            }
                            break;

                        // Manual recording
                        case "r":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
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
                                PARAMETERS.PARAM.Clear();
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
                    {
                        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
                    }
                }
                else
                {
                    if (CameraIndex >= 0 && CameraIndex < 4)
                    {
                        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Normal;
                    }
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

            if ((cameraIndex == 8 || (cameraIndex >= 0 && cameraIndex < 4)) && MULTI_WINDOW.formList[cameraIndex]?.DISPLAYED == true)
            {                
                retval = true;                
            }
            return retval;
        }

        //private static int GetNextCameraIndex(int cameraIndex)
        //{
        //    //if(cameraIndex == 8)
        //    //{
        //    //    //すべてのカメラ指定時
        //    //    //1から始まって9なので、0から始まると8になる
        //    //    return cameraIndex;
        //    //}
        //    //else if(cameraIndex >= 3)
        //    if (cameraIndex >= MULTI_WINDOW.displayedCameraCount)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return cameraIndex + 1;
        //    }
        //}
        
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
