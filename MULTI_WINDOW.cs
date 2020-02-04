using System;
using System.Windows.Forms;

namespace FaceDetection
{
    /// <summary>
    /// This class enables multi camera support. Revelant values taken from <see cref="Properties.Settings.Default.camera_count"/> 
    /// </summary>
    class MULTI_WINDOW
    {
        private static CameraForm form;
        public static CameraForm[] formList = new CameraForm[4];
        public static int displayedCameraCount = 0;
        //public static bool[] formArray = new bool[4];

        /// <summary>
        /// by passing two important parameters.
        /// </summary>
        public static void CreateCameraWindows()
        {
            int numberOfCamerasToDisplay = decimal.ToInt32(Properties.Settings.Default.camera_count);            

            if (displayedCameraCount < numberOfCamerasToDisplay) 
            {
                for(int i = 0; i < displayedCameraCount; i++)
                {
                    if (formList[i]?.DISPLAYED==false)
                    {
                        form = new CameraForm(i);
                        formList[i] = form;
                        form.Show();                        
                        displayedCameraCount++;                        
                    }
                }

                for (int i = displayedCameraCount; i < numberOfCamerasToDisplay; i++)
                {
                    form = new CameraForm(i);
                    formList[i] = form;                    
                    form.Show();
                    displayedCameraCount ++;                    
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
                        //displayedCameraCount--; //DONE within the form
                    }
                }
                catch (NullReferenceException nrx)
                {
                    LOGGER.Add(Resource.form_closing_failure + " " + nrx.Message);
                }
            }            
        }

        public static void GetVideoFormatByCamera(int cameraIndex)
        {
            formList[cameraIndex]?.GetVideoFormat();
        }

        public static void FormSettingsChanged()
        {
            for (int i = 0; i < displayedCameraCount; i++)
            {
                formList[i].SetWindowProperties();
                formList[i].MainCameraDisplay(i);
                //Also must check if the PREEVENT mode is needed
                //formList[i].SetCameraToDefaultMode();
            }
        }

        public static void EventRecorderOn(int cameraIndex)
        {
            PROPERTY_FUNCTIONS.GetPreAndPostEventTimes(cameraIndex, out int timeBeforeEvent, out int timeAfterEvent);
            bool preeventRecording = PreeventRecordingState(cameraIndex);

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
                formList[cameraIndex].SetHideRecordIcon(timeAfterEvent);
                LOGGER.Add(Resource.event_recording_starts);
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

        public static bool PreeventRecordingState(int cameraIndex)
        {   
            return formList[cameraIndex].crossbar.PREEVENT_RECORDING;
        }

        internal static void SET_REC_ICON(int cameraIndex)
        {
            formList[cameraIndex].SET_REC_ICON();            
        }

        //internal static void SetToPreviewMode(int camind)
        //{            
        //    formList[camind].SetToPreviewMode();            
        //}

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

        //internal static void EventRecorderOnOFFAll(bool status)
        //{
        //    for(int i = 0; i< formList.Length; i++)
        //    {
        //        if (!String.IsNullOrEmpty(formList[i].Text)) // Form is closed
        //        {
        //            if (formList[i].crossbar.GetRecordingState())
        //            {
        //                if (status)
        //                {
        //                    EventRecorderOn(i);
        //                }
        //                else
        //                {
        //                    EventRecorderOff(i);
        //                }                        
        //            }
        //        }
        //    }
        //}
    }
}
