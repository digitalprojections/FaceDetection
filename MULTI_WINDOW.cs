using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static void formSettingsChanged()
        {
            for (int i = 0; i < formList.Count; i++)
            {
                if (i == 0)
                {
                    formList[0].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C2x), decimal.ToInt32(Properties.Settings.Default.C2y));
                    formList[0].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(1);
                }
                else if (i == 1)
                {
                    formList[1].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C3x), decimal.ToInt32(Properties.Settings.Default.C3y));
                    formList[0].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(2);
                }
                else if (i == 2)
                {
                    formList[2].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C4x), decimal.ToInt32(Properties.Settings.Default.C4y));
                    formList[0].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(3);
                }
            }
        }
    }
}
