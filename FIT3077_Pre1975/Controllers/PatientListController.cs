using System;
using System.Collections.Generic;
using System.Linq;
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
            if (Patients == null || PractitionerController.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            return View(Patients);
        }
    }
}
