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
        CameraSettingsForm cameraSettingsForm;

        DsDevice[] capDevices;
        string[] camera_names;
        
       
        public settingsUI()
        {
            InitializeComponent();

            this.Width = 550;

            this.ControlBox = false;

            cameraSettingsForm = new CameraSettingsForm();
            
            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            camera_names = new string[capDevices.Length];
            for (int i = 0; i < capDevices.Length; i++)
            {
                camera_names[i] = capDevices[i].Name;
            }
            
            cm_camera_number.Items.AddRange(camera_names);
            

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
            Properties.Camera1.Default.Save();
            Properties.Camera2.Default.Save();
            Properties.Camera3.Default.Save();
            Properties.Camera4.Default.Save();


            MainForm.FormChangesApply();
            this.Hide();
        }

        private void applyChanges(object sender, EventArgs e)
        {
            /*
             Here handle all immediate changes
             */

            Properties.Settings.Default.Save();
            Properties.Camera1.Default.Save();
            Properties.Camera2.Default.Save();
            Properties.Camera3.Default.Save();
            Properties.Camera4.Default.Save();


            MainForm.FormChangesApply();
        }

        
                
        private void cameraSelected(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox) sender;
            Debug.WriteLine(comboBox.SelectedIndex);
            numericUpDownX.DataBindings.Clear();
            numericUpDownY.DataBindings.Clear();
            numericUpDownW.DataBindings.Clear();
            numericUpDownH.DataBindings.Clear();
            numericUpDownF.DataBindings.Clear();
            switch (comboBox.SelectedIndex)
            {
                
                case 0:
                    //Debug.WriteLine("Not firing");
                    
                    numericUpDownX.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera1.Default, "pos_x", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownY.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera1.Default, "pos_y", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownW.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera1.Default, "view_width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownH.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera1.Default, "view_height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownF.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera1.Default, "frame_rate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                    break;
                case 1:

                    numericUpDownX.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera2.Default, "pos_x", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownY.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera2.Default, "pos_y", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownW.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera2.Default, "view_width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownH.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera2.Default, "view_height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownF.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera2.Default, "frame_rate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));


                    break;
                case 2:
                    numericUpDownX.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera3.Default, "pos_x", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownY.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera3.Default, "pos_y", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownW.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera3.Default, "view_width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownH.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera3.Default, "view_height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownF.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera3.Default, "frame_rate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));


                    break;
                case 3:
                    numericUpDownX.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera4.Default, "pos_x", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownY.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera4.Default, "pos_y", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownW.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera4.Default, "view_width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownH.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera4.Default, "view_height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                    numericUpDownF.DataBindings.Add(new System.Windows.Forms.Binding("Value", Properties.Camera4.Default, "frame_rate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));


                    break;            }
            //
        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            cm_camera_number.SelectedIndex = 0;
            cm_capture_mode.SelectedIndex = 0;
            cm_language.SelectedItem = Properties.Settings.Default.language;

            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);

            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
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

        private void Button_capture_settings_Click(object sender, EventArgs e)
        {
            //Call the general settings window
            if (cameraSettingsForm.Visible == false)
            {
                cameraSettingsForm.TopMost = true;
                this.TopMost = false;
                Debug.WriteLine("topmost 414");
                cameraSettingsForm.Show();
            }
        }
    }
}
