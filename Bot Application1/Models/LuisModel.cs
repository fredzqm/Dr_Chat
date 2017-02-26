using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Bot_Application1
{
    public class LUISClient
    {
        public static async Task<Luis> ParseUserInput(string strInput)
        {
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                // TODO: put URI in config file
                // TODO: insert your LUIS URL here
                string luisURL = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/6720b52f-ea53-416a-8efb-b80e4ad1b8f0?subscription-key=476c0b3d0fae4edca23e5657e6202e46";
                string uri = luisURL + "&q=" + strEscaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<Luis>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
    }

    public class Luis
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
    }

}