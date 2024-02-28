using HaloDocDataAccess.DataContext;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using HaloDocDataAccess.ViewModels;
using HaloDocDataAccess.DataModels;

namespace HaloDocWeb.Controllers
{
    public class RequestsController : Controller
    {
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;

        public RequestsController(HaloDocDbContext context, IPatientService patientService)
        {
            _context = context;
            _patientService = patientService;
        }

        //GET-
        public IActionResult CreatePatientReq()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePatientReq(PatientSubmitRequests userdetails)
        {
            if(ModelState.IsValid)
            {
                _patientService.PatientRequest(userdetails);
                return RedirectToAction("SubmitReq", "Home");

            }
            return View(userdetails);
        }

        //GET-
        public IActionResult CreateFamReq()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateFamReq(FamilySubmitRequests viewdata)
        {
            if (ModelState.IsValid)
            {
                _patientService.FamilyRequest(viewdata);
                return RedirectToAction("SubmitReq", "Home");

            }
            return View(viewdata);
        }


        //GET-
        public IActionResult CreateConReq()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConReq(ConciergeSubmitRequests viewdata)
        {
            if (ModelState.IsValid)
            {
                _patientService.ConciergeRequest(viewdata);
                return RedirectToAction("SubmitReq", "Home");

            }
            return View(viewdata);
        }
        //GET-
        public IActionResult CreateBusReq()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBusReq(BusinessSubmitRequests viewdata)
        {
            if (ModelState.IsValid)
            {
                _patientService.BusinessRequest(viewdata);
                return RedirectToAction("SubmitReq", "Home");

            }
            return View(viewdata);
        }
    }
}
