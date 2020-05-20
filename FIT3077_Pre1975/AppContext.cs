using FIT3077_Pre1975.Models;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975
{
    public static class AppContext 
    {
        internal static PatientsList Patients { get; set; } = new PatientsList();
        
        internal static PatientsList MonitorPatients { get; set; } = new PatientsList();

        internal static PatientsList AnalysisData { get; set; } = new PatientsList();

        internal static MLContext MlContext = new MLContext();

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

        internal static int Interval { get; set; } = 10; //default value for Interval
    }
}
