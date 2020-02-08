using Data.Models;
using Data.ViewModel.Project;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IProjectService
    {
        Task<bool> Create(Project entity);
        Task<bool> Update(Project entity);
        Task<bool> Delete(int id);
        Task<Project> GetByID(int id);
        Task<List<Project>> GetAll();
        Task<PagedList<Project>> GetAllPaging(string keyword,int page, int pageSize);
        Task<List<ProjectViewModel>> GetListProject();
    }
}
