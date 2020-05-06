using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Address
    {
        public List<string> Line { get; set; }

        public string City { get; set; }
        
        public string District { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

    }
}
