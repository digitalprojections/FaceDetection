using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    class CameraNumberLabel : Label
    {
        private Label camera_number;

        public CameraNumberLabel(int camera_index)
        {
            camera_number = this;

            camera_number.Size = new Size(65, 65);
            camera_number.UseWaitCursor = false;
            camera_number.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            camera_number.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, "show_camera_no", true, DataSourceUpdateMode.OnPropertyChanged));
            camera_number.Text = (camera_index + 1).ToString();
            camera_number.Font = new Font("MS UI Gothic", 50);
            camera_number.TextAlign = ContentAlignment.MiddleCenter;
            camera_number.Location = new Point(this.Width - 89, 12);            
        }
    }
}
