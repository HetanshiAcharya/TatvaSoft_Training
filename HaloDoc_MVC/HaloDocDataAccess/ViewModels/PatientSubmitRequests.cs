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
    public class PatientSubmitRequests
    {
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
        public string? State { get; set; }
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
