using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WOW_Fusion.Views.Plant1
{
    public partial class frmReprint : Form
    {
        public frmReprint()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBoxStart.Text) && !string.IsNullOrEmpty(txtBoxEnd.Text))
            {
                if (int.TryParse(txtBoxStart.Text, out _) && int.TryParse(txtBoxEnd.Text, out _))
                {
                    if (int.Parse(txtBoxStart.Text) > 0 && int.Parse(txtBoxEnd.Text) <= frmLabelP1.totalQuantity)
                    {
                        if (int.Parse(txtBoxEnd.Text) >= int.Parse(txtBoxStart.Text))
                        {
                            frmLabelP1.startPage = int.Parse(txtBoxStart.Text);
                            frmLabelP1.endPage = int.Parse(txtBoxEnd.Text);
                            NotifierController.Success("Presione imprimir");
                            Close();
                        }
                        else
                        {
                            lblStatus.Text = "Cantidad final no puede ser menor a la inicial";
                        }
                    }
                    else
                    {
                        lblStatus.Text = "Cantidades fuera del rango permitido";
                    }
                }
                else
                {
                    lblStatus.Text = "Ingrese únicamente números enteros";
                }
            }
            else
            {
                lblStatus.Text = "Llene todos los campos";
            }
        }

        private void txtBoxStart_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxStart.Text, out _))
            {
                txtBoxStart.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBoxStart.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        private void txtBoxEnd_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxEnd.Text, out _))
            {
                txtBoxEnd.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBoxEnd.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }

        }

        private void cmbWorkOrders_DropDown(object sender, EventArgs e)
        {
            List<string> ordersPrinted = FileController.ContentFile(Constants.PathPrintedLables);
            if (ordersPrinted.Count > 0)
            {
                cmbWorkOrders.Items.Clear();    
                foreach (string order in ordersPrinted)
                {
                    cmbWorkOrders.Items.Add(order);
                }
            }
        }

        private void cmbWorkOrders_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
