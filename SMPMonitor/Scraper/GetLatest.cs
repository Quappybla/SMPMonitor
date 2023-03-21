using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMPMonitor.Scraper
{
    internal static class GetLatest
    {
        public static SMPReport GetLatestFromApi()
        {
            SMPReport? smp = new SMPReport();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.aeso.ca/report/v1.1/price/systemMarginalPrice/current");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-API-Key", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI5YWZscGYiLCJpYXQiOjE2Nzg5Mjc1Mjh9.XtrcwkBWAUC2qLMtjx-qMdhitAKzFqyDfZ2gLvclES0");

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            string json = response.Content.ReadAsStringAsync().Result;

            JObject obj = JObject.Parse(json);
            var item = obj.SelectToken("return");
            var report = obj.SelectToken("return['System Marginal Price Report']");
            //dynamic obj = JsonConvert.DeserializeObject(json);
            //var item = obj.return;
            //smp = JsonSerializer.Deserialize<SMPReport>(json);

            smp = JsonConvert.DeserializeObject<SMPReport>(report[0].ToString());
            return smp;
        }
    }
}
