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
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Reflection.Metadata;


namespace HaloDocWeb.Controllers
{
    public class AdminProfileController : Controller
    {
        private readonly HaloDocDbContext _context;
        private readonly IPatientService _patientService;
        private readonly IAuthService _authservice;
        private readonly IAdminService _adminservice;
        private readonly ILoginRepository _loginRepository;
        private readonly IJwtService _jwtService;
        private readonly INotyfService _notyf;

        public AdminProfileController(HaloDocDbContext context, IPatientService patientService, IAuthService authService, IAdminService adminservice, ILoginRepository loginRepository, IJwtService jwtService, INotyfService notyf)
        {
            _context = context;
            _patientService = patientService;
            _authservice = authService;
            _adminservice = adminservice;
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        public async Task<IActionResult> Index(string UserId)
        {
            ViewBag.AssignCase = _adminservice.AssignCase();
            if (UserId == null)
            {
                UserId = CV.UserId().ToString();
            }
            AdminDetailsInfo p = await _adminservice.GetProfileDetails(Convert.ToInt32(UserId));
            return View("../Admin/AdminProfile/Index", p);
        }
        #region EditPassword
        public async Task<IActionResult> EditPassword(string password, int adminId)
        {
            if (await _adminservice.EditPassword(password, adminId))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("Index", "AdminProfile");
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
            return RedirectToAction("Index", "AdminProfile");
        }
        #endregion

        #region BillingInfoEdit
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
            return RedirectToAction("Index", "AdminProfile");
        }
        #endregion
        #region AddAdminPost
        public IActionResult AddAdminPost(AdminProfile adminData)
        {
            bool res = _adminservice.AddAdminAccount(adminData);
            if (res)
            {
                _notyf.Success("Admin Added Successfully");

                return RedirectToAction("AddAdmin", "Admin");

            }
            else
            {
                _notyf.Error("Admin already exist");

                return View("AddAdmin", "AdminProfile");
            }
        }
        #endregion
        #region AddAdmin
        public IActionResult AddAdmin()
        {
            ViewBag.Role = _adminservice.ProviderRoleAdmin();
            ViewBag.AssignCase = _adminservice.AssignCase();
            return View("../Admin/AdminProfile/AddAdmin");
        }
        #endregion
    }
}
