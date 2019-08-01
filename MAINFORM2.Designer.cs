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
            this.camera_number = new System.Windows.Forms.Label();
            this.testing_params = new System.Windows.Forms.Label();
            this.controlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dateTimeLabel = new System.Windows.Forms.Label();
            this.folderButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.cameraButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.pbRecording = new System.Windows.Forms.PictureBox();
            this.imgCamUser = new Emgu.CV.UI.ImageBox();
            this.controlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).BeginInit();
            this.SuspendLayout();
            // 
            // camera_number
            // 
            this.camera_number.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.camera_number.AutoSize = true;
            this.camera_number.BackColor = System.Drawing.Color.Transparent;
            this.camera_number.Enabled = false;
            this.camera_number.Font = new System.Drawing.Font("MS UI Gothic", 50F);
            this.camera_number.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.camera_number.Location = new System.Drawing.Point(725, 9);
            this.camera_number.Name = "camera_number";
            this.camera_number.Size = new System.Drawing.Size(63, 67);
            this.camera_number.TabIndex = 10;
            this.camera_number.Text = "4";
            // 
            // testing_params
            // 
            this.testing_params.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testing_params.AutoSize = true;
            this.testing_params.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.testing_params.Location = new System.Drawing.Point(580, 24);
            this.testing_params.Name = "testing_params";
            this.testing_params.Size = new System.Drawing.Size(139, 12);
            this.testing_params.TabIndex = 11;
            this.testing_params.Text = "currently used parameters";
            // 
            // controlButtons
            // 
            this.controlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.controlButtons.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.controlButtons.Controls.Add(this.folderButton);
            this.controlButtons.Controls.Add(this.settingsButton);
            this.controlButtons.Controls.Add(this.snapshotButton);
            this.controlButtons.Controls.Add(this.cameraButton);
            this.controlButtons.Controls.Add(this.closeButton);
            this.controlButtons.Location = new System.Drawing.Point(496, 379);
            this.controlButtons.Name = "controlButtons";
            this.controlButtons.Size = new System.Drawing.Size(292, 59);
            this.controlButtons.TabIndex = 12;
            this.controlButtons.Visible = false;
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
            this.dateTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimeLabel.AutoSize = true;
            this.dateTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.dateTimeLabel.Enabled = false;
            this.dateTimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F);
            this.dateTimeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateTimeLabel.Location = new System.Drawing.Point(12, 417);
            this.dateTimeLabel.Name = "dateTimeLabel";
            this.dateTimeLabel.Size = new System.Drawing.Size(119, 24);
            this.dateTimeLabel.TabIndex = 13;
            this.dateTimeLabel.Text = "Date and time";
            this.dateTimeLabel.UseCompatibleTextRendering = true;
            // 
            // folderButton
            // 
            this.folderButton.BackColor = System.Drawing.SystemColors.Control;
            this.folderButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.folderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.folderButton.Image = global::FaceDetection.Properties.Resources.Windows_icon;
            this.folderButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.folderButton.Location = new System.Drawing.Point(3, 3);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(52, 52);
            this.folderButton.TabIndex = 3;
            this.folderButton.UseVisualStyleBackColor = false;
            this.folderButton.Click += new System.EventHandler(this.OpenStoreLocation);
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.SystemColors.Control;
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Image = global::FaceDetection.Properties.Resources.Settings_2_icon;
            this.settingsButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.settingsButton.Location = new System.Drawing.Point(61, 3);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(52, 52);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.ShowSettings);
            // 
            // snapshotButton
            // 
            this.snapshotButton.BackColor = System.Drawing.SystemColors.Control;
            this.snapshotButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.snapshotButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.snapshotButton.Image = global::FaceDetection.Properties.Resources.camera_icon;
            this.snapshotButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.snapshotButton.Location = new System.Drawing.Point(119, 3);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(52, 52);
            this.snapshotButton.TabIndex = 3;
            this.snapshotButton.UseVisualStyleBackColor = false;
            this.snapshotButton.Click += new System.EventHandler(this.SnapShot);
            // 
            // cameraButton
            // 
            this.cameraButton.BackColor = System.Drawing.SystemColors.Control;
            this.cameraButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.cameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cameraButton.Image = global::FaceDetection.Properties.Resources.video_camera_icon;
            this.cameraButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cameraButton.Location = new System.Drawing.Point(177, 3);
            this.cameraButton.Name = "cameraButton";
            this.cameraButton.Size = new System.Drawing.Size(52, 52);
            this.cameraButton.TabIndex = 3;
            this.cameraButton.UseVisualStyleBackColor = false;
            this.cameraButton.Click += new System.EventHandler(this.StartVideoRecording);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.Control;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Image = global::FaceDetection.Properties.Resources.delete_1_icon;
            this.closeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.closeButton.Location = new System.Drawing.Point(235, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(52, 52);
            this.closeButton.TabIndex = 3;
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.ShowButtons);
            // 
            // pbRecording
            // 
            this.pbRecording.BackColor = System.Drawing.Color.Transparent;
            this.pbRecording.Image = global::FaceDetection.Properties.Resources.Record_Pressed_icon1;
            this.pbRecording.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbRecording.Location = new System.Drawing.Point(12, 12);
            this.pbRecording.Name = "pbRecording";
            this.pbRecording.Size = new System.Drawing.Size(52, 52);
            this.pbRecording.TabIndex = 5;
            this.pbRecording.TabStop = false;
            this.pbRecording.Visible = false;
            // 
            // imgCamUser
            // 
            this.imgCamUser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgCamUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgCamUser.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imgCamUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.imgCamUser.Location = new System.Drawing.Point(0, 0);
            this.imgCamUser.Name = "imgCamUser";
            this.imgCamUser.Size = new System.Drawing.Size(800, 450);
            this.imgCamUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgCamUser.TabIndex = 3;
            this.imgCamUser.TabStop = false;
            this.imgCamUser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HoldButton);
            this.imgCamUser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ReleaseButton);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dateTimeLabel);
            this.Controls.Add(this.controlButtons);
            this.Controls.Add(this.testing_params);
            this.Controls.Add(this.camera_number);
            this.Controls.Add(this.pbRecording);
            this.Controls.Add(this.imgCamUser);
            this.Name = "MainForm";
            this.Text = "MAINFORM2";
            this.controlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imgCamUser;
        private System.Windows.Forms.PictureBox pbRecording;
        private System.Windows.Forms.Label camera_number;
        private System.Windows.Forms.Label testing_params;
        private System.Windows.Forms.FlowLayoutPanel controlButtons;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button snapshotButton;
        private System.Windows.Forms.Button cameraButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label dateTimeLabel;
    }
}