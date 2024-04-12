using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.ViewModels;
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
        public IActionResult PatientHistory(SearchInputs search)
        {
            var res = _patientservice.PatientHistory(search);
            return View("../Admin/Records/PatientHistory",res);
        }
        public IActionResult RecordsPatientExplore(int UserId)
        {
            var res = _adminservice.RecordsPatientExplore(UserId);
            return View("../Admin/Records/RecordsPatientExplore", res);
        }
        public IActionResult RecordsBlock(BlockHistory Formdata)
        {
            var res = _adminservice.RecordsBlock(Formdata);
            return View("../Admin/Records/RecordsBlock",res);
        }
        public IActionResult UnBlock(int reqId)
        {
            bool res = _adminservice.UnBlock(reqId);
            if (res)
            {
                _notyf.Success("Request UnBlocked Successfully");

            }
            return RedirectToAction("RecordsBlock");
        }
        public IActionResult RecordsSearch(SearchInputs search)
        {
            var res = _adminservice.RecordsSearch(search);
            return View("../Admin/Records/RecordsSearch", res);
        }
        public IActionResult RecordsDelete(int reqId)
        {
            bool var = _adminservice.RecordsDelete(reqId);
            if (var)
            {
                _notyf.Success("Record deleted successfully");
                
            }
            return RedirectToAction("RecordsSearch");
        }

        public IActionResult EmailLogs(SearchInputs emaillog)
        {
            var res = _adminservice.EmailLogs(emaillog);
            return View("../Admin/Records/EmailLogs",res);
        }
        public IActionResult RecordsSMSLog(SearchInputs search)
        {
            var res = _adminservice.RecordsSMSLog(search);
            return View("../Admin/Records/RecordsSMSLog",res);
        }
    }
}
