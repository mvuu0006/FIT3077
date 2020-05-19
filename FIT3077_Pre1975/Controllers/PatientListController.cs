using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FIT3077_Pre1975.Controllers
{
    public class PatientListController : Controller
    {

        // GET: /Practitioner/Detail
        //
        public IActionResult Index()
        {
            if (AppContext.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            else
            {
                return View();
            }

        }

        public ActionResult GetPatientList()
        {
            while (AppContext.Patients.IsLoading == true)
            {
                Thread.Sleep(500);
            }
            return PartialView(AppContext.Patients);
        }

        public ActionResult Monitor()
        {
            if (AppContext.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            else
            {
                while (AppContext.MonitorPatients.IsLoading == true)
                {
                    Thread.Sleep(500);
                }
                return View(AppContext.MonitorPatients);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateMonitor(List<string> ListId)
        {
            AppContext.MonitorPatients.IsLoading = true;
            PatientsList newMonitorList = new PatientsList();
            PatientsList queryPatients = new PatientsList();
            foreach (Patient patient in AppContext.Patients)
            {
                if (ListId.Contains(patient.Id))
                {
                    patient.Selected = true;
                    if (!patient.HasObservations)
                    {
                        queryPatients.AddPatient(patient);
                    }
                    else
                    {
                        newMonitorList.AddPatient(patient);
                    }
                }
                else
                {
                    patient.Selected = false;
                }
            }
            PatientsList queriedPatients = await FhirService.GetCholesterolValues(queryPatients);
            foreach (Patient patient in queriedPatients)
            {
                newMonitorList.AddPatient(patient);
            }

            
            AppContext.MonitorPatients = newMonitorList;

            return View("Monitor");
        }

        public async Task<ActionResult> ResetMonitorList(List<string> ListId)
        {
            PatientsList queriedPatients = await FhirService.GetCholesterolValues(AppContext.MonitorPatients);
            AppContext.MonitorPatients = queriedPatients;
            return View("Monitor");
        }

        public ActionResult ShowDetail(string Id)
        {
            return PartialView("PatientDetail", AppContext.MonitorPatients.GetPatientByID(Id));
        }
    } 
}

