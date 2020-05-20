using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FIT3077_Pre1975.Controllers
{
    public class MLAnalysisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Result()
        {
            return View();
        }

        public async Task<ActionResult> TrainModel()
        {
            // Read data from CSV file
            IDataView data = MLHelpers.ReadFromCsv();

            data = AppContext.MlContext.Data.Cache(data);

            IDataView cleanData = MLHelpers.PrepareData(data);

            var modelEstimator = AppContext.MlContext.BinaryClassification.Trainers.FastTree();

            // cross-validation
            var cvResults = AppContext.MlContext.BinaryClassification.CrossValidateNonCalibrated(cleanData, modelEstimator, numberOfFolds: 4);

            double[] f1Score = cvResults
                .OrderByDescending(fold => fold.Metrics.F1Score)
                .Select(fold => fold.Metrics.F1Score)
                .ToArray();

            double[] accuracy = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Metrics.Accuracy)
                .ToArray();

            ITransformer[] models = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Model)
                .ToArray();

            BinaryClassificationMetrics[] metrics = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Metrics)
                .ToArray();

            // Get Top Model
            ITransformer topModel = models[0];

            MLResultViewModel resultView = new MLResultViewModel
            {
                accuracy = f1Score[0],
                f1score = accuracy[0]
            };

            return View("Result", resultView);
        }

        public async Task<ActionResult> GetData()
        {
            _ = FhirService.GetData().ContinueWith((data) =>
              {
                  MLHelpers.WriteToCsv(data.Result);

                  foreach (Patient p in data.Result)
                  {
                      AppContext.AnalysisData.AddPatient(p);
                  }
              });
            /*await FhirService.GetData();*/

            return View("Index");
        }
    }
}