using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FaceDetection
{
    /// <summary>
    /// This class enables multi camera support. Revelant values taken from <see cref="Properties.Settings.Default.camera_count"/> 
    /// </summary>
    class MULTI_WINDOW
    {
        //private static CROSSBAR crossbar;
        //private static List<CROSSBAR> crossbarList = new List<CROSSBAR>();

        private static Form form;
        public static List<Form> formList = new List<Form>();
                
        public static void CreateCameraWindows(int numberCameraToDisplay)
        {
            if (Properties.Settings.Default.show_all_cams_simulteneously && formList.Count < (numberCameraToDisplay - 1)) 
            {
                for (int i = 1; i < numberCameraToDisplay; i++)
                {
                    form = new FormClass(i);
                    formList.Add(form);
                    form.Text = "UVC Camera Viewer - camera " + (i + 1); //counting from the second camera
                    form.Show();
                }
            }
            else if (Properties.Settings.Default.show_all_cams_simulteneously == false)
            {
                foreach (Form form in formList)
                {
                    form.Close();
                }
                formList.Clear();
            }
            else if ((numberCameraToDisplay - 1) < formList.Count)
            {
                for (int i = formList.Count; i >= numberCameraToDisplay; i--)
                {
                    formList[i-1].Close();
                }
            }
        }
    }
}
