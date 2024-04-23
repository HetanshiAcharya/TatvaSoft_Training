using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaloDocDataAccess.ViewModels.Constant;

namespace HaloDocDataAccess.ViewModels
{
    public class AdminDashboardList
    {
        public int Requestclientid { get; set; }
        public string? PatientName { get; set; }
        public DateTime Dob { get; set; }
        public string? PatientId { get; set; }
        public string? BMonth { get; set; }
        public int? Bdate { get; set; }
        public int? BYear { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? RequestorPhoneNumber { get; set; }
        public int? RequestID { get; set; }
        public int? RequestTypeID { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int? ProviderID { get; set; }
        public int? Status { get; set; }
        public string? ProviderName { get; set; }
        public string? Region { get; set; }
        public short ADStatus { get; set; }

    }
    public class CountStatusWiseRequestModel
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public int Regionid { get; set; }

        public List<AdminDashboardList>? adminDashboardList { get; set; }
    }
    public class Constant
    {
        public enum RequestType
        {
            Patient = 1,
            Business,
            Family,
            Concierge
        }
        public enum AdminDashStatus
        {
            New = 1,
            Pending,
            Active,
            Conclude,
            ToClose,
            UnPaid
        }
        public enum Status
        {
            Unassigned = 1,
            Accepted, Cancelled, MDEnRoute, MDONSite, Conclude, CancelledByPatients, Closed, Unpaid, Clear,
            Block

        }
        public enum ProviderStatus
        {
            Active = 1,
            Pending = 2,
            NotActive = 3
        }
        public enum onCallStatus
        {
            UnAvailable = 0,
            Available
        }
        public enum AccountType
        {
            All = 0,
            Patient,
            Provider,
            Admin,

        }
        public enum EmailAction
        {
            SendOrder = 1,
            Request,
            SendLink,
            SendAgreement,
            Forgot,
            NewRegistration,
            contact
        }
        public enum StateLists
        {
            Ahmedabad = 1,
            Gandhinagar,
            Pune,
            Mumbai,
            Manali
        }
    }
    public class ViewCaseData
    {
        public int? RequestClientId { get; set; }
        public int? RequestID { get; set; }
        public string? PatientNotes { get; set; }
        public string? ConfirmationNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime Dob { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? Email { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Room { get; set; }
        public int? RequestTypeID { get; set; }
        public int? UserID { get; set; }
    }
    public class TransferNotesData
    {
        public int Requeststatuslogid { get; set; }
        public int Requestid { get; set; }
        public int? Physicianid { get; set; }
        public int? Transtophysicianid { get; set; }
        public DateTime Createddate { get; set; }
        public string? Notes { get; set; }
        public short Status { get; set; }
        public BitArray? Transtoadmin { get; set; }
        public string? TransPhysician { get; set; }
        public string? Admin { get; set; }
        public string? Physician { get; set; }
        public string TransferNotes => $"{Admin} transferred <b> {Physician}  </b> to <b> {TransPhysician} </b> on {Createddate}: <b>{Notes}</b>";
    }
    public class ViewNotes
    {
        public int? Requestnotesid { get; set; }
        public int? Requestid { get; set; }
        public string? Strmonth { get; set; }
        public int? Intyear { get; set; }
        public int? Intdate { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNotes { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public short Status { get; set; }
        public string? Ip { get; set; }
        public virtual Request Request { get; set; } = null!;
        public List<TransferNotesData> transfernotes { get; set; } = null!;
        public List<TransferNotesData> cancel { get; set; } = null!;
        public List<TransferNotesData> cancelbyphysician { get; set; }
    }
    public class Orders
    {
        public int? Requestclientid { get; set; }
        public List<HealthProfessionalType> ProfessionTypes { get; set; }
        public List<HealthProfessional> HealthProfessionals { get; set; }
        public string? BusinessContact { get; set; }
        public string? Email { get; set; }
        public string? FaxNumber { get; set; }
        public string? Prescription { get; set; }
        public string? NumberOfRefills { get; set; }

    }
    public class sendAgreement
    {
        public int ReqId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
    public class sendLink
    {
        public int ReqId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }


    }
    public class ViewCloseCaseModel
    {
        public List<RequestWiseFile> documentslist { get; set; } = null;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public int RequestID { get; set; }
        public int RequestWiseFileID { get; set; }
        public string RC_FirstName { get; set; }
        public string RC_LastName { get; set; }
        public string RC_Email { get; set; }
        public string? BMonth { get; set; }
        public int? Bdate { get; set; }
        public DateTime RC_Dob { get; set; }
        public int? BYear { get; set; }
        public string RC_PhoneNumber { get; set; }
        public int RequestClientID { get; set; }
    }
    public class PaginatedViewModel
    {
        public List<AdminDashboardList>? adl { get; set; }
        public List<PatientDashboard>? pd { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? SearchInput { get; set; }
        public int? RegionId { get; set; }
        public int? RequestType { get; set; }
        public string? Status { get; set; }
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public bool? IsAscending { get; set; } = true;
        public string? SortedColumn { get; set; } = "RequestedDate";

    }
    public class EncounterInfo
    {
        public int? AdminId { get; set; }
        public int? PhysicianId { get; set; }
        public int? RequestID { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public DateTime Bdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? HistoryOfIllness { get; set; } = string.Empty;
        public string? MedicalHist { get; set; } = string.Empty;
        public string? Medications { get; set; } = string.Empty;
        public string? Allergies { get; set; } = string.Empty;
        public string? Temp { get; set; } = string.Empty;
        public string? HR { get; set; } = string.Empty;
        public string? RR { get; set; } = string.Empty;
        public string? BPS { get; set; } = string.Empty;
        public string? BPD { get; set; } = string.Empty;
        public string? O2 { get; set; } = string.Empty;
        public string? Pain { get; set; } = string.Empty;
        public string? heent { get; set; } = string.Empty;
        public string? CV { get; set; } = string.Empty;
        public string? Chest { get; set; } = string.Empty;
        public string? ABD { get; set; } = string.Empty;
        public string? Extr { get; set; } = string.Empty;
        public string? Skin { get; set; } = string.Empty;
        public string? Neuro { get; set; } = string.Empty;
        public string? Other { get; set; } = string.Empty;
        public string? Diagnosis { get; set; } = string.Empty;
        public string? TrtPlan { get; set; } = string.Empty;
        public string? MedDispensed { get; set; } = string.Empty;
        public string? Procedures { get; set; } = string.Empty;
        public string? Followup { get; set; } = string.Empty;
    }
    public class AdminDetailsInfo
    {
        public int? AdminId { get; set; }
        public string? AspNetUserId { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public short? Status { get; set; }
        public int? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ConfEmail { get; set; }
        public string? Phone { get; set; }
        public StateLists Regionid { get; set; }
        public string? Regionsid { get; set; }
        public List<Region>? Regionids { get; set; }
        public string? Add1 { get; set; }
        public string? Add2 { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }
        public string? Zip { get; set; }
        public string? PhoneForBill { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public class Regions
        {
            public int? RegionId { get; set; }
            public string? Name { get; set; }
        }

    }
    public class ProviderMenu
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public IEnumerable<ProviderList>? ProviderLists { get; set; }
    }
    public class ProviderList
    {
        public DateTime? CreatedDate;
        public int[] checkboxes { get; set; }
        public string Role { get; set; }
        public int? onCallStatus { get; set; } = 0;
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public ProviderStatus Status { get; set; }

        public int? shiftid { get; set; }
        public int? RoleId { get; set; }
        public string Phone { get; set; }
        public string MedLicence { get; set; }
        public string NpiNum { get; set; }
        public string SyncEmail { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneForBill { get; set; }
        public string Bname { get; set; }
        public string Bwebsite { get; set; }
        public int? Regionid { get; set; }
        public string? Regionsid { get; set; }
        public List<Region>? Regionids { get; set; }
        public IFormFile? Photo { get; set; } = null;
        public IFormFile? signature { get; set; } = null;
        public bool isAgreementDoc { get; set; }
        public bool isBackgroundDoc { get; set; }
        public bool isLicenseDoc { get; set; }
        public bool isCredentialDoc { get; set; }
        public bool isNonDisclosureDoc { get; set; }
        ///
        public string Message { get; set; }
        public int PhysicianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? OnCallStatus { get; set; }
        public bool Notification { get; set; }

    }
    public class AccessModel
    {
        public string Role { get; set; }
        public int RoleId { get; set; }

        public AccountType AccountType { get; set; }
        public string files { get; set; }
        public List<Menu> menus { get; set; }
        public List<RoleMenu> rolemenus { get; set; }

    }
    public class UserAccessData
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string AccountType { get; set; }
        public string AccountPOC { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public int OpenReq { get; set; }
        public int Id { get; set; }
        public bool isAdmin { get; set; }

    }
    public class Partners
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int VendorId { get; set; }
        public string Profession { get; set; }
        public string Business { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
    }
    public class AdminProfile
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "ConfirmEmail is required")]
        [Compare("Email", ErrorMessage = "Email doesn't match.")]
        public string ConformEmail { get; set; }
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public List<Region> RegionIds { get; set; }
        public string? RegionIdList { get; set; }
        public int AdminId { get; set; }
    }

}
