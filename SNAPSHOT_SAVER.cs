using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FaceDetection
{
    class SNAPSHOT_SAVER
    {

        internal static void TakeSnapShot()
        {
            //set it, but not use it here
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
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


            string picloc = Path.Combine(Properties.Settings.Default.video_file_location, "CAMERA");
            picloc = Path.Combine(picloc, "1");
            picloc = Path.Combine(picloc, "SNAPSHOT");
            Directory.CreateDirectory(picloc);
            var imgdate = DateTime.Now.ToString("yyyyMMddHHmmss");

            //two versions here. Depending on camera mode

            //pic from regular camera
            //MainForm.GetMainForm.GetSnapShot().Save(picloc + "/" + imgdate + ".jpg", myImageCodecInfo, myEncoderParameters);


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

    }
}
