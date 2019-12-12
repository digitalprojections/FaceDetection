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
        //private static CROSSBAR crossbar;
        //private static List<CROSSBAR> crossbarList = new List<CROSSBAR>();

        private static FormClass form;
        public static FormClass[] formList = new FormClass[3];
        public static int subCameraHasBeenDisplayed = 0;
                
        public static void CreateCameraWindows(int numberCameraToDisplay, int cam_index)
        {
            if (subCameraHasBeenDisplayed < (numberCameraToDisplay - 1)) 
            {
                for(int i = 1; i < subCameraHasBeenDisplayed + 1; i++)
                {
                    if (formList[i-1].Text == "")
                    {
                        form = new FormClass(i);
                        form.Text = "UVC Camera Viewer - camera " + (i + 1);
                        form.Show();
                        if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }
                    }
                }

                for (int i = subCameraHasBeenDisplayed + 1; i < numberCameraToDisplay; i++)
                {
                    form = new FormClass(i);
                    formList[i-1] = form;
                    form.Text = "UVC Camera Viewer - camera " + (i + 1); //counting from the second camera
                    form.Show();
                    subCameraHasBeenDisplayed ++;
                    if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                    {
                        form.WindowState = FormWindowState.Minimized;
                    }
                }
            }
            else if ((numberCameraToDisplay - 1) < subCameraHasBeenDisplayed)
            {
                for (int i = subCameraHasBeenDisplayed; i >= numberCameraToDisplay; i--)
                {
                    formList[i-1].Close();
                    subCameraHasBeenDisplayed--;
                }
            }
            else
            {
                for (int i = 1; i < numberCameraToDisplay; i++)
                {
                    if(formList[i-1].Text == "")
                    {
                        form = new FormClass(i);
                        form.Text = "UVC Camera Viewer - camera " + (i + 1);
                        formList[i-1] = form;
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
            for (int i = 0; i < subCameraHasBeenDisplayed; i++)
            {
                if (i == 0)
                {
                    formList[0].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C2x), decimal.ToInt32(Properties.Settings.Default.C2y));
                    formList[0].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(1);
                }
                else if (i == 1)
                {
                    formList[1].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C3x), decimal.ToInt32(Properties.Settings.Default.C3y));
                    formList[1].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(2);
                }
                else if (i == 2)
                {
                    formList[2].Location = new Point(decimal.ToInt32(Properties.Settings.Default.C4x), decimal.ToInt32(Properties.Settings.Default.C4y));
                    formList[2].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(3);
                }
            }
        }
    }
}
