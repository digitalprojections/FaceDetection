using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    /// <summary>
    /// This class enables multi camera support. Revelant values taken from <see cref="Properties.Settings.Default.camera_count"/> 
    /// </summary>
    class MULTI_WINDOW
    {
        //private delegate void dDateTimerUpdater();
        
        private static CameraForm form;
        public static CameraForm[] formList = new CameraForm[4];
        public static int displayedCameraCount = 0;
        public static bool[] formArray = new bool[3];

        /// <summary>
        /// by passing two important parameters.
        /// </summary>
        public static void CreateCameraWindows()
        {
            int numberOfCamerasToDisplay = decimal.ToInt32(Properties.Settings.Default.camera_count);
            int cam_index = decimal.ToInt32(Properties.Settings.Default.main_camera_index);

            if (displayedCameraCount < numberOfCamerasToDisplay) 
            {
                for(int i = 0; i < displayedCameraCount; i++)
                {
                    if (formArray[i] == false)
                    {
                        form = new CameraForm(i);                        
                        form.Show();
                        if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }
                        displayedCameraCount++;
                    }
                }

                for (int i = displayedCameraCount; i < numberOfCamerasToDisplay; i++)
                {
                    form = new CameraForm(i);
                    formList[i] = form;                    
                    form.Show();
                    displayedCameraCount ++;
                    if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                    {
                        form.WindowState = FormWindowState.Minimized;
                    }
                }
            }
            else
            {
                try
                {
                    for (int i = displayedCameraCount - 1; i >= numberOfCamerasToDisplay; i--)
                    {
                        //formList[i].closeFromSettings = true;
                        formList[i].Close();
                        formList[i] = null;
                        //displayedCameraCount--;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Main camera has been disconnected while application was not running");
                }
            }
            //else
            //{
            //    for (int i = 1; i < numberOfCamerasToDisplay; i++)
            //    {
            //        if(formList[i].Text == "")
            //        {
            //            form = new CameraForm(i);
            //            //form.Text = "UVC Camera Viewer - camera " + (i + 1);
            //            formList[i] = form;
            //            form.Show();
            //            if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
            //            {
            //                form.WindowState = FormWindowState.Minimized;
            //            }
            //        }
            //    }
            //}
        }

        public static void GetVideoFormatByCamera(int ind)
        {
            formList[ind].GetVideoFormat();
        }

        public static void formSettingsChanged()
        {
            for (int i = 0; i < displayedCameraCount; i++)
            {
                
                formList[i].SetWindowProperties();
                //Also must check if the PREEVENT mode is needed
                formList[i].SetCameraToDefaultMode();

                //Commented out because, each form can do it by itself

                //if (i == 0)
                //{
                //    formList[0].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C2x), decimal.ToInt32(Properties.Settings.Default.C2y));
                //    formList[0].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(1);
                //}
                //else if (i == 1)
                //{
                //    formList[1].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C3x), decimal.ToInt32(Properties.Settings.Default.C3y));
                //    formList[1].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(2);
                //}
                //else if (i == 2)
                //{
                //    formList[2].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C4x), decimal.ToInt32(Properties.Settings.Default.C4y));
                //    formList[2].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(3);
                //}
            }
        }
        public static void EventRecorderOn(int cameraIndex)
        {
            int timeBeforeEvent = 0, timeAfterEvent = 0;
            bool preeventRecording = false;

            PARAMETERS.PARAM.Clear();

            PROPERTY_FUNCTIONS.GetPreAndPostEventTimes(cameraIndex, out timeBeforeEvent, out timeAfterEvent);

            preeventRecording = PreeventRecordingState(cameraIndex);

            if (preeventRecording)
            {
                if (Properties.Settings.Default.capture_method == 0)
                {
                    TaskManager.EventAppeared(RECORD_PATH.EVENT, cameraIndex + 1, timeBeforeEvent, timeAfterEvent, DateTime.Now);

                    SET_REC_ICON(cameraIndex);
                    formList[cameraIndex].SetRecordIcon(cameraIndex, timeAfterEvent);
                }
            }
            else
            {
                formList[cameraIndex].crossbar?.Start(cameraIndex, CAMERA_MODES.EVENT);
                Logger.Add("EVENT RECORDING STARTS (console call using parameters)");
            }
        }
        internal static void EventRecorderOff(int cameraIndex)
        {   
            var preeventRecording = MULTI_WINDOW.PreeventRecordingState(cameraIndex);
            
            //SET it within each crossbar?
            if (!preeventRecording)
            {
                formList[cameraIndex].crossbar?.Start(cameraIndex, CAMERA_MODES.PREVIEW);
            }
        }

        public static bool PreeventRecordingState(int cam_index)
        {   
            return formList[cam_index].crossbar.PREEVENT_RECORDING;
        }

        internal static void SET_REC_ICON(int cameraIndex)
        {
            formList[cameraIndex].SET_REC_ICON();            
        }

        internal static void SetToPreviewMode(int camind)
        {            
            formList[camind].SetToPreviewMode();            
        }

        public static bool RecordingIsOn()
        {
            var recmodeison = false;

            for (int i = 0; i < displayedCameraCount; i++)
            {
                if (!String.IsNullOrEmpty(formList[i].Text)) // Form is closed
                {
                    if (formList[i].crossbar.GetRecordingState())
                    {
                        recmodeison = true;
                    }
                }
            }

            return recmodeison;
        }
    }
}
