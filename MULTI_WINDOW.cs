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
                
        /// <summary>
        /// by passing two important parameters.
        /// </summary>
        /// <param name="numberOfCamerasToDisplay"></param>
        /// <param name="cam_index"></param>
        public static void CreateCameraWindows(int numberOfCamerasToDisplay, int cam_index)
        {
            if (displayedCameraCount < (numberOfCamerasToDisplay)) 
            {
                for(int i = 1; i < displayedCameraCount; i++)
                {
                    if (formList[i].Text == "")
                    {
                        form = new CameraForm(i);                        
                        form.Show();
                        if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }
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
            else if ((numberOfCamerasToDisplay) < displayedCameraCount)
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
            else
            {
                for (int i = 1; i < numberOfCamerasToDisplay; i++)
                {
                    if(formList[i].Text == "")
                    {
                        form = new CameraForm(i);
                        //form.Text = "UVC Camera Viewer - camera " + (i + 1);
                        formList[i] = form;
                        form.Show();
                        if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }
                    }
                }
            }
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
                if (formList[i].crossbar.GetRecordingState())
                {
                    recmodeison = true;
                }
            }

            return recmodeison;
        }
    }
}
