using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using HaloDocWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HaloDocRepository.Interface;
using HaloDocRepository.Repositories;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HaloDocWeb.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly HaloDocDbContext _context;
        private readonly IScheduling _scheduling;
        private readonly IAdminService _adminservice;
        private readonly INotyfService _notyf;


        public SchedulingController(HaloDocDbContext context, IScheduling scheduling, IAdminService adminservice, INotyfService notyf)
        {
            _context = context;
            _scheduling = scheduling;
            _adminservice = adminservice;
            _notyf = notyf;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.AssignCase = _adminservice.AssignCase();
            ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
            SchedulingData modal = new SchedulingData();
            return View("../Admin/Scheduling/Index", modal);

        }
        public IActionResult GetPhysicianByRegion(int regionid)
        {
            var v = _adminservice.ProviderbyRegion(regionid);
            return Json(v);
        }
        #region
        //public IActionResult Scheduling()
        //{
        //    ViewBag.Adminname = HttpContext.Session.GetString("Adminname");
        //    ViewBag.RegionCombobox = _adminFunction.RegionComboBox();
        //    ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
        //    SchedulingModel modal = new SchedulingModel();
        //    modal.regions = _context.Regions.ToList();
        //    return View(modal);
        //}

        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.PhysicianRegions.Include(u => u.Physician).Where(u => u.RegionId == regionid).Select(u => u.Physician).ToList();
            if (regionid == 0)
            {
                physician = _context.Physicians.ToList();
            }

            switch (PartialName)
            {

                case "_DayWise":
                    DayWiseScheduling day = new DayWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ToList()
                    };
                    return PartialView("../Admin/Scheduling/_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).ToList()
                    };
                    return PartialView("../Admin/Scheduling/_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                        shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).ToList()
                    };
                    return PartialView("../Admin/Scheduling/_MonthWise", month);

                default:
                    return PartialView("../Admin/Scheduling/_DayWise");
            }
        }


        public IActionResult AddShift(SchedulingData model)
        {
            string adminId = CV.Id();
            var chk = Request.Form["repeatdays"].ToList();
            _scheduling.AddShift(model, chk);
            _notyf.Success("Shift Added Sucessfully");
            return RedirectToAction("Index");

        }



        public SchedulingData viewshift(int shiftdetailid)
        {
            SchedulingData modal = new SchedulingData();
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == shiftdetailid);

            if (shiftdetail != null)
            {
                _context.Entry(shiftdetail)
                    .Reference(s => s.Shift)
                    .Query()
                    .Include(s => s.Physician)
                    .Load();
            }
            modal.regionid = (int)shiftdetail.RegionId;
            modal.physicianname = shiftdetail.Shift.Physician.FirstName + " " + shiftdetail.Shift.Physician.LastName;
            modal.modaldate = shiftdetail.ShiftDate.ToString("yyyy-MM-dd");
            modal.starttime = shiftdetail.StartTime;
            modal.endtime = shiftdetail.EndTime;
            modal.shiftdetailid = shiftdetailid;
            return modal;
        }
        public IActionResult ViewShiftreturn(SchedulingData modal)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        #endregion
        #region editshiftsave
        public void EditShiftSave(SchedulingData modal)
        {
            _scheduling.EditShiftSave(modal, CV.Id());
            _notyf.Success("Shift Updated Sucessfully");


        }
        #endregion
        #region editshiftdelete
        public IActionResult EditShiftDelete(SchedulingData modal)
        {
            _scheduling.EditShiftDelete(modal, CV.Id());
            _notyf.Success("Shift Deleted Sucessfully");

            return RedirectToAction("Index");
        }
        #endregion

        #region Provider_on_call
        public IActionResult MDSOnCall(int? regionId)
        {
            ViewBag.AssignCase =  _adminservice.AssignCase();
            List<ProviderList> v =  _scheduling.PhysicianOnCall(regionId);
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../Admin/Scheduling/MDSOnCall", v);
        }
        #endregion
    }
}
