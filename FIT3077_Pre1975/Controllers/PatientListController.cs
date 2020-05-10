using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT3077_Pre1975.Controllers
{
    public class PatientListController : Controller
    {

        internal static PatientsList Patients { get; set; } = new PatientsList();

        // GET: /Practitioner/Detail
        //
        public IActionResult Index()
        {
            if (PractitionerController.Practitioner == null)
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
            while (Patients.IsLoading == true)
            {
                Thread.Sleep(500); 
            }
            return PartialView(Patients);
        }

        public IActionResult Monitor()
        {
            if (PractitionerController.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetMonitorList()
        {
            return PartialView(Patients);
        }
    }
}
