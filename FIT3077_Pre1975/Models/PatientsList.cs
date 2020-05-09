using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{

    public class PatientsList : IteratorAggregate, ISubject
    {
        private List<Patient> _patients;
        private ArrayList observers = new ArrayList();

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

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in observers)
            {
                observer.Update(this);
            }
        }
    }
}
