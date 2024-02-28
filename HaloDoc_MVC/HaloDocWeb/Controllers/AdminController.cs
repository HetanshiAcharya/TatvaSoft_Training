using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;

namespace HaloDocDataAccess.Controllers
{
    public class AdminController : Controller
    {
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IAuthService _authservice;
        private readonly IAdminService _adminservice;

        public AdminController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;

        }

        //GET
        public IActionResult IndexPlatformLogin()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPlatformLogin(AdminLogin adminLogin)
        {
            if (ModelState.IsValid)
            {
                if (_adminservice.AdminAuthentication(adminLogin))
                {

                    return RedirectToAction("Index","Admin");
                }
            }
            return View(adminLogin);
        }
        
        public IActionResult IndexForgotPass()
        {
            return View();
        }
        //////////////////////////////////
        //GET
        //public IActionResult AdminDashboard()
        //{
        //    return View();
        //}
        //POST
        public IActionResult Index()
        {
            CountStatusWiseRequestModel sm = _adminservice.Indexdata();

            return View(sm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(string Status)
        {
            if (Status == null)
            {
                Status = "1";
            }
            List<AdminDashboardList> contacts = _adminservice.GetRequests(Status);

            switch (Status)
            {
                case "1":
                    return PartialView("../Admin/_New", contacts);
                    break;

                case "2":
                    return PartialView("../Admin/_Pending", contacts);
                    break;

                case "4,5":
                    return PartialView("../Admin/_Active", contacts);
                    break;

                case "6":
                    return PartialView("../Admin/_Conclude", contacts);
                    break;

                case "3,7,8":
                    return PartialView("../Admin/_ToClose", contacts);
                    break;

                case "9":
                    return PartialView("../Admin/_UnPaid", contacts);
                    break;
            }


            return PartialView("../Admin/_New", contacts);
        }
        //GET
        public IActionResult ViewCase()
        {
            return View();
        }
        //GET
        public IActionResult ViewNotes()
        {
            return View();
        }
    }
}
