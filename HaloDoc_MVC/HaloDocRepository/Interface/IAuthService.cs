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
    public interface IAuthService
    {
        bool PatientAuthentication(PatientLoginDetails userDetails);
        bool PatientForgotPass(PatientForgotPassword model);
        void ResetPassword(PatientResetPassword model);

    }
}
