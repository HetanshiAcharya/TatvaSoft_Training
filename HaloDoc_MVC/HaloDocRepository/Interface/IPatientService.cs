using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;

namespace HaloDocRepository.Interface
{
    public interface IPatientService
    {
        void PatientRequest(PatientSubmitRequests userDetails);
        void FamilyRequest(FamilySubmitRequests viewdata);
        void ConciergeRequest(ConciergeSubmitRequests viewdata);
        void BusinessRequest(BusinessSubmitRequests viewdata);
        List<PatientDashboard> GetMedicalHistory(User user);
        void ProfileData(PatientProfile newdetails, int? id);
        public List<PatientProfile> PatientHistory(string fname, string lname, string email, string phone);
    }
}
