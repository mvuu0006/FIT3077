﻿using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Observation
    {

        public string Id { get; set; }

        public Patient Subject { get; set; }

        [Display(Name = "Date of Issue")]
        public DateTime? Issued { get; set; }

        public string Code { get; set; }

        public string CodeText { get; set; }

        public Measurement MeasurementResult { get; set; }
    }
}
