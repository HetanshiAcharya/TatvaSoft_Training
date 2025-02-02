﻿using HaloDocDataAccess.DataContext;
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
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Reflection.Metadata;
using Org.BouncyCastle.Utilities;
using System.Text;
using System.Security.Cryptography;

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

        public AdminController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice, ILoginRepository loginRepository, IJwtService jwtService, INotyfService notyf)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        #region GenerateSHA256
        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        #endregion
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
            aspNetUser.PasswordHash = GenerateSHA256(aspNetUser.PasswordHash);
            
            UserInfo u = await _loginRepository.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _jwtService.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Status", "1");
                if (u.Role == "Patient")
                {
                    return RedirectToAction("Dashboard", "PatientDashboard");
                }
                else if (u.Role == "Physician")
                {
                    return Redirect("../Admin/Index");
                }
                return Redirect("../Admin/Index");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../Admin/IndexPlatformLogin");
            }
        }
        public IActionResult IndexForgotPass()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexForgotPass(PatientForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                _adminservice.IndexForgotPass(model);
                //_notyf.Success("Mail Sent Successfully");
                return View();

            }
            else
            {
                //_notyf.Error("Mail Sent Failed");
                return View();

            }
        }
        //GET
        public IActionResult ResetPassAdmin(PatientResetPassword model)
        {
            return View(model);
        }
        //POST
        [HttpPost, ActionName("ResetPassAdmin")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(PatientResetPassword patientresetpass)
        {
            if (ModelState.IsValid)
            {
                _authservice.ResetPassword(patientresetpass);
                return RedirectToAction("IndexPlatformLogin", "Admin");
            }
            return View(patientresetpass);
        }
        [CheckAccess("Admin,Physician")]
        public IActionResult Index()
        {

            ViewBag.CancelCase = _adminservice.CancelCase();
            ViewBag.AssignCase = _adminservice.AssignCase();
            PaginatedViewModel sm = _adminservice.Indexdata(-1);
            if (CV.role() == "Physician")
            {
                sm = _adminservice.Indexdata(Convert.ToInt32(CV.UserId()));
            }


            return View("../Admin/Index", sm);
        }
        [HttpPost]
        public async Task<IActionResult> _SearchResult(PaginatedViewModel data, String Status)
        {
            if (Status == null)
            {
                Status = CV.CurrentStatus();

            }
            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);
             PaginatedViewModel contacts = _adminservice.GetRequests(data,Status);
            if (CV.role() == "Physician")
            {
                contacts = _adminservice.GetRequests(Status, data, Convert.ToInt32(CV.UserId()));
            }
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
            var adminId = CV.UserId();
            _adminservice.AssignCaseInfo(RequestId, PhysicianId, Notes, adminId);
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
        public IActionResult ViewNotes(int RequestId)
        {
            ViewNotes result = _adminservice.ViewNotes(RequestId);
            return View("../Admin/ViewNotes", result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _adminservice.ViewNotesUpdate(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    return RedirectToAction("ViewNotes", new { RequestId = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Not Updated");
                    return View("../Admin/ViewNotes");
                }
            }
            else
            {
                _notyf.Information("Please Select one of the note!!");
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }

        }
        public IActionResult ViewUploads(int requestId)
        {
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
            return View("../Admin/ViewUploads", document);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewDocument viewdata)
        {
            string UploadImage = "";
            var obj = _context.Requests.FirstOrDefault(x => x.RequestId == viewdata.RequestId);

            if (viewdata.File != null)
            {
                var fileName = Path.GetFileName(viewdata.File.FileName);
                string rootPath = "wwwroot\\Upload";
                string requestId = obj.RequestId.ToString();
                string userFolder = Path.Combine(rootPath, requestId);

                if (!Directory.Exists(userFolder))
                    Directory.CreateDirectory(userFolder);
                string fileNameWithPath = Path.Combine(userFolder, viewdata.File.FileName);
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
        public IActionResult DeleteAllFile(int requestid, int[] files)
        {
            _adminservice.DeleteFile(requestid, files);
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
            _notyf.Success("Order Sent Successfully...");

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
        public IActionResult SendAgreement(int ReqId, string PhoneNumber, string Email)
        {
            var agreementlink = Url.Action("ReviewAgreement", "Home", new { ReqId = ReqId }, Request.Scheme);
            sendAgreement sendAgreement = new()
            {
                ReqId = ReqId,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            _adminservice.SendAgreement(sendAgreement);
            _notyf.Success("Mail Sent Successfully");

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
        public IActionResult Encounter(int RId)
        {
            EncounterInfo ei = _adminservice.Encounterinfo(RId);
            return View(ei);
        }
        #region encounterpost
        [HttpPost]
        public IActionResult Encounter(EncounterInfo _viewencounterinfo)
        {
            if (ModelState.IsValid)
            {
                _adminservice.EncounterInfoPost(_viewencounterinfo);
                _notyf.Success("Saved Successfully...");

                return View();


            }
            return View(_viewencounterinfo);
        }
        #endregion

        #region encounterfinalize
        [HttpPost]
        public IActionResult EncounterFinalize(EncounterInfo _viewencounterinfo)
        {
            if (ModelState.IsValid)
            {
                _adminservice.EncounterFinalize(_viewencounterinfo);
                _notyf.Success("Finalized Successfully...");
                return View();

            }
            else
            {
                return RedirectToAction("Encounter", "Admin", _viewencounterinfo.RequestID);

            }

        }
        #endregion
        [HttpPost]
        public IActionResult SendAgreementModalFromUploads(int Reqid)
        {
            Request obj = _context.Requests.FirstOrDefault(x => x.RequestId == Reqid);
            sendAgreement sendAgreement = new()
            {
                ReqId = Reqid,
                PhoneNumber = obj.PhoneNumber,
                Email = obj.Email
            };
            return View("SendEmailFromUploads", sendAgreement);
        }
        [HttpPost]
        public IActionResult SendEmailFromUploads(int Reqid, string PhoneNumber, string Email)
        {
            var agreementlink = Url.Action("ReviewAgreement", "Home", new { Reqid = Reqid }, Request.Scheme);
            sendAgreement sendAgreement = new()
            {
                ReqId = Reqid,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            _adminservice.SendAgreement(sendAgreement);
            _notyf.Success("Mail Sent Successfully");

            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult _SendLink(string Email)
        {
            sendAgreement sendAgreement = new()
            {
                Email = Email,
                ReqId = _context.Requests.Where(req => req.Email == Email).Select(req => req.RequestId).FirstOrDefault()
            };
            _adminservice.SendLink(sendAgreement);
            _notyf.Success("Mail Sent Successfully");

            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Export(string status)
        {
            var requestData = _adminservice.Export(status);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].Bdate;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeID;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestID;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        public IActionResult AcceptCase(int RequestId, string Notes)
        {
            string PhysicianId = CV.UserId();
            _adminservice.AcceptCase(RequestId, PhysicianId, Notes);
            _notyf.Success("Request Accepted Successfully");
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult RejectCase(int RequestId, string Notes)
        {
            _adminservice.RejectCase(RequestId, Notes);
            _notyf.Success("Request Rejected Successfully");
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult HouseCall(int RequestId)
        {
            _adminservice.HouseCall(RequestId);
            _notyf.Success("Request Rejected Successfully");
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Consult(int RequestId)
        {
            _adminservice.Consult(RequestId);
            _notyf.Success("Request Rejected Successfully");
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ConcludeCare(int RequestID)
        {
            RequestClient request = _context.RequestClients.FirstOrDefault(r => r.RequestId == RequestID);
            Request req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestID);
            List<RequestWiseFile> fileList = _context.RequestWiseFiles.Where(reqFile => reqFile.RequestId == RequestID && reqFile.IsDeleted == new BitArray(1)).ToList();

            ViewDocument document = new()
            {
                requestwisefiles = fileList,
                RequestId = RequestID,
                ConfirmationNumber = req.ConfirmationNumber,
                UserName = request.FirstName + " " + request.LastName,
            };
            return View("ConcludeCare", document);
        }
        [HttpPost]
        public IActionResult ConcludeCarePost(int RequestID, string Notes)
        {
            bool sm = _adminservice.concludecare(RequestID,Notes);
            if (sm)
            {
                _notyf.Success("Case Concluded...");

            }
            else
            {
                _notyf.Error("First finalize the encounter form");
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult ConcludeCareViewUploads(ViewDocument viewdata)
        {
            string UploadImage = "";
            var obj = _context.Requests.FirstOrDefault(x => x.RequestId == viewdata.RequestId);

            if (viewdata.File != null)
            {
                var fileName = Path.GetFileName(viewdata.File.FileName);
                string rootPath = "wwwroot\\Upload";
                string requestId = obj.RequestId.ToString();
                string userFolder = Path.Combine(rootPath, requestId);

                if (!Directory.Exists(userFolder))
                    Directory.CreateDirectory(userFolder);
                string fileNameWithPath = Path.Combine(userFolder, viewdata.File.FileName);
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

            return ConcludeCare(viewdata.RequestId);
        }
        public IActionResult ConcludeDeleteFile(int requestid, int reqwisefileid)
        {
            _adminservice.DeleteFile(requestid, reqwisefileid);
            _notyf.Success("File Deleted Successfully");
            return RedirectToAction("ConcludeCare", new { requestId = requestid });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
    }
}

