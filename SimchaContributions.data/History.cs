using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaContributions.data
{
  public  class HistoryItem
    {
        public DateTime Date { get; set; }
        public string Transaction { get; set; }
        public decimal Amount { get; set; }
    }
}
