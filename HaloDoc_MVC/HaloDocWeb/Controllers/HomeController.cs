using HaloDocDataAccess.DataContext;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using HaloDocDataAccess.ViewModels;
using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HaloDocWeb.Controllers
{
    public class HomeController : Controller
    {     
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IAuthService _authservice;
        private readonly IAdminService _adminservice;
        private readonly INotyfService _notyf;

        public HomeController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice, INotyfService notyf)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;
            _notyf = notyf;
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
        public IActionResult PatientSite()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientLogin(PatientLoginDetails userdetails)
        {
            if (_authservice.PatientAuthentication(userdetails))
            {
                User user = _context.Users.FirstOrDefault(Au => Au.Email == userdetails.Email);
                HttpContext.Session.SetInt32("userId", user.UserId);
                return RedirectToAction("Dashboard", "PatientDashboard");

            }
            else
            {
                ViewData["error"] = "Invalid Id/Password";
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
        public IActionResult ReviewAgreement(int ReqId)
        {
            reviewAgreement obj = new reviewAgreement();
            obj.RequestId = ReqId;
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
        public IActionResult Register(string Email)
        {
            PatientSubmitRequests res = new PatientSubmitRequests();
            res.Email = Email;
            return View(res);
        }
        public IActionResult CreateAccount(PatientSubmitRequests viewPatientReq)
        {
            var res = _adminservice.CreateAccount(viewPatientReq);
            if (res)
            {
                _notyf.Success("Account Created successfully");
                return View("PatientLogin");
            }
            else
            {
                _notyf.Error("You are already registered...");
                return View("Register");
            }

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            return RedirectToAction("PatientLogin", "Home");
        }
    }
}