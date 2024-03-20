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


            PaginatedViewModel sm = _adminservice.Indexdata();

            return View(sm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(PaginatedViewModel data)
        {
            if (data.Status == null)
            {
                data.Status = CV.CurrentStatus();

            }

            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", data.Status);
            PaginatedViewModel contacts = _adminservice.GetRequests(data);

            switch (data.Status)
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
        //--------------Error-----------------
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
            _notyf.Success("Case Assigned Successfully");
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult CancelCase(int casetagId, int RequestId, string Notes)
        {

            _adminservice.CancelCaseInfo(casetagId, Notes, RequestId);
            _notyf.Success("Case Cancelled Successfully");
            return RedirectToAction("Index", "Admin");
            
        }
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            var res = _adminservice.BlockCaseInfo(RequestId, Notes);
            _notyf.Success("Case Blocked Successfully");
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ViewNotes(int reqClientId)
        {
            _ = HttpContext.Session.GetInt32("adminId");
            var obj = _adminservice.ViewNotes(reqClientId);
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewNotes(ViewNotes viewNotes)
        {
            if (ModelState.IsValid)
            {
                int? adminId = HttpContext.Session.GetInt32("adminId");
                _adminservice.ViewNotesUpdate(viewNotes);
                _notyf.Success("Note Updated Successfully", 3);
                return ViewNotes(viewNotes.Requestclientid);
            }
            return View(viewNotes);
        }
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
        public IActionResult DeleteFile(int requestid, int reqwisefileid)
        {
            _adminservice.DeleteFile(requestid, reqwisefileid);
            _notyf.Success("File Deleted Successfully");
            return RedirectToAction("ViewUploads", new { requestId = requestid });
        }
        public IActionResult SendOrders()
        {
            ViewBag.Professions = _adminservice.Professions();
            return View();
        }
        public IActionResult VendorByProfession(int Professionid)
        {
            var v = _adminservice.VendorByProfession(Professionid);
            return Json(v);
        }
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
        [HttpPost]
        public IActionResult ClearCase(int RequestId)
        {
            _adminservice.ClearCase(RequestId);
            _notyf.Success("Case Cleared Successfully");
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult TransferCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminservice.TransferCaseInfo(RequestId, PhysicianId, Notes);
            _notyf.Success("Case Transferred Successfully");
            return RedirectToAction("Index", "Admin");
        }
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
        public IActionResult infoByregion(int Regionid)
        {

            var v = _adminservice.InfoByRegion(Regionid);
            return Json(v);
        }
        #region CloseCase
        public async Task<IActionResult> CloseCase(int RequestID)
        {
            ViewCloseCaseModel vc = _adminservice.CloseCaseData(RequestID);
            return View("CloseCase", vc);
        }
        public IActionResult CloseCaseUnpaid(int id)
        {
            bool sm = _adminservice.CloseCase(id);
            if (sm)
            {
                _notyf.Success("Case Closed...");
                _notyf.Information("You can see Closed case in unpaid State...");

            }
            else
            {
                _notyf.Error("there is some error in CloseCase...");
            }
            return RedirectToAction("Index", "Admin");
        }
        #endregion
        public IActionResult Encounter(int?RId)
        {
            EncounterInfo? ei = _adminservice.Encounterinfo(RId);
            return View(ei);
        }
        [HttpPost]
        public IActionResult EncounterPost(EncounterInfo _viewencounterinfo)
        {
            if (ModelState.IsValid)
            {
                _adminservice.EncounterinfoPost(_viewencounterinfo);
                return RedirectToAction("Index", "Admin");

            }
            return View(_viewencounterinfo);
        }
        public async Task<IActionResult> AdminProfile()
        {
            ViewBag.AssignCase = _adminservice.AssignCase();

            AdminDetailsInfo p = await  _adminservice.GetProfileDetails(Convert.ToInt32(CV.UserId()));
            return View("../Admin/AdminProfile", p);
        }
        #region EditPassword
        public async Task<IActionResult> EditPassword(string password)
        {
            if (await _adminservice.EditPassword(password, Convert.ToInt32(CV.UserId())))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("AdminProfile", "Admin");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> EditAdministratorInfo(AdminDetailsInfo _viewAdminProfile)
        {
            if (await _adminservice.EditAdministratorInfo(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("AdminProfile", "Admin");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> BillingInfoEdit(AdminDetailsInfo _viewAdminProfile)
        {
            if (await _adminservice.BillingInfoEdit(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("AdminProfile","Admin");
        }
        #endregion
    }
}

