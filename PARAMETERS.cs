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
        /// Methodname is present
        /// </summary>
        public static bool MethodNameIsPresent = false;
        public static bool SwitchIsPresent = false;
        public static bool CamIndexIsPresent = false;
        public static bool TimerIsPresent = false;
        public static bool WrongParameter = false;

        public static bool parameterOnOffSwitch = false;
        public static int parameterTime = 0;

        public static string ParameterSet { get; private set; }

        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {
            PARAM = parameters.ToList<string>();
            
            MethodNameIsPresent = false;
            SwitchIsPresent = false;
            CamIndexIsPresent = false;
            TimerIsPresent = false;
            WrongParameter = false;

            parameterOnOffSwitch = false;
            parameterTime = 0;

        Logger.Add(param);

            param = String.Concat(parameters).ToLower();
            string elem;
            MethodName = " ";
            
            SwitchIsPresent = false;
            int cameraIndex = -1;
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
                                    if (elem.Substring(2).Length>0)
                                    {
                                        MethodName = elem.Substring(2,1);
                                        MethodNameIsPresent = true;
                                    }
                                    else
                                    {
                                        MethodName = " ";
                                        MethodNameIsPresent = false;
                                    }                                    
                                    break;
                                case "s":                                    
                                    try
                                    {                                        
                                        int sw = Int32.Parse(elem.Substring(2));
                                        if (sw != 0)
                                        {
                                            SwitchIsPresent = true;
                                            parameterOnOffSwitch = true;
                                        }else
                                        {
                                            SwitchIsPresent = true;
                                            parameterOnOffSwitch = false;
                                        }
                                        CurrentTestResult = elem.Substring(2) + " switch value";
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        WrongParameter = true;
                                        CurrentTestResult = "S " + parameterOnOffSwitch + " " + fx.Message;
                                    }
                                    catch (OverflowException ofx)
                                    {
                                        WrongParameter = true;
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
                                        CamIndexIsPresent = true;
                                        CurrentTestResult = cameraIndex + " camera index";
                                        //cameraIndex = -2;                                        
                                        CameraIndex = cameraIndex;
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    catch(OverflowException ofx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    break;
                                case "t":                                    
                                    try
                                    {
                                        parameterTime = Int32.Parse(elem.Substring(2));
                                        TimerIsPresent = true;
                                        CurrentTestResult = parameterTime + " time parameter";
                                        //CameraIndex = -1;
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    catch (OverflowException ofx)
                                    {
                                        WrongParameter = true;
                                        i = parameters.Count;
                                    }
                                    break;
                                
                            }                            
                        }
                        catch (Exception e) 
                        {
                            WrongParameter = true;                            
                            Debug.WriteLine(e.Message + " parameters in the command were sent with unexpected values");
                        }
                        //WhichCase = elem;
                    } 

                    //if(cameraIndex == -1)//カメラ番号が未入力の場合
                    //{
                    //    //if(MainForm.GetMainForm == null)
                    //    //{
                    //    //    cameraIndex = 0;
                    //    //    Properties.Settings.Default.main_camera_index = 0;
                    //    //    Properties.Settings.Default.camera_count = 1;
                    //    //}
                    //    //else 
                    //    if(CheckMethodsWhereCameraOmittionAllowed(MethodName) || MethodName.Length==0 || MethodName == "w")
                    //    {
                    //        cameraIndex = Properties.Settings.Default.main_camera_index;
                    //        CameraIndex = cameraIndex;
                    //    }
                    //}

                    /*
                     * There are 6 variations:
                     * ================
                     * M+S+C+T      All TTTT
                     * M+S+C            TTTF
                     * M+S              TTFF
                     * M+C              TFTF
                     * M                TFFF
                     * C                FFTF
                     * ================
                     * Impossible/Disallowed cases
                     * M+S+T            TTFT    Time must be coupled with Camera Index
                     * S                FTFF
                     */

                    GenerateTheParameterSet();

                    switch (ParameterSet)
                    {
                        case "TTTT":
                            AllParametersCase();
                            break;
                        case "TTTF":
                            TimelessCase();
                            break;
                        case "TTFF":
                            MethodSwitchCase();
                            break;
                        case "TFTF":
                            CheckMateCase();
                            break;
                        case "TFFF":
                            BossIsAlwaysRightCase();
                            break;
                        case "FFTF":
                            DontTouchMyCamsCase();
                            break;
                        case "TTFT":
                            WrongParameter = true;
                            break;
                        case "FTFF":
                            WrongParameter = true;
                            break;
                    }




                    // METHOD
                    switch (MethodName)
                    {
                        //No method
                        case "":                            
                            if (wakeUpCall && CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                            {
                                //START the camera

                            }
                            else if(wakeUpCall && (cameraIndex == 8))
                            {

                            }
                            CurrentTestResult = "No method " + cameraIndex;
                            break;

                        // Show Settings window
                        case "c":
                            try
                            {
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && SwitchIsPresent)
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
                                    CurrentTestResult = "C missing switch parameter or wrong index";
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
                                if (wakeUpCall)
                                {
                                    if (parameterTime > 0 && CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                                    {
                                        //when there is parameterTime, cameraindex is a must
                                        CurrentTestResult = "HUMAN SENSOR parameterTime index 1-4" + parameterTime + " " + cameraIndex;
                                    }
                                    else if (parameterTime > 0 && CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                                    {
                                        CurrentTestResult = "HUMAN SENSOR parameterTime index 9 " + parameterTime + " " + cameraIndex;
                                    }
                                    else if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex]?.recordingInProgress == false)
                                    {
                                        if (parameterOnOffSwitch)
                                        {
                                            PROPERTY_FUNCTIONS.Set_Human_Sensor(cameraIndex, true);
                                            PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(cameraIndex, true);
                                            MainForm.AllChangesApply();
                                            PROPERTY_FUNCTIONS.SetCycleTime(cameraIndex, parameterTime);
                                        }
                                        CurrentTestResult = "HUMAN SENSOR no time index 1-4" + parameterTime + " " + cameraIndex;
                                    }
                                }
                                else//the app is running
                                {
                                    if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && MULTI_WINDOW.formList[cameraIndex]?.recordingInProgress == false)
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

                                
                                //CurrentTestResult = "Human Sensor " + cameraIndex + " " + parameterTime + " " + wakeUpCall;
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Logger.Add(e.Message + " in method h");
                            }
                            break;

                        // Visible - Active
                        case "v":
                                if (CheckCameraIndex(cameraIndex) && cameraIndex == 8 && SwitchIsPresent)
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
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                                {
                                    if (parameterOnOffSwitch)
                                    {
                                        PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(cameraIndex, true);
                                        Properties.Settings.Default.Save();
                                        if (MainForm.GetMainForm != null)
                                            MainForm.AllChangesApply();
                                    }
                                    else
                                    {
                                        //There is no requirement to display with panels off
                                        //PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(cameraIndex, false);
                                    }                                    
                                }else if (CheckCameraIndex(cameraIndex) && (cameraIndex == 8))
                                {
                                    //Support all

                                }
                                PARAMETERS.PARAM.Clear();
                                CurrentTestResult = "W " + parameterOnOffSwitch + " " + cameraIndex;
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
                                if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && (cameraIndex == Properties.Settings.Default.main_camera_index)) // Main camera
                                {
                                    if (parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                    {
                                        MULTI_WINDOW.EventRecorderOn(cameraIndex);
                                    }
                                    else if (!parameterOnOffSwitch)
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].HideIcon();
                                        MULTI_WINDOW.EventRecorderOff(cameraIndex);
                                    }
                                }
                                else if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))  // Not main camera                              
                                {
                                    if (parameterOnOffSwitch && MULTI_WINDOW.formList[cameraIndex].recordingInProgress == false)
                                    {
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.recordFromParamNotMain = true;
                                        MULTI_WINDOW.formList[cameraIndex].SetRecordIcon(cameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                        MULTI_WINDOW.formList[cameraIndex].crossbar.Start(cameraIndex, CAMERA_MODES.MANUAL);
                                    }
                                    else if (!parameterOnOffSwitch)
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
                                }
                                else if (CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4) && !parameterOnOffSwitch)
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
                                PARAMETERS.PARAM.Clear();
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                Debug.WriteLine(e.ToString() + " in method r");
                            }
                            break;
                        default:
                            //throw new Exception("Wrong Method Name");                            
                            CurrentTestResult = "Wrong Method Name";
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

        private static void DontTouchMyCamsCase()
        {
            throw new NotImplementedException();
        }

        private static void BossIsAlwaysRightCase()
        {
            throw new NotImplementedException();
        }

        private static void CheckMateCase()
        {
            throw new NotImplementedException();
        }

        private static void MethodSwitchCase()
        {
            throw new NotImplementedException();
        }

        private static void TimelessCase()
        {
            throw new NotImplementedException();
        }

        private static void AllParametersCase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a string of initials for the flags
        /// </summary>
        private static void GenerateTheParameterSet()
        {
            ParameterSet = 
                MethodNameIsPresent.ToString().Substring(0, 1)+
                SwitchIsPresent.ToString().Substring(0, 1)+
                CamIndexIsPresent.ToString().Substring(0, 1)+
                TimerIsPresent.ToString().Substring(0, 1);
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
                switch (m)
                {
                    case "h":
                        if (parameterTime > 0)
                        {
                            retval = true;
                        }
                        break;
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
