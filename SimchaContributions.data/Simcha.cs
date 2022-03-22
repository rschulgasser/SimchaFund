using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaContributions.data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string NumberOfContributers { get; set; }
        public decimal Amount { get; set; }
        public decimal Total {get;set;}
    }
}
