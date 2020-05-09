using FIT3077_Pre1975.Models;
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
            var observation = new Observation
            {
                Id = element.Id
            };

            if (element.Issued != null)
            {
                observation.Issued = ((DateTimeOffset)element.Issued).DateTime;
            }
            else
            {
                observation.Issued = null;
            }

            observation.Code = element.Code.Coding[0].Code;

            observation.CodeText = element.Code.Text;

            Hl7.Fhir.Model.Quantity fhirQuantity = (Hl7.Fhir.Model.Quantity)element.Value;

            observation.MeasurementResult = new Measurement
            {
                Value = fhirQuantity.Value,
                Unit = fhirQuantity.Unit
            };

            return observation;
        }

        public override Hl7.Fhir.Model.Observation Map(Models.Observation element)
        {
            throw new NotImplementedException();
        }
    }
}
