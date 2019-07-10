namespace FaceDetection
{
    partial class settingsUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_all_cameras = new System.Windows.Forms.CheckBox();
            this.cm_language = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_camera_count = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.tb_days_old = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_delete_old = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label25_pcs = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.cb_always_on_top = new System.Windows.Forms.CheckBox();
            this.cb_window_pane = new System.Windows.Forms.CheckBox();
            this.cb_dateandtime = new System.Windows.Forms.CheckBox();
            this.cb_show_camera_number = new System.Windows.Forms.CheckBox();
            this.cb_show_rec_icon = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9_fps = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tb_y = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_x = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_frame_rate = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tb_cam_view_height = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_cam_view_width = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_backlight_on_recognition = new System.Windows.Forms.CheckBox();
            this.lb_manual_recording_time = new System.Windows.Forms.Label();
            this.lb_seconds = new System.Windows.Forms.Label();
            this.tb_manual_recording_time = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cb_face_recognition = new System.Windows.Forms.CheckBox();
            this.tb_recognition_interval = new System.Windows.Forms.TextBox();
            this.lb_milliseconds1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cb_record_upon_start = new System.Windows.Forms.CheckBox();
            this.tb_record_reinitiation_interval_seconds = new System.Windows.Forms.TextBox();
            this.cb_operator_capture = new System.Windows.Forms.CheckBox();
            this.cb_recording_during_facerec = new System.Windows.Forms.CheckBox();
            this.label18_seconds = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cm_capture_mode = new System.Windows.Forms.ComboBox();
            this.tb_capture_seconds = new System.Windows.Forms.TextBox();
            this.lb_seconds2 = new System.Windows.Forms.Label();
            this.lb_capture_seconds = new System.Windows.Forms.Label();
            this.lb_capture_mode = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.cb_event_recorder = new System.Windows.Forms.CheckBox();
            this.tb_post_event_seconds = new System.Windows.Forms.TextBox();
            this.tb_pre_event_seconds = new System.Windows.Forms.TextBox();
            this.lb_post_event = new System.Windows.Forms.Label();
            this.lb_pre_event = new System.Windows.Forms.Label();
            this.label23_seconds = new System.Windows.Forms.Label();
            this.label22_seconds = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cb_backlight_off_idling = new System.Windows.Forms.CheckBox();
            this.tb_backlight_off_idling_delay_seconds = new System.Windows.Forms.TextBox();
            this.lb_backlight_off_idling_delay = new System.Windows.Forms.Label();
            this.label20_minutes = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.button_settings_cancel = new System.Windows.Forms.Button();
            this.button_settings_save = new System.Windows.Forms.Button();
            this.cm_camera_number = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_all_cameras
            // 
            this.cb_all_cameras.AutoSize = true;
            this.cb_all_cameras.Checked = global::FaceDetection.Properties.Settings.Default.show_all_cams_simulteneously;
            this.cb_all_cameras.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_all_cams_simulteneously", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_all_cameras.Location = new System.Drawing.Point(36, 107);
            this.cb_all_cameras.Name = "cb_all_cameras";
            this.cb_all_cameras.Size = new System.Drawing.Size(205, 16);
            this.cb_all_cameras.TabIndex = 5;
            this.cb_all_cameras.Text = "Display all cameras simulteneously";
            this.cb_all_cameras.UseVisualStyleBackColor = true;
            // 
            // cm_language
            // 
            this.cm_language.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "language", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cm_language.FormattingEnabled = true;
            this.cm_language.Items.AddRange(new object[] {
            "English",
            "日本語"});
            this.cm_language.Location = new System.Drawing.Point(130, 27);
            this.cm_language.Name = "cm_language";
            this.cm_language.Size = new System.Drawing.Size(91, 20);
            this.cm_language.TabIndex = 2;
            this.cm_language.Text = global::FaceDetection.Properties.Settings.Default.language;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Display language";
            // 
            // tb_camera_count
            // 
            this.tb_camera_count.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "camera_count", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_camera_count.Location = new System.Drawing.Point(130, 53);
            this.tb_camera_count.Name = "tb_camera_count";
            this.tb_camera_count.Size = new System.Drawing.Size(35, 19);
            this.tb_camera_count.TabIndex = 3;
            this.tb_camera_count.Text = global::FaceDetection.Properties.Settings.Default.camera_count;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox12);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Location = new System.Drawing.Point(245, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 131);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select save path";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.tb_days_old);
            this.groupBox12.Controls.Add(this.label3);
            this.groupBox12.Controls.Add(this.cb_delete_old);
            this.groupBox12.Location = new System.Drawing.Point(6, 62);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(228, 62);
            this.groupBox12.TabIndex = 3;
            this.groupBox12.TabStop = false;
            // 
            // tb_days_old
            // 
            this.tb_days_old.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "erase_after", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_days_old.Location = new System.Drawing.Point(15, 28);
            this.tb_days_old.Name = "tb_days_old";
            this.tb_days_old.Size = new System.Drawing.Size(35, 19);
            this.tb_days_old.TabIndex = 1;
            this.tb_days_old.Text = global::FaceDetection.Properties.Settings.Default.erase_after;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "days old data to be deleted";
            // 
            // cb_delete_old
            // 
            this.cb_delete_old.AutoSize = true;
            this.cb_delete_old.Checked = global::FaceDetection.Properties.Settings.Default.erase_old;
            this.cb_delete_old.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_delete_old.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "erase_old", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_delete_old.Location = new System.Drawing.Point(15, 0);
            this.cb_delete_old.Name = "cb_delete_old";
            this.cb_delete_old.Size = new System.Drawing.Size(102, 16);
            this.cb_delete_old.TabIndex = 0;
            this.cb_delete_old.Text = "Delete old data";
            this.cb_delete_old.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(159, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Save file path";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "video_file_location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(6, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(147, 19);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = global::FaceDetection.Properties.Settings.Default.video_file_location;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label25_pcs);
            this.groupBox1.Controls.Add(this.tb_camera_count);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cm_language);
            this.groupBox1.Controls.Add(this.cb_all_cameras);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 154);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Capture Settings";
            // 
            // label25_pcs
            // 
            this.label25_pcs.AutoSize = true;
            this.label25_pcs.Location = new System.Drawing.Point(171, 56);
            this.label25_pcs.Name = "label25_pcs";
            this.label25_pcs.Size = new System.Drawing.Size(23, 12);
            this.label25_pcs.TabIndex = 4;
            this.label25_pcs.Text = "pcs";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(18, 56);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(106, 12);
            this.label26.TabIndex = 1;
            this.label26.Text = "Connected cameras";
            // 
            // cb_always_on_top
            // 
            this.cb_always_on_top.AutoSize = true;
            this.cb_always_on_top.Checked = global::FaceDetection.Properties.Settings.Default.window_on_top;
            this.cb_always_on_top.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_always_on_top.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "window_on_top", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_always_on_top.Location = new System.Drawing.Point(259, 18);
            this.cb_always_on_top.Name = "cb_always_on_top";
            this.cb_always_on_top.Size = new System.Drawing.Size(97, 16);
            this.cb_always_on_top.TabIndex = 5;
            this.cb_always_on_top.Text = "Always on top";
            this.cb_always_on_top.UseVisualStyleBackColor = true;
            // 
            // cb_window_pane
            // 
            this.cb_window_pane.AutoSize = true;
            this.cb_window_pane.Checked = global::FaceDetection.Properties.Settings.Default.show_window_pane;
            this.cb_window_pane.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_window_pane.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_window_pane", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_window_pane.Location = new System.Drawing.Point(259, 39);
            this.cb_window_pane.Name = "cb_window_pane";
            this.cb_window_pane.Size = new System.Drawing.Size(120, 16);
            this.cb_window_pane.TabIndex = 6;
            this.cb_window_pane.Text = "Show window pane";
            this.cb_window_pane.UseVisualStyleBackColor = true;
            // 
            // cb_dateandtime
            // 
            this.cb_dateandtime.AutoSize = true;
            this.cb_dateandtime.Checked = global::FaceDetection.Properties.Settings.Default.show_current_datetime;
            this.cb_dateandtime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_dateandtime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_current_datetime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_dateandtime.Location = new System.Drawing.Point(259, 61);
            this.cb_dateandtime.Name = "cb_dateandtime";
            this.cb_dateandtime.Size = new System.Drawing.Size(165, 16);
            this.cb_dateandtime.TabIndex = 7;
            this.cb_dateandtime.Text = "Show current date and time";
            this.cb_dateandtime.UseVisualStyleBackColor = true;
            // 
            // cb_show_camera_number
            // 
            this.cb_show_camera_number.AutoSize = true;
            this.cb_show_camera_number.Checked = global::FaceDetection.Properties.Settings.Default.show_camera_no;
            this.cb_show_camera_number.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_show_camera_number.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_camera_no", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_show_camera_number.Location = new System.Drawing.Point(259, 83);
            this.cb_show_camera_number.Name = "cb_show_camera_number";
            this.cb_show_camera_number.Size = new System.Drawing.Size(133, 16);
            this.cb_show_camera_number.TabIndex = 8;
            this.cb_show_camera_number.Text = "Show camera number";
            this.cb_show_camera_number.UseVisualStyleBackColor = true;
            // 
            // cb_show_rec_icon
            // 
            this.cb_show_rec_icon.AutoSize = true;
            this.cb_show_rec_icon.Checked = global::FaceDetection.Properties.Settings.Default.show_recording_icon;
            this.cb_show_rec_icon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_show_rec_icon.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_recording_icon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_show_rec_icon.Location = new System.Drawing.Point(259, 103);
            this.cb_show_rec_icon.Name = "cb_show_rec_icon";
            this.cb_show_rec_icon.Size = new System.Drawing.Size(131, 16);
            this.cb_show_rec_icon.TabIndex = 9;
            this.cb_show_rec_icon.Text = "Show \'recording\' icon";
            this.cb_show_rec_icon.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 92);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "Frame Rate";
            // 
            // label9_fps
            // 
            this.label9_fps.AutoSize = true;
            this.label9_fps.Location = new System.Drawing.Point(110, 92);
            this.label9_fps.Name = "label9_fps";
            this.label9_fps.Size = new System.Drawing.Size(21, 12);
            this.label9_fps.TabIndex = 4;
            this.label9_fps.Text = "fps";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tb_y);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.tb_x);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(6, 18);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(104, 65);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Position";
            // 
            // tb_y
            // 
            this.tb_y.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "display_pos_y", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_y.Location = new System.Drawing.Point(63, 40);
            this.tb_y.Name = "tb_y";
            this.tb_y.Size = new System.Drawing.Size(35, 19);
            this.tb_y.TabIndex = 3;
            this.tb_y.Text = global::FaceDetection.Properties.Settings.Default.display_pos_y;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Y";
            // 
            // tb_x
            // 
            this.tb_x.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "display_pos_x", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_x.Location = new System.Drawing.Point(63, 11);
            this.tb_x.Name = "tb_x";
            this.tb_x.Size = new System.Drawing.Size(35, 19);
            this.tb_x.TabIndex = 1;
            this.tb_x.Text = global::FaceDetection.Properties.Settings.Default.display_pos_x;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "X";
            // 
            // tb_frame_rate
            // 
            this.tb_frame_rate.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "frame_rate_fps", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_frame_rate.Location = new System.Drawing.Point(69, 89);
            this.tb_frame_rate.Name = "tb_frame_rate";
            this.tb_frame_rate.Size = new System.Drawing.Size(35, 19);
            this.tb_frame_rate.TabIndex = 3;
            this.tb_frame_rate.Text = global::FaceDetection.Properties.Settings.Default.frame_rate_fps;
            this.tb_frame_rate.Leave += new System.EventHandler(this.frameRateChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tb_cam_view_height);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.tb_cam_view_width);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Location = new System.Drawing.Point(116, 18);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(104, 65);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Size";
            // 
            // tb_cam_view_height
            // 
            this.tb_cam_view_height.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "view_height", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_cam_view_height.Location = new System.Drawing.Point(63, 40);
            this.tb_cam_view_height.Name = "tb_cam_view_height";
            this.tb_cam_view_height.Size = new System.Drawing.Size(35, 19);
            this.tb_cam_view_height.TabIndex = 3;
            this.tb_cam_view_height.Text = global::FaceDetection.Properties.Settings.Default.view_height;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Height";
            // 
            // tb_cam_view_width
            // 
            this.tb_cam_view_width.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "view_width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_cam_view_width.Location = new System.Drawing.Point(63, 11);
            this.tb_cam_view_width.Name = "tb_cam_view_width";
            this.tb_cam_view_width.Size = new System.Drawing.Size(35, 19);
            this.tb_cam_view_width.TabIndex = 1;
            this.tb_cam_view_width.Text = global::FaceDetection.Properties.Settings.Default.view_width;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Width";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.tb_frame_rate);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label9_fps);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cb_show_rec_icon);
            this.groupBox3.Controls.Add(this.cb_show_camera_number);
            this.groupBox3.Controls.Add(this.cb_dateandtime);
            this.groupBox3.Controls.Add(this.cb_window_pane);
            this.groupBox3.Controls.Add(this.cb_always_on_top);
            this.groupBox3.Location = new System.Drawing.Point(14, 26);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(479, 127);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View Settings";
            // 
            // cb_backlight_on_recognition
            // 
            this.cb_backlight_on_recognition.AutoSize = true;
            this.cb_backlight_on_recognition.Checked = global::FaceDetection.Properties.Settings.Default.backlight_on_upon_face_rec;
            this.cb_backlight_on_recognition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_backlight_on_recognition.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "backlight_on_upon_face_rec", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_backlight_on_recognition.Location = new System.Drawing.Point(259, 43);
            this.cb_backlight_on_recognition.Name = "cb_backlight_on_recognition";
            this.cb_backlight_on_recognition.Size = new System.Drawing.Size(148, 16);
            this.cb_backlight_on_recognition.TabIndex = 5;
            this.cb_backlight_on_recognition.Text = "Backlight on recognition";
            this.cb_backlight_on_recognition.UseVisualStyleBackColor = true;
            // 
            // lb_manual_recording_time
            // 
            this.lb_manual_recording_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_manual_recording_time.AutoSize = true;
            this.lb_manual_recording_time.Location = new System.Drawing.Point(283, 22);
            this.lb_manual_recording_time.Name = "lb_manual_recording_time";
            this.lb_manual_recording_time.Size = new System.Drawing.Size(118, 12);
            this.lb_manual_recording_time.TabIndex = 2;
            this.lb_manual_recording_time.Text = "Manual recording time";
            // 
            // lb_seconds
            // 
            this.lb_seconds.AutoSize = true;
            this.lb_seconds.Location = new System.Drawing.Point(445, 21);
            this.lb_seconds.Name = "lb_seconds";
            this.lb_seconds.Size = new System.Drawing.Size(23, 12);
            this.lb_seconds.TabIndex = 4;
            this.lb_seconds.Text = "sec";
            // 
            // tb_manual_recording_time
            // 
            this.tb_manual_recording_time.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "manual_record_maxtime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_manual_recording_time.Location = new System.Drawing.Point(407, 18);
            this.tb_manual_recording_time.Name = "tb_manual_recording_time";
            this.tb_manual_recording_time.Size = new System.Drawing.Size(32, 19);
            this.tb_manual_recording_time.TabIndex = 3;
            this.tb_manual_recording_time.Text = global::FaceDetection.Properties.Settings.Default.manual_record_maxtime;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cb_face_recognition);
            this.groupBox7.Controls.Add(this.tb_recognition_interval);
            this.groupBox7.Controls.Add(this.lb_milliseconds1);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Location = new System.Drawing.Point(7, 18);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(226, 47);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            // 
            // cb_face_recognition
            // 
            this.cb_face_recognition.AutoSize = true;
            this.cb_face_recognition.Checked = global::FaceDetection.Properties.Settings.Default.enable_face_recognition;
            this.cb_face_recognition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_face_recognition.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "enable_face_recognition", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_face_recognition.Location = new System.Drawing.Point(8, -1);
            this.cb_face_recognition.Name = "cb_face_recognition";
            this.cb_face_recognition.Size = new System.Drawing.Size(151, 16);
            this.cb_face_recognition.TabIndex = 0;
            this.cb_face_recognition.Text = "Enable Face Recognition";
            this.cb_face_recognition.UseVisualStyleBackColor = true;
            // 
            // tb_recognition_interval
            // 
            this.tb_recognition_interval.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "face_rec_interval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_recognition_interval.Location = new System.Drawing.Point(123, 21);
            this.tb_recognition_interval.Name = "tb_recognition_interval";
            this.tb_recognition_interval.Size = new System.Drawing.Size(52, 19);
            this.tb_recognition_interval.TabIndex = 2;
            this.tb_recognition_interval.Text = global::FaceDetection.Properties.Settings.Default.face_rec_interval;
            this.tb_recognition_interval.Leave += new System.EventHandler(this.captureIntervalChanged);
            // 
            // lb_milliseconds1
            // 
            this.lb_milliseconds1.AutoSize = true;
            this.lb_milliseconds1.Location = new System.Drawing.Point(181, 24);
            this.lb_milliseconds1.Name = "lb_milliseconds1";
            this.lb_milliseconds1.Size = new System.Drawing.Size(20, 12);
            this.lb_milliseconds1.TabIndex = 3;
            this.lb_milliseconds1.Text = "ms";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "Recognition interval";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox9);
            this.groupBox6.Controls.Add(this.groupBox10);
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.tb_manual_recording_time);
            this.groupBox6.Controls.Add(this.lb_seconds);
            this.groupBox6.Controls.Add(this.lb_manual_recording_time);
            this.groupBox6.Controls.Add(this.cb_backlight_on_recognition);
            this.groupBox6.Location = new System.Drawing.Point(13, 159);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(479, 262);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Functionality Settings";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cb_record_upon_start);
            this.groupBox9.Controls.Add(this.tb_record_reinitiation_interval_seconds);
            this.groupBox9.Controls.Add(this.cb_operator_capture);
            this.groupBox9.Controls.Add(this.cb_recording_during_facerec);
            this.groupBox9.Controls.Add(this.label18_seconds);
            this.groupBox9.Controls.Add(this.label17);
            this.groupBox9.Controls.Add(this.cm_capture_mode);
            this.groupBox9.Controls.Add(this.tb_capture_seconds);
            this.groupBox9.Controls.Add(this.lb_seconds2);
            this.groupBox9.Controls.Add(this.lb_capture_seconds);
            this.groupBox9.Controls.Add(this.lb_capture_mode);
            this.groupBox9.Location = new System.Drawing.Point(7, 72);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(226, 184);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            // 
            // cb_record_upon_start
            // 
            this.cb_record_upon_start.AutoSize = true;
            this.cb_record_upon_start.Checked = global::FaceDetection.Properties.Settings.Default.recording_on_start;
            this.cb_record_upon_start.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_record_upon_start.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "recording_on_start", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_record_upon_start.Location = new System.Drawing.Point(8, 100);
            this.cb_record_upon_start.Name = "cb_record_upon_start";
            this.cb_record_upon_start.Size = new System.Drawing.Size(116, 16);
            this.cb_record_upon_start.TabIndex = 7;
            this.cb_record_upon_start.Text = "Record upon start";
            this.cb_record_upon_start.UseVisualStyleBackColor = true;
            // 
            // tb_record_reinitiation_interval_seconds
            // 
            this.tb_record_reinitiation_interval_seconds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "recording_length_seconds", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_record_reinitiation_interval_seconds.Location = new System.Drawing.Point(142, 120);
            this.tb_record_reinitiation_interval_seconds.Name = "tb_record_reinitiation_interval_seconds";
            this.tb_record_reinitiation_interval_seconds.Size = new System.Drawing.Size(52, 19);
            this.tb_record_reinitiation_interval_seconds.TabIndex = 9;
            this.tb_record_reinitiation_interval_seconds.Text = global::FaceDetection.Properties.Settings.Default.recording_length_seconds;
            // 
            // cb_operator_capture
            // 
            this.cb_operator_capture.AutoSize = true;
            this.cb_operator_capture.Checked = global::FaceDetection.Properties.Settings.Default.capture_operator;
            this.cb_operator_capture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_operator_capture.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "capture_operator", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_operator_capture.Location = new System.Drawing.Point(7, -1);
            this.cb_operator_capture.Name = "cb_operator_capture";
            this.cb_operator_capture.Size = new System.Drawing.Size(112, 16);
            this.cb_operator_capture.TabIndex = 0;
            this.cb_operator_capture.Text = "Operator Capture";
            this.cb_operator_capture.UseVisualStyleBackColor = true;
            // 
            // cb_recording_during_facerec
            // 
            this.cb_recording_during_facerec.AutoSize = true;
            this.cb_recording_during_facerec.Checked = global::FaceDetection.Properties.Settings.Default.keeprecording_while_facerecognition;
            this.cb_recording_during_facerec.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_recording_during_facerec.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "keeprecording_while_facerecognition", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_recording_during_facerec.Location = new System.Drawing.Point(8, 78);
            this.cb_recording_during_facerec.Name = "cb_recording_during_facerec";
            this.cb_recording_during_facerec.Size = new System.Drawing.Size(196, 16);
            this.cb_recording_during_facerec.TabIndex = 6;
            this.cb_recording_during_facerec.Text = "Recording during face recognition";
            this.cb_recording_during_facerec.UseVisualStyleBackColor = true;
            // 
            // label18_seconds
            // 
            this.label18_seconds.AutoSize = true;
            this.label18_seconds.Location = new System.Drawing.Point(200, 123);
            this.label18_seconds.Name = "label18_seconds";
            this.label18_seconds.Size = new System.Drawing.Size(20, 12);
            this.label18_seconds.TabIndex = 10;
            this.label18_seconds.Text = "ms";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(7, 123);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(129, 29);
            this.label17.TabIndex = 8;
            this.label17.Text = "Recording re-initiation interval";
            // 
            // cm_capture_mode
            // 
            this.cm_capture_mode.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "capture_type", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cm_capture_mode.FormattingEnabled = true;
            this.cm_capture_mode.Items.AddRange(new object[] {
            "Video",
            "Snapshot"});
            this.cm_capture_mode.Location = new System.Drawing.Point(123, 18);
            this.cm_capture_mode.Name = "cm_capture_mode";
            this.cm_capture_mode.Size = new System.Drawing.Size(80, 20);
            this.cm_capture_mode.TabIndex = 2;
            this.cm_capture_mode.Text = global::FaceDetection.Properties.Settings.Default.capture_type;
            // 
            // tb_capture_seconds
            // 
            this.tb_capture_seconds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "recording_length_seconds", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_capture_seconds.Location = new System.Drawing.Point(82, 47);
            this.tb_capture_seconds.Name = "tb_capture_seconds";
            this.tb_capture_seconds.Size = new System.Drawing.Size(52, 19);
            this.tb_capture_seconds.TabIndex = 4;
            this.tb_capture_seconds.Text = global::FaceDetection.Properties.Settings.Default.recording_length_seconds;
            // 
            // lb_seconds2
            // 
            this.lb_seconds2.AutoSize = true;
            this.lb_seconds2.Location = new System.Drawing.Point(140, 50);
            this.lb_seconds2.Name = "lb_seconds2";
            this.lb_seconds2.Size = new System.Drawing.Size(23, 12);
            this.lb_seconds2.TabIndex = 5;
            this.lb_seconds2.Text = "sec";
            // 
            // lb_capture_seconds
            // 
            this.lb_capture_seconds.AutoSize = true;
            this.lb_capture_seconds.Location = new System.Drawing.Point(6, 50);
            this.lb_capture_seconds.Name = "lb_capture_seconds";
            this.lb_capture_seconds.Size = new System.Drawing.Size(71, 12);
            this.lb_capture_seconds.TabIndex = 3;
            this.lb_capture_seconds.Text = "Capture time";
            // 
            // lb_capture_mode
            // 
            this.lb_capture_mode.AutoSize = true;
            this.lb_capture_mode.Location = new System.Drawing.Point(5, 21);
            this.lb_capture_mode.Name = "lb_capture_mode";
            this.lb_capture_mode.Size = new System.Drawing.Size(76, 12);
            this.lb_capture_mode.TabIndex = 1;
            this.lb_capture_mode.Text = "Capture mode";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.cb_event_recorder);
            this.groupBox10.Controls.Add(this.tb_post_event_seconds);
            this.groupBox10.Controls.Add(this.tb_pre_event_seconds);
            this.groupBox10.Controls.Add(this.lb_post_event);
            this.groupBox10.Controls.Add(this.lb_pre_event);
            this.groupBox10.Controls.Add(this.label23_seconds);
            this.groupBox10.Controls.Add(this.label22_seconds);
            this.groupBox10.Location = new System.Drawing.Point(239, 140);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(234, 86);
            this.groupBox10.TabIndex = 7;
            this.groupBox10.TabStop = false;
            // 
            // cb_event_recorder
            // 
            this.cb_event_recorder.AutoSize = true;
            this.cb_event_recorder.Checked = global::FaceDetection.Properties.Settings.Default.event_recorder_on;
            this.cb_event_recorder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_event_recorder.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "event_recorder_on", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_event_recorder.Location = new System.Drawing.Point(12, 0);
            this.cb_event_recorder.Name = "cb_event_recorder";
            this.cb_event_recorder.Size = new System.Drawing.Size(103, 16);
            this.cb_event_recorder.TabIndex = 0;
            this.cb_event_recorder.Text = "Event Recorder";
            this.cb_event_recorder.UseVisualStyleBackColor = true;
            // 
            // tb_post_event_seconds
            // 
            this.tb_post_event_seconds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "seconds_after_event", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_post_event_seconds.Location = new System.Drawing.Point(148, 52);
            this.tb_post_event_seconds.Name = "tb_post_event_seconds";
            this.tb_post_event_seconds.Size = new System.Drawing.Size(52, 19);
            this.tb_post_event_seconds.TabIndex = 5;
            this.tb_post_event_seconds.Text = global::FaceDetection.Properties.Settings.Default.seconds_after_event;
            // 
            // tb_pre_event_seconds
            // 
            this.tb_pre_event_seconds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "seconds_before_event", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_pre_event_seconds.Location = new System.Drawing.Point(148, 18);
            this.tb_pre_event_seconds.Name = "tb_pre_event_seconds";
            this.tb_pre_event_seconds.Size = new System.Drawing.Size(52, 19);
            this.tb_pre_event_seconds.TabIndex = 2;
            this.tb_pre_event_seconds.Text = global::FaceDetection.Properties.Settings.Default.seconds_before_event;
            // 
            // lb_post_event
            // 
            this.lb_post_event.AutoSize = true;
            this.lb_post_event.Location = new System.Drawing.Point(33, 55);
            this.lb_post_event.Name = "lb_post_event";
            this.lb_post_event.Size = new System.Drawing.Size(109, 12);
            this.lb_post_event.TabIndex = 4;
            this.lb_post_event.Text = "Record (after event)";
            // 
            // lb_pre_event
            // 
            this.lb_pre_event.AutoSize = true;
            this.lb_pre_event.Location = new System.Drawing.Point(23, 21);
            this.lb_pre_event.Name = "lb_pre_event";
            this.lb_pre_event.Size = new System.Drawing.Size(117, 12);
            this.lb_pre_event.TabIndex = 1;
            this.lb_pre_event.Text = "Record (before event)";
            // 
            // label23_seconds
            // 
            this.label23_seconds.AutoSize = true;
            this.label23_seconds.Location = new System.Drawing.Point(204, 55);
            this.label23_seconds.Name = "label23_seconds";
            this.label23_seconds.Size = new System.Drawing.Size(20, 12);
            this.label23_seconds.TabIndex = 6;
            this.label23_seconds.Text = "ms";
            // 
            // label22_seconds
            // 
            this.label22_seconds.AutoSize = true;
            this.label22_seconds.Location = new System.Drawing.Point(204, 21);
            this.label22_seconds.Name = "label22_seconds";
            this.label22_seconds.Size = new System.Drawing.Size(20, 12);
            this.label22_seconds.TabIndex = 3;
            this.label22_seconds.Text = "ms";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cb_backlight_off_idling);
            this.groupBox8.Controls.Add(this.tb_backlight_off_idling_delay_seconds);
            this.groupBox8.Controls.Add(this.lb_backlight_off_idling_delay);
            this.groupBox8.Controls.Add(this.label20_minutes);
            this.groupBox8.Location = new System.Drawing.Point(239, 71);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(234, 63);
            this.groupBox8.TabIndex = 6;
            this.groupBox8.TabStop = false;
            // 
            // cb_backlight_off_idling
            // 
            this.cb_backlight_off_idling.AutoSize = true;
            this.cb_backlight_off_idling.Checked = true;
            this.cb_backlight_off_idling.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_backlight_off_idling.Location = new System.Drawing.Point(21, 0);
            this.cb_backlight_off_idling.Name = "cb_backlight_off_idling";
            this.cb_backlight_off_idling.Size = new System.Drawing.Size(142, 16);
            this.cb_backlight_off_idling.TabIndex = 0;
            this.cb_backlight_off_idling.Text = "Backlight off when idle";
            this.cb_backlight_off_idling.UseVisualStyleBackColor = true;
            // 
            // tb_backlight_off_idling_delay_seconds
            // 
            this.tb_backlight_off_idling_delay_seconds.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "backlight_offset_mins", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_backlight_off_idling_delay_seconds.Location = new System.Drawing.Point(148, 22);
            this.tb_backlight_off_idling_delay_seconds.Name = "tb_backlight_off_idling_delay_seconds";
            this.tb_backlight_off_idling_delay_seconds.Size = new System.Drawing.Size(52, 19);
            this.tb_backlight_off_idling_delay_seconds.TabIndex = 2;
            this.tb_backlight_off_idling_delay_seconds.Text = global::FaceDetection.Properties.Settings.Default.backlight_offset_mins;
            // 
            // lb_backlight_off_idling_delay
            // 
            this.lb_backlight_off_idling_delay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_backlight_off_idling_delay.AutoSize = true;
            this.lb_backlight_off_idling_delay.Location = new System.Drawing.Point(80, 25);
            this.lb_backlight_off_idling_delay.Name = "lb_backlight_off_idling_delay";
            this.lb_backlight_off_idling_delay.Size = new System.Drawing.Size(62, 12);
            this.lb_backlight_off_idling_delay.TabIndex = 1;
            this.lb_backlight_off_idling_delay.Text = "After idling";
            // 
            // label20_minutes
            // 
            this.label20_minutes.AutoSize = true;
            this.label20_minutes.Location = new System.Drawing.Point(206, 25);
            this.label20_minutes.Name = "label20_minutes";
            this.label20_minutes.Size = new System.Drawing.Size(20, 12);
            this.label20_minutes.TabIndex = 3;
            this.label20_minutes.Text = "ms";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.button_settings_cancel);
            this.groupBox11.Controls.Add(this.button_settings_save);
            this.groupBox11.Controls.Add(this.groupBox3);
            this.groupBox11.Controls.Add(this.groupBox6);
            this.groupBox11.Controls.Add(this.cm_camera_number);
            this.groupBox11.Location = new System.Drawing.Point(5, 169);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(502, 459);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Camera Number";
            // 
            // button_settings_cancel
            // 
            this.button_settings_cancel.Location = new System.Drawing.Point(417, 427);
            this.button_settings_cancel.Name = "button_settings_cancel";
            this.button_settings_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_settings_cancel.TabIndex = 3;
            this.button_settings_cancel.Text = "Cancel";
            this.button_settings_cancel.UseVisualStyleBackColor = true;
            this.button_settings_cancel.Click += new System.EventHandler(this.closeSettings);
            // 
            // button_settings_save
            // 
            this.button_settings_save.Location = new System.Drawing.Point(336, 427);
            this.button_settings_save.Name = "button_settings_save";
            this.button_settings_save.Size = new System.Drawing.Size(75, 23);
            this.button_settings_save.TabIndex = 3;
            this.button_settings_save.Text = "OK";
            this.button_settings_save.UseVisualStyleBackColor = true;
            this.button_settings_save.Click += new System.EventHandler(this.save_and_close);
            // 
            // cm_camera_number
            // 
            this.cm_camera_number.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "current_camera_index", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cm_camera_number.FormattingEnabled = true;
            this.cm_camera_number.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cm_camera_number.Location = new System.Drawing.Point(105, 0);
            this.cm_camera_number.Name = "cm_camera_number";
            this.cm_camera_number.Size = new System.Drawing.Size(91, 20);
            this.cm_camera_number.TabIndex = 0;
            this.cm_camera_number.Text = global::FaceDetection.Properties.Settings.Default.current_camera_index;
            // 
            // settingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(516, 631);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::FaceDetection.Properties.Settings.Default, "Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::FaceDetection.Properties.Settings.Default.Location;
            this.Name = "settingsUI";
            this.Text = "Settings";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_all_cameras;
        private System.Windows.Forms.ComboBox cm_language;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_camera_count;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_days_old;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox cb_delete_old;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_always_on_top;
        private System.Windows.Forms.CheckBox cb_window_pane;
        private System.Windows.Forms.CheckBox cb_dateandtime;
        private System.Windows.Forms.CheckBox cb_show_camera_number;
        private System.Windows.Forms.CheckBox cb_show_rec_icon;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9_fps;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tb_y;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_x;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_frame_rate;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tb_cam_view_height;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_cam_view_width;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cb_backlight_on_recognition;
        private System.Windows.Forms.Label lb_manual_recording_time;
        private System.Windows.Forms.Label lb_seconds;
        private System.Windows.Forms.TextBox tb_manual_recording_time;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox cb_face_recognition;
        private System.Windows.Forms.TextBox tb_recognition_interval;
        private System.Windows.Forms.Label lb_milliseconds1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox cb_record_upon_start;
        private System.Windows.Forms.TextBox tb_record_reinitiation_interval_seconds;
        private System.Windows.Forms.CheckBox cb_recording_during_facerec;
        private System.Windows.Forms.Label label18_seconds;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cm_capture_mode;
        private System.Windows.Forms.TextBox tb_capture_seconds;
        private System.Windows.Forms.Label lb_seconds2;
        private System.Windows.Forms.Label lb_capture_seconds;
        private System.Windows.Forms.Label lb_capture_mode;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.CheckBox cb_event_recorder;
        private System.Windows.Forms.TextBox tb_post_event_seconds;
        private System.Windows.Forms.TextBox tb_pre_event_seconds;
        private System.Windows.Forms.Label lb_post_event;
        private System.Windows.Forms.Label lb_pre_event;
        private System.Windows.Forms.Label label23_seconds;
        private System.Windows.Forms.Label label22_seconds;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox cb_backlight_off_idling;
        private System.Windows.Forms.TextBox tb_backlight_off_idling_delay_seconds;
        private System.Windows.Forms.Label lb_backlight_off_idling_delay;
        private System.Windows.Forms.Label label20_minutes;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label label25_pcs;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button button_settings_cancel;
        private System.Windows.Forms.Button button_settings_save;
        private System.Windows.Forms.ComboBox cm_camera_number;
        private System.Windows.Forms.CheckBox cb_operator_capture;
    }
}