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


namespace FaceDetection
{
    public partial class settingsUI : Form
    {
       
        public settingsUI()
        {
            InitializeComponent();
            
            

            /*cm_language.SelectedIndex = 0;
            tb_camera_count.Text = Properties.Settings.Default.camera_count.ToString();
            cb_all_cameras.Checked = Properties.Settings.Default.show_all_cams_simulteneously;
            cb_delete_old.Checked = Properties.Settings.Default.erase_old;
            tb_days_old.Text = Properties.Settings.Default.erase_after.ToString();
            cm_camera_number.SelectedIndex = Properties.Settings.Default.current_camera_index;
            tb_x.Text = Properties.Settings.Default.display_pos_x.ToString();
            tb_y.Text = Properties.Settings.Default.display_pos_y.ToString();
            tb_cam_view_width.Text = Properties.Settings.Default.view_width.ToString();
            tb_cam_view_height.Text = Properties.Settings.Default.view_height.ToString();
            tb_frame_rate.Text = Properties.Settings.Default.frame_rate_fps.ToString();
            cb_always_on_top.Checked = Properties.Settings.Default.window_on_top;
            cb_window_pane.Checked = Properties.Settings.Default.show_window_pane;
            cb_dateandtime.Checked = Properties.Settings.Default.show_current_datetime;
            cb_show_camera_number.Checked = Properties.Settings.Default.show_camera_no;
            cb_show_rec_icon.Checked = Properties.Settings.Default.show_recording_icon;
            cb_face_recognition.Checked = Properties.Settings.Default.enable_face_recognition;
            tb_recognition_interval.Text = Properties.Settings.Default.face_rec_interval.ToString();
            tb_manual_recording_time.Text = Properties.Settings.Default.manual_record_maxtime.ToString();
            cb_backlight_on_recognition.Checked = Properties.Settings.Default.backlight_on_upon_face_rec;
            cb_operator_capture.Checked = Properties.Settings.Default.capture_operator;
            cm_capture_mode.SelectedIndex = Properties.Settings.Default.capture_type;
            tb_capture_seconds.Text = Properties.Settings.Default.recording_length_seconds.ToString();
            cb_record_upon_start.Checked = Properties.Settings.Default.recording_on_start;
            tb_record_reinitiation_interval_seconds.Text = Properties.Settings.Default.interval_between_reinitiating_recording.ToString();
            cb_event_recorder.Checked = Properties.Settings.Default.event_recorder_on;
            tb_pre_event_seconds.Text = Properties.Settings.Default.seconds_before_event.ToString();
            tb_post_event_seconds.Text = Properties.Settings.Default.seconds_after_event.ToString();
            tb_backlight_off_idling_delay_seconds.Text = Properties.Settings.Default.backlight_offset_mins.ToString();
            */
        }

       
        
        private void closeSettings(object sender, EventArgs e)
        {
            this.Close();
        }

        private void save_and_close(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            
            this.Close();
        }

        private void captureIntervalChanged(object sender, EventArgs e)
        {
            MainForm.captureIntervalChange();
        }
        private void frameRateChanged(object sender, EventArgs e)
        {
            MainForm.frameRateChange();
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
            }
        }

        private void alwaysOnTop(object sender, EventArgs e)
        {
            
            Properties.Settings.Default.Save();
            MainForm.alwaysOnTop();
        }

        private void windowBorderStyle(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            MainForm.windowBorderStyle();
        }

        private void recordingIcon(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            MainForm.CameraButton_Click();
        }

        private void currentDate(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            MainForm.currentDate();
        }

        private void cameraNo(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            MainForm.cameraNoShow();
        }

        private void cameraSelected(object sender, EventArgs e)
        {
            /*
             * Selected camera is set as the current in settings section. 
             * The relevanty settings are only applied to the active camera
             */
        }
    }
}
