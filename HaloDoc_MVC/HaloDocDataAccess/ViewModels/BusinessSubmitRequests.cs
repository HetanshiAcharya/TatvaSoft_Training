using HaloDocDataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaloDocDataAccess.ViewModels.Constant;

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
        public StateLists? RegionId { get; set; }
        public int? State { get; set; }
        public string? ZipCode { get; set; }
        public string? CreatedBy { get; set; }



    }

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
        public string? Street { get; set; }
        public StateLists? RegionId { get; set; }
        public int? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
    }

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
        public StateLists? RegionId { get; set; }
        public int? State { get; set; }
        public string? ZipCode { get; set; }
        public string? RoomSuite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
    }
    public class PatientSubmitRequests
    {
        public string Pass { get; set; }
        [Compare("Pass", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPass { get; set; }
        public int Id { get; set; }
        public string? Symptoms { get; set; }
        public string? FirstName { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public StateLists? RegionId { get; set; }
        public int? State { get; set; }
        public string? ZipCode { get; set; }
        public string? RoomSite { get; set; }
        public string? UploadImage { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? FF_RelationWithPatient { get; set; }
    }
    public class ViewDocument
    {

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? SearchInput { get; set; }
        public int? RegionId { get; set; }
        
        public int? RequestType { get; set; }
        public string? Status { get; set; }
        public DateTime? DOB { get; set; }
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public bool? IsAscending { get; set; } = true;
        public string? SortedColumn { get; set; } = "RequestedDate";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; }
        public int RequestId { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<RequestWiseFile> requestwisefiles { get; set; }
        public IFormFile File { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public string Uploader { get; set; }
        public int RequestwisefilesId { get; set; }
    }

}
