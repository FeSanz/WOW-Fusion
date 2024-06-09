using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion.Views.Plant2
{
    public partial class frmLogin : Form
    {
        PopController pop;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            btnLogin.Enabled = false;
            txtUser.Clear();
            txtPassword.Clear();
        }

        public event EventHandler FormClosedEvent;
        private void lblExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = string.IsNullOrEmpty(txtPassword.Text) ? false : true;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                pop.Show(this);
                Constants.pop = "Verificando...";
                string credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(txtUser.Text + ":" + txtPassword.Text));
                if (await Authenticated(Constants.Plant2Id, credentials))
                {
                    pop.Close();
                    lblStatus.Text = string.Empty;

                    Hide();
                    frmSettingsP2 frmSettingsP2 = new frmSettingsP2();
                    frmSettingsP2.StartPosition = FormStartPosition.CenterParent;
                    if (frmSettingsP2.ShowDialog() == DialogResult.OK) { Close(); }
                }
                else
                {
                    pop.Close();
                    lblStatus.Text  = "Acceso no autorizado";
                }
            }
            else
            {
                lblStatus.Text = "Llene todos los campos";
            }
        }

        private static async Task<bool> Authenticated(string plantId, string credentials)
        {
            Task<string> tskItem = APIService.GetAuthAsync(String.Format(EndPoints.InventoryOrganizations, plantId), credentials);
            string response = await tskItem;
            if (string.IsNullOrEmpty(response))
            {
                NotifierController.Error(Constants.Exception);
                return false;
            }
            else
            {
                JObject objResponse = JObject.Parse(response);

                if ((int)objResponse["count"] == 0)
                {
                    NotifierController.Error("Sin los permisos necesarios");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }
    }
}
