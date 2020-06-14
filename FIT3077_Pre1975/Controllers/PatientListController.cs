using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT3077_Pre1975.Controllers
{
    /// <summary>
    /// Controller class for PatientList View and Monitor View
    /// </summary>
    public class PatientListController : Controller
    {

        // GET: /Practitioner/Detail
        //
        public IActionResult Index()
        {
            if (AppContext.Practitioner == null)
            {
                // Redirect to Login page if practitioner id is not entered
                return Redirect("/Practitioner/Login/");
            }
            else
            {
                return View();
            }

        }

        /// <summary>
        /// Get PatientList table
        /// </summary>
        /// <returns> a table contains all the patients of a practitioner </returns>
        public ActionResult GetPatientList()
        {
            // Sleep if it's still loading
            while (AppContext.Patients.IsLoading == true)
            {
                Thread.Sleep(500);
            }
            return PartialView(AppContext.Patients);
        }

        /// <summary>
        /// Get MonitorList table
        /// </summary>
        /// <returns> monitor list view </returns>
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
                    Thread.Sleep(200);
                }
                return View(AppContext.MonitorPatients);
            }
        }
        /// <summary>
        /// Handle Update Monitor event from Patient List View
        /// </summary>
        /// <param name="ListId"> list of patients id to be monitored </param>
        /// <returns> A updated Monitor View </returns>
        [HttpPost]
        public async Task<ActionResult> UpdateMonitor(List<string> ListId)
        {
            AppContext.MonitorPatients.IsLoading = true;
            PatientsList newMonitorList = new PatientsList();
            PatientsList queryPatients = new PatientsList();

            // Only add Patients haven't queried observations to avoid repeated query
            // One patient only needs to query once at the first time it is selected
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

            // query list of patients haven't queried Cholesterol
            PatientsList queriedPatients = await FhirService.GetObservationValues(queryPatients);
            foreach (Patient patient in queriedPatients)
            {
                newMonitorList.AddPatient(patient);
            }

            
            AppContext.MonitorPatients = newMonitorList;

            return View("Monitor");
        }

        /// <summary>
        /// Update the Monitor List after N seconds
        /// </summary>
        /// <param name="ListId"> </param>
        /// <returns> Updated Monitor List </returns>
        public async Task<ActionResult> ResetMonitorList(List<string> ListId)
        {
            PatientsList queriedPatients = await FhirService.GetObservationValues(AppContext.MonitorPatients);
            AppContext.MonitorPatients = queriedPatients;
            return View("Monitor");
        }

        /// <summary>
        /// Show patient details
        /// </summary>
        /// <param name="Id"> Patient Id to be shown </param>
        /// <returns> A popup modal view showing Patient details </returns>
        public ActionResult ShowDetail(string Id)
        {
            return PartialView("PatientDetail", AppContext.MonitorPatients.GetPatientByID(Id));
        }

        /// <summary>
        /// Remove a patient from a monitor table
        /// </summary>
        /// <param name="Id"> Patient Id to be removed </param>
        /// <returns></returns>
        public EmptyResult RemoveMonitorPatient(string Id)
        {
            AppContext.MonitorPatients.GetPatientByID(Id).Selected = false;
            AppContext.MonitorPatients.RemovePatientByID(Id);
            return new EmptyResult();
        }

        /// <summary>
        /// Set the Interval to update Monitor List
        /// </summary>
        /// <param name="newInterval"> new Interval time</param>
        /// <returns></returns>
        public EmptyResult SetUpdateInterval(int newInterval)
        {
            AppContext.Interval = newInterval;
            return new EmptyResult();
        }

        /// <summary>
        /// Get the Interval time to update Monitor List
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUpdateInterval()
        {
            return Json(AppContext.Interval);
        }
    } 
}

