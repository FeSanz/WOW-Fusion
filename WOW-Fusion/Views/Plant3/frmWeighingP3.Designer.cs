namespace WOW_Fusion.Views.Plant3
{
    partial class frmWeighingP3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWeighingP3));
            this.lblStatusProcess = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblWeight = new System.Windows.Forms.Label();
            this.btnGetWeight = new System.Windows.Forms.Button();
            this.txtItemDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pbWaitProcess = new System.Windows.Forms.PictureBox();
            this.btnCloseWeighing = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWaitProcess)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStatusProcess
            // 
            this.lblStatusProcess.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusProcess.ForeColor = System.Drawing.Color.White;
            this.lblStatusProcess.Location = new System.Drawing.Point(49, 6);
            this.lblStatusProcess.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusProcess.Name = "lblStatusProcess";
            this.lblStatusProcess.Size = new System.Drawing.Size(413, 18);
            this.lblStatusProcess.TabIndex = 192;
            this.lblStatusProcess.Text = "¡Bienvenido!";
            this.lblStatusProcess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.99065F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.00935F));
            this.tableLayoutPanel1.Controls.Add(this.lblWeight, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGetWeight, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 36);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(481, 55);
            this.tableLayoutPanel1.TabIndex = 190;
            // 
            // lblWeight
            // 
            this.lblWeight.BackColor = System.Drawing.Color.White;
            this.lblWeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeight.ForeColor = System.Drawing.Color.Black;
            this.lblWeight.Location = new System.Drawing.Point(4, 0);
            this.lblWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.lblWeight.Size = new System.Drawing.Size(319, 55);
            this.lblWeight.TabIndex = 191;
            this.lblWeight.Text = "0.0";
            this.lblWeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnGetWeight
            // 
            this.btnGetWeight.BackColor = System.Drawing.Color.LimeGreen;
            this.btnGetWeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGetWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetWeight.ForeColor = System.Drawing.Color.White;
            this.btnGetWeight.Location = new System.Drawing.Point(331, 4);
            this.btnGetWeight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGetWeight.Name = "btnGetWeight";
            this.btnGetWeight.Size = new System.Drawing.Size(146, 47);
            this.btnGetWeight.TabIndex = 75;
            this.btnGetWeight.Text = "PESAR";
            this.btnGetWeight.UseVisualStyleBackColor = false;
            this.btnGetWeight.Click += new System.EventHandler(this.btnGetWeight_Click);
            // 
            // txtItemDescription
            // 
            this.txtItemDescription.Location = new System.Drawing.Point(12, 121);
            this.txtItemDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtItemDescription.Name = "txtItemDescription";
            this.txtItemDescription.Size = new System.Drawing.Size(475, 22);
            this.txtItemDescription.TabIndex = 193;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(16, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(476, 26);
            this.label3.TabIndex = 194;
            this.label3.Text = "Descripción del producto";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbWaitProcess
            // 
            this.pbWaitProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbWaitProcess.Image = global::WOW_Fusion.Properties.Resources.preloader;
            this.pbWaitProcess.Location = new System.Drawing.Point(11, -2);
            this.pbWaitProcess.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbWaitProcess.Name = "pbWaitProcess";
            this.pbWaitProcess.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.pbWaitProcess.Size = new System.Drawing.Size(37, 34);
            this.pbWaitProcess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbWaitProcess.TabIndex = 191;
            this.pbWaitProcess.TabStop = false;
            this.pbWaitProcess.Visible = false;
            // 
            // btnCloseWeighing
            // 
            this.btnCloseWeighing.BackColor = System.Drawing.Color.Transparent;
            this.btnCloseWeighing.BackgroundImage = global::WOW_Fusion.Properties.Resources.close_outline;
            this.btnCloseWeighing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCloseWeighing.FlatAppearance.BorderSize = 0;
            this.btnCloseWeighing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseWeighing.Location = new System.Drawing.Point(471, 2);
            this.btnCloseWeighing.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCloseWeighing.Name = "btnCloseWeighing";
            this.btnCloseWeighing.Size = new System.Drawing.Size(27, 25);
            this.btnCloseWeighing.TabIndex = 197;
            this.btnCloseWeighing.UseVisualStyleBackColor = false;
            this.btnCloseWeighing.Click += new System.EventHandler(this.btnCloseWeighing_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lblStatus.Location = new System.Drawing.Point(11, 146);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(477, 42);
            this.lblStatus.TabIndex = 196;
            this.lblStatus.Text = "...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmWeighingP3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(66)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(504, 192);
            this.Controls.Add(this.btnCloseWeighing);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtItemDescription);
            this.Controls.Add(this.lblStatusProcess);
            this.Controls.Add(this.pbWaitProcess);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmWeighingP3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pesaje";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWeighingP3_FormClosed);
            this.Load += new System.EventHandler(this.frmWeighingP3_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbWaitProcess)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatusProcess;
        private System.Windows.Forms.PictureBox pbWaitProcess;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblWeight;
        private System.Windows.Forms.Button btnGetWeight;
        private System.Windows.Forms.TextBox txtItemDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCloseWeighing;
        private System.Windows.Forms.Label lblStatus;
    }
}