namespace WOW_Fusion
{
    partial class frmSettingsP2
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtBoxIpWeighing = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBoxPortWeighing = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBoxPortPrinter = new System.Windows.Forms.TextBox();
            this.txtBoxIpPrinter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtBoxArea = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbWorkCenters = new System.Windows.Forms.ComboBox();
            this.picBoxWaitWC = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancel.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(145, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 32);
            this.btnCancel.TabIndex = 124;
            this.btnCancel.Text = "CANCELAR";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.LimeGreen;
            this.btnSave.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(257, 362);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(97, 32);
            this.btnSave.TabIndex = 123;
            this.btnSave.Text = "GUARDAR";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtBoxIpWeighing
            // 
            this.txtBoxIpWeighing.Location = new System.Drawing.Point(61, 25);
            this.txtBoxIpWeighing.Name = "txtBoxIpWeighing";
            this.txtBoxIpWeighing.Size = new System.Drawing.Size(235, 21);
            this.txtBoxIpWeighing.TabIndex = 120;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBoxPortWeighing);
            this.groupBox1.Controls.Add(this.txtBoxIpWeighing);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Location = new System.Drawing.Point(27, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 102);
            this.groupBox1.TabIndex = 125;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Báscula";
            // 
            // txtBoxPortWeighing
            // 
            this.txtBoxPortWeighing.Location = new System.Drawing.Point(61, 60);
            this.txtBoxPortWeighing.Name = "txtBoxPortWeighing";
            this.txtBoxPortWeighing.Size = new System.Drawing.Size(235, 21);
            this.txtBoxPortWeighing.TabIndex = 121;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 21);
            this.label2.TabIndex = 124;
            this.label2.Text = "Puerto:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 21);
            this.label1.TabIndex = 123;
            this.label1.Text = "IP:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.Control;
            this.lblStatus.Location = new System.Drawing.Point(24, 342);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(330, 17);
            this.lblStatus.TabIndex = 126;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtBoxPortPrinter);
            this.groupBox2.Controls.Add(this.txtBoxIpPrinter);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Location = new System.Drawing.Point(27, 237);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 102);
            this.groupBox2.TabIndex = 126;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Impresora";
            // 
            // txtBoxPortPrinter
            // 
            this.txtBoxPortPrinter.Location = new System.Drawing.Point(61, 60);
            this.txtBoxPortPrinter.Name = "txtBoxPortPrinter";
            this.txtBoxPortPrinter.Size = new System.Drawing.Size(235, 21);
            this.txtBoxPortPrinter.TabIndex = 121;
            // 
            // txtBoxIpPrinter
            // 
            this.txtBoxIpPrinter.Location = new System.Drawing.Point(61, 25);
            this.txtBoxIpPrinter.Name = "txtBoxIpPrinter";
            this.txtBoxIpPrinter.Size = new System.Drawing.Size(235, 21);
            this.txtBoxIpPrinter.TabIndex = 120;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 21);
            this.label3.TabIndex = 124;
            this.label3.Text = "Puerto:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 21);
            this.label4.TabIndex = 123;
            this.label4.Text = "IP:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbWorkCenters);
            this.groupBox3.Controls.Add(this.txtBoxArea);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.picBoxWaitWC);
            this.groupBox3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Location = new System.Drawing.Point(27, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 102);
            this.groupBox3.TabIndex = 127;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Centro de trabajo";
            // 
            // txtBoxArea
            // 
            this.txtBoxArea.Enabled = false;
            this.txtBoxArea.Location = new System.Drawing.Point(61, 60);
            this.txtBoxArea.Name = "txtBoxArea";
            this.txtBoxArea.Size = new System.Drawing.Size(235, 21);
            this.txtBoxArea.TabIndex = 121;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 21);
            this.label5.TabIndex = 124;
            this.label5.Text = "Área:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 21);
            this.label6.TabIndex = 123;
            this.label6.Text = "Centro: ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbWorkCenters
            // 
            this.cmbWorkCenters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkCenters.FormattingEnabled = true;
            this.cmbWorkCenters.Location = new System.Drawing.Point(61, 25);
            this.cmbWorkCenters.Name = "cmbWorkCenters";
            this.cmbWorkCenters.Size = new System.Drawing.Size(235, 24);
            this.cmbWorkCenters.TabIndex = 128;
            this.cmbWorkCenters.DropDown += new System.EventHandler(this.cmbWorkCenters_DropDown);
            this.cmbWorkCenters.SelectedValueChanged += new System.EventHandler(this.cmbWorkCenters_SelectedValueChanged);
            // 
            // picBoxWaitWC
            // 
            this.picBoxWaitWC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBoxWaitWC.Image = global::WOW_Fusion.Properties.Resources.preloader_01;
            this.picBoxWaitWC.Location = new System.Drawing.Point(285, 17);
            this.picBoxWaitWC.Name = "picBoxWaitWC";
            this.picBoxWaitWC.Padding = new System.Windows.Forms.Padding(10);
            this.picBoxWaitWC.Size = new System.Drawing.Size(40, 40);
            this.picBoxWaitWC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxWaitWC.TabIndex = 129;
            this.picBoxWaitWC.TabStop = false;
            this.picBoxWaitWC.Visible = false;
            // 
            // frmSettingsP2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(366, 407);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSettingsP2";
            this.Text = "frmSettingsP2";
            this.Load += new System.EventHandler(this.frmSettingsP2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtBoxIpWeighing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBoxPortWeighing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBoxPortPrinter;
        private System.Windows.Forms.TextBox txtBoxIpPrinter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtBoxArea;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbWorkCenters;
        private System.Windows.Forms.PictureBox picBoxWaitWC;
    }
}