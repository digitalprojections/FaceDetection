using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FaceDetection
{
    class TaskManager
    {
        private static List<Task> listTask = new List<Task>();
        private static List<string> listRecordingFiles = new List<string>();
        private static Task task;
        private static TimeSpan timeSpanEnd;
        private static TimeSpan timeSpanStart;

        // Directory of the project
        private static string directory = Environment.CurrentDirectory;
        
        public TaskManager()
        {
        }

        private static void AddFilesInList()
        {
            // TODO : Add files in the list in a dynamic way, checking the DateTime
            // TODO : Delete files which are older than max parameters keeping files possible
            try
            {
                try
                {
                    string[] list = System.IO.Directory.GetFiles(@"D:\TEMP");
                    listRecordingFiles = list.ToList();
                }
                catch
                {
                    string[] list = System.IO.Directory.GetFiles(@"C:\TEMP");
                    listRecordingFiles = list.ToList();
                }
            }
            catch (IOException e) { }
        }

        public static void EventAppeared(string path, int paramTimeBeforeEvent, int paramTimeAfterEvent)
        {
            string videoInList, dateCutVideoString, preeventFilesName = "";
            int compareDateValue;
            DateTime dateCutVideo;
            TimeSpan tsBeforeEvent = new TimeSpan(0, 0, 0, paramTimeBeforeEvent);
            DateTime fullVideoStartTime = DateTime.Now;//.ToString("yyyyMMddHHmmss");
            timeSpanStart = new TimeSpan(0, 0, 0, paramTimeBeforeEvent);
            timeSpanEnd = new TimeSpan(0, 0, 0, paramTimeAfterEvent);
            task = new Task(timeSpanStart, timeSpanEnd, fullVideoStartTime, false, path);
            listTask.Add(task);

            AddFilesInList(); // Looking for files in the TEMP folder and add them to the list files
            
            if (listRecordingFiles.Count > 1)
            {
                for (int i = listRecordingFiles.Count; i >= 0; i--)
                {
                    videoInList = listRecordingFiles.ElementAt(i - 1);
                    dateCutVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                    dateCutVideo = new DateTime(Convert.ToInt32(dateCutVideoString.Substring(0, 4)), Convert.ToInt32(dateCutVideoString.Substring(4, 2)), Convert.ToInt32(dateCutVideoString.Substring(6, 2)), Convert.ToInt32(dateCutVideoString.Substring(8, 2)), Convert.ToInt32(dateCutVideoString.Substring(10, 2)), Convert.ToInt32(dateCutVideoString.Substring(12, 2)));
                    compareDateValue = DateTime.Compare(dateCutVideo, fullVideoStartTime);

                    if (compareDateValue < 0) // Date of the saved video is < at the started time of the full video => so we found the file to cut (And finished to look for more file)
                    {
                        //cutFile = CutVideoKeepEnd(listRecordingFiles.ElementAt(i - 1), decimal.ToInt32(Properties.Settings.Default.seconds_before_event));

                        if (preeventFilesName == "")
                        {
                            task.editPath(listRecordingFiles.ElementAt(i - 1));
                        }
                        else
                        {
                            task.editPath(preeventFilesName + "|" + listRecordingFiles.ElementAt(i - 1));
                        }
                        // TODO: Delete file older than necessary (Here ?)
                        break;
                    }
                    else // Date of the saved video is > at the started time of the full video => so we have to keep this file
                    {
                        if (preeventFilesName == "")
                        {
                            preeventFilesName = listRecordingFiles.ElementAt(i - 1);
                        }
                        else
                        {
                            preeventFilesName += "|" + listRecordingFiles.ElementAt(i - 1);
                        }
                    }
                }
            }
        }

        public static void RecordEnd() // Call by the main program when the camera status changed ?
        {
            string videoInList, cutFile, dateCutVideoString, startEventTime, fileToCutFromTheEnd;
            string posteventFilesName = "";
            string[] arrayListOfFiles;
            DateTime dateCutVideo, dateEventStartTime;
            int compareDateValue, listTaskIndex = 0;
            DateTime DateEventStop;

            try
            {
                // Check the date of the end of the event with the time of event + time after event in the task list to match which one we need to use
                DateTime recordEnd = DateTime.Now;
                for (int i = 0; i < listTask.Count; i++)
                {
                    DateEventStop = listTask.ElementAt(i).eventtime + listTask.ElementAt(i).stoptime;
                    if (Equals(recordEnd.ToString("yyyyMMddHHmmss"), DateEventStop.ToString("yyyyMMddHHmmss")))
                    {
                        listTaskIndex = i;
                        break;
                    }
                }

                startEventTime = listTask[listTaskIndex].eventtime.ToString("yyyyMMddHHmmss");
                dateEventStartTime = listTask[listTaskIndex].eventtime;

                // Cut the end of the file we found before that we need to start the full video
                arrayListOfFiles = listTask[listTaskIndex].allPreEventVideo.Split('|');
                fileToCutFromTheEnd = arrayListOfFiles.Last();
                CutVideoKeepEnd(fileToCutFromTheEnd, decimal.ToInt32(Properties.Settings.Default.seconds_before_event));
                
                if (listRecordingFiles.Count != 0)
                {
                    for (int i = listRecordingFiles.Count; i >= 0; i--)
                    {
                        videoInList = listRecordingFiles.ElementAt(i - 1);
                        dateCutVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateCutVideo = new DateTime(Convert.ToInt32(dateCutVideoString.Substring(0, 4)), Convert.ToInt32(dateCutVideoString.Substring(4, 2)), Convert.ToInt32(dateCutVideoString.Substring(6, 2)), Convert.ToInt32(dateCutVideoString.Substring(8, 2)), Convert.ToInt32(dateCutVideoString.Substring(10, 2)), Convert.ToInt32(dateCutVideoString.Substring(12, 2)));
                        compareDateValue = DateTime.Compare(dateCutVideo, dateEventStartTime);

                        if (compareDateValue > 0 && dateCutVideo <= (dateEventStartTime + listTask[listTaskIndex].stoptime)) // the date of the saved file is > to the event start time but <= to the event start + time parametered we need after the start
                        {
                            if (posteventFilesName == "")
                            {
                                posteventFilesName = listRecordingFiles.ElementAt(i - 1);
                            }
                            else
                            {
                                posteventFilesName += "|" + listRecordingFiles.ElementAt(i - 1);
                            }
                        }
                        else // We found the last file we need to save and need to cut it
                        {
                            cutFile = CutVideoKeepStart(listRecordingFiles.ElementAt(i - 1), decimal.ToInt32(Properties.Settings.Default.seconds_after_event));
                            if (posteventFilesName == "")
                            {
                                posteventFilesName = cutFile;
                            }
                            else
                            {
                                posteventFilesName += "|" + cutFile;
                            }
                            break;
                        }
                    }
                }

                ConcatVideo(listTask[listTaskIndex].allPreEventVideo, posteventFilesName, startEventTime); // Concat all path files to send to ffmpeg to make the final video with all cuts
                listTask.Remove(listTask[listTaskIndex]); // Delete the task ended
            }
            catch { }
        }


        private static string CutVideoKeepEnd(string videoToCut, int cutTimeParameter)
        {
            string videoCutStartTimeFormated, videoCutName, videoStartTime, videoDate;
            TimeSpan tsCut = new TimeSpan(0, 0, 0, cutTimeParameter);

            // Manage the duration to create the name of the cut video
            string videoDuration = GetVideoDuration(videoToCut);
            DateTime dateDuration = Convert.ToDateTime(videoDuration);
            int iduration = dateDuration.Hour * 10000 + dateDuration.Minute * 100 + dateDuration.Second;
            TimeSpan tsDuration = new TimeSpan(0, 0, 0, iduration);

            string path = videoToCut.Substring(0, videoToCut.Length - 18);
            int videoDurationSec = Convert.ToInt32(videoDuration.Substring(5, 2)) + Convert.ToInt32(videoDuration.Substring(2, 2)) * 60;
            int videoCutStartTime = videoDurationSec - cutTimeParameter;
            if ((videoCutStartTime % 60) >= 10)
            {
                videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":" + videoCutStartTime % 60;
            }
            else
            {
                videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":0" + videoCutStartTime % 60;
            }

            // Create name for the cut video
            videoDate = videoToCut.Substring(videoToCut.Length - 18, 14);
            videoStartTime = (new DateTime(Convert.ToInt32(videoDate.Substring(0, 4)), Convert.ToInt32(videoDate.Substring(4, 2)), Convert.ToInt32(videoDate.Substring(6, 2)), Convert.ToInt32(videoDate.Substring(8, 2)), Convert.ToInt32(videoDate.Substring(10, 2)), Convert.ToInt32(videoDate.Substring(12, 2))) + tsDuration - tsCut).ToString("yyyyMMddHHmmss");
            videoCutName = path + videoStartTime + ".avi";

            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-y -i " + videoToCut + " -ss " + videoCutStartTimeFormated + " -to 0" + videoDuration + " -c copy -avoid_negative_ts 1 " + videoCutName;
            Process.Start(startInfo);

            return videoToCut;
        }

        private static string CutVideoKeepStart(string videoToCut, int cutTimeParameter)
        {
            string videoCutEndTimeFormated, videoDate, videoStartTime, videoCutName;
            TimeSpan tsCut = new TimeSpan(0, 0, 0, cutTimeParameter);
            string path = videoToCut.Substring(0, videoToCut.Length - 18);

            if ((cutTimeParameter % 60) >= 10)
            {
                videoCutEndTimeFormated = "00:0" + cutTimeParameter / 60 + ":" + cutTimeParameter % 60;
            }
            else
            {
                videoCutEndTimeFormated = "00:0" + cutTimeParameter / 60 + ":0" + cutTimeParameter % 60;
            }

            // Create name for cut video
            videoDate = videoToCut.Substring(videoToCut.Length - 18, 14);
            videoStartTime = (new DateTime(Convert.ToInt32(videoDate.Substring(0, 4)), Convert.ToInt32(videoDate.Substring(4, 2)), Convert.ToInt32(videoDate.Substring(6, 2)), Convert.ToInt32(videoDate.Substring(8, 2)), Convert.ToInt32(videoDate.Substring(10, 2)), Convert.ToInt32(videoDate.Substring(12, 2))) + tsCut).ToString("yyyyMMddHHmmss");
            videoCutName = path + videoStartTime + ".avi";

            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-y -i " + videoToCut + " -ss 00:00:00 -to " + videoCutEndTimeFormated + " -c copy -avoid_negative_ts 1 " + videoCutName;
            Process.Start(startInfo);

            return videoToCut;
        }

        private static void ConcatVideo(string preEventVideoFiles, string postEventVideoFiles, string startTime)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-i concat:" + preEventVideoFiles + "|" + postEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location +"/" + startTime + ".avi";  // TODO: + Task.path ?
            Process.Start(startInfo);
        }


        private static string GetVideoDuration(string videoPath)
        {
            string cmd = string.Format("-v error -select_streams v:0 -show_entries stream=duration -sexagesimal -of default=noprint_wrappers=1:nokey=1  {0}", videoPath);
            Process proc = new Process();
            proc.StartInfo.FileName = directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffprobe.exe";
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
    }

    class Task
    {
        public TimeSpan starttime;
        public TimeSpan stoptime;
        public DateTime eventtime;
        public bool complete;
        public string path;
        public string allPreEventVideo;

        /// <summary>
        /// Initialize a task
        /// </summary>
        /// <param name="starttime">start time</param>
        /// <param name="stoptime">stop time</param>
        /// <param name="eventtime">event time</param>
        /// <param name="complete">complete</param>
        /// <param name="path">path</param>
        public Task(TimeSpan starttime, TimeSpan stoptime, DateTime eventtime, bool complete, string path)
        {
            this.path = path;
            this.starttime = starttime;
            this.stoptime = stoptime;
            this.eventtime = eventtime;
            this.complete = complete;
        }

        public void editPath(string path)
        {
            allPreEventVideo = path;
        }
    }
}
