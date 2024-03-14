using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocDataAccess.ViewModels
{
    public class reviewAgreement
    {
        public int RequestId { get; set; }

    }
    public class CancelAgreementModal
    {
        public int ReqId { get; set; }
        public string? PatientName { get; set; }
    }
}
