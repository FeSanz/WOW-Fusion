﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WOW_Fusion
{
    internal class APIService
    {
        private WebRequest request;
        private WebResponse response;
        private StreamReader reader;
        private StreamWriter writer;
        private Stream stream;

        private string url = "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";
        private string _user = "felipe.antonio@i-condor.com";
        private string _password = "CondorXR112";

        private string strResponse = "";

        public async Task<string> GetRequestAsync(string path)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/json";
                request.Headers.Add("REST-framework-version", "4");
                response = await request.GetResponseAsync();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);
                return await reader.ReadToEndAsync(); ;
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, $"Error GET [" + path.Split('/', '?')[1].ToLower() + "]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public async Task<string> PostRequestAsync(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/json";
                request.Method = "POST";

                writer = new StreamWriter(request.GetRequestStream());
                writer.Write(json);
                writer.Flush();
                writer.Close();

                response = await request.GetResponseAsync();
                reader = new StreamReader(response.GetResponseStream());
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public async Task<string> PostBatchRequestAsync(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/vnd.oracle.adf.batch+json";
                request.Method = "POST";

                writer = new StreamWriter(request.GetRequestStream());
                writer.Write(json);
                writer.Flush();
                writer.Close();

                response = await request.GetResponseAsync();
                reader = new StreamReader(response.GetResponseStream());
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string GetRequest(string path)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/json";
                request.Headers.Add("REST-framework-version", "4");
                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error GET. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string PostRequest(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/json";
                request.Method = "POST";

                writer = new StreamWriter(request.GetRequestStream());
                writer.Write(json);
                writer.Flush();
                writer.Close();

                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}