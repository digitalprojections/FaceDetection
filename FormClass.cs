using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class FormClass:Form
    {
        public PictureBox rec_icon;
        public Label camera_number;
        public int CAMERA_INDEX = 0;
        public CROSSBAR crossbar;
        public FaceDetectorX faceDetector;

        public FormClass(int camind)
        {
            this.Text = "UVC Camera Viewer - camera " + (camind + 1);//counting from the second camera            
            CAMERA_INDEX = camind;
            rec_icon = new PictureBox();
            camera_number = new Label();

            rec_icon.Size = new Size(65, 65);
            camera_number.Size = new Size(65,65);

            rec_icon.Image = Properties.Resources.player_record;
            rec_icon.SizeMode = PictureBoxSizeMode.Zoom;

            camera_number.UseWaitCursor = false;
            camera_number.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            ////////VISIBILITY///////////
            rec_icon.Visible = true;//SET FALSE
            camera_number.Visible = true;
            string camnum = "C" + (CAMERA_INDEX + 1) + "_show_camera_number";
            camera_number.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, camnum, true, DataSourceUpdateMode.OnPropertyChanged));
            /////////////////////////////

            Font font = new Font("MS UI Gothic", 50);            
            camera_number.Text = (CAMERA_INDEX + 1).ToString();
            camera_number.Font = font;
            camera_number.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(rec_icon);
            this.Controls.Add(camera_number);
            
            this.FormClosed += FormClass_FormClosed;
            this.ResizeEnd += FormClass_ResizeEnd;

            this.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CAMERA_INDEX);
            this.Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);

            rec_icon.Location = new Point(12, 12);
            camera_number.Location = new Point(this.Width - 89, 12);

            //CROSSBAR
            crossbar = new CROSSBAR(camind, this);
            crossbar.Start(camind, CAMERA_MODES.PREVIEW);//or whatever mode is relevant as per settings

            //FACE DETECTOR
            faceDetector = new FaceDetectorX(this);
            faceDetector.StartFaceTimer();
        }
                
        private void FormClass_ResizeEnd(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Camera_Window_Size(CAMERA_INDEX, this);
            this.crossbar.SetWindowPosition(this.Size);
        }

        private void FormClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Window_Location(CAMERA_INDEX, this);
            Properties.Settings.Default.Save();
        }

        internal void SET_REC_ICON_STATE_ON_VIEW()
        {
            rec_icon.Visible = PROPERTY_FUNCTIONS.Get_Rec_Icon(CAMERA_INDEX);
        }

        internal void SET_REC_ICON_STATE_IN_PROPERTIES(bool val)
        {
            PROPERTY_FUNCTIONS.Set_Rec_Icon(CAMERA_INDEX, val);
        }

    }
}
