using HaloDocDataAccess.DataModels;
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
    public class PatientLoginDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class PatientForgotPassword
    {
        [Required]
        public string Email { get; set; }

    }
    public class PatientResetPassword
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
    public class PatientDashboard
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<Request> Requests { get; set; }
        public List<int> DocumentCount { get; set; }

        public DateTime createdDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ConcludedDate { get; set; }
        public Status Status { get; set; }
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }

        public string PatientName { get; set; }
        public string Confirmation { get; set; }
        public string Physician { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }
    public class PatientProfile
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? CreatedBy { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Date { get; set; }
        public string? Type { get; set; }
        public string? CountryCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime DOB { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }

    }
    public class BlockHistory
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? createdDate { get; set; }
        public List<PatientDashboard> pd { get; set; }
    }

    public class SearchInputs
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int ReqStatus { get; set; }
        public string PatientName { get; set; }
        public int RequestTypeID { get; set; }
        public DateTime? StartDOS { get; set; }
        public DateTime? EndDOS { get; set; }
        public string PhyName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public List<SearchRecords> sr { get; set; }
        public List<PatientProfile> pp { get; set; }
        public List<Partners> pt { get; set; }
        public List<EmailLogRecords> el { get; set; }
        public List<Role> role { get; set; }
        public List<UserAccessData> ud { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Role { get; set; }
    }

    public class SearchRecords
    {
        public string PatientName { get; set; }
        public string Requestor { get; set; }
        public int RequestTypeID { get; set; }
        public int RequestID { get; set; }
        public DateTime? DateOfService { get; set; }
        public DateTime? CloseCaseDate { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }
        public Status Status { get; set; }
        public string? Physician { get; set; }
        public string? PhyNotes { get; set; }
        public string? CancelByPhyNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNotes { get; set; }
        public DateTime? Modifieddate { get; set; }
    }

    public class EmailLogRecords
    {
        public string Recipient { get; set; }
        public string EmailId { get; set; } = null!;
        public string? ConfirmationNumber { get; set; }
        public AccountType RoleId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SentDate { get; set; }
        public string IsEmailSent { get; set; }
        public int? SentTries { get; set; }
        public EmailAction Action { get; set; }
        public string Mobile { get; set; }
    }

}