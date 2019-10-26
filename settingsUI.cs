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

namespace FaceDetection
{

    public partial class SettingsUI : Form
    {
        static ComboBox comboBoxFrames;
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
        private int InitPanelWidth;
        private int InitPanelHeight;

        public SettingsUI()
        {
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }
            catch(IOException iox)
            {
                //resetting the default folders to C
                //hoping it exists
                Properties.Settings.Default.video_file_location = @"C:\UVCCAMERA";
                Properties.Settings.Default.Save();
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
            }
            

            InitializeComponent();
            //Setup window
            Size size = new Size(0,0);
               size = GetWidth(Properties.Settings.Default.main_camera_index);
            this.Width = 1024;
            this.Height = 760;
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
                default: return new Size(640,480);
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
            if(or_selected_camera_number.Items.Count> Properties.Settings.Default.main_camera_index)
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
            Properties.Settings.Default.Save();
            MainForm.FormChangesApply();
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                //Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
                Process.Start(Properties.Settings.Default.video_file_location);
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
            string camX = "C" + (Camera_index + 1) + "x";
            string camY = "C" + (Camera_index + 1) + "y";
            string camW = "C" + (Camera_index + 1) + "w";
            string camH = "C" + (Camera_index + 1) + "h";
            //string camF = "C" + (Camera_index + 1) + "f";
            //string camRes = "C" + (Camera_index + 1) + "res";
            
            numericUpDownX.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camX, true, DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownY.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camY, true, DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownW.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camW, true, DataSourceUpdateMode.OnPropertyChanged));
            numericUpDownH.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camH, true, DataSourceUpdateMode.OnPropertyChanged));
            //comboBoxFPS.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camF, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //comboBoxResolutions.DataBindings.Add(new System.Windows.Forms.Binding("Text", Properties.Settings.Default, camRes, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //comboBoxFrames = comboBoxFPS;
        }

        

        public static void SetComboBoxValues(string vs)
        {
            if(comboBoxFrames!=null)
            {
                comboBoxFrames.Items.Add(vs);
                Console.WriteLine(vs + " 177");
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


            //Camera.SetNumberOfCameras();
            

            SetCheckBoxValues();            
        }

        private void SetCheckBoxValues()
        {
            /*
            checkBox_full_screen.Checked = Properties.Settings.Default.main_window_full_screen;
            cb_all_cameras.Checked = Properties.Settings.Default.show_all_cams_simulteneously;
            cb_always_on_top.Checked = Properties.Settings.Default.window_on_top;
            cb_backlight_off_idling.Checked = Properties.Settings.Default.backlight_off_when_idle;
            cb_backlight_on_recognition.Checked = Properties.Settings.Default.backlight_on_upon_face_rec;
            cb_dateandtime.Checked = Properties.Settings.Default.show_current_datetime;
            cb_delete_old.Checked = Properties.Settings.Default.enable_delete_old_files;
            cb_event_recorder.Checked = Properties.Settings.Default.enable_event_recorder;
            cb_face_recognition.Checked = Properties.Settings.Default.enable_face_recognition;
            cb_operator_capture.Checked = Properties.Settings.Default.capture_operator;
            cb_recording_during_facerec.Checked = Properties.Settings.Default.recording_while_face_recognition;
            cb_record_upon_start.Checked = Properties.Settings.Default.recording_on_start;
            cb_show_camera_number.Checked = Properties.Settings.Default.show_camera_no;
            cb_show_rec_icon.Checked = Properties.Settings.Default.show_recording_icon;
            cb_window_pane.Checked = Properties.Settings.Default.show_window_pane;
            */
            pictureBox_irsensor.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.use_ir_sensor)];
            pictureBox_full_screen.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.main_window_full_screen)];
            pictureBox_allcam.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.show_all_cams_simulteneously)];
            pictureBox_DeleteOldData.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.enable_delete_old_files)];
            pictureBox_alwaysOnTop.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.window_on_top)];
            pictureBox_showWindowPane.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.show_window_pane)];
            pictureBox_showCurrentDate.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.show_current_datetime)];
            pictureBox_showCameraNo.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.show_camera_no)];
            pictureBox_showRecordingIcon.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.show_recording_icon)];
            pictureBox_faceRecognition.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.enable_face_recognition)];
            pictureBox_BacklightOnRecognition.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.backlight_on_upon_face_rec)];
            pictureBox_operatorCapture.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.capture_operator)];
            pictureBox_recordingDuringFaceRecognition.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.recording_while_face_recognition)];
            pictureBox_recordUponStart.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.recording_on_start)];
            pictureBox_backlightOffWhenIdle.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.enable_backlight_off_when_idle)];
            pictureBox_EventRecorder.Image = check_state_images.Images[Convert.ToInt32(Properties.Settings.Default.enable_event_recorder)];
        }



        private void Cm_language_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeLanguage();

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
            Properties.Settings.Default.Save();
            string lan = Properties.Settings.Default.culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsUI));
            var cult = new CultureInfo(lan);

            //foreach (Control c in this.Controls)
            foreach (Control c in this.tabControl1.Controls)
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
            Properties.Settings.Default.capture_method = cm_capture_mode.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void NumericUpDownCamCount_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.camera_count = numericUpDownCamCount.Value;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxStateChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            //ImageChangeOnCustomCheckBox();
            Console.WriteLine(checkBox + " checkbox clicked");
        }

        private void PicBox_Clicked(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_viewSettings, "System.Windows.Forms.CheckBox", picbox);
            //SetCheckBoxState(checkBox);
        }

        private void checkOnKids(Control control, string type, PictureBox picbox)
        {
            CheckBox checkBox;
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
                    Console.WriteLine(checkBox.Checked + " is +++++ " + checkBox.Text);                   
                }
                
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                {
                    checkOnKids(c, type, picbox);
                }
            }
        }

        //Function ----> PictureBox Change Action
        

        //Function ----> CheckBox Change Action
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
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_functionalitySettings, "System.Windows.Forms.CheckBox", picbox);
        }

        private void PictureBox_operatorCapture_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_OperatorCapture, "System.Windows.Forms.CheckBox", picbox);
        }

        private void PictureBox_EventRecorder_Click(object sender, EventArgs e)
        {
            PictureBox picbox = (PictureBox)sender;
            checkOnKids(this.groupBox_EventRecorder, "System.Windows.Forms.CheckBox", picbox);
        }
    }
}
