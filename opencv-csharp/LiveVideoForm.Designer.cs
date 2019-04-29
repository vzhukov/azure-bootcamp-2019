namespace OpenCVLiveVideoStreamAnalysis
{
    partial class LiveVideoForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.originFrame = new System.Windows.Forms.PictureBox();
            this.processedFrame = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.originFrame, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.processedFrame, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // originFrame
            // 
            this.originFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originFrame.Location = new System.Drawing.Point(3, 3);
            this.originFrame.Name = "originFrame";
            this.originFrame.Size = new System.Drawing.Size(394, 444);
            this.originFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.originFrame.TabIndex = 0;
            this.originFrame.TabStop = false;
            // 
            // processedFrame
            // 
            this.processedFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processedFrame.Location = new System.Drawing.Point(403, 3);
            this.processedFrame.Name = "processedFrame";
            this.processedFrame.Size = new System.Drawing.Size(394, 444);
            this.processedFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.processedFrame.TabIndex = 1;
            this.processedFrame.TabStop = false;
            // 
            // LiveVideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LiveVideoForm";
            this.Text = "Computer Vision with OpenCV";
            this.Load += new System.EventHandler(this.LiveVideoForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.originFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox originFrame;
        private System.Windows.Forms.PictureBox processedFrame;
    }
}

