﻿using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Patient
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public Address Address { get; set; }

        public Practitioner Carer;

        public List<Observation> Observations { get; set; }

        public bool HasObservations { get; set; } = false;

        public bool Selected { get; set; } = false;

        public bool ContainsObservation(string Text)
        {
            foreach (Observation observation in Observations)
            {
                if (observation.CodeText.Equals(Text))
                {
                    return true;
                }
            }
            return false;
        }

        public Observation GetObservationByCodeText(string Text)
        {
            foreach (Observation observation in Observations)
            {
                if (observation.CodeText.Equals(Text))
                {
                    return observation;
                }
            }
            return null;
        }
    }
}
