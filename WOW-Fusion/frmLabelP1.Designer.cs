﻿namespace WOW_Fusion
{
    partial class frmLabelP1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblItemDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblItemNumber = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblMachineName = new System.Windows.Forms.Label();
            this.lblMachineCode = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblPlannedQuantity = new System.Windows.Forms.Label();
            this.lblUoM = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPlannedCompletionDate = new System.Windows.Forms.Label();
            this.lblPlannedStartDate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWorkOrders = new System.Windows.Forms.ComboBox();
            this.picBoxWaitWO = new System.Windows.Forms.PictureBox();
            this.cmbWorkCenters = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.picBoxWaitWC = new System.Windows.Forms.PictureBox();
            this.btnReloadWO = new System.Windows.Forms.Button();
            this.btnPrintPallet = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureLabel = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBusinessUnit = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLocationCode = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblWorkAreaName = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblOrganizationName = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLabel)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblItemDescription
            // 
            this.lblItemDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblItemDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemDescription.Location = new System.Drawing.Point(83, 100);
            this.lblItemDescription.Name = "lblItemDescription";
            this.lblItemDescription.Size = new System.Drawing.Size(373, 29);
            this.lblItemDescription.TabIndex = 6;
            this.lblItemDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblItemDescription);
            this.groupBox1.Controls.Add(this.lblItemNumber);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.lblMachineName);
            this.groupBox1.Controls.Add(this.lblMachineCode);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.lblPlannedQuantity);
            this.groupBox1.Controls.Add(this.lblUoM);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblPlannedCompletionDate);
            this.groupBox1.Controls.Add(this.lblPlannedStartDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbWorkOrders);
            this.groupBox1.Controls.Add(this.picBoxWaitWO);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 212);
            this.groupBox1.TabIndex = 113;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orden de trabajo";
            // 
            // lblItemNumber
            // 
            this.lblItemNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblItemNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemNumber.Location = new System.Drawing.Point(83, 76);
            this.lblItemNumber.Name = "lblItemNumber";
            this.lblItemNumber.Size = new System.Drawing.Size(373, 15);
            this.lblItemNumber.TabIndex = 16;
            this.lblItemNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(6, 108);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(66, 13);
            this.label25.TabIndex = 4;
            this.label25.Text = "Descripción:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(19, 78);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Producto:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMachineName
            // 
            this.lblMachineName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMachineName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineName.Location = new System.Drawing.Point(269, 136);
            this.lblMachineName.Name = "lblMachineName";
            this.lblMachineName.Size = new System.Drawing.Size(187, 15);
            this.lblMachineName.TabIndex = 25;
            this.lblMachineName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMachineCode
            // 
            this.lblMachineCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMachineCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineCode.Location = new System.Drawing.Point(83, 138);
            this.lblMachineCode.Name = "lblMachineCode";
            this.lblMachineCode.Size = new System.Drawing.Size(160, 15);
            this.lblMachineCode.TabIndex = 24;
            this.lblMachineCode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(21, 138);
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
            this.lblPlannedQuantity.Location = new System.Drawing.Point(296, 34);
            this.lblPlannedQuantity.Name = "lblPlannedQuantity";
            this.lblPlannedQuantity.Size = new System.Drawing.Size(83, 26);
            this.lblPlannedQuantity.TabIndex = 22;
            this.lblPlannedQuantity.Text = "0";
            this.lblPlannedQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUoM
            // 
            this.lblUoM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUoM.Location = new System.Drawing.Point(378, 42);
            this.lblUoM.Name = "lblUoM";
            this.lblUoM.Size = new System.Drawing.Size(34, 13);
            this.lblUoM.TabIndex = 21;
            this.lblUoM.Text = "UoM";
            this.lblUoM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(311, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "META";
            // 
            // lblPlannedCompletionDate
            // 
            this.lblPlannedCompletionDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlannedCompletionDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedCompletionDate.Location = new System.Drawing.Point(269, 161);
            this.lblPlannedCompletionDate.Name = "lblPlannedCompletionDate";
            this.lblPlannedCompletionDate.Size = new System.Drawing.Size(187, 15);
            this.lblPlannedCompletionDate.TabIndex = 7;
            this.lblPlannedCompletionDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlannedStartDate
            // 
            this.lblPlannedStartDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlannedStartDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedStartDate.Location = new System.Drawing.Point(83, 162);
            this.lblPlannedStartDate.Name = "lblPlannedStartDate";
            this.lblPlannedStartDate.Size = new System.Drawing.Size(160, 15);
            this.lblPlannedStartDate.TabIndex = 6;
            this.lblPlannedStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(247, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Fin:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(38, 162);
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
            this.label1.Location = new System.Drawing.Point(45, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "OT: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbWorkOrders
            // 
            this.cmbWorkOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkOrders.Enabled = false;
            this.cmbWorkOrders.FormattingEnabled = true;
            this.cmbWorkOrders.Location = new System.Drawing.Point(83, 39);
            this.cmbWorkOrders.Name = "cmbWorkOrders";
            this.cmbWorkOrders.Size = new System.Drawing.Size(160, 21);
            this.cmbWorkOrders.TabIndex = 0;
            this.cmbWorkOrders.DropDown += new System.EventHandler(this.DropDownOpenWorkOrders);
            this.cmbWorkOrders.SelectedValueChanged += new System.EventHandler(this.SelectedIndexChangedWorkOrders);
            // 
            // picBoxWaitWO
            // 
            this.picBoxWaitWO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBoxWaitWO.Image = global::WOW_Fusion.Properties.Resources.preloader_01;
            this.picBoxWaitWO.Location = new System.Drawing.Point(233, 28);
            this.picBoxWaitWO.Name = "picBoxWaitWO";
            this.picBoxWaitWO.Padding = new System.Windows.Forms.Padding(10);
            this.picBoxWaitWO.Size = new System.Drawing.Size(45, 45);
            this.picBoxWaitWO.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxWaitWO.TabIndex = 119;
            this.picBoxWaitWO.TabStop = false;
            this.picBoxWaitWO.Visible = false;
            // 
            // cmbWorkCenters
            // 
            this.cmbWorkCenters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkCenters.FormattingEnabled = true;
            this.cmbWorkCenters.Location = new System.Drawing.Point(83, 34);
            this.cmbWorkCenters.Name = "cmbWorkCenters";
            this.cmbWorkCenters.Size = new System.Drawing.Size(187, 21);
            this.cmbWorkCenters.TabIndex = 27;
            this.cmbWorkCenters.DropDown += new System.EventHandler(this.DropDownOpenWorkCenters);
            this.cmbWorkCenters.SelectedValueChanged += new System.EventHandler(this.SelectedIndexChangedWorkCenters);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(53, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "CT:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // picBoxWaitWC
            // 
            this.picBoxWaitWC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBoxWaitWC.Image = global::WOW_Fusion.Properties.Resources.preloader_01;
            this.picBoxWaitWC.Location = new System.Drawing.Point(262, 19);
            this.picBoxWaitWC.Name = "picBoxWaitWC";
            this.picBoxWaitWC.Padding = new System.Windows.Forms.Padding(10);
            this.picBoxWaitWC.Size = new System.Drawing.Size(45, 45);
            this.picBoxWaitWC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxWaitWC.TabIndex = 120;
            this.picBoxWaitWC.TabStop = false;
            this.picBoxWaitWC.Visible = false;
            // 
            // btnReloadWO
            // 
            this.btnReloadWO.BackColor = System.Drawing.Color.Transparent;
            this.btnReloadWO.BackgroundImage = global::WOW_Fusion.Properties.Resources.upload_01;
            this.btnReloadWO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReloadWO.FlatAppearance.BorderSize = 0;
            this.btnReloadWO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReloadWO.Location = new System.Drawing.Point(519, 494);
            this.btnReloadWO.Name = "btnReloadWO";
            this.btnReloadWO.Size = new System.Drawing.Size(36, 34);
            this.btnReloadWO.TabIndex = 26;
            this.btnReloadWO.UseVisualStyleBackColor = false;
            this.btnReloadWO.Click += new System.EventHandler(this.btnReloadWO_Click);
            // 
            // btnPrintPallet
            // 
            this.btnPrintPallet.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnPrintPallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintPallet.Location = new System.Drawing.Point(393, 484);
            this.btnPrintPallet.Name = "btnPrintPallet";
            this.btnPrintPallet.Size = new System.Drawing.Size(120, 54);
            this.btnPrintPallet.TabIndex = 117;
            this.btnPrintPallet.Text = "REGISTRAR";
            this.btnPrintPallet.UseVisualStyleBackColor = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureLabel);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 239);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(485, 218);
            this.groupBox3.TabIndex = 118;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Etiqueta";
            // 
            // pictureLabel
            // 
            this.pictureLabel.Image = global::WOW_Fusion.Properties.Resources.label_icon;
            this.pictureLabel.Location = new System.Drawing.Point(13, 20);
            this.pictureLabel.Name = "pictureLabel";
            this.pictureLabel.Size = new System.Drawing.Size(443, 184);
            this.pictureLabel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureLabel.TabIndex = 31;
            this.pictureLabel.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblBusinessUnit);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cmbWorkCenters);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lblLocationCode);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.lblWorkAreaName);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.lblOrganizationName);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.picBoxWaitWC);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(519, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 212);
            this.groupBox2.TabIndex = 119;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ubicación";
            // 
            // lblBusinessUnit
            // 
            this.lblBusinessUnit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBusinessUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBusinessUnit.Location = new System.Drawing.Point(83, 150);
            this.lblBusinessUnit.Name = "lblBusinessUnit";
            this.lblBusinessUnit.Size = new System.Drawing.Size(187, 15);
            this.lblBusinessUnit.TabIndex = 121;
            this.lblBusinessUnit.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(48, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 120;
            this.label6.Text = "UN: ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLocationCode
            // 
            this.lblLocationCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLocationCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationCode.Location = new System.Drawing.Point(83, 127);
            this.lblLocationCode.Name = "lblLocationCode";
            this.lblLocationCode.Size = new System.Drawing.Size(187, 15);
            this.lblLocationCode.TabIndex = 24;
            this.lblLocationCode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(19, 127);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Ubicación:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWorkAreaName
            // 
            this.lblWorkAreaName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWorkAreaName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkAreaName.Location = new System.Drawing.Point(83, 74);
            this.lblWorkAreaName.Name = "lblWorkAreaName";
            this.lblWorkAreaName.Size = new System.Drawing.Size(187, 15);
            this.lblWorkAreaName.TabIndex = 16;
            this.lblWorkAreaName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(42, 75);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(35, 13);
            this.label19.TabIndex = 15;
            this.label19.Text = "Area: ";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOrganizationName
            // 
            this.lblOrganizationName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOrganizationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrganizationName.Location = new System.Drawing.Point(83, 101);
            this.lblOrganizationName.Name = "lblOrganizationName";
            this.lblOrganizationName.Size = new System.Drawing.Size(187, 15);
            this.lblOrganizationName.TabIndex = 6;
            this.lblOrganizationName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(2, 100);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(75, 13);
            this.label26.TabIndex = 4;
            this.label26.Text = "Organización: ";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmLabelP1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 653);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPrintPallet);
            this.Controls.Add(this.btnReloadWO);
            this.Name = "frmLabelP1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmLabelP1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxWaitWC)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureLabel)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblItemDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblItemNumber;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btnReloadWO;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblMachineName;
        private System.Windows.Forms.Label lblMachineCode;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblPlannedQuantity;
        private System.Windows.Forms.Label lblUoM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblPlannedCompletionDate;
        private System.Windows.Forms.Label lblPlannedStartDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWorkOrders;
        private System.Windows.Forms.Button btnPrintPallet;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox pictureLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbWorkCenters;
        private System.Windows.Forms.PictureBox picBoxWaitWO;
        private System.Windows.Forms.PictureBox picBoxWaitWC;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblLocationCode;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblWorkAreaName;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblOrganizationName;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lblBusinessUnit;
        private System.Windows.Forms.Label label6;
    }
}

