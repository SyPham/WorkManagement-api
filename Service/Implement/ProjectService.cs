using AutoMapper;
using Data;
using Data.Models;
using Data.ViewModel.Project;
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
    public class ProjectService : IProjectService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProjectService(DataContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Create(Project entity)
        {
            await _context.Projects.AddAsync(entity);

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
            var entity = await _context.Projects.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

             _context.Projects.Remove(entity);
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

        public async Task<List<Project>> GetAll()
        {
            return await _context.Projects.ToListAsync();
        }
        public async Task<List<ProjectViewModel>> GetListProject()
        {
            return _mapper.Map<List<ProjectViewModel>>(await _context.Projects.ToListAsync());
        }

        public async Task<PagedList<Project>> GetAllPaging(string keyword, int page, int pageSize)
        {
            var source = _context.Projects.AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.Contains(keyword));
            }
            return await PagedList<Project>.CreateAsync(source, page, pageSize);
        }

        public async Task<Project> GetByID(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<bool> Update(Project entity)
        {
           var item= await _context.Projects.FindAsync(entity.ID);
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
