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

        // GET: /PatientList/
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
        public IActionResult Monitor()
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
        /// Get HistoricalMonitor view
        /// </summary>
        /// <returns> monitor list view </returns>
        public ActionResult HistoricalMonitor()
        {
            while (AppContext.MonitorPatients.IsLoading == true)
            {
                Thread.Sleep(200);
            }
            List<Tracker> trackers = new List<Tracker>();
            foreach (Patient patient in AppContext.MonitorPatients)
            {
                if (patient.ContainsObservation("Systolic Blood Pressure"))
                {
                    trackers.Add(new Tracker(patient, "Systolic Blood Pressure"));
                }
            }
            return View(trackers);
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
        [HttpPost]
        public async Task<ActionResult> ResetMonitorList()
        {
            PatientsList queriedPatients = await FhirService.GetObservationValues(AppContext.MonitorPatients);
            AppContext.MonitorPatients = queriedPatients;
            return Json("Success");
        }

        // <summary>
        /// Update the View after N seconds
        /// </summary>
        public async Task<IActionResult> ResetView()
        {
            PatientsList queriedPatients = await FhirService.GetObservationValues(AppContext.MonitorPatients);
            AppContext.MonitorPatients = queriedPatients;
            return Json("Success");
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
        public IActionResult RemoveMonitorPatient(string Id)
        {
            AppContext.MonitorPatients.GetPatientByID(Id).Selected = false;
            AppContext.MonitorPatients.RemovePatientByID(Id);
            return Json("Success");
        }

        /// <summary>
        /// Set the Interval to update Monitor List
        /// </summary>
        /// <param name="newInterval"> new Interval time</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetUpdateInterval(int newInterval)
        {
            AppContext.Interval = newInterval;
            return Json("Success");
        }

        /// <summary>
        /// Get the Interval time to update Monitor List
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUpdateInterval()
        {
            return Json(AppContext.Interval);
        }
        public IActionResult SetThreshold(int newSystolicThreshold, int newDiastolicThreshold)
        {
            AppContext.SystolicThreshold = newSystolicThreshold;
            AppContext.DiastolicThreshold = newDiastolicThreshold;
            AppContext.BPThresholds[0] = newSystolicThreshold;
            AppContext.BPThresholds[1] = newDiastolicThreshold;
            return Json("Success");
        }

        public JsonResult GetThreshold()
        {
            return Json(AppContext.BPThresholds);
        }

        public JsonResult UpdateGraphData()
        {
            List<string> labels = new List<string>();
            List<string> data = new List<string>();
            
            foreach (Patient patient in AppContext.MonitorPatients) {
                if (patient.HasObservations)
                {
                    labels.Add(patient.Name);
                    data.Add(patient.Observations[0].MeasurementResult.Value.ToString());
                }
            }

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>
            {
                { "labels", labels },
                { "data", data }
            };

            return Json(dict);
        }
    } 
}

