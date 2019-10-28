using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    

    class FaceDetector
    {
        static Timer face_check_timer = new Timer();

        /// <summary>
        /// starts the face recognition module
        /// </summary>
        public static void START()
        {
            face_check_timer.Interval = decimal.ToInt32(Properties.Settings.Default.face_rec_interval);
            face_check_timer.Tick += Face_check_timer_Tick;
        }

        private static void Face_check_timer_Tick(object sender, EventArgs e)
        {
            //TODO
            //all face recognition ruitine starts here

            //

            //We detected a face so we call this
            if (MainForm.GetMainForm != null)
            {
                MainForm.ACTIVE_RECPATH = MainForm.RECPATH.EVENT;
                MainForm.GetMainForm.RecordMode();

            }
        }
    }
}
