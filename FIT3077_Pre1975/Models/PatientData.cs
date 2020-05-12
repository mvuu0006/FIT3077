using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class PatientData
    {
        [LoadColumn(0)]
        public string Id { get; set; }

        [LoadColumn(1)]
        public decimal Cholesterol { get; set; }
    }
}
