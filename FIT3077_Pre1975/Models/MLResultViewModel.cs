using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class MLResultViewModel
    {
        [DisplayName("Accuracy Score")]
        public double accuracy { get; set; }

        [DisplayName("F1 Score")]
        public double f1score { get; set; }
    }
}
