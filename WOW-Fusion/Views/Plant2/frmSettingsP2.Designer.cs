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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettingsP2));
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
            this.txtBoxWorkCenter = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbResources = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.picBoxWaitWC = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtPallet = new System.Windows.Forms.TextBox();
            this.txtRoll = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdbProd = new System.Windows.Forms.RadioButton();
            this.rdbTest = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancel.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(140, 614);
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
            this.btnSave.Location = new System.Drawing.Point(243, 614);
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
            this.groupBox1.Location = new System.Drawing.Point(13, 211);
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
            this.txtBoxPortWeighing.TextChanged += new System.EventHandler(this.txtBoxPortWeighing_TextChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 21);
            this.label2.TabIndex = 124;
            this.label2.Text = "Puerto:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 25);
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
            this.lblStatus.Location = new System.Drawing.Point(10, 594);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(330, 17);
            this.lblStatus.TabIndex = 126;
            this.lblStatus.Text = "...";
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
            this.groupBox2.Location = new System.Drawing.Point(13, 103);
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
            this.txtBoxPortPrinter.TextChanged += new System.EventHandler(this.txtBoxPortPrinter_TextChanged);
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
            this.label3.Location = new System.Drawing.Point(6, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 21);
            this.label3.TabIndex = 124;
            this.label3.Text = "Puerto:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 21);
            this.label4.TabIndex = 123;
            this.label4.Text = "IP:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtBoxWorkCenter);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.cmbResources);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.picBoxWaitWC);
            this.groupBox3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Location = new System.Drawing.Point(13, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 94);
            this.groupBox3.TabIndex = 127;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Centro de trabajo";
            // 
            // txtBoxWorkCenter
            // 
            this.txtBoxWorkCenter.Enabled = false;
            this.txtBoxWorkCenter.Location = new System.Drawing.Point(61, 59);
            this.txtBoxWorkCenter.Name = "txtBoxWorkCenter";
            this.txtBoxWorkCenter.Size = new System.Drawing.Size(235, 21);
            this.txtBoxWorkCenter.TabIndex = 130;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 21);
            this.label9.TabIndex = 131;
            this.label9.Text = "Centro:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbResources
            // 
            this.cmbResources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResources.FormattingEnabled = true;
            this.cmbResources.Location = new System.Drawing.Point(61, 25);
            this.cmbResources.Name = "cmbResources";
            this.cmbResources.Size = new System.Drawing.Size(235, 24);
            this.cmbResources.TabIndex = 128;
            this.cmbResources.DropDown += new System.EventHandler(this.cmbWorkCenters_DropDown);
            this.cmbResources.SelectedValueChanged += new System.EventHandler(this.cmbWorkCenters_SelectedValueChanged);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 21);
            this.label6.TabIndex = 123;
            this.label6.Text = "Recurso:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picBoxWaitWC
            // 
            this.picBoxWaitWC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBoxWaitWC.Image = global::WOW_Fusion.Properties.Resources.preloader;
            this.picBoxWaitWC.Location = new System.Drawing.Point(290, 17);
            this.picBoxWaitWC.Name = "picBoxWaitWC";
            this.picBoxWaitWC.Padding = new System.Windows.Forms.Padding(10);
            this.picBoxWaitWC.Size = new System.Drawing.Size(40, 40);
            this.picBoxWaitWC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxWaitWC.TabIndex = 129;
            this.picBoxWaitWC.TabStop = false;
            this.picBoxWaitWC.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtPallet);
            this.groupBox4.Controls.Add(this.txtRoll);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Location = new System.Drawing.Point(13, 319);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 102);
            this.groupBox4.TabIndex = 127;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "No. Impresiones";
            // 
            // txtPallet
            // 
            this.txtPallet.Location = new System.Drawing.Point(61, 60);
            this.txtPallet.Name = "txtPallet";
            this.txtPallet.Size = new System.Drawing.Size(235, 21);
            this.txtPallet.TabIndex = 121;
            this.txtPallet.TextChanged += new System.EventHandler(this.txtPallet_TextChanged);
            // 
            // txtRoll
            // 
            this.txtRoll.Location = new System.Drawing.Point(61, 25);
            this.txtRoll.Name = "txtRoll";
            this.txtRoll.Size = new System.Drawing.Size(235, 21);
            this.txtRoll.TabIndex = 120;
            this.txtRoll.TextChanged += new System.EventHandler(this.txtRoll_TextChanged);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 21);
            this.label7.TabIndex = 124;
            this.label7.Text = "Palet:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 21);
            this.label8.TabIndex = 123;
            this.label8.Text = "Rollo:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdbProd);
            this.groupBox5.Controls.Add(this.rdbTest);
            this.groupBox5.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox5.Location = new System.Drawing.Point(13, 427);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 56);
            this.groupBox5.TabIndex = 128;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ambientes";
            // 
            // rdbProd
            // 
            this.rdbProd.AutoSize = true;
            this.rdbProd.Location = new System.Drawing.Point(230, 23);
            this.rdbProd.Name = "rdbProd";
            this.rdbProd.Size = new System.Drawing.Size(57, 20);
            this.rdbProd.TabIndex = 126;
            this.rdbProd.TabStop = true;
            this.rdbProd.Text = "PROD";
            this.rdbProd.UseVisualStyleBackColor = true;
            this.rdbProd.CheckedChanged += new System.EventHandler(this.rdbProd_CheckedChanged);
            // 
            // rdbTest
            // 
            this.rdbTest.AutoSize = true;
            this.rdbTest.Location = new System.Drawing.Point(61, 23);
            this.rdbTest.Name = "rdbTest";
            this.rdbTest.Size = new System.Drawing.Size(47, 20);
            this.rdbTest.TabIndex = 125;
            this.rdbTest.TabStop = true;
            this.rdbTest.Text = "TEST";
            this.rdbTest.UseVisualStyleBackColor = true;
            this.rdbTest.CheckedChanged += new System.EventHandler(this.rdbTest_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox4);
            this.flowLayoutPanel1.Controls.Add(this.groupBox5);
            this.flowLayoutPanel1.Controls.Add(this.groupBox6);
            this.flowLayoutPanel1.Controls.Add(this.lblStatus);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(343, 667);
            this.flowLayoutPanel1.TabIndex = 129;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtPassword);
            this.groupBox6.Controls.Add(this.txtUser);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox6.Location = new System.Drawing.Point(13, 489);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(327, 102);
            this.groupBox6.TabIndex = 129;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Credenciales";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(61, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(235, 21);
            this.txtPassword.TabIndex = 121;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(61, 25);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(235, 21);
            this.txtUser.TabIndex = 120;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 21);
            this.label5.TabIndex = 124;
            this.label5.Text = "Contra.:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 21);
            this.label10.TabIndex = 123;
            this.label10.Text = "Usuario:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmSettingsP2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(370, 680);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettingsP2";
            this.Text = "frmSettingsP2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSettingsP2_FormClosed);
            this.Load += new System.EventHandler(this.frmSettingsP2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbResources;
        private System.Windows.Forms.PictureBox picBoxWaitWC;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPallet;
        private System.Windows.Forms.TextBox txtRoll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBoxWorkCenter;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rdbProd;
        private System.Windows.Forms.RadioButton rdbTest;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
    }
}