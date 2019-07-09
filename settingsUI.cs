using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FaceDetection
{
    public partial class settingsUI : Form
    {
        public settingsUI()
        {
            InitializeComponent();
            cm_language.SelectedIndex = 0;
            tb_camera_count.Text = Properties.Settings.Default.camera_count.ToString();
            cb_all_cameras.Checked = Properties.Settings.Default.show_all_cams_simulteneously;
            cb_delete_old.Checked = Properties.Settings.Default.show_all_cams_simulteneously;
            tb_days_old.Text = Properties.Settings.Default.erase_after.ToString();
            cm_camera_number.SelectedIndex = Properties.Settings.Default.current_camera_index;
        }

        private void GroupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox11_Enter(object sender, EventArgs e)
        {

        }
    }
}
