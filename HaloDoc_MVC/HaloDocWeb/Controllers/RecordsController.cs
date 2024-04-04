using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.DataContext;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocWeb.Controllers
{
    public class RecordsController : Controller
    {
        #region Constructor
        private readonly IProviderService _providerservice;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminservice;
        private readonly IPatientService _patientservice;

        private readonly HaloDocDbContext _context;


        public RecordsController(IProviderService IProviderRepository, INotyfService notyf, IAdminService adminservice, HaloDocDbContext context, IPatientService patientservice)
        {
            _providerservice = IProviderRepository;
            _notyf = notyf;
            _adminservice = adminservice;
            _context = context;
            _patientservice = patientservice;
        }
        #endregion
        public IActionResult PatientHistory(string firstname, string lastname, string email, string phone, string address)
        {
            var res = _patientservice.PatientHistory(firstname, lastname, email,phone,address);
            return View("../Admin/Records/PatientHistory",res);
        }
    }
}
