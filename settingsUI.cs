using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceDetection
{
    public partial class SettingsUI : Form
    {
        static ComboBox frame_rates_combo;
        static ComboBox resolutions_combo;
        static ComboBox selected_camera_combo;

        DsDevice[] capDevices;
        static string[] camera_names;

        private int MainCameraBeforeSettingsLoad;
        private bool operatorCaptureCbStateC1, operatorCaptureCbStateC2, operatorCaptureCbStateC3, operatorCaptureCbStateC4;
        private bool sensorEnabledCbStateC1, sensorEnabledCbStateC2, sensorEnabledCbStateC3, sensorEnabledCbStateC4;
        private bool faceDetectionCbStateC1, faceDetectionCbStateC2, faceDetectionCbStateC3, faceDetectionCbStateC4;
        private bool operatorActionCbStateC1, operatorActionCbStateC2, operatorActionCbStateC3, operatorActionCbStateC4;

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
                // There is no disk D, resetting the default folders to C
                Properties.Settings.Default.video_file_location = @"C:\UVCCAMERA";
                Properties.Settings.Default.temp_folder = @"C:\TEMP";

                Directory.CreateDirectory(Properties.Settings.Default.temp_folder);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }

            this.ControlBox = false;
            selected_camera_combo = cm_camera_number; //that makes the camera the default, main camera aka camera 1
            frame_rates_combo = comboBoxFPS;
            resolutions_combo = comboBoxResolutions;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        /// <summary>
        /// Get or set the MAIN CAMERA INDEX (Camera facing the user)
        /// </summary>
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

        public int getCameraIndexSelected()
        {
            return selected_camera_combo.SelectedIndex;
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
            Properties.Settings.Default.Reload();
            Properties.Settings.Default.main_camera_index = MainCameraBeforeSettingsLoad;
            Properties.Settings.Default.C1_enable_capture_operator = operatorCaptureCbStateC1;
            Properties.Settings.Default.C1_enable_Human_sensor = sensorEnabledCbStateC1;
            Properties.Settings.Default.C1_enable_face_recognition = faceDetectionCbStateC1;
            Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation = operatorActionCbStateC1;
            Properties.Settings.Default.C2_enable_capture_operator = operatorCaptureCbStateC2;
            Properties.Settings.Default.C2_enable_Human_sensor = sensorEnabledCbStateC2;
            Properties.Settings.Default.C2_enable_face_recognition = faceDetectionCbStateC2;
            Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation = operatorActionCbStateC2;
            Properties.Settings.Default.C3_enable_capture_operator = operatorCaptureCbStateC3;
            Properties.Settings.Default.C3_enable_Human_sensor = sensorEnabledCbStateC3;
            Properties.Settings.Default.C3_enable_face_recognition = faceDetectionCbStateC3;
            Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation = operatorActionCbStateC3;
            Properties.Settings.Default.C4_enable_capture_operator = operatorCaptureCbStateC4;
            Properties.Settings.Default.C4_enable_Human_sensor = sensorEnabledCbStateC4;
            Properties.Settings.Default.C4_enable_face_recognition = faceDetectionCbStateC4;
            Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation = operatorActionCbStateC4;
        }

        private void Save_and_close(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Camera.CountCamera();
            
            Hide();

            if (Properties.Settings.Default.show_all_cams_simulteneously == false)
            {
                for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                {
                    MULTI_WINDOW.formList[i].WindowState = FormWindowState.Minimized;
                }
            }
            else
            {
                for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                {
                     MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                }
            }

            MainForm.AllChangesApply();

            
            // this.Hide();

            // 4 Cameras: the selected camera became preevent mode (or preview), others became preview mode
            
                MULTI_WINDOW.formList[Camera_index].crossbar.Start(Camera_index, CAMERA_MODES.PREVIEW);
                for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
                {
                    if (i != Camera_index)
                    { 
                        MULTI_WINDOW.SetToPreviewMode(i);
                    }
                MULTI_WINDOW.formList[i].Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(i);                
            }
            
        }        

        private void SetCameraPropertiesFromMemory()
        {
            numericUpDownX.DataBindings.Clear();
            numericUpDownY.DataBindings.Clear();
            numericUpDownW.DataBindings.Clear();
            numericUpDownH.DataBindings.Clear();
            comboBoxFPS.DataBindings.Clear();
            comboBoxResolutions.DataBindings.Clear();
            cb_event_recorder.DataBindings.Clear();
            event_record_time_before_event.DataBindings.Clear();
            nud_event_record_after.DataBindings.Clear();
            cb_operator_capture.DataBindings.Clear();
            nud_seconds_before_event.DataBindings.Clear();
            nud_seconds_after_event.DataBindings.Clear();
            numericUpDown2.DataBindings.Clear();
            nud_reinitiation_interval.DataBindings.Clear();
            cb_face_recognition.DataBindings.Clear();
            cb_human_sensor.DataBindings.Clear();
            cb_recording_operation.DataBindings.Clear();
            cm_capture_mode.DataBindings.Clear();

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
            comboBoxFPS.DataBindings.Add(new Binding("Text", Properties.Settings.Default, camF, true, DataSourceUpdateMode.OnPropertyChanged));
            comboBoxResolutions.DataBindings.Add(new Binding("Text", Properties.Settings.Default, camRes, true, DataSourceUpdateMode.OnPropertyChanged));
            
            numericUpDownX.Enabled = !Properties.Settings.Default.main_window_full_screen;
            numericUpDownY.Enabled = !Properties.Settings.Default.main_window_full_screen;
            numericUpDownW.Enabled = !Properties.Settings.Default.main_window_full_screen;
            numericUpDownH.Enabled = !Properties.Settings.Default.main_window_full_screen;

            cm_capture_mode.DataBindings.Add(new Binding("Text", Properties.Settings.Default, "C" + (Camera_index + 1) + "_capture_type", true, DataSourceUpdateMode.OnPropertyChanged));
            cb_event_recorder.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (Camera_index + 1) + "_enable_event_recorder", true, DataSourceUpdateMode.OnPropertyChanged));
            event_record_time_before_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C"+ (Camera_index + 1) + "_event_record_time_before_event", true, DataSourceUpdateMode.OnPropertyChanged));
            nud_event_record_after.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (Camera_index + 1) + "_event_record_time_after_event", true, DataSourceUpdateMode.OnPropertyChanged));
            cb_operator_capture.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (Camera_index + 1) + "_enable_capture_operator", true, DataSourceUpdateMode.OnPropertyChanged));
            nud_seconds_before_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (Camera_index + 1) + "_seconds_before_event", true, DataSourceUpdateMode.OnPropertyChanged));
            nud_seconds_after_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (Camera_index + 1) + "_seconds_after_event", true, DataSourceUpdateMode.OnPropertyChanged));
            numericUpDown2.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (Camera_index + 1) + "_check_interval", true, DataSourceUpdateMode.OnPropertyChanged));
            nud_reinitiation_interval.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (Camera_index + 1) + "_interval_before_reinitiating_recording", true, DataSourceUpdateMode.OnPropertyChanged));
            cb_face_recognition.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (Camera_index + 1) + "_enable_face_recognition", true, DataSourceUpdateMode.OnPropertyChanged));
            cb_human_sensor.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (Camera_index + 1) + "_enable_Human_sensor", true, DataSourceUpdateMode.OnPropertyChanged));
            cb_recording_operation.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (Camera_index + 1) + "_Recording_when_at_the_start_of_operation", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        public static void SetComboBoxFPSValues(List<string> vs)
        {
            bool matching_fps_found = false;
            if (frame_rates_combo != null)
            {
                frame_rates_combo.Items.AddRange(vs.ToArray());
                for (int i=0; i<vs.Count; i++)
                {
                    if (vs[i] == Properties.Settings.Default.main_camera_fps)
                    {
                        matching_fps_found = true;
                        break;
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
            labelCameraNumber.Text = (Camera_index + 1).ToString();
            SetCameraPropertiesFromMemory();
        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            // Memorise in case of Cancel button 
            MainCameraBeforeSettingsLoad = Properties.Settings.Default.main_camera_index;
            operatorCaptureCbStateC1 = Properties.Settings.Default.C1_enable_capture_operator;
            sensorEnabledCbStateC1 = Properties.Settings.Default.C1_enable_Human_sensor;
            faceDetectionCbStateC1 = Properties.Settings.Default.C1_enable_face_recognition;
            operatorActionCbStateC1 = Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation;
            operatorCaptureCbStateC2 = Properties.Settings.Default.C2_enable_capture_operator;
            sensorEnabledCbStateC2 = Properties.Settings.Default.C2_enable_Human_sensor;
            faceDetectionCbStateC2 = Properties.Settings.Default.C2_enable_face_recognition;
            operatorActionCbStateC2 = Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation;
            operatorCaptureCbStateC3 = Properties.Settings.Default.C3_enable_capture_operator;
            sensorEnabledCbStateC3 = Properties.Settings.Default.C3_enable_Human_sensor;
            faceDetectionCbStateC3 = Properties.Settings.Default.C3_enable_face_recognition;
            operatorActionCbStateC3 = Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation;
            operatorCaptureCbStateC4 = Properties.Settings.Default.C4_enable_capture_operator;
            sensorEnabledCbStateC4 = Properties.Settings.Default.C4_enable_Human_sensor;
            faceDetectionCbStateC4 = Properties.Settings.Default.C4_enable_face_recognition;
            operatorActionCbStateC4 = Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation;

            if (cm_camera_number.Items.Count > 0)
            {
                cm_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index <= 0 ? 0 : Properties.Settings.Default.main_camera_index;
                //cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_method <= 0 ? 0 : Properties.Settings.Default.capture_method;
            }

            cm_language.SelectedItem = Properties.Settings.Default.language;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);
            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
            SetCameraPropertiesFromMemory();
            Camera.SetNumberOfCameras();

            int cam_index = Camera_index;
            if (cam_index == 0)
            { 
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C1_enable_capture_operator, cam_index);
            }
            else if (cam_index == 1)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C2_enable_capture_operator, cam_index);
            }
            else if (cam_index == 2)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C3_enable_capture_operator, cam_index);
            }
            else if (cam_index == 3)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C4_enable_capture_operator, cam_index);
            }

            this.ControlBox = false;
            this.MaximizeBox = false;
            this.TopMost = true;

            if (MainForm.GetMainForm.AnyRecordingInProgress == true)
            {
                cm_camera_number.Enabled = false;
                button_settings_save.Enabled = false;
            }
            else
            {
                cm_camera_number.Enabled = true;
                button_settings_save.Enabled = true;
            }
        }
                 
        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cm_language.SelectedItem != null)
            {
                if (cm_language.SelectedItem.ToString() == "English")
                {
                    Properties.Settings.Default.culture = "en-US";
                    Properties.Settings.Default.language = "English";
                    this.Text = "Settings";
                }
                else
                {
                    Properties.Settings.Default.culture = "ja-JP";
                    Properties.Settings.Default.language = "日本語";
                    this.Text = "設定";
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

        private void Cm_capture_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.C1_capture_method = cm_capture_mode.SelectedIndex;

            if (cm_capture_mode.SelectedIndex == 1)
            {
                nud_seconds_after_event.Enabled = false;
                nud_seconds_before_event.Enabled = false;
            }
            else if (cb_operator_capture.Checked == true)
            {
                if (cm_capture_mode.SelectedIndex == 0)
                {
                    nud_seconds_after_event.Enabled = true;
                    nud_seconds_before_event.Enabled = true;
                    nud_reinitiation_interval.Enabled = true;
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

        private void ChangeControlEnabled(Control control, bool enabled, int cam_index)
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
                if (cam_index == 0)
                {
                    Properties.Settings.Default.C1_enable_Human_sensor = false;
                    Properties.Settings.Default.C1_enable_face_recognition = false;
                    Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation = false;
                }
                else if (cam_index == 1)
                {
                    Properties.Settings.Default.C2_enable_Human_sensor = false;
                    Properties.Settings.Default.C2_enable_face_recognition = false;
                    Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation = false;
                }
                else if (cam_index == 2)
                {
                    Properties.Settings.Default.C3_enable_Human_sensor = false;
                    Properties.Settings.Default.C3_enable_face_recognition = false;
                    Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation = false;
                }
                else if (cam_index == 3)
                {
                    Properties.Settings.Default.C4_enable_Human_sensor = false;
                    Properties.Settings.Default.C4_enable_face_recognition = false;
                    Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation = false;
                }
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
            int cam_index = Camera_index;
            ChangeControlEnabled(this.groupBox_functionalitySettings, chk, cam_index);

            if (cb_operator_capture.Checked && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked))
            {
                nud_seconds_before_event.Enabled = true;
                nud_seconds_after_event.Enabled = true;
            }
            else
            {
                nud_seconds_before_event.Enabled = false;
                nud_seconds_after_event.Enabled = false;
            }
        }
        
        private void Cb_human_sensor_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if(check.Checked)
            {
                //cb_face_recognition.Checked = !check.Checked;
                if(Properties.Settings.Default.C1_enable_Human_sensor == true)
                {
                    Properties.Settings.Default.C1_enable_face_recognition = false;
                }
                if (Properties.Settings.Default.C2_enable_Human_sensor == true)
                {
                    Properties.Settings.Default.C2_enable_face_recognition = false;
                }
                if (Properties.Settings.Default.C3_enable_Human_sensor == true)
                {
                    Properties.Settings.Default.C3_enable_face_recognition = false;
                }
                if (Properties.Settings.Default.C4_enable_Human_sensor == true)
                {
                    Properties.Settings.Default.C4_enable_face_recognition = false;
                }
            }
            else
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
            }

            if (cb_operator_capture.Checked && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked))
            {
                nud_seconds_before_event.Enabled = true;
                nud_seconds_after_event.Enabled = true;
            }
            else
            {
                nud_seconds_before_event.Enabled = false;
                nud_seconds_after_event.Enabled = false;
            }
        }

        private void Cb_face_recognition_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
            {
                //cb_human_sensor.Checked = !check.Checked;
                if (Properties.Settings.Default.C1_enable_face_recognition == true)
                {
                    Properties.Settings.Default.C1_enable_Human_sensor = false;
                }
                if (Properties.Settings.Default.C2_enable_face_recognition == true)
                {
                    Properties.Settings.Default.C2_enable_Human_sensor = false;
                }
                if (Properties.Settings.Default.C3_enable_face_recognition == true)
                {
                    Properties.Settings.Default.C3_enable_Human_sensor = false;
                }
                if (Properties.Settings.Default.C4_enable_face_recognition == true)
                {
                    Properties.Settings.Default.C4_enable_Human_sensor = false;
                }
            }
            else
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
            }

            if (cb_operator_capture.Checked && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked))
            {
                nud_seconds_before_event.Enabled = true;
                nud_seconds_after_event.Enabled = true;
            }
            else
            {
                nud_seconds_before_event.Enabled = false;
                nud_seconds_after_event.Enabled = false;
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

            if (cb_operator_capture.Checked && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked))
            {
                if (cm_capture_mode.Text != "Snapshot")
                {
                nud_seconds_before_event.Enabled = true;
                nud_seconds_after_event.Enabled = true;
            }
            }
            else
            {
                nud_seconds_before_event.Enabled = false;
                nud_seconds_after_event.Enabled = false;
            }
        }

        private void DisableOperatorCaptureCheckBox_ifNeeded()
        {
            // CAMERA 1
            if (Properties.Settings.Default.C1_enable_Human_sensor || Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C1_Recording_when_at_the_start_of_operation)
            {                
                Properties.Settings.Default.C1_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C1_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 2
            if (Properties.Settings.Default.C2_enable_Human_sensor || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C2_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C2_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 3
            if (Properties.Settings.Default.C3_enable_Human_sensor || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C3_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C3_enable_capture_operator = false; //All three are off. Disable
            }

            // CAMERA 4
            if (Properties.Settings.Default.C4_enable_Human_sensor || Properties.Settings.Default.C4_enable_face_recognition || Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation)
            {
                Properties.Settings.Default.C4_enable_capture_operator = true;
            }
            else
            {
                Properties.Settings.Default.C4_enable_capture_operator = false; //All three are off. Disable
            }
        }

        private void Cb_backlight_off_idling_CheckStateChanged(object sender, EventArgs e)
        {
            numericUpDownBacklight.Enabled = cb_backlight_off_idling.Checked;
        }

        private void Nud_reinitiation_interval_ValueChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.C1_interval_before_reinitiating_recording < Properties.Settings.Default.C1_seconds_before_event)
            {
                Properties.Settings.Default.C1_interval_before_reinitiating_recording = Properties.Settings.Default.C1_seconds_before_event;
            }
            if (Properties.Settings.Default.C2_interval_before_reinitiating_recording < Properties.Settings.Default.C2_seconds_before_event)
            {
                Properties.Settings.Default.C2_interval_before_reinitiating_recording = Properties.Settings.Default.C2_seconds_before_event;
            }
            if (Properties.Settings.Default.C3_interval_before_reinitiating_recording < Properties.Settings.Default.C3_seconds_before_event)
            {
                Properties.Settings.Default.C3_interval_before_reinitiating_recording = Properties.Settings.Default.C3_seconds_before_event;
            }
            if (Properties.Settings.Default.C4_interval_before_reinitiating_recording < Properties.Settings.Default.C4_seconds_before_event)
            {
                Properties.Settings.Default.C4_interval_before_reinitiating_recording = Properties.Settings.Default.C4_seconds_before_event;
            }
        }

        private void Nud_seconds_before_event_ValueChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.C1_interval_before_reinitiating_recording < Properties.Settings.Default.C1_seconds_before_event)
            {
                Properties.Settings.Default.C1_interval_before_reinitiating_recording = Properties.Settings.Default.C1_seconds_before_event;
            }
            if (Properties.Settings.Default.C2_interval_before_reinitiating_recording < Properties.Settings.Default.C2_seconds_before_event)
            {
                Properties.Settings.Default.C2_interval_before_reinitiating_recording = Properties.Settings.Default.C2_seconds_before_event;
            }
            if (Properties.Settings.Default.C3_interval_before_reinitiating_recording < Properties.Settings.Default.C3_seconds_before_event)
            {
                Properties.Settings.Default.C3_interval_before_reinitiating_recording = Properties.Settings.Default.C3_seconds_before_event;
            }
            if (Properties.Settings.Default.C4_interval_before_reinitiating_recording < Properties.Settings.Default.C4_seconds_before_event)
            {
                Properties.Settings.Default.C4_interval_before_reinitiating_recording = Properties.Settings.Default.C4_seconds_before_event;
            }
        }

        private void SettingsUI_Shown(object sender, EventArgs e)
        {
            DisableOperatorCaptureCheckBox_ifNeeded();
        }

        private void ComboBoxResolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.resolution_changed = true;
        }

        private void NumericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDownX.Value > Screen.PrimaryScreen.Bounds.Width - 400)
            {
                numericUpDownX.Value = Screen.PrimaryScreen.Bounds.Width - 400;
            }
            else if (numericUpDownX.Value < 0)
            {
                numericUpDownX.Value = 0;
            }
        }

        private void NumericUpDownY_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownY.Value > Screen.PrimaryScreen.Bounds.Height - 300)
            {
                numericUpDownY.Value = Screen.PrimaryScreen.Bounds.Height - 300;
            }
            else if (numericUpDownY.Value < 0)
            {
                numericUpDownY.Value = 0;
            }
        }

        private void NumericUpDownW_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDownW.Value > Screen.PrimaryScreen.Bounds.Width)
            {
                numericUpDownW.Value = Screen.PrimaryScreen.Bounds.Width;
            }
            else if (numericUpDownW.Value < 0)
            {
                numericUpDownW.Value = 0;
            }
        }

        private void NumericUpDownH_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownH.Value > Screen.PrimaryScreen.Bounds.Height)
            {
                numericUpDownH.Value = Screen.PrimaryScreen.Bounds.Height;
            }
            else if (numericUpDownH.Value < 0)
            {
                numericUpDownH.Value = 0;
            }
        }

        private void StorePath_TextChanged(object sender, EventArgs e)
        {
            bool pathChanged = false;
            char[] invalidPathChars = {'*', '?', '!', '<','>','|'};

            char[] characters = storePath.Text.ToCharArray();
            for(int i=0; i< characters.Length; i++)
            {
                for (int j = 0; j < invalidPathChars.Length; j++)
                {
                    if(characters[i].Equals(invalidPathChars[j]))
                    {
                        characters[i] = '\0';
                        pathChanged = true;
                    }
                }
            }

            if(pathChanged == true)
            {
                storePath.Text = new string(characters);
                storePath.SelectionStart = storePath.Text.Length;
            }
        }

        /*
        
         * Checking full-width character is included in string.
             * If full-width character is included in string,
             * it will return true.
             * If is not, it will return false.
             * @param cmdl
             * @return
 
        public boolean isContainFullWidth(String cmdl)
        {
            boolean isFullWidth = false;
            for (char c : cmdl.toCharArray())
            {
                if (!isHalfWidth(c))
                {
                    isFullWidth = true;
                    break;
                }
            }

            return isFullWidth;
        }

        /**
         * Checking character is half-width or not.
         * Unicode value of half-width range:
         * '\u0000' - '\u00FF'
         * '\uFF61' - '\uFFDC'
         * '\uFFE8' - '\uFFEE'
         * If unicode value of character is within this range,
         * it will be half-width character.
         * @param c
         * @return
         
        public boolean isHalfWidth(char c)
        {
            return '\u0000' <= c && c <= '\u00FF'
                || '\uFF61' <= c && c <= '\uFFDC'
                || '\uFFE8' <= c && c <= '\uFFEE';
        }

             */


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

    }
}