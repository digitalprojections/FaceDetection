using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
    }
}
