﻿using FIT3077_Pre1975.Models;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975
{
    /// <summary>
    /// This class holds the data in the Application's run-time 
    /// </summary>
    public static class AppContext 
    {

        // List of Patients associated to a Practitioner
        internal static PatientsList Patients { get; set; } = new PatientsList();
        
        // List of Monitored Patients selected by user
        internal static PatientsList MonitorPatients { get; set; } = new PatientsList();

        // List of Patients containing data for ML task
        internal static PatientsList AnalysisData { get; set; } = new PatientsList();

        // Machine Learning Context
        internal static MLContext MlContext = new MLContext();

        private static Practitioner _practitioner;

        // Practitioner
        internal static Practitioner Practitioner
        {
            get
            {
                return _practitioner;
            }
            set
            {
                // Attach PatientsList to a Practitioner 
                // so whenever the practitioner id changes, the patients list will be updated
                _practitioner = value;
                _practitioner.Attach(Patients);
            }
        }

        internal static int Interval { get; set; } = 10; //default value for Interval
    }
}
