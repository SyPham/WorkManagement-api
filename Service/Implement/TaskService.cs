using AutoMapper;
using Data;
using Data.ViewModel.Task;
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
    public class TaskService : ITaskService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TaskService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<TreeViewTask>> GetListTree()
        {
            var listLevels = await _context.Tasks.OrderBy(x => x.Level).ToListAsync();
            var levels = new List<TreeViewTask>();
            foreach (var item in listLevels)
            {
                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                levelItem.JobName = item.JobName;
                levelItem.Level = item.Level;
                levelItem.ParentID = item.ParentID;
                levelItem.Description = item.Description;
                levelItem.DueDate = item.DueDate;
                levelItem.CreatedDate = item.CreatedDate;
                levels.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = levels.Where(c => c.ParentID == 0)
                            .Select(c => new TreeViewTask()
                            {
                                ID = c.ID,
                                JobName = c.JobName,
                                Level = c.Level,
                                Description = c.Description,
                                ParentID = c.ParentID,
                                ProjectID = c.ProjectID,
                                DueDate = c.DueDate,
                                CreatedDate = c.CreatedDate,
                                children = GetChildren(levels, c.ID)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        private void HieararchyWalk(List<TreeViewTask> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    //Console.WriteLine(string.Format("{0} {1}", item.Id, item.Text));
                    HieararchyWalk(item.children);
                }
            }
        }
        public List<TreeViewTask> GetChildren(List<TreeViewTask> levels, int parentid)
        {
            return levels
                    .Where(c => c.ParentID == parentid)
                    .Select(c => new TreeViewTask()
                    {
                        ID = c.ID,
                        JobName = c.JobName,
                        Level = c.Level,
                        Description = c.Description,
                        ProjectID = c.ProjectID,
                        DueDate = c.DueDate,
                        CreatedDate = c.CreatedDate,
                        children = GetChildren(levels, c.ID)
                    })
                    .ToList();
        }
        public async Task<object> CreateRemark(int taskID, string remark)
        {
            var item = await _context.Tasks.FindAsync(taskID);
            item.Remark = remark;
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

        public async Task<object> CreateSubTask(CreateTaskViewModel task)
        {
            var item = _mapper.Map<Data.Models.Task>(task);

            //Level cha tang len 1 va gan parentid cho subtask
            var taskParent = _context.Tasks.Find(item.ID);
            taskParent.Level = taskParent.Level + 1;
            item.ParentID = task.ParentID;

            await _context.Tasks.AddAsync(item);
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

        public async Task<object> CreateTask(CreateTaskViewModel task)
        {
            var item = _mapper.Map<Data.Models.Task>(task);
            await _context.Tasks.AddAsync(item);
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

        public async Task<object> Delete(int id)
        {
            var item = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(item);
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

        public async Task<object> LoadTask(string name, int userid, int ocID, int page, int pageSize)
        {
            var source = _context.Tasks.Where(x => x.CreatedBy == userid && x.Status == false).AsQueryable();

            var userBeAssignedTask = _context.Tags.Where(x => x.UserID == userid).Select(x => x.TaskID).ToArray();
            if (userBeAssignedTask.Count() > 0)
            {
                source = source.Where(x => userBeAssignedTask.Contains(x.ID));
            }

            if (!name.IsNullOrEmpty())
            {
                source = source.Where(x => x.JobName.Contains(name));
            }
            return await PagedList<Data.Models.Task>.CreateAsync(source, page, pageSize);
        }

        public async Task<object> LoadTaskHistory(string name, int userid, int ocID, int page, int pageSize)
        {
            var source = _context.Tasks.Where(x => x.CreatedBy == userid && x.Status == true).AsQueryable();

            var userBeAssignedTask = _context.Tags.Where(x => x.UserID == userid).Select(x => x.TaskID).ToArray();
            if (userBeAssignedTask.Count() > 0)
            {
                source = source.Where(x => userBeAssignedTask.Contains(x.ID));
            }

            if (!name.IsNullOrEmpty())
            {
                source = source.Where(x => x.JobName.Contains(name));
            }
            return await PagedList<Data.Models.Task>.CreateAsync(source, page, pageSize);
        }
        public async Task<object> UpdateTask(UpdateTaskViewModel task)
        {

            if (!await _context.Tasks.AnyAsync(x => x.ID == task.ID))
                return false;
            var update = await _context.Tasks.FindAsync(task.ID);
            update.JobName = task.JobName;
            update.Description = task.Description;
            update.From = task.From;
            update.CreatedBy = task.CreatedBy;
            update.Status = task.Status;

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
