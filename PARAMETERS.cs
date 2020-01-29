using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public static class PARAMETERS
    {
        public static List<string> PARAM;
        static string param;
        public static bool isMinimized = false;
        public static bool isControlButtonVisible = true;
        /// <summary>
        /// currentl index, not MAIN
        /// </summary>
        public static int CameraIndex = -1;
        public static bool wakeUpCall;
        /// <summary>
        /// Important for UNIT TEST Only variable. Ignore
        /// </summary>
        public static string CurrentTestResult = "";
        /// <summary>
        /// Method name
        /// </summary>
        public static string MethodName = "";

        /// <summary>
        /// Switch parameter is present in a parameter call, where it is required
        /// </summary>
        public static bool SwitchParameterPresent = false;

        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            PARAM = parameters.ToList<string>();
           
            Logger.Add(param);

            param = String.Concat(parameters).ToLower();
            string elem;
            MethodName = "";
            bool parameterOnOffSwitch = false;
            SwitchParameterPresent = false;
            int cameraIndex = -1;
            int parameterTime = 0;
            /*
             Handle the initial start up CL parameters, if exist
            */
            if (param.Contains("uvccameraviewer"))
            {
                if (parameters.Count > 1)
                {
                    //WhichCase = param;
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        elem = parameters.ElementAt(i).ToLower();
                        try
                        {
                            switch (elem.Substring(0, 1))
                            {
                                case "m":
                                    MethodName = elem.Substring(2);
                                    break;
                                case "s":                                    
                                    try
                                    {                                        
                                        int sw = Int32.Parse(elem.Substring(2));
                                        parameterOnOffSwitch = (sw!=0);
                                        SwitchParameterPresent = true;
                                        CurrentTestResult = elem.Substring(2) + " switch value";
                                        //cameraIndex = -2;

                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = anx.Message;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = "S " + parameterOnOffSwitch + " " + fx.Message;
                                        i = parameters.Count;
                                    }
                                    catch (OverflowException ofx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = ofx.Message;
                                        i = parameters.Count;
                                    }
                                    break;
                                case "c":
                                    //in this case any non digit value for the c parameter will turn to 0                                    
                                    //cameraIndex = Int32.Parse(elem.Substring(2)) - 1;
                                    //fixing it:
                                    try
                                    {
                                        
                                        int ci = Int32.Parse(elem.Substring(2));
                                        cameraIndex = ci-1;
                                        CurrentTestResult = cameraIndex + " camera index";
                                        //cameraIndex = -2;                                        
                                        CameraIndex = cameraIndex;
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = anx.Message;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = "C " + fx.Message;
                                        i = parameters.Count;
                                    }
                                    catch(OverflowException ofx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = ofx.Message;
                                        i = parameters.Count;
                                    }
                                    break;
                                case "t":                                    
                                    try
                                    {
                                        parameterTime = Int32.Parse(elem.Substring(2));
                                        CurrentTestResult = parameterTime + " time parameter";
                                        //CameraIndex = -1;
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = anx.Message;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = "T " + fx.Message;
                                        i = parameters.Count;
                                    }
                                    catch (OverflowException ofx)
                                    {
                                        MethodName = "";
                                        CurrentTestResult = ofx.Message;
                                        i = parameters.Count;
                                    }
                                    break;
                                
                            }                            
                        }
                        catch (Exception e) 
                        {
                            CurrentTestResult = "Exception";
                            Debug.WriteLine(e.Message + " parameters in the command were sent with unexpected values");
                        }
                        //WhichCase = elem;
                    }                    

                    if(cameraIndex == -1)//カメラ番号が未入力の場合
                    {
                        //if(MainForm.GetMainForm == null)
                        //{
                        //    cameraIndex = 0;
                        //    Properties.Settings.Default.main_camera_index = 0;
                        //    Properties.Settings.Default.camera_count = 1;
                        //}
                        //else 
                        if(CheckMethodsWhereCameraOmittionAllowed(MethodName))
                        {
                            cameraIndex = Properties.Settings.Default.main_camera_index;
                            
                        }
                    }

                    // METHOD
                    switch (MethodName)
                    {
                        //No method
                        case "": 
                            break;

                        // Show Settings window
                        case "c":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && SwitchParameterPresent)
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        CurrentTestResult = "Showing Settings window";
                                        if (MainForm.Settingui != null && MainForm.Settingui.Visible == false)
                                        {
                                            //MainForm.GetMainForm.TopMost = false;
                                            //WhichCase = "Showing Settings window";
                                            MainForm.Settingui.ShowSettings(cameraIndex);
                                        }
                                    }
                                    else
                                    {
                                        CurrentTestResult = "Hiding Settings window " + cameraIndex;                                        
                                        MainForm.Settingui?.Hide();
                                    }
                                }
                                else
                                {
                                    CurrentTestResult = "missing switch parameter or wrong index";
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                CurrentTestResult = "FAILURE";
                                Trace.WriteLine(e.ToString() + " in method c");
                            }
                            break;

                        // Change main camera
                        case "n":
                            if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                            {
                                if (MULTI_WINDOW.formList[cameraIndex]?.DISPLAYED == true && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false && MULTI_WINDOW.formList[Properties.Settings.Default.main_camera_index].recordingInProgress == false)
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
                                    MULTI_WINDOW.FormSettingsChanged();
                                }
                                else
                                {
                                    Logger.Add(Resource.parameter_execution_failure + " m=" + MethodName + ", c=" + cameraIndex);
                                }
                                CurrentTestResult = "N case, camera index conditions passed";
                                PARAM.Clear();
                            }
                            else
                            {
                                CurrentTestResult = "N case, conditions faled " + cameraIndex + " " + CheckCameraIndex(cameraIndex) + " " + (cameraIndex >= 0 && cameraIndex < 4);
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
                                if (CheckCameraIndex(cameraIndex) && MULTI_WINDOW.formList[cameraIndex]?.recordingInProgress == false)
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
                                CurrentTestResult = "Human Sensor " + cameraIndex + " " + parameterTime;
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " in method h");
                            }
                            break;

                        // Visible - Active
                        case "v":
                                if (CheckCameraIndex(cameraIndex) && cameraIndex == 8 && SwitchParameterPresent)
                                {
                                    for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                                    {
                                        if (parameterOnOffSwitch)
                                        {
                                            CurrentTestResult = "Show all windows";
                                            isMinimized = false;
                                            try
                                            {
                                                MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                                                MULTI_WINDOW.formList[i].Show();
                                                MULTI_WINDOW.formList[i].Activate();
                                            }
                                            catch (ArgumentOutOfRangeException)
                                            {

                                            }                                            
                                        }
                                        else
                                        {
                                            CurrentTestResult = "Hiding all windows";
                                            isMinimized = true;
                                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
                                        }
                                    }
                                CurrentTestResult = "Show all windows";
                                }
                                else if (CheckCameraIndex(cameraIndex))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        CurrentTestResult = "Show 1 window";
                                        isMinimized = false;
                                        try
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                                            MULTI_WINDOW.formList[cameraIndex]?.Show();
                                            MULTI_WINDOW.formList[cameraIndex]?.Activate();
                                        }
                                        catch (NullReferenceException)
                                        {
                                            
                                        }
                                    }
                                    else
                                    {
                                        CurrentTestResult = "Hiding 1 window";
                                        isMinimized = true;
                                        try
                                        {
                                            MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                                        }
                                        catch (NullReferenceException)
                                        {

                                        }                                        
                                    }                                
                                }
                                
                                PARAMETERS.PARAM.Clear();  
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
                                        PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(cameraIndex, true);
                                    }
                                    else
                                    {
                                        PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(cameraIndex, false);
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
                                    if (parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                    {
                                        MULTI_WINDOW.EventRecorderOn(cameraIndex);
                                    }
                                    else if (!parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress)
                                    {
                                        MULTI_WINDOW.EventRecorderOff(cameraIndex);
                                        MULTI_WINDOW.formList[cameraIndex].HideIcon();
                                    }
                                }
                                
                                else if (CheckCameraIndex(cameraIndex))  // Not main camera                              
                                {
                                    if (parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.recordFromParamNotMain = true;
                                        MULTI_WINDOW.formList[cameraIndex].SetRecordIcon(cameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.Start(cameraIndex, CAMERA_MODES.MANUAL);
                                    }
                                    else if (!parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress)
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
                        default:
                            throw new Exception("Wrong Method Name");
                            break;
                    }
                }
                else
                {
                    CurrentTestResult = "parameters missing";
                }
            }
            else
            {
                CurrentTestResult = "App Name missing";
            }
        }

        static bool CheckMethodsWhereCameraOmittionAllowed(string m)
        {
            bool retval = false;
            string[] vs = new string[] {"h", "d", "v", "w", "s", "r", "e", "b", "q", "c", "l"};
            foreach(string i in vs)
            {
                if (m==i)
                {
                    retval = true;
                }
            }
            return retval;
        }
        internal static void HandleWakeUpParameters()
        {
            if (PARAM != null && PARAM.Count > 0 && !PARAM.Contains("uvccameraviewer"))
            {
                PARAM.Reverse();
                PARAM.Add("uvccameraviewer");
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

            if ((cameraIndex == 8 || (cameraIndex >= 0 && cameraIndex < 4)))
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
