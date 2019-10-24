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
        //internal void RecTask(string sz)
        //{
        //    int reclen = Decimal.ToInt32(Properties.Settings.Default.recording_length_seconds);

        //    TimeSpan ts2 = new TimeSpan(0, 0, 0, reclen);
        //    Task task = new Task(sz);

        //    list.Add(task);

        //    DateTime iventtime = dt - ts2;

        //    Console.WriteLine(dt);
        //    Console.WriteLine(iventtime);

        //    settingsUI.Camera
          

        //    }
         
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


