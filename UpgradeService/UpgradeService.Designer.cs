namespace UpgradeService
{
    partial class UpgradeService
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradeService));
            this.imageList_downLoad = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox_load = new System.Windows.Forms.PictureBox();
            this.pictureBox_decompression = new System.Windows.Forms.PictureBox();
            this.label_info = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_load)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_decompression)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList_downLoad
            // 
            this.imageList_downLoad.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_downLoad.ImageStream")));
            this.imageList_downLoad.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_downLoad.Images.SetKeyName(0, "frame_start.png");
            this.imageList_downLoad.Images.SetKeyName(1, "frame_end.png");
            // 
            // pictureBox_load
            // 
            this.pictureBox_load.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_load.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_load.Name = "pictureBox_load";
            this.pictureBox_load.Size = new System.Drawing.Size(400, 300);
            this.pictureBox_load.TabIndex = 1;
            this.pictureBox_load.TabStop = false;
            // 
            // pictureBox_decompression
            // 
            this.pictureBox_decompression.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_decompression.Location = new System.Drawing.Point(120, 173);
            this.pictureBox_decompression.Name = "pictureBox_decompression";
            this.pictureBox_decompression.Size = new System.Drawing.Size(160, 120);
            this.pictureBox_decompression.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_decompression.TabIndex = 2;
            this.pictureBox_decompression.TabStop = false;
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.BackColor = System.Drawing.Color.Transparent;
            this.label_info.Font = new System.Drawing.Font("幼圆", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_info.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label_info.Location = new System.Drawing.Point(303, 279);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(65, 12);
            this.label_info.TabIndex = 3;
            this.label_info.Text = "label_info";
            // 
            // UpgradeService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.label_info);
            this.Controls.Add(this.pictureBox_decompression);
            this.Controls.Add(this.pictureBox_load);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpgradeService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UpgradeService";
            this.Load += new System.EventHandler(this.UpgradeService_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_load)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_decompression)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_load;
        private System.Windows.Forms.ImageList imageList_downLoad;
        private System.Windows.Forms.PictureBox pictureBox_decompression;
        private System.Windows.Forms.Label label_info;
    }
}

