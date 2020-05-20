using FIT3077_Pre1975.Models;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Helpers
{
    public static class MLHelpers
    {

        public const string TOTAL_CHOLESTEROL = "Total Cholesterol";

        public const double HIGH_CHOLESTEROL_THRESHOLD = 190f;

        public static string DATA_FILE_ROOT = @"Data\\data.csv";

        public static void WriteToCsv(PatientsList data)
        {
            List<PatientData> outputPatients = new List<PatientData>();

            foreach (Patient patient in data)
            {
                Single[] attributes = new Single[PatientData.NUMBER_OF_FEATURES];
                Observation CholesterolObservation = patient.GetObservationByCodeText(TOTAL_CHOLESTEROL);
                
                if (CholesterolObservation == null) continue;
                if (CholesterolObservation.MeasurementResult.Value == null) continue;

                double cholesterol = (double) CholesterolObservation.MeasurementResult.Value;
                bool label = (cholesterol > HIGH_CHOLESTEROL_THRESHOLD);

                int NaNCount = 0;

                for (int i = 0; i < PatientData.NUMBER_OF_FEATURES; i++)
                {
                    attributes[i] = Single.NaN;
                    Observation observation = patient.GetObservationByCodeText(PatientData.attributes[i]);
                    if (observation != null)
                    {
                        if (observation.MeasurementResult.Value != null)
                        {
                            attributes[i] = (Single)observation.MeasurementResult.Value;
                        }
                        else NaNCount++;
                    }
                    else NaNCount++;
                }

                if (NaNCount > 0) continue;

                PatientData newData = new PatientData
                {
                    HighCholesterol = label,
                    Features = attributes
                };

                outputPatients.Add(newData);
            }

            if (!File.Exists(DATA_FILE_ROOT))
            {
                File.WriteAllText(DATA_FILE_ROOT, string.Concat(
                string.Join(",", PatientData.Headers()) + "\n",
                string.Join("\n", outputPatients.Select(p => p.ToString()).ToArray())));
            }
            else
            {
                File.AppendAllText(DATA_FILE_ROOT, 
                    "\n" + string.Join("\n", outputPatients.Select(p => p.ToString()).ToArray()));
            }
        }

        public static IDataView ReadFromCsv()
        {

            //Load Data
            IDataView data = AppContext.MlContext.Data.LoadFromTextFile<PatientData>(DATA_FILE_ROOT, separatorChar: ',', hasHeader: true);

            return data;
        }

        public static IDataView PrepareData(IDataView data) 
        {
            // Replace NaN with Average value
            // Min-Max normalization
            var dataPrepEstimator = AppContext.MlContext.Transforms.ReplaceMissingValues("Features", replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean)
                .Append(AppContext.MlContext.Transforms.NormalizeMinMax("Features"))
                .AppendCacheCheckpoint(AppContext.MlContext);
                //.Append(AppContext.MlContext.BinaryClassification.Trainers.FastTree());

            ITransformer dataPrepTransformer = dataPrepEstimator.Fit(data);

            // Transform data
            IDataView finalData = dataPrepTransformer.Transform(data);

            /*IEnumerable<PatientData> patientDataEnumerable = AppContext.MlContext.Data.CreateEnumerable<PatientData>(finalData, reuseRowObject: true);

            // Iterate over each row
            foreach (PatientData row in patientDataEnumerable)
            {
                Debug.WriteLine(row.ToString());
            }*/

            return finalData;
        }
    }
}
