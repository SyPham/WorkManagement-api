﻿using Data;
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
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;
        public RoleService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(Role entity)
        {
            await _context.Roles.AddAsync(entity);

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
            var entity = await _context.Roles.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

             _context.Roles.Remove(entity);
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

        public async Task<List<Role>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<PagedList<Role>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _context.Roles.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<Role>.CreateAsync(source, page, pageSize);
        }

        public async Task<Role> GetByID(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<bool> Update(Role entity)
        {
           var item= await _context.Roles.FindAsync(entity.ID);
            item.Name = entity.Name;
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
