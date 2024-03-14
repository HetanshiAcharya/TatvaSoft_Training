using HaloDocDataAccess.DataContext;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using HaloDocDataAccess.ViewModels;
using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Http;


namespace HaloDocWeb.Controllers
{
    public class HomeController : Controller
    {     
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IAuthService _authservice;
        private readonly IAdminService _adminservice;

        public HomeController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SubmitReq()
        {
            return View();
        }
        //GET
        public IActionResult PatientLogin()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(PatientLoginDetails userdetails)
        {
            if (ModelState.IsValid)
            {
                if (_authservice.PatientAuthentication(userdetails)){
                    User user = _context.Users.FirstOrDefault(Au => Au.Email == userdetails.Email);
                    HttpContext.Session.SetInt32("userId", user.UserId);
                    return RedirectToAction("Dashboard", "PatientDashboard");

                }
                else
                {
                    ViewData["error"] = "Invalid Id/Password";
                }

            }
            return View(userdetails);
        }
        //GET
        public IActionResult PatientForgotPass()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientForgotPass(PatientForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                if (_authservice.PatientForgotPass(model))
                {
                    var user = _context.AspNetUsers.FirstOrDefault(rq => rq.Email == model.Email);
                    return RedirectToAction("ResetPass", user);
                }
            }
            return View(model);
        }
        //GET
        public IActionResult ResetPass(PatientResetPassword model)
        {
            return View(model);
        }
        //POST
        [HttpPost, ActionName("ResetPass")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(PatientResetPassword patientresetpass)
        {
            if (ModelState.IsValid)
            {
                _authservice.ResetPassword(patientresetpass);
                return RedirectToAction("PatientLogin", "Home");
            }
            return View(patientresetpass);
        }
        public IActionResult ReviewAgreement(int Reqid)
        {
            reviewAgreement obj = new reviewAgreement();
            obj.RequestId = Reqid;
            return View(obj);
        }
        [HttpPost]
        public IActionResult ReviewAgreement(reviewAgreement model)
        {
            _adminservice.SendAgreement_accept(model.RequestId);
            return RedirectToAction("PatientLogin");
        }
        public IActionResult CancelAgreementModal(int RequestId)
        {
            Request? req = _context.Requests.FirstOrDefault(x => x.RequestId == RequestId);
            CancelAgreementModal obj = new()
            {
                ReqId = RequestId,
                PatientName = req.FirstName + " " + req.LastName
            };

            return PartialView("_CancelAgreementModal", obj);
        }
        public IActionResult CancelAgreementSubmit(int Reqid, string Description)
        {
            _adminservice.CancelAgreementSubmit(Reqid, Description);
            return RedirectToAction("PatientLogin");
        }
    }
}