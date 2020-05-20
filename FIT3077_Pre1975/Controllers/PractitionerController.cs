using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FIT3077_Pre1975.Controllers
{
    public class PractitionerController : Controller
    {

        // GET: /Practitioner/Login
        //
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Practitioner/Login
        //
        [HttpPost]
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Practitioner newPractitioner;
            newPractitioner = await FhirService.GetPractitioner(model.Id);
            if (newPractitioner != null)
            {
                AppContext.Practitioner = newPractitioner;
                AppContext.MonitorPatients = new PatientsList();
                AppContext.Practitioner.Notify();
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
            if (AppContext.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            return View(AppContext.Practitioner); 
        }

    }
}