using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public partial class CameraSettingsForm : Form
    {
        public CameraSettingsForm()
        {
            InitializeComponent();
        }

        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void changeStoreLocation(object sender, EventArgs e)
        {
            folderBrowserDialogStoreFolder.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderBrowserDialogStoreFolder.ShowDialog();
            if (result == DialogResult.OK)
            {
                storePath.Text = folderBrowserDialogStoreFolder.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialogStoreFolder.RootFolder;
                Debug.WriteLine(storePath);
            }
        }

        private void CameraSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save settings
            Properties.Settings.Default.Save();
            MainForm.FormChangesApply();
        }
                
        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");

                Process.Start(Properties.Settings.Default.video_file_location);

            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }

        private void Button_apply_Click(object sender, EventArgs e)
        {
            //Save settings
            Properties.Settings.Default.Save();
            this.Hide();
        }

        private void Button_cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
