﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public static class PARAMETERS
    {
        public static List<string> PARAM { get; private set; }

        public static int MainCamera { get; private set; }

        static string param;

        public static bool[] minimizedByParameter = new bool[4];
        public static bool callByParameters;
        public static bool minimizedByParameters;

        //public static bool isMinimized { get; private set; }
        public static bool isControlButtonVisible { get; private set; }
        /// <summary>
        /// currentl index, not MAIN
        /// </summary>
        public static int CameraIndex = -1;
        public static bool WAKEUPCALL { get; set; }
        /// <summary>
        /// Important for UNIT TEST Only variable. Ignore
        /// </summary>
        public static string CurrentTestResult { get; private set; }
        /// <summary>
        /// Method name
        /// </summary>
        private static string MethodName = "";

        public static int ConnectedCameraCount { get; private set; }
        public static decimal AllowedCameraCount { get; private set; }

        /// <summary>
        /// Methodname is present
        /// </summary>
        public static bool MethodNameIsPresent { get; private set; }
        public static bool SwitchIsPresent { get; private set; }
        public static bool CamIndexIsPresent { get; private set; }
        public static bool TimerIsPresent { get; private set; }
        public static bool WrongParameter { get; private set; }
        //public static bool WRONGPARAMETER { get; private set; }

        private static bool parameterOnOffSwitch = false;
        private static int parameterTime = -1;

        public static string ParameterSet { get; private set; }

        static CultureInfo culture = CultureInfo.CurrentCulture;

        public static void HandleParameters(IReadOnlyCollection<string> parameters)
        {

            PARAM = CleanUpTheParams(parameters.ToList<string>());

            CurrentTestResult = "";
            MainCamera = Properties.Settings.Default.main_camera_index;

            MethodNameIsPresent = false;
            SwitchIsPresent = false;
            CamIndexIsPresent = false;
            TimerIsPresent = false;
            WrongParameter = false;

            parameterOnOffSwitch = false;
            parameterTime = 0;
            CameraIndex = -1;

            Logger.Add(param);

            param = String.Concat(parameters).ToLower(culture);
            string elem;
            MethodName = " ";

            ConnectedCameraCount = Camera.GetCameraCount().Length;
            AllowedCameraCount = Properties.Settings.Default.camera_count;

            /*
             Handle the initial start up CL parameters, if exist
            */
            if (param.Contains("uvccameraviewer") && ConnectedCameraCount>0)
            {
                if (parameters?.Count > 1)
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
                                    if (elem.Substring(2).Length > 0)
                                    {
                                        if (elem.Substring(2).Length < 2)
                                        {
                                            MethodName = elem.Substring(2, 1);
                                            MethodNameIsPresent = true;
                                        }
                                        else
                                        {
                                            //wierd entry
                                            WrongParameter = true;
                                        }
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
                                        if (sw == 1)
                                        {
                                            SwitchIsPresent = true;
                                            parameterOnOffSwitch = true;
                                        }
                                        else if(sw == 0)
                                        {
                                            SwitchIsPresent = true;
                                            parameterOnOffSwitch = false;
                                        }
                                        else
                                        {
                                            WrongParameter = true;
                                            i = parameters.Count;
                                        }
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
                                case "t":
                                    try
                                    {
                                        int v = CheckIntervalValue(Int32.Parse(elem.Substring(2)));
                                        if (v >= 0)
                                        {
                                            parameterTime = v;
                                            TimerIsPresent = true;
                                        }
                                        else
                                        {
                                            WrongParameter = true;
                                        }
                                        //CurrentTestResult = parameterTime + " time parameter";
                                        //CameraIndex = -1;
                                    }
                                    catch (ArgumentNullException anx)
                                    {
                                        WrongParameter = true;
                                        //i = parameters.Count;
                                    }
                                    catch (FormatException fx)
                                    {
                                        WrongParameter = true;
                                        //i = parameters.Count;
                                    }
                                    catch (OverflowException ofx)
                                    {
                                        WrongParameter = true;
                                        //i = parameters.Count;
                                    }
                                    break;

                            }
                        }
                        catch (Exception e)
                        {
                            WrongParameter = true;
                            Logger.Add(e);
                        }
                        //WhichCase = elem;
                    }

                    for(int j = 1; j < parameters.Count; j++)
                    {
                        elem = parameters.ElementAt(j).ToLower();
                            switch (elem.Substring(0, 1))
                            {
                            case "c":
                                try
                                {
                                    int ci = Int32.Parse(elem.Substring(2));
                                    if (CheckCameraIndex(ci - 1))
                                    {
                                        CamIndexIsPresent = true;
                                    }
                                    else
                                    {
                                        WrongParameter = true;
                                    }
                                }
                                catch (ArgumentNullException anx)
                                {
                                    WrongParameter = true;
                                    j = parameters.Count;
                                }
                                catch (FormatException fx)
                                {
                                    WrongParameter = true;
                                    j = parameters.Count;
                                }
                                catch (OverflowException ofx)
                                {
                                    WrongParameter = true;
                                    j = parameters.Count;
                                }
                                break;
                        }
                    }

                    #region OLD SWITCH LOGIC

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

                    // METHOD
                    //switch (MethodName)
                    //{
                    //No method
                    //case "":                            
                    //if (wakeUpCall && CheckCameraIndex(cameraIndex) && (cameraIndex >= 0 && cameraIndex < 4))
                    //{
                    //    //START the camera

                    //}
                    //else if(wakeUpCall && (cameraIndex == 8))
                    //{

                    //}
                    //CurrentTestResult = "No method " + cameraIndex;
                    //break;

                    // Show Settings window
                    //case "c":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && SwitchIsPresent)
                    //        {
                    //            if (parameterOnOffSwitch)
                    //            {
                    //                CurrentTestResult = "Showing Settings window";
                    //                if (MainForm.Settingui != null && MainForm.Settingui.Visible == false)
                    //                {
                    //                    //MainForm.GetMainForm.TopMost = false;
                    //                    //WhichCase = "Showing Settings window";
                    //                    MainForm.Settingui.ShowSettings(CameraIndex);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                CurrentTestResult = "Hiding Settings window " + CameraIndex;
                    //                MainForm.Settingui?.Hide();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            CurrentTestResult = "C missing switch parameter or wrong index";
                    //        }
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        CurrentTestResult = "FAILURE";
                    //        Trace.Add(e.ToString() + " in method c");
                    //    }
                    //    break;

                    // Change main camera
                    //case "n":
                    //    if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    //    {
                    //        if (MULTI_WINDOW.formList[CameraIndex]?.DISPLAYED == true && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false && MULTI_WINDOW.formList[Properties.Settings.Default.main_camera_index].recordingInProgress == false)
                    //        {
                    //            //MULTI_WINDOW.formList[Properties.Settings.Default.main_camera_index].Text = "UVC Camera Viewer -  camera " + (Properties.Settings.Default.main_camera_index + 1);
                    //            //MULTI_WINDOW.formList[cameraIndex].Text = $"UVC Camera Viewer - MAIN CAMERA {(cameraIndex + 1)}";
                    //            //Properties.Settings.Default.main_camera_index = cameraIndex;

                    //            //MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Normal;
                    //            //MULTI_WINDOW.formList[cameraIndex].Activate();
                    //            //if (!Properties.Settings.Default.show_all_cams_simulteneously)
                    //            //{
                    //            //    MULTI_WINDOW.formList[cameraIndex].WindowState = FormWindowState.Minimized;
                    //            //}
                    //            Properties.Settings.Default.main_camera_index = CameraIndex;
                    //            Properties.Settings.Default.Save();
                    //            MULTI_WINDOW.FormSettingsChanged();
                    //        }
                    //        else
                    //        {
                    //            Logger.Add(Resource.parameter_execution_failure + " m=" + MethodName + ", c=" + CameraIndex);
                    //        }
                    //        CurrentTestResult = "N case, camera index conditions passed";
                    //        PARAM.Clear();
                    //    }
                    //    else
                    //    {
                    //        CurrentTestResult = "N case, conditions faled " + CameraIndex + " " + CheckCameraIndex(CameraIndex) + " " + (CameraIndex >= 0 && CameraIndex < 4);
                    //    }
                    //    break;

                    // SNAPSHOT
                    //case "s":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    //        {
                    //            if (wakeUpCall)
                    //            {
                    //                wakeUpCall = false;
                    //                //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                    //                //{
                    //                //    SNAPSHOT_SAVER.TakeAsyncSnapShot(true, cameraIndex, "event");
                    //                //}
                    //                //else 

                    //                SNAPSHOT_SAVER.TakeAsyncSnapShot(false, CameraIndex, "event");

                    //            }
                    //        }
                    //        else
                    //        {
                    //            //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                    //            //{
                    //            //    SNAPSHOT_SAVER.TakeSnapShotAll();
                    //            //}
                    //            //else 
                    //            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false)
                    //            {
                    //                //if (CheckCameraIndex(cameraIndex) && cameraIndex == 8)
                    //                //{
                    //                //    SNAPSHOT_SAVER.TakeSnapShotAll();
                    //                //}
                    //                //else 

                    //                SNAPSHOT_SAVER.TakeSnapShot(CameraIndex, "event");
                    //                //SNAPSHOT_SAVER.TakeAsyncSnapShot();
                    //            }
                    //        }

                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.Message + " in method s");
                    //    }
                    //    break;

                    // SHOW / HIDE CONTROL BUTTONS
                    //case "b":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    //        {
                    //            if (parameterOnOffSwitch)
                    //            {
                    //                isControlButtonVisible = true;
                    //                MULTI_WINDOW.formList[CameraIndex].SettingChangesApply(CameraIndex);
                    //            }
                    //            else
                    //            {
                    //                isControlButtonVisible = false;
                    //                MULTI_WINDOW.formList[CameraIndex].SettingChangesApply(CameraIndex);
                    //            }                                                                       
                    //        }
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method b");
                    //    }
                    //    break;

                    // Enable/Disable Face detection
                    //case "d":
                    //    try
                    //    {

                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method d");
                    //    }
                    //    break;

                    // IR Sensor
                    //case "h":
                    //    try
                    //    {
                    //        if (wakeUpCall)
                    //        {
                    //            if (parameterTime > 0 && CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    //            {
                    //                //when there is parameterTime, cameraindex is a must
                    //                CurrentTestResult = "HUMAN SENSOR parameterTime index 1-4" + parameterTime + " " + CameraIndex;
                    //            }
                    //            else if (parameterTime > 0 && CheckCameraIndex(CameraIndex) && CameraIndex == 8)
                    //            {
                    //                CurrentTestResult = "HUMAN SENSOR parameterTime index 9 " + parameterTime + " " + CameraIndex;
                    //            }
                    //            else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false)
                    //            {
                    //                if (parameterOnOffSwitch)
                    //                {
                    //                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                    //                    PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                    //                    MainForm.AllChangesApply();
                    //                    PROPERTY_FUNCTIONS.SetCycleTime(CameraIndex, parameterTime);
                    //                }
                    //                CurrentTestResult = "HUMAN SENSOR no time index 1-4" + parameterTime + " " + CameraIndex;
                    //            }
                    //        }
                    //        else//the app is running
                    //        {
                    //            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false)
                    //            {
                    //                if (parameterOnOffSwitch)
                    //                {
                    //                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                    //                    PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                    //                }
                    //                else
                    //                {
                    //                    if (MainForm.RSensor != null)
                    //                    {
                    //                        MainForm.RSensor.Stop_IR_Timer();
                    //                    }

                    //                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, false);
                    //                }
                    //                MainForm.AllChangesApply();
                    //                PROPERTY_FUNCTIONS.SetCycleTime(CameraIndex, parameterTime);
                    //            }
                    //        }


                    //        //CurrentTestResult = "Human Sensor " + cameraIndex + " " + parameterTime + " " + wakeUpCall;
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.Message + " in method h");
                    //    }
                    //    break;

                    // Visible - Active
                    //case "v":
                    //        if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && SwitchIsPresent)
                    //        {
                    //            for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                    //            {
                    //                if (parameterOnOffSwitch)
                    //                {
                    //                    CurrentTestResult = "Show all windows";
                    //                    isMinimized = false;
                    //                    try
                    //                    {
                    //                        MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                    //                        MULTI_WINDOW.formList[i].Show();
                    //                        MULTI_WINDOW.formList[i].Activate();
                    //                    }
                    //                    catch (ArgumentOutOfRangeException)
                    //                    {

                    //                    }                                            
                    //                }
                    //                else
                    //                {
                    //                    CurrentTestResult = "Hiding all windows";
                    //                    isMinimized = true;
                    //                    MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
                    //                }
                    //            }
                    //        CurrentTestResult = "Show all windows";
                    //        }
                    //        else if (CheckCameraIndex(CameraIndex))
                    //        {
                    //            if (parameterOnOffSwitch)
                    //            {
                    //                CurrentTestResult = "Show 1 window";
                    //                isMinimized = false;
                    //                try
                    //                {
                    //                    MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Normal;
                    //                    MULTI_WINDOW.formList[CameraIndex]?.Show();
                    //                    MULTI_WINDOW.formList[CameraIndex]?.Activate();
                    //                }
                    //                catch (NullReferenceException)
                    //                {

                    //                }
                    //            }
                    //            else
                    //            {
                    //                CurrentTestResult = "Hiding 1 window";
                    //                isMinimized = true;
                    //                try
                    //                {
                    //                    MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
                    //                }
                    //                catch (NullReferenceException)
                    //                {

                    //                }                                        
                    //            }                                
                    //        }

                    //        PARAMETERS.PARAM.Clear();  
                    //    break;

                    // Backlight
                    //case "l":
                    //    try
                    //    {
                    //        if (parameterOnOffSwitch)
                    //        {
                    //            MainForm.GetMainForm.BackLight.ON();
                    //        }
                    //        else
                    //        {
                    //            MainForm.GetMainForm.BackLight.OFF();
                    //        }
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method l");
                    //    }
                    //    break;

                    // Window pane
                    //case "w":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    //        {
                    //            if (parameterOnOffSwitch)
                    //            {
                    //                PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(CameraIndex, true);
                    //                Properties.Settings.Default.Save();
                    //                if (MainForm.GetMainForm != null)
                    //                    MainForm.AllChangesApply();
                    //            }
                    //            else
                    //            {
                    //                //There is no requirement to display with panels off
                    //                //PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(cameraIndex, false);
                    //            }                                    
                    //        }else if (CheckCameraIndex(CameraIndex) && (CameraIndex == 8))
                    //        {
                    //            //Support all

                    //        }
                    //        PARAMETERS.PARAM.Clear();
                    //        CurrentTestResult = "W " + parameterOnOffSwitch + " " + CameraIndex;
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method w");
                    //    }
                    //    break;

                    // Close a window or all (exit app)
                    //case "q":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex))
                    //        {
                    //            if (CameraIndex >= 0 && CameraIndex<4)
                    //            {
                    //                MULTI_WINDOW.formList[CameraIndex].Close();
                    //            }
                    //            else if (CameraIndex == 8)
                    //            {                                        
                    //                Application.Exit();
                    //            }
                    //        }
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method q");
                    //    }
                    //    break;

                    // Event recorder
                    //case "e":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && (CameraIndex == Properties.Settings.Default.main_camera_index)) // Main camera
                    //        {
                    //            if (parameterOnOffSwitch && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false)
                    //            {
                    //                MULTI_WINDOW.EventRecorderOn(CameraIndex);
                    //            }
                    //            else if (!parameterOnOffSwitch)
                    //            {
                    //                MULTI_WINDOW.formList[CameraIndex].HideIcon();
                    //                MULTI_WINDOW.EventRecorderOff(CameraIndex);
                    //            }
                    //        }
                    //        else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))  // Not main camera                              
                    //        {
                    //            if (parameterOnOffSwitch && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false)
                    //            {
                    //                MULTI_WINDOW.formList[CameraIndex].crossbar.recordFromParamNotMain = true;
                    //                MULTI_WINDOW.formList[CameraIndex].SetRecordIcon(CameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                    //                MULTI_WINDOW.formList[CameraIndex].crossbar.Start(CameraIndex, CAMERA_MODES.MANUAL);
                    //            }
                    //            else if (!parameterOnOffSwitch)
                    //            {
                    //                MULTI_WINDOW.formList[CameraIndex].HideIcon();
                    //                MULTI_WINDOW.formList[CameraIndex].SetToPreviewMode();
                    //            }
                    //        }

                    //        PARAMETERS.PARAM.Clear();
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method e");
                    //    }
                    //    break;

                    // Manual recording
                    //case "r":
                    //    try
                    //    {
                    //        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false)
                    //        {
                    //            if (parameterOnOffSwitch)
                    //            {
                    //                MULTI_WINDOW.formList[CameraIndex].SetRecordIcon(CameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                    //                MULTI_WINDOW.formList[CameraIndex].crossbar.Start(CameraIndex, CAMERA_MODES.MANUAL);
                    //            }
                    //        }
                    //        else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && !parameterOnOffSwitch)
                    //        {
                    //            MULTI_WINDOW.formList[CameraIndex].HideIcon();
                    //            if (PROPERTY_FUNCTIONS.CheckPreEventTimes(CameraIndex))
                    //            {
                    //                if (MULTI_WINDOW.displayedCameraCount > 0)
                    //                {
                    //                    MULTI_WINDOW.formList[CameraIndex].SetToPreeventMode();
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (MULTI_WINDOW.displayedCameraCount > 0)
                    //                {
                    //                    MULTI_WINDOW.formList[CameraIndex].SetToPreviewMode();
                    //                }
                    //            }
                    //        }
                    //        PARAMETERS.PARAM.Clear();
                    //    }
                    //    catch (ArgumentOutOfRangeException e)
                    //    {
                    //        Logger.Add(e.ToString() + " in method r");
                    //    }
                    //    break;
                    //default:
                    //    //throw new Exception("Wrong Method Name");                            
                    //    CurrentTestResult = "Wrong Method Name";
                    //    break;
                    #endregion OLD SWITCH LOGIC
                }
            }
            else
            {
                WrongParameter = true;
            }

            /*
                     * There are 6 variations:
                     * ================
                     * M+S+C+T      All TTTT
                     * M+S+C            TTTF
                     * M+S              TTFF
                     * M+C              TFTF
                     * M                TFFF
                     * C                FFTF
                     *                  FFFF
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
                case "FFFF":
                    if(!WrongParameter)
                        WhiteMamba();
                    break;
                case "TTFT":
                    WrongParameter = true;
                    break;
                case "FTFF":
                    WrongParameter = true;
                    break;
            }
            WAKEUPCALL = false;
        }

        private static int CheckIntervalValue(int v)
        {
            if (v>=0 && v<=1000)
            {
                return v;
            }
            else
            {
                return -1;
            }
        }

        private static List<string> CleanUpTheParams(List<string> list)
        {
            for (int item = list.Count - 1; item > 0; item--)
            {
                if (list[item].Length != 3)
                {
                    list.RemoveAt(item);
                }
            }
            return list;
        }
        /// <summary>
        /// FFFF
        /// </summary>
        private static void WhiteMamba()
        {
            //Only show the main camera
            CurrentTestResult = "WhiteMamba";
        }
        /// <summary>
        /// FFTF
        /// </summary>
        private static void DontTouchMyCamsCase()
        {
            switch (MethodName)
            {
                case " ":// FFTF
                    if (WAKEUPCALL && CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                    {
                        for (int i = 0; i < Properties.Settings.Default.camera_count; i++)
                        {
                            if (i != CameraIndex)
                            {
                                minimizedByParameter[i] = true;
                                callByParameters = true;
                                minimizedByParameters = true;
                            }
                        }
                    }
                    else if (WAKEUPCALL && (CameraIndex == 8))
                    {
                        //Start all cams
                    }
                    CurrentTestResult = "No method " + CameraIndex;
                    break;

                default:// FFTF
                    WrongParameter = true;
                    break;
            }            
        }

        /// <summary>
        /// TFFF
        /// </summary>
        private static void BossIsAlwaysRightCase()
        {
            switch (MethodName)
            {
                // Snapshot
                case "s":// TFFF
                    try
                    {
                        if (!WAKEUPCALL)
                        {
                            SNAPSHOT_SAVER.TakeSnapShot(MainCamera, "event");
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Event recorder
                case "e":// TFFF
                    try
                    {
                        if (!WAKEUPCALL && MULTI_WINDOW.formList[MainCamera]?.recordingInProgress == false)
                        {
                            MULTI_WINDOW.EventRecorderOn(MainCamera);
                        }
                        else if(WAKEUPCALL)
                        {
                            WrongParameter = true;
                            
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Close form / Quit application
                case "q":// TFFF
                    try
                    {
                        if (!WAKEUPCALL)
                        {
                            MULTI_WINDOW.formList[MainCamera].applicationExit = true;
                            Application.Exit();
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                        WrongParameter = true;
                    }
                    break;

                // Change main camera
                case "n"://TFFF
                    if (!WAKEUPCALL)
                    {                        
                        if (MULTI_WINDOW.formList[GetNextCameraIndex(MainCamera)]?.DISPLAYED == true && MULTI_WINDOW.formList[GetNextCameraIndex(MainCamera)]?.recordingInProgress == false && MULTI_WINDOW.formList[GetNextCameraIndex(MainCamera)]?.recordingInProgress == false)
                        {
                            Properties.Settings.Default.main_camera_index = GetNextCameraIndex(MainCamera);
                            Properties.Settings.Default.Save();
                            MULTI_WINDOW.FormSettingsChanged();                            
                        }
                        else
                        {
                            Logger.Add(Resource.parameter_execution_failure + " m=" + MethodName + ", c=" + CameraIndex);
                            CurrentTestResult = "main camera change?";
                        }
                        PARAM.Clear();                     
                    }
                    else
                    {
                        WrongParameter = true;
                    }

                    break;
                default:
                    WrongParameter = true;
                    break;
            }
            PARAM.Clear();
            //CurrentTestResult = "MAIN CAMERA SET";
        }

        /// <summary>
        /// TFTF
        /// </summary>
        private static void CheckMateCase()
        {
            switch (MethodName)
            {
                // Snapshot
                case "s":// TFTF
                    try
                    {
                        if (!WAKEUPCALL)
                        {
                            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                            {
                                SNAPSHOT_SAVER.TakeAsyncSnapShot(false, CameraIndex, "event");
                            }
                            else if (CheckCameraIndex(CameraIndex) && CameraIndex == 8)
                            {
                                SNAPSHOT_SAVER.TakeSnapShotAll();
                            }
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Event recorder
                case "e":// TFTF
                    try
                    {
                        if (!WAKEUPCALL && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false) 
                        {
                            MULTI_WINDOW.EventRecorderOn(CameraIndex);
                        }
                        else if(WAKEUPCALL)
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Close form / Quit application
                case "q":// TFTF
                    try
                    {
                        if (!WAKEUPCALL && CheckCameraIndex(CameraIndex))
                        {
                            if (CameraIndex >= 0 && CameraIndex < 4)
                            {
                                MULTI_WINDOW.formList[CameraIndex]?.Close();
                            }
                            else if (CameraIndex == 8)
                            {
                                Application.Exit();
                            }
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                        WrongParameter = true;
                    }
                    break;

                // Change main camera
                case "n"://TFTF
                    if (!WAKEUPCALL)
                    {
                        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                        {
                            if (MULTI_WINDOW.formList[CameraIndex]?.DISPLAYED == true && MULTI_WINDOW.formList[CameraIndex].recordingInProgress == false && MULTI_WINDOW.formList[Properties.Settings.Default.main_camera_index].recordingInProgress == false)
                            {
                                Properties.Settings.Default.main_camera_index = CameraIndex;
                                Properties.Settings.Default.Save();
                                MULTI_WINDOW.FormSettingsChanged();
                            }
                            else
                            {
                                Logger.Add(Resource.parameter_execution_failure + " m=" + MethodName + ", c=" + CameraIndex);
                            }
                            PARAM.Clear();
                        }
                        CurrentTestResult = MethodName + " case, camera index conditions passed";
                    }
                    else
                    {
                        //can not call from start
                        WrongParameter = true;
                    }

                    break;
                default:// TFTF
                    WrongParameter = true;
                    break;
            }
            PARAM.Clear();
            CurrentTestResult = MethodName;
        }

        /// <summary>
        /// TTFF
        /// </summary>
        private static void MethodSwitchCase()
        {
            //Deal with wakeupcall
            switch (MethodName)
            {
                // Show / Hide buttons
                case "b"://TTFF
                    try
                    {
                        if (!WAKEUPCALL)
                        {
                            if (parameterOnOffSwitch)
                            {
                                isControlButtonVisible = true;
                            }
                            else
                            {
                                isControlButtonVisible = false;
                            }
                            MULTI_WINDOW.formList[MainCamera]?.SettingChangesApply(MainCamera);
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Show settings panel
                case "c":// TTFF
                    try
                    {
                        if(!WAKEUPCALL)
                        {
                            if (parameterOnOffSwitch)
                            {
                                if (MainForm.Settingui != null && MainForm.Settingui.Visible == false)
                                {
                                    MainForm.Settingui.ShowSettings(MainCamera);
                                }
                            }
                            else
                            {                                
                                MainForm.Settingui?.ForgetAndClose();
                            }
                        }
                        else
                        {
                            if (parameterOnOffSwitch)
                            {
                                if (MainForm.Settingui != null && MainForm.Settingui.Visible == false)
                                {
                                    MainForm.Settingui.ShowSettings(MainCamera);
                                }
                            }
                            else
                            {
                                WrongParameter = true;
                            }
                        }                        
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }                    
                    break;

                // IR Sensor
                case "h":// TTFF
                    if (MULTI_WINDOW.formList[MainCamera]?.recordingInProgress == false)
                    {
                        PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(MainCamera, out bool IrSensorEnabled);
                        if (parameterOnOffSwitch)
                        {
                            if (!IrSensorEnabled)
                            {
                                PROPERTY_FUNCTIONS.Set_Human_Sensor(MainCamera, true);
                                PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(MainCamera, true);
                                if (MainForm.GetMainForm != null)
                                {
                                    MainForm.RSensor?.Stop_IR_Timer();
                                    MainForm.RSensor?.SetInterval();
                                    MainForm.RSensor?.Start_IR_Timer();
                                }
                            }
                        }
                        else if(!parameterOnOffSwitch && !WAKEUPCALL)
                        {
                            if (IrSensorEnabled)
                            {
                                PROPERTY_FUNCTIONS.Set_Human_Sensor(MainCamera, false);
                                if (MainForm.GetMainForm != null)
                                {
                                    if (MainForm.RSensor != null)
                                    {
                                        MainForm.RSensor.Stop_IR_Timer();
                                    }
                                }
                                //if (MainForm.GetMainForm != null)
                                //{
                                //    MainForm.AllChangesApply();
                                //}
                            }
                        }                        
                    }
                    break;

                //Face recognition
                case "d":
                    if (WAKEUPCALL)
                    {
                        //only On switch is allowed
                    }
                    else
                    {
                        //both On and Off switch checked
                    }
                    break;

                // Manual record
                case "r"://TTFF
                    try
                    {
                        if (!WAKEUPCALL && MULTI_WINDOW.formList[MainCamera]?.recordingInProgress == false)
                        {
                            if (parameterOnOffSwitch)
                            {
                                MULTI_WINDOW.formList[MainCamera]?.SetRecordIcon(MainCamera, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                MULTI_WINDOW.formList[MainCamera]?.crossbar.Start(MainCamera, CAMERA_MODES.MANUAL);
                            }
                            else
                            {
                                MULTI_WINDOW.formList[MainCamera]?.HideIcon();
                                if (PROPERTY_FUNCTIONS.CheckPreEventTimes(MainCamera))
                                {
                                    if (MULTI_WINDOW.displayedCameraCount > 0)
                                    {
                                        MULTI_WINDOW.formList[MainCamera]?.SetToPreeventMode();
                                    }
                                }
                                else
                                {
                                    if (MULTI_WINDOW.displayedCameraCount > 0)
                                    {
                                        MULTI_WINDOW.formList[MainCamera]?.SetToPreviewMode();
                                    }
                                }
                            }
                        }
                        else if(WAKEUPCALL)
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Visible
                case "v"://TTFF
                    if (WAKEUPCALL)
                    {
                        if (!parameterOnOffSwitch)
                        {
                            CurrentTestResult = "Hiding 1 window";
                            //isMinimized = true;
                            //MULTI_WINDOW.formList[MainCamera].WindowState = FormWindowState.Minimized;
                            minimizedByParameter[0] = false;
                            minimizedByParameter[1] = false;
                            minimizedByParameter[2] = false;
                            minimizedByParameter[3] = false;
                            minimizedByParameter[MainCamera] = true;
                            callByParameters = true;
                            minimizedByParameters = true;
                        }
                    }
                    else
                    {
                        if (parameterOnOffSwitch)
                        {
                            CurrentTestResult = "Show 1 window";
                            //isMinimized = false;
                            try
                            {
                                switch (MainCamera)
                                {
                                    case 0:
                                        if (Properties.Settings.Default.C1_full_screen)
                                        {
                                            MULTI_WINDOW.formList[0].WindowState = FormWindowState.Maximized;
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[0].WindowState = FormWindowState.Normal;
                                        }
                                        break;
                                    case 1:
                                        if (Properties.Settings.Default.C2_full_screen)
                                        {
                                            MULTI_WINDOW.formList[1].WindowState = FormWindowState.Maximized;
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[1].WindowState = FormWindowState.Normal;
                                        }
                                        break;
                                    case 2:
                                        if (Properties.Settings.Default.C3_full_screen)
                                        {
                                            MULTI_WINDOW.formList[2].WindowState = FormWindowState.Maximized;
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[2].WindowState = FormWindowState.Normal;
                                        }
                                        break;
                                    case 3:
                                        if (Properties.Settings.Default.C4_full_screen)
                                        {
                                            MULTI_WINDOW.formList[3].WindowState = FormWindowState.Maximized;
                                        }
                                        else
                                        {
                                            MULTI_WINDOW.formList[3].WindowState = FormWindowState.Normal;
                                        }
                                        break;
                                }
                                //MULTI_WINDOW.formList[MainCamera].WindowState = FormWindowState.Normal;
                                MULTI_WINDOW.formList[MainCamera]?.Show();
                                MULTI_WINDOW.formList[MainCamera]?.Activate();
                            }
                            catch (NullReferenceException)
                            {

                            }
                        }
                        else
                        {
                            CurrentTestResult = "Hiding 1 window";
                            //isMinimized = true;
                            try
                            {
                                MULTI_WINDOW.formList[MainCamera].WindowState = FormWindowState.Minimized;
                            }
                            catch (NullReferenceException)
                            {

                            }
                        }
                    }
                    PARAM.Clear();
                    break;

                // Backlight
                case "l"://TTFF
                    try
                    {
                        if(!WAKEUPCALL)
                        {
                            if (parameterOnOffSwitch)
                            {
                                MainForm.GetMainForm?.BackLight.ON();
                            }
                            else
                            {
                                MainForm.GetMainForm?.BackLight.OFF();
                            }
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    PARAM.Clear();
                    break;

                // Window pane
                case "w"://TTFF
                    try
                    {
                        if(WAKEUPCALL)
                        {
                            if (parameterOnOffSwitch)
                            {
                                PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(MainCamera, true);
                                Properties.Settings.Default.Save();
                                if (MainForm.GetMainForm != null)
                                    MainForm.AllChangesApply();
                            }
                            else
                            {
                                WrongParameter = true;
                            }
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;
            }
            PARAM.Clear();
            CurrentTestResult = MethodName;
        }

        /// <summary>
        /// TTTF
        /// </summary>
        private static void TimelessCase()
        {
            switch (MethodName)
            {
                // Show / Hide buttons
                case "b"://TTTF
                    if (!WAKEUPCALL)
                    {
                        try
                        {
                            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                            {
                                if (parameterOnOffSwitch)
                                {
                                    isControlButtonVisible = true;
                                    MULTI_WINDOW.formList[CameraIndex]?.SettingChangesApply(CameraIndex);
                                }
                                else
                                {
                                    isControlButtonVisible = false;
                                    MULTI_WINDOW.formList[CameraIndex]?.SettingChangesApply(CameraIndex);
                                }
                            }
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Logger.Add(e);
                        }
                    }
                    else
                    {
                        WrongParameter = true;
                    }
                    break;

                // Visible
                case "v"://TTTF
                    if(WAKEUPCALL)
                    {
                        //StartAndHideWindows();
                        if (parameterOnOffSwitch)
                        {
                            minimizedByParameter[0] = true;
                            minimizedByParameter[1] = true;
                            minimizedByParameter[2] = true;
                            minimizedByParameter[3] = true;
                            minimizedByParameter[CameraIndex] = false;
                            callByParameters = true;
                            minimizedByParameters = true;
                        }
                        else
                        { 
                            minimizedByParameter[0] = false;
                            minimizedByParameter[1] = false;
                            minimizedByParameter[2] = false;
                            minimizedByParameter[3] = false;
                            minimizedByParameter[CameraIndex] = true;
                            callByParameters = true;
                            minimizedByParameters = true;
                        }
                    }
                    else
                    {
                        ShowOrHideWindows();
                    }
                    CurrentTestResult = "Show All Windows";
                    PARAM.Clear();
                    break;

                // Backlight
                case "l"://TTTF
                    if (!WAKEUPCALL)
                    {
                        try
                        {
                            if (parameterOnOffSwitch)
                            {
                                MainForm.GetMainForm?.BackLight.ON();
                            }
                            else
                            {
                                MainForm.GetMainForm?.BackLight.OFF();
                            }
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Logger.Add(e);
                        }
                    }
                    else
                    {
                        WrongParameter = true;
                    }
                    
                    break;

                // Window pane
                case "w"://TTTF
                    try
                    {
                        if (WAKEUPCALL)
                        {
                            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                            {
                                if (parameterOnOffSwitch)
                                {
                                    PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(CameraIndex, true);
                                    Properties.Settings.Default.Save();
                                }
                                else
                                {
                                    WrongParameter = true;
                                }
                            }
                            else if (CheckCameraIndex(CameraIndex) && (CameraIndex == 8))
                            {
                                //Support all
                                if (parameterOnOffSwitch)
                                {
                                    for (int i = 0; i < Properties.Settings.Default.camera_count; i++)
                                    {
                                        PROPERTY_FUNCTIONS.SetShowWindowPaneSwitch(i, true);
                                    }
                                    Properties.Settings.Default.Save();
                                }
                                else
                                {
                                    WrongParameter = true;
                                }
                            }
                            PARAM.Clear();
                        }
                        else
                        {
                            WrongParameter = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Manual record
                case "r"://TTTF
                    try
                    {
                        if (!WAKEUPCALL)
                        {
                            if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false)
                            {
                                if (parameterOnOffSwitch)
                                {
                                    MULTI_WINDOW.formList[CameraIndex]?.SetRecordIcon(CameraIndex, decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                                    MULTI_WINDOW.formList[CameraIndex]?.crossbar.Start(CameraIndex, CAMERA_MODES.MANUAL);
                                }
                            }
                            else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && !parameterOnOffSwitch)
                            {
                                MULTI_WINDOW.formList[CameraIndex]?.HideIcon();
                                if (PROPERTY_FUNCTIONS.CheckPreEventTimes(CameraIndex))
                                {
                                    if (MULTI_WINDOW.displayedCameraCount - 1 >= CameraIndex)
                                    {
                                        MULTI_WINDOW.formList[CameraIndex]?.SetToPreeventMode();
                                    }
                                }
                                else
                                {
                                    if (MULTI_WINDOW.displayedCameraCount - 1 >= CameraIndex)
                                    {
                                        MULTI_WINDOW.formList[CameraIndex]?.SetToPreviewMode();
                                    }
                                }
                            }
                        }
                        else
                        {
                            WrongParameter = true;
                        }                        
                        PARAM.Clear();
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // IR Sensor
                case "h"://TTTF
                    if(!WAKEUPCALL)
                    {
                        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false)
                        {
                            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(CameraIndex, out bool IrSensorEnabled);
                            if (parameterOnOffSwitch)
                            {
                                if (!IrSensorEnabled)
                                {
                                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                                    PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                                    if (MainForm.GetMainForm != null)
                                    {
                                        MainForm.RSensor?.Stop_IR_Timer();
                                        MainForm.RSensor?.SetInterval();
                                        MainForm.RSensor?.Start_IR_Timer();
                                        //MainForm.AllChangesApply();
                                    }
                                }
                            }
                            else
                            {
                                if (IrSensorEnabled)
                                {
                                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, false);
                                    if (MainForm.GetMainForm != null)
                                    {
                                        if (MainForm.RSensor != null)
                                        {
                                            MainForm.RSensor.Stop_IR_Timer();
                                        }
                                        //MainForm.AllChangesApply();
                                    }
                                }
                            }
                        }
                        else if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && MULTI_WINDOW.RecordingIsOn() == false)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                PROPERTY_FUNCTIONS.Set_Human_Sensor(i, parameterOnOffSwitch);
                                if (parameterOnOffSwitch)
                                    PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(i, parameterOnOffSwitch);
                            }

                            //if (MainForm.GetMainForm != null)
                            //{
                            //    //MainForm.AllChangesApply();
                            //}
                        }
                    }
                    else
                    {
                        if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                        {
                            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(CameraIndex, out bool IrSensorEnabled);
                            if (parameterOnOffSwitch)
                            {
                                if (!IrSensorEnabled)
                                {
                                    PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                                    PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                                    if (MainForm.GetMainForm != null)
                                    {
                                        MainForm.RSensor?.Stop_IR_Timer();
                                        MainForm.RSensor?.SetInterval();
                                        MainForm.RSensor?.Start_IR_Timer();
                                        //MainForm.AllChangesApply();
                                    }
                                }
                            }
                            else
                            {
                                WrongParameter = true;
                            }
                        }
                        else if (CheckCameraIndex(CameraIndex) && CameraIndex == 8)
                        {
                            if(parameterOnOffSwitch)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    PROPERTY_FUNCTIONS.Set_Human_Sensor(i, parameterOnOffSwitch);
                                    if (parameterOnOffSwitch)
                                        PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(i, parameterOnOffSwitch);
                                }
                            }
                            else
                            {
                                WrongParameter = true;
                            }
                            //if (MainForm.GetMainForm != null)
                            //{
                            //    //MainForm.AllChangesApply();
                            //}
                        }
                    }
                    
                    //No need to remember the state
                    //Properties.Settings.Default.Save();
                    break;

                // Face recognition
                case "d":
                    WrongParameter = true;
                    break;

                default:
                    WrongParameter = true;
                    break;

                //case "c":
                //    //C parameter does not require Time
                //    WrongParameter = true;
                //    break;
            }
            CurrentTestResult = MethodName;
        }

        private static void ShowOrHideWindows()
        {
            if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && SwitchIsPresent)
            {
                for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                {
                    if (parameterOnOffSwitch && MULTI_WINDOW.formList[i].DISPLAYED)
                    {
                        CurrentTestResult = "Show all windows";
                        //isMinimized = false;
                        try
                        {
                            switch (CameraIndex)
                            {
                                case 0:
                                    if (Properties.Settings.Default.C1_full_screen)
                                    {
                                        MULTI_WINDOW.formList[0].WindowState = FormWindowState.Maximized;
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.formList[0].WindowState = FormWindowState.Normal;
                                    }
                                    break;
                                case 1:
                                    if (Properties.Settings.Default.C2_full_screen)
                                    {
                                        MULTI_WINDOW.formList[1].WindowState = FormWindowState.Maximized;
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.formList[1].WindowState = FormWindowState.Normal;
                                    }
                                    break;
                                case 2:
                                    if (Properties.Settings.Default.C3_full_screen)
                                    {
                                        MULTI_WINDOW.formList[2].WindowState = FormWindowState.Maximized;
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.formList[2].WindowState = FormWindowState.Normal;
                                    }
                                    break;
                                case 3:
                                    if (Properties.Settings.Default.C4_full_screen)
                                    {
                                        MULTI_WINDOW.formList[3].WindowState = FormWindowState.Maximized;
                                    }
                                    else
                                    {
                                        MULTI_WINDOW.formList[3].WindowState = FormWindowState.Normal;
                                    }
                                    break;
                            }
                            MULTI_WINDOW.formList[i]?.Show();
                            MULTI_WINDOW.formList[i]?.Activate();
                        }
                        catch (ArgumentOutOfRangeException)
                        {

                        }
                    }
                    else
                    {
                        CurrentTestResult = "Hiding all windows";
                        //isMinimized = true;
                        MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
                    }
                }
                CurrentTestResult = "Show all windows";
            }
            else if (CheckCameraIndex(CameraIndex))
            {
                if (parameterOnOffSwitch && MULTI_WINDOW.formList[CameraIndex].DISPLAYED)
                {
                    CurrentTestResult = "Show 1 window";
                    //isMinimized = false;
                    try
                    {
                        switch (CameraIndex)
                        {
                            case 0:
                                if (Properties.Settings.Default.C1_full_screen)
                                {
                                    MULTI_WINDOW.formList[0].WindowState = FormWindowState.Maximized;
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[0].WindowState = FormWindowState.Normal;
                                }
                                break;
                            case 1:
                                if (Properties.Settings.Default.C2_full_screen)
                                {
                                    MULTI_WINDOW.formList[1].WindowState = FormWindowState.Maximized;
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[1].WindowState = FormWindowState.Normal;
                                }
                                break;
                            case 2:
                                if (Properties.Settings.Default.C3_full_screen)
                                {
                                    MULTI_WINDOW.formList[2].WindowState = FormWindowState.Maximized;
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[2].WindowState = FormWindowState.Normal;
                                }
                                break;
                            case 3:
                                if (Properties.Settings.Default.C4_full_screen)
                                {
                                    MULTI_WINDOW.formList[3].WindowState = FormWindowState.Maximized;
                                }
                                else
                                {
                                    MULTI_WINDOW.formList[3].WindowState = FormWindowState.Normal;
                                }
                                break;
                        }
                        MULTI_WINDOW.formList[CameraIndex]?.Show();
                        MULTI_WINDOW.formList[CameraIndex]?.Activate();
                    }
                    catch (NullReferenceException)
                    {

                    }
                }
                else
                {
                    CurrentTestResult = "Hiding 1 window";
                    //isMinimized = true;
                    try
                    {
                        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
                    }
                    catch (NullReferenceException)
                    {
                        // form isn't displayed
                    }
                }
            }
        }

        //private static void StartAndHideWindows()
        //{
        //    if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && SwitchIsPresent)
        //    {
        //        for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
        //        {
        //            if (!parameterOnOffSwitch)
        //            {
        //                CurrentTestResult = "Hiding all windows";
        //                //isMinimized = true;
        //                MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
        //            }
        //        }
        //    }
        //    else if (CheckCameraIndex(CameraIndex))
        //    {
        //        if (!parameterOnOffSwitch)
        //        {
        //            CurrentTestResult = "Hiding 1 window";
        //            //isMinimized = true;
        //            try
        //            {
        //                MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
        //            }
        //            catch (NullReferenceException)
        //            {

        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// TTTT
        /// </summary>
        private static void AllParametersCase()
        {
            switch (MethodName)
            {
                // IR Sensor
                case "h"://TTTT
                    try
                    {
                        if(WAKEUPCALL)
                        {
                            if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && MULTI_WINDOW.RecordingIsOn() == false)
                            {
                                PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(MainCamera, out bool IrSensorEnabled);
                                if (parameterOnOffSwitch)
                                {
                                    if (!IrSensorEnabled)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            PROPERTY_FUNCTIONS.Set_Human_Sensor(i, true);
                                            PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(i, true);
                                            PROPERTY_FUNCTIONS.SetCycleTime(i, parameterTime);
                                        }
                                        if (MainForm.GetMainForm != null)
                                        {
                                            MainForm.AllChangesApply();
                                        }
                                    }
                                }
                                else
                                {
                                    WrongParameter = true;
                                }

                            }
                            else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4))
                            {
                                PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(MainCamera, out bool IrSensorEnabled);
                                if (parameterOnOffSwitch)
                                {
                                    if (!IrSensorEnabled)
                                    {
                                        PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCycleTime(CameraIndex, parameterTime);
                                        if (MainForm.GetMainForm != null)
                                        {
                                            MainForm.AllChangesApply();
                                        }
                                    }
                                }
                                else
                                {
                                    WrongParameter = true;
                                }
                            }
                        }
                        else
                        {
                            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(MainCamera, out bool IrSensorEnabled);
                            if (CheckCameraIndex(CameraIndex) && CameraIndex == 8 && MULTI_WINDOW.RecordingIsOn() == false)
                            {
                                if (parameterOnOffSwitch)
                                {
                                    if (!IrSensorEnabled)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            PROPERTY_FUNCTIONS.Set_Human_Sensor(i, true);
                                            PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(i, true);
                                            PROPERTY_FUNCTIONS.SetCycleTime(i, parameterTime);
                                            if (MainForm.GetMainForm != null)
                                            {
                                                MainForm.AllChangesApply();
                                            }
                                        }
                                    }
                                }
                            }
                            else if (CheckCameraIndex(CameraIndex) && (CameraIndex >= 0 && CameraIndex < 4) && MULTI_WINDOW.formList[CameraIndex]?.recordingInProgress == false)
                            {
                                if (parameterOnOffSwitch)
                                {
                                    if (!IrSensorEnabled)
                                    {
                                        PROPERTY_FUNCTIONS.Set_Human_Sensor(CameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchDirectly(CameraIndex, true);
                                        PROPERTY_FUNCTIONS.SetCycleTime(CameraIndex, parameterTime);
                                        if (MainForm.GetMainForm != null)
                                        {
                                            MainForm.AllChangesApply();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Add(e);
                    }
                    break;

                // Face recognition
                case "d":
                    break;
            }
            CurrentTestResult = MethodName;
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

        //static bool CheckMethodsWhereCameraOmittionAllowed(string m)
        //{
        //    bool retval = false;
        //    string[] vs = new string[] {"h", "d", "v", "w", "s", "r", "e", "b", "q", "c", "l"};
        //    foreach(string i in vs)
        //    {
        //        if (m==i)
        //        {
        //            retval = true;
        //        }
        //        switch (m)
        //        {
        //            case "h":
        //                if (parameterTime > 0)
        //                {
        //                    retval = true;
        //                }
        //                break;
        //        }
        //    }
        //    return retval;
        //}

        internal static void HandleWakeUpParameters()
        {
            if (PARAM != null && PARAM.Count > 0 && !PARAM.Contains("uvccameraviewer"))
            {
                PARAM.Reverse();
                PARAM.Add("uvccameraviewer");
                PARAM.Reverse();
                WAKEUPCALL = true;
                HandleParameters(PARAM);
                if (PARAMETERS.WrongParameter)
                {
                    Application.Exit();
                }

                //if (isMinimized)
                //{
                //    if (CameraIndex >= 0 && CameraIndex < 4)
                //    {
                //        MULTI_WINDOW.formList[CameraIndex].WindowState = FormWindowState.Minimized;
                //    }
                //}
                //else
                //{
                //    if (CameraIndex >= 0 && CameraIndex < 4)
                //    {
                //        switch (CameraIndex)
                //        {
                //            case 0:
                //                if (Properties.Settings.Default.C1_full_screen)
                //                {
                //                    MULTI_WINDOW.formList[0].WindowState = FormWindowState.Maximized;
                //                }
                //                else
                //                {
                //                    MULTI_WINDOW.formList[0].WindowState = FormWindowState.Normal;
                //                }
                //                break;
                //            case 1:
                //                if (Properties.Settings.Default.C2_full_screen)
                //                {
                //                    MULTI_WINDOW.formList[1].WindowState = FormWindowState.Maximized;
                //                }
                //                else
                //                {
                //                    MULTI_WINDOW.formList[1].WindowState = FormWindowState.Normal;
                //                }
                //                break;
                //            case 2:
                //                if (Properties.Settings.Default.C3_full_screen)
                //                {
                //                    MULTI_WINDOW.formList[2].WindowState = FormWindowState.Maximized;
                //                }
                //                else
                //                {
                //                    MULTI_WINDOW.formList[2].WindowState = FormWindowState.Normal;
                //                }
                //                break;
                //            case 3:
                //                if (Properties.Settings.Default.C4_full_screen)
                //                {
                //                    MULTI_WINDOW.formList[3].WindowState = FormWindowState.Maximized;
                //                }
                //                else
                //                {
                //                    MULTI_WINDOW.formList[3].WindowState = FormWindowState.Normal;
                //                }
                //                break;
                //        }
                //    }
                //}

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
            if (ConnectedCameraCount<=cameraIndex && cameraIndex != 8 && MethodName != "n")
            {
                WrongParameter = true;
            }
            else
            {
                if ((cameraIndex == 8 || (cameraIndex >= 0 && cameraIndex < 4)))
                {
                    CameraIndex = cameraIndex;
                    retval = true;
                }
                else if (cameraIndex == 4 && MethodName == "n")
                {
                    CameraIndex = 0;
                    retval = true;
                }
            }
            
            return retval;
        }

        private static int GetNextCameraIndex(int cameraIndex)
        {
            if (cameraIndex >= 0)
            {
                if (cameraIndex >= MULTI_WINDOW.displayedCameraCount)
                {
                    return 0;
                }
                else
                {
                    return cameraIndex + 1;
                }
            }
            else
            {
                return 0;
            }
        }

    }
}
