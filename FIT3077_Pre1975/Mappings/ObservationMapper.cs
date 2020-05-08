using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    public class ObservationMapper : MapperBase<Models.Observation, Hl7.Fhir.Model.Observation>
    {
        public override Models.Observation Map(Hl7.Fhir.Model.Observation element)
        {
            throw new NotImplementedException();
        }

        public override Hl7.Fhir.Model.Observation Map(Models.Observation element)
        {
            throw new NotImplementedException();
        }
    }
}
