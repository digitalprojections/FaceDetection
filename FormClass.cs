using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceDetection
{
    public class FormClass : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label camera_number;
        private Label dateTimeLabel;
        private PictureBox rec_icon;
        private FlowLayoutPanel controlButtons;
        private Button folderButton;
        private Button settingsButton;
        private Button snapshotButton;
        private Button cameraButton;
        private Button closeButton;
        System.ComponentModel.ComponentResourceManager ressources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        private ImageList imageList;
        private System.Timers.Timer hideIconTimer = new System.Timers.Timer();
        private delegate void dHideRecIcon();
        private delegate void dDateTimerUpdater();

        public int CAMERA_INDEX = 0;
        public bool closeFromSettings = false;
        public CROSSBAR crossbar;
        public static CROSSBAR[] crossbarList = new CROSSBAR[3];
        static FormClass or_subform;
        public static FormClass GetSubForm => or_subform;

        public FormClass(int camind)
        {
            or_subform = this;

            this.Text = "UVC Camera Viewer - camera " + (camind + 1); //counting from the second camera            
            CAMERA_INDEX = camind;

            camera_number = new Label();
            camera_number.Size = new Size(65, 65);
            camera_number.UseWaitCursor = false;
            camera_number.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            camera_number.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, "show_camera_no", true, DataSourceUpdateMode.OnPropertyChanged));
            camera_number.Text = (CAMERA_INDEX + 1).ToString();
            camera_number.Font = new Font("MS UI Gothic", 50);
            camera_number.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(camera_number);
            camera_number.Location = new Point(this.Width - 89, 12);

            rec_icon = new PictureBox();
            rec_icon.Size = new Size(65, 65);
            rec_icon.Image = Properties.Resources.player_record;
            rec_icon.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(rec_icon);
            rec_icon.Location = new Point(12, 12);
            rec_icon.BackColor = Color.Transparent;
            rec_icon.Visible = false;
            hideIconTimer.AutoReset = false;
            hideIconTimer.Elapsed += new System.Timers.ElapsedEventHandler(HideIcon_tick);

            dateTimeLabel = new Label();
            dateTimeLabel.Name = "dateTimeLabel";
            dateTimeLabel.Size = new Size(125, 30);
            dateTimeLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            dateTimeLabel.DataBindings.Add(new Binding("Visible", Properties.Settings.Default, "show_current_datetime", true, DataSourceUpdateMode.OnPropertyChanged));
            dateTimeLabel.Text = "Date and time";
            dateTimeLabel.Font = new Font("MS UI Gothic", 14F);
            dateTimeLabel.AutoSize = true;
            dateTimeLabel.BackColor = Color.Black;
            dateTimeLabel.ForeColor = Color.White;
            dateTimeLabel.ImeMode = ImeMode.NoControl;
            dateTimeLabel.Padding = new Padding(3);
            dateTimeLabel.TabIndex = 13;
            dateTimeLabel.UseCompatibleTextRendering = true;
            this.Controls.Add(dateTimeLabel);
            dateTimeLabel.Location = new Point(12, this.Height - 80);

            SettingsButtonsDesigner();
            this.Controls.Add(this.controlButtons);

            this.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CAMERA_INDEX);
            this.Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);

            crossbar = new CROSSBAR(camind, this);
            crossbarList[camind-1] = crossbar;
            crossbar.PreviewMode();

            this.FormClosed += FormClass_FormClosed;
            this.ResizeEnd += FormClass_ResizeEnd;
            this.Click += FormClass_Click;
            this.DoubleClick += new EventHandler(this.FullScreen);
            this.SizeChanged += new EventHandler(this.WindowSizeUpdate);
        }

        public void SetToPreviewMode(int cam_index)
        {
            if (crossbarList[cam_index] != null)
            {
                crossbarList[cam_index].Start(cam_index, CAMERA_MODES.PREVIEW);
            }
        }

        public void SetToPreeventMode(int cam_index)
        {
            for (int i = 0; i < 3; i++)
            {
                if (crossbarList[i] != null)
                {
                    if (crossbarList[i].INDEX == cam_index)
                    {
                        crossbarList[i].Start(cam_index, CAMERA_MODES.PREEVENT);
                    }
                }
            }
        }

        public void StarttheTimer(int cam_index)
        {
            if (crossbarList[cam_index - 1] != null)
            {
                crossbarList[cam_index - 1].StartTimer();
            }
        }

        public bool PreeventRecordingState (int cam_index)
        {
            if (crossbarList[cam_index - 1] != null)
            {
                return crossbarList[cam_index - 1].PREEVENT_RECORDING;
            }
            else
            {
                return false;
            }
        }

        public void SetRecordIcon (int cam_index, int timeAfterEvent)
        {
            rec_icon.Visible = Properties.Settings.Default.show_recording_icon;
            crossbarList[cam_index - 1].No_Cap_Timer_ON(timeAfterEvent);
            crossbarList[cam_index - 1].icon_timer.Interval = decimal.ToInt32(timeAfterEvent) * 1000;
            crossbarList[cam_index - 1].icon_timer.Enabled = true;
            crossbarList[cam_index - 1].icon_timer.Start();
            hideIconTimer.Interval = timeAfterEvent * 1000;
            hideIconTimer.Start();
            MainForm.GetMainForm.recordingInProgress = true;
        }

        private void HideIcon_tick (object sender, EventArgs e) 
        {
            HideIcon();
        }

        public void HideIcon()
        {
            if (rec_icon.InvokeRequired)
            {
                var d = new dHideRecIcon(HideIcon);
                rec_icon.Invoke(d);
            }
            else
            {
                hideIconTimer.Enabled = false;
                rec_icon.Visible = false;
                MainForm.GetMainForm.recordingInProgress = false;
            }
        }

        private void FormClass_ResizeEnd(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Camera_Window_Size(CAMERA_INDEX, this);
            crossbar.SetWindowPosition(this.Size);
        }

        private void FormClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Window_Location(CAMERA_INDEX, this);
            Properties.Settings.Default.Save();
            crossbar.ReleaseSecondaryCamera();
            crossbarList[CAMERA_INDEX - 1] = null;

            if (closeFromSettings)
        {
                closeFromSettings = false;
        }
            else
        {
                Application.Exit();
            }

            if (Properties.Settings.Default.main_camera_index == CAMERA_INDEX) // The form closed was the main camera selected
            {
                Properties.Settings.Default.main_camera_index = 0;
            }
        }

        private void FullScreen(object sender, EventArgs eventArgs)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void WindowSizeUpdate(object sender, EventArgs eventArgs)
        {
            crossbar.SetWindowPosition(this.Size);
        }

        private void OpenStoreLocation(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.video_file_location);
                Process.Start(Properties.Settings.Default.video_file_location);
                this.TopMost = false;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message + " line OpenStoreLocation");
            }
        }

        private void FormClass_Click(object sender, EventArgs e)
        {
            if (this.controlButtons.Visible == false)
            {
                this.controlButtons.Visible = true;
            }
            else
            {
                this.controlButtons.Visible = false;
            }
        }

        private void SnapShot (object sender, EventArgs e)
        {
            string cameraSender;
            Button snd = (Button)sender;
            cameraSender = snd.TopLevelControl.ToString();
            cameraSender = cameraSender.Substring(cameraSender.Length - 1, 1);

            if (MainForm.Setting_ui.Camera_index == Convert.ToInt32(cameraSender) - 1)
            {
            SNAPSHOT_SAVER.TakeSnapShot(Convert.ToInt32(cameraSender) - 1);
        }
        }

        private void ManualVideoRecording(object sender, EventArgs e)
        {
            string cameraSender;
            Button snd = (Button)sender;
            cameraSender = snd.TopLevelControl.ToString();
            cameraSender = cameraSender.Substring(cameraSender.Length - 1, 1);

            rec_icon.Image = Properties.Resources.player_record;
            crossbar.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));

            bool eventRecorderEnabled = false, IRSensorEnabled = false, faceRecognitionEnabled = false, recordingWhenOperationEnabled = false;
            int eventRecordTimeBeforeEvent = 0, secondBeforeOperationEvent = 0;

            if (cameraSender == "2")
            {
                eventRecorderEnabled = Properties.Settings.Default.C2_enable_event_recorder;
                IRSensorEnabled = Properties.Settings.Default.C2_enable_Human_sensor;
                faceRecognitionEnabled = Properties.Settings.Default.C2_enable_face_recognition;
                recordingWhenOperationEnabled = Properties.Settings.Default.C2_Recording_when_at_the_start_of_operation;
                eventRecordTimeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C2_event_record_time_before_event);
                secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
            }
            else if (cameraSender == "3")
            {
                eventRecorderEnabled = Properties.Settings.Default.C3_enable_event_recorder;
                IRSensorEnabled = Properties.Settings.Default.C3_enable_Human_sensor;
                faceRecognitionEnabled = Properties.Settings.Default.C3_enable_face_recognition;
                recordingWhenOperationEnabled = Properties.Settings.Default.C3_Recording_when_at_the_start_of_operation;
                eventRecordTimeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C3_event_record_time_before_event);
                secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
            }
            else if (cameraSender == "4")
            {
                eventRecorderEnabled = Properties.Settings.Default.C4_enable_event_recorder;
                IRSensorEnabled = Properties.Settings.Default.C4_enable_Human_sensor;
                faceRecognitionEnabled = Properties.Settings.Default.C4_enable_face_recognition;
                recordingWhenOperationEnabled = Properties.Settings.Default.C4_Recording_when_at_the_start_of_operation;
                eventRecordTimeBeforeEvent = decimal.ToInt32(Properties.Settings.Default.C4_event_record_time_before_event);
                secondBeforeOperationEvent = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
            }

            if (MainForm.Setting_ui.Camera_index == Convert.ToInt32(cameraSender)-1)
            {
                try
                {
                    if ((String)cameraButton.Tag == "play")
                    {
                        if (MainForm.GetMainForm.recordingInProgress == false)
                        {
                            cameraButton.Tag = "rec";
                            crossbar.Start(Convert.ToInt32(cameraSender) - 1, CAMERA_MODES.MANUAL);
                            rec_icon.Visible = Properties.Settings.Default.show_recording_icon;
                            MainForm.GetMainForm.recordingInProgress = true;
                        }
                    }
                    else
                    {
                        rec_icon.Visible = false;
                        MainForm.GetMainForm.recordingInProgress = false;
                        cameraButton.Tag = "play";
                        if ((eventRecorderEnabled && eventRecordTimeBeforeEvent > 0)
                            || ((IRSensorEnabled || faceRecognitionEnabled || recordingWhenOperationEnabled) && secondBeforeOperationEvent > 0))
                        {
                            crossbar.Start(Convert.ToInt32(cameraSender) - 1, CAMERA_MODES.PREEVENT);
                        }
                        else
                        {
                            crossbar.Start(Convert.ToInt32(cameraSender) - 1, CAMERA_MODES.PREVIEW);
                        }
                    }
                }
                catch (InvalidOperationException iox)
                {
                    Logger.Add(iox);
                }
            }
        }

        public static void ParametersChangesApply(int cam_index)
        {
            if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0)
            {
                if (PARAMETERS.isControlButtonVisible)
                {
                    MULTI_WINDOW.formList[cam_index].controlButtons.Visible = true;
                }
                else
                {
                    MULTI_WINDOW.formList[cam_index].controlButtons.Visible = false;
                }

                PARAMETERS.PARAM.Clear();
            }
        }

        public void DateTimeUpdater()
        {
            if (dateTimeLabel != null && dateTimeLabel.InvokeRequired)
            {
                var d = new dDateTimerUpdater(DateTimeUpdater);
                dateTimeLabel.Invoke(d);
            }
            else
            {
                try
                {
                    dateTimeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine(e.Message + " DateTimeUpdater()");
                }
            }
        }

        private void SettingsButtonsDesigner()
        {
            this.components = new System.ComponentModel.Container();
            this.controlButtons = new FlowLayoutPanel();
            this.folderButton = new Button();
            this.imageList = new ImageList(this.components);
            this.settingsButton = new Button();
            this.snapshotButton = new Button();
            this.cameraButton = new Button();
            this.closeButton = new Button();
            // 
            // controlButtons
            // 
            this.controlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.controlButtons.BackColor = System.Drawing.Color.Transparent;
            this.controlButtons.Controls.Add(this.folderButton);
            this.controlButtons.Controls.Add(this.settingsButton);
            this.controlButtons.Controls.Add(this.snapshotButton);
            this.controlButtons.Controls.Add(this.cameraButton);
            this.controlButtons.Controls.Add(this.closeButton);
            this.controlButtons.Location = new Point(this.Width - 335, this.Height - 110);
            this.controlButtons.Name = "controlButtons";
            this.controlButtons.Size = new System.Drawing.Size(292, 59);
            this.controlButtons.TabIndex = 12;
            this.controlButtons.Visible = false;
            // 
            // folderButton
            // 
            this.folderButton.BackColor = System.Drawing.SystemColors.Control;
            this.folderButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.folderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.folderButton.ImageIndex = 16;
            this.folderButton.ImageList = this.imageList;
            this.folderButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.folderButton.Location = new System.Drawing.Point(3, 3);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(52, 52);
            this.folderButton.TabIndex = 3;
            this.folderButton.UseVisualStyleBackColor = false;
            this.folderButton.Click += new System.EventHandler(this.OpenStoreLocation);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(ressources.GetObject("imageList1.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "camera-icon.png");
            this.imageList.Images.SetKeyName(1, "Configuration-icon.png");
            this.imageList.Images.SetKeyName(2, "video-camera-icon.png");
            this.imageList.Images.SetKeyName(3, "Windows-icon.png");
            this.imageList.Images.SetKeyName(4, "Actions-window-close-icon.png");
            this.imageList.Images.SetKeyName(5, "Camera-Front-icon.png");
            this.imageList.Images.SetKeyName(6, "delete-1-icon.png");
            this.imageList.Images.SetKeyName(7, "Folder-icon.png");
            this.imageList.Images.SetKeyName(8, "Settings-2-icon.png");
            this.imageList.Images.SetKeyName(9, "Tools-icon.png");
            this.imageList.Images.SetKeyName(10, "Video-Camera-2-icon.png");
            this.imageList.Images.SetKeyName(11, "Pause-Normal-Red-icon.png");
            this.imageList.Images.SetKeyName(12, "Record-Pressed-icon.png");
            this.imageList.Images.SetKeyName(13, "189-1894906_photography-camera-outline-comments-camera-icon-outline-hd.png");
            this.imageList.Images.SetKeyName(14, "405-4054031_png-file-svg-transparent-video-camera-icon-clipart.png");
            this.imageList.Images.SetKeyName(15, "26894-200.png");
            this.imageList.Images.SetKeyName(16, "Blank_Folder-512.png");
            this.imageList.Images.SetKeyName(17, "settings__184833.png");
            this.imageList.Images.SetKeyName(18, "584abf212912007028bd9334.png");
            this.imageList.Images.SetKeyName(19, "5879208-video-camera-png-vector-psd-and-clipart-with-transparent-video-camera-log" + "o-png-360_360_preview.png");
            this.imageList.Images.SetKeyName(20, "player_record.png");
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.SystemColors.Control;
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.ImageIndex = 17;
            this.settingsButton.ImageList = this.imageList;
            this.settingsButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.settingsButton.Location = new System.Drawing.Point(61, 3);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(52, 52);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(MainForm.GetMainForm.ShowSettings);
            // 
            // snapshotButton
            // 
            this.snapshotButton.BackColor = System.Drawing.SystemColors.Control;
            this.snapshotButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.snapshotButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.snapshotButton.ImageIndex = 18;
            this.snapshotButton.ImageList = this.imageList;
            this.snapshotButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.snapshotButton.Location = new System.Drawing.Point(119, 3);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(52, 52);
            this.snapshotButton.TabIndex = 3;
            this.snapshotButton.UseVisualStyleBackColor = false;
            this.snapshotButton.Click += new System.EventHandler(SnapShot);
            // 
            // cameraButton
            // 
            this.cameraButton.BackColor = System.Drawing.SystemColors.Control;
            this.cameraButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.cameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cameraButton.ImageIndex = 14;
            this.cameraButton.ImageList = this.imageList;
            this.cameraButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cameraButton.Location = new System.Drawing.Point(177, 3);
            this.cameraButton.Name = "cameraButton";
            this.cameraButton.Padding = new System.Windows.Forms.Padding(10);
            this.cameraButton.Size = new System.Drawing.Size(52, 52);
            this.cameraButton.TabIndex = 3;
            this.cameraButton.Tag = "play";
            this.cameraButton.UseVisualStyleBackColor = false;
            this.cameraButton.Click += new System.EventHandler(ManualVideoRecording);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.Control;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ImageIndex = 15;
            this.closeButton.ImageList = this.imageList;
            this.closeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.closeButton.Location = new System.Drawing.Point(235, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(52, 52);
            this.closeButton.TabIndex = 3;
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(MainForm.GetMainForm.ShowButtons);
        }
    }
}
