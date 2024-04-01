using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using HaloDocRepository.Repositories;
using HaloDocWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

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
        public IActionResult Index(int Region=-1)
        {
            ViewBag.CancelCase = _adminservice.CancelCase();
            ViewBag.AssignCase = _adminservice.AssignCase();
            var obj = _adminservice.ProviderMenu(Region);
            return View("../Admin/Provider/Index", obj);
        }
        #endregion

        #region channgenoti
        public IActionResult changeNoti(int[] files, int region=-1)
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
        public IActionResult SendEmailProvider(string Email, string Message)
        {
            bool res = _adminservice.SendEmailProvider(Email, Message);
            return RedirectToAction("Index");

        }
        #endregion

        #region addphysician
        public IActionResult AddPhysician(ProviderList PhysiciansData, int[] checkboxes, string UserId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Add";
            return View("../Admin/Provider/EditPhysician");
        }
        #endregion
        [HttpPost]
        #region addphysician
        public IActionResult AddPhysicianPost(ProviderList PhysiciansData, int[] checkboxes, string UserId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Add";
            var res = _adminservice.AddProviderAccount(PhysiciansData, checkboxes, UserId);
            _notyf.Success("Physician Added Successfully");
            return View("../Admin/Provider/EditPhysician");
        }
        #endregion

        #region editphysician
        public async Task<IActionResult> EditPhysician(int pId)
        {
            ViewBag.Status = _adminservice.ProviderRole();
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewData["Heading"] = "Edit";
            var res = await _adminservice.GetProviderProfileDetails(pId);
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
                _notyf.Success("Password changed Successfully...");
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
    }
}
