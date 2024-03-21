using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocRepository.Interface
{
    public interface IAdminService
    {
       //bool AdminAuthentication(AdminLogin userDetails);
        public List<AdminDashboardList> NewRequestData();
        public PaginatedViewModel Indexdata();
        public PaginatedViewModel GetRequests(PaginatedViewModel data);
        public ViewCaseData GetRequestForViewCase(int id);
        public ViewCaseData NewRequestData(int? RId, int? RTId);
        public ViewNotes ViewNotes(int RequestId);
        public bool ViewNotesUpdate(string? adminnotes, string? physiciannotes, int RequestID);
        public List<HealthProfessional> VendorByProfession(int Professionid);
        public HealthProfessional SendOrdersInfo(int selectedValue);
        public ViewCaseData Edit(ViewCaseData vdvc, int? RId, int? RTId);
        public List<Physician> ProviderbyRegion(int Regionid);
        public List<Region> AssignCase();
        public List<CaseTag> CancelCase();
        public List<HealthProfessionalType> Professions();
        public bool SendOrders(int requestid, OrderDetail o);
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes);
        public void CancelCaseInfo(int casetagId, string Notes, int RequestId);
        public bool BlockCaseInfo(int requestId, string notes);
        public void DeleteFile(int requestid, int reqwisefileid);
        public void ClearCase(int RequestId);
        public void TransferCaseInfo(int RequestId, int PhysicianId, string Notes);
        public bool SendAgreement(sendAgreement sendAgreement);
        public bool SendAgreementfromUploads(sendAgreement sendAgreement);

        public Boolean SendAgreement_accept(int RequestID);
        public Boolean SendAgreement_Reject(int RequestID, string Notes);
        public void CancelAgreementSubmit(int Reqid, string Description);
        public List<AdminDashboardList> InfoByRegion(int Regionid);
        public ViewCloseCaseModel CloseCaseData(int RequestID);
        public bool EditForCloseCase(ViewCloseCaseModel model);
        public bool CloseCase(int RequestID);
        public EncounterInfo Encounterinfo(int rId);
        public Task<AdminDetailsInfo> GetProfileDetails(int id);
        public Task<bool> EditPassword(string password, int adminId);
        public  Task<bool> BillingInfoEdit(AdminDetailsInfo _viewAdminProfile);
        public  Task<bool> EditAdministratorInfo(AdminDetailsInfo _viewAdminProfile);
        public EncounterInfo EncounterInfoPost( EncounterInfo ve);
        public void EncounterFinalize(EncounterInfo ve);
        public bool SendLink(sendAgreement sendAgreement);
        List<AdminDashboardList> Export(string status);
    }

}
