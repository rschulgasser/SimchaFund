using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaContributions.data;

namespace SimchaCont.web.Models
{
    public class ContributerModel
    {
        public List<Contributer> Contributers { get; set; }
        public Decimal Total { get; set; }
    }
}
