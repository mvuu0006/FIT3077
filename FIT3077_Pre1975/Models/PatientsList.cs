using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Models.PractitionerViewModels;
using FIT3077_Pre1975.Observers;
using FIT3077_Pre1975.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class PatientsList : IteratorAggregate, IObserver
    {
        private List<Patient> _patients;

        private Practitioner _practitioner;

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
            return new PatientsListIterator(this);
        }

        public void Update(IObservableSubject subject)
        {
            if (_practitioner == null || (subject as Practitioner).Id != _practitioner.Id)
            {
                _practitioner = (Practitioner)subject;
                _patients = FhirService.GetPatientsOfPractitioner(_practitioner.Id);
            }
        }
    }
}
