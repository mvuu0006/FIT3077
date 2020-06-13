using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    public class ComponentObservationMapper : MapperBase<Models.Observation, Hl7.Fhir.Model.Observation.ComponentComponent>
    {
        public override Models.Observation Map(Hl7.Fhir.Model.Observation.ComponentComponent element)
        {
            /// create a new Observation object and map values from Fhir model to it
            var observation = new Observation
            {
                Code = element.Code.Coding[0].Code,

                CodeText = element.Code.Text
            };

            Hl7.Fhir.Model.Quantity fhirQuantity = (Hl7.Fhir.Model.Quantity)element.Value;

            observation.MeasurementResult = new Measurement
            {
                Value = fhirQuantity.Value,
                Unit = fhirQuantity.Unit
            };

            return observation;
        }

        public override Hl7.Fhir.Model.Observation.ComponentComponent Map(Models.Observation element)
        {
            throw new NotImplementedException();
        }
    }
}
