using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Observers
{
    public interface IObservableSubject
    {

        // Attach an observer to the subject
        void Attach(IObserver observer);

        // Detach an observer from the subject
        void Detach(IObserver observer);

        // Notify all observers about an event
        void Notify();
        
    }
}
