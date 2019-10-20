using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using DirectShowLib;


namespace FaceDetection
{
    public partial class settingsUI : Form
    {
        

        DsDevice[] capDevices;
        string[] camera_names;

        private int camera_index;
        private int camera_number;
        public int Camera_index { get { return camera_index = Properties.Settings.Default.current_camera_index; } private set { Properties.Settings.Default.current_camera_index = value; Properties.Settings.Default.Save(); } }
        public int Camera_number { get { return camera_number = Properties.Settings.Default.current_camera_index+1;  } }

        

        public settingsUI()
        {
            InitializeComponent();
            //Setup window
            this.Width = 1080;
            this.Height = 760;
            this.ControlBox = false;

            //get reference to the mainform
            
            
            Console.WriteLine(MainForm.GetCamera());

        }       
        private void ArrangeCameraNames(int len)
        {
            camera_names = new string[len];
            for (int i = 0; i < len; i++)
            {
                camera_names[i] = capDevices[i].Name;
                cm_camera_number.Items.Add(i+1);
            }

            
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
        private void closeSettings(object sender, EventArgs e)
        {
            /*CANCEL BUTTON
             * 変更内容は捨てられる
            */
            this.Hide();
        }

        private void save_and_close(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();            
            MainForm.FormChangesApply();
            this.Hide();
        }

        private void applyChanges(object sender, EventArgs e)
        {
            /*
             Here handle all immediate changes
             */
            Properties.Settings.Default.Save();            
            MainForm.FormChangesApply();
        }

        private void SetCameraPropertiesFromMemory()
        {   
            numericUpDownX.DataBindings.Clear();
            numericUpDownY.DataBindings.Clear();
            numericUpDownW.DataBindings.Clear();
            numericUpDownH.DataBindings.Clear();
            //comboBoxFPS.DataBindings.Clear();
            //comboBoxResolutions.DataBindings.Clear();
            string camX = "C" + camera_number + "x";
            string camY = "C" + camera_number + "y";
            string camW = "C" + camera_number + "w";
            string camH = "C" + camera_number + "h";
            string camF = "C" + camera_number + "f";
            string camRes = "C" + camera_number + "res";
            //numericUpDownF.DataBindings.Clear();
            numericUpDownX.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Settings.Default, camX, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownY.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Settings.Default, camY, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownW.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Settings.Default, camW, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownH.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Settings.Default, camH, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //comboBoxFPS.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camF, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //comboBoxResolutions.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camRes, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));


            comboBoxFrames = comboBoxFPS;
            
        }

        static ComboBox comboBoxFrames;

        public static void SetComboBoxValues(List<string> vs)
        {
            comboBoxFrames.DataSource = vs;
        }

        private void cameraSelected(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Debug.WriteLine(comboBox.SelectedIndex);
            camera_index = comboBox.SelectedIndex;
            camera_number = Int32.Parse(comboBox.SelectedItem.ToString());
            SetCameraPropertiesFromMemory();
        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            if (cm_camera_number.Items.Count>0)
            {
                cm_camera_number.SelectedIndex = camera_index;
                cm_capture_mode.SelectedIndex = camera_index;
            }
            SetCameraPropertiesFromMemory();

            cm_language.SelectedItem = Properties.Settings.Default.language;

            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);

            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            numericUpDownCamCount.Value = Properties.Settings.Default.camera_count;

                if (capDevices.Length > numericUpDownCamCount.Value)
                {
                    MessageBox.Show("The settings do not allow more than " + numericUpDownCamCount.Value + " cameras");
                    ArrangeCameraNames(Decimal.ToInt32(numericUpDownCamCount.Value));
                }
                else
                {
                    //settings are missing
                    if (capDevices.Length > 4)
                    {
                        ArrangeCameraNames(4);
                    }
                    else
                    {
                        ArrangeCameraNames(capDevices.Length);
                    }
                }
        }

        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {            
            ChangeLanguage();
        }

        private void ChangeLanguage()
        {
            if (cm_language.SelectedItem.ToString() == "English")
            {
                Properties.Settings.Default.culture = "en-US";
                Properties.Settings.Default.language = "English";
            }
            else
            {
                Properties.Settings.Default.culture = "ja-JP";
                Properties.Settings.Default.language = "日本語";
            }
            Properties.Settings.Default.Save();
            string lan = Properties.Settings.Default.culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(settingsUI));
            var cult = new CultureInfo(lan);
            
            foreach (Control c in this.Controls)
            {                
                resources.ApplyResources(c, c.Name, cult);
                Debug.WriteLine(c.GetType().ToString());
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    checkOnKids(cult, c, resources);
                }
            }
        }
        private void checkOnKids(CultureInfo cult, Control control, ComponentResourceManager crm)
        {
            foreach (Control c in control.Controls)
            {
                crm.ApplyResources(c, c.Name, cult);
                
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    checkOnKids(cult, c, crm);
                }
            }
        }

        private void Cm_capture_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.selectedCaptureMethod = cm_capture_mode.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void Button_cameraProperties_MouseClick(object sender, MouseEventArgs e)
        {
            //Show the property window for the selected camera

        }

        private void NumericUpDownCamCount_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.camera_count = numericUpDownCamCount.Value;
            Properties.Settings.Default.Save();
        }

        private void PictureBox_allcam_Click(object sender, EventArgs e)
        {
            pictureBox_allcam.Image = Properties.Resources.check;
        }
    }
}
