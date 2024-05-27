using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOW_Fusion.Models;

namespace WOW_Fusion.Services
{
    internal class ApexService
    {
        public static async void CreatePallet(string organization, string palletId, int palletNumber, string wo, string item, float tare, float weight, string shift, int itemsOnPallet)
        {
            dynamic jsonPallet = JObject.Parse(Payloads.weightPallet);

            jsonPallet.DateMark = palletId;
            jsonPallet.OrganizationId = organization;
            jsonPallet.WorkOrderNumber = wo;
            jsonPallet.ItemNumber = item;
            jsonPallet.PalletNumber = palletNumber;
            jsonPallet.Content = itemsOnPallet;
            jsonPallet.Tare = tare;
            jsonPallet.Weight = weight;
            jsonPallet.Shift = shift;

            string jsonSerialized = JsonConvert.SerializeObject(jsonPallet, Formatting.Indented);

            Task<string> postWeightPallet = APIService.PostApexAsync(String.Format(EndPoints.WeightPallets, "WO", "RS", organization), jsonSerialized);
            string response = await postWeightPallet;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar palet [{DateService.Today()}]", Color.Red);
            }
        }

        public static async void UpdatePallet(int pallet, float tare, float weight, string wo)
        {
            dynamic jsonPallet = JObject.Parse(Payloads.weightPallet);

            jsonPallet.Tare = tare;
            jsonPallet.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonPallet, Formatting.Indented);

            Task<string> putWeightPallet = APIService.PutApexAsync(String.Format(EndPoints.WeightPallets, wo, pallet, Constants.Plant3Id), jsonSerialized);
            string response = await putWeightPallet;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar palet [{DateService.Today()}]", Color.Red);
            }
        }

        public static async void CreateWeightItem(int roll, float weight, string palletId, string org)
        {
            dynamic jsonRoll = JObject.Parse(Payloads.weightRoll);
            string rollId = DateService.EpochTime();
            jsonRoll.DateMark = rollId;
            jsonRoll.PalletId = palletId;
            jsonRoll.RSNumber = roll;
            jsonRoll.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);

            Task<string> postWeightRoll = APIService.PostApexAsync(String.Format(EndPoints.WeightRolls, "WO", "RS", org), jsonSerialized);
            string response = await postWeightRoll;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar peso [{DateService.Today()}]", Color.Red);
            }
        }

        public static async void UpdateWeightItem(int pallet, float roll, float weight, string wo, string org)
        {
            dynamic jsonRoll = JObject.Parse(Payloads.weightRollUpdate);

            jsonRoll.Pallet = pallet;
            jsonRoll.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);

            Task<string> putWeightRoll = APIService.PutApexAsync(String.Format(EndPoints.WeightRolls, wo, roll, org), jsonSerialized);
            string response = await putWeightRoll;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar peso [{DateService.Today()}]", Color.Red);
            }
        }
    }
}
