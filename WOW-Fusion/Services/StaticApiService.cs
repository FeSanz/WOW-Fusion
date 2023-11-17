﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion
{
    internal class StaticApiService
    {
        private static string url = Constants.FusionUrl;
        private static string _user = Constants.AuthenticationUser;
        private static string _password = Constants.AuthenticationPassword;

        public static async Task<string> GetRequestAsync(string path)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                WebRequest request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
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
                MessageBox.Show(ex.Message, $"Error GET [" + path.Split('/', '?')[1].ToLower() + "]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<string> PostRequestAsync(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                WebRequest request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
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
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<string> PostBatchRequestAsync(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                WebRequest request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
                request.ContentType = "application/vnd.oracle.adf.batch+json";
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
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string GetRequest(string path)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                WebRequest request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
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
                MessageBox.Show("Error GET. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string PostRequest(string path, string json)
        {
            string credential = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_user + ":" + _password));
            try
            {
                WebRequest request = WebRequest.Create(url + path);
                request.Headers.Add("Authorization", "Basic " + credential);
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
                MessageBox.Show("Error POST. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}

