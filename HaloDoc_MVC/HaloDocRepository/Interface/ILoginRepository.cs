using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocRepository.Interface
{
    public interface ILoginRepository
    {
        Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser);
        public bool isAccessGranted(int roleId, string menuName);
    }
}
