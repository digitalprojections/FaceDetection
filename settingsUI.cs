using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        private bool fullScreenCbStateC1, fullScreenCbStateC2, fullScreenCbStateC3, fullScreenCbStateC4;
        private bool alwaysOnTopCbStateC1, alwaysOnTopCbStateC2, alwaysOnTopCbStateC3, alwaysOnTopCbStateC4;
        private bool showWindowPanelCbStateC1, showWindowPanelCbStateC2, showWindowPanelCbStateC3, showWindowPanelCbStateC4;
        private bool showDateCbStateC1, showDateCbStateC2, showDateCbStateC3, showDateCbStateC4;
        private bool showCamNumberCbStateC1, showCamNumberCbStateC2, showCamNumberCbStateC3, showCamNumberCbStateC4;
        private bool showRecIconCbStateC1, showRecIconCbStateC2, showRecIconCbStateC3, showRecIconCbStateC4;
        private string resolutionC1, resolutionC2, resolutionC3, resolutionC4;
        private string fpsC1, fpsC2, fpsC3, fpsC4;
        private decimal c1w, c2w, c3w, c4w, c1h, c2h, c3h, c4h;

        /// <summary>
        /// current camera, not the Main Camera
        /// </summary>
        private int currentCameraIndex = 0;
        private int formWhereSettingsWasOpened = 0;
        /// <summary>
        /// private field MAIN Camera
        /// </summary>
        private int cameraindex;
        /// <summary>
        /// Only set true when the camera number is selected manually
        /// </summary>
        //private bool cameraSelectedManually;
        private delegate void dSettingFromProperties();

        public SettingsUI()
        {
            InitializeComponent();
            Properties.Settings.Default.Reload();
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location + "/Camera");
                Properties.Settings.Default.temp_folder = @"D:\TEMP";
                Directory.CreateDirectory(Properties.Settings.Default.temp_folder);
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
            SetMinMaxValues();
        }

        /// <summary>
        /// Get from properties or set the MAIN CAMERA INDEX (Camera facing the user)
        /// </summary>
        //public int Camera_index
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.main_camera_index;
        //    }
        //    private set
        //    {
        //        Properties.Settings.Default.main_camera_index = value;
        //        //Properties.Settings.Default.Save();
        //        //cameraindex = value;
        //    }
        //}

        //public int getCameraIndexSelected()
        //{
        //    return selected_camera_combo.SelectedIndex;
        //}

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
                selected_camera_combo.SelectedIndex = Properties.Settings.Default.main_camera_index <=0 ? 0 : currentCameraIndex;
            }
            //else
            //{
            //    Properties.Settings.Default.main_camera_index = 0;
            //}
        }

        private void ChangeStoreLocation(object sender, EventArgs e)
        {
            folderBrowserDialogStoreFolder.ShowNewFolderButton = true;
            folderBrowserDialogStoreFolder.Description = Resource.store_location_description;
            folderBrowserDialogStoreFolder.SelectedPath = Properties.Settings.Default.video_file_location;
            
            DialogResult result = folderBrowserDialogStoreFolder.ShowDialog();
            if (result == DialogResult.OK && this.CanFocus)
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
                //this.TopMost = false;
                ChangeStoreLocation(sender, e);
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message);
            }
        }

        public void ForgetAndClose()
        {
            Properties.Settings.Default.Reload();
            Hide();
            
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
            Properties.Settings.Default.C1_full_screen = fullScreenCbStateC1;
            Properties.Settings.Default.C2_full_screen = fullScreenCbStateC2;
            Properties.Settings.Default.C3_full_screen = fullScreenCbStateC3;
            Properties.Settings.Default.C4_full_screen = fullScreenCbStateC4;
            Properties.Settings.Default.C1_window_on_top = alwaysOnTopCbStateC1;
            Properties.Settings.Default.C2_window_on_top = alwaysOnTopCbStateC2;
            Properties.Settings.Default.C3_window_on_top = alwaysOnTopCbStateC3;
            Properties.Settings.Default.C4_window_on_top = alwaysOnTopCbStateC4;
            Properties.Settings.Default.C1_show_window_pane = showWindowPanelCbStateC1;
            Properties.Settings.Default.C2_show_window_pane = showWindowPanelCbStateC2;
            Properties.Settings.Default.C3_show_window_pane = showWindowPanelCbStateC3;
            Properties.Settings.Default.C4_show_window_pane = showWindowPanelCbStateC4;
            Properties.Settings.Default.C1_show_date_time = showDateCbStateC1;
            Properties.Settings.Default.C2_show_date_time = showDateCbStateC2;
            Properties.Settings.Default.C3_show_date_time = showDateCbStateC3;
            Properties.Settings.Default.C4_show_date_time = showDateCbStateC4;
            Properties.Settings.Default.C1_show_camera_number = showCamNumberCbStateC1;
            Properties.Settings.Default.C2_show_camera_number = showCamNumberCbStateC2;
            Properties.Settings.Default.C3_show_camera_number = showCamNumberCbStateC3;
            Properties.Settings.Default.C4_show_camera_number = showCamNumberCbStateC4;
            Properties.Settings.Default.C1_show_record_icon = showRecIconCbStateC1;
            Properties.Settings.Default.C2_show_record_icon = showRecIconCbStateC2;
            Properties.Settings.Default.C3_show_record_icon = showRecIconCbStateC3;
            Properties.Settings.Default.C4_show_record_icon = showRecIconCbStateC4;
            Properties.Settings.Default.C1f = fpsC1;
            Properties.Settings.Default.C2f = fpsC2;
            Properties.Settings.Default.C3f = fpsC3;
            Properties.Settings.Default.C4f = fpsC4;
            Properties.Settings.Default.C1res = resolutionC1;
            Properties.Settings.Default.C2res = resolutionC2;
            Properties.Settings.Default.C3res = resolutionC3;
            Properties.Settings.Default.C4res = resolutionC4;
            Properties.Settings.Default.C1w = c1w;
            Properties.Settings.Default.C2w = c2w;
            Properties.Settings.Default.C3w = c3w;
            Properties.Settings.Default.C4w = c4w;
            Properties.Settings.Default.C1h = c1h;
            Properties.Settings.Default.C2h = c2h;
            Properties.Settings.Default.C3h = c3h;
            Properties.Settings.Default.C4h = c4h;
        }

        private void CloseSettings(object sender, EventArgs e)
        {
            ForgetAndClose();
        }

        private void SaveAndClose(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.SetFPS(currentCameraIndex, comboBoxFPS.SelectedItem?.ToString());
            Properties.Settings.Default.Save();

            if (String.IsNullOrEmpty(storePath.Text))
            {
                if (Directory.Exists(@"D:\TEMP"))
                {
                    storePath.Text = @Resource.d_drive_location;
                }
                else
                {
                    storePath.Text = @Resource.c_drive_location;
                }
                storePath.SelectionStart = storePath.Text.Length;
            }
            else // Delete last character if it is a space
            {
                char[] characters = storePath.Text.ToCharArray();
                if(char.IsWhiteSpace(characters[characters.Length-1]))
                {
                    storePath.Text = storePath.Text.Substring(0, storePath.Text.Length-1);
                }
            }

            Properties.Settings.Default.main_camera_index = cm_camera_number.SelectedIndex;
            for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MULTI_WINDOW.formList[0].secondsBeforeTriggerEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                        MULTI_WINDOW.formList[0].secondsAfterTriggerEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                        MULTI_WINDOW.formList[0].intervalAfterEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording);
                        break;
                    case 1:
                        MULTI_WINDOW.formList[1].secondsBeforeTriggerEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
                        MULTI_WINDOW.formList[1].secondsAfterTriggerEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
                        MULTI_WINDOW.formList[1].intervalAfterEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording);
                        break;
                    case 2:
                        MULTI_WINDOW.formList[2].secondsBeforeTriggerEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
                        MULTI_WINDOW.formList[2].secondsAfterTriggerEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
                        MULTI_WINDOW.formList[2].intervalAfterEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording);
                        break;
                    case 3:
                        MULTI_WINDOW.formList[3].secondsBeforeTriggerEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
                        MULTI_WINDOW.formList[3].secondsAfterTriggerEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
                        MULTI_WINDOW.formList[3].intervalAfterEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording);
                        break;
                }

                if (MULTI_WINDOW.formList[i].DISPLAYED == true)
                {
                    PROPERTY_FUNCTIONS.Set_Window_Location_Set(i, MULTI_WINDOW.formList[i]);
                    //MULTI_WINDOW.formList[i].ClientSize = PROPERTY_FUNCTIONS.GetCameraSize(i);
                    MULTI_WINDOW.formList[i].ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(i);
                }
            }

            MainForm.GetMainForm.BackLight.Start();
            Camera.CountCamera();
            Hide();
            MainForm.AllChangesApply();
        }

        private void SetCameraPropertiesFromMemory()
        {
            if (this.InvokeRequired)
            {
                var d = new dSettingFromProperties(SetCameraPropertiesFromMemory);
                this.Invoke(d);
            }
            else
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
                checkBox_full_screen.DataBindings.Clear();
                cb_always_on_top.DataBindings.Clear();
                cb_dateandtime.DataBindings.Clear();
                cb_window_pane.DataBindings.Clear();
                cb_show_camera_number.DataBindings.Clear();
                cb_show_rec_icon.DataBindings.Clear();

                string camX = "C" + (currentCameraIndex + 1) + "x";
                string camY = "C" + (currentCameraIndex + 1) + "y";
                string camW = "C" + (currentCameraIndex + 1) + "w";
                string camH = "C" + (currentCameraIndex + 1) + "h";
                string camF = "C" + (currentCameraIndex + 1) + "f";
                string camRes = "C" + (currentCameraIndex + 1) + "res";
                string camFullScreen = "C" + (currentCameraIndex + 1) + "_full_screen";
                string camOnTop = "C" + (currentCameraIndex + 1) + "_window_on_top";
                string camDateTime = "C" + (currentCameraIndex + 1) + "_show_date_time";
                string camWindowPane = "C" + (currentCameraIndex + 1) + "_show_window_pane";
                string camNumber = "C" + (currentCameraIndex + 1) + "_show_camera_number";
                string camRecordIcon = "C" + (currentCameraIndex + 1) + "_show_record_icon";

                numericUpDownX.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camX, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownY.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camY, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownW.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camW, true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDownH.DataBindings.Add(new Binding("Value", Properties.Settings.Default, camH, true, DataSourceUpdateMode.OnPropertyChanged));
                comboBoxFPS.DataBindings.Add(new Binding("Text", Properties.Settings.Default, camF, true, DataSourceUpdateMode.OnPropertyChanged));
                comboBoxResolutions.DataBindings.Add(new Binding("Text", Properties.Settings.Default, camRes, true, DataSourceUpdateMode.OnPropertyChanged));
                checkBox_full_screen.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camFullScreen, true, DataSourceUpdateMode.OnPropertyChanged));
                cb_always_on_top.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camOnTop, true, DataSourceUpdateMode.OnPropertyChanged));
                cb_dateandtime.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camDateTime, true, DataSourceUpdateMode.OnPropertyChanged));
                cb_window_pane.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camWindowPane, true, DataSourceUpdateMode.OnPropertyChanged));
                cb_show_camera_number.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camNumber, true, DataSourceUpdateMode.OnPropertyChanged));
                cb_show_rec_icon.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, camRecordIcon, true, DataSourceUpdateMode.OnPropertyChanged));

                bool fs = !PROPERTY_FUNCTIONS.CheckFullScreenByIndex(currentCameraIndex);
                gbWindowPosition.Enabled = fs;
                gbWindowSize.Enabled = fs;

                cm_capture_mode.DataBindings.Add(new Binding("Text", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_capture_type", true, DataSourceUpdateMode.OnPropertyChanged));
                cb_event_recorder.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_enable_event_recorder", true, DataSourceUpdateMode.OnPropertyChanged));
                event_record_time_before_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_event_record_time_before_event", true, DataSourceUpdateMode.OnPropertyChanged));
                nud_event_record_after.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_event_record_time_after_event", true, DataSourceUpdateMode.OnPropertyChanged));
                nud_seconds_before_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_seconds_before_event", true, DataSourceUpdateMode.OnPropertyChanged));
                nud_seconds_after_event.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_seconds_after_event", true, DataSourceUpdateMode.OnPropertyChanged));
                numericUpDown2.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_check_interval", true, DataSourceUpdateMode.OnPropertyChanged));
                nud_reinitiation_interval.DataBindings.Add(new Binding("Value", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_interval_before_reinitiating_recording", true, DataSourceUpdateMode.OnPropertyChanged));
                cb_face_recognition.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_enable_face_recognition", true, DataSourceUpdateMode.OnPropertyChanged));
                cb_human_sensor.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_enable_Human_sensor", true, DataSourceUpdateMode.OnPropertyChanged));
                cb_recording_operation.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_Recording_when_at_the_start_of_operation", true, DataSourceUpdateMode.OnPropertyChanged));
                cb_operator_capture.DataBindings.Add(new Binding("Checked", Properties.Settings.Default, "C" + (currentCameraIndex + 1) + "_enable_capture_operator", true, DataSourceUpdateMode.OnPropertyChanged));
            }
        }

        private void ComboBoxResolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            //PROPERTY_FUNCTIONS.resolution_changed = true;
            if (comboBoxResolutions.SelectedItem != null)
            {
                PROPERTY_FUNCTIONS.SetResolution(currentCameraIndex, comboBoxResolutions.SelectedItem.ToString());
                SettingsUI.SetComboBoxFPSValues(MULTI_WINDOW.formList[currentCameraIndex].videoFormat, currentCameraIndex);
            }
        }

        private void comboBoxFPS_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBoxFPS.SelectedItem!=null)
            //{
            //    PROPERTY_FUNCTIONS.SetFPS(currentCameraIndex, comboBoxFPS.SelectedItem?.ToString());
            //}
        }

        /// <summary>
        /// The ComboBox class searches for the specified object by using the IndexOf method. 
        /// This method uses the Equals method to determine equality.
        /// </summary>
        /// <param name="vs"></param>
        public static void SetComboBoxResolutionValues(List<string> vs, int cameraIndex)
        {
            resolutions_combo.Items.Clear();
            if (resolutions_combo != null)
            {
                resolutions_combo.Items.AddRange(vs.ToArray());
                if (resolutions_combo.Items.Count > 0)
                {
                    if (PROPERTY_FUNCTIONS.GetResolution(cameraIndex) != null)
                    {
                        resolutions_combo.SelectedItem = PROPERTY_FUNCTIONS.GetResolution(cameraIndex);
                    }
                }
            }

            if (resolutions_combo.SelectedItem!=null)
            {
                SettingsUI.SetComboBoxFPSValues(MULTI_WINDOW.formList[cameraIndex].videoFormat, cameraIndex);
            }
        }

        public static void SetComboBoxFPSValues(FaceDetectionX.UsbCamera.VideoFormat[] vs, int cameraIndex)
        {
            frame_rates_combo.Items.Clear();
            var matching_fps_found = false;
            var vsPicked = new List<string>();
            string found_fps = "";

            if (frame_rates_combo != null)
            {
                //frame_rates_combo.Items.AddRange(vs);
                Size size = PROPERTY_FUNCTIONS.Get_Stored_Resolution(cameraIndex);
                for (int i = 0; i < vs.Length; i++)
                {
                    if (vs[i].Size.Width == size.Width && vs[i].Size.Height == size.Height)
                    {
                        matching_fps_found = true;
                        found_fps = Convert.ToString(10000000 / vs[i].TimePerFrame);
                        vsPicked.Add(found_fps);
                        break;
                    }
                }
                frame_rates_combo.Items.AddRange(vsPicked.ToArray());
                if (vsPicked.Count > 0)
                {
                    if (found_fps == PROPERTY_FUNCTIONS.GetFPS(cameraIndex))
                    {
                        frame_rates_combo.SelectedItem = found_fps;
                    }
                    else
                    {
                        //value not found, set whatever applicable: 0 index
                        frame_rates_combo.SelectedItem = vsPicked[0];
                    }
                }
                
            }
            if (!matching_fps_found)
            {
                //PROPERTY_FUNCTIONS.SetFPS(cameraIndex, vsPicked[0]);
            }
        }

        //void CameraSetAsMain(object sender, EventArgs e)
        //{
        //    CameraSetAsMain();
        //}

        //void CameraSetAsMain() { 
        //    if (CBSetAsMainCam.Checked && Properties.Settings.Default.main_camera_index != cm_camera_number.SelectedIndex)
        //    {
        //        Camera_index = cm_camera_number.SelectedIndex;
        //        cm_camera_number.Enabled = false;
        //    }
        //    else
        //    {
        //        cm_camera_number.Enabled = true;
        //        Camera_index = MainCameraBeforeSettingsLoad;
        //    }
        //    labelCameraNumber.Text = (Camera_index + 1).ToString();
        //}


        private void SettingsUI_Load(object sender, EventArgs e)
        {
            Camera.SetNumberOfCameras();
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
            fullScreenCbStateC1 = Properties.Settings.Default.C1_full_screen;
            fullScreenCbStateC2 = Properties.Settings.Default.C2_full_screen;
            fullScreenCbStateC3 = Properties.Settings.Default.C3_full_screen;
            fullScreenCbStateC4 = Properties.Settings.Default.C4_full_screen;
            alwaysOnTopCbStateC1 = Properties.Settings.Default.C1_window_on_top;
            alwaysOnTopCbStateC2 = Properties.Settings.Default.C2_window_on_top;
            alwaysOnTopCbStateC3 = Properties.Settings.Default.C3_window_on_top;
            alwaysOnTopCbStateC4 = Properties.Settings.Default.C4_window_on_top;
            showWindowPanelCbStateC1 = Properties.Settings.Default.C1_show_window_pane;
            showWindowPanelCbStateC2 = Properties.Settings.Default.C2_show_window_pane;
            showWindowPanelCbStateC3 = Properties.Settings.Default.C3_show_window_pane;
            showWindowPanelCbStateC4 = Properties.Settings.Default.C4_show_window_pane;
            showDateCbStateC1 = Properties.Settings.Default.C1_show_date_time;
            showDateCbStateC2 = Properties.Settings.Default.C2_show_date_time;
            showDateCbStateC3 = Properties.Settings.Default.C3_show_date_time;
            showDateCbStateC4 = Properties.Settings.Default.C4_show_date_time;
            showCamNumberCbStateC1 = Properties.Settings.Default.C1_show_camera_number;
            showCamNumberCbStateC2 = Properties.Settings.Default.C2_show_camera_number;
            showCamNumberCbStateC3 = Properties.Settings.Default.C3_show_camera_number;
            showCamNumberCbStateC4 = Properties.Settings.Default.C4_show_camera_number;
            showRecIconCbStateC1 = Properties.Settings.Default.C1_show_record_icon;
            showRecIconCbStateC2 = Properties.Settings.Default.C2_show_record_icon;
            showRecIconCbStateC3 = Properties.Settings.Default.C3_show_record_icon;
            showRecIconCbStateC4 = Properties.Settings.Default.C4_show_record_icon;
            resolutionC1 = Properties.Settings.Default.C1res;
            resolutionC2 = Properties.Settings.Default.C2res;
            resolutionC3 = Properties.Settings.Default.C3res;
            resolutionC4 = Properties.Settings.Default.C4res;
            fpsC1 = Properties.Settings.Default.C1f;
            fpsC2 = Properties.Settings.Default.C2f;
            fpsC3 = Properties.Settings.Default.C3f;
            fpsC4 = Properties.Settings.Default.C4f;
            c1w = Properties.Settings.Default.C1w;
            c2w = Properties.Settings.Default.C2w;
            c3w = Properties.Settings.Default.C3w;
            c4w = Properties.Settings.Default.C4w;
            c1h = Properties.Settings.Default.C1h;
            c2h = Properties.Settings.Default.C2h;
            c3h = Properties.Settings.Default.C3h;
            c4h = Properties.Settings.Default.C4h;

            if (cm_camera_number.Items.Count > 0)
            {
                cm_camera_number.SelectedIndex = Properties.Settings.Default.main_camera_index <= 0 ? 0 : Properties.Settings.Default.main_camera_index;
                //cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_method <= 0 ? 0 : Properties.Settings.Default.capture_method;
            }

            cm_language.SelectedItem = Properties.Settings.Default.language;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Properties.Settings.Default.culture);
            //CBSetAsMainCam.Text = Resource.setAsMainCam;
            ChangeLanguage();
            Debug.WriteLine(CultureInfo.CurrentCulture + " current culture");
            this.Text = Resource.settingsWindowTitle;

            //numericUpDownCamCount.Maximum = Camera.GetCameraCount().Length;

            //SetCameraPropertiesFromMemory();

            if (currentCameraIndex == 0)
            { 
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C1_enable_capture_operator, currentCameraIndex);
            }
            else if (currentCameraIndex == 1)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C2_enable_capture_operator, currentCameraIndex);
            }
            else if (currentCameraIndex == 2)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C3_enable_capture_operator, currentCameraIndex);
            }
            else if (currentCameraIndex == 3)
            {
                ChangeControlEnabled(this.groupBox_functionalitySettings, Properties.Settings.Default.C4_enable_capture_operator, currentCameraIndex);
            }

            this.ControlBox = false;
            this.MaximizeBox = false;

            SetOKButtonState();
        }

        public void SetOKButtonState()
        {
            bool rip = MainForm.AnyRecordingInProgress;
            cm_camera_number.Enabled = !rip;
            button_settings_save.Enabled = !rip;
        }

        public void DisabledButtonWhenRecording()
        {
            bool rip = MainForm.AnyRecordingInProgress;
            
            cm_camera_number.Enabled = !rip;
            //CBSetAsMainCam.Enabled = !rip;
            button_settings_save.Enabled = !rip;
            //if (cb_operator_capture.CheckState == CheckState.Unchecked && cb_event_recorder.CheckState==CheckState.Unchecked)
            //{
            //    //User wants to stop recordings and set the app to no buffer mode. Simply allow OK button
            //    button_settings_save.Enabled = true;
            //}            
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
                System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.culture);
                this.Text = Resource.settingsWindowTitle;
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
            else if (cb_operator_capture.Checked == true && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked))
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

        private void Button_cameraProperties_Click(object sender, EventArgs e)
        {
            DisplayPropertyPage();
        }

        /// <summary>
        /// Displays a property page for the current Camera
        /// </summary>
        /// <param name="dev">The filter for which to display a property page</param>
        public void DisplayPropertyPage()
        {            
            var dev = (IBaseFilter)DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, currentCameraIndex);

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
            hr = DLLIMPORT.OleCreatePropertyFrame(this.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
            Marshal.ReleaseComObject(pProp);
            //Marshal.ReleaseComObject(filterInfo);
            Marshal.ReleaseComObject(dev);
        }

        //internal void _DisplayPropertyPage()
        //{
        //    var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, currentCameraIndex);
        //    var dev = DirectShow.FindPin(filter, currentCameraIndex, DirectShow.PIN_DIRECTION.PINDIR_OUTPUT);

        //    //var dev = (IBaseFilter)DirectShow.CreateFilter(DirectShow.DsGuid., currentCameraIndex);
        //    if (dev == null)
        //        return;

        //    //Get the ISpecifyPropertyPages for the filter
        //    ISpecifyPropertyPages pProp = dev as ISpecifyPropertyPages;
        //    int hr = 0;

        //    if (pProp == null)
        //    {
        //        //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
        //        IAMVfwCompressDialogs compressDialog = dev as IAMVfwCompressDialogs;
        //        if (compressDialog != null)
        //        {

        //            hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
        //            DsError.ThrowExceptionForHR(hr);
        //        }
        //        return;
        //    }

        //    string caption = string.Empty;

        //    if (dev is IBaseFilter)
        //    {
        //        //Get the name of the filter from the FilterInfo struct
        //        IBaseFilter as_filter = dev as IBaseFilter;
        //        FilterInfo filterInfo;
        //        hr = as_filter.QueryFilterInfo(out filterInfo);
        //        DsError.ThrowExceptionForHR(hr);

        //        caption = filterInfo.achName;

        //        if (filterInfo.pGraph != null)
        //        {
        //            Marshal.ReleaseComObject(filterInfo.pGraph);
        //        }
        //    }
        //    else
        //    if (dev is IPin)
        //    {
        //        //Get the name of the filter from the FilterInfo struct
        //        IPin as_pin = dev as IPin;
        //        PinInfo pinInfo;
        //        hr = as_pin.QueryPinInfo(out pinInfo);
        //        DsError.ThrowExceptionForHR(hr);

        //        caption = pinInfo.name;
        //    }


        //    // Get the propertypages from the property bag
        //    DsCAUUID caGUID;
        //    hr = pProp.GetPages(out caGUID);
        //    DsError.ThrowExceptionForHR(hr);

        //    // Create and display the OlePropertyFrame
        //    object oDevice = (object)dev;
        //    hr = DLLIMPORT.OleCreatePropertyFrame(this.Handle, 0, 0, caption, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
        //    DsError.ThrowExceptionForHR(hr);

        //    // Release COM objects
        //    Marshal.FreeCoTaskMem(caGUID.pElems);
        //    Marshal.ReleaseComObject(pProp);
        //}

        private void Cb_delete_old_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.ManageDeleteOldFilesTimer(cb_delete_old.Checked);
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
                //PROPERTY_FUNCTIONS.Set_Human_Sensor(cam_index, false);
                //PROPERTY_FUNCTIONS.Set_Face_Switch(cam_index, false);
                //PROPERTY_FUNCTIONS.SetOnOperationStartSwitch(cam_index, false);
                PROPERTY_FUNCTIONS.DisableOperatorCaptureCheckBoxIfNeeded();
            }
        }

        private void CheckBox_full_screen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;

            gbWindowPosition.Enabled = !cbx.Checked;
            gbWindowSize.Enabled = !cbx.Checked;
        }

        private void Cb_operator_capture_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                CheckBox chb = (CheckBox)sender;
                bool chk = chb.Checked;

                //ChangeControlEnabled(this.groupBox_functionalitySettings, chk, currentCameraIndex);
                if (!chk)
                {
                    cb_human_sensor.Enabled = false;
                    cb_face_recognition.Enabled = false;
                    cb_recording_operation.Enabled = false;
                    nud_reinitiation_interval.Enabled = false;
                    numericUpDown2.Enabled = false;
                    cm_capture_mode.Enabled = false;
                    label17.Enabled = false;
                    label10.Enabled = false;
                    label12.Enabled = false;
                    label11.Enabled = false;

                    PROPERTY_FUNCTIONS.Set_Human_Sensor(currentCameraIndex, false);
                    PROPERTY_FUNCTIONS.Set_Face_Switch(currentCameraIndex, false);
                    PROPERTY_FUNCTIONS.SetOnOperationStartSwitch(currentCameraIndex, false);
                }
                else
                {
                    cb_human_sensor.Enabled = true;
                    cb_face_recognition.Enabled = true;
                    cb_recording_operation.Enabled = true;
                    nud_reinitiation_interval.Enabled = true;
                    numericUpDown2.Enabled = true;
                    cm_capture_mode.Enabled = true;
                    label17.Enabled = true;
                    label10.Enabled = true;
                    label12.Enabled = true;
                    label11.Enabled = true;
                }

                PROPERTY_FUNCTIONS.GetCaptureMethod(currentCameraIndex, out string capturemethod);
                if (cb_operator_capture.Checked && (cb_human_sensor.Checked || cb_face_recognition.Checked || cb_recording_operation.Checked) && capturemethod == "Video")
                {
                    nud_seconds_before_event.Enabled = true;
                    nud_seconds_after_event.Enabled = true;
                }
                else
                {
                    nud_seconds_before_event.Enabled = false;
                    nud_seconds_after_event.Enabled = false;
                }

                DisabledButtonWhenRecording();
            }
        }

        private void Cb_human_sensor_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                CheckBox check = (CheckBox)sender;
                if (check.Checked)
                {
                    //cb_face_recognition.Checked = !check.Checked;
                    if (Properties.Settings.Default.C1_enable_Human_sensor == true)
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
        }

        private void Cb_face_recognition_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
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
        }

        private void Cb_recording_operation_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                CheckBox check = (CheckBox)sender;
                if (!check.Checked)
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
        }

        private void ResetPreeventTime(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (!checkBox.Checked)
            {
                //event_record_time_before_event.Value = 0;
                DisabledButtonWhenRecording();
            }
        }

        private void numericUpDownW_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.KeyValue);
            if (BlockedKeys(e.KeyValue))
            {
                e.SuppressKeyPress = true;
            }
        }

        bool BlockedKeys(int key)
        {
            var retval = false;
            int [] blockedKeys = { 188, 190, 110, 189, 109 };

            foreach (int x in blockedKeys)
            {
                if (x==key)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private void numericUpDownCamCount_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown cameracount_nud = (NumericUpDown)sender;
            if (Properties.Settings.Default.main_camera_index + 1 > cameracount_nud.Value) 
                // The number of cameras explude the current MAIN CAMERA index
            {
                Properties.Settings.Default.main_camera_index = 0;
            }
        }

        private void DisableOperatorCaptureCheckBox_ifNeeded()
        {
            //PROPERTY_FUNCTIONS.SetCaptureOperatorSwitchImplicitly(currentCameraIndex);
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
        
        //private void Nud_reinitiation_interval_ValueChanged(object sender, EventArgs e)
        //{
        //    //SetMaxValues();
        //    //SetMaxValues(GetTheMaxValue());
        //}

        private void event_record_time_before_event_ValueChanged(object sender, EventArgs e)
        {
            //SetMinMaxValues();
        }

        private void Nud_seconds_before_event_ValueChanged(object sender, EventArgs e)
        {            
            SetMinMaxValues();
        }

        //void SetMaxValues()
        //{
        //}

        void SetMinMaxValues()
        {
            //if (nud_seconds_before_event.Value > 0)
            //{
            //   nud_reinitiation_interval.Minimum = nud_seconds_before_event.Value;
            //}
            //else
            //{
            if (nud_seconds_before_event.Value == 0)
            {
                nud_reinitiation_interval.Minimum = 1;
            }
        }

        //decimal GetTheMaxValue()
        //{
        //    return Math.Max(event_record_time_before_event.Value, nud_seconds_before_event.Value);
        //}

        private void Nud_reinitiation_interval_MouseUp(object sender, MouseEventArgs e)
        {
           SetIntervalProps();
        }

        private void Nud_seconds_before_event_MouseUp(object sender, MouseEventArgs e)
        {
            SetIntervalProps();
        }

        void SetIntervalProps()
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

        private void Nud_seconds_before_event_KeyUp(object sender, KeyEventArgs e)
        {
            decimal event_value_Keyup = (decimal)nud_seconds_before_event.Value;
            decimal interval_value_Keyup = (decimal)nud_reinitiation_interval.Value;

            if (event_value_Keyup > interval_value_Keyup)
            {
                Properties.Settings.Default.C1_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C2_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C3_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C4_interval_before_reinitiating_recording = event_value_Keyup;
            }
        }

        private void Nud_reinitiation_interval_KeyUp(object sender, KeyEventArgs e)
        {
            decimal event_value_Keyup = (decimal)nud_seconds_before_event.Value;
            decimal interval_value_Keyup = (decimal)nud_reinitiation_interval.Value;

            if (event_value_Keyup > interval_value_Keyup)
            {
                Properties.Settings.Default.C1_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C2_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C3_interval_before_reinitiating_recording = event_value_Keyup;
                Properties.Settings.Default.C4_interval_before_reinitiating_recording = event_value_Keyup;
            }
        }

        /// <summary>
        /// Select a camera number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraSelected(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            //CBSetAsMainCam.Checked = (Properties.Settings.Default.main_camera_index == comboBox.SelectedIndex);
            //CBSetAsMainCam.Enabled = !(Properties.Settings.Default.main_camera_index == comboBox.SelectedIndex);

            //if (cameraSelectedManually)
            //{
                currentCameraIndex = comboBox.SelectedIndex;
                //cameraSelectedManually = false;
            //}
            labelCameraNumber.Text = (currentCameraIndex + 1).ToString(); // (Properties.Settings.Default.main_camera_index + 1).ToString();
            SetCameraPropertiesFromMemory();
            MULTI_WINDOW.GetVideoFormatByCamera(currentCameraIndex);
            numericUpDownCamCount.Minimum = Properties.Settings.Default.main_camera_index + 1;
        }

        /// <summary>
        /// Visibility changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsUI_Shown(object sender, EventArgs e)
        {
            this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(formWhereSettingsWasOpened);
            if (this.Visible)
            {
                DisableOperatorCaptureCheckBox_ifNeeded();
                cm_camera_number.SelectedIndex = formWhereSettingsWasOpened;
            }            
        }

        internal void ShowSettings(int cameraIndex)
        {
            formWhereSettingsWasOpened = cameraIndex;
            for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
            {
                if (MULTI_WINDOW.formList[i].DISPLAYED == true && MULTI_WINDOW.formList[i].WindowState != FormWindowState.Maximized)
                {
                    PROPERTY_FUNCTIONS.Set_Window_Location(i, MULTI_WINDOW.formList[i]);
                }
            }

            MainCameraBeforeSettingsLoad = Properties.Settings.Default.main_camera_index;
            //ShowDialog();
            try
            {
                ShowDialog(MULTI_WINDOW.formList[formWhereSettingsWasOpened]);
            }
            catch (InvalidOperationException ioe)
            {
                // Settings openened by command then OK button
            }
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

        private void cm_camera_number_MouseHover(object sender, EventArgs e)
        {
            //cameraSelectedManually = true;
        }

        private void cm_camera_number_MouseLeave(object sender, EventArgs e)
        {
            //cameraSelectedManually = false;
        }

        //private void tabControl1_Selected(object sender, TabControlEventArgs e)
        //{
        //    cm_camera_number.Enabled = !(e.TabPageIndex == 1 && CBSetAsMainCam.Checked);
        //}

        private void StorePath_TextChanged(object sender, EventArgs e)
        {
            bool pathChanged;
            char[] invalidPathChars = {'\"', '*', '?', '!', '<', '>', '|', ':', '.'};

            char[] characters = storePath.Text.ToCharArray();

            pathChanged = false;

            for (int i = 0; i < characters.Length; i++)
            {
                if (i == 0) // First character must be a letter (disk name), so if not, back to D
                {   
                    if (characters[i] != 'A' && characters[i] != 'B' && characters[i] != 'C' && characters[i] != 'D' && characters[i] != 'E' && characters[i] != 'F'
                        && characters[i] != 'G' && characters[i] != 'H' && characters[i] != 'I' && characters[i] != 'J' && characters[i] != 'K' && characters[i] != 'L'
                        && characters[i] != 'M' && characters[i] != 'N' && characters[i] != 'O' && characters[i] != 'P' && characters[i] != 'Q' && characters[i] != 'R'
                        && characters[i] != 'S' && characters[i] != 'T' && characters[i] != 'U' && characters[i] != 'V' && characters[i] != 'W' && characters[i] != 'X'
                        && characters[i] != 'Y' && characters[i] != 'Z')
                    {
                        if (Directory.Exists(@"D:\TEMP"))
                        {
                            characters[i] = 'D';
                        }
                        else
                        {
                            characters[i] = 'C';
                        }
                        pathChanged = true;
                    }
                }
                else if (i == 1 && characters[i] != ':') // This character must be ":"
                {
                    characters[i] = ':';
                    pathChanged = true;
                }
                else if (i == 2 && characters[i] != '\\') // This character must be "\"
                {
                    characters[i] = '\\';
                    pathChanged = true;
                }
                else if (i > 2)
                { 
                    for (int j = 0; j < invalidPathChars.Length; j++)
                    {
                        if (characters[i].Equals(invalidPathChars[j]))
                        {
                            characters[i] = '\0';
                            pathChanged = true;
                        }
                    }
                }
            }

            if (pathChanged == true)
            {
                storePath.Text = new string(characters);
                storePath.SelectionStart = storePath.Text.Length;
            }
        }

        private void SettingsUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < MULTI_WINDOW.displayedCameraCount; i++)
            {
                if (MULTI_WINDOW.formList[i].DISPLAYED == true && MULTI_WINDOW.formList[i].Location.X > 0)
                {
                    PROPERTY_FUNCTIONS.Set_Window_Location(i, MULTI_WINDOW.formList[i]);
                }
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
    }

    class DLLIMPORT
    {

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