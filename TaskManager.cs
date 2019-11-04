using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;

namespace FaceDetection
{
    class TaskManager
    {
        private static List<Task> listTask = new List<Task>();
        private static List<string> listRecordingFiles = new List<string>();
        private static Task task;
        private static TimeSpan timeSpanStart = new TimeSpan(0, 0, 0, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event)); // Time to keep before event
        private static TimeSpan timeSpanEnd = new TimeSpan(0, 0, 0, decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event)); // Time to keep after event
        private static TimeSpan fiveMinutes = new TimeSpan(0, 0, 5, 0);
        private static string directory = Environment.CurrentDirectory; // Directory of the project
        
        public TaskManager()
        {
        }

        public static void EventAppeared(string path, int numCamera, int b, int a) // An event appeared
        {
            string videoInList, dateCutVideoString, preeventFilesName = "";
            int compareDateValue, compareNotTooOld;
            DateTime dateCutVideo;
            DateTime eventTime = DateTime.Now;
            task = new Task(timeSpanStart, timeSpanEnd, eventTime, false, path, numCamera);
            listTask.Add(task);
            Timer taskTimer = new Timer(a * 1000); // Timer the end of the task
            taskTimer.Enabled = true;
            taskTimer.Elapsed += OnTimerEvent;

            AddFilesInList(); // Looking for files in the TEMP folder and add them to the list files
            
            if (listRecordingFiles.Count != 0)
            {
                for (int i = listRecordingFiles.Count; i > 0; i--)
                {
                    videoInList = listRecordingFiles.ElementAt(i - 1);
                    dateCutVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                    dateCutVideo = new DateTime(Convert.ToInt32(dateCutVideoString.Substring(0, 4)), Convert.ToInt32(dateCutVideoString.Substring(4, 2)), Convert.ToInt32(dateCutVideoString.Substring(6, 2)), Convert.ToInt32(dateCutVideoString.Substring(8, 2)), Convert.ToInt32(dateCutVideoString.Substring(10, 2)), Convert.ToInt32(dateCutVideoString.Substring(12, 2)));
                    compareNotTooOld = DateTime.Compare(dateCutVideo, eventTime - fiveMinutes);
                    compareDateValue = DateTime.Compare(dateCutVideo, eventTime - timeSpanStart);

                    if (compareNotTooOld > 0) // The file found is not too old (older than 5 minutes before the full video start)
                    {
                        if (compareDateValue < 0) // Date of the saved video is < than the started time of the full video. So we found the file to cut (And finished to look for more file)
                        {
                            if (preeventFilesName == "")
                            {
                                task.editPath(listRecordingFiles.ElementAt(i - 1));
                            }
                            else
                            {
                                task.editPath(listRecordingFiles.ElementAt(i - 1) + "|" + preeventFilesName);
                            }
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
                                preeventFilesName = listRecordingFiles.ElementAt(i - 1) + "|" + preeventFilesName;
                            }
                        }
                    }
                }
            }
        }

        private static void RecordEnd() // The event (task) is finished
        {
            string videoInList, cutFile, dateCutVideoString, startEventTime, fileToCutFromTheEnd, startVideoForFullFile, startVideoForFullFileName;
            string posteventFilesName = "";
            string[] arrayListOfFiles;
            DateTime dateCutVideo, dateEventStartTime, dateEventStop, dateStartVideoEvent;
            int compareDateValue, listTaskIndex = 0;
            TimeSpan tsTimeBeforeEvent;

            try
            {
                RefreshFilesInList(); // Add the new files recorded since the event started in the list

                // Check the date of the end of the event with the time of event + time after event in the task list to match which one we need to use
                DateTime recordEnd = DateTime.Now;
                for (int i = 0; i < listTask.Count; i++)
                {
                    dateEventStop = listTask.ElementAt(i).eventtime + listTask.ElementAt(i).stoptime;
                    if (Equals(recordEnd.ToString("yyyyMMddHHmmss"), dateEventStop.ToString("yyyyMMddHHmmss")))
                    {
                        listTaskIndex = i;
                        break;
                    }
                }

                startEventTime = listTask[listTaskIndex].eventtime.ToString("yyyyMMddHHmmss");
                dateEventStartTime = listTask[listTaskIndex].eventtime;

                
                // Cut the end of the file we found before that we need to start the full video
                arrayListOfFiles = listTask[listTaskIndex].allPreEventVideo.Split('|');
                fileToCutFromTheEnd = arrayListOfFiles.First();

                try
                {
                    // Look for the interval between the event and the beginning of the video which contain the event to know the interval we need to cut in the video before this file
                    startVideoForFullFileName = arrayListOfFiles.Last().Substring(arrayListOfFiles.Last().Length - 18, 18);
                    dateStartVideoEvent = new DateTime(Convert.ToInt32(startVideoForFullFileName.Substring(0, 4)), Convert.ToInt32(startVideoForFullFileName.Substring(4, 2)), Convert.ToInt32(startVideoForFullFileName.Substring(6, 2)), Convert.ToInt32(startVideoForFullFileName.Substring(8, 2)), Convert.ToInt32(startVideoForFullFileName.Substring(10, 2)), Convert.ToInt32(startVideoForFullFileName.Substring(12, 2)));
                    tsTimeBeforeEvent = listTask[listTaskIndex].eventtime - dateStartVideoEvent;

                    startVideoForFullFile = CutVideoKeepEnd(fileToCutFromTheEnd, decimal.ToInt32(Properties.Settings.Default.event_record_time_before_event) - tsTimeBeforeEvent.Seconds);
                }
                catch (Exception e) // no file before the event
                {
                    startVideoForFullFile = "";
                }

                if (listRecordingFiles.Count != 0)
                {
                    for (int i = 1; i <= listRecordingFiles.Count; i++)
                    {
                        videoInList = listRecordingFiles.ElementAt(i - 1);
                        dateCutVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateCutVideo = new DateTime(Convert.ToInt32(dateCutVideoString.Substring(0, 4)), Convert.ToInt32(dateCutVideoString.Substring(4, 2)), Convert.ToInt32(dateCutVideoString.Substring(6, 2)), Convert.ToInt32(dateCutVideoString.Substring(8, 2)), Convert.ToInt32(dateCutVideoString.Substring(10, 2)), Convert.ToInt32(dateCutVideoString.Substring(12, 2)));
                        compareDateValue = DateTime.Compare(dateCutVideo, dateEventStartTime - timeSpanStart);
                        if (compareDateValue > 0) // the date of the saved file is > to the event start time. We keep only files after event started
                        {
                            if (dateCutVideo <= (dateEventStartTime + listTask[listTaskIndex].stoptime)) // the date of the saved file is <= to the event start + time parametered we need after the start
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
                                cutFile = CutVideoKeepStart(listRecordingFiles.ElementAt(i - 1), decimal.ToInt32(Properties.Settings.Default.event_record_time_after_event));
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
                }

                System.Threading.Thread.Sleep(100); // Wait for the last files is released by the system

                ConcatVideo(startVideoForFullFile, posteventFilesName, startEventTime, listTaskIndex); // Concat all path files to send to ffmpeg to make the final video with all cuts
                listTask.Remove(listTask[listTaskIndex]); // Delete the task ended
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + " line 169");
            }
        }

        private static string CutVideoKeepEnd(string videoToCut, int cutTimeParameter) // Cut the video we need for the beginning of the full video keeping just the end of the file
        {
            string videoCutStartTimeFormated, videoCutName, videoStartTime, videoDate;
            TimeSpan tsCut = new TimeSpan(0, 0, 0, cutTimeParameter);

            try
            {
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
            }
            catch (Exception e) // No video to cut. The application has probably just started
            {
                Console.WriteLine(e.Message + " line 209");
                videoCutName = "";
            }
            return videoCutName;
        }

        private static string CutVideoKeepStart(string videoToCut, int cutTimeParameter) // Cut the video we need for the end of the full video keeping just the beginning of the file
        {
            string videoCutEndTimeFormated, videoDate, videoEndTime, videoCutName;
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
            videoEndTime = (new DateTime(Convert.ToInt32(videoDate.Substring(0, 4)), Convert.ToInt32(videoDate.Substring(4, 2)), Convert.ToInt32(videoDate.Substring(6, 2)), Convert.ToInt32(videoDate.Substring(8, 2)), Convert.ToInt32(videoDate.Substring(10, 2)), Convert.ToInt32(videoDate.Substring(12, 2))) + tsCut).ToString("yyyyMMddHHmmss");
            videoCutName = path + videoEndTime + ".avi";

            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-y -i " + videoToCut + " -ss 00:00:00 -to " + videoCutEndTimeFormated + " -c copy -avoid_negative_ts 1 " + videoCutName;
            Process.Start(startInfo);

            return videoCutName;
        }

        private static void ConcatVideo(string preEventVideoFiles, string postEventVideoFiles, string startTime, int TaskIndex) // Concat all videos needed to construct the full video
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffmpeg.exe");
            if (preEventVideoFiles != "")
            {
                startInfo.Arguments = @"-i concat:" + preEventVideoFiles + "|" + postEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[TaskIndex].cameraNumber.ToString() + "\\" + listTask[TaskIndex].path + "\\" + startTime + ".avi"; 
            }
            else // No file to keep before the event
            {
                startInfo.Arguments = @"-i concat:" + postEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[TaskIndex].cameraNumber.ToString() + "\\" + listTask[TaskIndex].path + "\\" + startTime + ".avi";
            }
            Process.Start(startInfo);
        }

        private static void AddFilesInList() // Add all files in the temp folder into the list
        {
            try
            {
                if (Directory.Exists(@"D:\TEMP"))
                {
                    DeleteOldFiles(@"D:\TEMP");
                    string[] list = Directory.GetFiles(@"D:\TEMP");
                    listRecordingFiles = list.ToList();
                }
                else
                {
                    DeleteOldFiles(@"C:\TEMP");
                    string[] list = Directory.GetFiles(@"C:\TEMP");
                    listRecordingFiles = list.ToList();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + " line 275");
            }
        }

        private static void RefreshFilesInList() // Add all files in the temp folder into the list
        {
            try
            {
                if (Directory.Exists(@"D:\TEMP"))
                {
                    string[] list = Directory.GetFiles(@"D:\TEMP");
                    listRecordingFiles = list.ToList();
                }
                else
                {
                    string[] list = Directory.GetFiles(@"C:\TEMP");
                    listRecordingFiles = list.ToList();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + " line 296");
            }
        }

        private static void DeleteOldFiles(string pathTempFolder) // Delete old files that we don't need anymore
        {
            DirectoryInfo dir;
            string fileName;
            DateTime fileDate;
            TimeSpan tsTimeMaxToKeep = new TimeSpan(10, 0, 15, 0); // TODO: How many time to keep ?

            try
            {
                dir = new DirectoryInfo(pathTempFolder);
                FileInfo[] filesList = dir.GetFiles();
                foreach (FileInfo files in filesList)
                {
                    fileName = files.FullName;
                    fileDate = DateTime.ParseExact(fileName.Substring(files.FullName.Length - 18, 14), "yyyyMMddHHmmss", null);
                    if (DateTime.Compare(fileDate, (DateTime.Now - tsTimeMaxToKeep)) < 0)
                    {
                        File.Delete(files.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " line 323");
            }
        }

        private static string GetVideoDuration(string videoPath) // Get the duration of the video file gave in parameter
        {
            string cmd = string.Format("-v error -select_streams v:0 -show_entries stream=duration -sexagesimal -of default=noprint_wrappers=1:nokey=1  {0}", videoPath);
            Process proc = new Process();
            proc.StartInfo.FileName = directory + @"\ffmpeg-20191025-155508c-win64-static\bin\ffprobe.exe";
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
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

        public static void OnTimerEvent(object sender, EventArgs e) // The event task is finished
        {
            RecordEnd();
        }
    }

    class Task
    {
        public TimeSpan starttime;
        public TimeSpan stoptime;
        public DateTime eventtime;
        public bool complete; // not used
        public string path;
        public string allPreEventVideo = "";
        public int cameraNumber;

        /// <summary>
        /// Initialize a task
        /// </summary>
        /// <param name="starttime">start time</param>
        /// <param name="stoptime">stop time</param>
        /// <param name="eventtime">event time</param>
        /// <param name="complete">complete</param>
        /// <param name="path">path</param>
        public Task(TimeSpan starttime, TimeSpan stoptime, DateTime eventtime, bool complete, string path, int cameraNumber)
        {
            this.path = path;
            this.starttime = starttime;
            this.stoptime = stoptime;
            this.eventtime = eventtime;
            this.complete = complete;
            this.cameraNumber = cameraNumber;
        }

        public void editPath(string path)
        {
            allPreEventVideo = path;
        }
    }
}
