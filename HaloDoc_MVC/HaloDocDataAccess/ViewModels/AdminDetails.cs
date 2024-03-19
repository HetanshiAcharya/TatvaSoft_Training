using HaloDocDataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class ViewNotes
    {
        public int Requestclientid { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? PhysicianName { get; set; }
        public string? AdminNotes { get; set; }
        public string? TransferNotes { get; set; }
        public string? TextBox { get; set; }

        public List<RequestStatusLog>? Statuslogs { get; set; }
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
    public class ViewCloseCaseModel
    {
        public List<ViewDocument> documentslist { get; set; } = null;
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

    }
    public class EncounterInfo
    {
        public int? RequestID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public DateTime Bdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string HistoryOfIllness { get; set; }
        public string MedicalHist { get; set; }
        public string Medications { get; set; }
        public string Allergies { get; set; }
        public string Temp { get; set; }
        public string HR { get; set; }
        public string RR{ get; set; }
        public string BP { get; set; }
        //BP again
        public string O2 { get; set; }
        public string Pain { get; set; }
        public string heent { get; set; }
        public string CV { get; set; }
        public string Chest { get; set; }
        public string ABD { get; set; }
        public string Extr { get; set; }
        public string Skin { get; set; }
        public string Neuro { get; set; }
        public string Other { get; set; }
        public string Diagnosis { get; set; }
        public string TrtPlan { get; set; }
        public string MedDispensed { get; set; }
        public string Procedures { get; set; }
        public string Followup { get; set; }

    }
    public class AdminDetailsInfo
    {
        public int? AdminId { get; set; }
        public string AspNetUserId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public short? Status { get; set; }
        public int? Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ConfEmail { get; set; }
        public string Phone { get; set; }
        public int? Regionid { get; set; }
        public string? Regionsid { get; set; }
        public List<Region>? Regionids { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneForBill { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }


    }
}
