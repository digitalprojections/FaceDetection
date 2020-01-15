using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetection
{
    class SNAPSHOT_SAVER
    {
        private delegate void dTakeAsyncSnapShot();
        static int CameraIndex = 0;
        static ImageCodecInfo myImageCodecInfo;
        static Encoder myEncoder;
        static EncoderParameter myEncoderParameter;
        static EncoderParameters myEncoderParameters;
        

        internal static void TakeSnapShot(int cameraIndex)
        {
            CameraIndex = cameraIndex;
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
            picloc = Path.Combine(picloc, (CameraIndex + 1).ToString());
            picloc = Path.Combine(picloc, "snapshot");
            Directory.CreateDirectory(picloc);
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            Bitmap bitmap = new Bitmap(1, 1);
            try
            {
                
                bitmap = MULTI_WINDOW.formList[cameraIndex].GetSnapShot();
                
                if (bitmap != null)
                {
                    bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
                }
            }
            catch (NullReferenceException nrx)
            {
                Logger.Add(nrx);
            }
        }

        internal static void TakeSnapShot(int cameraIndex, string ev)
        {
            CameraIndex = cameraIndex;

            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
            picloc = Path.Combine(picloc, (CameraIndex + 1).ToString());
            picloc = Path.Combine(picloc, ev);
            Directory.CreateDirectory(picloc);
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            Bitmap bitmap = new Bitmap(1, 1);
            try
            {
                bitmap = MULTI_WINDOW.formList[cameraIndex].GetSnapShot();

                if (bitmap != null)
                {
                    bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
                }
            }
            catch (NullReferenceException nrx)
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

        public static void TakeAsyncSnapShot(bool allcam, int cameraIndex, string v)
        {
            Thread newThread = new Thread(() => ThreadProc(allcam, cameraIndex, v));
            newThread.Name = String.Format("Thread SNAPSHOT");
            newThread.IsBackground = true;
            newThread.Start();
            //ThreadProc();
        }

        private static void ThreadProc(bool allcam, int cameraIndex, string v)
        {
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            if (MainForm.GetMainForm.InvokeRequired)
            {                
                var d = new dTakeAsyncSnapShot(() => ThreadProc(allcam, cameraIndex, v));
                if(MainForm.GetMainForm!=null)
                    MainForm.GetMainForm.Invoke(d);
            }
            else {
                bool snap = false;
                while (snap == false)
                {
                    if (MainForm.GetMainForm != null && (cameraIndex >= 0 && cameraIndex < 4))
                    {
                        if (MULTI_WINDOW.formList[cameraIndex].crossbar != null) // && MainForm.GetMainForm.crossbar.ANY_CAMERA_ON())
                        {
                            //Thread.Sleep(1000);
                            
                            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
                            picloc = Path.Combine(picloc, (cameraIndex + 1).ToString());
                            picloc = Path.Combine(picloc, v);
                            Directory.CreateDirectory(picloc);
                            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");
                            
                            Bitmap bitmap = MULTI_WINDOW.formList[cameraIndex].crossbar.GetBitmap();
                            bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
                            
                            //TakeSnapShot(cameraIndex);
                            
                        }

                    }else if(cameraIndex==8)
                    {
                        Thread.Sleep(1000);
                        for(int i = 0; i<MULTI_WINDOW.displayedCameraCount;i++)
                        {
                            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "Camera");
                            picloc = Path.Combine(picloc, (i + 1).ToString());
                            picloc = Path.Combine(picloc, v);
                            Directory.CreateDirectory(picloc);
                            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

                            Bitmap bitmap = MULTI_WINDOW.formList[i].crossbar.GetBitmap();
                            bitmap.Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);
                        }                        
                    }

                    snap = true;
                    PARAMETERS.PARAM.Clear();
                }
                Console.WriteLine("Snapshot DONE! by " + Thread.CurrentThread.Name);
            }
        }

        internal static void TakeSnapShotAll()
        {
            for (int i=0; i<MULTI_WINDOW.displayedCameraCount;i++)
            {
                TakeSnapShot(i, "event");
            }
        }

        internal static void TakeAsyncSnapShotAll()
        {
            throw new NotImplementedException();
        }
    }
}
