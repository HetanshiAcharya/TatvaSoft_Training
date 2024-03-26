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
        public IActionResult Index()
        {
            ViewBag.CancelCase = _adminservice.CancelCase();
            ViewBag.AssignCase = _adminservice.AssignCase();
            var obj = _adminservice.ProviderMenu();
            return View("../Admin/Provider/Index", obj);
        }
    }
}
