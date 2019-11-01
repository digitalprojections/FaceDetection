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
        public int Camera_index {
            get {
                return Properties.Settings.Default.main_camera_index;
            }
            private set {
                Properties.Settings.Default.main_camera_index = value;
                Properties.Settings.Default.Save();
            }
        }

        public SettingsUI()
        {
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
                Properties.Settings.Default.temp_folder = @"C:TEMP";
                
                Directory.CreateDirectory(Properties.Settings.Default.temp_folder);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }


            InitializeComponent();
            //Setup window
            Size size = new Size(0, 0);
            size = GetWidth(Properties.Settings.Default.main_camera_index);
        
            this.ControlBox = false;

            or_selected_camera_number = cm_camera_number;
            //get reference to the mainform
            //Console.WriteLine(MainForm.GetCamera());
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



        public static void ArrangeCameraNames(int len)
        {
            or_selected_camera_number.Items.Clear();
            camera_names = new string[len];
            for (int i = 0; i < len; i++)
            {
                or_selected_camera_number.Items.Add(i + 1);
            }
            if (or_selected_camera_number.Items.Count > Properties.Settings.Default.main_camera_index)
            {

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
            //Properties.Settings.Default.Save();
            MainForm.AllChangesApply();
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
                //Process.Start(Properties.Settings.Default.video_file_location);
                changeStoreLocation(sender, e);
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
            MainForm.AllChangesApply();
            this.Hide();
        }

        //private void applyChanges(object sender, EventArgs e)
        //{
        //    /*
        //     Here handle all immediate changes
        //     */
        //    Properties.Settings.Default.Save();
        //    MainForm.AllChangesApply();
        //}

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
            comboBoxFrames = comboBoxFPS;
            comboBoxResolution = comboBoxResolutions;

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
                comboBoxFrames.SelectedItem = Properties.Settings.Default.C1f;
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

                if (comboBoxResolution.Items.Count>0)
                {
                    Console.WriteLine(Properties.Settings.Default.C1res);
                    
                    comboBoxResolution.SelectedItem = Properties.Settings.Default.C1res;                    
                    //Console.WriteLine(comboBoxResolution.SelectedItem + Properties.Settings.Default.C1res);
                }
            }
        }

        private void cameraSelected(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            //Debug.WriteLine(comboBox.SelectedIndex);
            Camera_index = comboBox.SelectedIndex;
            Properties.Settings.Default.main_camera_index = Camera_index;
            SetCameraPropertiesFromMemory();
        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            if (cm_camera_number.Items.Count > 0)
            {
                //cm_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index;
                cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_method;
            }
            SetCameraPropertiesFromMemory();
            cm_language.SelectedItem = Properties.Settings.Default.language;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);
            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
            SetCameraPropertiesFromMemory();
            Camera.SetNumberOfCameras();
        }


        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cm_language.SelectedIndex !=-1)
            {
                ChangeLanguage();
            }

            //this.flowLayoutPanel1.Width = InitPanelWidth;
            //this.flowLayoutPanel1.Height = InitPanelHeight;
            //this.flowLayoutPanel1.Refresh();
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
            //Properties.Settings.Default.Save();
            string lan = Properties.Settings.Default.culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsUI));
            var cult = new CultureInfo(lan);
            
            //foreach (Control c in this.tabControl1.Controls)
            foreach (Control c in this.Controls)
            {
                resources.ApplyResources(c, c.Name, cult);
                //Debug.WriteLine(c.GetType().ToString());
                //if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
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
                Debug.WriteLine(c.GetType().ToString() + " name " + c.Name);
                //if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    checkOnKids(cult, c, crm);
                }
            }
        }
        private void Cm_capture_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.capture_method = cm_capture_mode.SelectedIndex;
            Console.WriteLine(cm_capture_mode.SelectedIndex);
            numericUpDown5.Enabled = cm_capture_mode.SelectedIndex == 1 ? false : true;
            numericUpDown1.Enabled = cm_capture_mode.SelectedIndex == 1 ? false : true;
            numericUpDown4.Enabled = cm_capture_mode.SelectedIndex == 1 ? false : true;

        }

        private void NumericUpDownCamCount_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.camera_count = numericUpDownCamCount.Value;
            //Properties.Settings.Default.Save();
        }

        private void CheckBoxStateChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            //ImageChangeOnCustomCheckBox();
            Console.WriteLine(checkBox + " checkbox clicked");
        }

        private void Cb_face_recognition_CheckedChanged(object sender, EventArgs e)
        {
            //bool chk = checkOnKids(this.groupBox_functionalitySettings, "System.Windows.Forms.CheckBox");
            //checkOnKids(this.groupBox_OperatorCapture, "System.Windows.Forms.CheckBox", pictureBox_operatorCapture);
            CheckBox chb = (CheckBox)sender;
            bool chk = chb.Checked;
            cb_operator_capture.Checked = !chk;
            changeControlEnabled(this.groupBox_functionalitySettings, chk);
            changeControlEnabled(this.groupBox_OperatorCapture, !chk);
        }
        private void Cb_operator_capture_CheckedChanged(object sender, EventArgs e)
        {
            //bool chk = checkOnKids(this.groupBox_OperatorCapture, "System.Windows.Forms.CheckBox", sender);
            //checkOnKids(this.groupBox_functionalitySettings, "System.Windows.Forms.CheckBox", pictureBox_faceRecognition);
            CheckBox chb = (CheckBox)sender;
            bool chk = chb.Checked;
            cb_face_recognition.Checked = !chk;
            changeControlEnabled(this.groupBox_OperatorCapture, chk);
            changeControlEnabled(this.groupBox_functionalitySettings, !chk);
        }
        

        private bool checkOnKids(Control control, string type, PictureBox picbox)
        {
            CheckBox checkBox;
            bool ret = false;
            foreach (Control c in control.Controls)
            {
                if (c.GetType().ToString()==type && c.Tag == picbox.Tag)
                {
                    //we found the control and the tag we need
                    //set the values eg: CheckBox checked state                    
                    checkBox = c as CheckBox;
                    if(checkBox.Checked)
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

        //Function ----> PictureBox Change Action
        

        //Function ----> CheckBox Change Action
       
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
            var dev = (IBaseFilter) DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, Camera_index);
            
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
            hr = OleCreatePropertyFrame(FaceDetection.MainForm.GetMainForm.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
            Marshal.ReleaseComObject(pProp);
            //Marshal.ReleaseComObject(filterInfo);
            Marshal.ReleaseComObject(dev);
        }

        private void GroupBox_viewSettings_Enter(object sender, EventArgs e)
        {

        }

        private void PictureBox_DeleteOldData_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_Environment, "System.Windows.Forms.CheckBox", picbox);
           
        }

        private void PictureBox_faceRecognition_Click(object sender, EventArgs e)
        {
            //PictureBox picbox = (PictureBox)sender;
            
        }

        private void PictureBox_operatorCapture_Click(object sender, EventArgs e)
        {
            //PictureBox picbox = (PictureBox)sender;
            
        }

        private void changeControlEnabled(Control control, bool enabled)
        {
            foreach (Control c in control.Controls)
            {
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    changeControlEnabled(c, enabled);
                }
                else if (c.Name != "cb_operator_capture" && c.Name != "cb_face_recognition")
                {
                    c.Enabled = enabled;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void PictureBox_operatorCapture_Click_ItemCheck(object sender, ItemCheckEventArgs e)
        {
           
        }

        private void PictureBox_EventRecorder_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_EventRecorder, "System.Windows.Forms.CheckBox", picbox);
        }

        private void NumericUpDownX_ValueChanged(object sender, EventArgs e)
        {
           
           
        }

        private void NumericUpDownY_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void NumericUpDownW_ValueChanged(object sender, EventArgs e)
        {
            //MainForm.GetMainForm.ReverseWindowSizeUpdate();
        }

        private void NumericUpDownH_ValueChanged(object sender, EventArgs e)
        {
            //MainForm.GetMainForm.ReverseWindowSizeUpdate();
        }

        

        private void PictureBox_recording_operator_capture_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_OperatorCapture, "System.Windows.Forms.CheckBox", picbox);
        }

        private void PictureBox_Recording_start_operation_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_OperatorCapture, "System.Windows.Forms.CheckBox", picbox);
        }

        private void Cb_recording_operator_capture_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
