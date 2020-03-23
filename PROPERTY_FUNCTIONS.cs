using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    public static class PROPERTY_FUNCTIONS
    {
        public static bool resolution_changed { get; set; }

        public static Size GetCameraSize(int cam_ind)
        {
            Size retval;
            switch (cam_ind)
            {
                case 0:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));

                    return retval;
                case 1:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C2w), decimal.ToInt32(Properties.Settings.Default.C2h));

                    return retval;
                case 2:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C3w), decimal.ToInt32(Properties.Settings.Default.C3h));

                    return retval;
                case 3:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C4w), decimal.ToInt32(Properties.Settings.Default.C4h));

                    return retval;
                default: return new Size(640, 480);

            }
        }

        internal static void SetCycleTime(int cameraIndex, int time)
        {
            switch (cameraIndex)
            {
                case 0:
                Properties.Settings.Default.C1_check_interval = Convert.ToDecimal(time);
                    break;
                case 1:
                Properties.Settings.Default.C2_check_interval = Convert.ToDecimal(time);
                    break;
                case 2:                
                Properties.Settings.Default.C3_check_interval = Convert.ToDecimal(time);
                    break;
                case 3:
                Properties.Settings.Default.C4_check_interval = Convert.ToDecimal(time);
                    break;
            }
        }
        

        internal static void GetCaptureOperatorSwitch(int camindex, out bool captureOperatorEnabled)
        {
            switch (camindex)
            {
                case 0:
                    captureOperatorEnabled = Properties.Settings.Default.C1_enable_capture_operator;                    
                    break;
                case 1:
                    captureOperatorEnabled = Properties.Settings.Default.C2_enable_capture_operator;                    
                    break;
                case 2:
                    captureOperatorEnabled = Properties.Settings.Default.C3_enable_capture_operator;                    
                    break;
                case 3:
                    captureOperatorEnabled = Properties.Settings.Default.C4_enable_capture_operator;                    
                    break;
                default:
                    captureOperatorEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Check all sensor switches and set the operator capture for the selected camera
        /// </summary>        
        internal static void SetCaptureOperatorSwitchImplicitly(int camindex)
        {
            Get_Human_Sensor_Enabled(camindex, out bool IRSENSOR);
            GetOnOperationStartSwitch(camindex, out bool OPERSTART);
            GetFaceRecognitionSwitch(camindex, out bool FACECHECK);
            
            switch (camindex)
            {
                case 0:
                    Properties.Settings.Default.C1_enable_capture_operator = (IRSENSOR || OPERSTART || FACECHECK);
                    break;
                case 1:
                    Properties.Settings.Default.C2_enable_capture_operator = (IRSENSOR || OPERSTART || FACECHECK);
                    break;
                case 2:
                    Properties.Settings.Default.C3_enable_capture_operator = (IRSENSOR || OPERSTART || FACECHECK);
                    break;
                case 3:
                    Properties.Settings.Default.C4_enable_capture_operator = (IRSENSOR || OPERSTART || FACECHECK);
                    break;
            }
        }

        /// <summary>
        /// Set CAPTURE OPERATOR directly
        /// </summary>
        /// <param name="camindex"></param>
        internal static void SetCaptureOperatorSwitchDirectly(int camindex, bool value)
        {
            switch (camindex)
            {
                case 0:
                    Properties.Settings.Default.C1_enable_capture_operator = value;
                    break;
                case 1:
                    Properties.Settings.Default.C2_enable_capture_operator = value;
                    break;
                case 2:
                    Properties.Settings.Default.C3_enable_capture_operator = value;
                    break;
                case 3:
                    Properties.Settings.Default.C4_enable_capture_operator = value;
                    break;
            }
        }

        internal static void GetFaceRecognitionSwitch(int camindex, out bool faceRecognitionEnabled)
        {
            switch (camindex)
            {
                case 0:
                    faceRecognitionEnabled = Properties.Settings.Default.C1_enable_face_recognition;
                    break;
                case 1:
                    faceRecognitionEnabled = Properties.Settings.Default.C2_enable_face_recognition;
                    break;
                case 2:
                    faceRecognitionEnabled = Properties.Settings.Default.C3_enable_face_recognition;
                    break;
                case 3:
                    faceRecognitionEnabled = Properties.Settings.Default.C4_enable_face_recognition;
                    break;
                default:
                    faceRecognitionEnabled = false;
                    break;
            }
        }

        internal static void SetOnOperationStartSwitch(int camindex, bool recordWhenOperation)
        {
            switch (camindex)
            {
                case 0:
                    Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation = recordWhenOperation;
                    break;
                case 1:
                    Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation = recordWhenOperation;
                    break;
                case 2:
                    Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation = recordWhenOperation;
                    break;
                case 3:
                    Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation = recordWhenOperation;
                    break;
                default:
                    Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation = recordWhenOperation;
                    break;
            }
        }

        internal static void GetOnOperationStartSwitch(int camindex, out bool recordWhenOperation)
        {
            switch (camindex)
            {
                case 0:
                    recordWhenOperation = Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation;
                    break;
                case 1:
                    recordWhenOperation = Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation;
                    break;
                case 2:
                    recordWhenOperation = Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation;
                    break;
                case 3:
                    recordWhenOperation = Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation;
                    break;
                default:
                    recordWhenOperation = true;
                    break;
            }
        }

        //public static int GetCheckTimerInterval(int v)
        //{
        //    decimal retval;
        //    switch (v)
        //    {
        //        case 0:
        //            retval = Properties.Settings.Default.C1_check_interval;
        //            break;
        //        case 1:
        //            retval = Properties.Settings.Default.C2_check_interval;
        //            break;
        //        case 2:
        //            retval = Properties.Settings.Default.C3_check_interval;
        //            break;
        //        case 3:
        //            retval = Properties.Settings.Default.C4_check_interval;
        //            break;
        //        default:
        //            retval = 500;
        //            break;
        //    }
        //    return decimal.ToInt32(retval);
        //}

        /// <summary>
        /// Used by unit tests
        /// </summary>
        /// <returns></returns>
        //public static int GetMainCameraIndex()
        //{
        //    return Properties.Settings.Default.main_camera_index;
        //}

        //public static void Set_Window_Size_From_Camera_Resolution(int cam_ind)
        //{
        //    char[] vs = { 'x' };            
        //    switch (cam_ind)
        //    {
        //        case 0:

        //                Properties.Settings.Default.C1w = decimal.Parse(Properties.Settings.Default.C1res.Split(vs)[0]);
        //                Properties.Settings.Default.C1h = decimal.Parse(Properties.Settings.Default.C1res.Split(vs)[1]);                        

        //            break;
        //        case 1:

        //                Properties.Settings.Default.C2w = decimal.Parse(Properties.Settings.Default.C2res.Split(vs)[0]);
        //                Properties.Settings.Default.C2h = decimal.Parse(Properties.Settings.Default.C2res.Split(vs)[1]);

        //            break;
        //        case 2:

        //                Properties.Settings.Default.C3w = decimal.Parse(Properties.Settings.Default.C3res.Split(vs)[0]);
        //                Properties.Settings.Default.C3h = decimal.Parse(Properties.Settings.Default.C3res.Split(vs)[1]);                        

        //            break;
        //        case 3:
        //                Properties.Settings.Default.C4w = decimal.Parse(Properties.Settings.Default.C4res.Split(vs)[0]);
        //                Properties.Settings.Default.C4h = decimal.Parse(Properties.Settings.Default.C4res.Split(vs)[1]);                        

        //            break;
        //    }
        //    if (resolution_changed)
        //    {
        //        MainForm.GetMainForm.crossbar.RESTART_CAMERA();
        //        resolution_changed = false;
        //    }
        //}

        //internal static bool Get_Rec_Icon(int cam_ind)
        //{
        //    bool retval = false;
        //    switch (cam_ind)
        //    {
        //        case 0:
        //            retval = Properties.Settings.Default.show_recording_icon;
        //            break;
        //        case 1:
        //            retval = Properties.Settings.Default.C2_show_record_icon;
        //            break;
        //        case 2:
        //            retval = Properties.Settings.Default.C3_show_record_icon;
        //            break;
        //        case 3:
        //            retval = Properties.Settings.Default.C4_show_record_icon;
        //            break;
        //    }
        //    return retval;
        //}

        internal static bool CheckPreEventTimes(int cameraindex)
        {
            var retval = false;
            switch (cameraindex)
            {
                case 0:
                    if ((Properties.Settings.Default.C1_enable_event_recorder && Properties.Settings.Default.C1_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C1_enable_Human_sensor || Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C1_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 1:
                    if ((Properties.Settings.Default.C2_enable_event_recorder && Properties.Settings.Default.C2_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C2_enable_Human_sensor || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C2_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 2:
                    if ((Properties.Settings.Default.C3_enable_event_recorder && Properties.Settings.Default.C3_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C3_enable_Human_sensor || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C3_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
                case 3:
                    if ((Properties.Settings.Default.C4_enable_event_recorder && Properties.Settings.Default.C4_event_record_time_before_event > 0)
                        || ((Properties.Settings.Default.C4_enable_Human_sensor || Properties.Settings.Default.C4_enable_face_recognition || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation) && Properties.Settings.Default.C4_seconds_before_event > 0))
                    {
                        retval = true;
                    }
                    break;
            }
            return retval;
        }

        internal static bool GetShowDateTimeSwitch(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    return Properties.Settings.Default.C1_show_date_time;
                case 1:
                    return Properties.Settings.Default.C2_show_date_time;
                case 2:
                    return Properties.Settings.Default.C3_show_date_time;
                case 3:
                    return Properties.Settings.Default.C4_show_date_time;
                default:
                    return true;
            }
        }

        internal static void SetShowWindowPaneSwitch(int cameraIndex, bool v)
        {
            switch (cameraIndex)
            {
                case 0:
                    Properties.Settings.Default.C1_show_window_pane = v;
                    break;
                case 1:
                    Properties.Settings.Default.C2_show_window_pane = v;
                    break;
                case 2:
                    Properties.Settings.Default.C3_show_window_pane = v;
                    break;
                case 3:
                    Properties.Settings.Default.C4_show_window_pane = v;
                    break;
            }
        }

        internal static bool GetShowWindowPaneSwitch(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    return Properties.Settings.Default.C1_show_window_pane;
                case 1:
                    return Properties.Settings.Default.C2_show_window_pane;
                case 2:
                    return Properties.Settings.Default.C3_show_window_pane;
                case 3:
                    return Properties.Settings.Default.C4_show_window_pane;
                default:
                    return false;
            }
        }

        internal static bool GetShowCameraNumberSwitch(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    return Properties.Settings.Default.C1_show_camera_number;
                case 1:
                    return Properties.Settings.Default.C2_show_camera_number;
                case 2:
                    return Properties.Settings.Default.C3_show_camera_number;
                case 3:
                    return Properties.Settings.Default.C4_show_camera_number;
                default:
                    return true;
            }
        }

        internal static bool GetRecordingIconSwitch(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    return Properties.Settings.Default.C1_show_record_icon;
                case 1:
                    return Properties.Settings.Default.C2_show_record_icon;
                case 2:
                    return Properties.Settings.Default.C3_show_record_icon;
                case 3:
                    return Properties.Settings.Default.C4_show_record_icon;
                default:
                    return true;
            }
        }

        //internal static void Set_Rec_Icon(int cam_ind, bool val)
        //{            
        //    switch (cam_ind)
        //    {
        //        case 0:
        //            Properties.Settings.Default.C1_show_record_icon = val;
        //            break;
        //        case 1:
        //            Properties.Settings.Default.C2_show_record_icon = val;
        //            break;
        //        case 2:
        //            Properties.Settings.Default.C3_show_record_icon = val;
        //            break;
        //        case 3:
        //            Properties.Settings.Default.C4_show_record_icon = val;
        //            break;
        //    }
        //}

        internal static Point Get_Window_Location(int cam_ind)
        {
            Point retval;
            switch (cam_ind)
            {
                case 0:
                    retval = Properties.Settings.Default.C1_window_location;
                    return retval;
                case 1:
                    retval = Properties.Settings.Default.C2_window_location;
                    return retval;
                case 2:
                    retval = Properties.Settings.Default.C3_window_location;
                    return retval;
                case 3:
                    retval = Properties.Settings.Default.C4_window_location;
                    return retval;
                default: return new Point(16, 16);
            }
        }

        internal static void Set_Window_Location(int cam_ind, CameraForm subCamWindow)
        {
            if (subCamWindow.Location.X >=0 && subCamWindow.Location.Y>=0)
            {
                switch (cam_ind)
                {
                    case 0:
                        Properties.Settings.Default.C1_window_location = subCamWindow.Location;
                        Properties.Settings.Default.C1x = subCamWindow.Location.X;
                        Properties.Settings.Default.C1y = subCamWindow.Location.Y;
                        break;
                    case 1:
                        Properties.Settings.Default.C2_window_location = subCamWindow.Location;
                        Properties.Settings.Default.C2x = subCamWindow.Location.X;
                        Properties.Settings.Default.C2y = subCamWindow.Location.Y;
                        break;
                    case 2:
                        Properties.Settings.Default.C3_window_location = subCamWindow.Location;
                        Properties.Settings.Default.C3x = subCamWindow.Location.X;
                        Properties.Settings.Default.C3y = subCamWindow.Location.Y;
                        break;
                    case 3:
                        Properties.Settings.Default.C4_window_location = subCamWindow.Location;
                        Properties.Settings.Default.C4x = subCamWindow.Location.X;
                        Properties.Settings.Default.C4y = subCamWindow.Location.Y;
                        break;
                }
            }            
        }

        internal static void Set_Window_Location_Set(int cam_ind, CameraForm subCamWindow)
        {
            if (subCamWindow.Location.X >= 0 && subCamWindow.Location.Y >= 0)
            {
                switch (cam_ind)
                {
                    case 0:
                        subCamWindow.Location = new Point(Decimal.ToInt32(Properties.Settings.Default.C1x), Decimal.ToInt32(Properties.Settings.Default.C1y));
                        Properties.Settings.Default.C1_window_location = subCamWindow.Location;
                        break;
                    case 1:
                        subCamWindow.Location = new Point(Decimal.ToInt32(Properties.Settings.Default.C2x), Decimal.ToInt32(Properties.Settings.Default.C2y));
                        Properties.Settings.Default.C2_window_location = subCamWindow.Location;
                        break;
                    case 2:
                        subCamWindow.Location = new Point(Decimal.ToInt32(Properties.Settings.Default.C3x), Decimal.ToInt32(Properties.Settings.Default.C3y));
                        Properties.Settings.Default.C3_window_location = subCamWindow.Location;
                        break;
                    case 3:
                        subCamWindow.Location = new Point(Decimal.ToInt32(Properties.Settings.Default.C4x), Decimal.ToInt32(Properties.Settings.Default.C4y));
                        Properties.Settings.Default.C4_window_location = subCamWindow.Location;
                        break;
                }
            }
        }

        internal static bool CheckOnTopByIndex(int camera_index)
        {
            switch (camera_index)
            {
                case 0:
                    return Properties.Settings.Default.C1_window_on_top;
                case 1:
                    return Properties.Settings.Default.C2_window_on_top;
                case 2:
                    return Properties.Settings.Default.C3_window_on_top;
                case 3:
                    return Properties.Settings.Default.C4_window_on_top;
                default:
                    return true;
            }
        }

        internal static bool CheckFullScreenByIndex(int camera_index)
        {
            switch (camera_index)
            {
                case 0:
                    return Properties.Settings.Default.C1_full_screen;
                case 1:
                    return Properties.Settings.Default.C2_full_screen;
                case 2:
                    return Properties.Settings.Default.C3_full_screen;
                case 3:
                    return Properties.Settings.Default.C4_full_screen;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Settings UI has variable window size values stored as per camera index.
        /// You can get those values by camera index
        /// </summary>
        /// <param name="cam_ind"></param>
        /// <returns></returns>
        public static Size Get_Camera_Window_Size(int cam_ind)
        {
            Size size = new Size(640,480);
            switch (cam_ind)
            {
                case 0:
                    size = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));
                    break;
                case 1:
                    size = new Size(decimal.ToInt32(Properties.Settings.Default.C2w), decimal.ToInt32(Properties.Settings.Default.C2h));
                    break;
                case 2:
                    size = new Size(decimal.ToInt32(Properties.Settings.Default.C3w), decimal.ToInt32(Properties.Settings.Default.C3h));
                    break;
                case 3:
                    size = new Size(decimal.ToInt32(Properties.Settings.Default.C4w), decimal.ToInt32(Properties.Settings.Default.C4h));
                    break;
            }
            return size;
        }

        /// <summary>
        /// Set individual window sizes for each camera
        /// </summary>
        /// <param name="cam_ind"></param>
        /// <returns></returns>
        public static void Set_Camera_Window_Size(int cam_ind, Form form)
        {
            switch (cam_ind)
            {
                case 0:
                    Properties.Settings.Default.C1w = Convert.ToDecimal(form.Width);
                    Properties.Settings.Default.C1h = Convert.ToDecimal(form.Height);
                    break;
                case 1:
                    Properties.Settings.Default.C2w = Convert.ToDecimal(form.Width);
                    Properties.Settings.Default.C2h = Convert.ToDecimal(form.Height);
                    break;
                case 2:
                    Properties.Settings.Default.C3w = Convert.ToDecimal(form.Width);
                    Properties.Settings.Default.C3h = Convert.ToDecimal(form.Height);
                    break;
                case 3:
                    Properties.Settings.Default.C4w = Convert.ToDecimal(form.Width);
                    Properties.Settings.Default.C4h = Convert.ToDecimal(form.Height);
                    break;
            }
        }

        internal static void GetPreAndPostEventTimes(int cameraIndex, out int timeBeforeEvent, out int timeAfterEvent)
        {
            switch (cameraIndex)
            {
                case 0:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_after_event);

                    break;
                case 1:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_after_event);

                    break;
                case 2:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_after_event);

                    break;
                case 3:
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_after_event);
                    break;
                default: //default is set to 0 simply to satisfy the function requirement. This point is never hit
                    timeBeforeEvent = 0;
                    timeAfterEvent = 0;
                    break;
            }
        }

        internal static void Set_Human_Sensor(int cameraIndex, bool v)
        {
            if (cameraIndex == 0)
            {
                Properties.Settings.Default.C1_enable_Human_sensor = v;
            }
            else if (cameraIndex == 1)
            {
                Properties.Settings.Default.C2_enable_Human_sensor = v;
            }
            else if (cameraIndex == 2)
            {
                Properties.Settings.Default.C3_enable_Human_sensor = v;
            }
            else if (cameraIndex == 3)
            {
                Properties.Settings.Default.C4_enable_Human_sensor = v;
            }
            SetCaptureOperatorSwitchImplicitly(cameraIndex);
        }

        internal static void Set_Face_Switch(int cameraIndex, bool faceSwitch)
        {
            if (cameraIndex == 0)
            {
                Properties.Settings.Default.C1_enable_face_recognition = faceSwitch;
            }
            else if (cameraIndex == 1)
            {
                Properties.Settings.Default.C2_enable_face_recognition = faceSwitch;
            }
            else if (cameraIndex == 2)
            {
                Properties.Settings.Default.C3_enable_face_recognition = faceSwitch;
            }
            else if (cameraIndex == 3)
            {
                Properties.Settings.Default.C4_enable_face_recognition = faceSwitch;
            }

            SetCaptureOperatorSwitchImplicitly(cameraIndex);
        }

        internal static void GetSecondsBeforeEvent(int cameraIndex, out int secondBeforeOperationEvent)
        {
            switch (cameraIndex)
            {
                case 0:
                    secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                    break;
                case 1:
                    secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
                    break;
                case 2:
                    secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
                    break;
                case 3:
                    secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
                    break;
                default:
                    secondBeforeOperationEvent = 300;
                    break;
            }
        }

        internal static void GetSecondsAfterEvent(int cameraIndex, out int secondsAfterEvent)
        {
            switch (cameraIndex)
            {
                case 0:                
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                    break;
                case 1:
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
                    break;
                case 2:
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
                    break;
                case 3:
                secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
                    break;
                default:
                    secondsAfterEvent = 300;
                    break;
            }
        }

        internal static void GetEventRecorderSwitch(int cameraIndex, out bool eventRecorderEnabled)
        {
            switch (cameraIndex)
            {
                case 0:
                    eventRecorderEnabled = Properties.Settings.Default.C1_enable_event_recorder;
                    break;
                case 1:
                    eventRecorderEnabled = Properties.Settings.Default.C2_enable_event_recorder;
                    break;
                case 2:
                    eventRecorderEnabled = Properties.Settings.Default.C3_enable_event_recorder;
                    break;
                case 3:
                    eventRecorderEnabled = Properties.Settings.Default.C4_enable_event_recorder;
                    break;
                default:
                    eventRecorderEnabled = true;
                    break;
            }
        }

        internal static void GetCaptureMethod(int cameraIndex, out string captureMethod)
        {
            switch (cameraIndex)
            {
                case 0:
                    captureMethod = Properties.Settings.Default.C1_capture_type;
                    break;
                case 1:
                    captureMethod = Properties.Settings.Default.C2_capture_type;
                    break;
                case 2:
                    captureMethod = Properties.Settings.Default.C3_capture_type;
                    break;
                case 3:
                    captureMethod = Properties.Settings.Default.C4_capture_type;
                    break;
                default:
                    captureMethod = "Video";
                    break;
            }
        }

        internal static void GetReInitiationInterval(int cameraIndex, out int intervalBeforeReinitiating)
        {
            switch (cameraIndex)
            {
                case 0:
                    intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording);
                    break;
                case 1:
                    intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording);
                    break;
                case 2:
                    intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording);
                    break;
                case 3:
                    intervalBeforeReinitiating = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording);
                    break;
                default:
                    intervalBeforeReinitiating = 60;
                    break;
            }
        }

        internal static void GetSensorCheckInterval(int cameraIndex, out int checkInterval)
        {
            switch (cameraIndex)
            {
                case 0:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C1_check_interval);
                    break;
                case 1:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C2_check_interval);
                    break;
                case 2:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C3_check_interval);
                    break;
                case 3:
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C4_check_interval);
                    break;
                default:
                    checkInterval = 500;
                    break;
            }
        }
        
        public static string GetResolution(int cameraIndex)
        {
            string res = "";
            switch (cameraIndex)
            {
                case 0:
                    res = Properties.Settings.Default.C1res;
                    break;
                case 1:
                    res = Properties.Settings.Default.C2res;
                    break;
                case 2:
                    res = Properties.Settings.Default.C3res;
                    break;
                case 3:
                    res = Properties.Settings.Default.C4res;
                    break;
            }
            return res;
        }
        
        public static void SetResolution(int cameraIndex, string val)
        {
            switch (cameraIndex)
            {
                case 0:
                    Properties.Settings.Default.C1res = val;
                    break;
                case 1:
                    Properties.Settings.Default.C2res = val;
                    break;
                case 2:
                    Properties.Settings.Default.C3res = val;
                    break;
                case 3:
                    Properties.Settings.Default.C4res = val;
                    break;
            }
            Properties.Settings.Default.Save();
        }

        public static System.Drawing.Size Get_Stored_Resolution(int cameraIndex)
        {
            System.Drawing.Size retval;
            string[] res;
            switch (cameraIndex)
            {
                case 0:
                    res = Properties.Settings.Default.C1res.Split('x');                    
                    break;
                case 1:
                    res = Properties.Settings.Default.C2res.Split('x');
                    break;
                case 2:
                    res = Properties.Settings.Default.C3res.Split('x');
                    break;
                case 3:
                    res = Properties.Settings.Default.C4res.Split('x');
                    break;
                default:
                    res = Properties.Settings.Default.C1res.Split('x');
                    //retval = new System.Drawing.Size(640, 480);
                    break;
            }
            try
            {
                retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
            }
            catch (ArgumentNullException anx)
            {
                retval = new System.Drawing.Size(640, 480);
            }
            catch (FormatException fx)
            {
                retval = new System.Drawing.Size(640, 480);
            }
            catch (OverflowException ofx)
            {
                retval = new System.Drawing.Size(640, 480);
            }
            return retval;
        }

        internal static void SetFPS(int cameraIndex, string v)
        {
            switch (cameraIndex)
            {
                case 0:
                    Properties.Settings.Default.C1f = v;
                    break;
                case 1:
                    Properties.Settings.Default.C2f = v;
                    break;
                case 2:
                    Properties.Settings.Default.C3f = v;
                    break;
                case 3:
                    Properties.Settings.Default.C4f = v;
                    break;
            }
        }

        public static string GetFPS(int cameraIndex)
        {
            string fps = "15";
            switch (cameraIndex)
            {
                case 0:
                    fps = Properties.Settings.Default.C1f;
                    break;
                case 1:
                    fps = Properties.Settings.Default.C2f;
                    break;
                case 2:
                    fps = Properties.Settings.Default.C3f;
                    break;
                case 3:
                    fps = Properties.Settings.Default.C4f;
                    break;
            }
            return fps;
        }

        /// <summary>
        /// Is human sensor switch ON for the selected/MAIN camera?
        /// </summary>
        /// <param name="cameraIndex"></param>
        /// <returns></returns>
        public static void Get_Human_Sensor_Enabled(int cameraIndex, out bool iRSensorEnabled)
        {
            switch (cameraIndex)
            {
                case 0:
                    iRSensorEnabled = Properties.Settings.Default.C1_enable_Human_sensor;
                    break;
                case 1:
                    iRSensorEnabled = Properties.Settings.Default.C2_enable_Human_sensor;
                    break;
                case 2:
                    iRSensorEnabled = Properties.Settings.Default.C3_enable_Human_sensor;
                    break;
                case 3:
                    iRSensorEnabled = Properties.Settings.Default.C4_enable_Human_sensor;
                    break;
                default:
                    iRSensorEnabled = false;
                    break;
            }
        }


        public static void DisableOperatorCaptureCheckBoxIfNeeded()
        {
            // CAMERA 1
            if (Properties.Settings.Default.C1_enable_Human_sensor || Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C1_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C1_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 2
            if (Properties.Settings.Default.C2_enable_Human_sensor || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C2_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C2_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 3
            if (Properties.Settings.Default.C3_enable_Human_sensor || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C3_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C3_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 4
            if (Properties.Settings.Default.C4_enable_Human_sensor || Properties.Settings.Default.C4_enable_face_recognition || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C4_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C4_enable_capture_operator = false; //All three are off. Disable
            }
        }
    }
}
