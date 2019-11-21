using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    /// <summary>
    /// This class enables multi camera support. Revelant values taken from <see cref="Properties.Settings.Default.camera_count"/> 
    /// </summary>
    class MULTI_WINDOW
    {
        //private static CROSSBAR crossbar;
        //private static List<CROSSBAR> crosbarList = new List<CROSSBAR>();

        private static FormClass form;
        private static List<FormClass> formList = new List<FormClass>();
                
        /// <summary>
        /// MULTI-CAMERA
        /// </summary>
        /// <param name="camera_count"></param>
        public static void CreateCameraWindows(int camera_count)
        {
            if(Properties.Settings.Default.show_all_cams_simulteneously && formList.Count==0)
            {
                for (int i = 1; i < camera_count; i++)
                {
                    //WINDOW
                    form = new FormClass(i);
                    formList.Add(form);                    
                    form.Show();
                    form.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(i);                                        
                }
            }            
        }

        internal static void START_FACE_TIMERS()
        {
            for (int i=0; i<formList.Count; i++)
            {
                formList[i].faceDetector.StartFaceTimer();
            }
        }
    }
}
