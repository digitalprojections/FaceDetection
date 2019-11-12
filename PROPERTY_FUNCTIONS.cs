using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection
{
    class PROPERTY_FUNCTIONS
    {
        public static Size GetWidth(int cam_ind)
        {
            Size retval;
            switch (cam_ind)
            {
                case 0:
                    retval = Properties.Settings.Default.cam1_size;

                    return retval;
                case 1:
                    retval = Properties.Settings.Default.cam2_size;

                    return retval;
                case 2:
                    retval = Properties.Settings.Default.cam3_size;

                    return retval;
                case 3:
                    retval = Properties.Settings.Default.cam4_size;

                    return retval;
                default: return new Size(640, 480);

            }

        }

        public static void SetWidth(int cam_ind)
        {
            char[] vs = { 'x' };
            bool resolution_changed = false;
            switch (cam_ind)
            {
                case 0:
                    if ((Properties.Settings.Default.cam1_size.Width != Properties.Settings.Default.camera_1_width) && (Properties.Settings.Default.cam1_size.Height != Properties.Settings.Default.camera_1_height))
                    {
                        Properties.Settings.Default.cam1_size = new Size(decimal.ToInt32(Properties.Settings.Default.camera_1_width), decimal.ToInt32(Properties.Settings.Default.camera_1_height));
                        resolution_changed = true;
                    }
                    break;
                case 1:
                    if ((Properties.Settings.Default.cam2_size.Width != Properties.Settings.Default.camera_2_width) && (Properties.Settings.Default.cam2_size.Height != Properties.Settings.Default.camera_2_height))
                    {
                        Properties.Settings.Default.cam2_size = new Size(decimal.ToInt32(Properties.Settings.Default.camera_2_width), decimal.ToInt32(Properties.Settings.Default.camera_2_height));
                        resolution_changed = true;
                    }
                    break;
                case 2:
                    if ((Properties.Settings.Default.cam3_size.Width != Properties.Settings.Default.camera_3_width) && (Properties.Settings.Default.cam3_size.Height != Properties.Settings.Default.camera_3_height))
                    {
                        Properties.Settings.Default.cam3_size = new Size(decimal.ToInt32(Properties.Settings.Default.camera_3_width), decimal.ToInt32(Properties.Settings.Default.camera_3_height));
                        resolution_changed = true;
                    }
                    break;
                case 3:
                    if ((Properties.Settings.Default.cam4_size.Width != Properties.Settings.Default.camera_4_width) && (Properties.Settings.Default.cam4_size.Height != Properties.Settings.Default.camera_4_height))
                    {
                        Properties.Settings.Default.cam4_size = new Size(decimal.ToInt32(Properties.Settings.Default.camera_4_width), decimal.ToInt32(Properties.Settings.Default.camera_4_height));
                        resolution_changed = true;
                    }
                    break;
            }
            if (resolution_changed)
            {
                MainForm.GetMainForm.crossbar.RESTART_CAMERA();
            }
        }
        public static System.Drawing.Size GetResolution(int cam_ind)
        {
            System.Drawing.Size retval;
            string[] res;
            switch (cam_ind)
            {
                case 0:
                  
                    retval = Properties.Settings.Default.cam1_resolution;
                    break;
                case 1:
                   
                    retval = Properties.Settings.Default.cam2_resolution;
                    break;
                case 2:
                    
                    retval = Properties.Settings.Default.cam3_resolution;
                    break;
                case 3:
                    
                    retval = Properties.Settings.Default.cam4_resolution;
                    break;
                default:
                    retval = new System.Drawing.Size(640, 480);
                    break;
            }
            return retval;
        }
        public static int GetFPS(int cam_ind)
        {
            int fps = 15;
            switch (cam_ind)
            {
                case 0:
                    fps = Int32.Parse(Properties.Settings.Default.cam1f);
                    break;
                case 1:
                    fps = Int32.Parse(Properties.Settings.Default.cam2f);
                    break;
                case 2:
                    fps = Int32.Parse(Properties.Settings.Default.cam3f);
                    break;
                case 3:
                    fps = Int32.Parse(Properties.Settings.Default.cam4f);
                    break;
            }
            return fps;
        }
    }
}
