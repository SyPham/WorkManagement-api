using Data.ViewModel.Project;
using Data.ViewModel.Task;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface ITaskService
    {
        Task<object> LoadTask(string name,int userid, int ocID, int page, int pageSize);
        Task<object> LoadTaskHistory(string name, int userid, int ocID, int page, int pageSize);
        Task<List<TreeViewTask>> GetListTree();
        Task<object> CreateTask(CreateTaskViewModel task);
        Task<object> CreateSubTask(CreateTaskViewModel task);
        Task<object> CreateRemark(int taskID,string remark);
        Task<object> Delete(int id);
        Task<object> From(int userid);
        Task<object> Done(int id);
        Task<List<TreeViewTask>> GetListTreeHistory();
        Task<object> GetListUser(int userid);
        Task<List<ProjectViewModel>> GetListProject();
        Task<object> UpdateTask(UpdateTaskViewModel task);
        Task<object> Remark(RemarkViewModel remark);
    }
}
