using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaContributions.data
{
   public class Contributer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Cell { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal AmountPerSimcha { get; set; }
        public bool Included { get; set; }

    }
}
