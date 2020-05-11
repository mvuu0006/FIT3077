using FIT3077_Pre1975.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    public sealed class PractitionerMapper : MapperBase<FIT3077_Pre1975.Models.Practitioner, Hl7.Fhir.Model.Practitioner>
    {

        private const string ID_SYSTEM = "http://hl7.org/fhir/sid/us-npi";

        public override Models.Practitioner Map(Hl7.Fhir.Model.Practitioner element)
        {
            var practitioner = new Models.Practitioner();
            
            foreach (var identifier in element.Identifier)
            {
                if (identifier.System.Equals(ID_SYSTEM))
                {
                    practitioner.Id = identifier.Value;
                    break;
                }
            }

            practitioner.Name = element.Name[0].ToString();

            if (element.BirthDateElement != null)
            {
                practitioner.BirthDate = ((DateTimeOffset)element.BirthDateElement.ToDateTimeOffset()).DateTime;
            }
            else
            {
                practitioner.BirthDate = null;
            }

            if (Enum.TryParse(element.GenderElement.TypeName, out Gender gender))
            {
                practitioner.Gender = gender;
            }

            if (element.Address.Count > 0)
            {
                Hl7.Fhir.Model.Address address = element.Address[0];
                AddressMapper mapper = new AddressMapper();
                practitioner.Address = mapper.Map(address);
            }
            
            return practitioner;
        }

        public override Hl7.Fhir.Model.Practitioner Map(Models.Practitioner element)
        {
            throw new NotImplementedException();
        }
    }
}
