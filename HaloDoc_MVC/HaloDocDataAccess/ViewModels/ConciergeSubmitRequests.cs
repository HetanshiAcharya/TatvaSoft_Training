using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocDataAccess.ViewModels
{
    public class ConciergeSubmitRequests
    {
        public string? CON_FirstName { get; set; }
        public string? CON_LastName { get; set; }
        public string? CON_PhoneNumber { get; set; }
        public string? CON_Email { get; set; }
        public string? CON_PropertyName { get; set; }
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }
        public string? FirstName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CON_Street { get; set; }
        public string? CON_City { get; set; }
        public string? CON_State { get; set; }
        public string? CON_ZipCode { get; set; }
        public string? RoomSuite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
    }
}
