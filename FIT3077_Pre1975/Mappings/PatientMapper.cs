using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    public class PatientMapper : MapperBase<FIT3077_Pre1975.Models.Patient, Hl7.Fhir.Model.Patient>
    {
        public override Models.Patient Map(Hl7.Fhir.Model.Patient element)
        {
            var patient = new Models.Patient();

            patient.Id = element.Id;

            patient.Name = element.Name[0].ToString();

            if (element.BirthDateElement != null)
            {
                patient.BirthDate = ((DateTimeOffset)element.BirthDateElement.ToDateTimeOffset()).DateTime;
            }
            else
            {
                patient.BirthDate = null;
            }

            if (Enum.TryParse(element.GenderElement.TypeName, out Gender gender))
            {
                patient.Gender = gender;
            }

            if (element.Address.Count > 0)
            {
                Hl7.Fhir.Model.Address address = element.Address[0];
                AddressMapper mapper = new AddressMapper();
                patient.Address = mapper.Map(address);
            }

            return patient;
        }

        public override Hl7.Fhir.Model.Patient Map(Models.Patient element)
        {
            throw new NotImplementedException();
        }
    }
}
