using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    class PROPERTY_FUNCTIONS
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
        internal static Point Get_Camera_Window_Location(int cam_ind)
        {
            Point retval;
            switch (cam_ind)
            {
                case 0:
                    retval = Properties.Settings.Default.window_location;
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
                    captureOperatorEnabled = Properties.Settings.Default.C1_enable_capture_operator;
                    break;
            }
        }
        /// <summary>
        /// Check all sensor switches and set the operator capture for the selected camera
        /// </summary>        
        internal static void SetCaptureOperatorSwitchImplicitly(int camindex)
        {
            Get_Human_Sensor_Enabled(camindex, out bool IRSENSOR);
            GetCaptureOnOperationStartSwitch(camindex, out bool OPERSTART);
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
            Properties.Settings.Default.Save();
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
            Properties.Settings.Default.Save();
        }

        internal static void GetReInitiationInterval(int cameraindex, out int intervalBeforeReinitiating)
        {
            switch (cameraindex)
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
                    intervalBeforeReinitiating = 0;
                    break;
            }
        }

        internal static void GetCaptureOnOperationStartSwitch(int camindex, out bool recordWhenOperation)
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
                    recordWhenOperation = Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation;
                    break;
            }
        }

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
        internal static bool Get_Rec_Icon(int cam_ind)
        {
            bool retval = false;
            switch (cam_ind)
            {
                case 0:
                    retval = Properties.Settings.Default.show_recording_icon;
                    break;
                case 1:
                    retval = Properties.Settings.Default.C2_show_record_icon;
                    break;
                case 2:
                    retval = Properties.Settings.Default.C3_show_record_icon;
                    break;
                case 3:
                    retval = Properties.Settings.Default.C4_show_record_icon;
                    break;
            }
            return retval;
        }

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

        internal static void Set_Rec_Icon(int cam_ind, bool val)
        {            
            switch (cam_ind)
            {
                case 0:
                    Properties.Settings.Default.show_recording_icon = val;
                    break;
                case 1:
                    Properties.Settings.Default.C2_show_record_icon = val;
                    break;
                case 2:
                    Properties.Settings.Default.C3_show_record_icon = val;
                    break;
                case 3:
                    Properties.Settings.Default.C4_show_record_icon = val;
                    break;
            }
        }

        internal static void Set_Window_Location(int cam_ind, CameraForm subCamWindow)
        {
            switch (cam_ind)
            {
                case 0:
                    Properties.Settings.Default.window_location = subCamWindow.Location;
                    break;
                case 1:
                    Properties.Settings.Default.C2_window_location = subCamWindow.Location;
                    break;
                case 2:
                    Properties.Settings.Default.C3_window_location = subCamWindow.Location;
                    break;
                case 3:
                    Properties.Settings.Default.C4_window_location = subCamWindow.Location;
                    break;
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
            Size size = new Size(100,100);
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
                default: //default is set to 0 index simply to satisfy the function requirement. This point is never hit
                    timeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_before_event);
                    timeAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_event_record_time_after_event);
                    break;
            }
        }

        internal static void Set_Human_Sensor(int cameraIndex, bool v)
        {
            if (cameraIndex == 0)
            {
                Properties.Settings.Default.C1_enable_Human_sensor = v;
            }
            //IS THIS PART EVENT VALID??? THERE IS ONLY 1 HUMAN SENSOR
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

        internal static void GetSecondsAfterEvent(int index, out int secondsAfterEvent)
        {
            switch (index)
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
                    secondsAfterEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                    break;
            }

        }

        internal static void GetEventRecorderSwitch(int index, out bool eventRecorderEnabled)
        {
            switch (index)
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
                    eventRecorderEnabled = Properties.Settings.Default.C1_enable_event_recorder;
                    break;
            }
        }
        internal static void GetCaptureMethod(int camindex, out string captureMethod)
        {
            switch (camindex)
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

        internal static void GetInterval(int camindex, out int checkInterval)
        {
            switch (camindex)
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
                    checkInterval = decimal.ToInt32(Properties.Settings.Default.C1_check_interval);
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
                    faceRecognitionEnabled = Properties.Settings.Default.C1_enable_face_recognition;
                    break;
            }
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

        public static System.Drawing.Size Get_Stored_Resolution(int cam_ind)
        {
            System.Drawing.Size retval;
            string[] res;
            switch (cam_ind)
            {
                case 0:
                    res = Properties.Settings.Default.C1res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 1:
                    res = Properties.Settings.Default.C2res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 2:
                    res = Properties.Settings.Default.C3res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 3:
                    res = Properties.Settings.Default.C4res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                default:
                    retval = new System.Drawing.Size(640, 480);
                    break;
            }
            return retval;
        }

        internal static void GetSecondsBeforeEvent(int camsen, out int secondBeforeOperationEvent)
        {

            switch (camsen)
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
                    secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                    break;
            }
        }

        public static int Get_FPS(int cam_ind)
        {
            int fps = 15;
            switch (cam_ind)
            {
                case 0:
                    fps = Int32.Parse(Properties.Settings.Default.C1f);
                    break;
                case 1:
                    fps = Int32.Parse(Properties.Settings.Default.C2f);
                    break;
                case 2:
                    fps = Int32.Parse(Properties.Settings.Default.C3f);
                    break;
                case 3:
                    fps = Int32.Parse(Properties.Settings.Default.C4f);
                    break;
            }
            return fps;
        }        
        /// <summary>
        /// Is human sensor switch ON for the selected/MAIN camera?
        /// </summary>
        /// <param name="cam_ind"></param>
        /// <returns></returns>
            public static void Get_Human_Sensor_Enabled(int cam_ind, out bool iRSensorEnabled)
        {
            switch (cam_ind)
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
    }
}
