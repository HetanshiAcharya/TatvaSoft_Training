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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Globalization;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocRepository.Repositories
{
    public class AdminService : IAdminService
    {
        private readonly HaloDocDbContext _context;
        private readonly IConfiguration _config;
        public AdminService(HaloDocDbContext context, IConfiguration config)
        {
            _context = context;
            _config= config ;

        }
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

        /*public bool AdminAuthentication(AdminLogin adminDetails)
        {
            string hashPassword = GenerateSHA256(adminDetails.Password);
            return _context.AspNetUsers.Any(Au => Au.Email == adminDetails.Email && Au.PasswordHash == hashPassword);
        }*/

        #region NewRequestData
        public List<AdminDashboardList> NewRequestData()
        {
            var req = _context.Requests;
            var list = _context.Requests.Join
                        (_context.RequestClients,
                        requestclients => requestclients.RequestId, requests => requests.RequestId,
                        (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                        )
                        .Where(req => req.Request.Status == 1)
                        .Select(req => new AdminDashboardList()
                        {
                            RequestID = req.Request.RequestId,
                            PatientName = req.Requestclient.FirstName + " " + req.Requestclient.LastName,
                            //ProviderID=req.Request.PhysicianId,
                            Email = req.Requestclient.Email,
                            Dob = new DateTime((int)req.Requestclient.IntYear, int.Parse(req.Requestclient.StrMonth), (int)req.Requestclient.IntDate),
                            //DateOfBirth = new DateTime((int)req.Requestclient.Intyear, Convert.ToInt32(req.Requestclient.Strmonth.Trim()), (int)req.Requestclient.Intdate),
                            RequestTypeID = req.Request.RequestTypeId,
                            Requestor = req.Request.FirstName + " " + req.Request.LastName,
                            RequestedDate = (DateTime)req.Request.CreatedDate,
                            PhoneNumber = req.Requestclient.PhoneNumber,
                            RequestorPhoneNumber = req.Request.PhoneNumber,
                            Notes = req.Requestclient.Notes,
                            Requestclientid = req.Requestclient.RequestClientId,
                            Address = req.Requestclient.Address + " " + req.Requestclient.Street + " " + req.Requestclient.City + " " + req.Requestclient.State + " " + req.Requestclient.ZipCode
                        })
                        .OrderByDescending(req => req.RequestedDate)
                        .ToList();
            return list;
        }
        #endregion

        #region GetRequests
        public List<AdminDashboardList> GetRequests(string Status)
        {
            List<int> statusdata = Status.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                join reqClient in _context.RequestClients
                                                on req.RequestId equals reqClient.RequestId into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _context.Physicians
                                                on req.PhysicianId equals phys.PhysicianId into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _context.Regions
                                                on rc.RegionId equals reg.RegionId into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where statusdata.Contains(req.Status)
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    Requestclientid = rc.RequestClientId,
                                                    RequestID = req.RequestId,
                                                    RequestTypeID = req.RequestTypeId,
                                                    Requestor = req.FirstName + " " + req.LastName,
                                                    PatientName = rc.FirstName + " " + rc.LastName,
                                                    Bdate = rc.IntDate,
                                                    BMonth = rc.StrMonth,
                                                    BYear = rc.IntYear,
                                                    City = rc.City,
                                                    State = rc.State,
                                                    Street = rc.Street,
                                                    ZipCode = rc.ZipCode,
                                                    RequestedDate = (DateTime)req.CreatedDate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    ProviderID = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            return allData;
        }
        #endregion

        #region Indexdata
        public CountStatusWiseRequestModel Indexdata()
        {
            return new CountStatusWiseRequestModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),

            };
        }
        #endregion

        #region GetRequestForViewCase
        public ViewCaseData GetRequestForViewCase(int id)
        {
            var n = _context.Requests.FirstOrDefault(E => E.RequestId == id);

            var l = _context.RequestClients.FirstOrDefault(E => E.RequestId == id);

            var region = _context.Regions.FirstOrDefault(E => E.RegionId == l.RegionId);

            ViewCaseData requestforviewcase = new ViewCaseData
            {
                RequestID = id,
                //RequestClientId=l.RequestClientId,
                Region = region.Name,
                FirstName = l.FirstName,
                LastName = l.LastName,
                PhoneNumber = l.PhoneNumber,
                PatientNotes = l.Notes,
                Email = l.Email,
                RequestTypeID = n.RequestTypeId,
                Address = l.Street + "," + l.City + "," + l.State,
                Room = l.Address,
                ConfirmationNumber = n.ConfirmationNumber,
                //Dob = new DateTime((int)l.IntYear, DateTime.ParseExact(l.StrMonth, "MMMM", new CultureInfo("en-US")).Month, (int)l.IntDate)
            };
            return requestforviewcase;
        }
        #endregion

        #region NewRequestData
        public ViewCaseData NewRequestData(int? RId, int? RTId)
        {
            ViewCaseData? caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewCaseData()
                                        {
                                            UserID = req.Request.UserId,
                                            RequestClientId = req.RequestClientId,
                                            RequestID = (int)RId,
                                            RequestTypeID = (int)RTId,
                                            ConfirmationNumber = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                                            PatientNotes = req.Notes,
                                            FirstName = req.FirstName,
                                            LastName = req.LastName,
                                            Dob = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                                            PhoneNumber = req.PhoneNumber,
                                            Email = req.Email,
                                            Address = req.Address
                                        }).FirstOrDefault();
            //_context.Update(caseList);
            _context.SaveChanges();
            return caseList;
        }
        #endregion

        #region Edit
        public ViewCaseData Edit(ViewCaseData vdvc, int? RId, int? RTId)
        {
            try
            {
                RequestClient RC = _context.RequestClients.FirstOrDefault(E => E.RequestId == vdvc.RequestID);


                RC.PhoneNumber = vdvc.PhoneNumber;
                RC.Email = vdvc.Email;




                _context.Update(RC);
                _context.SaveChanges();

                ViewCaseData? caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewCaseData()
                                        {
                                            UserID = req.Request.UserId,
                                            RequestID = (int)RId,
                                            RequestTypeID = (int)RTId,
                                            ConfirmationNumber = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                                            PatientNotes = req.Notes,
                                            FirstName = req.FirstName,
                                            LastName = req.LastName,
                                            Dob = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                                            PhoneNumber = req.PhoneNumber,
                                            Email = req.Email,
                                            Address = req.Address
                                        }).FirstOrDefault();

                return caseList;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(vdvc.RequestID))
                {
                    throw;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region RequestExists
        private bool RequestExists(object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region getdropdownregion
        public List<Region> getdropdownregion()
        {
            var dropdown = _context.Regions.ToList();
            return dropdown;
        }
        #endregion

        #region ProviderbyRegion
        public List<Physician> ProviderbyRegion(int Regionid)
        {
            var result = _context.Physicians
                        .Where(req => req.RegionId == Regionid)
                        .Select(req => new Physician()
                        {
                            PhysicianId = req.PhysicianId,
                            FirstName = req.FirstName,
                            LastName = req.LastName
                        }).ToList();
            return result;

        }
        #endregion

        #region Professions
        public List<HealthProfessionalType> Professions()
        {
            var data = _context.HealthProfessionalTypes.ToList();
            return (data);
        }
        #endregion

        #region VendorByProfession
        public List<HealthProfessional> VendorByProfession(int Professionid)
        {
            var result = _context.HealthProfessionals
                        .Where(req => req.Profession == Professionid)
                        .ToList();
            return result;
        }
        #endregion

        #region SendOrdersInfo
        public HealthProfessional SendOrdersInfo(int selectedValue)
        {
            var result = _context.HealthProfessionals
                        .FirstOrDefault(req => req.VendorId == selectedValue);

            return result;
        }
        #endregion

        #region SendOrders
        public bool SendOrders(int requestid, OrderDetail o)
        {
            OrderDetail od = new OrderDetail
            {
                RequestId = requestid,
                VendorId = o.VendorId,
                FaxNumber = o.FaxNumber,
                Email = o.Email,
                NoOfRefill = o.NoOfRefill,
                BusinessContact = o.BusinessContact,
                Prescription = o.Prescription,
                CreatedDate = DateTime.Now,
            };
            _context.OrderDetails.Add(od);
            _context.SaveChanges();
            return true;


        }
        #endregion

        #region AssignCase
        public List<Region> AssignCase()
        {
            var regiondata = _context.Regions.ToList();
            return (regiondata);
        }
        #endregion

        #region AssignCaseInfo
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.PhysicianId = PhysicianId;
            request.Status = 2;
            //_context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = PhysicianId;
            rsl.Notes = Notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();


        }
        #endregion

        #region CancelCase
        public List<CaseTag> CancelCase()
        {
            var casetagdata = _context.CaseTags.ToList();
            return (casetagdata);
        }
        #endregion

        #region CancelCaseInfo
        public void CancelCaseInfo(int casetagId, string Notes, int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            var casetag = _context.CaseTags.FirstOrDefault(req => req.CaseTagId == casetagId);

            request.CaseTag = casetag.Name;
            request.Status = 8;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.Notes = Notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 8;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();


        }
        #endregion

        #region TransferCaseInfo
        public void TransferCaseInfo(int RequestId, int PhysicianId, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = (int)request.PhysicianId;
            rsl.TransToPhysicianId = PhysicianId;
            rsl.Notes = Notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();

            request.PhysicianId = PhysicianId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

           
        }
        #endregion

        #region clearcase
        public void ClearCase(int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.Status = 10;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 10;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();


        }
        #endregion

        #region blockcaseinfo
        public bool BlockCaseInfo(int requestId, string notes)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == requestId);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    BlockRequest blc = new BlockRequest
                    {
                        RequestId = requestData.RequestId,
                        PhoneNumber = requestData.PhoneNumber,
                        Email = requestData.Email,
                        Reason = notes,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.BlockRequests.Add(blc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ViewNotes
        public ViewNotes ViewNotes(int reqClientId)
        {
            RequestClient? req = _context.RequestClients.FirstOrDefault(x => x.RequestClientId == reqClientId);
            RequestNote? obj = _context.RequestNotes.FirstOrDefault(x => x.RequestId == req.RequestId);
            Physician physician = _context.Physicians.First(x => x.PhysicianId == 1);
            List<RequestStatusLog> requeststatuslog = _context.RequestStatusLogs.Where(x => x.RequestId == req.RequestId).ToList();


            ViewNotes? viewNote = new()
            {
                PhysicianName = physician.FirstName,
                AdminNotes = obj.AdminNotes,
                PhysicianNotes = obj.PhysicianNotes,
                Statuslogs = requeststatuslog,
            };
            return viewNote;
        }
        #endregion

        #region ViewNotesUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void ViewNotesUpdate(ViewNotes viewNotes)
        {
            RequestClient? req = _context.RequestClients.FirstOrDefault(x => x.RequestClientId == viewNotes.Requestclientid);
            RequestNote? obj = _context.RequestNotes.FirstOrDefault(x => x.RequestId == req.RequestId);

            obj.AdminNotes = viewNotes.TextBox;

        }
        #endregion

        #region deletefile
        public void DeleteFile(int requestid, int reqwisefileid)
        {
            var requestData = _context.RequestWiseFiles.FirstOrDefault(e => e.RequestWiseFileId == reqwisefileid);
            requestData.IsDeleted[0] = true;
            _context.RequestWiseFiles.Update(requestData);
            _context.SaveChanges();

        }
        #endregion

        #region sendagreement
        public void SendAgreement(sendAgreement sendAgreement, string link)
        {
            RequestClient reqCli = _context.RequestClients.FirstOrDefault(requestCli => requestCli.RequestId == sendAgreement.ReqId);

            string? senderEmail = _config.GetSection("OutlookSMTP")["Sender"];
            string? senderPassword = _config.GetSection("OutlookSMTP")["Password"];

            SmtpClient client = new("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            MailMessage mailMessage = new()
            {
                From = new MailAddress(senderEmail, "HaloDoc"),
                Subject = "Halodoc review agreement",
                IsBodyHtml = true,
                Body = "<h3>Admin has sent you the agreement papers to review. Click on the link below to read the agreement.</h3><a href=\"" + link + "\">Review Agreement link</a>",
            };

            mailMessage.To.Add(sendAgreement.Email);

            client.Send(mailMessage);
        }
        #endregion

        #region SendAgreement_accept
        public Boolean SendAgreement_accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;

                rsl.Status = 4;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        #region SendAgreement_Reject
        public Boolean SendAgreement_Reject(int RequestID, string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;

                rsl.Status = 7;
                rsl.Notes = Notes;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        #region cancelagreementsubmit
        public void CancelAgreementSubmit(int Reqid, string Description)
        {
            Request request = _context.Requests.FirstOrDefault(x => x.RequestId == Reqid);
            //Request request = _context.Requests.FirstOrDefault(x => x.Requestid == requestclient.Requestid);

            RequestStatusLog requeststatuslog = new()
            {
                RequestId = request.RequestId,
                Status = 7,
                CreatedDate = DateTime.Now,
                Notes = Description
            };
            _context.RequestStatusLogs.Add(requeststatuslog);

            request.Status = 7; //cancelled by patient
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();
        }
        #endregion

        public List<AdminDashboardList> InfoByRegion(int Regionid)
        {
            int? statusdata = Regionid;
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                join reqClient in _context.RequestClients
                                                on req.RequestId equals reqClient.RequestId into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _context.Physicians
                                                on req.PhysicianId equals phys.PhysicianId into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _context.Regions
                                                on rc.RegionId equals reg.RegionId into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where statusdata== rc.RegionId
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    Requestclientid = rc.RequestClientId,
                                                    RequestID = req.RequestId,
                                                    RequestTypeID = req.RequestTypeId,
                                                    Requestor = req.FirstName + " " + req.LastName,
                                                    PatientName = rc.FirstName + " " + rc.LastName,
                                                    Bdate = rc.IntDate,
                                                    BMonth = rc.StrMonth,
                                                    BYear = rc.IntYear,
                                                    City = rc.City,
                                                    State = rc.State,
                                                    Street = rc.Street,
                                                    ZipCode = rc.ZipCode,
                                                    RequestedDate = (DateTime)req.CreatedDate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    ProviderID = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            return allData;
        }
        public ViewCloseCaseModel CloseCaseData(int RequestID)
        {
            ViewCloseCaseModel alldata = new ViewCloseCaseModel();

            var result = from requestWiseFile in _context.RequestWiseFiles
                         join request in _context.Requests on requestWiseFile.RequestId equals request.RequestId
                         join physician in _context.Physicians on request.PhysicianId equals physician.PhysicianId into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.AdminId equals admin.AdminId into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.RequestId == RequestID
                         select new
                         {

                             Uploader = requestWiseFile.PhysicianId != null ? phys.FirstName :
                             (requestWiseFile.AdminId != null ? adm.FirstName : request.FirstName),
                             requestWiseFile.FileName,
                             requestWiseFile.CreatedDate,
                             requestWiseFile.RequestWiseFileId

                         };
            List<ViewDocument> doc = new List<ViewDocument>();
            foreach (var item in result)
            {
                doc.Add(new ViewDocument
                {
                    CreatedDate = item.CreatedDate,
                    FileName = item.FileName,
                    Uploader = item.Uploader,
                    RequestwisefilesId = item.RequestWiseFileId

                });

            }
            alldata.documentslist = doc;
            Request req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestID);

            alldata.FirstName = req.FirstName;
            alldata.RequestID = req.RequestId;
            alldata.ConfirmationNumber = req.ConfirmationNumber;
            alldata.LastName = req.LastName;

            var reqcl = _context.RequestClients.FirstOrDefault(e => e.RequestId == RequestID);

            alldata.RC_Email = reqcl.Email;
            //alldata.RC_Dob = new DateTime((int)reqcl.IntYear, DateTime.ParseExact(reqcl.StrMonth, "MMMM", new CultureInfo("en-US")).Month, (int)reqcl.IntDate);
            alldata.RC_FirstName = reqcl.FirstName;
            alldata.RC_LastName = reqcl.LastName;
            alldata.RC_PhoneNumber = reqcl.PhoneNumber;
            return alldata;
        }
        public bool EditForCloseCase(ViewCloseCaseModel model)
        {
            try
            {
                RequestClient client = _context.RequestClients.FirstOrDefault(E => E.RequestId == model.RequestID);
                if (client != null)
                {
                    client.PhoneNumber = model.RC_PhoneNumber;
                    client.Email = model.RC_Email;
                    _context.RequestClients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 9;
                    requestData.ModifiedDate = DateTime.Now;

                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,


                        Status = 9,
                        CreatedDate = DateTime.Now

                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}

