﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceDetection
{
    public class CameraForm : Form
    {

        private delegate void dShowControlButtons();

        private readonly System.Timers.Timer mouse_down_timer = new System.Timers.Timer();

        public bool recordingInProgress;

        private PictureBox pb_recording;
        //private FlowLayoutPanel controlBut;
        //public FlowLayoutPanel gbox_controlBut { get => controlBut; set => controlBut = value; }
        public PictureBox picbox_recording { get => pb_recording; set => pb_recording = value; }

       
        /// <summary>
        /// FACEDETECTOR
        /// </summary>
        FaceDetector faceDetector;

        private System.ComponentModel.IContainer components = null;
        private CameraNumberLabel camera_number;
        private DateTimeLabel dateTimeLabel;
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

        
        internal FaceDetector FaceDetector { get => faceDetector; set => faceDetector = value; }

        public int CAMERA_INDEX = 0;
        public bool closeFromSettings = false;
        public CROSSBAR crossbar;
        public CROSSBAR[] crossbarList = new CROSSBAR[3];
        CameraForm or_subform;
        public CameraForm GetSubForm => or_subform;

        public CameraForm(int camind)
        {
            or_subform = this;            

            this.Text = "UVC Camera Viewer - camera " + (camind + 1); //counting from the second camera            
            CAMERA_INDEX = camind;
            camera_number = new CameraNumberLabel(camind);            
            this.Controls.Add(camera_number);
            

            rec_icon = new PictureBox();
            this.Controls.Add(rec_icon);

            hideIconTimer.AutoReset = false;
            hideIconTimer.Elapsed += new System.Timers.ElapsedEventHandler(HideIcon_tick);

            dateTimeLabel = new DateTimeLabel();
            this.Controls.Add(dateTimeLabel);
            

            SettingsButtonsDesigner();
            this.Controls.Add(this.controlButtons);

            this.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CAMERA_INDEX);
            this.Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);

            crossbar = new CROSSBAR(camind, this);
            //crossbarList[camind-1] = crossbar;
            crossbar.PreviewMode();

            
            this.MouseDown += HideButtons;
            this.Load += CameraForm_Load;
            this.FormClosed += FormClass_FormClosed;
            this.ResizeEnd += FormClass_ResizeEnd;
            this.Click += FormClass_Click;
            this.DoubleClick += new EventHandler(this.FullScreen);
            this.SizeChanged += new EventHandler(this.WindowSizeUpdate);
        }

        private void CameraForm_Load(object sender, EventArgs e)
        {
            ///SET THE MAIN WINDOW ICONS AND BUTTON POSITIONS MANUALLY
            controlButtons.Location = new Point(this.Width - 335, this.Height - 110);

            mouse_down_timer.Elapsed += ShowButtonsDelayed;//制御ボタンの非/表示用クリックタイマー
            mouse_down_timer.Interval = 1000;


            ///////////////////////////////////////////////////////////
        }
        public void ShowButtons(object sender, EventArgs eventArgs)
        {
            if (folderButton.Visible == false)
            {
                mouse_down_timer.Start();
            }
            else
            {
                controlButtons.Visible = false;
            }
        }

        private void ShowButtons(object sender, MouseEventArgs e)
        {
            if (folderButton.Visible == false)
            {
                mouse_down_timer.Start();
            }
            else
            {
                controlButtons.Visible = false;
            }
        }
        public void ParametersChangesApply()
        {
            if (PARAMETERS.PARAM != null && PARAMETERS.PARAM.Count > 0)
            {
                if (PARAMETERS.isControlButtonVisible)
                {
                    controlButtons.Visible = true;
                }
                else
                {
                    controlButtons.Visible = false;
                }

                PARAMETERS.PARAM.Clear();
            }
            if (Properties.Settings.Default.C1_enable_face_recognition || Properties.Settings.Default.C2_enable_face_recognition || Properties.Settings.Default.C3_enable_face_recognition || Properties.Settings.Default.C4_enable_face_recognition)
            {
                if (faceDetector != null)
                {
                    FaceDetector.StopFaceTimer();
                }
                faceDetector.SetInterval();
                faceDetector.StartFaceTimer();
            }
            else
            {
                if (faceDetector != null)
                {
                    FaceDetector.StopFaceTimer();
                }
            }
        }

        /// <summary>
        /// Get the bitmap from the 
        /// <see cref="crossbar/>
        /// </summary>
        /// <returns></returns>
        internal Bitmap GetSnapShot()
        {
            return crossbar?.GetBitmap();
        }



        internal void SetWindowProperties()
        {
            int cameraIndex = MainForm.Setting_ui.Camera_index;

            if (MainForm.Setting_ui == null)
            {
                picbox_recording.Visible = false;
                recordingInProgress = false;
                MainForm.Setting_ui = new SettingsUI();
            }

            camera_number.Visible = Properties.Settings.Default.show_camera_no;
            //or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index + 1).ToString();
            //MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);
            dateTimeLabel.Visible = Properties.Settings.Default.show_current_datetime;

            if (Properties.Settings.Default.main_window_full_screen)
            {
                if (cameraIndex == 0)
                {
                    MainForm.GetMainForm.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                    {
                        if (i == cameraIndex - 1)
                        {
                            MULTI_WINDOW.formList[i].WindowState = FormWindowState.Maximized;
                        }
                    }
                }
            }
            else
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Normal;
                for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                {
                    if (i == cameraIndex - 1)
                    {
                        MULTI_WINDOW.formList[i].WindowState = FormWindowState.Normal;
                    }
                }
            }

            if (cameraIndex != 0 && !Properties.Settings.Default.show_all_cams_simulteneously)
            {
                MainForm.GetMainForm.WindowState = FormWindowState.Minimized;
            }

            //Window pane
            if (Properties.Settings.Default.show_window_pane == true)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                ControlBox = true;

                for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                {
                    MULTI_WINDOW.formList[i].FormBorderStyle = FormBorderStyle.Sizable;
                }
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;

                for (int i = 0; i < MULTI_WINDOW.DisplayedCameraCount; i++)
                {
                    MULTI_WINDOW.formList[i].FormBorderStyle = FormBorderStyle.None;
                }
            }

            
            Size = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CAMERA_INDEX);
            Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CAMERA_INDEX);
        }

        public void SetRecordButtonState(string state)
        {
            cameraButton.Tag = state;
        }
        /// <summary>
        /// Manual Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToggleVideoRecording(object sender, EventArgs e)
        {
            picbox_recording.Image = Properties.Resources.player_record;
            crossbar?.No_Cap_Timer_ON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));

            try
            {
                if ((String)cameraButton.Tag == "play")
                {
                    if (recordingInProgress == false)
                    {
                        SetRecordButtonState("rec");
                        crossbar?.Start(0, CAMERA_MODES.MANUAL);
                        SET_REC_ICON();
                    }
                }
                else
                {
                    //it really depends if we shoul PREVIEW or PREEVENT
                    //set the deciding factors
                    //for now we can use this value as a test
                    //ONLY 0 index camera or the main camera is the one to be used to the manual recording?

                    picbox_recording.Visible = false;
                    recordingInProgress = false;
                    SetRecordButtonState("play");
                    SetCameraToDefaultMode();
                }
            }
            catch (InvalidOperationException iox)
            {
                Logger.Add(iox);
            }
        }
        public void SetToPreviewMode()
        {
            /*if (crossbarList[cam_index] != null)
            {
                crossbarList[cam_index].Start(cam_index, CAMERA_MODES.PREVIEW);
            }
            */
            crossbar.Start(CAMERA_INDEX, CAMERA_MODES.PREVIEW);
        }

        public void SetToPreeventMode()
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    if (crossbarList[i] != null)
            //    {
            //        if (crossbarList[i].INDEX == cam_index)
            //        {
            //            crossbarList[i].Start(cam_index, CAMERA_MODES.PREEVENT);
            //        }
            //    }
            //}
            if (crossbar.INDEX == CAMERA_INDEX)
                    {
                        crossbar.Start(CAMERA_INDEX, CAMERA_MODES.PREEVENT);
                    }
        }
        public void SET_REC_ICON()
        {
            picbox_recording.Visible = Properties.Settings.Default.show_recording_icon;
            recordingInProgress = true;
        }
        public void StarttheTimer()
        {
            //if (crossbarList[cam_index - 1] != null)
            //{
            //    crossbarList[cam_index - 1].StartTimer();
            //}
            crossbar.StartTimer();
        }

        

        public void SetRecordIcon (int cam_index, int timeAfterEvent)
        {
            rec_icon.Visible = Properties.Settings.Default.show_recording_icon;
            //crossbarList[cam_index - 1].No_Cap_Timer_ON(timeAfterEvent);
            //crossbarList[cam_index - 1].icon_timer.Interval = decimal.ToInt32(timeAfterEvent) * 1000;
            //crossbarList[cam_index - 1].icon_timer.Enabled = true;
            //crossbarList[cam_index - 1].icon_timer.Start();
            crossbar.No_Cap_Timer_ON(timeAfterEvent);
            crossbar.icon_timer.Interval = decimal.ToInt32(timeAfterEvent) * 1000;
            crossbar.icon_timer.Enabled = true;
            crossbar.icon_timer.Start();
            hideIconTimer.Interval = timeAfterEvent * 1000;
            hideIconTimer.Start();
            recordingInProgress = true;
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
                recordingInProgress = false;
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
            crossbar = null;

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
                        if (recordingInProgress == false)
                        {
                            cameraButton.Tag = "rec";
                            crossbar.Start(Convert.ToInt32(cameraSender) - 1, CAMERA_MODES.MANUAL);
                            rec_icon.Visible = Properties.Settings.Default.show_recording_icon;
                            recordingInProgress = true;
                        }
                    }
                    else
                    {
                        rec_icon.Visible = false;
                        recordingInProgress = false;
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

        public void ParametersChangesApply(int cam_index)
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
        /// <summary>
        /// One second timer to update UI datetime (it also deletes old files)
        /// </summary>
        private void UpdateDateTimeText(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            DateTimeUpdater();            
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
        public void StopAllTimers()
        {
            mouse_down_timer.Stop();
            
            if (FaceDetector != null)
                FaceDetector.Destroy();
            //if (RSensor != null)
            //    RSensor.Destroy();
            try
            {
                mouse_down_timer.Dispose();                
            }
            catch (Exception x)
            {
                Logger.Add(x);
            }
            crossbar?.ReleaseCameras();
        }

        private void HideButtons(object sender, MouseEventArgs e)
        {
            mouse_down_timer.Stop();
        }


        private void ShowButtonsDelayed(object o, EventArgs ElapsedEventArgs)
        {
            ShowButtonsDelayed();
        }
        private void ShowButtonsDelayed()
        {
            if (controlButtons.InvokeRequired)
            {
                var d = new dShowControlButtons(ShowButtonsDelayed);
                controlButtons.Invoke(d);
            }
            else
            {
                if (mouse_down_timer.Enabled == true)
                {
                    mouse_down_timer.Stop();
                    controlButtons.Visible = true;
                }
            }
        }
        public void SetCameraToDefaultMode()
        {
            if (MainForm.AtLeastOnePreEventTimeIsNotZero(CAMERA_INDEX))
            {
                if (CAMERA_INDEX == 0)
                {
                    MainForm.GetMainForm.crossbar?.Start(0, CAMERA_MODES.PREEVENT);
                }
                else
                {
                    if (MULTI_WINDOW.DisplayedCameraCount > 0)
                    {
                        SetToPreeventMode();
                    }
                }
            }
            else
            {
                if (CAMERA_INDEX == 0)
                {
                    MainForm.GetMainForm.crossbar?.Start(0, CAMERA_MODES.PREVIEW);
                }
                else
                {
                    if (MULTI_WINDOW.DisplayedCameraCount > 0)
                    {
                        SetToPreviewMode();
                    }
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
            this.closeButton.Click += new System.EventHandler(ShowButtons);
        }
    }
}