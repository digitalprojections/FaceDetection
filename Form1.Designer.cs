namespace FaceDetection
{
    partial class mainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pbRecording = new System.Windows.Forms.PictureBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.cameraButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.folderButton = new System.Windows.Forms.Button();
            this.imgCamUser = new Emgu.CV.UI.ImageBox();
            this.dateTimeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).BeginInit();
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
            // pbRecording
            // 
            this.pbRecording.BackColor = System.Drawing.Color.Transparent;
            this.pbRecording.Image = global::FaceDetection.Properties.Resources.Record_Pressed_icon1;
            this.pbRecording.Location = new System.Drawing.Point(12, 12);
            this.pbRecording.Name = "pbRecording";
            this.pbRecording.Size = new System.Drawing.Size(52, 52);
            this.pbRecording.TabIndex = 4;
            this.pbRecording.TabStop = false;
            this.pbRecording.Visible = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ImageIndex = 6;
            this.closeButton.ImageList = this.imageList1;
            this.closeButton.Location = new System.Drawing.Point(598, 440);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(52, 52);
            this.closeButton.TabIndex = 3;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Visible = false;
            this.closeButton.Click += new System.EventHandler(this.ShowButtons);
            // 
            // cameraButton
            // 
            this.cameraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.cameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cameraButton.ImageIndex = 10;
            this.cameraButton.ImageList = this.imageList1;
            this.cameraButton.Location = new System.Drawing.Point(540, 440);
            this.cameraButton.Name = "cameraButton";
            this.cameraButton.Size = new System.Drawing.Size(52, 52);
            this.cameraButton.TabIndex = 3;
            this.cameraButton.UseVisualStyleBackColor = true;
            this.cameraButton.Visible = false;
            this.cameraButton.Click += new System.EventHandler(this.CameraButton_Click);
            // 
            // snapshotButton
            // 
            this.snapshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.snapshotButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.snapshotButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.snapshotButton.ImageIndex = 5;
            this.snapshotButton.ImageList = this.imageList1;
            this.snapshotButton.Location = new System.Drawing.Point(482, 440);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(52, 52);
            this.snapshotButton.TabIndex = 3;
            this.snapshotButton.UseVisualStyleBackColor = true;
            this.snapshotButton.Visible = false;
            // 
            // settingsButton
            // 
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.ImageIndex = 9;
            this.settingsButton.ImageList = this.imageList1;
            this.settingsButton.Location = new System.Drawing.Point(424, 440);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(52, 52);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Visible = false;
            this.settingsButton.Click += new System.EventHandler(this.showSettings);
            // 
            // folderButton
            // 
            this.folderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.folderButton.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.folderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.folderButton.ImageIndex = 7;
            this.folderButton.ImageList = this.imageList1;
            this.folderButton.Location = new System.Drawing.Point(366, 440);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(52, 52);
            this.folderButton.TabIndex = 3;
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Visible = false;
            this.folderButton.Click += new System.EventHandler(this.openStoreLocation);
            // 
            // imgCamUser
            // 
            this.imgCamUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgCamUser.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imgCamUser.Location = new System.Drawing.Point(0, 0);
            this.imgCamUser.Name = "imgCamUser";
            this.imgCamUser.Size = new System.Drawing.Size(662, 504);
            this.imgCamUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgCamUser.TabIndex = 2;
            this.imgCamUser.TabStop = false;
            this.imgCamUser.DoubleClick += new System.EventHandler(this.fullScreen);
            this.imgCamUser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.holdButton);
            this.imgCamUser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.releaseButton);
            // 
            // dateTimeLabel
            // 
            this.dateTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dateTimeLabel.AutoSize = true;
            this.dateTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.dateTimeLabel.Enabled = false;
            this.dateTimeLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dateTimeLabel.Location = new System.Drawing.Point(12, 471);
            this.dateTimeLabel.Name = "dateTimeLabel";
            this.dateTimeLabel.Size = new System.Drawing.Size(119, 24);
            this.dateTimeLabel.TabIndex = 5;
            this.dateTimeLabel.Text = "Date and time";
            this.dateTimeLabel.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(505, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "currently used paramenters";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 504);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimeLabel);
            this.Controls.Add(this.pbRecording);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.cameraButton);
            this.Controls.Add(this.snapshotButton);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.folderButton);
            this.Controls.Add(this.imgCamUser);
            this.Name = "mainForm";
            this.Text = "UVC Camera Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pbRecording)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCamUser)).EndInit();
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
        private System.Windows.Forms.Label label1;
    }
}

