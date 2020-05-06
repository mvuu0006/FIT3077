using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Helpers
{
    public abstract class IteratorAggregate : IEnumerable
    {
        // Returns an Iterator or another IteratorAggregate for the implementing object.
        public abstract IEnumerator GetEnumerator();
    }
}
