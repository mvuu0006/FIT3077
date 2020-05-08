using FIT3077_Pre1975.Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{

    public class Practitioner : ISubject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public Address Address { get; set; }

        private ArrayList Observers = new ArrayList();

        public void Attach(IObserver observer)
        {
            Observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in Observers)
            {
                observer.Update(this);
            }
        }
    }
}
