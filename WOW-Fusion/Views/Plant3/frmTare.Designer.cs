namespace WOW_Fusion.Views.Plant3
{
    partial class frmTare
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
            this.btnWeight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReloadTare = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.54562F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.477547F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.97683F));
            this.tableLayoutPanel1.Controls.Add(this.btnReloadTare, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnWeight, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(450, 38);
            this.tableLayoutPanel1.TabIndex = 182;
            // 
            // btnWeight
            // 
            this.btnWeight.BackColor = System.Drawing.Color.DarkOrange;
            this.btnWeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnWeight.Enabled = false;
            this.btnWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWeight.Location = new System.Drawing.Point(348, 3);
            this.btnWeight.Name = "btnWeight";
            this.btnWeight.Size = new System.Drawing.Size(99, 32);
            this.btnWeight.TabIndex = 75;
            this.btnWeight.Text = "PESAR";
            this.btnWeight.UseVisualStyleBackColor = false;
            this.btnWeight.Click += new System.EventHandler(this.btnWeight_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 38);
            this.label1.TabIndex = 76;
            this.label1.Text = "0.0";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReloadTare
            // 
            this.btnReloadTare.BackColor = System.Drawing.Color.Transparent;
            this.btnReloadTare.BackgroundImage = global::WOW_Fusion.Properties.Resources.zero;
            this.btnReloadTare.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnReloadTare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReloadTare.FlatAppearance.BorderSize = 0;
            this.btnReloadTare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReloadTare.Location = new System.Drawing.Point(315, 3);
            this.btnReloadTare.Name = "btnReloadTare";
            this.btnReloadTare.Size = new System.Drawing.Size(27, 32);
            this.btnReloadTare.TabIndex = 143;
            this.btnReloadTare.UseVisualStyleBackColor = false;
            this.btnReloadTare.Visible = false;
            // 
            // frmTare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 96);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmTare";
            this.Text = "frmTare";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnWeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReloadTare;
    }
}