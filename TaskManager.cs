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
        private static List<TaskItem> listTask = new List<TaskItem>();
        private static List<string> listRecordingFiles = new List<string>();
        private static TaskItem task;
        private static TimeSpan timeSpanStart; // Time to keep before event
        private static TimeSpan timeSpanEnd;  // Time to keep after event
        private static TimeSpan tenMinutes = new TimeSpan(0, 0, 10, 0); // Time span of 10 minutes
        private static string directory = Environment.CurrentDirectory; // Directory of the project
        private const int BUFFERDURATION = BUFFER_DURATION.BUFFERDURATION; // Duration of the buffer (each time a new TEMP video file is created)

        public TaskManager()
        {
        }

        public static void EventAppeared(string path, int numCamera, int timeBeforeEvent, int timeAfterEvent, DateTime triggerTime) // An event appeared
        {
            if (Directory.Exists(@"D:\TEMP"))
            {
                Directory.CreateDirectory(@"D:\TEMP\" + numCamera + @"\CutTemp");
            }
            else
            {
                Directory.CreateDirectory(@"C:\TEMP\" + numCamera + @"\CutTemp");
            }

            if (Directory.Exists(@"ffmpeg-20191101-53c21c2-win32-static"))
            {
                string videoInList, dateTempVideoString, preeventFilesName = "";
                int compareDateValue, compareNotTooOld;
                DateTime dateTempVideo;
                DateTime eventTime = triggerTime; // DateTime.Now;

                Console.WriteLine("Event Appeared ! eventTime: " + eventTime);
                LOGGER.Add("Event Appeared ! eventTime: " + eventTime);

                timeSpanStart = new TimeSpan(0, 0, 0, timeBeforeEvent);
                timeSpanEnd = new TimeSpan(0, 0, 0, timeAfterEvent);
                task = new TaskItem(timeSpanStart, timeSpanEnd, eventTime, false, path, numCamera);
                listTask.Add(task);
                Timer taskTimer = new Timer(timeAfterEvent * 1000); // Timer of the end of the task
                taskTimer.Enabled = true;
                taskTimer.Elapsed += OnTimerEvent;
                taskTimer.AutoReset = false;

                RefreshFilesInList(numCamera); // Looking for files in the TEMP folder and add them to the list files

                if (listRecordingFiles.Count == 1) // There is only one file in the TEMP folder. So we need this file and don't have to compare dates of files.
                {
                    task.EditPath(listRecordingFiles.ElementAt(0));
                }
                else if (listRecordingFiles.Count != 0) // Several files in TEMP folder, we need to look for which ones we have to keep or not
                {
                    try
                    {
                        for (int i = listRecordingFiles.Count; i > 0; i--)
                        {
                            videoInList = listRecordingFiles.ElementAt(i - 1);
                            dateTempVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                            dateTempVideo = new DateTime(Convert.ToInt32(dateTempVideoString.Substring(0, 4)), Convert.ToInt32(dateTempVideoString.Substring(4, 2)), Convert.ToInt32(dateTempVideoString.Substring(6, 2)), Convert.ToInt32(dateTempVideoString.Substring(8, 2)), Convert.ToInt32(dateTempVideoString.Substring(10, 2)), Convert.ToInt32(dateTempVideoString.Substring(12, 2)));
                            compareNotTooOld = DateTime.Compare(dateTempVideo, eventTime - tenMinutes);
                            compareDateValue = DateTime.Compare(dateTempVideo, eventTime - timeSpanStart);

                            if (compareNotTooOld >= 0) // The file found is not too old (older than 10 minutes before the full video start) (If buffer 5 minutes and time wantedbefore is 5 minutes, we need to look until 10 minutes before files)
                            {
                                if (compareDateValue < 0) // Date of the temp video is < than the started time of the full video. So we found the file to cut (And finished to look for more file)
                                {
                                    if (preeventFilesName == "")
                                    {
                                        task.EditPath(listRecordingFiles.ElementAt(i - 1));
                                    }
                                    else
                                    {
                                        task.EditPath(listRecordingFiles.ElementAt(i - 1) + "|" + preeventFilesName);
                                    }
                                    break;
                                }
                                else // Date of the temp video is > at the started time of the full video => so we have to keep this file
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
                    catch (Exception ex) // Unexpected files in TEMP folder
                    {
                        Console.WriteLine(ex.Message + " TaskManager in EventAppeared()");
                        LOGGER.Add(ex.Message + " TaskManager in EventAppeared()");
                    }
                }

                // Create event folder if it doesn't exist (ffmpeg can't create folder itself)
                if (!Directory.Exists(Properties.Settings.Default.video_file_location + "/Camera/" + numCamera + @"/" + path))
                {
                    try
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera/" + numCamera + @"/" + path);
                    }
                    catch (IOException iox)
                    {
                        Console.WriteLine(iox.Message);
                    }
                }
            }
        }

        private static void RecordEnd() // The event (task) is finished
        {
            string videoInList, cutFile, dateCutVideoString, startEventTime, fileToCutFromTheEnd, startVideoForFullFile, startVideoForFullFileName, fileToCutFromTheEndDate, durationOfVideoToCutString;
            string posteventFilesName = "";
            string[] arrayListOfFiles;
            DateTime dateCutVideo, dateEventStartTime, dateEventStop, dateStartVideoEvent, dateEventMinusWantedTime, dateFileToCutFromTheEnd;
            int compareDateWithEvent, compareDateWithFilePreevent, cutTime, durationOfVideoToCut, timeToCutLastVideo, timeWanted, listTaskIndex = 0;
            TimeSpan tsTimeBeforeEvent, timeFromStartVideoToEvent, timeWantedToCut;
            bool fileSaved = false;
            TimeSpan tsBufferLengthDuration = new TimeSpan(0, 0, 0, BUFFERDURATION / 1000);
            DateTime recordEnd = DateTime.Now;

            try
            {
                RefreshFilesInList(listTask[listTask.Count-1].cameraNumber); // Add the new files recorded since the event started in the list (from the last task)
                listRecordingFiles.Sort(); // If files are in wrong order (Touch panel issues), bring them back in a right order

                // Check the date of the end of the event with the time of event + time after event in the task list to match which one we need to use
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
                    timeWanted = listTask[listTaskIndex].starttime.Minutes * 60 + listTask[listTaskIndex].starttime.Seconds;

                    cutTime = timeWanted - (tsTimeBeforeEvent.Minutes * 60 + tsTimeBeforeEvent.Seconds); // Time wanted (param) before event - time ellapsed in the file before event appeared
                    if (cutTime < 0) // The event and the time before we need to keep are in the same file. No need to look for interval between the event video file and the file before AND the file is finished
                    {
                        if (((dateEventStartTime - listTask[listTaskIndex].starttime) > dateStartVideoEvent) && ((dateEventStartTime - dateStartVideoEvent + listTask[listTaskIndex].stoptime) < tsBufferLengthDuration)) // Time before anf afterthe event are in the same file
                        {
                            timeFromStartVideoToEvent = listTask[listTaskIndex].eventtime - dateStartVideoEvent;
                            startVideoForFullFile = CutVideoFromEvent(fileToCutFromTheEnd, timeFromStartVideoToEvent, listTask[listTaskIndex].starttime.Minutes * 60 + listTask[listTaskIndex].starttime.Seconds, listTask[listTaskIndex].stoptime.Minutes * 60 + listTask[listTaskIndex].stoptime.Seconds, listTaskIndex);
                            fileSaved = true;
                        }
                        else
                        {
                            durationOfVideoToCutString = GetVideoDuration(fileToCutFromTheEnd);
                            durationOfVideoToCut = Convert.ToInt32(durationOfVideoToCutString.Substring(2, 2)) * 60 + Convert.ToInt32(durationOfVideoToCutString.Substring(5, 2));
                            cutTime = (durationOfVideoToCut - (tsTimeBeforeEvent.Minutes * 60 + tsTimeBeforeEvent.Seconds)) + (listTask[listTaskIndex].starttime.Minutes * 60 + listTask[listTaskIndex].starttime.Seconds);
                            startVideoForFullFile = CutVideoKeepEnd(fileToCutFromTheEnd, cutTime);
                        }
                    }
                    else
                    {
                        if (listRecordingFiles.Last() != fileToCutFromTheEnd) // If this is different, it's means that there are several files in TEMP folder. 
                        {
                            // Looking for the time to cut in the video concerned
                            dateEventMinusWantedTime = dateEventStartTime - listTask[listTaskIndex].starttime;
                            fileToCutFromTheEndDate = fileToCutFromTheEnd.Substring(fileToCutFromTheEnd.Length - 18, 18);
                            dateFileToCutFromTheEnd = new DateTime(Convert.ToInt32(fileToCutFromTheEndDate.Substring(0, 4)), Convert.ToInt32(fileToCutFromTheEndDate.Substring(4, 2)), Convert.ToInt32(fileToCutFromTheEndDate.Substring(6, 2)), Convert.ToInt32(fileToCutFromTheEndDate.Substring(8, 2)), Convert.ToInt32(fileToCutFromTheEndDate.Substring(10, 2)), Convert.ToInt32(fileToCutFromTheEndDate.Substring(12, 2)));
                            timeWantedToCut = dateEventMinusWantedTime - dateFileToCutFromTheEnd;
                            cutTime = (BUFFERDURATION/1000) - (timeWantedToCut.Minutes * 60 + timeWantedToCut.Seconds);

                            startVideoForFullFile = CutVideoKeepEnd(fileToCutFromTheEnd, cutTime);
                        }
                        else // Just one file in TEMP folder, means that the time we want to keep before the event is > to the time which has actually passed since the beginning of the video. Don't need to keep this file here, the postevent will take care of it
                        {
                            startVideoForFullFile = "";
                        }
                    }
                }
                catch (Exception e) // no file to cut
                {
                    startVideoForFullFile = "";
                }

                if (fileSaved == false && listRecordingFiles.Count != 0)
                {
                    for (int i = 1; i <= listRecordingFiles.Count; i++)
                    {
                        videoInList = listRecordingFiles.ElementAt(i - 1);
                        dateCutVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateCutVideo = new DateTime(Convert.ToInt32(dateCutVideoString.Substring(0, 4)), Convert.ToInt32(dateCutVideoString.Substring(4, 2)), Convert.ToInt32(dateCutVideoString.Substring(6, 2)), Convert.ToInt32(dateCutVideoString.Substring(8, 2)), Convert.ToInt32(dateCutVideoString.Substring(10, 2)), Convert.ToInt32(dateCutVideoString.Substring(12, 2)));
                        compareDateWithEvent = DateTime.Compare(dateCutVideo, dateEventStartTime);
                        compareDateWithFilePreevent = DateTime.Compare(dateCutVideo, (listTask[listTaskIndex].eventtime - listTask[listTaskIndex].starttime));

                        if (compareDateWithEvent < 0 && compareDateWithFilePreevent > 0) // the date of the saved file is > to the event start time. We keep only files after event started
                        {
                            if (i != listRecordingFiles.Count) // Not last file
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
                                timeToCutLastVideo = (listTask[listTaskIndex].stoptime.Minutes * 60 + listTask[listTaskIndex].stoptime.Seconds) - ((dateCutVideo - listTask[listTaskIndex].eventtime).Minutes * 60 + (dateCutVideo - listTask[listTaskIndex].eventtime).Seconds);
                                cutFile = CutVideoKeepStart(listRecordingFiles.ElementAt(i - 1), timeToCutLastVideo);
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
                        else if (compareDateWithFilePreevent > 0) // We finished to look for files before the event. Here are the files after the event, but different with the files we have already kept
                        {
                            if (i != listRecordingFiles.Count) // Not the last file
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
                                timeToCutLastVideo = (listTask[listTaskIndex].stoptime.Minutes * 60 + listTask[listTaskIndex].stoptime.Seconds) - ((dateCutVideo - listTask[listTaskIndex].eventtime).Minutes * 60 + (dateCutVideo - listTask[listTaskIndex].eventtime).Seconds);
                                cutFile = CutVideoKeepStart(listRecordingFiles.ElementAt(i - 1), timeToCutLastVideo);
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

                if (fileSaved == false) // The file isn't saved yet  (Because the function CutVideoFromEvent() saved it directly, so don't need to pass in the Concat() function in that case)
                {
                    System.Threading.Thread.Sleep(10000); // Wait for the last files is released by the system. (On PC short time is enough but it touch panel it is better to wait more to be sure)
                    ConcatVideo(startVideoForFullFile, posteventFilesName, startEventTime, listTaskIndex); // Concat all path files to send to ffmpeg to make the final video with all cuts
                }

                fileSaved = false;
                listTask[listTaskIndex].complete = true;
                //listTask.Remove(listTask[listTaskIndex]); // Delete the task ended --> If we delete the task, the index of the list change. So if a task is running, it will become a problem
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " TaskManager in RecordEnd()");
                LOGGER.Add(ex.Message + " TaskManager in RecordEnd()");
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
                int iduration = dateDuration.Minute * 60 + dateDuration.Second;
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
                videoCutName = path + @"CutTemp\" + videoStartTime + ".avi";

                ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191101-53c21c2-win32-static\bin\ffmpeg.exe");
                startInfo.Arguments = @"-loglevel quiet -y -i " + videoToCut + " -ss " + videoCutStartTimeFormated + " -to 0" + videoDuration + " -c copy -avoid_negative_ts 1 " + videoCutName;
                Console.WriteLine("CutVideoKeepEnd() : " + startInfo.Arguments); // DEBUG
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo);
            }
            catch (Exception ex) // No video to cut. The application has probably just started
            {
                Console.WriteLine(ex.Message + " TaskManager in CutVideoKeepEnd()");
                LOGGER.Add(ex.Message + " TaskManager in CutVideoKeepEnd()");
                videoCutName = "";
            }
            return videoCutName;
        }

        private static string CutVideoFromEvent(string videoToCut, TimeSpan tsEventTime, int cutTimeBeforeParameter, int cutTimeAfterParameter, int taskIndex) // Cut the video we need for the beginning of the full video keeping just the part we need from the event
        {
            string videoCutStartTimeFormated, videoCutEndTimeFormated, videoCutName, videoStartTime, videoDate, path;
            int videoCutStartTime, videoCutEndTime;
            TimeSpan tsCutBefore = new TimeSpan(0, 0, 0, cutTimeBeforeParameter);

            try
            {
                path = videoToCut.Substring(0, videoToCut.Length - 18);
                videoCutStartTime = tsEventTime.Minutes * 60 + tsEventTime.Seconds - cutTimeBeforeParameter;
                if ((videoCutStartTime % 60) >= 10)
                {
                    videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":" + videoCutStartTime % 60;
                }
                else
                {
                    videoCutStartTimeFormated = "00:0" + videoCutStartTime / 60 + ":0" + videoCutStartTime % 60;
                }

                videoCutEndTime = tsEventTime.Minutes * 60 + tsEventTime.Seconds + cutTimeAfterParameter;
                if ((videoCutEndTime % 60) >= 10)
                {
                    videoCutEndTimeFormated = "00:0" + videoCutEndTime / 60 + ":" + videoCutEndTime % 60;
                }
                else
                {
                    videoCutEndTimeFormated = "00:0" + videoCutEndTime / 60 + ":0" + videoCutEndTime % 60;
                }

                // Create name for the cut video
                videoDate = videoToCut.Substring(videoToCut.Length - 18, 14);
                videoStartTime = (new DateTime(Convert.ToInt32(videoDate.Substring(0, 4)), Convert.ToInt32(videoDate.Substring(4, 2)), Convert.ToInt32(videoDate.Substring(6, 2)), Convert.ToInt32(videoDate.Substring(8, 2)), Convert.ToInt32(videoDate.Substring(10, 2)), Convert.ToInt32(videoDate.Substring(12, 2))) + tsEventTime - tsCutBefore).ToString("yyyyMMddHHmmss");
                videoCutName = path + videoStartTime + ".avi";

                ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191101-53c21c2-win32-static\bin\ffmpeg.exe");
                startInfo.Arguments = @"-loglevel quiet -y -i " + videoToCut + " -ss " + videoCutStartTimeFormated + " -to " + videoCutEndTimeFormated + " -c copy -avoid_negative_ts 1 " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[taskIndex].cameraNumber.ToString() + "\\" + listTask[taskIndex].path + "\\" + videoStartTime + ".avi";

                System.Threading.Thread.Sleep(BUFFERDURATION - (cutTimeAfterParameter * 1000)); // Wait for the last files is released by the system -> buffer file - time after event. we can't know better...
                Console.WriteLine("CutVideoFromEvent() : " + startInfo.Arguments); // DEBUG
                LOGGER.Add("CutVideoFromEvent() : " + startInfo.Arguments);
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo); // --/!\-- We send the file directly in the final destination
            }
            catch (Exception ex) // No video to cut. The application has probably just started
            {
                Console.WriteLine(ex.Message + " TaskManager in CutVideoFromEvent()");
                LOGGER.Add(ex.Message + " TaskManager in CutVideoFromEvent()");
                videoCutName = "";
            }
            return videoCutName;
        }

        private static string CutVideoKeepStart(string videoToCut, int cutTimeParameter) // Cut the video we need for the end of the full video keeping just the beginning of the file
        {
            string videoCutEndTimeFormated, videoDate, videoEndTime, videoCutName;
            TimeSpan tsCut = new TimeSpan(0, 0, 0, 1); // Can't use the same name for the cut file and the original file, so just add 1 second in the name
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
            videoCutName = path + @"CutTemp\" + videoEndTime + ".avi";

            ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191101-53c21c2-win32-static\bin\ffmpeg.exe");
            startInfo.Arguments = @"-loglevel quiet -y -i " + videoToCut + " -ss 00:00:00 -to " + videoCutEndTimeFormated + " -c copy -avoid_negative_ts 1 " + videoCutName;
            System.Threading.Thread.Sleep(BUFFERDURATION - (cutTimeParameter - 100)); // Wait for the last files is released by the system -> buffer file - time we keep, that is already passed since the file started (+100ms to be sur the fil is released). 
            Console.WriteLine("CutVideoKeepStart() : " + startInfo.Arguments); // DEBUG
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo);

            return videoCutName;
        }

        private static void ConcatVideo(string preEventVideoFiles, string postEventVideoFiles, string startTime, int TaskIndex) // Concat all videos needed to construct the full video
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(directory + @"\ffmpeg-20191101-53c21c2-win32-static\bin\ffmpeg.exe");
                if (preEventVideoFiles != "" && postEventVideoFiles != "")
                {
                    startInfo.Arguments = @"-loglevel quiet -y -i concat:" + preEventVideoFiles + "|" + postEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[TaskIndex].cameraNumber.ToString() + "\\" + listTask[TaskIndex].path + "\\" + startTime + ".avi";
                }
                else if (preEventVideoFiles != "" && postEventVideoFiles == "") // All the video stream kept are inside the same file
                {
                    //startInfo.Arguments = @"-i concat:" + preEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[TaskIndex].cameraNumber.ToString() + "\\" + listTask[TaskIndex].path + "\\" + startTime + ".avi";
                    // --/!\-- Sent directly in the function CutVideoFromEvent()
                    return;
                }
                else // No file to keep before the event
                {
                    startInfo.Arguments = @"-loglevel quiet -y -i concat:" + postEventVideoFiles + " -c copy " + Properties.Settings.Default.video_file_location + "\\Camera\\" + listTask[TaskIndex].cameraNumber.ToString() + "\\" + listTask[TaskIndex].path + "\\" + startTime + ".avi";
                }
                Console.WriteLine("ConcatVideo() : " + startInfo.Arguments);
                LOGGER.Add("ConcatVideo() : " + startInfo.Arguments);
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(startInfo);

                Console.WriteLine("preEventVideoFiles: " + preEventVideoFiles);
                Console.WriteLine("postEventVideoFiles " + postEventVideoFiles);
                LOGGER.Add("preEventVideoFiles: " + preEventVideoFiles);
                LOGGER.Add("postEventVideoFiles " + postEventVideoFiles);
                if (preEventVideoFiles != "")
                {
                    DeleteCutFileFromTemp(preEventVideoFiles, listTask[TaskIndex].cameraNumber); // Delete file cut for the first part of the full video to not taking it in the next event 
                }
                DeleteCutFileFromTemp(postEventVideoFiles, listTask[TaskIndex].cameraNumber); // Delete file cut for the last part of the full video to not taking it in the next event 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " TaskManager in Concat()  pre: " + preEventVideoFiles + " post: " + postEventVideoFiles);
                LOGGER.Add(ex.Message + " TaskManager in Concat()  pre: " + preEventVideoFiles + " post: " + postEventVideoFiles);
            }
        }

        //private static void AddFilesInList() // Add all files in the TEMP folder into the list
        //{
        //    try
        //    {
        //        if (Directory.Exists(@"D:\TEMP"))
        //        {
        //            //DeleteOldFiles(@"D:\TEMP");
        //            string[] list = Directory.GetFiles(@"D:\TEMP");
        //            listRecordingFiles = list.ToList();
        //        }
        //        else
        //        {
        //            //DeleteOldFiles(@"C:\TEMP");
        //            string[] list = Directory.GetFiles(@"C:\TEMP");
        //            listRecordingFiles = list.ToList();
        //        }
        //    }
        //    catch (IOException e)
        //    {
        //        Console.WriteLine(e.Message + " TaskManager in AddFilesInList()");
        //    }
        //}

        private static void RefreshFilesInList(int numCamera) // Add all files in the TEMP folder into the list
        {
            try
            {
                if (Directory.Exists(@"D:\TEMP"))
                {
                    string[] list = Directory.GetFiles(@"D:\TEMP\" + numCamera);
                    listRecordingFiles = list.ToList();
                }
                else
                {
                    string[] list = Directory.GetFiles(@"C:\TEMP\" + numCamera);
                    listRecordingFiles = list.ToList();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message + " TaskManager in RefreshFilesInList()");
                LOGGER.Add(ex.Message + " TaskManager in RefreshFilesInList()");
            }
        }

        public static void DeleteOldFiles(string pathTempFolder) // Delete old files that we don't need anymore
        {
            DirectoryInfo dir;
            string fileName;
            DateTime fileDate;
            TimeSpan tsTimeMaxToKeep = new TimeSpan(0, 0, 6, 0);

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
                        File.SetAttributes(files.FullName, FileAttributes.Normal); // Add in case of weird attribute on the file
                        File.Delete(files.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " TaskManager in DeleteOldFiles()" + pathTempFolder);
                LOGGER.Add(ex.Message + " TaskManager in DeleteOldFiles()" + pathTempFolder);
            }
        }

        private static string GetVideoDuration(string videoPath) // Get the duration of the video file gave in parameter
        {
            string cmd = string.Format("-loglevel quiet -v error -select_streams v:0 -show_entries stream=duration -sexagesimal -of default=noprint_wrappers=1:nokey=1  {0}", videoPath);
            Process proc = new Process();
            proc.StartInfo.FileName = directory + @"\ffmpeg-20191101-53c21c2-win32-static\bin\ffprobe.exe";
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
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

        private static void DeleteCutFileFromTemp(string videoFiles, int numCamera) // Delete files in cutTemp folder after using
        {
            System.Threading.Thread.Sleep(30000);

            try
            {
                string file = videoFiles.Substring(videoFiles.Length - 18, 18);
                if (Directory.Exists(@"D:\TEMP"))
                {
                    File.Delete(@"D:\TEMP\" + numCamera + @"\CutTemp\" + file);
                }
                else
                {
                    File.Delete(@"C:\TEMP\" + numCamera + @"\CutTemp\" + file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " TaskManager in DeleteCutFileFromTemp()" + videoFiles);
                LOGGER.Add(ex.Message + " TaskManager in DeleteCutFileFromTemp()" + videoFiles);
            }
        }

        private static void OnTimerEvent(object sender, EventArgs e) // The event task is finished
        {
            RecordEnd();
        }
    }

    class TaskItem
    {
        public TimeSpan starttime;
        public TimeSpan stoptime;
        public DateTime eventtime;
        public bool complete;
        public string path;
        public string allPreEventVideo = "";
        public int cameraNumber;

        public TaskItem(TimeSpan starttime, TimeSpan stoptime, DateTime eventtime, bool complete, string path, int cameraNumber)
        {
            this.path = path;
            this.starttime = starttime;
            this.stoptime = stoptime;
            this.eventtime = eventtime;
            this.complete = complete;
            this.cameraNumber = cameraNumber;
        }

        public void EditPath(string path)
        {
            allPreEventVideo = path;
        }
    }
}
