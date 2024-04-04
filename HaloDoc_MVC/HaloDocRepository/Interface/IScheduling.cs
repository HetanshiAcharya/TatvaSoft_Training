using HaloDocDataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocRepository.Interface
{
    public interface IScheduling
    {
        public void AddShift(SchedulingData model, List<string?>? chk);
        public void ViewShift(int shiftdetailid);
        public void ViewShiftreturn(SchedulingData modal);
    }
}
