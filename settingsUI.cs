using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Environment;

namespace FaceDetection
{
    public partial class SettingsUI : Form
    {
        static ComboBox frame_rates_combo;
        static ComboBox resolutions_combo;
        static ComboBox selected_camera_combo;

        DsDevice[] capDevices;
        static string[] camera_names;

        public int Camera_index
        {
            get
            {
                return Properties.Settings.Default.main_camera_index;
            }
            private set
            {
                Properties.Settings.Default.main_camera_index = value;
                Properties.Settings.Default.Save();
            }
        }

        public SettingsUI()
        {
            InitializeComponent();
            Properties.Settings.Default.Reload();
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.temp_folder);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }
            catch (IOException iox)
            {
                //resetting the default folders to C
                //hoping it exists
                Properties.Settings.Default.video_file_location = @"C:\UVCCAMERA";
                Properties.Settings.Default.temp_folder = @"C:\TEMP";

                Directory.CreateDirectory(Properties.Settings.Default.temp_folder);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }

            //Setup window
            //Size size = new Size(0, 0);
            //size = GetWidth(Properties.Settings.Default.main_camera_index);

            this.ControlBox = false;
            selected_camera_combo = cm_camera_number;//that makes the camera the default, main camera aka camera 1
            frame_rates_combo = comboBoxFPS;
            resolutions_combo = comboBoxResolutions;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        public void ArrangeCameraNames(int len)
        {
            selected_camera_combo.Items.Clear();
            camera_names = new string[len];
            
            for (int i = 0; i < len; i++)
            {
                selected_camera_combo.Items.Add(i + 1);
            }
            if (Properties.Settings.Default.main_camera_index>=0 && selected_camera_combo.Items.Count >= Properties.Settings.Default.main_camera_index)
            {                  
                selected_camera_combo.SelectedIndex = Properties.Settings.Default.main_camera_index <=0 ? 0 : Properties.Settings.Default.main_camera_index;
            }
            else
            {
                Properties.Settings.Default.main_camera_index = 0;
            }
        }

        private void ChangeStoreLocation(object sender, EventArgs e)
        {
            folderBrowserDialogStoreFolder.ShowNewFolderButton = true;
            folderBrowserDialogStoreFolder.Description = Properties.Settings.Default.store_location_description;
            folderBrowserDialogStoreFolder.SelectedPath = Properties.Settings.Default.video_file_location;
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
            //MainForm.AllChangesApply(); --> Robin delete: Don't save parameters if click on "Cancel" button !
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                ChangeStoreLocation(sender, e);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }

        private void CloseSettings(object sender, EventArgs e)
        {           
            Hide();
            Properties.Settings.Default.Reload();  //Matsuura add 20191115            
        }

        private void Save_and_close(object sender, EventArgs e)
        {
            
            //PROPERTY_FUNCTIONS.SetCameraResolution(Camera_index);
            Properties.Settings.Default.Save();
            MainForm.AllChangesApply();

            MainForm.GetMainForm.Width = decimal.ToInt32(Properties.Settings.Default.C1w);
            MainForm.GetMainForm.Height = decimal.ToInt32(Properties.Settings.Default.C1h);
            Hide();
        }        

        private void SetCameraPropertiesFromMemory()
        {            
                numericUpDownX.DataBindings.Clear();
                numericUpDownY.DataBindings.Clear();
                numericUpDownW.DataBindings.Clear();
                numericUpDownH.DataBindings.Clear();
                comboBoxFPS.DataBindings.Clear();
                comboBoxResolutions.DataBindings.Clear();
                string camX = "C" + (Camera_index + 1) + "x";
                string camY = "C" + (Camera_index + 1) + "y";
                string camW = "C" + (Camera_index + 1) + "w";
                string camH = "C" + (Camera_index + 1) + "h";
                string camF = "C" + (Camera_index + 1) + "f";
                string camRes = "C" + (Camera_index + 1) + "res";

                numericUpDownX.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camX, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownY.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camY, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownW.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camW, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownH.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camH, true, DataSourceUpdateMode.OnPropertyChanged));
                comboBoxFPS.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camF, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                comboBoxResolutions.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camRes, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                
                numericUpDownX.Enabled = !Properties.Settings.Default.main_window_full_screen;
                numericUpDownY.Enabled = !Properties.Settings.Default.main_window_full_screen;
                numericUpDownW.Enabled = !Properties.Settings.Default.main_window_full_screen;
                numericUpDownH.Enabled = !Properties.Settings.Default.main_window_full_screen;                        
        }

        public static void SetComboBoxFPSValues(List<string> vs)
        {
            bool matching_fps_found = false;
            if (frame_rates_combo != null)
            {
                frame_rates_combo.Items.AddRange(vs.ToArray());
                for (int i=0; i<vs.Count;i++)
                {
                    if (vs[i] == Properties.Settings.Default.main_camera_fps)
                    {
                        matching_fps_found = true;
                    }
                }                
            }
            Properties.Settings.Default.main_camera_fps = matching_fps_found ? Properties.Settings.Default.main_camera_fps : vs[0];
        }

        /// <summary>
        /// The ComboBox class searches for the specified object by using the IndexOf method. 
        /// This method uses the Equals method to determine equality.
        /// </summary>
        /// <param name="vs"></param>
        public static void SetComboBoxResolutionValues(List<string> vs)
        {
            if (resolutions_combo != null)
            {
                resolutions_combo.Items.AddRange(vs.ToArray());
                if (resolutions_combo.Items.Count > 0)
                {
                    Console.WriteLine(Properties.Settings.Default.C1res);
                    resolutions_combo.SelectedItem = Properties.Settings.Default.C1res;
                }
            }
        }

        private void CameraSelected(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Camera_index = comboBox.SelectedIndex;
            Properties.Settings.Default.main_camera_index = Camera_index;
            SetCameraPropertiesFromMemory();
        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            if (cm_camera_number.Items.Count > 0)
            {
                cm_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index <=0?0: Properties.Settings.Default.main_camera_index;
                cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_method<=0 ? 0 : Properties.Settings.Default.capture_method;
            }
            //SetCameraPropertiesFromMemory();
            //if(cm_language.SelectedItem!=null)

            cm_language.SelectedItem = Properties.Settings.Default.language;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);
            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
            SetCameraPropertiesFromMemory();
            Camera.SetNumberOfCameras();
            ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.capture_operator);

            this.ControlBox = false;
            this.MaximizeBox = false;
        }
                 
        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cm_language.SelectedItem != null)
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
                ChangeLanguage();
            }
        }

        private void ChangeLanguage()
        {  
            string lan = Properties.Settings.Default.culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsUI));
            var cult = new CultureInfo(lan);

            foreach (Control c in this.Controls)
            {
                resources.ApplyResources(c, c.Name, cult);
                if (c.GetType().ToString() == "System.Windows.Forms.TabControl")
                {
                    CheckOnKids(cult, c, resources);
                }
            }
        }

        private void CheckOnKids(CultureInfo cult, Control control, ComponentResourceManager crm)
        {
            foreach (Control c in control.Controls)
            {
                crm.ApplyResources(c, c.Name, cult);
                {
                    CheckOnKids(cult, c, crm);
                }
            }
        }
        private void Cm_capture_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.capture_method = cm_capture_mode.SelectedIndex;
            Console.WriteLine(cm_capture_mode.SelectedIndex);
            if (cm_capture_mode.SelectedIndex == 1)
            {
                nud_seconds_after.Enabled = false;
                nud_seconds_before_event.Enabled = false;
                nud_reinitiation_interval.Enabled = false;
            }
            else if (cb_operator_capture.Checked == true)
            {
                if (cm_capture_mode.SelectedIndex == 0)
                {
                    nud_seconds_after.Enabled = true;
                    nud_seconds_before_event.Enabled = true;
                    nud_reinitiation_interval.Enabled = true;
                }
            }
        }

        private bool CheckOnKids(Control control, string type, PictureBox picbox)
        {
            CheckBox checkBox;
            bool ret = false;
            foreach (Control c in control.Controls)
            {
                if (c.GetType().ToString() == type && c.Tag == picbox.Tag)
                {
                    //we found the control and the tag we need
                    //set the values eg: CheckBox checked state                    
                    checkBox = c as CheckBox;
                    if (checkBox.Checked)
                    {
                        checkBox.Checked = false;
                        picbox.Image = check_state_images.Images[0];
                    }
                    else
                    {
                        checkBox.Checked = true;
                        picbox.Image = check_state_images.Images[1];
                    }
                    ret = checkBox.Checked;
                    Console.WriteLine(checkBox.Checked + " is +++++ " + checkBox.Text);
                }

                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    ret = CheckOnKids(c, type, picbox);
                }
            }
            return ret;
        }

        //private void SetCheckBoxState(CheckBox ChBox)
        //{
        //    if (ChBox.Checked == true)
        //    {
        //        ChBox.Checked = false;
        //    }
        //    else
        //    {
        //        ChBox.Checked = true;
        //    }
        //}

        private void Button_cameraProperties_Click(object sender, EventArgs e)
        {
            DisplayPropertyPage();
        }

        /// <summary>
        /// Displays a property page for a filter
        /// </summary>
        /// <param name="dev">The filter for which to display a property page</param>
        public void DisplayPropertyPage()
        {
            //Camera_index
            var dev = (IBaseFilter)DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, Camera_index);

            //Get the ISpecifyPropertyPages for the filter
            ISpecifyPropertyPages pProp = dev as ISpecifyPropertyPages;
            int hr = 0;

            if (pProp == null)
            {
                //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
                IAMVfwCompressDialogs compressDialog = dev as IAMVfwCompressDialogs;
                if (compressDialog != null)
                {

                    hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                return;
            }

            //Get the name of the filter from the FilterInfo struct
            FilterInfo filterInfo;
            hr = dev.QueryFilterInfo(out filterInfo);
            DsError.ThrowExceptionForHR(hr);

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            //Create and display the OlePropertyFrame
            object oDevice = (object)dev;
            hr = OleCreatePropertyFrame(this.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
            Marshal.ReleaseComObject(pProp);
            //Marshal.ReleaseComObject(filterInfo);
            Marshal.ReleaseComObject(dev);
        }

        private void ChangeControlEnabled(Control control, bool enabled)
        {
            foreach (Control c in control.Controls)
            {
                if (c!=cb_operator_capture)
                {
                    c.Enabled = enabled;                    
                }
            }

            if (!enabled)
            {
                Properties.Settings.Default.enable_Human_sensor = false;
                Properties.Settings.Default.enable_face_recognition = false;
                Properties.Settings.Default.Recording_when_at_the_start_of_operation = false;
            }
        }

        private void CheckBox_full_screen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;

            numericUpDownX.Enabled = !cbx.Checked;
            numericUpDownY.Enabled = !cbx.Checked;
            numericUpDownW.Enabled = !cbx.Checked;
            numericUpDownH.Enabled = !cbx.Checked;
        }

        private void Cb_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            bool chk = chb.Checked;            
            ChangeControlEnabled(this.groupBox_functionalitySettings, chk);
        }
        
        private void Cb_human_sensor_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if(check.Checked)
            {
                //cb_human_sensor.Checked = check.Checked;
                cb_face_recognition.Checked = !check.Checked;
            }
            else
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
            }
        }

        private void Cb_face_recognition_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
            {
                cb_human_sensor.Checked = !check.Checked;
                //cb_face_recognition.Checked = check.Checked;
            }
            else
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
            }
        }
        private void Cb_recording_operation_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
            {
                
            }
            else
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
            }
        }

        private void DisableOperatorCaptureCheckBox_ifNeeded()
        {
            if (Properties.Settings.Default.enable_Human_sensor || Properties.Settings.Default.enable_face_recognition || Properties.Settings.Default.Recording_when_at_the_start_of_operation)
            {                
                Properties.Settings.Default.capture_operator = true;
            }
            else
            {
                //All three are off. Disable
                Properties.Settings.Default.capture_operator = false;
            }
        }

        private void Cb_backlight_off_idling_CheckStateChanged(object sender, EventArgs e)
        {
            numericUpDownBacklight.Enabled = cb_backlight_off_idling.Checked;
        }
        
        //private void Nud_seconds_after_Click(object sender, EventArgs e)
        //{
        //    CustomMessage.Add(Properties.Settings.Default.seconds_after_event);
        //    CustomMessage.Add(Properties.Settings.Default.seconds_before_event);
        //    CustomMessage.Add(Properties.Settings.Default.event_record_time_before_event);
        //    CustomMessage.Add(Properties.Settings.Default.event_record_time_after_event);
        //    CustomMessage.Add(Properties.Settings.Default.manual_record_time);
        //    CustomMessage.Add(Properties.Settings.Default.interval_before_reinitiating_recording);
        //}

        

        private void Nud_reinitiation_interval_ValueChanged(object sender, EventArgs e)
        {
            //check the other nud
            if (Properties.Settings.Default.interval_before_reinitiating_recording < Properties.Settings.Default.seconds_before_event)
            {
                Properties.Settings.Default.interval_before_reinitiating_recording = Properties.Settings.Default.seconds_before_event;
            }
        }

        private void Nud_seconds_before_event_ValueChanged(object sender, EventArgs e)
        {
            //check the other nud
            if (Properties.Settings.Default.interval_before_reinitiating_recording < Properties.Settings.Default.seconds_before_event)
            {
                Properties.Settings.Default.interval_before_reinitiating_recording = Properties.Settings.Default.seconds_before_event;
            }
        }
        private void SettingsUI_Shown(object sender, EventArgs e)
        {
            DisableOperatorCaptureCheckBox_ifNeeded();
        }

        #region DLLIMPORT
        [DllImport("olepro32.dll")]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType=UnmanagedType.IUnknown)]
           ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved);
        #endregion

        private void ComboBoxResolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.resolution_changed = true;
        }
    }
}