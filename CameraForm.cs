﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using FaceDetectionX;

namespace FaceDetection
{
    public class CameraForm : Form
    {
        private delegate void dShowControlButtons();
        private readonly System.Timers.Timer mouse_down_timer = new System.Timers.Timer();
        public bool recordingInProgress;
        public UsbCamera.VideoFormat[] videoFormat;
        /// <summary>
        /// SettingsUI resolutions data, generated each time. But set to the memory value
        /// </summary>
        private List<string> vf_resolutions = new List<string>();
        private List<string> vf_fps = new List<string>();

        private bool displayed = false;
        public bool DISPLAYED { get => displayed; set => displayed = value; }

        //private FlowLayoutPanel controlBut;
        //public FlowLayoutPanel gbox_controlBut { get => controlBut; set => controlBut = value; }
        public PictureBox picbox_recording { get => rec_icon; }
        public List<string> Vf_resolutions { get => vf_resolutions; set => vf_resolutions = value; }
        public List<string> Vf_fps { get => vf_fps; set => vf_fps = value; }

        private System.ComponentModel.IContainer components = null;
        private CameraNumberLabel camera_number;
        private DateTimeLabel dateTimeLabel;
        private RecIcon rec_icon;
        private FlowLayoutPanel controlButtons;
        private Button folderButton;
        private Button settingsButton;
        private Button snapshotButton;
        private Button cameraButton;
        private Button closeButton;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        private ImageList imageList;
        private System.Timers.Timer hideIconTimer = new System.Timers.Timer();
        private delegate void dHideRecIcon();
        private delegate void dDateTimerUpdater();
        private FormWindowState windowState = FormWindowState.Maximized;

        /// <summary>
        /// Current camera index, not MAIN
        /// </summary>
        public int CameraIndex = 0;
        public CROSSBAR crossbar;
        public bool applicationExit = false;
        public int[] secondsBeforeTriggerEvent = new int[4];
        public int[] secondsAfterTriggerEvent = new int[4];
        public int[] intervalAfterEvent = new int[4] { 10, 10, 10, 10 };

        public CameraForm(int camind)
        {
            InitializeComponent();
            CameraIndex = camind;
                        
            hideIconTimer.AutoReset = false;
            hideIconTimer.Elapsed += new System.Timers.ElapsedEventHandler(HideIcon_tick);
            videoFormat = UsbCamera.GetVideoFormat(camind);
            this.Load += CameraForm_Load;
        }

        private void CameraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.main_camera_index == CameraIndex && applicationExit == false) // The form closed was the main camera selected
            {
                this.Activate();
                DialogResult dr = MessageBox.Show(Resource.main_window_close_warning, Resource.ask_exit_application, MessageBoxButtons.OKCancel);
                switch (dr)
                {
                    case DialogResult.OK:
                        applicationExit = true;
                        Application.Exit();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Call when closing the window
        /// </summary>
        public void Destroy()
        {            
            this.MouseDown -= HideButtons;
            this.Load -= CameraForm_Load;
            this.FormClosing -= CameraForm_FormClosing;
            this.FormClosed -= FormClass_FormClosed;
            this.ResizeEnd -= FormClass_ResizeEnd;
            this.MouseDown -= FormClass_Down;
            this.MouseUp -= CameraForm_MouseUp;
            this.DoubleClick -= FullScreen;
            this.SizeChanged -= WindowSizeUpdate;

            mouse_down_timer.Stop();
            mouse_down_timer.Dispose();
        }

        /// <summary>
        /// LOAD elements in precise order. changing the order will cause issues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraForm_Load(object sender, EventArgs e)
        {
            this.MouseDown += HideButtons;
            this.FormClosing += CameraForm_FormClosing;
            this.FormClosed += FormClass_FormClosed;
            this.ResizeEnd += FormClass_ResizeEnd;
            this.MouseDown += FormClass_Down;
            this.MouseUp += CameraForm_MouseUp;
            this.DoubleClick += FullScreen;
            this.SizeChanged += FormClass_SizeChanged;

            crossbar = new CROSSBAR(CameraIndex, this);

            //var mci = Properties.Settings.Default.main_camera_index;
            //if (CameraIndex == mci)// && PROPERTY_FUNCTIONS.CheckPreEventTimes(CameraIndex))
            //{
            //    this.Text = $"UVC Camera Viewer - MAIN CAMERA {(CameraIndex + 1)}";                
            //}
            //else
            //{
            //    this.Text = "UVC Camera Viewer -  camera " + (CameraIndex + 1);                
            //}
            //crossbar.Start(CameraIndex, CAMERA_MODES.PREVIEW);

            this.SizeChanged += WindowSizeUpdate;

            SettingsButtonsDesigner();

            this.Controls.Add(this.controlButtons);

            rec_icon = new RecIcon();
            this.Controls.Add(rec_icon);
            camera_number = new CameraNumberLabel(CameraIndex);
            camera_number.Location = new Point(this.Width - 89, 12);
            this.Controls.Add(camera_number);
            dateTimeLabel = new DateTimeLabel(CameraIndex);
            dateTimeLabel.Location = new Point(12, this.Height - 80);
            this.Controls.Add(dateTimeLabel);

            switch (CameraIndex)
            {
                case 0:
                    this.secondsBeforeTriggerEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_seconds_before_event);
                    this.secondsAfterTriggerEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_seconds_after_event);
                    this.intervalAfterEvent[0] = decimal.ToInt32(Properties.Settings.Default.C1_interval_before_reinitiating_recording);
                    break;
                case 1:
                    this.secondsBeforeTriggerEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_seconds_before_event);
                    this.secondsAfterTriggerEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_seconds_after_event);
                    this.intervalAfterEvent[1] = decimal.ToInt32(Properties.Settings.Default.C2_interval_before_reinitiating_recording);
                    break;
                case 2:
                    this.secondsBeforeTriggerEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_seconds_before_event);
                    this.secondsAfterTriggerEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_seconds_after_event);
                    this.intervalAfterEvent[2] = decimal.ToInt32(Properties.Settings.Default.C3_interval_before_reinitiating_recording);
                    break;
                case 3:
                    this.secondsBeforeTriggerEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_seconds_before_event);
                    this.secondsAfterTriggerEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_seconds_after_event);
                    this.intervalAfterEvent[3] = decimal.ToInt32(Properties.Settings.Default.C4_interval_before_reinitiating_recording);
                    break;
            }

            ///SET THE MAIN WINDOW ICONS AND BUTTON POSITIONS MANUALLY
            controlButtons.Location = new Point(this.Width - 335, this.Height - 110);
            
            mouse_down_timer.Elapsed += ShowButtonsDelayed;//制御ボタンの非/表示用クリックタイマー
            mouse_down_timer.Interval = 1000;


            //// Full screen
            //if (Properties.Settings.Default.main_window_full_screen)
            //    WindowState = FormWindowState.Maximized;
            //else
            //    WindowState = FormWindowState.Normal;
            //SetWindowProperties();
            FillResolutionList();
            DISPLAYED = true;

            if (CameraIndex == Properties.Settings.Default.main_camera_index)
            {
                camera_number.ForeColor = Color.Red;
            }

            this.ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CameraIndex);
            this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);
        }

        /// <summary>
        /// Loop through the camera properties to select all available resolutions and FPS
        /// </summary>
        public void FillResolutionList()
        {
            long FPS;
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////
            //Showing video formats
            
            for (int k = 0; k < videoFormat.Length; k++)
            {
                if (UniqueVideoParameter(Vf_resolutions, videoFormat[k].Size) != true)
                {
                    Vf_resolutions.Add(videoFormat[k].Size.Width + "x" + videoFormat[k].Size.Height);
                }
                FPS = 10000000 / videoFormat[k].TimePerFrame;
                if (UniqueFPS(FPS) != true)
                {
                    vf_fps.Add(FPS.ToString());
                }
            }
            
            //Logger.Add("UniqueVideoParameter " + vf_resolutions.Count);
            //////////////////////////////////////////////////////////////////
            ///VIDEOFORMAT
            //////////////////////////////////////////////////////////////////            
            SortVideoFormats();
        }

        public void SortVideoFormats()
        {
            if (Vf_resolutions.Count>1)
            {
                List<int[]> resolutions = GetIntegers();
                int[][] vsres = new int[Vf_resolutions.Count][];

                for (int i = 0; i < resolutions.Count; i++)
                {
                    vsres[i] = resolutions[i];
                }
                Array.Sort(vsres, (a, b) => CheckValues(a, b)); ;
                for (int j = 0; j < resolutions.Count; j++)
                {
                    Vf_resolutions[j] = vsres[j][0] + "x" + vsres[j][1];
                }
                //Console.WriteLine(resolutions[0][0].ToString());
            }
        }

        private int CheckValues(int[] a, int[] b)
        {
            if (a[1] >= b[1] && a[0] >= b[0])
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private List<int[]> GetIntegers()
        {
            List<int[]> resolutions = new List<int[]>();
            string[] vs = new string[2];
            int[][] vsInt = new int[2][];
            for (int i = 0; i < Vf_resolutions.Count; i++)
            {
                vs = Vf_resolutions[i].Split('x');
                
                resolutions.Add(new int[2] { Int32.Parse(vs[0]), Int32.Parse(vs[1]) });
            }
            return resolutions;            
        }

        private bool UniqueFPS(long fps)
        {
            bool retval = false;
            for (int i = 0; i < vf_fps.Count; i++)
            {
                if (UInt32.Parse(vf_fps[i]) == fps)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private bool UniqueVideoParameter(List<string> vf, Size s)
        {
            bool retval = false;
            string temp = s.Width + "x" + s.Height;
            
            for (int i = 0; i < vf.Count; i++)
            {
                if (vf[i] == temp)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public void GetVideoFormat()
        {
            SettingsUI.SetComboBoxResolutionValues(Vf_resolutions, CameraIndex);
        }

        private void CameraForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down_timer.Stop();
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

        /// <summary>
        /// Get the bitmap from the 
        /// <see cref="crossbar"/>
        /// </summary>
        /// <returns></returns>
        internal Bitmap GetSnapShot()
        {
            return crossbar?.GetBitmap();
        }

        internal void SetWindowProperties()
        {            
            //int cameraIndex = Properties.Settings.Default.main_camera_index;
            //picbox_recording.Visible = false;
            //recordingInProgress = false;
            camera_number.Visible = PROPERTY_FUNCTIONS.GetShowCameraNumberSwitch(CameraIndex);
            //or_camera_num_txt.Text = (Properties.Settings.Default.main_camera_index + 1).ToString();
            //MainForm.GetMainForm.TopMost = Properties.Settings.Default.window_on_top;
            //Location = PROPERTY_FUNCTIONS.Get_Camera_Window_Location(CameraIndex);
            dateTimeLabel.Visible = PROPERTY_FUNCTIONS.GetShowDateTimeSwitch(CameraIndex);
            

            if (CameraIndex == Properties.Settings.Default.main_camera_index)
            {
                Text = $"UVC Camera Viewer - MAIN CAMERA {(CameraIndex + 1)}";
            }
            else
            {
                Text = "UVC Camera Viewer -  camera " + (CameraIndex + 1);
            }           

            //Window pane
            if (PROPERTY_FUNCTIONS.GetShowWindowPaneSwitch(CameraIndex) == true)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                ControlBox = true;                
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                FormBorderStyle = FormBorderStyle.None;
                // Full screen
                if (PROPERTY_FUNCTIONS.CheckFullScreenByIndex(CameraIndex))
                {
                    this.WindowState = FormWindowState.Normal;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);
                }
            }

            ClientSize = PROPERTY_FUNCTIONS.Get_Camera_Window_Size(CameraIndex);
            Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);

            // Minimized sub if "all display" is not checked
            if ((!Properties.Settings.Default.show_all_cams_simulteneously && CameraIndex != Properties.Settings.Default.main_camera_index) || PARAMETERS.minimizedByParameters)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                // Full screen
                if (PROPERTY_FUNCTIONS.CheckFullScreenByIndex(CameraIndex))
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    
                    PROPERTY_FUNCTIONS.Set_Window_Location_Set(CameraIndex, this);
                    this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);
                }
            }

            // Bring main to the front
            if (CameraIndex == Properties.Settings.Default.main_camera_index)
            {
                this.Activate();
            }
            else if(PROPERTY_FUNCTIONS.CheckOnTopByIndex(CameraIndex) && this.WindowState != FormWindowState.Minimized) 
            // Bring sub to the front if "on top" is selected
            {
                if (PROPERTY_FUNCTIONS.CheckFullScreenByIndex(CameraIndex))
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);
                }
                this.Activate();
            }

            crossbar.RESET_INTERVAL();

            SetCameraToDefaultMode();
        }

        public void SetRecordButtonState(string state)
        {
            cameraButton.Tag = state;
        }
        
        //public void SetToPreviewMode()
        //{
        //    crossbar.Start(CameraIndex, CAMERA_MODES.PREVIEW);
        //}

        //public void SetToPreeventMode()
        //{
        //    if (crossbar.INDEX == CameraIndex)
        //    {
        //        crossbar.Start(CameraIndex, CAMERA_MODES.PREEVENT);
        //    }
        //}

        public void SET_REC_ICON()
        {
            picbox_recording.Visible = PROPERTY_FUNCTIONS.GetRecordingIconSwitch(CameraIndex);
            recordingInProgress = true;
        }

        public void SetRecordIcon (int cam_index, int timeAfterEvent)
        {
            rec_icon.Visible = PROPERTY_FUNCTIONS.GetRecordingIconSwitch(cam_index);
            crossbar.NoCapTimerON(timeAfterEvent);
            crossbar.icon_timer.Interval = decimal.ToInt32(timeAfterEvent) * 1000;
            crossbar.icon_timer.Enabled = true;
            crossbar.icon_timer.Start();
            hideIconTimer.Interval = timeAfterEvent * 1000;
            hideIconTimer.Start();
            recordingInProgress = true;
            this.cameraButton.Enabled = false;
            this.snapshotButton.Enabled = false;
            MainForm.Settingui.SetOKButtonState();
        }

        public void SetHideRecordIcon(int timeAfterEvent)
        {
            hideIconTimer.Interval = timeAfterEvent * 1000;
            hideIconTimer.Start();
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
                this.cameraButton.Enabled = true;
                SnapshotEnable();
            }
            MainForm.Settingui.SetOKButtonState();
        }

        private void FormClass_ResizeEnd(object sender, EventArgs e)
        {
            PROPERTY_FUNCTIONS.Set_Camera_Window_Size(CameraIndex, this);
            crossbar.SetWindowPosition(this.Size);
        }

        private void FormClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            crossbar.ReleaseCamera();
            crossbar = null;

            //if (Properties.Settings.Default.main_camera_index == CameraIndex) // The form closed was the main camera selected
            //{
            //    for(int i=0; i < 4; i++)
            //    {
            //        if(MULTI_WINDOW.formList[i].DISPLAYED == true)
            //        {
            //            Properties.Settings.Default.main_camera_index = i;
            //            break;
            //        }
            //    }
            //    //Properties.Settings.Default.main_camera_index = 0;
            //}

            PROPERTY_FUNCTIONS.Set_Window_Location(CameraIndex, this);
            Properties.Settings.Default.Save();
            DISPLAYED = false;
            Destroy();
            MULTI_WINDOW.displayedCameraCount--;
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
                this.Location = PROPERTY_FUNCTIONS.Get_Window_Location(CameraIndex);
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
                //this.TopMost = false;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(ioe.Message + " line OpenStoreLocation");
            }
        }

        private void FormClass_Down(object sender, EventArgs e)
        {
            mouse_down_timer.Start();
        }

        private void FormClass_SizeChanged(object sender, EventArgs e)
        {
            if (windowState != this.WindowState && PROPERTY_FUNCTIONS.GetShowWindowPaneSwitch(CameraIndex))
            {
                camera_number.Location = new Point(this.Width - 89, 12);
                controlButtons.Location = new Point(this.Width - 335, this.Height - 110);
                dateTimeLabel.Location = new Point(12, this.Height - 80);
                windowState = this.WindowState;
            }
            else if (windowState != this.WindowState && !PROPERTY_FUNCTIONS.GetShowWindowPaneSwitch(CameraIndex))
            {
                camera_number.Location = new Point(this.Width - 89, 12);
                controlButtons.Location = new Point(this.Width - 335, this.Height - 80);
                dateTimeLabel.Location = new Point(12, this.Height - 50);
                windowState = this.WindowState;
            }
        }

        private void SnapShot (object sender, EventArgs e)
        {
            string cameraSender;
            Button snd = (Button)sender;
            cameraSender = snd.TopLevelControl.ToString();
            cameraSender = cameraSender.Substring(cameraSender.Length - 1, 1);

            SNAPSHOT_SAVER.TakeSnapShot(Convert.ToInt32(cameraSender) - 1, "snapshot");
        }

        private void ManualVideoRecording(object sender, EventArgs e)
        {
            ManualVideoRecording();
        }

        public void ManualVideoRecording ()
        {
            //string cameraSender;
            //Button snd = (Button)sender;
            //cameraSender = snd.TopLevelControl.ToString();
            //cameraSender = cameraSender.Substring(cameraSender.Length - 1, 1);
            //int camsen = int.Parse(cameraSender) - 1;
            PROPERTY_FUNCTIONS.GetEventRecorderSwitch(CameraIndex, out bool eventRecorderEnabled);
            PROPERTY_FUNCTIONS.Get_Human_Sensor_Enabled(CameraIndex, out bool IRSensorEnabled);
            PROPERTY_FUNCTIONS.GetFaceRecognitionSwitch(CameraIndex, out bool faceRecognitionEnabled);
            PROPERTY_FUNCTIONS.GetOnOperationStartSwitch(CameraIndex, out bool recordingWhenOperationEnabled);
            PROPERTY_FUNCTIONS.GetPreAndPostEventTimes(CameraIndex, out int eventRecordTimeBeforeEvent, out int nouse);
            PROPERTY_FUNCTIONS.GetSecondsBeforeEvent(CameraIndex, out int secondBeforeOperationEvent);

            try
            {
                if ((string)cameraButton.Tag == "play")
                {
                    if (recordingInProgress == false)
                    {
                        cameraButton.Tag = "rec";
                        crossbar.Start(CameraIndex, CAMERA_MODES.MANUAL);
                        rec_icon.Visible = PROPERTY_FUNCTIONS.GetRecordingIconSwitch(CameraIndex);
                        recordingInProgress = true;
                        crossbar.NoCapTimerON(decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                        crossbar.SetIconTimer(decimal.ToInt32(Properties.Settings.Default.manual_record_time));
                        snapshotButton.Enabled = false;
                    }
                }
                else
                {
                    rec_icon.Visible = false;
                    recordingInProgress = false;
                    cameraButton.Tag = "play";
                    SnapshotEnable();
                    if ((eventRecorderEnabled && eventRecordTimeBeforeEvent > 0)
                            || ((IRSensorEnabled || faceRecognitionEnabled || recordingWhenOperationEnabled) && secondBeforeOperationEvent > 0))
                    {
                        crossbar.Start(CameraIndex, CAMERA_MODES.PREEVENT);
                    }
                    else
                    {
                        crossbar.Start(CameraIndex, CAMERA_MODES.PREVIEW);
                    }
                }
            }
            catch (InvalidOperationException iox)
            {
                LOGGER.Add(iox);
            }
        }

        /// <summary>
        /// Apply setting changes immediately
        /// </summary>
        /// <param name="cam_index"></param>
        public void SettingChangesApply(int cam_index)
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
                    if (controlButtons.Visible)
                    {
                        controlButtons.Visible = false;
                    }
                    else
                    {
                        //camera_number.Location = new Point(this.Width - 89, 12);
                        //controlButtons.Location = new Point(this.Width - 335, this.Height - 110);
                        controlButtons.Visible = true;
                    }
                }
            }
        }

        public void SetCameraToDefaultMode()
        {
            if (CameraIndex == Properties.Settings.Default.main_camera_index && PROPERTY_FUNCTIONS.CheckPreEventTimes(CameraIndex))
            {                
                crossbar?.Start(CameraIndex, CAMERA_MODES.PREEVENT);
            }
            else
            {
                //if (MULTI_WINDOW.displayedCameraCount > 0)
                //{
                crossbar?.Start(CameraIndex, CAMERA_MODES.PREVIEW);
                    //SetToPreviewMode();
                //}
            }
        }

        public void DateTimeUpdater()
        {
            try
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
            catch (ObjectDisposedException ODEx)
            {
                Debug.WriteLine(ODEx.Message + " DateTimeUpdater()");
            }
        }

        public void MainCameraDisplay (int cameraIndex)
        {
            if (cameraIndex == Properties.Settings.Default.main_camera_index)
            {
                camera_number.ForeColor = Color.Red;
            }
            else
            {
                camera_number.ForeColor = Color.Black;
            }
        }

        public void SnapshotEnable()
        {
            snapshotButton.Enabled = true;
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
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
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
            //this.imageList.Images.SetKeyName(21, "Capture.png");
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
            this.settingsButton.Tag = CameraIndex.ToString();
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
            //this.cameraButton.ImageIndex = 14;
            this.cameraButton.Image = Properties.Resources.video_camera_icon;
            //this.cameraButton.ImageList = this.imageList;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CameraForm
            //             
            this.MinimumSize = new System.Drawing.Size(345, 150);
            this.Name = "CameraForm";
            this.ResumeLayout(false);
        }
    }
}
