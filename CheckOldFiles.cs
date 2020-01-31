using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FaceDetection
{
    class CheckOldFiles
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(string lpRootPathName,
            out uint lpSectorsPerCluster,
            out uint lpBytesPerSector,
            out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        private static TimeSpan maxSavedDate;
        private static List<string> listRecordingEventFiles1 = new List<string>();
        private static List<string> listRecordingMovieFiles1 = new List<string>();
        private static List<string> listRecordingSnapshotFiles1 = new List<string>();
        private static List<string> listRecordingEventFiles2 = new List<string>();
        private static List<string> listRecordingMovieFiles2 = new List<string>();
        private static List<string> listRecordingSnapshotFiles2 = new List<string>();
        private static List<string> listRecordingEventFiles3 = new List<string>();
        private static List<string> listRecordingMovieFiles3 = new List<string>();
        private static List<string> listRecordingSnapshotFiles3 = new List<string>();
        private static List<string> listRecordingEventFiles4 = new List<string>();
        private static List<string> listRecordingMovieFiles4 = new List<string>();
        private static List<string> listRecordingSnapshotFiles4 = new List<string>();
        private static string[] listEventFiles;
        private static string[] listMovieFiles;
        private static string[] listSnapshotFiles;

        public CheckOldFiles() { }

        /// Delete too old files (according to parameters)
        public static void DeleteOldFiles()
        {
            string videoInList, dateVideoString;
            int dateFileTooOld;
            DateTime dateVideo;
            DateTime now = DateTime.Now;

            maxSavedDate = new TimeSpan(decimal.ToInt32(Properties.Settings.Default.keep_old_files_days), 0, 0, 0);
            AddFilesToList();

            try
            {
                // event folder
                if (listRecordingEventFiles1.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingEventFiles1.Count; i > 0; i--)
                    {
                        videoInList = listRecordingEventFiles1.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingEventFiles1.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingEventFiles1.ElementAt(i - 1));
                        }
                    }
                }
                // movie folder
                if (listRecordingMovieFiles1.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingMovieFiles1.Count; i > 0; i--)
                    {
                        videoInList = listRecordingMovieFiles1.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingMovieFiles1.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingMovieFiles1.ElementAt(i - 1));
                        }
                    }
                } 
                // snapshot folder
                if (listRecordingSnapshotFiles1.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingSnapshotFiles1.Count; i > 0; i--)
                    {
                        videoInList = listRecordingSnapshotFiles1.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingSnapshotFiles1.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingSnapshotFiles1.ElementAt(i - 1));
                        }
                    }
                }

                // event folder
                if (listRecordingEventFiles2.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingEventFiles2.Count; i > 0; i--)
                    {
                        videoInList = listRecordingEventFiles2.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingEventFiles2.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingEventFiles2.ElementAt(i - 1));
                        }
                    }
                }
                // movie folder
                if (listRecordingMovieFiles2.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingMovieFiles2.Count; i > 0; i--)
                    {
                        videoInList = listRecordingMovieFiles2.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingMovieFiles2.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingMovieFiles2.ElementAt(i - 1));
                        }
                    }
                }
                // snapshot folder
                if (listRecordingSnapshotFiles2.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingSnapshotFiles2.Count; i > 0; i--)
                    {
                        videoInList = listRecordingSnapshotFiles2.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingSnapshotFiles2.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingSnapshotFiles2.ElementAt(i - 1));
                        }
                    }
                }

                // event folder
                if (listRecordingEventFiles3.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingEventFiles3.Count; i > 0; i--)
                    {
                        videoInList = listRecordingEventFiles3.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingEventFiles3.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingEventFiles3.ElementAt(i - 1));
                        }
                    }
                }
                // movie folder
                if (listRecordingMovieFiles3.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingMovieFiles3.Count; i > 0; i--)
                    {
                        videoInList = listRecordingMovieFiles3.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingMovieFiles3.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingMovieFiles3.ElementAt(i - 1));
                        }
                    }
                }
                // snapshot folder
                if (listRecordingSnapshotFiles3.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingSnapshotFiles3.Count; i > 0; i--)
                    {
                        videoInList = listRecordingSnapshotFiles3.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingSnapshotFiles3.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingSnapshotFiles3.ElementAt(i - 1));
                        }
                    }
                }

                // event folder
                if (listRecordingEventFiles4.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingEventFiles4.Count; i > 0; i--)
                    {
                        videoInList = listRecordingEventFiles4.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingEventFiles4.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingEventFiles4.ElementAt(i - 1));
                        }
                    }
                }
                // movie folder
                if (listRecordingMovieFiles4.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingMovieFiles4.Count; i > 0; i--)
                    {
                        videoInList = listRecordingMovieFiles4.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingMovieFiles4.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingMovieFiles4.ElementAt(i - 1));
                        }
                    }
                }
                // snapshot folder
                if (listRecordingSnapshotFiles4.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingSnapshotFiles4.Count; i > 0; i--)
                    {
                        videoInList = listRecordingSnapshotFiles4.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingSnapshotFiles4.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingSnapshotFiles4.ElementAt(i - 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AddFilesToList()
        {
            try
            {
                if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\1"))
                {
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\1\\event"))
                    {
                        listEventFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\event\\");
                        listRecordingEventFiles1 = listEventFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\1\\movie"))
                    {
                        listMovieFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\movie\\");
                        listRecordingMovieFiles1 = listMovieFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\1\\snapshot"))
                    {
                        listSnapshotFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\snapshot\\");
                        listRecordingSnapshotFiles1 = listSnapshotFiles.ToList();
                    }
                }

                if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\2"))
                {
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\2\\event"))
                    {
                        listEventFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\2\\event\\");
                        listRecordingEventFiles2 = listEventFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\2\\movie"))
                    {
                        listMovieFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\2\\movie\\");
                        listRecordingMovieFiles2 = listMovieFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\2\\snapshot"))
                    {
                        listSnapshotFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\2\\snapshot\\");
                        listRecordingSnapshotFiles2 = listSnapshotFiles.ToList();
                    }
                }

                if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\3"))
                {
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\3\\event"))
                    {
                        listEventFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\3\\event\\");
                        listRecordingEventFiles3 = listEventFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\3\\movie"))
                    {
                        listMovieFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\3\\movie\\");
                        listRecordingMovieFiles3 = listMovieFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\3\\snapshot"))
                    {
                        listSnapshotFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\3\\snapshot\\");
                        listRecordingSnapshotFiles3 = listSnapshotFiles.ToList();
                    }
                }

                if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\4"))
                {
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\4\\event"))
                    {
                        listEventFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\4\\event\\");
                        listRecordingEventFiles4 = listEventFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\4\\movie"))
                    {
                        listMovieFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\4\\movie\\");
                        listRecordingMovieFiles4 = listMovieFiles.ToList();
                    }
                    if (Directory.Exists(Properties.Settings.Default.video_file_location + "\\Camera\\4\\snapshot"))
                    {
                        listSnapshotFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\4\\snapshot\\");
                        listRecordingSnapshotFiles4 = listSnapshotFiles.ToList();
                    }
                }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
    }
}
