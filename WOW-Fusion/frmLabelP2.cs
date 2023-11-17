﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        Random rnd = new Random();

        APIService api;
        LabelService label;

        RadwagController weighing;
        PopController pop;

        //Fusion parametros
        public static string pylOrganization = string.Empty;
        public static string organizationId = "300000002650049";

        //Datagrid parametros
        private int _rollNumber = 0;

        //Pesos params
        private float _tareWeight = 0;
        private float _palletWeight = 0;


        public frmLabelP2()
        {
            InitializeComponent();
        }

        private void frmLabelP2_Load(object sender, EventArgs e)
        {
            //lblVersion.Text = "v " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            api = new APIService();
            weighing = new RadwagController();
            pop = new PopController();

            btnGetWeight.Text = "TARA";

            RequestOrganizationData();
            RequestWorkOrdersList();

            pictureLabel.Image = Image.FromStream(label.Create());
        }

        private async void RequestOrganizationData()
        {
            try
            {
                Task<string> tskOrganizationData = api.GetRequestAsync("/inventoryOrganizations?fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode&onlyData=true" +
                                                                     "&limit=500&totalResults=true&q=OrganizationId=" + organizationId);
                string response = await tskOrganizationData;
                if (!string.IsNullOrEmpty(response))
                {
                    pylOrganization = response;
                    var doOrganizationData = JsonConvert.DeserializeObject<dynamic>(response);
                    lblLocationCode.Text = (string)doOrganizationData["items"][0]["LocationCode"];
                }
                else
                {
                    pop.Notifier("Sin respuesta", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestWorkOrdersList()
        {
            try
            {
                pop.Show(this);
                Task<string> tskWorkOrdersList = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&fields=WorkOrderNumber&" +
                                                                    "q=OrganizationId=" + organizationId + " and WorkOrderStatusCode='ORA_RELEASED'");
                string response = await tskWorkOrdersList;
                pop.Close();
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderList = JsonConvert.DeserializeObject<dynamic>(response);

                    int itemCount = (int)doWorkOrderList["count"];

                    if (itemCount >= 1)
                    {
                        cmbWorkOrders.Items.Clear();

                        for (int i = 0; i < itemCount; i++)
                        {
                            cmbWorkOrders.Items.Add((string)doWorkOrderList["items"][i]["WorkOrderNumber"]);
                        }
                    }
                    else
                    {
                        pop.Notifier("Sin ordenes de trabajo", null);
                    }
                }
                else
                {
                    pop.Notifier("Sin respuesta", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            string selectedWO = cmbWorkOrders.SelectedItem.ToString();
            try
            {
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&expand=WorkOrderOperation&" +
                                                                    "fields=WorkOrderId,WorkOrderNumber,WorkOrderStatusCode,ItemNumber,Description,UOMCode;" +
                                                                    "WorkOrderOperation:OperationSequenceNumber,OperationName,PlannedStartDate," +
                                                                    "PlannedCompletionDate,ReadyQuantity,CompletedQuantity&" +
                                                                    "q=OrganizationId=" + organizationId + ";WorkOrderStatusCode=ORA_RELEASED;WorkOrderNumber=" + selectedWO);
                string response = await tskWorkOrdersData;
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderData = JsonConvert.DeserializeObject<dynamic>(response);

                    lblPlannedQuantity.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["ReadyQuantity"];
                    lblCompletedQuantity.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["CompletedQuantity"];

                    lblOperation.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["OperationName"];

                    lblPlannedStartDate.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedStartDate"];
                    lblPlannedCompletionDate.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedCompletionDate"];

                    //lblMachineCode.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedStartDate"];
                    //lblMachineName.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];

                    lblItemNumber.Text = (string)doWorkOrderData["items"][0]["ItemNumber"];
                    lblItemDescription.Text = (string)doWorkOrderData["items"][0]["Description"];
                    lblUoM.Text = (string)doWorkOrderData["items"][0]["UOMCode"];
                }
                else
                {
                    pop.Notifier("No se encontraron datos", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            txtBoxWeight.Text = "";
            pop.Show(this);
            if (string.IsNullOrEmpty(lblPalletTare.Text))
            {
                //Solicitar peso tara
                string responseTare = weighing.SocketWeighing("T");
                if (responseTare.Equals("OK"))
                {
                    string requestTareWeight = weighing.SocketWeighing("OT");
                    if (!requestTareWeight.Equals("EX"))
                    {
                        pop.Close();
                        _tareWeight = float.Parse(requestTareWeight);
                        lblPalletTare.Text = requestTareWeight;
                        txtBoxWeight.Text = requestTareWeight;
                        btnGetWeight.Text = "OBTENER";
                        _rollNumber = 0;
                    }
                    else
                    {
                        pop.Close();
                        pop.Notifier("Tiempo de espera agotado, vuelva a  intentar", null);
                    }
                }
                else
                {
                    pop.Close();
                    MessageBox.Show(responseTare, "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                //Obtiene peso neto acomulado del pallet (sin tara)
                string palletNetWeight = weighing.SocketWeighing("S");

                if (palletNetWeight == "EX")
                {
                    pop.Close();
                    pop.Notifier("Tiempo de espera agotado, vuelva a  intentar", null);
                }
                else
                {
                    pop.Close();
                    _rollNumber++;

                    //Calcular pero neto de cada rollo (sin tara)
                    float rollNetKg = float.Parse(palletNetWeight) - _palletWeight;
                    txtBoxWeight.Text = rollNetKg.ToString();

                    //Calcular pero bruto de cada rollo (con tara)
                    float rollGrossKg = rollNetKg + _tareWeight;

                    //Agregar a datagrid (Rollo, Neto, Bruto)
                    string[] row = new string[] { _rollNumber.ToString(), rollNetKg.ToString(), rollGrossKg.ToString() };
                    dgWeights.Rows.Add(row);

                    _palletWeight = float.Parse(palletNetWeight);
                    lblPalletNet.Text = palletNetWeight;
                    lblPalletGross.Text = (_palletWeight + _tareWeight).ToString();

                    if (dgWeights.RowCount == 1)
                    {
                        DataGridViewButtonColumn dgViewButtonPrint = new DataGridViewButtonColumn();
                        {
                            dgViewButtonPrint.HeaderText = "Acción";
                            dgViewButtonPrint.Name = "btnPrintLabel";
                            dgViewButtonPrint.FlatStyle = FlatStyle.Flat;
                            dgViewButtonPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dgViewButtonPrint.UseColumnTextForButtonValue = true;
                        }

                        dgWeights.Columns.Add(dgViewButtonPrint);
                    }
                }


                /*int palletWeight = dgWeights.Rows.Cast<DataGridViewRow>().Sum(t => Convert.ToInt32(t.Cells[1].Value)) + 
                                   int.Parse(lblTareWeight.Text.Remove(lblTareWeight.Text.Length - 3, 3));
                lblPalletWeight.Text = palletWeight.ToString() + " KG";*/
            }
        }
       
        private void dgWeight_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //Columna a colocar icono
            if (e.ColumnIndex == 3)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Resources.printer_01.Width / 20;
                var h = Resources.printer_01.Height / 20;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Resources.printer_01, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void btnReloadWO_Click(object sender, EventArgs e)
        {
            RequestWorkOrdersList();
        }

        private void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                DataGridViewRow row = dgWeights.Rows[e.RowIndex];
                string data = row.Cells[0].Value.ToString();
                MessageBox.Show(data);
                //MessageBox.Show(e.RowIndex.ToString() + "," + e.ColumnIndex.ToString());
            }
        }

        private void btnPrintPallet_Click(object sender, EventArgs e)
        {

        }
    }
}