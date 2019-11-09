using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetection
{
    class SNAPSHOT_SAVER
    {
        static int CameraIndex = 0;
        private static Mutex mutex = new Mutex();
        static ImageCodecInfo myImageCodecInfo;
        static Encoder myEncoder;
        static EncoderParameter myEncoderParameter;
        static EncoderParameters myEncoderParameters;
        internal static void TakeSnapShot(int cameraIndex)
        {
            CameraIndex = cameraIndex;
            //set it, but not use it here
            
            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with quality level 80.
            myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;


            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
            picloc = Path.Combine(picloc, (CameraIndex + 1).ToString());
            picloc = Path.Combine(picloc, "snapshot");
            Directory.CreateDirectory(picloc);
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            //two versions here. Depending on camera mode

            //pic from regular camera
            Bitmap bitmap = new Bitmap(1, 1);
            try
            {
                bitmap = MainForm.GetMainForm.GetSnapShot();
                if (bitmap != null)
                    bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
            }catch(NullReferenceException nrx)
            {
                Logger.Add(nrx);
            }
            


        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static void TakeAsyncSnapShot(int cameraIndex)
        {
            Thread newThread = new Thread(new ThreadStart(ThreadProc));
            newThread.Name = String.Format("Thread SNAPSHOT");
            newThread.Start();
        }
        private static void ThreadProc()
        {
            mutex.WaitOne();
            bool snap = false;
            while (snap == false)
            {
                if (MainForm.GetMainForm != null)
                {
                    if (MainForm.GetMainForm.crossbar != null && MainForm.GetMainForm.crossbar.ANY_CAMERA_ON())
                    {
                        Thread.Sleep(1000);
                        string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
                        picloc = Path.Combine(picloc, (CameraIndex + 1).ToString());
                        picloc = Path.Combine(picloc, "snapshot");
                        Directory.CreateDirectory(picloc);
                        var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");                        
                        Bitmap bitmap = MainForm.GetMainForm.crossbar.GetBitmap();

                        bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
                        snap = true;

                    }

                }
            }
            Console.WriteLine("Snapshot DONE! by " + Thread.CurrentThread.Name);
            mutex.ReleaseMutex();
        }
    }
}
