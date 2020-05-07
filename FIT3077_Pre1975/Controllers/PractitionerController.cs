using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT3077_Pre1975.Controllers
{
    public class PractitionerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult EnterPractitionerId(string Id)
        {

            Hl7.Fhir.Model.Practitioner fhirPractitioner;
            Practitioner practitioner;
            if (ModelState.IsValid)
            {
                fhirPractitioner = FhirService.GetPractitioner(Id);
                if (fhirPractitioner != null)
                {
                    PractitionerMapper mapper = new PractitionerMapper();
                    practitioner = mapper.Map(fhirPractitioner);
                    return View("Detail", practitioner);
                }
            }
            return View("Index");
        }
    }
}