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
        bool AdminAuthentication(AdminLogin userDetails);
        public List<AdminDashboardList> NewRequestData();
        public CountStatusWiseRequestModel Indexdata();
        public List<AdminDashboardList> GetRequests(string Status);
        public ViewCaseData GetRequestForViewCase(int id);
        public ViewCaseData NewRequestData(int? RId, int? RTId);
        public ViewNotes ViewNotes(int reqClientId);

        public ViewCaseData Edit(ViewCaseData vdvc, int? RId, int? RTId);
        public List<Physician> ProviderbyRegion(int Regionid);
        public List<Region> AssignCase();
        public List<CaseTag> CancelCase();
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes);
        public void CancelCaseInfo(int casetagId, string Notes, int RequestId);
       public bool BlockCaseInfo(int requestId, string notes);
    }
}
