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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Metrics;
using System.IO;
using System.Drawing;
using static HaloDocDataAccess.ViewModels.AdminDetailsInfo;
using static HaloDocDataAccess.ViewModels.Constant;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HaloDocRepository.Repositories
{
    public class AdminService : IAdminService
    {
        private readonly HaloDocDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailConfiguration _emailConfig;

        public AdminService(HaloDocDbContext context, IConfiguration config, EmailConfiguration emailConfig)
        {
            _context = context;
            _config = config;
            _emailConfig = emailConfig;
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

        #region indexforgotpass
        public bool IndexForgotPass(PatientForgotPassword model)
        {
            PatientLoginDetails patientResetPassword = new PatientLoginDetails();
            var check = _context.AspNetUsers.Any(x => x.Email == model.Email);
            if (check != null)
            {
                var agreementUrl = "https://localhost:7299/Admin/ResetPassAdmin?Email=" + model.Email;
                var subject = "Reset Your Password";
                var EmailTemplate = $"To reset your password click below link <a href='{agreementUrl}'>Reset Password</a>";
                bool sent = _emailConfig.SendMail(model.Email, subject, EmailTemplate);
                EmailLog em = new EmailLog
                {
                    //RequestId = model.re,
                    EmailTemplate = EmailTemplate,
                    SubjectName = subject,
                    EmailId = model.Email,
                    //ConfirmationNumber = _context.Requests.Where(req => req.RequestId == requestid).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 5,//action5 forgot pass
                    RoleId = 3,
                };
                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
                return true;

            }
            return false;
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
        public PaginatedViewModel GetRequests(PaginatedViewModel data)
        {
            List<int> statusdata = data.Status.Split(',').Select(int.Parse).ToList();
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
                                                where statusdata.Contains((int)req.Status) && (data.SearchInput == null ||
                                                rc.FirstName.Contains(data.SearchInput) || rc.LastName.Contains(data.SearchInput) ||
                                                req.FirstName.Contains(data.SearchInput) || req.LastName.Contains(data.SearchInput) ||
                                                rc.Email.Contains(data.SearchInput) || rc.PhoneNumber.Contains(data.SearchInput) ||
                                                rc.Street.Contains(data.SearchInput) || rc.Notes.Contains(data.SearchInput) ||
                                                p.FirstName.Contains(data.SearchInput) || p.LastName.Contains(data.SearchInput) || rc.Street.Contains(data.SearchInput) || rc.City.Contains(data.SearchInput) || rc.State.Contains(data.SearchInput) || rc.ZipCode.Contains(data.SearchInput) ||
                                                rg.Name.Contains(data.SearchInput)) && (data.RegionId == null || rc.RegionId == data.RegionId)
                                                && (data.RequestType == null || req.RequestTypeId == data.RequestType)
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
            if (data.IsAscending == true)
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderBy(x => x.Requestor).ToList(),
                    //"Dob" => allData.OrderBy(x => x.Dob).ToList(),
                    "Address" => allData.OrderBy(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderBy(x => x.RequestedDate).ToList(),
                    _ => allData.OrderBy(x => x.RequestedDate).ToList()
                };
            }
            else
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderByDescending(x => x.Requestor).ToList(),
                    //"Dob" => allData.OrderByDescending(x => x.Dob).ToList(),
                    "Address" => allData.OrderByDescending(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderByDescending(x => x.RequestedDate).ToList(),
                    _ => allData.OrderByDescending(x => x.RequestedDate).ToList()
                };
            }
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            PaginatedViewModel paginatedViewModel = new PaginatedViewModel
            {
                adl = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput,
                IsAscending = data.IsAscending,
                SortedColumn = data.SortedColumn
            };
            return paginatedViewModel;
        }
        #endregion

        #region Indexdata
        public PaginatedViewModel Indexdata()
        {
            return new PaginatedViewModel
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
        public List<HaloDocDataAccess.DataModels.Region> getdropdownregion()
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
            try
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

                var subject = "Order details";
                var EmailTemplate = "Order details: <h3>Hear is the details of order </h3> <p>" + o.Prescription + "</p> ";
                bool sent = _emailConfig.SendMail(od.Email, subject, EmailTemplate);
                EmailLog em = new EmailLog
                {
                    RequestId = requestid,
                    EmailTemplate = EmailTemplate,
                    SubjectName = subject,
                    EmailId = od.Email,
                    ConfirmationNumber = _context.Requests.Where(req => req.RequestId == requestid).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 1,
                    RoleId = 3,
                };
                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region AssignCase
        public List<HaloDocDataAccess.DataModels.Region> AssignCase()
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
        public ViewNotes ViewNotes(int RequestId)
        {
            //RequestClient? req = _context.RequestClients.FirstOrDefault(x => x.RequestClientId == reqClientId);
            //RequestNote? obj = _context.RequestNotes.FirstOrDefault(x => x.RequestId == req.RequestId);
            //Physician physician = _context.Physicians.First(x => x.PhysicianId == 1);
            //List<RequestStatusLog> requeststatuslog = _context.RequestStatusLogs.Where(x => x.RequestId == req.RequestId).ToList();


            //ViewNotes? viewNote = new()
            //{
            //    PhysicianName = physician.FirstName,
            //    AdminNotes = obj.AdminNotes,
            //    PhysicianNotes = obj.PhysicianNotes,
            //    Statuslogs = requeststatuslog,
            //};
            //return viewNote;
            var req = _context.Requests.FirstOrDefault(W => W.RequestId == RequestId);
            //var symptoms = _context.RequestClients.FirstOrDefault(W => W.RequestId == RequestId);
            var transferlog = (from rs in _context.RequestStatusLogs
                               join py in _context.Physicians on rs.PhysicianId equals py.PhysicianId into pyGroup
                               from py in pyGroup.DefaultIfEmpty()
                               join p in _context.Physicians on rs.TransToPhysicianId equals p.PhysicianId into pGroup
                               from p in pGroup.DefaultIfEmpty()
                                   //join a in _context.Admins on rs.AdminId equals a.AdminId into aGroup
                                   //from a in aGroup.DefaultIfEmpty()
                               where rs.RequestId == RequestId && rs.Status == 2
                               select new TransferNotesData
                               {
                                   TransPhysician = p.FirstName,
                                   //Admin = a.FirstName,
                                   Physician = py.FirstName,
                                   Requestid = rs.RequestId,
                                   Notes = rs.Notes,
                                   Status = rs.Status,
                                   Physicianid = rs.PhysicianId,
                                   Createddate = rs.CreatedDate,
                                   Requeststatuslogid = rs.RequestStatusLogId,
                                   Transtoadmin = rs.TransToAdmin,
                                   Transtophysicianid = rs.TransToPhysicianId
                               }).ToList();
            var cancelbyprovider = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.TransToAdmin != null));
            var cancel = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.Status == 7 || E.Status == 3));
            var model = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestId);
            ViewNotes allData = new ViewNotes();
            allData.Requestid = RequestId;
            //allData.PatientNotes = symptoms.Notes;
            if (model == null)
            {
                allData.PhysicianNotes = "-";
                allData.AdminNotes = "-";
            }
            else
            {
                allData.Status = (short)req.Status;
                allData.Requestnotesid = model.RequestNotesId;
                allData.PhysicianNotes = model.PhysicianNotes ?? "-";
                allData.AdminNotes = model.AdminNotes ?? "-";
            }

            List<TransferNotesData> transfer = new List<TransferNotesData>();
            foreach (var item in transferlog)
            {
                transfer.Add(new TransferNotesData
                {
                    TransPhysician = item.TransPhysician,
                    // Admin = item.Admin,
                    Physician = item.Physician,
                    Requestid = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.Physicianid,
                    Createddate = item.Createddate,
                    Requeststatuslogid = item.Requeststatuslogid,
                    Transtoadmin = item.Transtoadmin,
                    Transtophysicianid = item.Transtophysicianid
                });
            }
            allData.transfernotes = transfer;
            List<TransferNotesData> cancelbyphysician = new List<TransferNotesData>();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancelbyphysician = cancelbyphysician;

            List<TransferNotesData> cancelrq = new List<TransferNotesData>();
            foreach (var item in cancel)
            {
                cancelrq.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancel = cancelrq;
            return allData;
        }
        #endregion

        #region ViewNotesUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ViewNotesUpdate(string? adminnotes, string? physiciannotes, int RequestID)
        {
            //RequestClient? req = _context.RequestClients.FirstOrDefault(x => x.RequestClientId == viewNotes.Requestclientid);
            //RequestNote? obj = _context.RequestNotes.FirstOrDefault(x => x.RequestId == req.RequestId);

            //obj.AdminNotes = viewNotes.TextBox;
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestID);
                if (notes != null)
                {
                    if (physiciannotes != null)
                    {
                        if (notes != null)
                        {
                            notes.PhysicianNotes = physiciannotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (adminnotes != null)
                    {
                        if (notes != null)
                        {
                            notes.AdminNotes = adminnotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RequestNote rn = new RequestNote
                    {
                        RequestId = RequestID,
                        AdminNotes = adminnotes,
                        PhysicianNotes = physiciannotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "gg"

                    };
                    _context.RequestNotes.Add(rn);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

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

        #region deleteallfile
        public void DeleteFile(int requestid, int[] reqwisefileid)
        {
            var filesForRequest = _context.RequestWiseFiles
                    .Where(file => reqwisefileid.Contains(file.RequestWiseFileId))
                    .ToList();
            foreach (var file in filesForRequest)
            {
                file.RequestId = requestid;
                file.IsDeleted[0] = true;

                _context.Update(file);
                _context.SaveChanges();

            }
        }
        #endregion

        #region sendagreement
        public bool SendAgreement(sendAgreement sendAgreement)
        {
            var agreementUrl = "https://localhost:7299/Home/ReviewAgreement?ReqId=" + sendAgreement.ReqId;           
            var subject = "Agreement for your request";
            var EmailTemplate = $"Agreement for your request <a href='{agreementUrl}'>Agree/Disagree</a>";
            bool sent = _emailConfig.SendMail(sendAgreement.Email, subject, EmailTemplate);
            EmailLog em = new EmailLog
            {
                RequestId = sendAgreement.ReqId,
                EmailTemplate = EmailTemplate,
                SubjectName = subject,
                EmailId = sendAgreement.Email,
                ConfirmationNumber = _context.Requests.Where(req => req.RequestId == sendAgreement.ReqId).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1),
                SentTries = 1,
                Action = 4, //action 4 - send agreement
                RoleId = 3, //role admin 
            };
            if (sent)
            {
                em.IsEmailSent[0] = true;
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region SendAgreement_accept
        public System.Boolean SendAgreement_accept(int RequestID)
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
        public System.Boolean SendAgreement_Reject(int RequestID, string Notes)
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
            HaloDocDataAccess.DataModels.Request request = _context.Requests.FirstOrDefault(x => x.RequestId == Reqid);
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
                                                where statusdata == rc.RegionId
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
            HaloDocDataAccess.DataModels.Request req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestID);

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


        #region enounterinfo
        public EncounterInfo Encounterinfo(int rId)
        {

            if (rId == null) return null;
            var encounter = (from rc in _context.RequestClients
                             join en in _context.EncounterForms on rc.RequestId equals en.RequestId into renGroup
                             from subEn in renGroup.DefaultIfEmpty()
                             where rc.RequestId == rId
                             select new EncounterInfo
                             {
                                 RequestID = rc.RequestId,
                                 FirstName = rc.FirstName,
                                 LastName = rc.LastName,
                                 Location = rc.Address,
                                 Bdate = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                 PhoneNumber = rc.PhoneNumber,
                                 Email = rc.Email,
                                 HistoryOfIllness = subEn.HistoryOfPresentIllnessOrInjury,
                                 MedicalHist = subEn.MedicalHistory,
                                 Medications = subEn.Medications,
                                 Allergies = subEn.Allergies,
                                 Temp = subEn.Temp,
                                 HR = subEn.Hr,
                                 RR = subEn.Rr,
                                 BPS = subEn.BloodPressureSystolic,
                                 BPD = subEn.BloodPressureDiastolic,
                                 O2 = subEn.O2,
                                 Pain = subEn.Pain,
                                 heent = subEn.Heent,
                                 CV = subEn.Cv,
                                 Chest = subEn.Chest,
                                 ABD = subEn.Abd,
                                 Extr = subEn.Extremeties,
                                 Skin = subEn.Skin,
                                 Neuro = subEn.Neuro,
                                 Other = subEn.Other,
                                 Diagnosis = subEn.Diagnosis,
                                 TrtPlan = subEn.TreatmentPlan,
                                 MedDispensed = subEn.MedicationsDispensed,
                                 Procedures = subEn.Procedures,
                                 Followup = subEn.FollowUp,
                             }).FirstOrDefault();

            return encounter;
        }

        #endregion

        #region EncounterSave
        public EncounterInfo EncounterInfoPost(EncounterInfo ve)
        {
            var RC = _context.RequestClients.FirstOrDefault(rc => rc.RequestId == ve.RequestID);
            if (RC == null) return null;
            RC.FirstName = ve.FirstName;
            RC.LastName = ve.LastName;
            RC.Address = ve.Location;
            RC.StrMonth = ve.Bdate.Month.ToString();
            RC.IntDate = ve.Bdate.Day;
            RC.IntYear = ve.Bdate.Year;
            RC.PhoneNumber = ve.PhoneNumber;

            RC.Email = ve.Email;
            _context.Update(RC);
            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestID);
            if (E == null)
            {
                E = new EncounterForm { RequestId = (int)ve.RequestID };
                _context.EncounterForms.Add(E);
            }
            //E.DateOfService = ve.CreatedDate;
            E.MedicalHistory = ve.MedicalHist;
            E.HistoryOfPresentIllnessOrInjury = ve.HistoryOfIllness;
            E.Medications = ve.Medications;
            E.Allergies = ve.Allergies;
            E.Temp = ve.Temp;
            E.Hr = ve.HR;
            E.Rr = ve.RR;
            E.BloodPressureSystolic = ve.BPS;
            E.BloodPressureDiastolic = ve.BPD;
            E.O2 = ve.O2;
            E.Pain = ve.Pain;
            E.Heent = ve.heent;
            E.Cv = ve.CV;
            E.Chest = ve.Chest;
            E.Abd = ve.ABD;
            E.Extremeties = ve.Extr;
            E.Skin = ve.Skin;
            E.Neuro = ve.Neuro;
            E.Other = ve.Other;
            E.Diagnosis = ve.Diagnosis;
            E.TreatmentPlan = ve.TrtPlan;
            E.MedicationsDispensed = ve.MedDispensed;
            E.Procedures = ve.Procedures;
            E.FollowUp = ve.Followup;
            E.IsFinalize = false;
            _context.SaveChanges();
            return ve;
        }
        #endregion

        #region encounterfinalize
        public void EncounterFinalize(EncounterInfo ve)
        {
            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestID);
            E.IsFinalize = true;
            _context.SaveChanges();
        }
        #endregion

        #region GetProfile
        public async Task<AdminDetailsInfo> GetProfileDetails(int id)
        {
            AdminDetailsInfo? v = await (from r in _context.Admins
                                         join Aspnetuser in _context.AspNetUsers
                                               on r.AspNetUserId equals Aspnetuser.Id into aspGroup
                                         from asp in aspGroup.DefaultIfEmpty()
                                         where r.AdminId == id
                                         select new AdminDetailsInfo
                                         {
                                             Role = r.RoleId,
                                             AdminId = r.AdminId,
                                             UserName = asp.UserName,
                                             Add1 = r.Address1,
                                             Add2 = r.Address2,
                                             PhoneForBill = r.AltPhone,
                                             City = r.City,
                                             AspNetUserId = r.AspNetUserId,
                                             CreatedBy = r.CreatedBy,
                                             Email = r.Email,
                                             CreatedDate = r.CreatedDate,
                                             Phone = r.Mobile,
                                             //ModifiedBy = r.ModifiedBy,
                                             //Modifieddate = r.ModifiedDate,
                                             Regionid = r.RegionId,
                                             LastName = r.LastName,
                                             FirstName = r.FirstName,
                                             Status = r.Status,
                                             Zip = r.Zip,
                                         }).FirstOrDefaultAsync();
            List<HaloDocDataAccess.DataModels.Region> regions = new List<HaloDocDataAccess.DataModels.Region>();
            regions = await _context.AdminRegions
                  .Where(r => r.AdminId == id)
                  .Select(req => new HaloDocDataAccess.DataModels.Region()
                  {
                      RegionId = req.RegionId

                  })
                  .ToListAsync();
            v.Regionids = regions;
            return v;
        }
        #endregion

        #region EditPassword
        public async Task<bool> EditPassword(string password, int adminId)
        {
            var req = await _context.Admins.Where(W => W.AdminId == adminId).FirstOrDefaultAsync();
            AspNetUser? U = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == req.AspNetUserId);

            if (U != null)
            {
                U.PasswordHash = GenerateSHA256(password);
                _context.AspNetUsers.Update(U);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region BillingInfoEdit
        public async Task<bool> BillingInfoEdit(AdminDetailsInfo _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = _viewAdminProfile.Add1;
                        DataForChange.Address2 = _viewAdminProfile.Add2;
                        DataForChange.City = _viewAdminProfile.City;
                        DataForChange.AltPhone = _viewAdminProfile.PhoneForBill;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EditAdministratorInfo
        public async Task<bool> EditAdministratorInfo(AdminDetailsInfo _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = _viewAdminProfile.Email;
                        DataForChange.FirstName = _viewAdminProfile.FirstName;
                        DataForChange.LastName = _viewAdminProfile.LastName;
                        DataForChange.Mobile = _viewAdminProfile.Phone;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId).Select(req => req.RegionId).ToListAsync();
                        List<int> priceList = _viewAdminProfile.Regionsid.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                AdminRegion ar = new()
                                {
                                    RegionId = item,
                                    AdminId = (int)_viewAdminProfile.AdminId
                                };
                                _context.AdminRegions.Update(ar);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                AdminRegion ar = await _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId && r.RegionId == item).FirstAsync();
                                _context.AdminRegions.Remove(ar);
                                await _context.SaveChangesAsync();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region sendagreement
        public bool SendAgreementfromUploads(sendAgreement sendAgreement)
        {
            var agreementUrl = "https://localhost:7299/Home/ReviewAgreement?ReqId=" + sendAgreement.ReqId;
            _emailConfig.SendMail(sendAgreement.Email, "Agreement for your request", $"Agreement for your request <a href='{agreementUrl}'>Agree/Disagree</a>");
            return true;
            //RequestClient reqCli = _context.RequestClients.FirstOrDefault(requestCli => requestCli.RequestId == sendAgreement.ReqId);

            //string? senderEmail = _config.GetSection("OutlookSMTP")["Sender"];
            //string? senderPassword = _config.GetSection("OutlookSMTP")["Password"];

            //SmtpClient client = new("smtp.office365.com")
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(senderEmail, senderPassword),
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false
            //};

            //MailMessage mailMessage = new()
            //{
            //    From = new MailAddress(senderEmail, "HaloDoc"),
            //    Subject = "Halodoc review agreement",
            //    IsBodyHtml = true,
            //    Body = "<h3>Admin has sent you the agreement papers to review. Click on the link below to read the agreement.</h3><a href=\"" + link + "\">Review Agreement link</a>",
            //};

            //mailMessage.To.Add(sendAgreement.Email);

            //client.Send(mailMessage);
        }
        #endregion

        #region SendLink

        public bool SendLink(sendAgreement sendAgreement)
        {
            var agreementUrl = "https://localhost:7299/Home/SubmitReq?ReqId=" + sendAgreement.ReqId;
            var subject = "Create Request from here !! ";
            var EmailTemplate = $"You can create request just by clicking below <a href='{agreementUrl}'>Accept and Generate Request</a>";
            bool sent = _emailConfig.SendMail(sendAgreement.Email, subject, EmailTemplate);
            EmailLog em = new EmailLog
            {
                RequestId = sendAgreement.ReqId,
                EmailTemplate = EmailTemplate,
                SubjectName = subject,
                EmailId = sendAgreement.Email,
                ConfirmationNumber = _context.Requests.Where(req => req.RequestId == sendAgreement.ReqId).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1),
                SentTries = 1,
                Action = 3,//action 3 - send link
                RoleId = 3,
            };
            if (sent)
            {
                em.IsEmailSent[0] = true;
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region Export
        public List<AdminDashboardList> Export(string status)
        {
            List<int> statusdata = status.Split(',').Select(int.Parse).ToList();
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
                                                where statusdata.Contains((int)req.Status)
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestID = req.RequestId,
                                                    RequestTypeID = req.RequestTypeId,
                                                    Requestor = req.FirstName + " " + req.LastName,
                                                    PatientName = rc.FirstName + " " + rc.LastName,
                                                    //Bdate = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                                    RequestedDate = (DateTime)req.CreatedDate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address,
                                                    Notes = rc.Notes,
                                                    ProviderID = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            return allData;
        }
        #endregion

        #region ProviderMenu
        public ProviderMenu ProviderMenu(int Region, int pageinfo)
        {
            var pageSize = 3;
            if (pageinfo == 0)
            {
                pageinfo = 1;
            }
            var providerMenu = from phy in _context.Physicians
                               join role in _context.Roles on phy.RoleId equals role.RoleId
                               join phyid in _context.PhysicianNotifications on phy.PhysicianId equals phyid.PhysicianId
                               where phy.IsDeleted == new BitArray(1)
                               && (Region == -1 || phy.RegionId == Region)
                               select new ProviderList
                               {
                                   UserName = phy.FirstName + phy.LastName,
                                   Email = phy.Email,
                                   PhysicianId = phy.PhysicianId,
                                   FirstName = phy.FirstName,
                                   LastName = phy.LastName,
                                   Status = (ProviderStatus)phy.Status,
                                   Role = role.Name,
                                   OnCallStatus = phy.IsNonDisclosureDoc,
                                   Notification = (bool)phyid.IsNotificationStopped
                               };


            int totalItemCount = providerMenu.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            List<ProviderList> list1 = providerMenu.Skip((pageinfo - 1) * pageSize).Take(pageSize).ToList();
            ProviderMenu datanew = new ProviderMenu
            {
                ProviderLists = list1,
                CurrentPage = pageinfo,
                TotalPages = totalPages,
                PageSize = pageSize,
            };
            return datanew;
        }

        #endregion

        #region changeNoti
        public bool ChangeNoti(int[] files, int region)
        {

            List<PhysicianNotification> PhysicianNotification = (from noti in _context.PhysicianNotifications
                                                                 join
                                                                 phy in _context.Physicians on noti.PhysicianId equals phy.PhysicianId
                                                                 where (region == -1 || phy.RegionId == region)
                                                                 select noti).ToList();
            foreach (var item in PhysicianNotification)
            {
                if (files.Contains(item.PhysicianId))
                {
                    item.IsNotificationStopped = true;
                    _context.PhysicianNotifications.Update(item);
                    _context.SaveChanges();
                }
                else
                {
                    item.IsNotificationStopped = false;
                    _context.PhysicianNotifications.Update(item);
                    _context.SaveChanges();
                }
            }
            return true;
        }
        #endregion

        #region sendemailprovider
        public bool SendEmailProvider(string Email, string Message)
        {
            var subject = "Message From Admin ~ Contact Provider ";
            var EmailTemplate = Message;
            bool sent = _emailConfig.SendMail(Email, subject, EmailTemplate);
            EmailLog em = new EmailLog
            {
                //RequestId = sendAgreement.ReqId,
                EmailTemplate = EmailTemplate,
                SubjectName = subject,
                EmailId = Email,
                //ConfirmationNumber = _context.Requests.Where(req => req.RequestId == sendAgreement.ReqId).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1),
                SentTries = 1,
                Action = 7,//action 7 - contact provider
                RoleId = 3,
            };
            if (sent)
            {
                em.IsEmailSent[0] = true;
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
            return true;
        }

        #endregion
        public bool SendMessage(string? Message)
        {
            string contact = "+919313474649";
            bool sms = _emailConfig.SendSMS(contact, Message).Result;
            Smslog em = new Smslog
            {
                Smstemplate = Message,
                MobileNumber = contact,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsSmssent = new BitArray(1),
                SentTries = 1,
                Action = 3, // action 3 for send link of submit request
                RoleId = 2, // role 2 for admin
            };

            if (sms)
            {
                em.IsSmssent[0] = true;
            };
            _context.Smslogs.Add(em);
            _context.SaveChanges();
            return true;
        }

        #region ProviderRoleViewBag
        public List<Role> ProviderRole()
        {
            var role = _context.Roles.Where(r => r.AccountType == 2).ToList();
            return (role);
        }
        #endregion

        #region GetProviderProfileDetails
        public async Task<ProviderList> GetProviderProfileDetails(int id)
        {
            ProviderList? v = await (from r in _context.Physicians
                                     join Aspnetuser in _context.AspNetUsers
                                     on r.AspNetUserId equals Aspnetuser.Id
                                     where r.PhysicianId == id
                                     select new ProviderList
                                     {
                                         RoleId = r.RoleId,
                                         PhysicianId = r.PhysicianId,
                                         UserName = Aspnetuser.UserName,
                                         Password = Aspnetuser.PasswordHash,
                                         LastName = r.LastName,
                                         FirstName = r.FirstName,
                                         Email = r.Email,
                                         Phone = r.Mobile,
                                         MedLicence = r.MedicalLicense,
                                         NpiNum = r.Npinumber,
                                         SyncEmail = r.SyncEmailAddress,
                                         Add1 = r.Address1,
                                         Add2 = r.Address2,
                                         PhoneForBill = r.AltPhone,
                                         City = r.City,
                                         Regionid = r.RegionId,
                                         Status = (ProviderStatus)r.Status,
                                         Zip = r.Zip,
                                         Bname = r.BusinessName,
                                         Bwebsite = r.BusinessWebsite,
                                         isAgreementDoc = r.IsAgreementDoc[0],
                                         isLicenseDoc = r.IsLicenseDoc[0],
                                         isBackgroundDoc = r.IsBackgroundDoc[0],
                                         isCredentialDoc = r.IsCredentialDoc[0],
                                         isNonDisclosureDoc = (bool)r.IsNonDisclosureDoc
                                     }).FirstOrDefaultAsync();
            List<HaloDocDataAccess.DataModels.Region> regions = new List<HaloDocDataAccess.DataModels.Region>();
            regions = await _context.PhysicianRegions
                  .Where(r => r.PhysicianId == id)
                  .Select(req => new HaloDocDataAccess.DataModels.Region()
                  {
                      RegionId = req.RegionId

                  })
                  .ToListAsync();
            v.Regionids = regions;
            return v;

        }
        #endregion

        #region EditProviderAccInfo
        public async Task<bool> EditProviderAccInfo(ProviderList p)
        {
            var req = await _context.Physicians.Where(W => W.PhysicianId == p.PhysicianId).FirstOrDefaultAsync();
            AspNetUser? U = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == req.AspNetUserId);

            if (U != null)
            {
                if (req != null)
                {

                    req.Status = (short?)(ProviderStatus)p.Status;
                    req.RoleId = p.RoleId;
                    _context.Physicians.Update(req);
                    _context.SaveChanges();
                }
                U.PasswordHash = GenerateSHA256(p.Password);
                U.Email = p.Email;
                _context.AspNetUsers.Update(U);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region EditProviderInfo
        public async Task<bool> EditProviderInfo(ProviderList p)
        {
            try
            {
                if (p == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians.Where(W => W.PhysicianId == p.PhysicianId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = p.Email;
                        DataForChange.FirstName = p.FirstName;
                        DataForChange.LastName = p.LastName;
                        DataForChange.Mobile = p.Phone;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.PhysicianRegions.Where(r => r.PhysicianId == p.PhysicianId).Select(req => req.RegionId).ToListAsync();
                        List<int> priceList = p.Regionsid.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                PhysicianRegion ar = new()
                                {
                                    RegionId = item,
                                    PhysicianId = (int)p.PhysicianId
                                };
                                _context.PhysicianRegions.Update(ar);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                PhysicianRegion ar = await _context.PhysicianRegions.Where(r => r.PhysicianId == p.PhysicianId && r.RegionId == item).FirstAsync();
                                _context.PhysicianRegions.Remove(ar);
                                await _context.SaveChangesAsync();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EditProviderMailingInfo
        public async Task<bool> EditProviderMailingInfo(ProviderList p)
        {
            try
            {
                if (p == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians.Where(W => W.PhysicianId == p.PhysicianId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = p.Add1;
                        DataForChange.Address2 = p.Add2;
                        DataForChange.City = p.City;
                        DataForChange.AltPhone = p.PhoneForBill;
                        DataForChange.Zip = p.Zip;

                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ProviderProfileInfo
        public async Task<bool> ProviderProfileInfo(ProviderList p)
        {
            try
            {
                if (p == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians.Where(W => W.PhysicianId == p.PhysicianId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        var fileName = Path.GetFileName(p.Photo.FileName);
                        //var fileName2 = Path.GetFileName(p.signature.FileName);
                        if ((fileName) == null)
                        {
                            fileName = string.Empty;
                        }


                        DataForChange.BusinessName = p.Bname;
                        DataForChange.BusinessWebsite = p.Bwebsite;
                        DataForChange.Photo = fileName;
                        //DataForChange.Signature = fileName2;
                        DataForChange.AdminNotes = p.Message;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region saveProviderbutton
        public bool SaveProvider(int[] checkboxes, int physicianid)
        {
            Physician phy = _context.Physicians.Where(x => x.PhysicianId == physicianid).FirstOrDefault();
            foreach (var i in checkboxes)
            {
                switch (i)
                {
                    case 1:
                        phy.IsAgreementDoc = new BitArray(1);
                        phy.IsAgreementDoc[0] = true; break;
                    case 2:
                        phy.IsBackgroundDoc = new BitArray(1);
                        phy.IsBackgroundDoc[0] = true; break;
                    case 3:
                        phy.IsCredentialDoc = new BitArray(1);
                        phy.IsCredentialDoc[0] = true; break;
                    case 4:
                        phy.IsNonDisclosureDoc = true; break;
                    case 5:
                        phy.IsLicenseDoc = new BitArray(1);
                        phy.IsLicenseDoc[0] = true; break;
                }

            }
            _context.Physicians.Update(phy);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region AddProviderAccount
        public bool AddProviderAccount(ProviderList PhysiciansData, int[] checkboxes, string UserId)
        {
            var Data = new Physician();
            var Aspnetuser = new AspNetUser();
            var AspNetUserRoles = new AspNetUserRole();
            var phyNoti = new PhysicianNotification();
            Guid g = Guid.NewGuid();
            Aspnetuser.Id = g.ToString();
            Aspnetuser.UserName = PhysiciansData.FirstName;
            Aspnetuser.PasswordHash = PhysiciansData.Password;
            Aspnetuser.Email = PhysiciansData.Email;
            Aspnetuser.PhoneNumber = PhysiciansData.Phone;
            Aspnetuser.CreatedDate = DateTime.Now;
            _context.AspNetUsers.Add(Aspnetuser);
            _context.SaveChanges();

            AspNetUserRoles.UserId = Aspnetuser.Id;
            AspNetUserRoles.RoleId = "3";
            _context.AspNetUserRoles.Add(AspNetUserRoles);
            _context.SaveChanges();

            Data.AspNetUserId = Aspnetuser.Id;
            Data.FirstName = PhysiciansData.FirstName;
            Data.LastName = PhysiciansData.LastName;
            Data.Mobile = PhysiciansData.Phone;
            Data.Email = PhysiciansData.Email;
            Data.MedicalLicense = PhysiciansData.MedLicence;
            Data.Npinumber = PhysiciansData.NpiNum;
            Data.SyncEmailAddress = PhysiciansData.SyncEmail;
            Data.Address1 = PhysiciansData.Add1;
            Data.Address2 = PhysiciansData.Add2;
            Data.City = PhysiciansData.City;
            Data.Zip = PhysiciansData.Zip;
            Data.Mobile = PhysiciansData.Phone;
            Data.BusinessName = PhysiciansData.Bname;
            Data.BusinessWebsite = PhysiciansData.Bwebsite;
            Data.AdminNotes = PhysiciansData.Message;
            Data.RoleId = Convert.ToInt32(PhysiciansData.Role);
            Data.IsDeleted = new BitArray(1);
            Data.Status = (short?)(ProviderStatus)PhysiciansData.Status;
            Data.CreatedBy = "0d15d42d-2f13-4d03-bc8a-2c57c34969ac";
            Data.AltPhone = PhysiciansData.PhoneForBill;
            Data.CreatedDate = DateTime.Now;

            if (PhysiciansData.signature != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, PhysiciansData.signature.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    PhysiciansData.signature.CopyTo(stream);
                }

                Data.Signature = PhysiciansData.signature.FileName;

            }
            if (PhysiciansData.Photo != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, PhysiciansData.Photo.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    PhysiciansData.Photo.CopyTo(stream);
                }

                Data.Photo = PhysiciansData.Photo.FileName;
            }
            if (checkboxes != null)
            {
                foreach (var i in checkboxes)
                {
                    switch (i)
                    {
                        case 1:
                            Data.IsAgreementDoc = new BitArray(1);
                            Data.IsAgreementDoc[0] = true; break;
                        case 2:
                            Data.IsBackgroundDoc = new BitArray(1);
                            Data.IsBackgroundDoc[0] = true; break;
                        case 3:
                            Data.IsCredentialDoc = new BitArray(1);
                            Data.IsCredentialDoc[0] = true; break;
                        case 4:
                            Data.IsNonDisclosureDoc = true; break;
                        case 5:
                            Data.IsLicenseDoc = new BitArray(1);
                            Data.IsLicenseDoc[0] = true; break;
                    }
                }

                _context.Physicians.Add(Data);
                _context.SaveChanges();

            }
            if (PhysiciansData.Regionsid != null)
            {
                List<int> regions = _context.PhysicianRegions.Where(r => r.PhysicianId == Data.PhysicianId).Select(req => req.RegionId).ToList();
                List<int> priceList = PhysiciansData.Regionsid.Split(',').Select(int.Parse).ToList();
                foreach (var item in priceList)
                {
                    if (regions.Contains(item))
                    {
                        regions.Remove(item);
                    }
                    else
                    {
                        PhysicianRegion ar = new()
                        {
                            RegionId = item,
                            PhysicianId = (int)Data.PhysicianId
                        };
                        _context.PhysicianRegions.Add(ar);
                        _context.SaveChanges();
                        regions.Remove(item);
                    }
                }
                if (regions.Count > 0)
                {
                    foreach (var item in regions)
                    {
                        PhysicianRegion ar = _context.PhysicianRegions.Where(r => r.PhysicianId == Data.PhysicianId && r.RegionId == item).First();
                        _context.PhysicianRegions.Remove(ar);
                        _context.SaveChanges();
                    }
                }
            }
            phyNoti.IsNotificationStopped = false;
            phyNoti.PhysicianId = Data.PhysicianId;
            _context.PhysicianNotifications.Add(phyNoti);
            _context.SaveChanges();

            return true;
        }
        #endregion

        #region DeleteProvider
        public bool DeleteProvider(int PhysicianId)
        {
            Physician phy = _context.Physicians.Where(x => x.PhysicianId == PhysicianId).FirstOrDefault();
            phy.IsDeleted[0] = true;
            _context.Physicians.Update(phy);
            _context.SaveChanges();
            return true;

        }
        #endregion

        #region RolebyAccountType
        public List<Menu> RolebyAccountType(AccountType Account)
        {
            int accounttype = (int)Account;
            if (accounttype == 4)
            {
                var result = _context.Menus.ToList();
                return result;

            }
            else
            {
                var result = _context.Menus
                  .Where(req => req.AccountType == 4 || req.AccountType == accounttype)
                  .ToList();
                return result;
            }

        }
        #endregion

        #region SaveEditRole
        public bool SaveEditRole(AccessModel roles, string userId)
        {
            var data = new Role();
            data.Name = roles.Role;
            data.AccountType = (short)roles.AccountType;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = userId;

            _context.Roles.Add(data);
            _context.SaveChanges();

            List<int> menus = roles.files.Split(',').Select(int.Parse).ToList();

            foreach (var item in menus)
            {
                var obj = new RoleMenu();
                obj.RoleId = data.RoleId;
                obj.MenuId = item;
                _context.RoleMenus.Add(obj);
                _context.SaveChanges();
            }


            return true;
        }
        #endregion

        #region ViewEditRole
        public AccessModel ViewEditRole(int RoleId)
        {
            AccessModel? v = (from p in _context.Roles

                              where p.RoleId == RoleId
                              select new AccessModel
                              {
                                  Role = p.Name,
                                  AccountType = (AccountType)p.AccountType,
                              }).FirstOrDefault();
            List<Menu> Menu = _context.Menus
                .Where(req => req.AccountType == (short)v.AccountType).ToList();
            v.menus = Menu;
            List<RoleMenu> rm = _context.RoleMenus
                                .Where(obj => obj.RoleId == RoleId).ToList();
            v.rolemenus = rm;
            return v;
        }
        #endregion

        #region SaveEditRoleAccess     
        public bool SaveEditRoleAccess(AccessModel roles)
        {
            List<int> selectedmenus = roles.files.Split(',').Select(int.Parse).ToList();
            List<int> rolemenus = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).Select(req => req.RoleMenuId).ToList();

            if (rolemenus.Count > 0)
            {
                foreach (var item in rolemenus)
                {
                    RoleMenu ar = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).First();
                    _context.RoleMenus.Remove(ar);
                    _context.SaveChanges();
                }
            }
            foreach (var item in selectedmenus)
            {
                RoleMenu ar = new()
                {
                    RoleId = roles.RoleId,
                    MenuId = item
                };
                _context.RoleMenus.Update(ar);
                _context.SaveChanges();
                //regions.Remove(item);
            }
            return true;
        }
        #endregion

        #region DeleteRole
        public bool DeleteRole(int RoleId)
        {
            Role phy = _context.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();
            phy.IsDeleted[0] = true;
            _context.Roles.Update(phy);
            _context.SaveChanges();
            return true;

        }
        #endregion

        #region RoleViewBag
        public List<AspNetRole> Role()
        {
            var Role = _context.AspNetRoles.ToList();
            return (Role);
        }
        #endregion
        public SearchInputs AccessAccount(int pageinfo)
        {
            var list = _context.Roles.Where(r => r.IsDeleted == new BitArray(1)).ToList();
            var pagesize = 5;
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<Role> list1 = list.Skip((pageinfo - 1) * pagesize).Take(pagesize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                role = list1,
                CurrentPage = pageinfo,
                TotalPages = totalPages,
                PageSize = pagesize,
            };

            return datanew;
        }

        #region UserAccess
        public SearchInputs UserAccessData(string AccountType, int pageinfo)
        {
            var pageSize = 5;
            var result = (from aspuser in _context.AspNetUsers
                          join admin in _context.Admins
                          on aspuser.Id equals admin.AspNetUserId into AdminGroup
                          from admin in AdminGroup.DefaultIfEmpty()
                          join physician in _context.Physicians
                          on aspuser.Id equals physician.AspNetUserId into PhyGroup
                          from physician in PhyGroup.DefaultIfEmpty()
                          where (admin != null || physician != null) && (admin.IsDeleted == new BitArray(1) || physician.IsDeleted == new BitArray(1))
                          select new UserAccessData
                          {
                              Id = admin != null ? admin.AdminId : (physician != null ? physician.PhysicianId : 0),
                              AccountType = admin != null ? "Admin" : (physician != null ? "Physician" : null),
                              AccountPOC = admin != null ? admin.FirstName + " " + admin.LastName : (physician != null ? physician.FirstName + " " + physician.LastName : null),
                              Status = (int)(admin != null ? admin.Status : (physician != null ? physician.Status : null)),
                              Phone = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                              OpenReq = _context.Requests.Count(r => r.PhysicianId == physician.PhysicianId),
                              isAdmin = admin != null
                          }).ToList();
            if (AccountType != null)
            {
                result = result.Where(req => req.AccountType == AccountType).ToList();
            }
         
          
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            List<UserAccessData> list1 = result.Skip((pageinfo - 1) * pageSize).Take(pageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                ud = list1,
                CurrentPage = pageinfo,
                TotalPages = totalPages,
                PageSize = pageSize,
            };

            return datanew;
        }
        #endregion

        #region prefilleddataofvendors
        public HealthProfessional EditPartners(int VendorId)
        {
            var result = _context.HealthProfessionals.Where(Req => Req.VendorId == VendorId).FirstOrDefault();
            return result;
        }
        #endregion

        #region GetPartnersByProfession
        public SearchInputs GetPartnersByProfession(string searchValue, int Profession, int pageinfo)
        {
            var pageSize = 3;
            if (pageinfo == 0)
            {
                pageinfo = 1;
            }
            var result = (from Hp in _context.HealthProfessionals
                          join Hpt in _context.HealthProfessionalTypes
                          on Hp.Profession equals Hpt.HealthProfessionalId into AdminGroup
                          from asp in AdminGroup.DefaultIfEmpty()
                          where (searchValue == null || Hp.VendorName.Contains(searchValue))
                             && (Profession == 0 || Hp.Profession == Profession) && (Hp.IsDeleted == new BitArray(1))
                          select new Partners
                          {
                              VendorId = Hp.VendorId,
                              Profession = Hp.VendorName,
                              Business = asp.ProfessionName,
                              Email = Hp.Email,
                              FaxNumber = Hp.FaxNumber,
                              PhoneNumber = Hp.PhoneNumber,
                              BusinessNumber = Hp.BusinessContact,
                          }).ToList();
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            List<Partners> list1 = result.Skip((pageinfo - 1) * pageSize).Take(pageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                pt = list1,
                CurrentPage = pageinfo,
                TotalPages = totalPages,
                PageSize = pageSize,
            };

            return datanew;
        }
        #endregion

        #region AddBusiness
        public bool AddBusiness(HealthProfessional hp)
        {
            var Data = _context.HealthProfessionals.Where(req => req.VendorId == hp.VendorId).FirstOrDefault();
            if (Data != null)
            {
                Data.Profession = hp.Profession;
                Data.VendorName = hp.VendorName;
                Data.Email = hp.Email;
                Data.FaxNumber = hp.FaxNumber;
                Data.PhoneNumber = hp.PhoneNumber;
                Data.BusinessContact = hp.BusinessContact;
                Data.Address = hp.Address;
                Data.City = hp.City;
                Data.Zip = hp.Zip;
                Data.State = hp.State;
                _context.HealthProfessionals.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                var data = new HealthProfessional();

                data.Profession = hp.Profession;
                data.VendorName = hp.VendorName;
                data.Email = hp.Email;
                data.FaxNumber = hp.FaxNumber;
                data.PhoneNumber = hp.PhoneNumber;
                data.BusinessContact = hp.BusinessContact;
                data.Address = hp.Address;
                data.City = hp.City;
                data.Zip = hp.Zip;
                data.State = hp.State;
                data.IsDeleted = new BitArray(1);
                data.CreatedDate = DateTime.Now;
                _context.HealthProfessionals.Add(data);
                _context.SaveChanges();
                return true;
            }
        }
        #endregion

        #region deletevendor
        public bool DeleteBusiness(int vendorId)
        {
            HealthProfessional hp = _context.HealthProfessionals.Where(x => x.VendorId == vendorId).FirstOrDefault();
            hp.IsDeleted[0] = true;
            _context.HealthProfessionals.Update(hp);
            _context.SaveChanges();
            return true;
        }

        #endregion

        #region RecordsPatientExplore
        public List<PatientDashboard> RecordsPatientExplore(int UserId)
        {
            List<PatientDashboard> allData = (from req in _context.Requests
                                              join reqClient in _context.RequestClients
                                              on req.RequestId equals reqClient.RequestId into reqClientGroup
                                              from rc in reqClientGroup.DefaultIfEmpty()
                                              join phys in _context.Physicians
                                              on req.PhysicianId equals phys.PhysicianId into physGroup
                                              from p in physGroup.DefaultIfEmpty()
                                              where req.UserId == UserId
                                              select new PatientDashboard
                                              {
                                                  PatientName = rc.FirstName + " " + rc.LastName,
                                                  RequestedDate = ((DateTime)req.CreatedDate),
                                                  Confirmation = req.ConfirmationNumber,
                                                  Physician = p.FirstName + " " + p.LastName,
                                                  ConcludedDate = (DateTime)req.CreatedDate,
                                                  Status = (Status)req.Status,
                                                  RequestTypeId = req.RequestTypeId,
                                                  RequestId = req.RequestId
                                              }).ToList();
            return allData;
        }
        #endregion

        #region unblockreq
        public bool UnBlock(int reqId)
        {
            BlockRequest r = _context.BlockRequests.Where(x => x.RequestId == reqId).FirstOrDefault();
            r.IsActive[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.BlockRequests.Update(r);
            _context.SaveChanges();

            HaloDocDataAccess.DataModels.Request req = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            req.Status = 1;
            req.ModifiedDate = DateTime.Now;
            _context.Requests.Update(req);
            _context.SaveChanges();

            return true;

        }
        #endregion

        #region blockreqshow
        public BlockHistory RecordsBlock(BlockHistory formData)
        {
            var data = (from req in _context.BlockRequests
                        join r in _context.Requests on req.RequestId equals r.RequestId
                        where (string.IsNullOrEmpty(formData.PatientName) || r.FirstName.Contains(formData.PatientName))
                           && (formData.createdDate == null || req.CreatedDate.Value.Date == formData.createdDate)
                           && (string.IsNullOrEmpty(formData.Email) || req.Email.Contains(formData.Email))
                           && (string.IsNullOrEmpty(formData.Mobile) || req.PhoneNumber.Contains(formData.Mobile))
                        select new PatientDashboard
                        {
                            PatientName = r.FirstName,
                            Email = req.Email,
                            createdDate = (DateTime)req.CreatedDate,
                            IsActive = req.IsActive[0],
                            RequestId = Convert.ToInt32(req.RequestId),
                            Mobile = req.PhoneNumber,
                            Notes = req.Reason
                        }).ToList();
            int totalItemCount = data.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)formData.PageSize);
            List<PatientDashboard> list1 = data.Skip((formData.CurrentPage - 1) * formData.PageSize).Take(formData.PageSize).ToList();
            BlockHistory datanew = new BlockHistory
            {
                pd = list1,
                CurrentPage = formData.CurrentPage,
                TotalPages = totalPages,
                PageSize = formData.PageSize,
            };

            return datanew;
        }
        #endregion

        #region RecordsSearch
        public SearchInputs RecordsSearch(SearchInputs rm)
        {
            List<SearchRecords> allData = (from req in _context.Requests
                                           join reqClient in _context.RequestClients
                                           on req.RequestId equals reqClient.RequestId into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _context.Physicians
                                           on req.PhysicianId equals phys.PhysicianId into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join nts in _context.RequestNotes
                                           on req.RequestId equals nts.RequestId into ntsgrp
                                           from nt in ntsgrp.DefaultIfEmpty()
                                           where (req.IsDeleted == new BitArray(1) && rm.ReqStatus == 0 || req.Status == rm.ReqStatus) &&
                                                    (rm.RequestTypeID == 0 || req.RequestTypeId == rm.RequestTypeID) &&
                                                    (!rm.StartDOS.HasValue || req.CreatedDate >= rm.StartDOS.Value.Date) &&
                                                    (!rm.EndDOS.HasValue || req.CreatedDate <= rm.EndDOS.Value.Date) &&
                                                    (rm.PatientName.IsNullOrEmpty() || (req.FirstName + " " + req.LastName).ToLower().Contains(rm.PatientName.ToLower())) &&
                                                    (rm.PhyName.IsNullOrEmpty() || (p.FirstName + " " + p.LastName).ToLower().Contains(rm.PhyName.ToLower())) &&
                                                    (rm.Email.IsNullOrEmpty() || rc.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                                    (rm.Mobile.IsNullOrEmpty() || rc.PhoneNumber.ToLower().Contains(rm.Mobile.ToLower()))
                                           orderby req.CreatedDate
                                           select new SearchRecords
                                           {
                                               //Modifieddate = req.Modifieddate,
                                               PatientName = req.FirstName + " " + req.LastName,
                                               RequestTypeID = req.RequestTypeId,
                                               DateOfService = req.CreatedDate,
                                               Email = rc.Email ?? "-",
                                               Mobile = rc.PhoneNumber ?? "-",
                                               Address = rc.Address + "," + rc.City,
                                               Zip = rc.ZipCode,
                                               Status = (Status)req.Status,
                                               Physician = p.FirstName + " " + p.LastName ?? "-",
                                               PhyNotes = nt != null ? nt.PhysicianNotes ?? "-" : "-",
                                               AdminNotes = nt != null ? nt.AdminNotes ?? "-" : "-",
                                               PatientNotes = rc.Notes ?? "-",
                                               RequestID = req.RequestId,
                                               Modifieddate = req.ModifiedDate
                                           }).ToList();



            for (int i = 0; i < allData.Count; i++)
            {
                if (allData[i].Status == (Status)9)
                {
                    allData[i].CloseCaseDate = allData[i].Modifieddate;
                }
                else
                {
                    allData[i].CloseCaseDate = null;
                }
                if (allData[i].Status == (Status)3 && allData[i].Physician != null)
                {
                    var res = _context.RequestStatusLogs.FirstOrDefault(x => (x.Status == 3) && (x.RequestId == allData[i].RequestID));
                    allData[i].CancelByPhyNotes = res.Notes;
                }
            }

            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SearchRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                sr = list1,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
            };

            return datanew;
        }


        #endregion

        #region deleterecords
        public bool RecordsDelete(int reqId)
        {
            HaloDocDataAccess.DataModels.Request hp = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            hp.IsDeleted[0] = true;
            _context.Requests.Update(hp);
            _context.SaveChanges();
            return true;
        }

        #endregion

        #region emailogs
        public SearchInputs EmailLogs(SearchInputs rm)
        {
            List<EmailLogRecords> allData = (from em in _context.EmailLogs
                                             join req in _context.Requests
                                             on em.RequestId equals req.RequestId into Group
                                             from rc in Group.DefaultIfEmpty()
                                             where (rm.Role == 0 || em.RoleId == rm.Role) &&
                                                   (!rm.StartDOS.HasValue || em.CreateDate == rm.StartDOS.Value.Date) &&
                                                   (!rm.EndDOS.HasValue || em.SentDate == rm.EndDOS.Value.Date) &&
                                                   (rm.FirstName.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(rm.FirstName.ToLower())) &&
                                                   (rm.Email.IsNullOrEmpty() || em.EmailId.ToLower().Contains(rm.Email.ToLower()))
                                             select new EmailLogRecords
                                             {
                                                 Recipient = rc.FirstName,
                                                 ConfirmationNumber = em.ConfirmationNumber,
                                                 CreateDate = em.CreateDate,
                                                 SentDate = (DateTime)em.SentDate,
                                                 RoleId = (AccountType)em.RoleId,
                                                 EmailId = em.EmailId,
                                                 IsEmailSent = (em.IsEmailSent == new BitArray(0) ? "No" : "Yes"),
                                                 SentTries = em.SentTries,
                                                 Action = (EmailAction)em.Action,

                                             }).ToList();

            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                el = list1,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
            };

            return datanew;
        }

        #endregion

        public SearchInputs RecordsSMSLog(SearchInputs rm)
        {
            List<EmailLogRecords> allData = (from em in _context.Smslogs
                                             join req in _context.Requests
                                             on em.RequestId equals req.RequestId into Group
                                             from rc in Group.DefaultIfEmpty()
                                             where (rm.Role == 0 || em.RoleId == rm.Role) &&
                                                   (!rm.StartDOS.HasValue || em.CreateDate.Date == rm.StartDOS.Value.Date) &&
                                                   (!rm.EndDOS.HasValue || em.SentDate == rm.EndDOS.Value.Date) &&
                                                   (rm.FirstName.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(rm.FirstName.ToLower())) &&
                                                   (rm.Mobile.IsNullOrEmpty() || em.MobileNumber.ToLower().Contains(rm.Mobile.ToLower()))
                                             select new EmailLogRecords
                                             {
                                                 Recipient = rc.FirstName,
                                                 ConfirmationNumber = em.ConfirmationNumber,
                                                 CreateDate = em.CreateDate,
                                                 SentDate = (DateTime)em.SentDate,
                                                 RoleId = (AccountType)em.RoleId,
                                                 Mobile = em.MobileNumber,
                                                 IsEmailSent = (em.IsSmssent == new BitArray(0) ? "No" : "Yes"),
                                                 SentTries = em.SentTries,
                                                 Action = (EmailAction)em.Action,

                                             }).ToList();
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();
            SearchInputs datanew = new SearchInputs
            {
                el = list1,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
            };

            return datanew;
        }
    }
}

