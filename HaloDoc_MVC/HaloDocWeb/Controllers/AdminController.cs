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
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;
using System.Data;
using Microsoft.AspNetCore.Identity;
using HaloDocWeb.Controllers.Admin;
using System;
using Microsoft.AspNetCore.Routing;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HaloDocDataAccess.Controllers
{
    public class AdminController : Controller
    {
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IAuthService _authservice;
        private readonly IAdminService _adminservice;
        private readonly ILoginRepository _loginRepository;
        private readonly IJwtService _jwtService;
        private readonly INotyfService _notyf;

        public AdminController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice, ILoginRepository loginRepository,IJwtService jwtService, INotyfService notyf)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        //--------------Admin Login-----------------
        //GET
        public IActionResult IndexPlatformLogin()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPlatformLogin(AspNetUser aspNetUser)
        {
            UserInfo u = await _loginRepository.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _jwtService.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../Admin/IndexPlatformLogin");
            }
        }

        //--------------Old Admin Login-----------------

        //public IActionResult IndexPlatformLogin(AdminLogin adminLogin)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (_adminservice.AdminAuthentication(adminLogin))
        //        {
        //            AspNetUser? aspuser = _context.AspNetUsers.FirstOrDefault(Au => Au.Email == adminLogin.Email);
        //            HttpContext.Session.SetString("userId", aspuser.Id);
        //            return RedirectToAction("Index", "Admin");
        //        }
        //        else
        //        {
        //            ViewData["error"] = "Invalid Id/Password";

        //        }
        //    }
        //    return View(adminLogin);
        //}

        //--------------Admin Forgot Pass--------------
        public IActionResult IndexForgotPass()
        {
            return View();
        }
        //-------Logout--------------------------------
        #region end_session
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
        #endregion

        //--------------Admin Dashboard----------------
        [CheckPhysicianAccess("Admin")]
        public IActionResult Index()
        {
            //string? userId = HttpContext.Session.GetString("userId");
            //if (userId == null)
            //{
            //    return View("Error");

            //}
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
        //--------------View Case--------------------------
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
        //--------------Error-----------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //--------------Physician by region-----------------
        public IActionResult PhysicianbyRegion(int Regionid)
        {
            var v = _adminservice.ProviderbyRegion(Regionid);
            return Json(v);
        }
        //--------------Assign case-------------------------
        [HttpPost]
        public IActionResult AssignCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminservice.AssignCaseInfo(RequestId, PhysicianId, Notes);
            _notyf.Success("Case Assigned Successfully");
            return RedirectToAction("Index", "Admin");
        }
        //--------------Cancel Case-------------------------
        [HttpPost]
        public IActionResult CancelCase(int casetagId, int RequestId, string Notes)
        {

            _adminservice.CancelCaseInfo(casetagId, Notes, RequestId);
            _notyf.Success("Case Cancelled Successfully");
            return RedirectToAction("Index", "Admin");
            
        }
        //--------------Block Case---------------------------
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            var res = _adminservice.BlockCaseInfo(RequestId, Notes);
            _notyf.Success("Case Blocked Successfully");
            return RedirectToAction("Index", "Admin");
        }
        //--------------View Notes----------------------------
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
        //--------------View Uploads--------------------------
        public IActionResult ViewUploads(int requestId)
        {
            //int? userid = HttpContext.Session.GetInt32("userId");
            RequestClient request = _context.RequestClients.FirstOrDefault(r => r.RequestId == requestId);
            Request req = _context.Requests.FirstOrDefault(r => r.RequestId == requestId);
            List<RequestWiseFile> fileList = _context.RequestWiseFiles.Where(reqFile => reqFile.RequestId == requestId && reqFile.IsDeleted == new BitArray(1)).ToList();

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
                    IsDeleted = new BitArray(1)
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }

            return ViewUploads(viewdata.RequestId);
        }
        //--------------Delete Files---------------------------
        public IActionResult DeleteFile(int requestid, int reqwisefileid)
        {
            _adminservice.DeleteFile(requestid, reqwisefileid);
            _notyf.Success("File Deleted Successfully");
            return RedirectToAction("ViewUploads", new { requestId = requestid });
        }
        //--------------Send Orders--------------------------
        public IActionResult SendOrders()
        {
            ViewBag.Professions = _adminservice.Professions();
            return View();
        }
        //--------------Vendors by profession -----------------
        public IActionResult VendorByProfession(int Professionid)
        {
            var v = _adminservice.VendorByProfession(Professionid);
            return Json(v);
        }
        //--------------Send Orders----------------------------
        public IActionResult SendOrdersData(int selectedValue)
        {
            var v = _adminservice.SendOrdersInfo(selectedValue);

            return Json(v);
        }
        [HttpPost]
        public IActionResult SendOrders(int ReqId, OrderDetail o)
        {
            var v = _adminservice.SendOrders(ReqId, o);
            return RedirectToAction("Index", "Admin");
        }
        //--------------Clear Case---------------------------
        [HttpPost]
        public IActionResult ClearCase(int RequestId)
        {
            _adminservice.ClearCase(RequestId);
            _notyf.Success("Case Cleared Successfully");
            return RedirectToAction("Index", "Admin");
        }
        //--------------Transfer Case-----------------------------
        [HttpPost]
        public IActionResult TransferCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminservice.TransferCaseInfo(RequestId, PhysicianId, Notes);
            _notyf.Success("Case Transferred Successfully");
            return RedirectToAction("Index", "Admin");
        }
        //--------------Send Agreement-----------------------------

        [HttpPost]
        public IActionResult SendAgreementModal(int Reqid)
        {
            Request obj = _context.Requests.FirstOrDefault(x => x.RequestId == Reqid);
            sendAgreement sendAgreement = new()
            {
                ReqId = Reqid,
                PhoneNumber = obj.PhoneNumber,
                Email = obj.Email
            };
            return View("SendAgreement", sendAgreement);
        }

        [HttpPost]
        public IActionResult SendAgreement(int Reqid, string PhoneNumber, string Email)
        {
            var agreementlink = Url.Action("ReviewAgreement", "Home" , new {Reqid=Reqid},Request.Scheme);
            sendAgreement sendAgreement = new()
            {
                ReqId = Reqid,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            _adminservice.SendAgreement(sendAgreement,agreementlink);
            _notyf.Success("Main Sent Successfully");

            return RedirectToAction("Index", "Admin");
        }

    }
}
