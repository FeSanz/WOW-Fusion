using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal class Batchs
    {
        public static string BatchPayload(List<string> paths)
        {
            Part[] part = new Part[paths.Count];
            for (int i = 0; i < paths.Count; i++)
            {
                part[i] = new Part
                {
                    id = $"{i + 1}",
                    path = paths[i],
                    operation = "get",
                };
            }

            BatchParts batch = new BatchParts { parts = part };

            return JsonConvert.SerializeObject(batch, Formatting.Indented);
        }
    }

    class BatchParts
    {
        public Part[] parts { get; set; }
    }
    public class Part
    {
        public string id { get; set; }
        public string path { get; set; }
        public string operation { get; set; }
    }
}
