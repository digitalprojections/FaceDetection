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
        static ComboBox comboBoxFrames;
        static ComboBox comboBoxResolution;
        DsDevice[] capDevices;
        static string[] camera_names;


        static ComboBox or_selected_camera_number;
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
        private int InitPanelWidth;
        private int InitPanelHeight;

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
            or_selected_camera_number = cm_camera_number;            
            comboBoxFrames = comboBoxFPS;
            comboBoxResolution = comboBoxResolutions;
        }

        private Size GetWidth(int cam_ind)
        {
            Size retval;


            switch (cam_ind)
            {
                case 0:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C1w), decimal.ToInt32(Properties.Settings.Default.C1h));

                    return retval;
                case 1:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C2w), decimal.ToInt32(Properties.Settings.Default.C2h));

                    return retval;
                case 2:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C3w), decimal.ToInt32(Properties.Settings.Default.C3h));

                    return retval;
                case 3:
                    retval = new Size(decimal.ToInt32(Properties.Settings.Default.C4w), decimal.ToInt32(Properties.Settings.Default.C4h));

                    return retval;
                default: return new Size(640, 480);

            }

        }



        public void ArrangeCameraNames(int len)
        {
            or_selected_camera_number.Items.Clear();
            camera_names = new string[len];
            Logger.Add(len);

            for (int i = 0; i < len; i++)
            {
                or_selected_camera_number.Items.Add(i + 1);
            }
            if (or_selected_camera_number.Items.Count >= Properties.Settings.Default.main_camera_index)
            {
                Logger.Add(Properties.Settings.Default.main_camera_index);
                or_selected_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index;
            }
            else
            {
                Properties.Settings.Default.main_camera_index = 0;
            }

        }


        private void changeStoreLocation(object sender, EventArgs e)
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
            MainForm.AllChangesApply();
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                changeStoreLocation(sender, e);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }
        private void closeSettings(object sender, EventArgs e)
        {           
            Hide();
        }

        private void save_and_close(object sender, EventArgs e)
        {
            Logger.Add(Properties.Settings.Default.manual_record_time);

            Properties.Settings.Default.Save();
            MainForm.AllChangesApply();
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
            if (comboBoxFrames != null)
            {
                comboBoxFrames.Items.AddRange(vs.ToArray());
                for (int i=0; i<vs.Count;i++)
                {
                    
                        if (vs[i] == Properties.Settings.Default.C1f)
                        {
                            comboBoxFrames.SelectedItem = Properties.Settings.Default.C1f;
                        }else
                    {
                        comboBoxFrames.SelectedItem = vs[0];
                    }
                    
                }

                Console.WriteLine(vs.Count + " 177");
            }
        }

        /// <summary>
        /// The ComboBox class searches for the specified object by using the IndexOf method. 
        /// This method uses the Equals method to determine equality.
        /// </summary>
        /// <param name="vs"></param>
        public static void SetComboBoxResolutionValues(List<string> vs)
        {
            if (comboBoxResolution != null)
            {
                comboBoxResolution.Items.AddRange(vs.ToArray());

                if (comboBoxResolution.Items.Count > 0)
                {
                    Console.WriteLine(Properties.Settings.Default.C1res);

                    comboBoxResolution.SelectedItem = Properties.Settings.Default.C1res;
                }
            }
        }

        private void cameraSelected(object sender, EventArgs e)
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
                cm_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index;
                cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_method;
            }
            SetCameraPropertiesFromMemory();
            cm_language.SelectedItem = Properties.Settings.Default.language;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);
            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
            SetCameraPropertiesFromMemory();
            Camera.SetNumberOfCameras();
            changeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.capture_operator);
           
        }
        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeLanguage();
        }

        private void ChangeLanguage()
        {
            try
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
            }
            catch (NullReferenceException nrx)
            {
                Logger.Add(nrx);
            }
            string lan = Properties.Settings.Default.culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsUI));
            var cult = new CultureInfo(lan);

            foreach (Control c in this.Controls)
            {
                resources.ApplyResources(c, c.Name, cult);
                if (c.GetType().ToString() == "System.Windows.Forms.TabControl")
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
                {
                    checkOnKids(cult, c, crm);
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

        private bool checkOnKids(Control control, string type, PictureBox picbox)
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
                    ret = checkOnKids(c, type, picbox);
                }

            }
            return ret;
        }

        private void SetCheckBoxState(CheckBox ChBox)
        {
            if (ChBox.Checked == true)
            {
                ChBox.Checked = false;
            }
            else
            {
                ChBox.Checked = true;
            }
        }

        private void button_cameraProperties_Click(object sender, EventArgs e)
        {
            DisplayPropertyPage();
        }

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

        private void changeControlEnabled(Control control, bool enabled)
        {
            foreach (Control c in control.Controls)
            {
                if (c!=cb_operator_capture)
                {
                    c.Enabled = enabled;
                }
            }
        }


        private void checkBox_full_screen_CheckedChanged(object sender, EventArgs e)
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
            changeControlEnabled(this.groupBox_functionalitySettings, chk);
        }

        private void NUD_MouseClick(object sender, MouseEventArgs e)
        {
            NumericUpDown upDown = (NumericUpDown)sender;

            Logger.Add(upDown.Value);
        }

        private void Cb_human_sensor_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if(check.Checked)
            {
                cb_human_sensor.Checked = check.Checked;
                cb_face_recognition .Checked = !check.Checked;
            }
        }

        private void Cb_face_recognition_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
            {
                cb_human_sensor.Checked = !check.Checked;
                cb_face_recognition.Checked = check.Checked;
            }
        }

        private void Nud_seconds_after_Click(object sender, EventArgs e)
        {
            Logger.Add(Properties.Settings.Default.seconds_after_event);
            Logger.Add(Properties.Settings.Default.seconds_before_event);
            Logger.Add(Properties.Settings.Default.event_record_time_before_event);
            Logger.Add(Properties.Settings.Default.event_record_time_after_event);
            Logger.Add(Properties.Settings.Default.manual_record_time);
            Logger.Add(Properties.Settings.Default.interval_before_reinitiating_recording);
        }
    }
}