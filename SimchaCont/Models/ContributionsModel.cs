using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SimchaContributions.data;
using SimchaCont.web.Models;

namespace SimchaCont.web.Models
{
    public class ContributionsModel
    {
        public List<Contributer> Contributers { get; set; }
        public Simcha Simcha { get; set; }
        public int Counter  { get; set; }
    }
}
