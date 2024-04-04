using HaloDocDataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IO;
using System.Reflection.Emit;


namespace HaloDocRepository.Repositories
{
    public class PatientService : IPatientService
    {
        private readonly HaloDocDbContext _context;
        private int IntDate;
        private int IntYear;

        public PatientService(HaloDocDbContext context)
        {
            _context = context;
        }
        //REQUEST SUBMIT BY PATIENT//
        public void PatientRequest(PatientSubmitRequests viewpatientcreaterequest)
        {
            var Aspnetuser = new AspNetUser();
            var AspNetUserRoles = new AspNetUserRole();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewpatientcreaterequest.Email);
            if (isexist == null)
            {
                // Aspnetuser
                Guid g = Guid.NewGuid();
                Aspnetuser.Id = g.ToString();
                Aspnetuser.UserName = (viewpatientcreaterequest.FirstName + ' ' + viewpatientcreaterequest.LastName);
                Aspnetuser.PasswordHash = viewpatientcreaterequest.LastName;
                Aspnetuser.PhoneNumber = viewpatientcreaterequest.PhoneNumber;
                Aspnetuser.CreatedDate = DateTime.Now;
                Aspnetuser.Email = viewpatientcreaterequest.Email;
                _context.AspNetUsers.Add(Aspnetuser);
                _context.SaveChanges();

                AspNetUserRoles.UserId = Aspnetuser.Id;
                AspNetUserRoles.RoleId = "1";
                _context.AspNetUserRoles.Add(AspNetUserRoles);
                _context.SaveChanges();

                User.AspNetUserId = Aspnetuser.Id;
                User.FirstName = viewpatientcreaterequest.FirstName;
                User.LastName = viewpatientcreaterequest.LastName;
                User.Email = viewpatientcreaterequest.Email;
                User.Street = viewpatientcreaterequest.Street;
                User.City = viewpatientcreaterequest.City;
                User.State = viewpatientcreaterequest.State;
                User.ZipCode = viewpatientcreaterequest.ZipCode;
                User.Mobile = viewpatientcreaterequest.PhoneNumber;
                User.StrMonth = (viewpatientcreaterequest.BirthDate.Month).ToString();
                User.IntDate = viewpatientcreaterequest.BirthDate.Day;
                User.IntYear = viewpatientcreaterequest.BirthDate.Year;
                User.CreatedBy = Aspnetuser.Id;
                User.CreatedDate = DateTime.Now;
                _context.Users.Add(User);
                _context.SaveChanges();
            }
            Request.RequestTypeId = 1;
            Request.Status = 1;

            if (isexist == null)
            {
                Request.UserId = User.UserId;
            }
            else
            {
                Request.UserId = isexist.UserId;
            }
            Request.FirstName = viewpatientcreaterequest.FirstName;
            Request.LastName = viewpatientcreaterequest.LastName;
            Request.Email = viewpatientcreaterequest.Email;
            Request.PhoneNumber = viewpatientcreaterequest.PhoneNumber;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.CreatedDate = DateTime.Now;
            Request.ConfirmationNumber = viewpatientcreaterequest.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewpatientcreaterequest.LastName.Substring(0, 2) + viewpatientcreaterequest.FirstName.Substring(0, 2) + "002";

            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewpatientcreaterequest.FirstName;
            Requestclient.Address = viewpatientcreaterequest.Street;
            Requestclient.LastName = viewpatientcreaterequest.LastName;
            Requestclient.Email = viewpatientcreaterequest.Email;
            Requestclient.PhoneNumber = viewpatientcreaterequest.PhoneNumber;
            Requestclient.Notes = viewpatientcreaterequest.Symptoms;
            Requestclient.StrMonth = (viewpatientcreaterequest.BirthDate.Month).ToString();
            Requestclient.IntDate = viewpatientcreaterequest.BirthDate.Day;
            Requestclient.IntYear = viewpatientcreaterequest.BirthDate.Year;
            Requestclient.Street = viewpatientcreaterequest.Street;
            Requestclient.City = viewpatientcreaterequest.City;
            Requestclient.State = viewpatientcreaterequest.State;
            Requestclient.ZipCode = viewpatientcreaterequest.ZipCode;
            Requestclient.StrMonth = (viewpatientcreaterequest.BirthDate.Month).ToString();
            Requestclient.IntDate = viewpatientcreaterequest.BirthDate.Day;
            Requestclient.IntYear = viewpatientcreaterequest.BirthDate.Year;

            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewpatientcreaterequest.UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewpatientcreaterequest.UploadFile.FileName);
                viewpatientcreaterequest.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewpatientcreaterequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewpatientcreaterequest.UploadFile.CopyTo(stream);
                }

                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewpatientcreaterequest.UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1)

                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
        //REQUEST SUBMIT BY FAMILY//


        public void FamilyRequest(FamilySubmitRequests viewdata)
        {
            var Request = new Request
            {
                RequestTypeId = 3,
                Status = 1,
                FirstName = viewdata.FF_FirstName,
                LastName = viewdata.FF_LastName,
                Email = viewdata.FF_Email,
                RelationName = viewdata.FF_RelationWithPatient,
                PhoneNumber = viewdata.FF_PhoneNumber,
                CreatedDate = DateTime.Now,
                IsUrgentEmailSent = new BitArray(1)

            };
            _context.Requests.Add(Request);
            _context.SaveChanges();

            var Requestclient = new RequestClient
            {
                Request = Request,
                RequestId = Request.RequestId,
                Notes = viewdata.Symptoms,
                FirstName = viewdata.FirstName,
                LastName = viewdata.LastName,
                PhoneNumber = viewdata.PhoneNumber,
                Email = viewdata.Email,
                Street = viewdata.Street,
                State = viewdata.State,
                City = viewdata.City,
                ZipCode = viewdata.ZipCode,
                StrMonth = (viewdata.BirthDate.Month).ToString(),
                IntDate = viewdata.BirthDate.Day,
                IntYear = viewdata.BirthDate.Year,
                Address = viewdata.Street + viewdata.City + viewdata.State + viewdata.ZipCode
            };
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();
            if (viewdata.UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
                viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewdata.UploadFile.CopyTo(stream);
                }

                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewdata.UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1)

                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }

        //REQUEST SUBMIT BY concierge//

        public void ConciergeRequest(ConciergeSubmitRequests viewdata)
        {
            var Concierge = new Concierge();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var Requestconcierge = new RequestConcierge();

            Concierge.ConciergeName = viewdata.CON_FirstName + " " + viewdata.CON_LastName;
            Concierge.Street = viewdata.CON_Street;
            Concierge.City = viewdata.CON_City;
            Concierge.State = viewdata.CON_State;
            Concierge.ZipCode = "38585";
            Concierge.RegionId = 1;
            Concierge.Address = Concierge.Street + " " + Concierge.City + " " + Concierge.State;
            Concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(Concierge);
            _context.SaveChanges();
            int id1 = Concierge.ConciergeId;


            Request.RequestTypeId = 4;
            Request.Status = 1;
            Request.FirstName = viewdata.FirstName;
            Request.LastName = viewdata.LastName;
            Request.Email = viewdata.Email;
            Request.PhoneNumber = viewdata.PhoneNumber;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.CreatedDate = DateTime.Now;


            _context.Requests.Add(Request);
            _context.SaveChanges();
            int id2 = Request.RequestId;

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewdata.FirstName;
            Requestclient.LastName = viewdata.LastName;
            Requestclient.Email = viewdata.Email;
            Requestclient.PhoneNumber = viewdata.PhoneNumber;
            Requestclient.Street = viewdata.Street;
            Requestclient.State = viewdata.State;
            Requestclient.City = viewdata.City;
            Requestclient.Notes = viewdata.Symptoms;
            Requestclient.ZipCode = viewdata.ZipCode;
            Requestclient.StrMonth = (viewdata.BirthDate.Month).ToString();
            Requestclient.IntDate = viewdata.BirthDate.Day;
            Requestclient.IntYear = viewdata.BirthDate.Year;
            Requestclient.Address = viewdata.Street + viewdata.City + viewdata.State + viewdata.ZipCode;

            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Requestconcierge.RequestId = id2;
            Requestconcierge.ConciergeId = id1;

            _context.RequestConcierges.Add(Requestconcierge);
            _context.SaveChanges();
            if (viewdata.UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
                viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewdata.UploadFile.CopyTo(stream);
                }

                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewdata.UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1)

                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }

        //REQUEST SUBMIT BY Business//
        public void BusinessRequest(BusinessSubmitRequests viewdata)
        {
            int requests = _context.Requests.Where(u => u.CreatedDate == DateTime.Now.Date).Count();
            var Business = new Business();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var Requestbusiness = new RequestBusiness();
            Random _random = new Random();

            Business.Name = viewdata.bus_FirstName + viewdata.bus_LastName;
            Business.CreatedBy = viewdata.bus_FirstName;
            Business.RegionId = 1;
            Business.CreatedDate = DateTime.Now;
            _context.Businesses.Add(Business);
            _context.SaveChanges();
            int id1 = Business.BusinessId;


            Request.RequestTypeId = 2;
            Request.Status = 1;
            Request.FirstName = viewdata.FirstName;
            Request.LastName = viewdata.LastName;
            Request.Email = viewdata.Email;
            Request.PhoneNumber = viewdata.PhoneNumber;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.CreatedDate = DateTime.Now;
            Request.ConfirmationNumber = viewdata.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewdata.LastName.Substring(0, 2) + viewdata.FirstName.Substring(0, 2) + "002";

            _context.Requests.Add(Request);
            _context.SaveChanges();
            int id2 = Request.RequestId;

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewdata.FirstName;
            Requestclient.LastName = viewdata.LastName;
            Requestclient.Email = viewdata.Email;
            Requestclient.PhoneNumber = viewdata.PhoneNumber;
            Requestclient.Street = viewdata.Street;
            Requestclient.State = viewdata.State;
            Requestclient.City = viewdata.City;
            Requestclient.Notes = viewdata.Symptoms;
            Requestclient.ZipCode = viewdata.ZipCode;
            Requestclient.StrMonth = (viewdata.BirthDate.Month).ToString();
            Requestclient.IntDate = viewdata.BirthDate.Day;
            Requestclient.IntYear = viewdata.BirthDate.Year;
            Requestclient.Address = viewdata.Street + viewdata.City + viewdata.State + viewdata.ZipCode;

            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Requestbusiness.RequestId = id2;
            Requestbusiness.BusinessId = id1;
            _context.RequestBusinesses.Add(Requestbusiness);
            _context.SaveChanges();
        }

        //Showing Medical History
        public List<PatientDashboard> GetMedicalHistory(User user)
        {
            var medicalhistory = new List<PatientDashboard>();
            //var medicalhistory = (from request in _context.Requests
            //                      join requestfile in _context.Requestwisefiles
            //                      on request.Requestid equals requestfile.Requestid
            //                      where request.Email == user.Email && request.Email != null
            //                      group requestfile by request.Requestid into groupedFiles
            //                      select new PatientDashboard
            //                      {
            //                          FirstName = user.Firstname,
            //                          reqId = groupedFiles.Select(x => x.Request.Requestid).FirstOrDefault(),
            //                          Createddate = groupedFiles.Select(x => x.Request.Createddate).FirstOrDefault(),
            //                          Status = groupedFiles.Select(x => x.Request.Status).FirstOrDefault().ToString(),
            //                          File = groupedFiles.Select(x => x.Filename.ToString()).ToList()
            //                      }).ToList();
            List<int> fileCount = new();
            //for (int i = 0; i < medicalhistory.Count; i++)
            //{
            //    int count = _context.Requestwisefiles.Count(rf => rf.Requestid == medicalhistory[i].reqId);
            //    fileCount.Add(count);
            //}
            return medicalhistory;
        }

        public List<PatientDashboard> GetPatientInfos()
        {
            return new List<PatientDashboard> { };
        }

        public void ProfileData(PatientProfile newdetails, int? id)
        {
            try
            {

                User userToUpdate = _context.Users.Find(id);

                userToUpdate.FirstName = newdetails.FirstName;
                userToUpdate.LastName = newdetails.LastName;
                userToUpdate.Mobile = newdetails.Phone;
                userToUpdate.Email = newdetails.Email;
                userToUpdate.State = newdetails.State;
                userToUpdate.Street = newdetails.Street;
                userToUpdate.City = newdetails.City;
                userToUpdate.ZipCode = newdetails.ZipCode;
                userToUpdate.IntDate = newdetails.DOB.Day;
                userToUpdate.IntYear = newdetails.DOB.Year;
                userToUpdate.StrMonth = newdetails.DOB.Month.ToString();
                userToUpdate.ModifiedBy = newdetails.CreatedBy;
                userToUpdate.ModifiedDate = DateTime.Now;
                _context.Update(userToUpdate);
                _context.SaveChanges();

            }
            catch
            {
                throw;
            }
        }

        //#region PatientRecordsinAdminPage
        //public List<PatientProfile> PatientHistory()
        //{
        //    var pHis = (from req in _context.Users
        //                select new PatientProfile
        //                {
        //                    FirstName= req.FirstName,
        //                    LastName= req.LastName,
        //                    Email=req.Email,
        //                    Phone=req.Mobile,
        //                    Address= req.Street + req.City + req.State
        //                }).ToList();
        //    return pHis;
        //}
        //#endregion
        #region PatientRecordsinAdminPage
        public List<PatientProfile> PatientHistory(string fname, string lname, string email, string phone, string address)
        {
            var pHis  = _context.Users.
                        Select(req => new PatientProfile
                        {
                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            Email = req.Email,
                            Phone = req.Mobile,
                            Address = req.Street + req.City + req.State
                        }).Where(pp => string.IsNullOrEmpty(fname) || pp.FirstName.Contains(fname))
                          .Where(pp => string.IsNullOrEmpty(lname) || pp.LastName.Contains(lname))
                          .Where(pp => string.IsNullOrEmpty(email) || pp.Email.Contains(email))
                          .Where(pp => string.IsNullOrEmpty(phone) || pp.Phone.Contains(phone))
                          .Where(pp => string.IsNullOrEmpty(address) || pp.Address.Contains(address)).ToList();
            return pHis;
        }
        #endregion
    }
}

//public List<Employee> GetEmployeeSearchData(string email, string name, string department, string designation)
//{
//    // Assuming 'context' is your Entity Framework database context
//    // and 'Employee' is the entity that maps to 'EmsTblEmployee' table
//    var query = context.Employees.AsQueryable();

//    if (!string.IsNullOrEmpty(email))
//    {
//        query = query.Where(e => DbFunctions.Like(e.Email, $"%{email}%"));
//    }
//    if (!string.IsNullOrEmpty(name))
//    {
//        query = query.Where(e => DbFunctions.Like(e.FirstName + " " + e.LastName, $"%{name}%"));
//    }
//    if (!string.IsNullOrEmpty(department))
//    {
//        query = query.Where(e => DbFunctions.Like(e.Department, $"%{department}%"));
//    }
//    if (!string.IsNullOrEmpty(designation))
//    {
//        query = query.Where(e => DbFunctions.Like(e.Designation, $"%{designation}%"));
//    }

//    try
//    {
//        // ToList will execute the query and return a List of Employee
//        return query.ToList();
//    }
//    catch (Exception ex)
//    {
//        // Handle the exception as needed
//        // Replace this with your error handling code
//        throw new Exception("Error in fetching data from database: " + ex.Message);
//    }
//}
