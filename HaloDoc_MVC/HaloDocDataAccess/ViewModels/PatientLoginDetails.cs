using HaloDocDataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    public class PatientProfile
    {
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

}
