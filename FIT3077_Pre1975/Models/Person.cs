﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public enum GenderCode
    {
        male,
        female,
        other,
        unknown
    }

    public class Person
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public GenderCode Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public Address Address { get; set; }
    }
}