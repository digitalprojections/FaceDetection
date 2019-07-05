using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Diagnostics;


namespace FaceDetection
{
    public partial class Form1 : Form
    {
       
        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;
        static Timer myTimer = new Timer();
        
        public Form1()
        {
            InitializeComponent();
            _capture = new VideoCapture();
            myTimer.Tick += new EventHandler(timer1_Tick);

            // Sets the timer interval to 5 seconds.
            myTimer.Interval = 1000;
            myTimer.Start();
        }
        private void timer1_Tick(object sender, EventArgs eventArgs)
        {
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_alt2.xml");

            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray,byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                    foreach (var face in faces)
                    {
                        imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                    }
                }
                imgCamUser.Image = imageFrame;
                Debug.WriteLine(imageFrame);
            }
        }
    }
}
