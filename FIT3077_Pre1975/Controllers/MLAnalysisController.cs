using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT3077_Pre1975.Controllers
{
    public class MLAnalysisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BuildModel()
        {
            return View("Index");
        }


        public async Task<ActionResult> LoadData()
        {
            foreach (Patient patient in AppContext.Patients)
            {
                if (!AppContext.AnalysisData.Contains(patient))
                {
                    Patient fullPatient = await FhirService.GetDataForAnalysis(patient);
                    AppContext.AnalysisData.AddPatient(fullPatient);
                }
            }

            MLHelpers.PrepareData();

            return View("Index");
        }
    }
}