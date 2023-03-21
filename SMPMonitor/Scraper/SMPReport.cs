using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMPMonitor.Scraper
{
    internal class SMPReport
    {
        public DateTime begin_datetime_utc { get; set; }
        public DateTime begin_datetime_mpt { get; set; }
        public decimal system_marginal_price { get; set; }
        public decimal volume { get; set; }
    }
}
