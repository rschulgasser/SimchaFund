using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaContributions.data;


namespace SimchaCont.web.Models
{
    public class HistoryModel
    {
       public List<HistoryItem> HistoryItems { get; set; }
        public decimal Total { get; set; }
        
    }
}
