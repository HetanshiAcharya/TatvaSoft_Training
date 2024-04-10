using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HaloDocDataAccess.ViewModels.Constant;

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
        public List<HaloDocDataAccess.DataModels.Region> AssignCase();
        public List<CaseTag> CancelCase();
        public List<HealthProfessionalType> Professions();
        public bool SendOrders(int requestid, OrderDetail o);
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes);
        public void CancelCaseInfo(int casetagId, string Notes, int RequestId);
        public bool BlockCaseInfo(int requestId, string notes);
        public void DeleteFile(int requestid, int reqwisefileid);
        public void DeleteFile(int requestid, int[] reqwisefileid);
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
        public bool IndexForgotPass(PatientForgotPassword model);
        ProviderMenu ProviderMenu(int Region);
        public bool ChangeNoti(int[] files, int region);
        public bool SendEmailProvider(string Email, string Message);
        public Task<ProviderList> GetProviderProfileDetails(int id);
        public List<Role> ProviderRole();
        public Task<bool> EditProviderAccInfo(ProviderList p);
        public Task<bool> EditProviderInfo(ProviderList p);
        public Task<bool> EditProviderMailingInfo(ProviderList p);
        public Task<bool> ProviderProfileInfo(ProviderList p);
        public bool SaveProvider(int[] checkboxes, int physicianid);
        public bool AddProviderAccount(ProviderList PhysiciansData, int[] checkboxes, string UserId);
        public bool DeleteProvider(int PhysicianId);
        public List<Menu> RolebyAccountType(AccountType Account);
        public bool SaveEditRole(AccessModel roles, string userId);
        public AccessModel ViewEditRole(int RoleId);
        public bool SaveEditRoleAccess(AccessModel roles);
        public bool DeleteRole(int RoleId);
        public List<AspNetRole> Role();
        public List<UserAccessData> UserAccessData(string AccountType);
        public List<Partners> GetPartnersByProfession(string searchValue, int Profession);
        public bool AddBusiness(HealthProfessional obj);
        public HealthProfessional EditPartners(int VendorId);
        public bool DeleteBusiness(int vendorId);
        public List<PatientDashboard> RecordsPatientExplore(int UserId);
        public bool UnBlock(int reqId);
        public BlockHistory RecordsBlock(BlockHistory formData);
        public SearchInputs RecordsSearch(SearchInputs rm);
        public bool RecordsDelete(int reqId);
    }

}
