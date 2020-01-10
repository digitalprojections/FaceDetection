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
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
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
            // backgroundWorkerMain
            // 
            this.backgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerMain_DoWork);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = global::FaceDetection.Properties.Settings.Default.main_screen_size;
            this.DataBindings.Add(new System.Windows.Forms.Binding("ClientSize", global::FaceDetection.Properties.Settings.Default, "main_screen_size", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "UVC Camera Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
    }
}