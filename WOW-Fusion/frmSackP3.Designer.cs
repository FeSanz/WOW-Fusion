namespace WOW_Fusion
{
    partial class frmSackP3
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSackP3));
            this.lblItemNumber = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblItemDescription = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.btnGetWeight = new System.Windows.Forms.Button();
            this.txtBoxWeight = new System.Windows.Forms.TextBox();
            this.btnPrintPallet = new System.Windows.Forms.Button();
            this.lblMachineName = new System.Windows.Forms.Label();
            this.lblMachineCode = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblPlannedQuantity = new System.Windows.Forms.Label();
            this.lblUoM = new System.Windows.Forms.Label();
            this.lblOperation = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnReloadWO = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCompletedQuantity = new System.Windows.Forms.Label();
            this.lblPlannedCompletionDate = new System.Windows.Forms.Label();
            this.lblPlannedStartDate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWorkOrders = new System.Windows.Forms.ComboBox();
            this.lblLocationCode = new System.Windows.Forms.Label();
            this.dgWeights = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoRollo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Peso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgWeights)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblItemNumber
            // 
            this.lblItemNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblItemNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemNumber.Location = new System.Drawing.Point(101, 147);
            this.lblItemNumber.Name = "lblItemNumber";
            this.lblItemNumber.Size = new System.Drawing.Size(373, 15);
            this.lblItemNumber.TabIndex = 16;
            this.lblItemNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(37, 149);
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
            this.lblItemDescription.Location = new System.Drawing.Point(101, 168);
            this.lblItemDescription.Name = "lblItemDescription";
            this.lblItemDescription.Size = new System.Drawing.Size(373, 29);
            this.lblItemDescription.TabIndex = 6;
            this.lblItemDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(24, 176);
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
            this.btnGetWeight.Location = new System.Drawing.Point(341, 12);
            this.btnGetWeight.Name = "btnGetWeight";
            this.btnGetWeight.Size = new System.Drawing.Size(103, 47);
            this.btnGetWeight.TabIndex = 95;
            this.btnGetWeight.Text = "OBTENER";
            this.btnGetWeight.UseVisualStyleBackColor = false;
            // 
            // txtBoxWeight
            // 
            this.txtBoxWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxWeight.Location = new System.Drawing.Point(12, 12);
            this.txtBoxWeight.Multiline = true;
            this.txtBoxWeight.Name = "txtBoxWeight";
            this.txtBoxWeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBoxWeight.Size = new System.Drawing.Size(308, 43);
            this.txtBoxWeight.TabIndex = 92;
            this.txtBoxWeight.Text = "0";
            this.txtBoxWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPrintPallet
            // 
            this.btnPrintPallet.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnPrintPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintPallet.Location = new System.Drawing.Point(403, 472);
            this.btnPrintPallet.Name = "btnPrintPallet";
            this.btnPrintPallet.Size = new System.Drawing.Size(120, 54);
            this.btnPrintPallet.TabIndex = 89;
            this.btnPrintPallet.Text = "REGISTRAR";
            this.btnPrintPallet.UseVisualStyleBackColor = false;
            // 
            // lblMachineName
            // 
            this.lblMachineName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMachineName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineName.Location = new System.Drawing.Point(287, 125);
            this.lblMachineName.Name = "lblMachineName";
            this.lblMachineName.Size = new System.Drawing.Size(187, 15);
            this.lblMachineName.TabIndex = 25;
            this.lblMachineName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMachineCode
            // 
            this.lblMachineCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMachineCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineCode.Location = new System.Drawing.Point(101, 127);
            this.lblMachineCode.Name = "lblMachineCode";
            this.lblMachineCode.Size = new System.Drawing.Size(160, 15);
            this.lblMachineCode.TabIndex = 24;
            this.lblMachineCode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(39, 127);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(51, 13);
            this.label22.TabIndex = 23;
            this.label22.Text = "Máquina:";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPlannedQuantity
            // 
            this.lblPlannedQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlannedQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedQuantity.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblPlannedQuantity.Location = new System.Drawing.Point(302, 35);
            this.lblPlannedQuantity.Name = "lblPlannedQuantity";
            this.lblPlannedQuantity.Size = new System.Drawing.Size(83, 34);
            this.lblPlannedQuantity.TabIndex = 22;
            this.lblPlannedQuantity.Text = "0";
            this.lblPlannedQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUoM
            // 
            this.lblUoM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUoM.Location = new System.Drawing.Point(480, 46);
            this.lblUoM.Name = "lblUoM";
            this.lblUoM.Size = new System.Drawing.Size(34, 13);
            this.lblUoM.TabIndex = 21;
            this.lblUoM.Text = "UoM";
            this.lblUoM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOperation
            // 
            this.lblOperation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOperation.Location = new System.Drawing.Point(101, 76);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(373, 15);
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
            this.label7.Text = "Operación:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(413, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "REAL";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblItemDescription);
            this.groupBox1.Controls.Add(this.lblItemNumber);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.btnReloadWO);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.lblMachineName);
            this.groupBox1.Controls.Add(this.lblMachineCode);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.lblPlannedQuantity);
            this.groupBox1.Controls.Add(this.lblUoM);
            this.groupBox1.Controls.Add(this.lblOperation);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblCompletedQuantity);
            this.groupBox1.Controls.Add(this.lblPlannedCompletionDate);
            this.groupBox1.Controls.Add(this.lblPlannedStartDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbWorkOrders);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 213);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orden de trabajo";
            // 
            // btnReloadWO
            // 
            this.btnReloadWO.BackColor = System.Drawing.Color.Transparent;
            this.btnReloadWO.BackgroundImage = global::WOW_Fusion.Properties.Resources.upload_01;
            this.btnReloadWO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReloadWO.FlatAppearance.BorderSize = 0;
            this.btnReloadWO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReloadWO.Location = new System.Drawing.Point(249, 31);
            this.btnReloadWO.Name = "btnReloadWO";
            this.btnReloadWO.Size = new System.Drawing.Size(36, 34);
            this.btnReloadWO.TabIndex = 26;
            this.btnReloadWO.UseVisualStyleBackColor = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(329, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "META";
            // 
            // lblCompletedQuantity
            // 
            this.lblCompletedQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCompletedQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletedQuantity.ForeColor = System.Drawing.Color.Firebrick;
            this.lblCompletedQuantity.Location = new System.Drawing.Point(391, 35);
            this.lblCompletedQuantity.Name = "lblCompletedQuantity";
            this.lblCompletedQuantity.Size = new System.Drawing.Size(83, 34);
            this.lblCompletedQuantity.TabIndex = 9;
            this.lblCompletedQuantity.Text = "0";
            this.lblCompletedQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlannedCompletionDate
            // 
            this.lblPlannedCompletionDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlannedCompletionDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedCompletionDate.Location = new System.Drawing.Point(314, 100);
            this.lblPlannedCompletionDate.Name = "lblPlannedCompletionDate";
            this.lblPlannedCompletionDate.Size = new System.Drawing.Size(160, 15);
            this.lblPlannedCompletionDate.TabIndex = 7;
            this.lblPlannedCompletionDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlannedStartDate
            // 
            this.lblPlannedStartDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlannedStartDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedStartDate.Location = new System.Drawing.Point(101, 101);
            this.lblPlannedStartDate.Name = "lblPlannedStartDate";
            this.lblPlannedStartDate.Size = new System.Drawing.Size(160, 15);
            this.lblPlannedStartDate.TabIndex = 6;
            this.lblPlannedStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(284, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Fin:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(56, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Inicio:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Orden de trabajo:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbWorkOrders
            // 
            this.cmbWorkOrders.FormattingEnabled = true;
            this.cmbWorkOrders.Location = new System.Drawing.Point(101, 39);
            this.cmbWorkOrders.Name = "cmbWorkOrders";
            this.cmbWorkOrders.Size = new System.Drawing.Size(142, 21);
            this.cmbWorkOrders.TabIndex = 0;
            this.cmbWorkOrders.Text = "Seleccione orden";
            this.cmbWorkOrders.DropDown += new System.EventHandler(this.cmbWorkOrders_DropDown);
            this.cmbWorkOrders.SelectedIndexChanged += new System.EventHandler(this.cmbWorkOrders_SelectedIndexChanged);
            // 
            // lblLocationCode
            // 
            this.lblLocationCode.AutoSize = true;
            this.lblLocationCode.Font = new System.Drawing.Font("Bahnschrift SemiBold", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationCode.Location = new System.Drawing.Point(469, 58);
            this.lblLocationCode.Name = "lblLocationCode";
            this.lblLocationCode.Size = new System.Drawing.Size(57, 13);
            this.lblLocationCode.TabIndex = 82;
            this.lblLocationCode.Text = "LOC.CODE";
            this.lblLocationCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgWeights
            // 
            this.dgWeights.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgWeights.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgWeights.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgWeights.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgWeights.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Tai Le", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgWeights.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgWeights.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgWeights.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.NoRollo,
            this.Peso});
            this.dgWeights.Location = new System.Drawing.Point(12, 295);
            this.dgWeights.Name = "dgWeights";
            this.dgWeights.ReadOnly = true;
            this.dgWeights.RowTemplate.Height = 45;
            this.dgWeights.Size = new System.Drawing.Size(514, 171);
            this.dgWeights.TabIndex = 86;
            this.dgWeights.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgWeights_CellContentClick);
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.No.DefaultCellStyle = dataGridViewCellStyle3;
            this.No.HeaderText = "No.";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.Width = 50;
            // 
            // NoRollo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NoRollo.DefaultCellStyle = dataGridViewCellStyle4;
            this.NoRollo.HeaderText = "Peso Neto (Kg)";
            this.NoRollo.Name = "NoRollo";
            this.NoRollo.ReadOnly = true;
            // 
            // Peso
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Peso.DefaultCellStyle = dataGridViewCellStyle5;
            this.Peso.HeaderText = "Peso Bruto (Kg)";
            this.Peso.Name = "Peso";
            this.Peso.ReadOnly = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(466, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(60, 47);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 84;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label2.Location = new System.Drawing.Point(12, 487);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 34);
            this.label2.TabIndex = 112;
            this.label2.Text = "0";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(38, 471);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 111;
            this.label5.Text = "TARA";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Green;
            this.label6.Location = new System.Drawing.Point(137, 487);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 34);
            this.label6.TabIndex = 110;
            this.label6.Text = "0";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(340, 498);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 109;
            this.label8.Text = "KG";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(273, 471);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 108;
            this.label11.Text = "BRUTO";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(164, 471);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 13);
            this.label12.TabIndex = 107;
            this.label12.Text = "NETO";
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Firebrick;
            this.label13.Location = new System.Drawing.Point(251, 487);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 34);
            this.label13.TabIndex = 106;
            this.label13.Text = "0";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmSackP3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 545);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnGetWeight);
            this.Controls.Add(this.txtBoxWeight);
            this.Controls.Add(this.btnPrintPallet);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblLocationCode);
            this.Controls.Add(this.dgWeights);
            this.Name = "frmSackP3";
            this.Text = "WOW P3 Sacos";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgWeights)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblItemNumber;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblItemDescription;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btnGetWeight;
        private System.Windows.Forms.TextBox txtBoxWeight;
        private System.Windows.Forms.Button btnPrintPallet;
        private System.Windows.Forms.Button btnReloadWO;
        private System.Windows.Forms.Label lblMachineName;
        private System.Windows.Forms.Label lblMachineCode;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblPlannedQuantity;
        private System.Windows.Forms.Label lblUoM;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCompletedQuantity;
        private System.Windows.Forms.Label lblPlannedCompletionDate;
        private System.Windows.Forms.Label lblPlannedStartDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWorkOrders;
        private System.Windows.Forms.Label lblLocationCode;
        private System.Windows.Forms.DataGridView dgWeights;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoRollo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Peso;
    }
}