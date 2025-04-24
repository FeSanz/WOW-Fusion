using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
using WOW_Fusion.Services;

namespace WOW_Fusion.Views.Plant3
{
    public partial class frmWeighingP3 : Form
    {
        public frmWeighingP3()
        {
            InitializeComponent();
        }

        private async void frmWeighingP3_Load(object sender, EventArgs e)
        {
            ShowWait(false);

            //♥ Consultar template etiqueta en APEX  ♥
            dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, "ANYPL3", "NULL");
            if (labelApex.LabelName.ToString().Equals("null"))
            {
                btnGetWeight.Enabled = false;
                txtItemDescription.Enabled = false;
                MessageBox.Show("Etiqueta de cliente/producto no encontrada", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            if(txtItemDescription.Text.Length > 20)
            {
                lblStatus.Text = $"Máximo 20 carácteres en la descripción, escribió {txtItemDescription.Text.Length} carácteres incluyendo espacios";
                return;
            }

            lblStatus.Text = string.Empty;

            btnGetWeight.Enabled = false;
            ShowWait(true, "Obteniendo peso ...");

            string responseWeighing = await RadwagController.SocketWeighing("S");

            if (responseWeighing == "EX")
            {
                ShowWait(false);
                NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
            }
            else
            {
                if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                {
                    ShowWait(false);
                    if (_weightFromWeighing > 0)
                    {
                        lblWeight.Text = _weightFromWeighing.ToString("F1");
                        FillLabelWeight();
                        await LabelService.PrintP3(0, "WEIGHING", string.Empty);
                        txtItemDescription.Clear();
                    }
                    else
                    {
                        MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F1")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    ShowWait(false);
                    NotifierController.Warning($"Peso invalido. {responseWeighing}");
                }
            }
            btnGetWeight.Enabled = true;
        }

        private void btnCloseWeighing_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShowWait(bool show, string message = "")
        {
            lblStatusProcess.Text = message;
            pbWaitProcess.Visible = show;
        }

        #region Labels Fill
        private void FillLabelWeight()
        {
            dynamic label = JObject.Parse(Constants.LabelJson);

            label.ITEMDESCRIPTION = string.IsNullOrEmpty(txtItemDescription.Text) ? " " : txtItemDescription.Text;
            label.DATE = DateService.Now();
            label.WEIGHTKG= string.IsNullOrEmpty(lblWeight.Text) ? " " : lblWeight.Text;

            Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
        }
        #endregion

        public event EventHandler FormClosedEvent;
        private void frmWeighingP3_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClosedEvent?.Invoke(this, EventArgs.Empty);

            frmLabelP3 frmLabelP3 = Application.OpenForms.OfType<frmLabelP3>().FirstOrDefault();
            if (frmLabelP3 != null)
            {
                frmLabelP3.TemplateLabel();
            }
        }
    }
}
