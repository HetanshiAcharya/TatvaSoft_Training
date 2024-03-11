using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;

namespace HaloDocRepository.Interface
{
    public interface IAdminService
    {
       //bool AdminAuthentication(AdminLogin userDetails);
        public List<AdminDashboardList> NewRequestData();
        public CountStatusWiseRequestModel Indexdata();
        public List<AdminDashboardList> GetRequests(string Status);
        public ViewCaseData GetRequestForViewCase(int id);
        public ViewCaseData NewRequestData(int? RId, int? RTId);
        public ViewNotes ViewNotes(int reqClientId);
        void ViewNotesUpdate(ViewNotes viewNotes);
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
        public void SendAgreement(sendAgreement sendAgreement);
    }
}
