using System.Drawing;
using System.Windows.Forms;

namespace FaceDetection
{
    class DateTimeLabel:Label
    {
        Label dateTimeLabel;
        public DateTimeLabel(int camera_index)
        {
            dateTimeLabel = this;

            dateTimeLabel.Name = "dateTimeLabel";
            dateTimeLabel.Size = new Size(125, 30);
            dateTimeLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            dateTimeLabel.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, "show_current_datetime", true, DataSourceUpdateMode.OnPropertyChanged));
            dateTimeLabel.Text = "My Custom Date and time";
            dateTimeLabel.Font = new Font("MS UI Gothic", 14F);
            dateTimeLabel.AutoSize = true;
            dateTimeLabel.BackColor = Color.Black;
            dateTimeLabel.ForeColor = Color.White;
            dateTimeLabel.ImeMode = ImeMode.NoControl;
            dateTimeLabel.Padding = new Padding(3);
            dateTimeLabel.TabIndex = 13;
            dateTimeLabel.UseCompatibleTextRendering = true;
            dateTimeLabel.Location = new Point(12, PROPERTY_FUNCTIONS.Get_Camera_Window_Size(camera_index).Height - 80);            
        }
    }
}
