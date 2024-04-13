﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Properties;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WOW_Fusion
{
    internal class APIService
    {  
        //*********************************** Servicios para FUSION ***********************************
        public static async Task<string> GetRequestAsync(string path)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Headers.Add("Authorization", "Basic " + Settings.Default.Credentials);
                request.ContentType = "application/json";
                request.Headers.Add("REST-framework-version", "4");
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Error de consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }



        public async Task<string> PostRequestAsync(string path, string json)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Headers.Add("Authorization", "Basic " + Settings.Default.Credentials);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    writer.Write(json);
                    await writer.FlushAsync();
                }

                using (WebResponse response = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en envío de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<string> PostBatchRequestAsync(string json)
        {
            try
            {
                WebRequest request = WebRequest.Create(Settings.Default.FusionUrl);
                request.Headers.Add("Authorization", "Basic " + Settings.Default.Credentials);
                request.ContentType = "application/vnd.oracle.adf.batch+json";
                request.Headers.Add("REST-framework-version", "4");
                request.Method = "POST";

                using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    writer.Write(json);
                    await writer.FlushAsync();
                }

                using (WebResponse response = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[BATCH] Error en el servicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string GetRequest(string path)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Headers.Add("Authorization", "Basic " + Settings.Default.Credentials);
                request.ContentType = "application/json";
                request.Headers.Add("REST-framework-version", "4");
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Error de consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string PostRequest(string path, string json)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Headers.Add("Authorization", "Basic " + Settings.Default.Credentials);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                    writer.Flush();
                }

                using (WebResponse response = request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Error en envío de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //*********************************** Servicios para APEX ***********************************
        public static async Task<string> GetApexAsync(string path)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.ContentType = "application/json";
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "[APEX] Error de consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<string> PostApexAsync(string path, string json)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    writer.Write(json);
                    await writer.FlushAsync();
                }

                using (WebResponse response = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[APEX] Error en envío de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<string> PutApexAsync(string path, string json)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.ContentType = "application/json";
                request.Method = "PUT";

                using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    writer.Write(json);
                    await writer.FlushAsync();
                }

                using (WebResponse response = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[APEX] Error en actualización de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
