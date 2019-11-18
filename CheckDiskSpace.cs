using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FaceDetection
{
    class CheckDiskSpace
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(string lpRootPathName,
            out uint lpSectorsPerCluster,
            out uint lpBytesPerSector,
            out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        private static TimeSpan maxSavedDate;
        private static List<string> listRecordingEventFiles = new List<string>();
        private static List<string> listRecordingMovieFiles = new List<string>();
        private static List<string> listRecordingSnapshotFiles = new List<string>();
        private static string[] listEventFiles;
        private static string[] listMovieFiles;
        private static string[] listSnapshotFiles;

        public CheckDiskSpace() { }

        // Return the free space in the disk
        public static int CheckDisk()
        {
            string freeSpace;
            int returnValue = 0;

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                //Console.WriteLine("checking drives..." + drives.Length);

                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady == true)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(Math.Round(drive.AvailableFreeSpace / 1e+9, 0));
                        freeSpace = stringBuilder.ToString();
                        try
                        {
                            returnValue = int.Parse(freeSpace);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            returnValue = 0;
                        }
                    }
                }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
            return returnValue;
        }

        // Delete too old files (according to parameters)
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
                if (listRecordingEventFiles.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingEventFiles.Count; i > 0; i--)
                    {
                        videoInList = listRecordingEventFiles.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingEventFiles.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingEventFiles.ElementAt(i - 1));
                        }
                    }
                }

                // movie folder
                if (listRecordingMovieFiles.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingMovieFiles.Count; i > 0; i--)
                    {
                        videoInList = listRecordingMovieFiles.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingMovieFiles.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingMovieFiles.ElementAt(i - 1));
                        }
                    }
                }

                // snapshot folder
                if (listRecordingSnapshotFiles.Count != 0) // Check if there are files or not into the folder
                {
                    for (int i = listRecordingSnapshotFiles.Count; i > 0; i--)
                    {
                        videoInList = listRecordingSnapshotFiles.ElementAt(i - 1);
                        dateVideoString = videoInList.Substring(videoInList.Length - 18, 18);
                        dateVideo = new DateTime(Convert.ToInt32(dateVideoString.Substring(0, 4)), Convert.ToInt32(dateVideoString.Substring(4, 2)), Convert.ToInt32(dateVideoString.Substring(6, 2)), Convert.ToInt32(dateVideoString.Substring(8, 2)), Convert.ToInt32(dateVideoString.Substring(10, 2)), Convert.ToInt32(dateVideoString.Substring(12, 2)));
                        dateFileTooOld = DateTime.Compare(dateVideo, now - maxSavedDate);

                        if (dateFileTooOld < 0) // The file is older than the max keep date
                        {
                            File.SetAttributes(listRecordingSnapshotFiles.ElementAt(i - 1), FileAttributes.Normal); // Add in case of weird attribute on the file
                            File.Delete(listRecordingSnapshotFiles.ElementAt(i - 1));
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
            listEventFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\event\\");
            listRecordingEventFiles = listEventFiles.ToList();
            listMovieFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\movie\\");
            listRecordingMovieFiles = listMovieFiles.ToList();
            listSnapshotFiles = Directory.GetFiles(Properties.Settings.Default.video_file_location + "\\Camera\\1\\snapshot\\");
            listRecordingSnapshotFiles = listSnapshotFiles.ToList();
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
    }
}
