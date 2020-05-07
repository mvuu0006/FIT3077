using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    public class AddressMapper : MapperBase<Models.Address, Hl7.Fhir.Model.Address>
    {
        public override Models.Address Map(Hl7.Fhir.Model.Address element)
        {
            var address = new Address();
            
            address.Line = new List<string>();
            foreach (var line in element.LineElement)
            {
                address.Line.Add(line.Value);
            }

            address.City = element.City;
            address.District = element.District;
            address.State = element.State;
            address.PostalCode = element.PostalCode;
            address.Country = element.Country;

            return address;
        }

        public override Hl7.Fhir.Model.Address Map(Models.Address element)
        {
            throw new NotImplementedException();
        }
    }
}
