using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Translation.V2;
using System.Net.Http;
using System.Collections;
//using System.Web.Script.Serialization;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;
using System.Windows.Forms;

namespace WOW_Fusion.Services
{
    internal class TranslateService
    {
        public static string Translate(string input)
        {
            try
            {
                // Api de Google para traducción, indicar lenguajes
                string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", "es", "en", Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;

                // Deserializar respuesta
                var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(result);

                // Obntener primer elemento (Dato de valor)
                var translationItems = jsonData[0];

                string translation = "";

                // Extraer coleccion de datos del objeto de respuesta
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }

                // Remover primer caracter en blanco
                if (translation.Length > 1) { translation = translation.Substring(1); };

                return translation;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [Translate]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }
    }
}
