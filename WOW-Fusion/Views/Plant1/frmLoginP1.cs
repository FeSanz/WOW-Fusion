using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion.Views.Plant1
{
    public partial class frmLoginP1 : Form
    {
        PopController pop;
        public frmLoginP1()
        {
            InitializeComponent();
        }

        private void frmLoginP1_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            btnLogin.Enabled = false;
            txtUser.Clear();
            txtPassword.Clear();

            //txtUser.Text = "PRINT.TEST";
            //txtPassword.Text = "GWow2024";    
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                pop.Show(this);
                Constants.pop = "Verificando...";
                if (await Authenticated())
                {
                    pop.Close();
                    lblStatus.Text = string.Empty;

                    
                    txtUser.Text = string.Empty;
                    txtPassword.Text = string.Empty;

                    frmLabelP1 labelP1 = new frmLabelP1();
                    labelP1.StartPosition = FormStartPosition.CenterScreen;
                    labelP1.Show();
                    Hide();
                }
                else
                {
                    pop.Close();
                }
            }
            else
            {
                lblStatus.Text = "Llene todos los campos";
            }
        }

        private async Task<bool> Authenticated()
        {
            string credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(txtUser.Text + ":" + txtPassword.Text));
            Task<string> tskAuth = APIService.GetApexAsync(String.Format(EndPoints.Auth, credentials));
            string response = await tskAuth;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                if (string.IsNullOrEmpty(responsePayload.UserId.ToString()) || responsePayload.UserId.ToString() == "null")
                {
                    lblStatus.Text = "Usuario no encontrado";
                    return false;
                }
                else
                {
                    int id = (int)responsePayload.UserId;
                    if(id > 0 )
                    {
                        Constants.UserId = (int)responsePayload.UserId;
                        Constants.UserName = txtUser.Text;
                        lblStatus.Text = "Acceso autorizado";
                        return true;
                    }
                    else
                    {
                        lblStatus.Text = responsePayload.Message;
                        return false;
                    }
                }
            }
            else
            {
                lblStatus.Text = "Sin respuesta";
                return false;
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = string.IsNullOrEmpty(txtPassword.Text) ? false : true;
        }

        private void frmLoginP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }
    }
}
