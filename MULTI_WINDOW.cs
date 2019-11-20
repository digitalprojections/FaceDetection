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
        private static CROSSBAR crossbar;
        private static List<CROSSBAR> crosbarList = new List<CROSSBAR>();

        private static Form form;
        private static List<Form> formList = new List<Form>();
                
        /// <summary>
        /// MULTI-CAMERA
        /// </summary>
        /// <param name="camera_count"></param>
        public static void CreateCameraWindows(int camera_count)
        {
            if(Properties.Settings.Default.show_all_cams_simulteneously && crosbarList.Count==0)
            {
                for (int i = 1; i < camera_count; i++)
                {
                    //WINDOW
                    form = new Form();
                    formList.Add(form);
                    form.Text = "UVC Camera Viewer - camera " + (i);//counting from the second camera
                    form.Show();
                    form.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(i);
                    
                    //CROSSBAR
                    crossbar = new CROSSBAR(i, form);
                    crosbarList.Add(crossbar);
                    crossbar.Start(i, CAMERA_MODES.PREVIEW);                    
                }
            }            
        }
    }
}
