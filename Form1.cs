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

        //private VideoCapture _capture;

        private CascadeClassifier _cascadeClassifier;
        static Timer myTimer = new Timer();
        private Image<Bgr, Byte> img;
        private Image<Gray, Byte> gray;
        public Form1()
        {
            InitializeComponent();
            //_capture = new VideoCapture();
            
            

            myTimer.Tick += (object sender, EventArgs eventArgs) => {
                timer1_Tick();
            };

            // Sets the timer interval to 5 seconds.
            myTimer.Interval = 1000;
            myTimer.Start();
        }
        private void timer1_Tick()
        {
            img = new Image<Bgr, byte>(Application.StartupPath + "/input.jpeg");
            gray = img.Convert<Gray, Byte>();
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_frontalface_default.xml");

            var faces = _cascadeClassifier.DetectMultiScale(gray, 1.1, 10, Size.Empty); //the actual face detection happens here
            foreach (var face in faces)
            {
                img.Draw(face, new Bgr(Color.YellowGreen), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

            }

            imgCamUser.Image = img;
            
                Debug.WriteLine(img.Data.Length);
            }
            
    }
}
