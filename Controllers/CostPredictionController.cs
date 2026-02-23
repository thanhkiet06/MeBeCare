using Microsoft.AspNetCore.Mvc;
using MeBeCare.Models;

namespace MeBeCare.Controllers
{
    public class CostPredictionController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View(new CostPredictionViewModel());
        }

        [HttpPost]
        public IActionResult Index(CostPredictionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            switch (model.Category)
            {
                case "Medical":
                    model.DoctorFee = 300000 * model.DurationInMonths;
                    model.MedicationCost = 200000 * model.DurationInMonths;
                    model.SupplementCost = 150000 * model.DurationInMonths;
                    model.UltrasoundCost = 120000 * model.DurationInMonths;
                    model.LabTestCost = 100000 * model.DurationInMonths;
                    break;

                case "Childbirth":
                    model.HospitalStayCost = 3000000;
                    model.PostpartumCareCost = 2500000;
                    model.BabyCareCost = 1200000;
                    model.AmbulanceCost = 500000;

                    if (model.ChildbirthType == "Normal")
                    {
                        model.DeliveryCost = 5000000;
                        model.AnesthesiaCost = 0;
                    }
                    else if (model.ChildbirthType == "C-section")
                    {
                        model.DeliveryCost = 9000000;
                        model.AnesthesiaCost = 1500000;
                    }
                    break;
                case "Other":
                    model.OtherCost = 100000 * model.DurationInMonths;
                    break;
            }

            model.PredictedCost =
                model.MedicationCost + model.DoctorFee + model.LabTestCost +
                model.VaccinationCost + model.OtherCost +
                model.HospitalStayCost + model.DeliveryCost +
                model.AnesthesiaCost + model.PostpartumCareCost;

            return View(model);
        }
    }
}
