namespace FaceDetection
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.or_controlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.folderButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.settingsButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.cameraButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.or_dateTimeLabel = new System.Windows.Forms.Label();
            this.camera_number_txt = new System.Windows.Forms.Label();
            this.pbRecording = new System.Windows.Forms.PictureBox();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.or_controlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).BeginInit();
            this.SuspendLayout();
            // 
            // or_controlButtons
            // 
            this.or_controlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.or_controlButtons.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.or_controlButtons.Controls.Add(this.folderButton);
            this.or_controlButtons.Controls.Add(this.settingsButton);
            this.or_controlButtons.Controls.Add(this.snapshotButton);
            this.or_controlButtons.Controls.Add(this.cameraButton);
            this.or_controlButtons.Controls.Add(this.closeButton);
            this.or_controlButtons.Location = new System.Drawing.Point(320, 365);
            this.or_controlButtons.Name = "or_controlButtons";
            this.or_controlButtons.Size = new System.Drawing.Size(292, 64);
            this.or_controlButtons.TabIndex = 12;
            this.or_controlButtons.Visible = false;
            // 
            // folderButton
            // 
            this.folderButton.BackColor = System.Drawing.SystemColors.Control;
            this.folderButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.folderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.folderButton.ImageIndex = 16;
            this.folderButton.ImageList = this.imageList1;
            this.folderButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.folderButton.Location = new System.Drawing.Point(3, 3);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(52, 56);
            this.folderButton.TabIndex = 3;
            this.folderButton.UseVisualStyleBackColor = false;
            this.folderButton.Click += new System.EventHandler(this.OpenStoreLocation);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "camera-icon.png");
            this.imageList1.Images.SetKeyName(1, "Configuration-icon.png");
            this.imageList1.Images.SetKeyName(2, "video-camera-icon.png");
            this.imageList1.Images.SetKeyName(3, "Windows-icon.png");
            this.imageList1.Images.SetKeyName(4, "Actions-window-close-icon.png");
            this.imageList1.Images.SetKeyName(5, "Camera-Front-icon.png");
            this.imageList1.Images.SetKeyName(6, "delete-1-icon.png");
            this.imageList1.Images.SetKeyName(7, "Folder-icon.png");
            this.imageList1.Images.SetKeyName(8, "Settings-2-icon.png");
            this.imageList1.Images.SetKeyName(9, "Tools-icon.png");
            this.imageList1.Images.SetKeyName(10, "Video-Camera-2-icon.png");
            this.imageList1.Images.SetKeyName(11, "Pause-Normal-Red-icon.png");
            this.imageList1.Images.SetKeyName(12, "Record-Pressed-icon.png");
            this.imageList1.Images.SetKeyName(13, "189-1894906_photography-camera-outline-comments-camera-icon-outline-hd.png");
            this.imageList1.Images.SetKeyName(14, "405-4054031_png-file-svg-transparent-video-camera-icon-clipart.png");
            this.imageList1.Images.SetKeyName(15, "26894-200.png");
            this.imageList1.Images.SetKeyName(16, "Blank_Folder-512.png");
            this.imageList1.Images.SetKeyName(17, "settings__184833.png");
            this.imageList1.Images.SetKeyName(18, "584abf212912007028bd9334.png");
            this.imageList1.Images.SetKeyName(19, "5879208-video-camera-png-vector-psd-and-clipart-with-transparent-video-camera-log" +
        "o-png-360_360_preview.png");
            this.imageList1.Images.SetKeyName(20, "player_record.png");
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.SystemColors.Control;
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.ImageIndex = 17;
            this.settingsButton.ImageList = this.imageList1;
            this.settingsButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.settingsButton.Location = new System.Drawing.Point(61, 3);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(52, 56);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.ShowSettings);
            // 
            // snapshotButton
            // 
            this.snapshotButton.BackColor = System.Drawing.SystemColors.Control;
            this.snapshotButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.snapshotButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.snapshotButton.ImageIndex = 18;
            this.snapshotButton.ImageList = this.imageList1;
            this.snapshotButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.snapshotButton.Location = new System.Drawing.Point(119, 3);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(52, 56);
            this.snapshotButton.TabIndex = 3;
            this.snapshotButton.UseVisualStyleBackColor = false;
            this.snapshotButton.Click += new System.EventHandler(this.SnapShot);
            // 
            // cameraButton
            // 
            this.cameraButton.BackColor = System.Drawing.SystemColors.Control;
            this.cameraButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.cameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cameraButton.ImageIndex = 14;
            this.cameraButton.ImageList = this.imageList1;
            this.cameraButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cameraButton.Location = new System.Drawing.Point(177, 3);
            this.cameraButton.Name = "cameraButton";
            this.cameraButton.Padding = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.cameraButton.Size = new System.Drawing.Size(52, 56);
            this.cameraButton.TabIndex = 3;
            this.cameraButton.Tag = "play";
            this.cameraButton.UseVisualStyleBackColor = false;
            this.cameraButton.Click += new System.EventHandler(this.ToggleVideoRecording);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.Control;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ImageIndex = 15;
            this.closeButton.ImageList = this.imageList1;
            this.closeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.closeButton.Location = new System.Drawing.Point(235, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(52, 56);
            this.closeButton.TabIndex = 3;
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.ShowButtons);
            // 
            // or_dateTimeLabel
            // 
            this.or_dateTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.or_dateTimeLabel.AutoSize = true;
            this.or_dateTimeLabel.BackColor = System.Drawing.Color.Black;
            this.or_dateTimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F);
            this.or_dateTimeLabel.ForeColor = System.Drawing.Color.White;
            this.or_dateTimeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.or_dateTimeLabel.Location = new System.Drawing.Point(12, 398);
            this.or_dateTimeLabel.Name = "or_dateTimeLabel";
            this.or_dateTimeLabel.Padding = new System.Windows.Forms.Padding(3);
            this.or_dateTimeLabel.Size = new System.Drawing.Size(125, 30);
            this.or_dateTimeLabel.TabIndex = 13;
            this.or_dateTimeLabel.Text = "Date and time";
            this.or_dateTimeLabel.UseCompatibleTextRendering = true;
            // 
            // camera_number_txt
            // 
            this.camera_number_txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.camera_number_txt.AutoSize = true;
            this.camera_number_txt.BackColor = System.Drawing.SystemColors.Control;
            this.camera_number_txt.Font = new System.Drawing.Font("MS UI Gothic", 50F);
            this.camera_number_txt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.camera_number_txt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.camera_number_txt.Location = new System.Drawing.Point(549, 10);
            this.camera_number_txt.Name = "camera_number_txt";
            this.camera_number_txt.Size = new System.Drawing.Size(63, 67);
            this.camera_number_txt.TabIndex = 10;
            this.camera_number_txt.Text = "1";
            this.camera_number_txt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbRecording
            // 
            this.pbRecording.BackColor = System.Drawing.Color.Transparent;
            this.pbRecording.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbRecording.Image = global::FaceDetection.Properties.Resources.player_record;
            this.pbRecording.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbRecording.Location = new System.Drawing.Point(12, 13);
            this.pbRecording.Name = "pbRecording";
            this.pbRecording.Size = new System.Drawing.Size(52, 56);
            this.pbRecording.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRecording.TabIndex = 5;
            this.pbRecording.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.pbRecording);
            this.Controls.Add(this.or_dateTimeLabel);
            this.Controls.Add(this.or_controlButtons);
            this.Controls.Add(this.camera_number_txt);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::FaceDetection.Properties.Settings.Default, "cam1_position", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::FaceDetection.Properties.Settings.Default.cam1_position;
            this.Name = "MainForm";
            this.Text = "UVC Camera Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.WindowSizeUpdate);
            this.Click += new System.EventHandler(this.ShowButtons);
            this.DoubleClick += new System.EventHandler(this.FullScreen);
            this.or_controlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private System.Windows.Forms.PictureBox pbRecording;
        private System.Windows.Forms.FlowLayoutPanel or_controlButtons;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button snapshotButton;
        private System.Windows.Forms.Button cameraButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label or_dateTimeLabel;
        private System.Windows.Forms.Label camera_number_txt;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
    }
}