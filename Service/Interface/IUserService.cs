using Data.Models;
using Data.ViewModel.User;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IUserService
    {
        Task<bool> Create(UserViewModel entity);
        Task<bool> Update(User entity);
        Task<bool> Delete(int id);
        Task<User> GetByID(int id);
        Task<List<User>> GetAll();
        Task<object> GetListUser();
        Task<PagedList<User>> GetAllPaging(string keyword, int page, int pageSize);

    }
}
