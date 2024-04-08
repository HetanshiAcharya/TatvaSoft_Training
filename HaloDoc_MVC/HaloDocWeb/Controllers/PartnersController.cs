using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HaloDocWeb.Controllers
{
    public class PartnersController : Controller
    {
        #region Constructor
        private readonly IProviderService _providerservice;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminservice;
        private readonly HaloDocDbContext _context;

        public PartnersController(HaloDocDbContext context,IProviderService IProviderRepository, INotyfService notyf, IAdminService adminservice)
        {
            _providerservice = IProviderRepository;
            _notyf = notyf;
            _adminservice = adminservice;
            _context = context;
        }
        #endregion
        public IActionResult Index(string searchValue, int Profession)
        {
            ViewBag.Profession =  _context.HealthProfessionalTypes.ToList(); 
            List<Partners> data =  _adminservice.GetPartnersByProfession(searchValue, Profession);
            return View("../Admin/Partners/Index", data);       
        }
        
        public IActionResult AddEditBusiness(int VendorId)
        {
            ViewBag.Profession = _context.HealthProfessionalTypes.ToList();

            if (VendorId == 0)
            {
                ViewData["Heading"] = "Add";
                return View("../Admin/Partners/AddEditBusiness");

            }
            else
            {
                ViewData["Heading"] = "Update";
                var res=_adminservice.EditPartners(VendorId);
                return View("../Admin/Partners/AddEditBusiness", res);

            }
        }
        [HttpPost]
        public IActionResult AddEditBusiness(HealthProfessional obj)
        {
            var res = _adminservice.AddBusiness(obj);
            if (res)
            {
                _notyf.Success("Data Added Successfully");
            }
            return RedirectToAction("Index");

        }
        public IActionResult DeleteBusiness(int VendorId)
        {
            var res = _adminservice.DeleteBusiness(VendorId);
            if (res)
            {
                _notyf.Success("Vendor Deleted Successfully");
            }
            else
            {
                _notyf.Error("Vendor not deleted");
            }
            return RedirectToAction("Index");
        }
    }
}
