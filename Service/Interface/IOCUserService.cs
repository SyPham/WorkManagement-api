using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IOCUserService
    {
        Task<object> GetListUser(int ocid);
        Task<object> AddOrUpdate(int userid, int ocid, bool status);

    }
}
