using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocDataAccess.ViewModels
{
    public class BusinessSubmitRequests
    {
    
        public string? bus_FirstName { get; set; } = string.Empty;
        public string? bus_LastName { get; set; }
        public string? bus_PhoneNumber { get; set; }
        public string? bus_Email { get; set; }
        public string? bus_PropertyName { get; set; }
        public string? bus_CaseNum { get; set; }
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }
        public string? FirstName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RoomSuite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? CreatedBy { get; set; }
        public int RegionId { get; set; }



    }
}
