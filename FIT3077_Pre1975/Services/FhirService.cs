using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Services
{
    public static class FhirService
    {
        private const string SERVICE_ROOT_URL = "https://fhir.monash.edu/hapi-fhir-jpaserver/fhir";

        private const int SERVICE_TIMEOUT = 60 * 1000;

        private static readonly FhirClient Client = new FhirClient(SERVICE_ROOT_URL) { Timeout = SERVICE_TIMEOUT };

        public static Practitioner GetPractitioner(string practitionerId)
        {
            Practitioner practitioner = null;
            try
            {
                var PractitionerQuery = new SearchParams()
                    .Where("identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId);

                Bundle PractitionerResult = Client.Search<Practitioner>(PractitionerQuery);

                if (PractitionerResult.Entry.Count > 0)
                {
                    practitioner = (Practitioner)PractitionerResult.Entry[0].Resource;
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }
            return practitioner;
        }

        public static List<Patient> GetPatientsOfPractitioner(string practitionerId)
        {
            List<Patient> patients = new List<Patient>();

            SortedSet<string> patientIdList = new SortedSet<string>();

            try
            {
                var encounterQuery = new SearchParams()
                    .Where("participant.identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId)
                    .Include("Encounter.participant.individual")
                    .Include("Encounter.patient");
                Bundle Result = Client.Search<Encounter>(encounterQuery);

                foreach (var Entry in Result.Entry)
                {
                    Encounter encounter = (Encounter)Entry.Resource;
                    string patientRef = encounter.Subject.Reference;
                    string patientId = patientRef.Split('/')[1];
                    patientIdList.Add(patientId);
                }

                foreach (var patientId in patientIdList)
                {
                    Bundle PatientResult = Client.SearchById<Patient>(patientId);

                    if (PatientResult.Entry.Count > 0)
                    {
                        Patient patient = (Patient)PatientResult.Entry[0].Resource;
                        patients.Add(patient);
                    }
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }
            
            return patients;
        }
    }
}
