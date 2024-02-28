using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.ViewModels;

namespace HaloDocRepository.Interface
{
    public interface IAdminService
    {
        bool AdminAuthentication(AdminLogin userDetails);
        public List<AdminDashboardList> NewRequestData();
        public CountStatusWiseRequestModel Indexdata();
        public List<AdminDashboardList> GetRequests(string Status);
    }
}
