using FIT3077_Pre1975.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class MonitoredPatientsList : PatientsList
    {
        List<Patient> _patients;

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

