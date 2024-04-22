using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using HaloDocRepository.Repositories;
using HaloDocWeb.Models;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using iText.Layout;
using Document = iText.Layout.Document;

namespace HaloDocWeb.Controllers
{
    public class ProviderController : Controller
    {
        #region Constructor
        private readonly IProviderService _providerservice;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminservice;


        public ProviderController(IProviderService IProviderRepository, INotyfService notyf, IAdminService adminservice)
        {
            _providerservice = IProviderRepository;
            _notyf = notyf;
            _adminservice = adminservice;
        }
        #endregion

        #region Index
        public IActionResult Index(int pageinfo = 1, int Region = -1)
        {
            ViewBag.CancelCase = _adminservice.CancelCase();
            ViewBag.AssignCase = _adminservice.AssignCase();
            var obj = _adminservice.ProviderMenu(Region, pageinfo);
            return View("../Admin/Provider/Index", obj);
        }
        #endregion

        #region channgenoti
        public IActionResult changeNoti(int[] files, int region = -1)
        {
            bool res = _adminservice.ChangeNoti(files, region);
            if (res == true)
            {
                _notyf.Success("Information Changed Successfully");
            }
            else
            {
                _notyf.Error("Information not changed");

            }
            return RedirectToAction("Index");
        }
        #endregion

        #region sendEmail
        public IActionResult SendEmailProvider(string Email, string Message, int radio)
        {
            bool result = false;
            bool sms = false;
            if (radio == 1)
            {
                sms = _adminservice.SendMessage(Message);
            }
            else if (radio == 2)
            {
                result = _adminservice.SendEmailProvider(Email, Message);
            }
            else
            {
                result = _adminservice.SendEmailProvider(Email, Message);
                sms = _adminservice.SendMessage(Message);
            }
            if (result)
            {
                _notyf.Success("Email sent Successfully.");
            }
            if (sms)
            {
                _notyf.Success("Message sent Successfully.");
            }

            return RedirectToAction("Index");

        }
        #endregion

        #region addphysician
        public IActionResult AddPhysician()
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Add";
            return View("../Admin/Provider/AddPhysician");
        }
        #endregion

        #region AddPhysicianPost
        public IActionResult AddPhysicianPost(ProviderList PhysiciansData, int[] checkboxes, string UserId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Add";
            var res = _adminservice.AddProviderAccount(PhysiciansData, checkboxes, UserId);
            if (res)
            {
                _notyf.Success("Physician Added Successfully");
                return View("../Admin/Provider/AddPhysician");

            }
            else
            {
                _notyf.Error("Physician already exist");
                return View("../Admin/Provider/AddPhysician");

            }
        }
        #endregion

        #region editphysician
        public IActionResult EditPhysician(int pId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Edit Physician Account";
            var res = _adminservice.GetProviderProfileDetails(pId);
            return View("../Admin/Provider/EditPhysician", res);
        }
        #endregion

        #region EditProviderAccInfo
        [HttpPost]
        public async Task<IActionResult> EditProviderAccInfo(ProviderList p)
        {
            if (await _adminservice.EditProviderAccInfo(p))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditPhysician", "Provider", new { pId = p.PhysicianId });
        }
        #endregion

        #region EditProviderInfo
        [HttpPost]
        public async Task<IActionResult> EditProviderInfo(ProviderList p)
        {
            if (await _adminservice.EditProviderInfo(p))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditPhysician", "Provider", new { pId = p.PhysicianId });
        }
        #endregion

        #region EditProviderMailingInfo
        [HttpPost]
        public async Task<IActionResult> EditProviderMailingInfo(ProviderList p)
        {
            if (await _adminservice.EditProviderMailingInfo(p))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditPhysician", "Provider", new { pId = p.PhysicianId });
        }
        #endregion

        #region ProviderProfileInfo
        [HttpPost]
        public async Task<IActionResult> ProviderProfileInfo(ProviderList p)
        {
            if (await _adminservice.ProviderProfileInfo(p))
            {
                _notyf.Success("Data changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditPhysician", "Provider", new { pId = p.PhysicianId });
        }
        #endregion

        #region SaveProvider
        [HttpPost]
        public async Task<IActionResult> SaveProvider(int[] checkboxes, int physicianid)
        {
            bool res = _adminservice.SaveProvider(checkboxes, physicianid);
            _notyf.Success("Information changed Successfully...");
            return RedirectToAction("EditPhysician", "Provider", new { pId = physicianid });
        }
        #endregion

        #region DeleteProviderAccount
        [HttpPost]
        public IActionResult DeleteProviderAccount(int PhysicianId)
        {
            bool res = _adminservice.DeleteProvider(PhysicianId);
            _notyf.Success("Information changed Successfully...");
            return RedirectToAction("Index", "Provider");
        }
        #endregion

        #region ProviderProfile
        public IActionResult ProviderProfile(int pId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "My Profile";
            var res = _adminservice.GetProviderProfileDetails(pId);
            return View("../Admin/Provider/EditPhysician", res);
        }
        #endregion

        #region RequestToAdmin
        public IActionResult RequestToAdmin(string Notes)
        {
            int obj = Convert.ToInt32(CV.UserId());
            bool res = _adminservice.RequestToAdmin(obj, Notes);
            _notyf.Success("Mail Sent Successfully");
            return RedirectToAction("ProviderProfile", new { pId = obj });
        }
        #endregion

        #region isEncounterFinalize
        public IActionResult isEncounterFinalize(int RequestId)
        {
            var res = _adminservice.isEncounterFinalize(RequestId);
            return Json(res);
        }
        #endregion

        #region GeneratePdf
        public IActionResult GeneratePdf(int RequestId)
        {
            try
            {
                if (RequestId == 0 || RequestId < 0)
                {
                    throw new Exception("Invalid Request");
                }
                EncounterInfo model = _adminservice.Encounterinfo(RequestId);
                if (model == null) throw new Exception("Medical Report Not Exist For this Request");
                using (var ms = new MemoryStream())
                {
                    var writer = new PdfWriter(ms);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    // Add a title
                    var title = new Paragraph("Medical Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20);
                    document.Add(title);
                    // Add a table
                    var table = new iText.Layout.Element.Table(new float[] { 4, 6 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell("Property");
                    table.AddHeaderCell("Value");

                    // Add properties
                    table.AddCell("RequestId");
                    table.AddCell(model.RequestID.ToString());
                    table.AddCell("FirstName");
                    table.AddCell(model.FirstName);
                    table.AddCell("LastName");
                    table.AddCell(model.LastName);
                    table.AddCell("Location");
                    table.AddCell(model.Location ?? "");
                    table.AddCell("DateOfBirth");
                    table.AddCell(model.Bdate.ToString());
                    table.AddCell("DateOfService");
                    table.AddCell(model.CreatedDate.ToString());
                    table.AddCell("Mobile");
                    table.AddCell(model.PhoneNumber);
                    table.AddCell("Email");
                    table.AddCell(model.Email);
                    table.AddCell("HistoryOfPresentIllness");
                    table.AddCell(model.HistoryOfIllness ?? "");
                    table.AddCell("MedicalHistory");
                    table.AddCell(model.MedicalHist ?? "");
                    table.AddCell("Medication");
                    table.AddCell(model.Medications ?? "");
                    table.AddCell("Allergies");
                    table.AddCell(model.Allergies ?? "");
                    table.AddCell("Temprature");
                    table.AddCell(model.Temp ?? "");
                    table.AddCell("HeartRate");
                    table.AddCell(model.HR ?? "");
                    table.AddCell("RespiratoryRate");
                    table.AddCell(model.RR ?? "");
                    table.AddCell("BloodPressureDiastolic");
                    table.AddCell(model.BPD ?? "");
                    table.AddCell("BloodPressureSystolic");
                    table.AddCell(model.BPS ?? "");
                    table.AddCell("O2Level");
                    table.AddCell(model.O2 ?? "");
                    table.AddCell("Pain");
                    table.AddCell(model.Pain ?? "");
                    table.AddCell("HEENT");
                    table.AddCell(model.heent ?? "");
                    table.AddCell("CvReading");
                    table.AddCell(model.CV ?? "");
                    table.AddCell("Chest");
                    table.AddCell(model.Chest ?? "");
                    table.AddCell("ABD");
                    table.AddCell(model.ABD ?? "");
                    table.AddCell("Extr");
                    table.AddCell(model.Extr ?? "");
                    table.AddCell("Skin");
                    table.AddCell(model.Skin ?? "");
                    table.AddCell("Neuro");
                    table.AddCell(model.Neuro ?? "");
                    table.AddCell("Other");
                    table.AddCell(model.Other ?? "");
                    table.AddCell("Diagnosis");
                    table.AddCell(model.Diagnosis ?? "");
                    table.AddCell("TreatmentPlan");
                    table.AddCell(model.TrtPlan ?? "");
                    table.AddCell("MedicationDispensed");
                    table.AddCell(model.MedDispensed ?? "");
                    table.AddCell("Procedures");
                    table.AddCell(model.Procedures ?? "");
                    table.AddCell("FollowUp");
                    table.AddCell(model.Followup ?? "");
                    document.Add(table);

                    // Close the document
                    document.Close();

                    // Return the PDF as a file
                    byte[] pdfBytes = ms.ToArray();
                    string filename = "Medical-Report-" + RequestId + DateTime.Now.ToString("_dd-MM-yyyy-hh-mm-ss-fff") + ".pdf";
                    
                    return File(pdfBytes, "application/pdf", filename);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message })
                {
                    
                    StatusCode = 500
                };
            }
        }
        #endregion
    }
}
