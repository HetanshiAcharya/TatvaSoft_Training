using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using HaloDocDataAccess.DataContext;
using HaloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;

namespace HaloDocRepository.Repositories
{
    public class Scheduling : IScheduling
    {
        private readonly HaloDocDbContext _context;

        public Scheduling(HaloDocDbContext context)
        {
            _context = context;
        }
        #region Scheduling

        public void AddShift(SchedulingData model, List<string?>? chk)
        {


            var shiftid = _context.Shifts.Where(u => u.PhysicianId == model.physicianid).Select(u => u.ShiftId).ToList();
            if (shiftid.Count() > 0)
            {
                foreach (var obj in shiftid)
                {
                    var shiftdetailchk = _context.ShiftDetails.Where(u => u.ShiftId == obj && u.ShiftDate == model.shiftdate).ToList();
                    if (shiftdetailchk.Count() > 0)
                    {
                        foreach (var item in shiftdetailchk)
                        {
                            if ((model.starttime >= item.StartTime && model.starttime <= item.EndTime) || (model.endtime >= item.StartTime && model.endtime <= item.EndTime))
                            {
                                //TempData["error"] = "Shift is already assigned in this time";
                                return;
                            }
                        }
                    }
                }
            }
            Shift shift = new Shift
            {
                PhysicianId = model.physicianid,
                StartDate = DateOnly.FromDateTime(model.shiftdate),
                RepeatUpto = model.repeatcount,
                CreatedDate = DateTime.Now,
                CreatedBy = "0d15d42d-2f13-4d03-bc8a-2c57c34969ac",
            };
            foreach (var obj in chk)
            {
                shift.WeekDays += obj;
            }
            if (model.repeatcount > 0)
            {
                shift.IsRepeat = new BitArray(new[] { true });
            }
            else
            {
                shift.IsRepeat = new BitArray(new[] { false });
            }
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            DateTime curdate = model.shiftdate;
            ShiftDetail shiftdetail = new ShiftDetail();
            shiftdetail.ShiftId = shift.ShiftId;
            shiftdetail.ShiftDate = curdate;
            shiftdetail.RegionId = model.regionid;
            shiftdetail.StartTime = model.starttime;
            shiftdetail.EndTime = model.endtime;

            shiftdetail.IsDeleted = new BitArray(new[] { false });
            _context.ShiftDetails.Add(shiftdetail);
            _context.SaveChanges();
            ShiftDetailRegion shiftregionnews = new ShiftDetailRegion
            {
                ShiftDetailId = shiftdetail.ShiftDetailId,
                RegionId = model.regionid,
                IsDeleted = new BitArray(new[] { false })
            };
            _context.ShiftDetailRegions.Add(shiftregionnews);
            _context.SaveChanges();
            var dayofweek = model.shiftdate.DayOfWeek.ToString();
            int valueforweek;
            if (dayofweek == "Sunday")
            {
                valueforweek = 0;
            }
            else if (dayofweek == "Monday")
            {
                valueforweek = 1;
            }
            else if (dayofweek == "Tuesday")
            {
                valueforweek = 2;
            }
            else if (dayofweek == "Wednesday")
            {
                valueforweek = 3;
            }
            else if (dayofweek == "Thursday")
            {
                valueforweek = 4;
            }
            else if (dayofweek == "Friday")
            {
                valueforweek = 5;
            }
            else
            {
                valueforweek = 6;
            }

            if (shift.IsRepeat[0] == true)
            {
                for (int j = 0; j < shift.WeekDays.Count(); j++)
                {
                    var z = shift.WeekDays;
                    var p = shift.WeekDays.ElementAt(j).ToString();
                    int ele = Int32.Parse(p);
                    int x;
                    if (valueforweek > ele)
                    {
                        x = 6 - valueforweek + 1 + ele;
                    }
                    else
                    {
                        x = ele - valueforweek;
                    }
                    if (x == 0)
                    {
                        x = 7;
                    }
                    DateTime newcurdate = model.shiftdate.AddDays(x);
                    for (int i = 0; i < model.repeatcount; i++)
                    {
                        ShiftDetail shiftdetailnew = new ShiftDetail
                        {
                            ShiftId = shift.ShiftId,
                            ShiftDate = newcurdate,
                            RegionId = model.regionid,
                            StartTime = model.starttime,
                            EndTime = model.endtime,

                            IsDeleted = new BitArray(new[] { false })
                        };
                        _context.ShiftDetails.Add(shiftdetailnew);
                        _context.SaveChanges();
                        ShiftDetailRegion shiftregionnew = new ShiftDetailRegion
                        {
                            ShiftDetailId = shiftdetailnew.ShiftDetailId,
                            RegionId = model.regionid,
                            IsDeleted = new BitArray(new[] { false })

                        };
                        _context.ShiftDetailRegions.Add(shiftregionnew);
                        _context.SaveChanges();
                        newcurdate = newcurdate.AddDays(7);
                    }
                }

            }
        }

        public void ViewShift(int shiftdetailid)
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

        }

        public void ViewShiftreturn(SchedulingData modal)
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
        }


        #endregion
        #region EditShiftSave
        public bool EditShiftSave(SchedulingData modal, string id)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail != null)
            {
                shiftdetail.ShiftDate = modal.shiftdate;
                shiftdetail.StartTime = modal.starttime;
                shiftdetail.EndTime = modal.endtime;
                shiftdetail.ModifiedBy = id;
                shiftdetail.ModifiedDate = DateTime.Now;
                _context.ShiftDetails.Update(shiftdetail);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion

        #region editshiftdelete
        public bool EditShiftDelete(SchedulingData modal, string id)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            var shiftdetailRegion = _context.ShiftDetailRegions.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            string adminname = id;

            shiftdetail.IsDeleted = new BitArray(new[] { true });
            shiftdetail.ModifiedDate = DateTime.Now;
            shiftdetail.ModifiedBy = adminname;
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();

            shiftdetailRegion.IsDeleted = new BitArray(new[] { true });
            _context.ShiftDetailRegions.Update(shiftdetailRegion);
            _context.SaveChanges();

            return true;
        }
        #endregion

        #region PhysicianOnCall
        public List<ProviderList> PhysicianOnCall(int? region)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTimeOfDay = TimeOnly.FromDateTime(DateTime.Now);

            List<ProviderList> pl = (from r in _context.Physicians
                                     where r.IsDeleted == new BitArray(1)
                                     select new ProviderList
                                     {
                                         CreatedDate = r.CreatedDate,
                                         PhysicianId = r.PhysicianId,
                                         Add1 = r.Address1,
                                         Add2 = r.Address2,
                                         Message = r.AdminNotes,
                                         PhoneForBill = r.AltPhone,
                                         Bname = r.BusinessName,
                                         Bwebsite = r.BusinessWebsite,
                                         City = r.City,
                                         FirstName = r.FirstName,
                                         LastName = r.LastName,
                                         Status = (Constant.ProviderStatus)r.Status,
                                         Email = r.Email,
                                     }).ToList();
            if (region != null)
            {
                pl = (
                        from pr in _context.PhysicianRegions

                        join ph in _context.Physicians
                            on pr.PhysicianId equals ph.PhysicianId into rGroup
                        from r in rGroup.DefaultIfEmpty()
                        where pr.RegionId == region && r.IsDeleted == new BitArray(1)
                        select new ProviderList
                        {
                            CreatedDate = r.CreatedDate,
                            PhysicianId = r.PhysicianId,
                            Add1 = r.Address1,
                            Add2 = r.Address2,
                            Message = r.AdminNotes,
                            PhoneForBill = r.AltPhone,
                            Bname = r.BusinessName,
                            Bwebsite = r.BusinessWebsite,
                            City = r.City,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            Status = (Constant.ProviderStatus)r.Status,
                            Email = r.Email,

                        })
                        .ToList();
            }

            foreach (var item in pl)
            {
                List<int> shiftIds = (from s in _context.Shifts
                                           where s.PhysicianId == item.PhysicianId
                                           select s.ShiftId).ToList();

                foreach (var shift in shiftIds)
                {
                    var shiftDetail = (from sd in _context.ShiftDetails
                                       where sd.ShiftId == shift &&
                                             sd.ShiftDate.Date == currentDateTime.Date &&
                                             sd.StartTime <= currentTimeOfDay &&
                                             currentTimeOfDay <= sd.EndTime
                                       select sd).FirstOrDefault();

                    if (shiftDetail != null)
                    {
                        item.onCallStatus = 1;
                    }
                }
            }

            return pl;


        }
        #endregion
    }
}