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
        private static CameraForm form;
        public static CameraForm[] formList = new CameraForm[4];
        public static int subCameraHasBeenDisplayed = 0;
                
        public static void CreateCameraWindows(int numberCameraToDisplay, int cam_index)
        {
            if (subCameraHasBeenDisplayed < (numberCameraToDisplay)) 
            {
                for(int i = 1; i < subCameraHasBeenDisplayed; i++)
                {
                    if (formList[i].Text == "")
                    {
                        form = new CameraForm(i);
                        //form.Text = "UVC Camera Viewer - camera " + (i + 1);
                        form.Show();
                        if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }
                    }
                }

                for (int i = subCameraHasBeenDisplayed; i < numberCameraToDisplay; i++)
                {
                    form = new CameraForm(i);
                    formList[i] = form;
                    //form.Text = "UVC Camera Viewer - camera " + (i + 1); //counting from the second camera
                    form.Show();
                    subCameraHasBeenDisplayed ++;
                    if (!Properties.Settings.Default.show_all_cams_simulteneously && (i != cam_index))
                    {
                        form.WindowState = FormWindowState.Minimized;
                    }
                }
            }
            else if ((numberCameraToDisplay) < subCameraHasBeenDisplayed)
            {
                try
                {
                for (int i = subCameraHasBeenDisplayed; i >= numberCameraToDisplay; i--)
                {
                    formList[i].closeFromSettings = true;
                    formList[i].Close();
                    subCameraHasBeenDisplayed--;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Main camera has been disconnected while application was not running");
                }
            }
            else
            {
                for (int i = 1; i < numberCameraToDisplay; i++)
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
