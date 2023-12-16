namespace WOW_Fusion
{
    partial class frmPetP3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPetP3));
            this.lblItemNumber = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblItemDescription = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.btnGetWeight = new System.Windows.Forms.Button();
            this.txtBoxWeight = new System.Windows.Forms.TextBox();
            this.btnPrintPallet = new System.Windows.Forms.Button();
            this.btnReloadWO = new System.Windows.Forms.Button();
            this.lblOperation = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFechaEntrada = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWorkOrders = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLocationCode = new System.Windows.Forms.Label();
            this.lblNeto = new System.Windows.Forms.Label();
            this.lblUoM = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblBruto = new System.Windows.Forms.Label();
            this.lblTara = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblItemNumber
            // 
            this.lblItemNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblItemNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemNumber.Location = new System.Drawing.Point(101, 115);
            this.lblItemNumber.Name = "lblItemNumber";
            this.lblItemNumber.Size = new System.Drawing.Size(383, 15);
            this.lblItemNumber.TabIndex = 16;
            this.lblItemNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(38, 117);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Producto:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblItemDescription
            // 
            this.lblItemDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblItemDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemDescription.Location = new System.Drawing.Point(101, 151);
            this.lblItemDescription.Name = "lblItemDescription";
            this.lblItemDescription.Size = new System.Drawing.Size(383, 29);
            this.lblItemDescription.TabIndex = 6;
            this.lblItemDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(26, 159);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(66, 13);
            this.label25.TabIndex = 4;
            this.label25.Text = "Descripción:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGetWeight
            // 
            this.btnGetWeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnGetWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetWeight.Location = new System.Drawing.Point(300, 17);
            this.btnGetWeight.Name = "btnGetWeight";
            this.btnGetWeight.Size = new System.Drawing.Size(103, 47);
            this.btnGetWeight.TabIndex = 95;
            this.btnGetWeight.Text = "OBTENER";
            this.btnGetWeight.UseVisualStyleBackColor = false;
            this.btnGetWeight.Click += new System.EventHandler(this.btnGetWeight_Click);
            // 
            // txtBoxWeight
            // 
            this.txtBoxWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxWeight.Location = new System.Drawing.Point(15, 17);
            this.txtBoxWeight.Multiline = true;
            this.txtBoxWeight.Name = "txtBoxWeight";
            this.txtBoxWeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBoxWeight.Size = new System.Drawing.Size(279, 43);
            this.txtBoxWeight.TabIndex = 92;
            this.txtBoxWeight.Text = "0";
            this.txtBoxWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPrintPallet
            // 
            this.btnPrintPallet.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnPrintPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintPallet.Location = new System.Drawing.Point(399, 289);
            this.btnPrintPallet.Name = "btnPrintPallet";
            this.btnPrintPallet.Size = new System.Drawing.Size(120, 54);
            this.btnPrintPallet.TabIndex = 89;
            this.btnPrintPallet.Text = "REGISTRAR";
            this.btnPrintPallet.UseVisualStyleBackColor = false;
            // 
            // btnReloadWO
            // 
            this.btnReloadWO.BackColor = System.Drawing.Color.Transparent;
            this.btnReloadWO.BackgroundImage = global::WOW_Fusion.Properties.Resources.upload_01;
            this.btnReloadWO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReloadWO.FlatAppearance.BorderSize = 0;
            this.btnReloadWO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReloadWO.Location = new System.Drawing.Point(274, 31);
            this.btnReloadWO.Name = "btnReloadWO";
            this.btnReloadWO.Size = new System.Drawing.Size(36, 34);
            this.btnReloadWO.TabIndex = 26;
            this.btnReloadWO.UseVisualStyleBackColor = false;
            // 
            // lblOperation
            // 
            this.lblOperation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOperation.Location = new System.Drawing.Point(101, 76);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(383, 15);
            this.lblOperation.TabIndex = 16;
            this.lblOperation.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(33, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Proveedor:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.lblItemDescription);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.lblItemNumber);
            this.groupBox1.Controls.Add(this.btnReloadWO);
            this.groupBox1.Controls.Add(this.lblOperation);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblFechaEntrada);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbWorkOrders);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(15, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 197);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orden de trabajo";
            // 
            // lblFechaEntrada
            // 
            this.lblFechaEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFechaEntrada.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaEntrada.Location = new System.Drawing.Point(324, 39);
            this.lblFechaEntrada.Name = "lblFechaEntrada";
            this.lblFechaEntrada.Size = new System.Drawing.Size(160, 21);
            this.lblFechaEntrada.TabIndex = 6;
            this.lblFechaEntrada.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(373, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Fecha Entrada";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Orden de Compra:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbWorkOrders
            // 
            this.cmbWorkOrders.FormattingEnabled = true;
            this.cmbWorkOrders.Location = new System.Drawing.Point(101, 39);
            this.cmbWorkOrders.Name = "cmbWorkOrders";
            this.cmbWorkOrders.Size = new System.Drawing.Size(160, 21);
            this.cmbWorkOrders.TabIndex = 0;
            this.cmbWorkOrders.Text = "Seleccione orden";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(474, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(60, 47);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 84;
            this.pictureBox1.TabStop = false;
            // 
            // lblLocationCode
            // 
            this.lblLocationCode.AutoSize = true;
            this.lblLocationCode.Font = new System.Drawing.Font("Bahnschrift SemiBold", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationCode.Location = new System.Drawing.Point(478, 51);
            this.lblLocationCode.Name = "lblLocationCode";
            this.lblLocationCode.Size = new System.Drawing.Size(56, 13);
            this.lblLocationCode.TabIndex = 82;
            this.lblLocationCode.Text = "PLANTA 3";
            this.lblLocationCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNeto
            // 
            this.lblNeto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNeto.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNeto.ForeColor = System.Drawing.Color.Green;
            this.lblNeto.Location = new System.Drawing.Point(140, 299);
            this.lblNeto.Name = "lblNeto";
            this.lblNeto.Size = new System.Drawing.Size(83, 34);
            this.lblNeto.TabIndex = 103;
            this.lblNeto.Text = "0";
            this.lblNeto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUoM
            // 
            this.lblUoM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUoM.Location = new System.Drawing.Point(343, 310);
            this.lblUoM.Name = "lblUoM";
            this.lblUoM.Size = new System.Drawing.Size(34, 13);
            this.lblUoM.TabIndex = 102;
            this.lblUoM.Text = "KG";
            this.lblUoM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(276, 283);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 101;
            this.label10.Text = "BRUTO";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(167, 283);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 100;
            this.label9.Text = "NETO";
            // 
            // lblBruto
            // 
            this.lblBruto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBruto.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBruto.ForeColor = System.Drawing.Color.Firebrick;
            this.lblBruto.Location = new System.Drawing.Point(254, 299);
            this.lblBruto.Name = "lblBruto";
            this.lblBruto.Size = new System.Drawing.Size(83, 34);
            this.lblBruto.TabIndex = 99;
            this.lblBruto.Text = "0";
            this.lblBruto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTara
            // 
            this.lblTara.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTara.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTara.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblTara.Location = new System.Drawing.Point(15, 299);
            this.lblTara.Name = "lblTara";
            this.lblTara.Size = new System.Drawing.Size(83, 34);
            this.lblTara.TabIndex = 105;
            this.lblTara.Text = "0";
            this.lblTara.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(41, 283);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "TARA";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(409, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 47);
            this.button1.TabIndex = 106;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmPetP3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 357);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblTara);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblNeto);
            this.Controls.Add(this.lblUoM);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblBruto);
            this.Controls.Add(this.btnGetWeight);
            this.Controls.Add(this.txtBoxWeight);
            this.Controls.Add(this.btnPrintPallet);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblLocationCode);
            this.Name = "frmPetP3";
            this.Text = "frmPetP3";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblItemNumber;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblItemDescription;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btnGetWeight;
        private System.Windows.Forms.TextBox txtBoxWeight;
        private System.Windows.Forms.Button btnPrintPallet;
        private System.Windows.Forms.Button btnReloadWO;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblFechaEntrada;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWorkOrders;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLocationCode;
        private System.Windows.Forms.Label lblNeto;
        private System.Windows.Forms.Label lblUoM;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblBruto;
        private System.Windows.Forms.Label lblTara;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}