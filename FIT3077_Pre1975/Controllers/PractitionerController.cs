using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Models.PractitionerViewModels;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FIT3077_Pre1975.Controllers
{
    public class PractitionerController : Controller
    {

        private static Practitioner _practitioner;

        internal static Practitioner Practitioner { 
            get
            {
                return _practitioner;
            }
            set
            {
                _practitioner = value;
                _practitioner.Attach(PatientListController.Patients);
            }
        }

        // GET: /Practitioner/Login
        //
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Practitioner/Login
        //
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Practitioner newPractininer;
            newPractininer = FhirService.GetPractitioner(model.Id);
            if (newPractininer != null)
            {
                Practitioner = newPractininer;
                Practitioner.Notify();
                return Redirect("/Practitioner/");
            }
            else
            {
                TempData["ErrorMessage"] = "Practitioner Not Found. Please try again!";
                return View("Login");
            }
        }

        // GET: /Practitioner/Detail
        //
        public IActionResult Index()
        {
            if (Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            return View(Practitioner); 
        }

    }
}