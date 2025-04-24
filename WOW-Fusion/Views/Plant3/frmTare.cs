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
using WOW_Fusion.Properties;

namespace WOW_Fusion.Views.Plant3
{
    public partial class frmTare : Form
    {
        public frmTare()
        {
            InitializeComponent();
        }

        private void btnCloseFrm_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void frmTare_Load(object sender, EventArgs e)
        {
            ShowWait(false);
            lblStatusProcess.Text = "¡Coloque y pese TARIMA!";
            //♥ Consultar template etiqueta en APEX  ♥
            dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, "AEROPUERTOPL3", "NULL");
            if (labelApex.LabelName.ToString().Equals("null"))
            {
                btnGetWeight.Enabled = false;
                btnGetWeight.BackColor = Color.Gray;
                MessageBox.Show("Etiqueta de cliente/producto no encontrada", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowWait(bool show, string message = "")
        {
            lblStatusProcess.Text = message;
            pbWaitProcess.Visible = show;
        }

        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;

            btnGetWeight.Enabled = false;
            btnGetWeight.BackColor = Color.Gray;
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
                        if (string.IsNullOrEmpty(lblTare.Text))
                        {
                            if (_weightFromWeighing >= Settings.Default.TareMinWeight && _weightFromWeighing <= Settings.Default.TareMaxWeight)
                            {
                                btnReset.Visible = false;
                                lblTare.Text = _weightFromWeighing.ToString("F1");
                                lblBag.Text = string.Empty;
                                lblStatusProcess.Text = "¡Coloque y pese SACO!";
                            }
                            else
                            {
                                if (_weightFromWeighing > Settings.Default.TareMaxWeight)
                                {
                                    MessageBox.Show($"Peso [{_weightFromWeighing.ToString("F1")} kg] por encima del estándar de una TARA, verifique.", "¡No esta pesando una TARA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else if (_weightFromWeighing < Settings.Default.TareMinWeight)
                                {
                                    MessageBox.Show($"Peso [{_weightFromWeighing.ToString("F1")} kg] debajo del estándar de una TARA, verifique.", "¡No esta pesando una TARA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    MessageBox.Show($"Peso de tara invalido [{_weightFromWeighing.ToString("F1")}], verifique.", "¡Precaución!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(lblTare.Text) && string.IsNullOrEmpty(lblBag.Text))
                        {
                            if (_weightFromWeighing <= Settings.Default.BagMaxWeight)
                            {
                                btnReset.Visible = true;
                                lblBag.Text = _weightFromWeighing.ToString("F1");
                                FillLabelWeight();
                                await LabelService.PrintP3(0, "WEIGHING", string.Empty);
                            }
                            else
                            {
                                MessageBox.Show($"Peso [{_weightFromWeighing.ToString("F1")} kg] por encima del estándar de un SACO, verifique.", "¡No esta pesando un SACO!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            
                        }
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
            if (lblTare.Text != string.Empty && lblBag.Text != string.Empty)
            {
                btnGetWeight.Enabled = false;
                btnGetWeight.BackColor = Color.Gray;
            }
            else
            {
                btnGetWeight.Enabled = true;
                btnGetWeight.BackColor = Color.LimeGreen;
            }
        }

        private void FillLabelWeight()
        {
            dynamic label = JObject.Parse(Constants.LabelJson);

            JObject jObj = (JObject)label;
            foreach (JProperty property in jObj.Properties()) { property.Value = null; }

            label.DATE = DateService.Now();
            label.WTAREKG = string.IsNullOrEmpty(lblTare.Text) ? " " : lblTare.Text;
            label.WSACKKG = string.IsNullOrEmpty(lblBag.Text) ? " " : lblBag.Text;
            label.WTARESACKKG = (!string.IsNullOrEmpty(lblTare.Text) && !string.IsNullOrEmpty(lblBag.Text)) ? (float.Parse(lblTare.Text) + float.Parse(lblBag.Text)).ToString("F1") : " " ;

            Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lblBag.Text = string.Empty;
            lblTare.Text = string.Empty;
            btnGetWeight.Enabled = true;
            btnGetWeight.BackColor = Color.LimeGreen;
            lblStatusProcess.Text = "¡Coloque y pese TARIMA!";
        }

        public event EventHandler FormClosedEvent;
        private void frmTare_FormClosed(object sender, FormClosedEventArgs e)
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
