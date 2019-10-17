namespace FaceDetection
{
    partial class CameraSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.pictureBox_recordUponStart = new System.Windows.Forms.PictureBox();
            this.nud_erase_old = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_delete_old = new System.Windows.Forms.CheckBox();
            this.numericUpDownCamCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.storePath = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.cb_all_cameras = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialogStoreFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.button_apply = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_recordUponStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_erase_old)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCamCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_cancel);
            this.groupBox1.Controls.Add(this.button_apply);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.groupBox12);
            this.groupBox1.Controls.Add(this.numericUpDownCamCount);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.cb_all_cameras);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(489, 262);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Capture settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(6, 84);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 34);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox12.Controls.Add(this.pictureBox_recordUponStart);
            this.groupBox12.Controls.Add(this.nud_erase_old);
            this.groupBox12.Controls.Add(this.label3);
            this.groupBox12.Controls.Add(this.cb_delete_old);
            this.groupBox12.Location = new System.Drawing.Point(6, 155);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(218, 101);
            this.groupBox12.TabIndex = 3;
            this.groupBox12.TabStop = false;
            // 
            // pictureBox_recordUponStart
            // 
            this.pictureBox_recordUponStart.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox_recordUponStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_recordUponStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox_recordUponStart.Location = new System.Drawing.Point(6, 0);
            this.pictureBox_recordUponStart.Name = "pictureBox_recordUponStart";
            this.pictureBox_recordUponStart.Size = new System.Drawing.Size(40, 34);
            this.pictureBox_recordUponStart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_recordUponStart.TabIndex = 10;
            this.pictureBox_recordUponStart.TabStop = false;
            // 
            // nud_erase_old
            // 
            this.nud_erase_old.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nud_erase_old.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FaceDetection.Properties.Settings.Default, "erase_after", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nud_erase_old.Font = new System.Drawing.Font("MS UI Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nud_erase_old.Location = new System.Drawing.Point(69, 22);
            this.nud_erase_old.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nud_erase_old.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_erase_old.Name = "nud_erase_old";
            this.nud_erase_old.Size = new System.Drawing.Size(143, 55);
            this.nud_erase_old.TabIndex = 3;
            this.nud_erase_old.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_erase_old.Value = global::FaceDetection.Properties.Settings.Default.erase_after;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(67, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "days old data to be deleted";
            // 
            // cb_delete_old
            // 
            this.cb_delete_old.AutoSize = true;
            this.cb_delete_old.Checked = global::FaceDetection.Properties.Settings.Default.erase_old;
            this.cb_delete_old.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "erase_old", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_delete_old.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cb_delete_old.Location = new System.Drawing.Point(33, 0);
            this.cb_delete_old.Name = "cb_delete_old";
            this.cb_delete_old.Size = new System.Drawing.Size(102, 16);
            this.cb_delete_old.TabIndex = 0;
            this.cb_delete_old.Text = "Delete old data";
            this.cb_delete_old.UseVisualStyleBackColor = true;
            // 
            // numericUpDownCamCount
            // 
            this.numericUpDownCamCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCamCount.Font = new System.Drawing.Font("MS UI Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numericUpDownCamCount.Location = new System.Drawing.Point(120, 21);
            this.numericUpDownCamCount.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownCamCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCamCount.Name = "numericUpDownCamCount";
            this.numericUpDownCamCount.Size = new System.Drawing.Size(104, 55);
            this.numericUpDownCamCount.TabIndex = 7;
            this.numericUpDownCamCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownCamCount.Value = global::FaceDetection.Properties.Settings.Default.camera_count;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button_Browse);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.storePath);
            this.groupBox2.Location = new System.Drawing.Point(240, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 171);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select save path";
            // 
            // button_Browse
            // 
            this.button_Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Browse.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Browse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button_Browse.Location = new System.Drawing.Point(113, 58);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(124, 107);
            this.button_Browse.TabIndex = 2;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.OpenStoreLocation);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(155, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Save file path";
            // 
            // storePath
            // 
            this.storePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.storePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FaceDetection.Properties.Settings.Default, "video_file_location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.storePath.Location = new System.Drawing.Point(6, 33);
            this.storePath.Name = "storePath";
            this.storePath.Size = new System.Drawing.Size(231, 19);
            this.storePath.TabIndex = 1;
            this.storePath.Text = global::FaceDetection.Properties.Settings.Default.video_file_location;
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label26.AutoSize = true;
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(118, 79);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(106, 12);
            this.label26.TabIndex = 1;
            this.label26.Text = "Connected cameras";
            // 
            // cb_all_cameras
            // 
            this.cb_all_cameras.AutoSize = true;
            this.cb_all_cameras.Checked = global::FaceDetection.Properties.Settings.Default.show_all_cams_simulteneously;
            this.cb_all_cameras.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FaceDetection.Properties.Settings.Default, "show_all_cams_simulteneously", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_all_cameras.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cb_all_cameras.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cb_all_cameras.Location = new System.Drawing.Point(29, 102);
            this.cb_all_cameras.Name = "cb_all_cameras";
            this.cb_all_cameras.Size = new System.Drawing.Size(205, 16);
            this.cb_all_cameras.TabIndex = 5;
            this.cb_all_cameras.Text = "Display all cameras simulteneously";
            this.cb_all_cameras.UseVisualStyleBackColor = true;
            // 
            // button_apply
            // 
            this.button_apply.Location = new System.Drawing.Point(240, 196);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(106, 60);
            this.button_apply.TabIndex = 11;
            this.button_apply.Text = "OK";
            this.button_apply.UseVisualStyleBackColor = true;
            this.button_apply.Click += new System.EventHandler(this.Button_apply_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(377, 195);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(106, 60);
            this.button_cancel.TabIndex = 11;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.Button_cancel_Click);
            // 
            // CameraSettingsForm
            // 
            this.AcceptButton = this.button_apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(509, 286);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Name = "CameraSettingsForm";
            this.Text = "Capture settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CameraSettingsForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_recordUponStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_erase_old)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCamCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownCamCount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.NumericUpDown nud_erase_old;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cb_delete_old;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox storePath;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.CheckBox cb_all_cameras;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogStoreFolder;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox_recordUponStart;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_apply;
    }
}