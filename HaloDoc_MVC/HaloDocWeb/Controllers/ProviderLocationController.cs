using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocRepository.Interface;
using HaloDocRepository.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocWeb.Controllers.Admin
{
    public class ProviderLocationController : Controller
    {
        #region Constructor
        private readonly IProviderService _providerservice;
        private readonly INotyfService _notyf;

        public ProviderLocationController(IProviderService IProviderRepository, INotyfService notyf)
        {
            _providerservice = IProviderRepository;
            _notyf = notyf;
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            ViewBag.Log =  _providerservice.FindPhysicianLocation();
            return View("../Admin/ProviderLocation/Index");
        }
    }
}