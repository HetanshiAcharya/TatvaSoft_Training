using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocRepository.Interface;
using HaloDocRepository.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        #region editphysician
        public IActionResult EditPhysician()
        {
            return View("../Admin/Provider/EditPhysician");
        }
        #endregion
    }
}
