using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Patient : Person
    {

        public bool Deceased { get; set; }
        
        public DateTime DeceasedDateTime { get; set; }

    }
}
