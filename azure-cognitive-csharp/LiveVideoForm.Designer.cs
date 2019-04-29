namespace LiveVideoStreamAnalysis
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
            this.formLayout = new System.Windows.Forms.TableLayoutPanel();
            this.frameOriginal = new System.Windows.Forms.PictureBox();
            this.processedImage = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.formLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frameOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedImage)).BeginInit();
            this.SuspendLayout();
            // 
            // formLayout
            // 
            this.formLayout.ColumnCount = 2;
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formLayout.Controls.Add(this.frameOriginal, 0, 0);
            this.formLayout.Controls.Add(this.processedImage, 1, 0);
            this.formLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formLayout.Location = new System.Drawing.Point(0, 0);
            this.formLayout.Name = "formLayout";
            this.formLayout.RowCount = 1;
            this.formLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 599F));
            this.formLayout.Size = new System.Drawing.Size(1022, 599);
            this.formLayout.TabIndex = 0;
            // 
            // frameOriginal
            // 
            this.frameOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameOriginal.Location = new System.Drawing.Point(3, 3);
            this.frameOriginal.Name = "frameOriginal";
            this.frameOriginal.Size = new System.Drawing.Size(505, 593);
            this.frameOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.frameOriginal.TabIndex = 0;
            this.frameOriginal.TabStop = false;
            // 
            // processedImage
            // 
            this.processedImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processedImage.Location = new System.Drawing.Point(514, 3);
            this.processedImage.Name = "processedImage";
            this.processedImage.Size = new System.Drawing.Size(505, 593);
            this.processedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.processedImage.TabIndex = 1;
            this.processedImage.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(967, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 36);
            this.label1.TabIndex = 2;
            this.label1.Text = "fps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 36);
            this.label2.TabIndex = 3;
            this.label2.Text = "fps";
            // 
            // LiveVideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 599);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.formLayout);
            this.Name = "LiveVideoForm";
            this.Text = "Comuter Vision with Microsoft Azure Cognitive Services";
            this.Load += new System.EventHandler(this.LiveVideoForm_Load);
            this.formLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.frameOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox frameOriginal;
        public System.Windows.Forms.TableLayoutPanel formLayout;
        private System.Windows.Forms.PictureBox processedImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

