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
    public class AdminLogin
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class CountStatusWiseRequestModel
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
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
}
