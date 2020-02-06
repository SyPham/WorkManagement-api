using Data.Models;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IUserService
    {
        Task<bool> Create(User entity);
        Task<bool> Update(User entity);
        Task<bool> Delete(int id);
        Task<User> GetByID(int id);
        Task<List<User>> GetAll();
        Task<PagedList<User>> GetAllPaging(string keyword, int page, int pageSize);

    }
}
