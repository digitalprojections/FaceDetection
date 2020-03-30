using System;
using System.Management;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace FaceDetection
{
    class USBManager
    {
        BackgroundWorker bgwDriveDetector = new BackgroundWorker();
        bool driveDConnected;
        bool messageDiplayed;

        public USBManager()
        {
            bgwDriveDetector.DoWork += bgwDriveDetector_DoWork;
            bgwDriveDetector.RunWorkerAsync();
        }

        void bgwDriveDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            var insertQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            var insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += DeviceInsertedEvent;
            insertWatcher.Start();

            var removeQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            var removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += DeviceRemovedEvent;
            removeWatcher.Start();
        }

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == @"D:\")
                {
                    driveDConnected = true;
                    messageDiplayed = false;
                    break;
                }
            }
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            string saveDisk;

            driveDConnected = false;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == @"D:\")
                {
                    driveDConnected = true;
                    break;
                }
            }

            if (driveDConnected == false && messageDiplayed == false)
            {
                LOGGER.Add(@"D:\ is removed");
                saveDisk = Properties.Settings.Default.video_file_location.Substring(0, 2);
                if (saveDisk == @"D:")
                {
                    messageDiplayed = true;
                    MessageBox.Show(Resource.save_path_not_exist);
                }
            }
        }
    }
}
