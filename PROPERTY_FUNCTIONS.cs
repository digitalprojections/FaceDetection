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
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));

                    return retval;
                case 1:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C2w), decimal.ToInt32(Properties.Settings.Default.C2h));

                    return retval;
                case 2:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C3w), decimal.ToInt32(Properties.Settings.Default.C3h));

                    return retval;
                case 3:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C4w), decimal.ToInt32(Properties.Settings.Default.C4h));

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
                    if (Properties.Settings.Default.C1w != decimal.Parse(Properties.Settings.Default.C1res.Split(vs)[0]))
                    {
                        Properties.Settings.Default.C1w = decimal.Parse(Properties.Settings.Default.C1res.Split(vs)[0]);
                        Properties.Settings.Default.C1h = decimal.Parse(Properties.Settings.Default.C1res.Split(vs)[1]);
                        resolution_changed = true;
                    }
                    break;
                case 1:
                    if (Properties.Settings.Default.C2w != decimal.Parse(Properties.Settings.Default.C2res.Split(vs)[0]))
                    {
                        Properties.Settings.Default.C2w = decimal.Parse(Properties.Settings.Default.C2res.Split(vs)[0]);
                        Properties.Settings.Default.C2h = decimal.Parse(Properties.Settings.Default.C2res.Split(vs)[1]);
                        resolution_changed = true;
                    }
                    break;
                case 2:
                    if (Properties.Settings.Default.C3w != decimal.Parse(Properties.Settings.Default.C3res.Split(vs)[0]))
                    {
                        Properties.Settings.Default.C3w = decimal.Parse(Properties.Settings.Default.C3res.Split(vs)[0]);
                        Properties.Settings.Default.C3h = decimal.Parse(Properties.Settings.Default.C3res.Split(vs)[1]);
                        resolution_changed = true;
                    }
                    break;
                case 3:
                    if (Properties.Settings.Default.C4w != decimal.Parse(Properties.Settings.Default.C4res.Split(vs)[0]))
                    {
                        Properties.Settings.Default.C4w = decimal.Parse(Properties.Settings.Default.C4res.Split(vs)[0]);
                        Properties.Settings.Default.C4h = decimal.Parse(Properties.Settings.Default.C4res.Split(vs)[1]);
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
                    res = Properties.Settings.Default.C1res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 1:
                    res = Properties.Settings.Default.C2res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 2:
                    res = Properties.Settings.Default.C3res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
                    break;
                case 3:
                    res = Properties.Settings.Default.C4res.Split('x');
                    retval = new System.Drawing.Size(Int32.Parse(res[0]), Int32.Parse(res[1]));
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
                    fps = Int32.Parse(Properties.Settings.Default.C1f);
                    break;
                case 1:
                    fps = Int32.Parse(Properties.Settings.Default.C2f);
                    break;
                case 2:
                    fps = Int32.Parse(Properties.Settings.Default.C3f);
                    break;
                case 3:
                    fps = Int32.Parse(Properties.Settings.Default.C4f);
                    break;
            }
            return fps;
        }
    }
}
