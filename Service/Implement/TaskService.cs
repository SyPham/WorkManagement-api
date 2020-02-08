using AutoMapper;
using Data;
using Data.Models;
using Data.ViewModel.OC;
using Data.ViewModel.Project;
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
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IOCService _ocService;

        public TaskService(DataContext context, IMapper mapper, IUserService userService, IProjectService projectService, IOCService ocService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _projectService = projectService;
            _ocService = ocService;
        }

        public async Task<List<TreeViewTask>> GetListTree()
        {
            var listLevels = await _context.Tasks
                .Where(x => x.Status == false)
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();
            var levels = new List<TreeViewTask>();
            foreach (var item in listLevels)
            {

                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray());
                levelItem.JobName = item.JobName;
                levelItem.Level = item.Level;
                levelItem.ParentID = item.ParentID;
                levelItem.Description = item.Description;
                levelItem.DueDate = String.Format("{0:D}", item.DueDate);
                levelItem.CreatedDate = String.Format("{0:D}", item.CreatedDate);
                levelItem.User = item.User;
                levelItem.JobName = item.ProjectID == 0 ? item.JobName : _context.Projects.Find(item.ProjectID).Name;
                levelItem.state = item.Status == false ? "Undone":"Done";
                levelItem.Remark = item.Remark ?? "#N/A";

                levelItem.From = item.OCID > 0 ? _context.OCs.Find(item.OCID).Name : item.User.Username ?? "#N/A";
                levels.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = levels.Where(c => c.ParentID == 0)
                            .Select(c => new TreeViewTask()
                            {
                                ID = c.ID,
                                JobName = c.JobName,
                                Level = c.Level,
                                Remark = c.Remark,
                                Description = c.Description,
                                ProjectID = c.ProjectID,
                                DueDate = c.DueDate,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                                From = c.From,
                                ProjectName = c.ProjectName,
                                state = c.state,
                                PIC = c.PIC,
                                children = GetChildren(levels, c.ID)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        public async Task<List<TreeViewTask>> GetListTreeHistory()
        {
            var listLevels = await _context.Tasks
                .Where(x => x.Status == true)
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();
            var levels = new List<TreeViewTask>();
            foreach (var item in listLevels)
            {

                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray());
                levelItem.JobName = item.JobName;
                levelItem.Level = item.Level;
                levelItem.ParentID = item.ParentID;
                levelItem.Description = item.Description;
                levelItem.DueDate = String.Format("{0:D}", item.DueDate);
                levelItem.CreatedDate = String.Format("{0:D}", item.CreatedDate);
                levelItem.User = item.User;
                levelItem.JobName = item.ProjectID == 0 ? item.JobName : _context.Projects.Find(item.ProjectID).Name;
                levelItem.state = item.Status == false ? "Undone" : "Done";
                levelItem.Remark = item.Remark ?? "#N/A";

                levelItem.From = item.OCID > 0 ? _context.OCs.Find(item.OCID).Name : item.User.Username ?? "#N/A";
                levels.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = levels.Where(c => c.ParentID == 0)
                            .Select(c => new TreeViewTask()
                            {
                                ID = c.ID,
                                JobName = c.JobName,
                                Level = c.Level,
                                Remark = c.Remark,
                                Description = c.Description,
                                ProjectID = c.ProjectID,
                                DueDate = c.DueDate,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                                From = c.From,
                                ProjectName = c.ProjectName,
                                state = c.state,
                                PIC = c.PIC,
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
                        Remark = c.Remark,
                        Description = c.Description,
                        ProjectID = c.ProjectID,
                        DueDate = c.DueDate,
                        CreatedBy = c.CreatedBy,
                        CreatedDate = c.CreatedDate,
                        From = c.From,
                        ProjectName = c.ProjectName,
                        state = c.state,
                        PIC = c.PIC,
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
            var taskParent = _context.Tasks.Find(item.ParentID);
            item.Level = taskParent.Level + 1;
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
        public async Task<object> GetListUser(int userid)
        {
            var user = await _userService.GetByID(userid);
            var ocID = user.OCID;
            var oc = await _context.OCs.FindAsync(ocID);
            var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
            var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).Where(x => x != ocID).ToArray();
            return await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => new { x.ID, x.Username }).ToListAsync();
        }
        public async Task<List<ProjectViewModel>> GetListProject()
        {
            return await _projectService.GetListProject();
        }

        public async Task<object> From(int userid)
        {
            var user = await _userService.GetByID(userid);
            var ocID = user.OCID;
            var oc = await _context.OCs.FindAsync(ocID);
            var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
            var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();

            var users = await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => new { x.ID, x.Username }).ToListAsync();
            var ocs = await _context.OCs.Where(x => arrOCs.Contains(x.ID)).Select(x => new { x.ID, x.Name }).ToListAsync();
            return new
            {
                users,
                ocs
            };
        }

        public IEnumerable<TreeViewOC> GetAllDescendants(IEnumerable<TreeViewOC> rootNodes)
        {
            var descendants = rootNodes.SelectMany(_ => GetAllDescendants(_.children));
            return rootNodes.Concat(descendants);
        }

        public async Task<object> CreateTask(CreateTaskViewModel task)
        {
            try
            {
                var item = _mapper.Map<Data.Models.Task>(task);
                item.Level = 1;

                await _context.Tasks.AddAsync(item);
                await _context.SaveChangesAsync();

                var tags = new List<Tag>();
                foreach (var pic in task.PIC)
                {
                    tags.Add(new Tag
                    {
                        UserID = pic,
                        TaskID = item.ID
                    });
                }
                await _context.Tags.AddRangeAsync(tags);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<object> Delete(int id)
        {
            var item = await _context.Tasks.FindAsync(id);
            //var OCS = await _ocService.GetListTreeOC(item.ParentID, item.ID);
            //var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();
            //var items = await _context.Tasks.Where(x => arrOCs.Contains(x.ID)).ToListAsync();
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
        public async Task<object> Done(int id)
        {
            var item = await _context.Tasks.FindAsync(id);
            //var OCS = await _ocService.GetListTreeOC(item.ParentID, item.ID);
            //var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();

            //var items = await _context.Tasks.Where(x => arrOCs.Contains(x.ID)).ToListAsync();
            //items.ForEach(x =>
            //{
            //    x.Status = true;
            //});
            item.Status = true;
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
        public async Task<object> Remark(RemarkViewModel remark)
        {

            if (!await _context.Tasks.AnyAsync(x => x.ID == remark.ID))
                return false;
            var update = await _context.Tasks.FindAsync(remark.ID);
            update.Remark = remark.Remark;
        

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
