using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(User entity)
        {
            await _context.Users.AddAsync(entity);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

             _context.Users.Remove(entity);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<PagedList<User>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _context.Users.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Username.Contains(keyword) || x.Email.Contains(keyword));
            }
            return await PagedList<User>.CreateAsync(source, page, pageSize);
        }

        public async Task<User> GetByID(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> Update(User entity)
        {
           var item= await _context.Users.FindAsync(entity.ID);
            item.Username = entity.Username;
            item.Email = entity.Email;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}
