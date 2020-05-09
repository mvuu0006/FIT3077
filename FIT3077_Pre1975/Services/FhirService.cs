using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
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

        public static Models.PractitionerViewModels.Practitioner GetPractitioner(string practitionerId)
        {
            Models.PractitionerViewModels.Practitioner practitioner = null;
            Hl7.Fhir.Model.Practitioner fhirPractitioner = null;

            try
            {
                var PractitionerQuery = new SearchParams()
                    .Where("identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId);

                Bundle PractitionerResult = Client.Search<Hl7.Fhir.Model.Practitioner>(PractitionerQuery);

                if (PractitionerResult.Entry.Count > 0)
                {
                    fhirPractitioner = (Hl7.Fhir.Model.Practitioner)PractitionerResult.Entry[0].Resource;
                    PractitionerMapper mapper = new PractitionerMapper();
                    practitioner = mapper.Map(fhirPractitioner);
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

        public static List<Models.Patient> GetPatientsOfPractitioner(string practitionerId)
        {
            List<Models.Patient> patientList = new List<Models.Patient>();

            SortedSet<string> patientIdList = new SortedSet<string>();

            Models.PractitionerViewModels.Practitioner carer = GetPractitioner(practitionerId);

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
                    Bundle PatientResult = Client.SearchById<Hl7.Fhir.Model.Patient>(patientId);

                    if (PatientResult.Entry.Count > 0)
                    {
                        Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient)PatientResult.Entry[0].Resource;
                        PatientMapper mapper = new PatientMapper();
                        Models.Patient patient = mapper.Map(fhirPatient);
                        patient.Carer = carer;
                        patientList.Add(patient);
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
            
            return patientList;
        }

        public static PatientsList GetCholesterolValue(PatientsList patients)
        {
            PatientsList MonitoredPatientList = new PatientsList();

            foreach (Models.Patient MonitoredPatient in patients)
            {
                try
                {

                    var ObservationQuery = new SearchParams()
                            .Where("patient=" + MonitoredPatient.Id)
                            .Where("code=2093-3")
                            .OrderBy("-date");
                            
                    Bundle ObservationResult = Client.Search<Hl7.Fhir.Model.Observation>(ObservationQuery);

                    if (ObservationResult.Entry.Count > 0)
                    {
                        Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)ObservationResult.Entry[0].Resource;
                        Models.Patient patient = MonitoredPatient;
                        ObservationMapper mapper = new ObservationMapper();
                        Models.Observation cholesterolObservation = mapper.Map(fhirObservation);
                        cholesterolObservation.Subject = MonitoredPatient;
                        patient.Observations = new List<Models.Observation>();
                        patient.Observations.Add(cholesterolObservation);
                        MonitoredPatientList.AddPatient(patient);
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
            }
            return MonitoredPatientList;
        }
    }
}
