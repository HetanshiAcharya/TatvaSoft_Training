using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Http;

namespace HaloDocDataAccess.ViewModels
{
    public class FamilySubmitRequests
    {
        public string? FF_FirstName { get; set; }
        public string? FF_LastName { get; set; }
        public string? FF_PhoneNumber { get; set; }
        public string? FF_Email { get; set; }
        public string? FF_RelationWithPatient { get; set; }
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }
        public string? FirstName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? RoomSuite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
    }
}
