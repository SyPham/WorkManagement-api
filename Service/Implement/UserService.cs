using AutoMapper;
using Data;
using Data.Models;
using Data.ViewModel.User;
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
        private readonly IMapper _mapper;

        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Create(UserViewModel entity)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(entity.Password, out passwordHash, out passwordSalt);
            var user = _mapper.Map<User>(entity);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);

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
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
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

        public async Task<object> GetListUser()
        {
            return await _context.Users.Select(x => new { x.ID, x.Username, x.Email, RoleName = x.Role.Name, x.RoleID }).ToListAsync();

        }

        public async Task<bool> Update(User entity)
        {
            var item = await _context.Users.FindAsync(entity.ID);
            item.Username = entity.Username;
            item.Email = entity.Email;
            item.RoleID = entity.RoleID;
            if (item.PasswordHash.IsNullOrEmpty() && item.PasswordSalt.IsNullOrEmpty())
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("1", out passwordHash, out passwordSalt);

                item.PasswordHash = passwordHash;
                item.PasswordSalt = passwordSalt;
            }
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
