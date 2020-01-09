using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    class RecIcon:PictureBox
    {
        private PictureBox rec_icon;

        public RecIcon()
        {
            rec_icon.Size = new Size(65, 65);
            rec_icon.Image = Properties.Resources.player_record;
            rec_icon.SizeMode = PictureBoxSizeMode.Zoom;
            rec_icon.Location = new Point(12, 12);
            rec_icon.BackColor = Color.Transparent;
            rec_icon.Visible = false;
        }
    }
}
