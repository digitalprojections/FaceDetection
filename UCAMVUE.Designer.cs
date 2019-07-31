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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dateTimeLabel = new System.Windows.Forms.Label();
            this.testing_params = new System.Windows.Forms.Label();
            this.controlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.folderButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.cameraButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.camera_number = new System.Windows.Forms.Label();
            this.pbRecording = new System.Windows.Forms.PictureBox();
            this.imgCamUser = new Emgu.CV.UI.ImageBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.controlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            // 
            // dateTimeLabel
            // 
            resources.ApplyResources(this.dateTimeLabel, "dateTimeLabel");
            this.dateTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.dateTimeLabel.Name = "dateTimeLabel";
            this.dateTimeLabel.UseCompatibleTextRendering = true;
            // 
            // testing_params
            // 
            resources.ApplyResources(this.testing_params, "testing_params");
            this.testing_params.Name = "testing_params";
            // 
            // controlButtons
            // 
            resources.ApplyResources(this.controlButtons, "controlButtons");
            this.controlButtons.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.controlButtons.Controls.Add(this.folderButton);
            this.controlButtons.Controls.Add(this.settingsButton);
            this.controlButtons.Controls.Add(this.snapshotButton);
            this.controlButtons.Controls.Add(this.cameraButton);
            this.controlButtons.Controls.Add(this.closeButton);
            this.controlButtons.Name = "controlButtons";
            // 
            // folderButton
            // 
            this.folderButton.BackColor = System.Drawing.SystemColors.Control;
            this.folderButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.folderButton, "folderButton");
            this.folderButton.ImageList = this.imageList1;
            this.folderButton.Name = "folderButton";
            this.folderButton.UseVisualStyleBackColor = false;
            this.folderButton.Click += new System.EventHandler(this.OpenStoreLocation);
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.SystemColors.Control;
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.settingsButton, "settingsButton");
            this.settingsButton.ImageList = this.imageList1;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.ShowSettings);
            // 
            // snapshotButton
            // 
            this.snapshotButton.BackColor = System.Drawing.SystemColors.Control;
            this.snapshotButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.snapshotButton, "snapshotButton");
            this.snapshotButton.ImageList = this.imageList1;
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.UseVisualStyleBackColor = false;
            this.snapshotButton.Click += new System.EventHandler(this.SnapShot);
            // 
            // cameraButton
            // 
            this.cameraButton.BackColor = System.Drawing.SystemColors.Control;
            this.cameraButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.cameraButton, "cameraButton");
            this.cameraButton.ImageList = this.imageList1;
            this.cameraButton.Name = "cameraButton";
            this.cameraButton.UseVisualStyleBackColor = false;
            this.cameraButton.Click += new System.EventHandler(this.CameraButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.Control;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.ImageList = this.imageList1;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.ShowButtons);
            // 
            // camera_number
            // 
            resources.ApplyResources(this.camera_number, "camera_number");
            this.camera_number.BackColor = System.Drawing.Color.Transparent;
            this.camera_number.Name = "camera_number";
            // 
            // pbRecording
            // 
            this.pbRecording.BackColor = System.Drawing.Color.Transparent;
            this.pbRecording.Image = global::FaceDetection.Properties.Resources.Record_Pressed_icon1;
            resources.ApplyResources(this.pbRecording, "pbRecording");
            this.pbRecording.Name = "pbRecording";
            this.pbRecording.TabStop = false;
            // 
            // imgCamUser
            // 
            this.imgCamUser.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            resources.ApplyResources(this.imgCamUser, "imgCamUser");
            this.imgCamUser.Name = "imgCamUser";
            this.imgCamUser.TabStop = false;
            this.imgCamUser.DoubleClick += new System.EventHandler(this.FullScreen);
            this.imgCamUser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HoldButton);
            this.imgCamUser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ReleaseButton);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.camera_number);
            this.Controls.Add(this.controlButtons);
            this.Controls.Add(this.testing_params);
            this.Controls.Add(this.dateTimeLabel);
            this.Controls.Add(this.pbRecording);
            this.Controls.Add(this.imgCamUser);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::FaceDetection.Properties.Settings.Default, "Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::FaceDetection.Properties.Settings.Default.Location;
            this.Name = "MainForm";
            this.LocationChanged += new System.EventHandler(this.LastPositionUpdate);
            this.SizeChanged += new System.EventHandler(this.WindowSizeUpdate);
            this.controlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imgCamUser;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button snapshotButton;
        private System.Windows.Forms.Button cameraButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.PictureBox pbRecording;
        private System.Windows.Forms.Label dateTimeLabel;
        private System.Windows.Forms.Label testing_params;
        private System.Windows.Forms.FlowLayoutPanel controlButtons;
        private System.Windows.Forms.Label camera_number;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

