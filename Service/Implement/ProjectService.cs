using AutoMapper;
using Data;
using Data.Models;
using Data.ViewModel.Notification;
using Data.ViewModel.Project;
using Data.ViewModel.Task;
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
    public class ProjectService : IProjectService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public ProjectService(DataContext context, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<bool> Create(Project entity)
        {
            try
            {
                entity.CreatedByName = (await _context.Users.FindAsync(entity.CreatedBy)).Username ?? "";
                await _context.Projects.AddAsync(entity);
                await _context.SaveChangesAsync();

                var room = new Room
                {
                    Name = entity.Name,
                    ProjectID = entity.ID
                };
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();

                var update = await _context.Projects.FirstOrDefaultAsync(x => x.ID.Equals(room.ProjectID));
                update.Room = room.ID;
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
            try
            {

                var entity = await _context.Projects.FindAsync(id);

                if (entity == null)
                {
                    return false;
                }
                _context.Rooms.Remove(await _context.Rooms.FirstOrDefaultAsync(_ => _.ProjectID == id));
                _context.Managers.RemoveRange(await _context.Managers.Where(_ => _.ProjectID == id).ToListAsync());
                _context.TeamMembers.RemoveRange(await _context.TeamMembers.Where(_ => _.ProjectID == id).ToListAsync());

                var listTask = await _context.Tasks.Where(_ => _.ProjectID == id).ToListAsync();
                _context.Tags.RemoveRange(await _context.Tags.Where(_ => listTask.Select(x => x.ID).Contains(_.TaskID)).ToListAsync());
                _context.Tasks.RemoveRange(listTask);
                _context.Follows.RemoveRange(await _context.Follows.Where(_ => listTask.Select(x => x.ID).Contains(_.TaskID)).ToListAsync());

                _context.Projects.Remove(entity);

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

        public async Task<PagedList<ProjectViewModel>> GetAllPaging(int userid, int page, int pageSize, string keyword)
        {
            var model = await _context.Projects
                .Include(x => x.TeamMembers)
                .ThenInclude(x => x.User)
                .Include(x => x.Managers)
                .ThenInclude(x => x.User)
                .ToListAsync();

            var source = _mapper.Map<List<ProjectViewModel>>(model);
            source = source.Where(_ => _.Manager.Contains(userid) || _.Members.Contains(userid) || _.CreatedBy == userid).ToList();

            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Name.ToLower().Contains(keyword.Trim().ToLower()) || x.CreatedByName.ToLower().Contains(keyword.Trim().ToLower())).ToList();
            }
            return PagedList<ProjectViewModel>.Create(source, page, pageSize);
        }

        public async Task<Project> GetByID(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<bool> Update(Project entity)
        {
            var item = await _context.Projects.FindAsync(entity.ID);
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

        public async Task<Tuple<bool, string>> AddManager(AddManagerViewModel addManager)
        {
            try
            {
                var listUsers = new List<int>();
                if (addManager.Users.Length > 0)
                {
                    //get old manager list
                    var oldManagers = await _context.Managers.Where(x => x.ProjectID == addManager.ProjectID).Select(x => x.UserID).ToArrayAsync();
                    //new manager list from client
                    var newManagers = addManager.Users;
                    //get value of old managers list without value in new manager list
                    var withOutInOldManagers = newManagers.Except(oldManagers).ToArray();
                    if (withOutInOldManagers.Length > 0)
                    {
                        var managers = new List<Manager>();
                        foreach (var pic in withOutInOldManagers)
                        {
                            managers.Add(new Manager
                            {
                                UserID = pic,
                                ProjectID = addManager.ProjectID
                            });
                        }
                        await _context.Managers.AddRangeAsync(managers);
                        var project = await _context.Projects.FindAsync(addManager.ProjectID);
                        var user = await _context.Users.FindAsync(addManager.UserID);
                        string urlResult = $"/project-detail/{project.ID}";
                        var message = $"The {user.Username.ToTitleCase()} account has assigned you as manager of {project.Name} project";
                        await _notificationService.Create(new CreateNotifyParams
                        {
                            AlertType = Data.Enum.AlertType.Manager,
                            Message = message,
                            Users = withOutInOldManagers.Distinct().ToList(),
                            URL = urlResult,
                            UserID = addManager.UserID
                        });
                        listUsers.AddRange(withOutInOldManagers);
                    }
                    else
                    {
                        //Day la userID se bi xoa
                        var withOutInNewManagers = oldManagers.Where(x => !newManagers.Contains(x)).ToArray();
                        var listDeleteManagers = await _context.Managers.Where(x => withOutInNewManagers.Contains(x.UserID) && x.ProjectID.Equals(addManager.ProjectID)).ToListAsync();
                        _context.Managers.RemoveRange(listDeleteManagers);
                    }
                }
                await _context.SaveChangesAsync();
                return Tuple.Create(true, string.Join(",", listUsers.Distinct().ToArray()));
            }
            catch (Exception)
            {
                return Tuple.Create(false, "");

            }
        }

        public async Task<Tuple<bool, string>> AddMember(AddMemberViewModel addMember)
        {
            try
            {
                var listUsers = new List<int>();

                if (addMember.Users.Length > 0)
                {
                    //get old member list
                    var oldMembers = await _context.TeamMembers.Where(x => x.ProjectID == addMember.ProjectID).Select(x => x.UserID).ToArrayAsync();
                    //new member list from client
                    var newMembers = addMember.Users;
                    //get value of old members list without value in new member list
                    var withOutInOldMembers = newMembers.Except(oldMembers).ToArray();
                    if (withOutInOldMembers.Length > 0)
                    {
                        var members = new List<TeamMember>();
                        foreach (var pic in withOutInOldMembers)
                        {
                            members.Add(new TeamMember
                            {
                                UserID = pic,
                                ProjectID = addMember.ProjectID
                            });
                        }
                        await _context.TeamMembers.AddRangeAsync(members);
                        var project = await _context.Projects.FindAsync(addMember.ProjectID);
                        var user = await _context.Users.FindAsync(addMember.UserID);
                        string urlResult = $"/project-detail/{project.ID}";
                        var message = $"The {user.Username.ToTitleCase()} account has assigned you as member of {project.Name} project";
                        await _notificationService.Create(new CreateNotifyParams
                        {
                            AlertType = Data.Enum.AlertType.Member,
                            Message = message,
                            Users = withOutInOldMembers.Distinct().ToList(),
                            URL = urlResult,
                            UserID = addMember.UserID
                        });
                        listUsers.AddRange(withOutInOldMembers);
                    }
                    else
                    {
                        //Day la userID se bi xoa
                        var withOutInNewMembers = oldMembers.Where(x => !newMembers.Contains(x)).ToArray();
                        var listDeleteMembers = await _context.TeamMembers.Where(x => withOutInNewMembers.Contains(x.UserID) && x.ProjectID.Equals(addMember.ProjectID)).ToListAsync();
                        _context.TeamMembers.RemoveRange(listDeleteMembers);
                    }
                }

                await _context.SaveChangesAsync();
                return Tuple.Create(true, string.Join(",", listUsers.Distinct().ToArray()));
            }
            catch (Exception)
            {
                return Tuple.Create(false, "");
            }
        }

        public async Task<object> GetUsers()
        {
            return await _context.Users.Where(x => x.RoleID != 1).Select(x => new { x.ID, x.Username }).ToListAsync();
        }

        public async Task<object> GetUserByProjectID(int id)
        {
            try
            {
                var item = await _context.Managers.Include(x => x.Project).Include(x => x.User).FirstOrDefaultAsync(x => x.ProjectID == id);
                return new
                {
                    status = true,
                    room = item.Project.Room,
                    title = item.Project.Name,
                    createdBy = item.Project.CreatedBy,
                    selectedManager = await _context.Managers.Include(x => x.User).Where(x => x.ProjectID == id).Select(x => new { ID = x.User.ID, Username = x.User.Username }).ToArrayAsync(),
                    selectedMember = await _context.TeamMembers.Include(x => x.User).Where(x => x.ProjectID == id).Select(x => new { ID = x.User.ID, Username = x.User.Username }).ToArrayAsync(),
                };
            }
            catch (Exception)
            {
                return new
                {
                    status = false,
                };
            }

        }

        public async Task<object> GetProjects(int userid, int page, int pageSize, string projectName)
        {
            var members = _context.TeamMembers.Where(_ => _.UserID == userid).Select(x => x.ProjectID).ToArray();

            var model = await _context.Projects
                .Include(x => x.TeamMembers)
                .ThenInclude(x => x.User)
                .Include(x => x.Managers)
                .ThenInclude(x => x.User).Select(x => new
                {
                    x.ID,
                    x.Name,
                    Manager = x.TeamMembers.Select(a => a.User.Username).ToArray(),
                    ManagerID = x.TeamMembers.Select(a => a.User.ID).ToArray(),
                    Members = x.TeamMembers.Select(a => a.User.Username).ToArray(),
                    MemberIDs = x.TeamMembers.Select(a => a.User.ID).ToArray(),
                    x.CreatedBy
                }).ToListAsync();
            model = model.Where(_ => _.ManagerID.Contains(userid) || _.MemberIDs.Contains(userid) || _.CreatedBy == userid).ToList();
            if (!projectName.IsNullOrEmpty())
            {
                projectName = projectName.Trim().ToLower();
                model = model.Where(x => x.Name.ToLower().Contains(projectName)).ToList();
            }
            var totalCount = model.Count();
            model = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new
            {
                project = model,
                data = model,
                total = (int)Math.Ceiling(totalCount / (double)pageSize),
                totalPage = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public Task<object> GetProjects()
        {
            throw new NotImplementedException();
        }
        public async Task<object> ProjectDetail(int projectID)
        {
            var item = await _context.Projects.FindAsync(projectID);
            return item;
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
