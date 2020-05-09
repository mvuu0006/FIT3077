using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Measurement
    {

        public decimal? Value { get; set; }

        public string Unit { get; set; }
    }
}
