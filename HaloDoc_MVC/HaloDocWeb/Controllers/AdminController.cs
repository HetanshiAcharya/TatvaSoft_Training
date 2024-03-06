using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using HaloDocWeb.Models;
using System.Diagnostics;
using System.Collections;

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
                    AspNetUser? aspuser = _context.AspNetUsers.FirstOrDefault(Au => Au.Email == adminLogin.Email);
                    HttpContext.Session.SetString("userId", aspuser.Id);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewData["error"] = "Invalid Id/Password";

                }
            }
            return View(adminLogin);
        }
        
        public IActionResult IndexForgotPass()
        {
            return View();
        }

        public IActionResult Index()
        {
            string? userId = HttpContext.Session.GetString("userId");
            if (userId == null)
            {
                return View("Error");

            }
            ViewBag.CancelCase = _adminservice.CancelCase();
            ViewBag.AssignCase = _adminservice.AssignCase();
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

        public IActionResult ViewCase(int? RId, int? RTId)
        {
            ViewBag.AssignCase = _adminservice.AssignCase();

            ViewCaseData vdvc = _adminservice.NewRequestData(RId, RTId);
            return View(vdvc);
        }

        [HttpPost]
        public IActionResult ViewCase(ViewCaseData vdvc, int? RId, int? RTId)
        {
            ViewCaseData vc = _adminservice.Edit(vdvc, RId, RTId);
            return View(vc);
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult PhysicianbyRegion(int Regionid)
        {
            var v = _adminservice.ProviderbyRegion(Regionid);
            return Json(v);
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminservice.AssignCaseInfo(RequestId, PhysicianId, Notes);
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult CancelCase(int casetagId, int RequestId, string Notes)
        {
            _adminservice.CancelCaseInfo(casetagId, Notes, RequestId);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            var res =_adminservice.BlockCaseInfo(RequestId, Notes);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ViewNotes(int reqClientId)
        {
            int? adminId = HttpContext.Session.GetInt32("adminId");
            var obj = _adminservice.ViewNotes(reqClientId);
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewNotesUpdate(ViewNotes viewNotes)
        {
            if (ModelState.IsValid)
            {
                int? adminId = HttpContext.Session.GetInt32("adminId");
                _adminservice.ViewNotesUpdate(viewNotes);
                return ViewNotes(viewNotes.Requestclientid);
            }
            return View(viewNotes);
        }
        public IActionResult ViewUploads(int requestId)
        {
            //int? userid = HttpContext.Session.GetInt32("userId");
            RequestClient request = _context.RequestClients.FirstOrDefault(r => r.RequestId == requestId);
            Request req = _context.Requests.FirstOrDefault(r => r.RequestId == requestId);
            List<RequestWiseFile> fileList = _context.RequestWiseFiles.Where(reqFile => reqFile.RequestId == requestId && reqFile.IsDeleted== new BitArray(1)).ToList();

            ViewDocument document = new()
            {
                requestwisefiles = fileList,
                RequestId = requestId,
                ConfirmationNumber = req.ConfirmationNumber,
                UserName = request.FirstName + " " + request.LastName,
            };
            return View(document);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewDocument viewdata)
        {
            string UploadImage = "";
            if (viewdata.File != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileNameWithPath = Path.Combine(path, viewdata.File.FileName);
                UploadImage = viewdata.File.FileName;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewdata.File.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = viewdata.RequestId,
                    FileName = viewdata.File.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted= new BitArray(1)
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }

            return ViewUploads(viewdata.RequestId);
        }
        public IActionResult DeleteFile(int requestid, int reqwisefileid)
        {
             _adminservice.DeleteFile(requestid, reqwisefileid);
            return RedirectToAction("ViewUploads",new {requestId= requestid } );
        }
    }
}
