using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.DataContext;
using HaloDocRepository.Interface;
using HaloDocWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HaloDocDataAccess.ViewModels;
using static HaloDocDataAccess.ViewModels.Constant;
using HaloDocDataAccess.DataModels;
using System.Collections;

namespace HaloDocWeb.Controllers
{
    public class AccessController : Controller
    {
        #region Constructor
        private readonly IProviderService _providerservice;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminservice;
        private readonly HaloDocDbContext _context;


        public AccessController(IProviderService IProviderRepository, INotyfService notyf, IAdminService adminservice, HaloDocDbContext context)
        {
            _providerservice = IProviderRepository;
            _notyf = notyf;
            _adminservice = adminservice;
            _context = context;
        }
        #endregion

        #region AccessIndex
        public IActionResult Index()
        {
            var res = _context.Roles.Where(r => r.IsDeleted == new BitArray(1)).ToList();
            return View("../Admin/Access/Index", res);
        }

        #endregion

        #region CreateRoleAccess
        public IActionResult CreateRoleAccess()
        {
            return View("../Admin/Access/CreateRoleAccess");

        }

        #endregion

        #region CreateRoleAccess
        [HttpPost]
        public IActionResult RolebyAccountType(AccountType Account)
        {
            var v = _adminservice.RolebyAccountType(Account);
            return Json(v);

        }

        #endregion

        #region SaveEditRole
        public IActionResult SaveEditRole(AccessModel roles)
        {
            var userId = (CV.UserId());
            var v = _adminservice.SaveEditRole(roles, userId);
            _notyf.Success("Role Created Successfully");
            ModelState.Clear();
            return View("../Admin/Access/CreateRoleAccess");
        }
        #endregion

        #region EditRoleAccess
        public IActionResult EditRoleAccess(int RoleId)
        {
            var res = _adminservice.ViewEditRole(RoleId);
            return View("../Admin/Access/EditRoleAccess", res);
        }
        #endregion

        #region EditRoleAccess
        public IActionResult SaveEditRoleAccess(AccessModel roles)
        {
            var res = _adminservice.SaveEditRoleAccess(roles);
            _notyf.Success("Role Edited Successfully");
            return RedirectToAction("EditRoleAccess", new { RoleId = roles.RoleId });
        }
        #endregion

        #region DeleteRole
        public IActionResult DeleteRole(int RoleId)
        {
            var res = _adminservice.DeleteRole(RoleId);
            _notyf.Success("Role Edited Successfully");
            return RedirectToAction("Index");
        }
        #endregion

        #region useraccess
        public IActionResult UserAccess(string AccountType)
        {
            ViewBag.role = _adminservice.Role();
            var res = _adminservice.UserAccessData(AccountType);
            return View("../Admin/Access/UserAccess", res);
        }
        #endregion
    }
}
