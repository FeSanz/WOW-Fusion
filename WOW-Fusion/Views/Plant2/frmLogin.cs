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
using WOW_Fusion.Properties;

namespace WOW_Fusion.Views.Plant2
{
    public partial class frmLogin : Form
    {
        private bool authenticated = true;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            txtUser.Clear();
            txtPassword.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                if(authenticated)
                {
                    //Settings.Default.Credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(txtUser.Text + ":" + txtPassword.Text));
                    //Settings.Default.Save();

                    frmSettingsP2 frmSettingsP2 = new frmSettingsP2();
                    frmSettingsP2.StartPosition = FormStartPosition.CenterParent;
                    frmSettingsP2.ShowDialog();

                    Close();
                }
            }
            else
            {
                NotifierController.Warning("Llene todos los campos");
            }
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = string.IsNullOrEmpty(txtPassword.Text) ? false : true;
        }
    }
}
