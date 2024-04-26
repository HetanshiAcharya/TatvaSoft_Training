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
using static HaloDocDataAccess.ViewModels.Constant;
using iText.Html2pdf.Attach;
using System.Security.Cryptography;

namespace HaloDocRepository.Repositories
{
    public class PatientService : IPatientService
    {
        private readonly HaloDocDbContext _context;
        private readonly EmailConfiguration _emailconfig;
        private int IntDate;
        private int IntYear;

        public PatientService(HaloDocDbContext context, EmailConfiguration emailconfig)
        {
            _context = context;
            _emailconfig = emailconfig;
        }
        Random random = new Random();

        #region GenerateSHA256
        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        #endregion
        //Pateint Request
        public void PatientRequest(PatientSubmitRequests viewPatientReq)
        {

            var Aspnetuser = new AspNetUser();
            var role = new AspNetUserRole();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewPatientReq.Email);

            if (isexist == null)
            {
                Guid g = Guid.NewGuid();
                Aspnetuser.Id = g.ToString();
                Aspnetuser.UserName = viewPatientReq.FirstName;
                Aspnetuser.PasswordHash = GenerateSHA256(viewPatientReq.Pass);
                Aspnetuser.Email = viewPatientReq.Email;
                Aspnetuser.PhoneNumber = viewPatientReq.PhoneNumber;
                Aspnetuser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(Aspnetuser);
                _context.SaveChanges();
                role.UserId = Aspnetuser.Id;
                role.RoleId = "1";
                _context.AspNetUserRoles.Add(role);
                _context.SaveChanges();
                User.AspNetUserId = Aspnetuser.Id;
                User.FirstName = viewPatientReq.FirstName;
                User.LastName = viewPatientReq.LastName;
                User.Email = viewPatientReq.Email;
                User.Mobile = viewPatientReq.PhoneNumber;
                User.Street = viewPatientReq.Street;
                User.City = viewPatientReq.City;
                User.RegionId = viewPatientReq.State;
                User.State = Enum.GetName(typeof(StateLists), viewPatientReq.State);
                User.ZipCode = viewPatientReq.ZipCode;
                User.StrMonth = viewPatientReq.BirthDate.Month.ToString();
                User.IntDate = viewPatientReq.BirthDate.Day;
                User.IntYear = viewPatientReq.BirthDate.Year;
                User.Status = 1; //for new request
                User.CreatedBy = Aspnetuser.Id;
                User.CreatedDate = DateTime.Now;
                _context.Users.Add(User);
                _context.SaveChanges();
            }
            Request.RequestTypeId = 1;
            Request.Status = 1;
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            else
            {
                Request.UserId = User.UserId;

            }

            Request.FirstName = viewPatientReq.FirstName;
            Request.LastName = viewPatientReq.LastName;
            Request.Email = viewPatientReq.Email;
            Request.PhoneNumber = viewPatientReq.PhoneNumber;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.ConfirmationNumber = viewPatientReq.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewPatientReq.LastName.Substring(0, 2) + viewPatientReq.FirstName.Substring(0, 2) + random.Next(0,100);

            _context.Requests.Add(Request);
            _context.SaveChanges();
            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewPatientReq.FirstName;
            Requestclient.LastName = viewPatientReq.LastName;
            Requestclient.Address = viewPatientReq.Street + "," + viewPatientReq.City + "," + viewPatientReq.State + "," + viewPatientReq.ZipCode;
            Requestclient.Email = viewPatientReq.Email;
            Requestclient.Street = viewPatientReq.Street;
            Requestclient.City = viewPatientReq.City;
            Requestclient.State = Enum.GetName(typeof(StateLists), viewPatientReq.State);
            Requestclient.RegionId = viewPatientReq.State;
            Requestclient.ZipCode = viewPatientReq.ZipCode;
            Requestclient.PhoneNumber = viewPatientReq.PhoneNumber;
            Requestclient.Notes = viewPatientReq.Symptoms;
            Requestclient.IntDate = viewPatientReq.BirthDate.Day;
            Requestclient.IntYear = viewPatientReq.BirthDate.Year;
            Requestclient.StrMonth = (viewPatientReq.BirthDate.Month).ToString();
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewPatientReq.UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewPatientReq.UploadFile.FileName);
                viewPatientReq.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewPatientReq.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewPatientReq.UploadFile.CopyTo(stream);
                }

                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewPatientReq.UploadFile.FileName,
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
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
            var User = new User();
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7299/Home/Register?Email=" + viewdata.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailconfig.SendMail(viewdata.Email, Subject, template);
                EmailLog em = new EmailLog
                {

                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewdata.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }

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
                IsUrgentEmailSent = new BitArray(1),
                ConfirmationNumber = viewdata.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewdata.LastName.Substring(0, 2) + viewdata.FirstName.Substring(0, 2) + random.Next(0, 100)
            };
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            else
            {
                Request.UserId = User.UserId;

            }
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
                State = Enum.GetName(typeof(StateLists), viewdata.State),
                RegionId = viewdata.State,
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
            var User = new User();
            var Requestclient = new RequestClient();
            var Requestconcierge = new RequestConcierge();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email); Concierge.ConciergeName = viewdata.CON_FirstName + " " + viewdata.CON_LastName;
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7299/Home/Register?Email=" + viewdata.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailconfig.SendMail(viewdata.Email, Subject, template);
                EmailLog em = new EmailLog
                {

                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewdata.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }

            Concierge.Street = viewdata.CON_Street;
            Concierge.City = viewdata.CON_City;
            Concierge.State = Enum.GetName(typeof(StateLists), viewdata.CON_State);
            Concierge.RegionId = viewdata.State; 
            Concierge.ZipCode = "38585";
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

            Request.ConfirmationNumber = viewdata.CON_City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewdata.LastName.Substring(0, 2) + viewdata.FirstName.Substring(0, 2) + random.Next(0, 100);
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            else
            {
                Request.UserId = User.UserId;

            }

            _context.Requests.Add(Request);
            _context.SaveChanges();
            int id2 = Request.RequestId;

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewdata.FirstName;
            Requestclient.LastName = viewdata.LastName;
            Requestclient.Email = viewdata.Email;
            Requestclient.PhoneNumber = viewdata.PhoneNumber;
            Requestclient.Street = viewdata.CON_Street;
            Requestclient.State = Enum.GetName(typeof(StateLists), viewdata.CON_State);
            Requestclient.RegionId = viewdata.CON_State;
            Requestclient.City = viewdata.CON_City;
            Requestclient.Notes = viewdata.Symptoms;
            Requestclient.ZipCode = viewdata.CON_ZipCode;
            Requestclient.StrMonth = (viewdata.BirthDate.Month).ToString();
            Requestclient.IntDate = viewdata.BirthDate.Day;
            Requestclient.IntYear = viewdata.BirthDate.Year;
            Requestclient.Address = viewdata.CON_Street + viewdata.CON_City + viewdata.CON_State + viewdata.CON_ZipCode;

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
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
            var User = new User();

            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7299/Home/Register?Email=" + viewdata.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailconfig.SendMail(viewdata.Email, Subject, template);
                EmailLog em = new EmailLog
                {

                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewdata.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }
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
            Request.ConfirmationNumber = viewdata.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewdata.LastName.Substring(0, 2) + viewdata.FirstName.Substring(0, 2) + random.Next(0, 100);
            _context.Requests.Add(Request);
            _context.SaveChanges();
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            else
            {
                Request.UserId = User.UserId;

            }
            int id2 = Request.RequestId;

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewdata.FirstName;
            Requestclient.LastName = viewdata.LastName;
            Requestclient.Email = viewdata.Email;
            Requestclient.PhoneNumber = viewdata.PhoneNumber;
            Requestclient.Street = viewdata.Street;
            Requestclient.State = Enum.GetName(typeof(StateLists), viewdata.State);
            Requestclient.RegionId = viewdata.State;
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

        #region PatientRecordsinAdminPage
        public SearchInputs PatientHistory(SearchInputs search)
        {
            var pHis = _context.Users.
                        Select(req => new PatientProfile
                        {
                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            Email = req.Email,
                            Phone = req.Mobile,
                            Address = req.Street + req.City + req.State,
                            UserId = req.UserId
                        }).Where(pp => string.IsNullOrEmpty(search.FirstName) || pp.FirstName.Contains(search.FirstName))
                          .Where(pp => string.IsNullOrEmpty(search.LastName) || pp.LastName.Contains(search.LastName))
                          .Where(pp => string.IsNullOrEmpty(search.Email) || pp.Email.Contains(search.Email))
                          .Where(pp => string.IsNullOrEmpty(search.Mobile) || pp.Phone.Contains(search.Mobile))
                          .ToList();

            int totalItemCount = pHis.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)search.PageSize);
            List<PatientProfile> list1 = pHis.Skip((search.CurrentPage - 1) * search.PageSize).Take(search.PageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                pp = list1,
                CurrentPage = search.CurrentPage,
                TotalPages = totalPages,
                PageSize = search.PageSize,
            };
            return datanew;
        }
        #endregion
    }
}
