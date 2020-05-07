using FIT3077_Pre1975.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class PatientsList : IteratorAggregate
    {
        private List<Patient> _patients;

        public PatientsList() {
            _patients = new List<Patient>();
        }

        public PatientsList(List<Patient> patients) {
            _patients = patients;
        }

        public int Count
        {
            get { return _patients.Count; }
        }

        public void AddPatient(Patient patient) {
            _patients.Add(patient);
        }

        public Patient GetPatientAt(int index) {
            if (index >= 0 && index < Count)
            {
                return _patients[index];
            } 
            else
            {
                return null;
            }
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
