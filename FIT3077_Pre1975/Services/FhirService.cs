using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.FhirPath.Sprache;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Services
{
    public static class FhirService
    {
        private const string SERVICE_ROOT_URL = "https://fhir.monash.edu/hapi-fhir-jpaserver/fhir";

        private const int SERVICE_TIMEOUT = 60 * 1000;

        private const int LIMIT_ENTRY = 200;

        private const int NUMBER_OF_DATA_RECORD = 200;

        private static readonly FhirClient Client = new FhirClient(SERVICE_ROOT_URL) { Timeout = SERVICE_TIMEOUT };

        public static async Task<Models.Practitioner> GetPractitioner(string practitionerId)
        {
            Models.Practitioner practitioner = null;
            Hl7.Fhir.Model.Practitioner fhirPractitioner = null;

            try
            {
                var PractitionerQuery = new SearchParams()
                    .Where("identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId)
                    .LimitTo(LIMIT_ENTRY);

                Bundle PractitionerResult = await Client.SearchAsync<Hl7.Fhir.Model.Practitioner>(PractitionerQuery);

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

        public static async Task<List<Models.Patient>> GetPatientsOfPractitioner(string practitionerId)
        {
            List<Models.Patient> patientList = new List<Models.Patient>();

            SortedSet<string> patientIdList = new SortedSet<string>();

            Models.Practitioner carer = await GetPractitioner(practitionerId);

            try
            {
                var encounterQuery = new SearchParams()
                    .Where("participant.identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId)
                    .Include("Encounter.participant.individual")
                    .Include("Encounter.patient")
                    .LimitTo(LIMIT_ENTRY);
                Bundle Result = await Client.SearchAsync<Encounter>(encounterQuery);

                while (Result != null)
                {
                    foreach (var Entry in Result.Entry)
                    {
                        Encounter encounter = (Encounter)Entry.Resource;
                        string patientRef = encounter.Subject.Reference;
                        string patientId = patientRef.Split('/')[1];
                        patientIdList.Add(patientId);
                    }

                    Result = Client.Continue(Result, PageDirection.Next);
                }

                foreach (var patientId in patientIdList)
                {
                    Bundle PatientResult = await Client.SearchByIdAsync<Hl7.Fhir.Model.Patient>(patientId);

                    if (PatientResult.Entry.Count > 0)
                    {
                        Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient)PatientResult.Entry[0].Resource;
                        PatientMapper mapper = new PatientMapper();
                        Models.Patient patient = mapper.Map(fhirPatient);
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

        public static async Task<PatientsList> GetCholesterolValues(PatientsList patients)
        {
            PatientsList MonitoredPatientList = new PatientsList();

            foreach (Models.Patient MonitoredPatient in patients)
            {
                try
                {

                    var ObservationQuery = new SearchParams()
                            .Where("patient=" + MonitoredPatient.Id)
                            .Where("code=2093-3")
                            .OrderBy("-date")
                            .LimitTo(1);

                    Bundle ObservationResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(ObservationQuery);

                    Models.Patient patient = MonitoredPatient;
                    patient.Observations = new List<Models.Observation>();
                    patient.HasObservations = true;

                    if (ObservationResult.Entry.Count > 0)
                    {
                        Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)ObservationResult.Entry[0].Resource;
                        ObservationMapper mapper = new ObservationMapper();
                        Models.Observation cholesterolObservation = mapper.Map(fhirObservation);
                        cholesterolObservation.Subject = patient;
                        patient.Observations.Add(cholesterolObservation);

                    }
                    MonitoredPatientList.AddPatient(patient);
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

        public static async Task<Models.Patient> GetDataForAnalysis(Models.Patient patient)
        {
            Models.Patient currentPatient = patient;
            currentPatient.Observations = new List<Models.Observation>();
            currentPatient.HasObservations = true;
            try
            {
                var ObservationQuery = new SearchParams()
                        .Where("patient=" + currentPatient.Id)
                        .OrderBy("-date")
                        .LimitTo(LIMIT_ENTRY);

                Bundle ObservationResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(ObservationQuery);

                foreach (var Entry in ObservationResult.Entry)
                {
                    Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)Entry.Resource;
                    if (fhirObservation.Value != null && fhirObservation.Value is Hl7.Fhir.Model.Quantity)
                    {
                        ObservationMapper mapper = new ObservationMapper();
                        Models.Observation observation = mapper.Map(fhirObservation);
                        if (!currentPatient.ContainsObservation(observation.CodeText))
                        {
                            observation.Subject = currentPatient;
                            currentPatient.Observations.Add(observation);
                        }
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

            return currentPatient;
        }

        public static async Task<PatientsList> GetData()
        {
            int count = 0;
            bool stop = false;

            PatientsList data = new PatientsList();

            try
            {

                var PatientQuery = new SearchParams().LimitTo(LIMIT_ENTRY);

                Bundle PatientResult = await Client.SearchAsync<Hl7.Fhir.Model.Patient>(PatientQuery);

                while (PatientResult != null)
                {
                    if (stop) break;

                    foreach (var Entry in PatientResult.Entry)
                    {
                        Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient) Entry.Resource;
                        PatientMapper mapper = new PatientMapper();
                        Models.Patient patient = mapper.Map(fhirPatient);
                        
                        if (!AppContext.AnalysisData.Contains(patient) && !data.Contains(patient))
                        {
                            var CholesterolQuery = new SearchParams()
                                .Where("patient=" + patient.Id)
                                .Where("code=2093-3")
                                .OrderBy("-date")
                                .LimitTo(1);

                            Bundle CholesterolResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(CholesterolQuery);
                            if (CholesterolResult.Entry.Count > 0)
                            {
                                data.AddPatient(await GetDataForAnalysis(patient));
                                count++;
                            }
                        }

                        if (count == NUMBER_OF_DATA_RECORD)
                        {
                            stop = true;
                            break;
                        }
                    }

                    PatientResult = Client.Continue(PatientResult, PageDirection.Next);
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

            return data;
        }
    }
}
