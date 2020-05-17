using FIT3077_Pre1975.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Patterns
{
    public interface IObserver
    {
        // Receive update from model
        void Update(IObservableSubject subject);

        Task UpdateAsync(IObservableSubject subject);
    }
}   
