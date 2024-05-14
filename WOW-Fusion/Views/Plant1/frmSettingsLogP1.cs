using Newtonsoft.Json.Linq;
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
using WOW_Fusion.Models;
using WOW_Fusion.Properties;

namespace WOW_Fusion.Views.Plant1
{
    public partial class frmSettingsLogP1 : Form
    {
        PopController pop;
        public frmSettingsLogP1()
        {
            InitializeComponent();
        }

        private void frmSettingsLogP1_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            btnLogin.Enabled = false;
            txtUser.Clear();
            txtPassword.Clear();
        }

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
                if (await Authenticated(Constants.Plant1Id, credentials))
                {
                    pop.Close();
                    lblStatus.Text = string.Empty;
                    Settings.Default.Credentials = credentials;
                    Settings.Default.Save();

                    Hide();
                    Close();

                    frmSettingsP1 frmSettingsP1 = new frmSettingsP1();
                    frmSettingsP1.StartPosition = FormStartPosition.CenterParent;
                    frmSettingsP1.ShowDialog();
                }
                else
                {
                    pop.Close();
                    lblStatus.Text = "Acceso no autorizado";
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
    }
}
