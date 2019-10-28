using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int secafter = Decimal.ToInt32(Properties.Settings.Default.seconds_after_event % 60);
            int secbefore = Decimal.ToInt32(Properties.Settings.Default.seconds_before_event % 60);
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

            if (list.Count > 0 && MainForm.CURRENT_MODE == MainForm.CAMERA_MODES.ERROR)
            {


            }
            else if (MainForm.CURRENT_MODE == MainForm.CAMERA_MODES.CAPTURE)
            {
                //MainForm.GetMainForm.GetRecorder().Release();


            }
        }

        // Robin 2019_10_26 start
        public string cutVideo()
        {
            string videoCutStartTimeFormated;
            string cutVideo = "output.avi";
            string videoDuration = GetVideoDuration();
            int videoDurationSec = Convert.ToInt32(videoDuration.Substring(5, 2)) + Convert.ToInt32(videoDuration.Substring(2, 2)) * 60;
            int videoCutStartTime = videoDurationSec - 5; //TODO: use the parametered time before event
            if ((videoCutStartTime % 60) >= 10)
            {
                videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":" + videoCutStartTime % 60;
            }
            else
            {
                videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":0" + videoCutStartTime % 60;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\MousePro04\source\repos\TaskManagerTest\TaskManagerTest\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe"); //TODO: Use path where ffmpeg.exe is present
            startInfo.Arguments = @"-i " + @"C:\1\20191026112530.avi" + " -ss " + videoCutStartTimeFormated + " -to 0" + videoDuration + " -c copy -avoid_negative_ts 1 " + @"C:\UVCCAMERA\" + cutVideo; //TODO: use path and video name
            Process.Start(startInfo);

            return cutVideo;
        }

        public void concatVideo()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\MousePro04\source\repos\TaskManagerTest\TaskManagerTest\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-i concat:" + @"C:\1\20191026112530.avi" + "|" + @"C:\1\20191026085927.avi" + " -c copy " + @"C:\UVCCAMERA\output.avi";  //TODO: use path and video name
            Process.Start(startInfo);
        }


        private static string GetVideoDuration()
        {
            string basePath = @"C:\1\";
            string filePath = basePath + @"20191026112530.avi"; //TODO: use path and video name
            string cmd = string.Format("-v error -select_streams v:0 -show_entries stream=duration -sexagesimal -of default=noprint_wrappers=1:nokey=1  {0}", filePath);
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\Users\MousePro04\source\repos\TaskManagerTest\TaskManagerTest\ffmpeg-20191025-155508c-win64-static\bin\ffprobe.exe"; //TODO: Use path where ffprobe.exe is present
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                Console.WriteLine("Error starting");
                return "Error";
            }
            string duration = proc.StandardOutput.ReadToEnd().Replace("\r\n", "");
            // Remove the milliseconds
            duration = duration.Substring(0, duration.LastIndexOf("."));
            proc.WaitForExit();
            proc.Close();

            return duration;
        }
        // Robin 2019_10_26 end

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
