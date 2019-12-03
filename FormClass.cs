using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class FormClass : Form
    {
        public Label camera_number;
        public int CAMERA_INDEX = 0;
        public CROSSBAR crossbar;

        public FormClass(int camind)
        {
            this.Text = "UVC Camera Viewer - camera " + (camind + 1); //counting from the second camera            
            CAMERA_INDEX = camind;
            camera_number = new Label();
            camera_number.Size = new Size(65, 65); camera_number.UseWaitCursor = false;
            camera_number.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            camera_number.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, "show_camera_no", true, DataSourceUpdateMode.OnPropertyChanged));
            Font font = new Font("MS UI Gothic", 50);
            camera_number.Text = (CAMERA_INDEX + 1).ToString();
            camera_number.Font = font;
            camera_number.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(camera_number);
            camera_number.Location = new Point(this.Width - 89, 12);

            this.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CAMERA_INDEX);
            this.Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);

            crossbar = new CROSSBAR(camind, this);
            crossbar.PreviewMode();

            this.FormClosed += FormClass_FormClosed;
            this.ResizeEnd += FormClass_ResizeEnd;
            this.DoubleClick += new EventHandler(this.FullScreen);
            this.SizeChanged += new System.EventHandler(this.WindowSizeUpdate);
        }

        private void FormClass_ResizeEnd(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Camera_Window_Size(CAMERA_INDEX, this);
            crossbar.SetWindowPosition(this.Size);
        }

        private void FormClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Window_Location(CAMERA_INDEX, this);
            Properties.Settings.Default.Save();
            MULTI_WINDOW.formList.Remove(this);
            crossbar.ReleaseSecondaryCamera();
        }

        private void FullScreen(object sender, EventArgs eventArgs)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void WindowSizeUpdate(object sender, EventArgs eventArgs)
        {
            crossbar.SetWindowPosition(this.Size);
        }
    }
}
