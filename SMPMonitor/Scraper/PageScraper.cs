using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMPMonitor.Scraper
{
    internal static class PageScraper
    {
        public static ProjectedPool ExportPageData()
        {
            ProjectedPool pool = new ProjectedPool();
            var url = "http://ets.aeso.ca/ets_web/ip/Market/Reports/CSMPriceReportServlet";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            int i = 0;
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                if (i == 1)
                {
                    Regex regex = new Regex("[0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    var hour = regex.Match(table.InnerText).Value;
                    regex = new Regex("\\$[0-9]*\\.[0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    var price = regex.Match(table.InnerText).Value;
                    regex = new Regex("[0-9]+:[0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    var time = regex.Match(table.InnerText).Value;

                    pool.price = price;
                    pool.hourEnding = hour;
                    pool.asOfTime = time;
                    //test.Value.Split("is");
                }
                if (i == 2)
                {
                    string path = "C:\\APPS\\SMPReport\\" + DateTime.Now.ToString("yyyy-MM-dd_HHmm") + ".csv";
                    FileStream fs = File.Create(path);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("Date (HE),Time,Price ($),Volume (MW)");
                    var myTableRows = table.Descendants("tr");
                    foreach (var tr in myTableRows)
                    {
                        string line = string.Join(",", tr.Descendants("td").Select(td => td.InnerText));
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        sw.WriteLine(line);
                        //Console.WriteLine(line);
                    }
                    sw.Close();
                    fs.Close();
                    //sw.Close();
                }
                i++;
            }

            return pool;
        }
    }
}
