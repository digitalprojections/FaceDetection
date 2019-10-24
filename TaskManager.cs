using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection
{
    public class TaskManager
    {

        List<Task> list;
        
        public TaskManager()
        {
            //PROPERTY

            list = new List<Task>();


        }
        internal void SuetTask(string sv)
        {
            int secafter = Decimal.ToInt32(Properties.Settings.Default.seconds_after_event%60);
            int secbefore = Decimal.ToInt32(Properties.Settings.Default.seconds_before_event%60);
            DateTime dt = DateTime.Now;

            TimeSpan ts1 = new TimeSpan(0, 0, 0, secbefore);
            TimeSpan ts2 = new TimeSpan(0, 0, 0, secafter);


            Task task = new Task(sv);
            list.Add(task);

            Console.WriteLine(list.Count);

            DateTime starttime = dt - ts1;
            DateTime stoptime = dt + ts2;



            Console.WriteLine(dt);
            Console.WriteLine(starttime);
            Console.WriteLine(stoptime);

        }

        internal void StatusCheck()
        {

            if (list.Count>0 && MainForm.CURRENT_MODE == MainForm.CAMERA_MODES.NONE)
            {
                

            }
            else if (MainForm.CURRENT_MODE == MainForm.CAMERA_MODES.CAPTURE)
            {
                MainForm.GetMainForm.GetRecorder().Release();

            }
           MainForm.GetCameraInstance();
           
        }

    }

}
    class Task
    {
        public Task(string path)
        {
            this.path = path;
        }
        string starttime;
        string stoptime;
        string iventtime;
        bool complete;
        string path;
    }


