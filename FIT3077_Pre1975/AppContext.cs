using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975
{
    public static class AppContext 
    {
        internal static PatientsList Patients { get; set; } = new PatientsList();
        
        internal static PatientsList MonitorPatients { get; set; } = new PatientsList();

        private static Practitioner _practitioner;

        internal static Practitioner Practitioner
        {
            get
            {
                return _practitioner;
            }
            set
            {
                _practitioner = value;
                _practitioner.Attach(Patients);
            }
        }
    }
}
