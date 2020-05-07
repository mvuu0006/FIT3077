using FIT3077_Pre1975.Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Practitioner : Person, ISubject
    {
        private ArrayList observers = new ArrayList();

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
