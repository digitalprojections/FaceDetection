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

    public partial class settingsUI : Form
    {
        static ComboBox comboBoxFrames;
        DsDevice[] capDevices;
        string[] camera_names;

        private int camera_index;
        private int camera_number;
        public int Camera_index { get { return camera_index = Properties.Settings.Default.current_camera_index; } private set { Properties.Settings.Default.current_camera_index = value; Properties.Settings.Default.Save(); } }
        public int Camera_number { get { return camera_number = Properties.Settings.Default.current_camera_index+1;  } }

        private int InitPanelWidth;
        private int InitPanelHeight;

        public settingsUI()
        {
            Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
            Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");

            InitializeComponent();
            //Setup window
            this.Width = 1024;
            this.Height = 760;
            this.ControlBox = false;
            
            InitPanelWidth = this.flowLayoutPanel1.Width;
            InitPanelHeight = this.flowLayoutPanel1.Height;

            //get reference to the mainform
            Console.WriteLine(MainForm.GetCamera());

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

        private void ArrangeCameraNames(int len)
        {
            camera_names = new string[len];
            for (int i = 0; i < len; i++)
            {
                camera_names[i] = capDevices[i].Name;
                cm_camera_number.Items.Add(i + 1);
            }
            //cm_camera_number.SelectedIndex = Properties.Settings.Default.current_camera_index;
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
            
            SetCameraPropertiesFromMemory();
        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {
            if (cm_camera_number.Items.Count > 0)
            {
                //cm_camera_number.SelectedIndex = Properties.Settings.Default.current_camera_index;
                cm_capture_mode.SelectedIndex = Properties.Settings.Default.selectedCaptureMethod;
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
                //MessageBox.Show("The settings do not allow more than " + numericUpDownCamCount.Value + " cameras");
                ArrangeCameraNames(capDevices.Length);
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
                Console.WriteLine(capDevices.Length + " capdevices");
                    ArrangeCameraNames(capDevices.Length);
                }
                PicBoxInitFunction();
        }

        private void PicBoxInitFunction()
        {
            if (cb_all_cameras.Checked == true)
            {
                pictureBox_allcam.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_allcam.Image = Properties.Resources.checkered;
            }

            if (cb_delete_old.Checked == true)
            {
                //pictureBox_DeleteOldData.Image = Properties.Resources.check;
            }
            else
            {
                //pictureBox_DeleteOldData.Image = Properties.Resources.checkered;
            }

            if (cb_always_on_top.Checked == true)
            {
                pictureBox_alwaysOnTop.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_alwaysOnTop.Image = Properties.Resources.checkered;
            }

            if (cb_window_pane.Checked == true)
            {
                pictureBox_showWindowPane.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_showWindowPane.Image = Properties.Resources.checkered;
            }

            if (cb_dateandtime.Checked == true)
            {
                pictureBox_showCurrentDate.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_showCurrentDate.Image = Properties.Resources.checkered;
            }

            if (cb_show_camera_number.Checked == true)
            {
                pictureBox_showCameraNo.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_showCameraNo.Image = Properties.Resources.checkered;
            }

            if (cb_show_rec_icon.Checked == true)
            {
                pictureBox_showRecordingIcon.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_showRecordingIcon.Image = Properties.Resources.checkered;
            }

            if (cb_face_recognition.Checked == true)
            {
                pictureBox_faceRecognition.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_faceRecognition.Image = Properties.Resources.checkered;
            }

            if (cb_backlight_on_recognition.Checked == true)
            {
                pictureBox_BacklightOnRecognition.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_BacklightOnRecognition.Image = Properties.Resources.checkered;
            }

            if (cb_operator_capture.Checked == true)
            {
                pictureBox_operatorCapture.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_operatorCapture.Image = Properties.Resources.checkered;
            }

            if (cb_recording_during_facerec.Checked == true)
            {
                pictureBox_recordingDuringFaceRecognition.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_recordingDuringFaceRecognition.Image = Properties.Resources.checkered;
            }

            if (cb_record_upon_start.Checked == true)
            {
                pictureBox_recordUponStart.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_recordUponStart.Image = Properties.Resources.checkered;
            }

            if (cb_backlight_off_idling.Checked == true)
            {
                pictureBox_backlightOffWhenIdle.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_backlightOffWhenIdle.Image = Properties.Resources.checkered;
            }

            if (cb_event_recorder.Checked == true)
            {
                pictureBox_EventRecorder.Image = Properties.Resources.check;
            }
            else
            {
                pictureBox_EventRecorder.Image = Properties.Resources.checkered;
            }
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(settingsUI));
            var cult = new CultureInfo(lan);

            //foreach (Control c in this.Controls)
            foreach (Control c in this.flowLayoutPanel1.Controls)
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

        private void cb_all_cameras_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_all_cameras, pictureBox_allcam);
        }

        private void PictureBox_allcam_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_all_cameras);
            cb_all_cameras_CheckedChanged(sender, e);
        }

        private void cb_delete_old_CheckedChanged(object sender, EventArgs e)
        {
            //Img_Chenge(cb_delete_old, pictureBox_DeleteOldData);
        }

        private void pictureBox_DeleteOldData_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_delete_old);
            cb_delete_old_CheckedChanged(sender, e);
        }

        private void cb_always_on_top_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_always_on_top, pictureBox_alwaysOnTop);
        }

        private void pictureBox_alwaysOnTop_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_always_on_top);
            cb_always_on_top_CheckedChanged(sender, e);
        }

        private void cb_window_pane_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_window_pane, pictureBox_showWindowPane);
        }

        private void pictureBox_showWindowPane_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_window_pane);
            cb_window_pane_CheckedChanged(sender, e);
        }

        private void cb_dateandtime_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_dateandtime, pictureBox_showCurrentDate);
        }

        private void pictureBox_showCurrentDate_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_dateandtime);
            cb_dateandtime_CheckedChanged(sender, e);
        }

        private void cb_show_camera_number_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_show_camera_number, pictureBox_showCameraNo);
        }

        private void pictureBox_showCameraNo_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_show_camera_number);
            cb_show_camera_number_CheckedChanged(sender, e);
        }

        private void cb_show_rec_icon_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_show_rec_icon, pictureBox_showRecordingIcon);
        }

        private void pictureBox_showRecordingIcon_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_show_rec_icon);
            cb_show_rec_icon_CheckedChanged(sender, e);
        }

        private void cb_face_recognition_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_face_recognition, pictureBox_faceRecognition);
        }

        private void pictureBox_faceRecognition_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_face_recognition);
            cb_face_recognition_CheckedChanged(sender, e);
        }

        private void cb_backlight_on_recognition_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_backlight_on_recognition, pictureBox_BacklightOnRecognition);
        }

        private void pictureBox_BacklightOnRecognition_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_backlight_on_recognition);
            cb_backlight_on_recognition_CheckedChanged(sender, e);
        }

        private void cb_operator_capture_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_operator_capture, pictureBox_operatorCapture);
        }

        private void pictureBox_operatorCapture_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_operator_capture);
            cb_operator_capture_CheckedChanged(sender, e);
        }

        private void cb_record_upon_start_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_record_upon_start, pictureBox_recordUponStart);
        }

        private void pictureBox_recordUponStart_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_record_upon_start);
            cb_record_upon_start_CheckedChanged(sender, e);
        }

        private void cb_recording_during_facerec_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_recording_during_facerec, pictureBox_recordingDuringFaceRecognition);
        }

        private void pictureBox_recordingDuringFaceRecognition_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_recording_during_facerec);
            cb_recording_during_facerec_CheckedChanged(sender, e);
        }

        private void cb_backlight_off_idling_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_backlight_off_idling, pictureBox_backlightOffWhenIdle);
        }

        private void pictureBox_backlightOffWhenIdle_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_backlight_off_idling);
            cb_backlight_off_idling_CheckedChanged(sender, e);
        }

        private void cb_event_recorder_CheckedChanged(object sender, EventArgs e)
        {
            Img_Chenge(cb_event_recorder, pictureBox_EventRecorder);
        }

        private void pictureBox_EventRecorder_Click(object sender, EventArgs e)
        {
            CB_Chenge(cb_event_recorder);
            cb_event_recorder_CheckedChanged(sender, e);
        }


        //Function ----> PictureBox Chenge Action
        private void Img_Chenge(CheckBox ChkObj, PictureBox ImgObj)
        {
            if (ChkObj.Checked == true)
            {
                ImgObj.Image = Properties.Resources.check;
            }
            else
            {
                ImgObj.Image = Properties.Resources.checkered;
            }
        }

        //Function ----> CheckBox Chenge Action
        private void CB_Chenge(CheckBox ChBox)
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

        }

    }
}
