using Google.Apis.Translate.v2;
using Newtonsoft.Json;
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
                ExceptionWebService(ex, 1, "Error de consulta");
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
            catch (WebException ex)
            {
                ExceptionWebService(ex, 1, "Error en envío de datos");
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
            catch (WebException ex)
            {
                ExceptionWebService(ex, 1, "[BATCH] Error en el servicio");
                return null;
            }
        }

        public static async Task<string> GetAuthAsync(string path, string credentials)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Headers.Add("Authorization", "Basic " + credentials);
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
                ExceptionWebService(ex, 2, "Auth");
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
                ExceptionWebService(ex, 1, "[APEX] Error de consulta");
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
            catch (WebException ex)
            {
                ExceptionWebService(ex, 1, "[APEX] Error en envío de datos");
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
            catch (WebException ex)
            {
                ExceptionWebService(ex, 1, "[APEX] Error en actualización de datos");
                return null;
            }
        }

        private static void ExceptionWebService(WebException ex, int mType, string title)
        {
            PopController pop = new PopController();
            pop.Close();

            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: //200
                        Constants.Exception = "OK";
                        break;
                    case HttpStatusCode.Created: //201
                        Constants.Exception = "Creado";
                        break;
                    case HttpStatusCode.Accepted: // 202
                        Constants.Exception = "Aceptado";
                        break;
                    case HttpStatusCode.BadRequest: //400
                        Constants.Exception = "Solicitud incorrecta, falta información obligatoria o no válida";
                        break;
                    case HttpStatusCode.Unauthorized: //401
                        Constants.Exception = "No autorizado";
                        break;
                    case HttpStatusCode.Forbidden: //403
                        Constants.Exception = "No permitido, sin permisos para realizar la solicitud";
                        break;
                    case HttpStatusCode.NotFound: //404
                        Constants.Exception = "No encontrado";
                        break;
                    case HttpStatusCode.MethodNotAllowed: //405
                        Constants.Exception = "Método de la solicitud no permitido";
                        break;
                    case HttpStatusCode.InternalServerError: //500
                        Constants.Exception = "Error interno del servidor";
                        break;
                    default:
                        Constants.Exception = $"{(int)response.StatusCode}. {response.StatusCode}";
                        break;
                }
                
            }
            else
            {
                Constants.Exception = $"{ex.Message}";
            }

            if (mType == 1)
            {
                MessageBox.Show(Constants.Exception, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
