using AutoMapper;
using Data;
using Data.Models;
using Data.ViewModel.Notification;
using Data.ViewModel.OC;
using Data.ViewModel.Project;
using Data.ViewModel.Task;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class TaskService : ITaskService
    {
        #region Properties
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IOCService _ocService;
        private readonly INotificationService _notificationService;
        private readonly string[] _urlArray = {
        "project-detail",
        "routine",
        "abnormal"
      };
        private readonly string[] _monthArray = {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
      };
        private readonly string[] _quarterArray ={
        "First quarter",
        "Second quarter",
        "Third quarter",
        "Fourth quarter"
      };
        private readonly string[] _everydayArray = {
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
        };
        #endregion

        #region Constructor

        public TaskService(DataContext context, INotificationService notificationService, IMapper mapper, IUserService userService, IProjectService projectService, IOCService ocService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _projectService = projectService;
            _ocService = ocService;
            _notificationService = notificationService;
        }

        #endregion

        #region Helpers
        private string CheckMessageRemark(int jobtype, string content, string project, string jobName, string username)
        {
            string message = string.Empty;
            switch (jobtype)
            {
                case (int)Data.Enum.JobType.Project:

                    message = $"The {username} account has already updated remark content of '{jobName}' in {project} project.";
                    break;
                case (int)Data.Enum.JobType.Abnormal:
                case (int)Data.Enum.JobType.Routine:
                    message = $"The {username} account has already updated remark content of '{jobName}': '{content}.'";
                    break;
                default:
                    break;
            }
            return message;
        }
        private string ToFullTextMonthly(string month)
        {
            if (month.IsNullOrEmpty())
                return "#N/A";
            var result = string.Empty;
            foreach (var item in this._monthArray)
            {
                if (item.ToLower().Contains(month.ToLower()))
                {
                    result = item;
                    break;
                }
            };
            return result;
        }
        private string ToFullTextQuarterly(string quarter)
        {
            if (quarter.IsNullOrEmpty())
                return "#N/A";
            var result = string.Empty;
            foreach (var item in this._quarterArray)
            {
                if (item.ToLower().Contains(quarter.ToLower()))
                {
                    result = item;
                    break;
                }
            };
            return result;
        }
        private string ToFullTextEveryDay(string day)
        {
            if (day.IsNullOrEmpty())
                return "#N/A";
            var result = string.Empty;
            foreach (var item in this._everydayArray)
            {
                if (item.ToLower().Contains(day.ToLower()))
                {
                    result = item;
                    break;
                }
            };
            return result;
        }
        public string CastPriority(string value)
        {
            value = value.ToSafetyString().ToUpper() ?? "";
            if (value == "H")
                return "High";
            if (value == "M")
                return "Medium";
            if (value == "L")
                return "Low";
            return value;
        }
        public void HieararchyWalk(List<TreeViewTask> hierarchy)
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
        public List<TreeViewTask> GetChildren(List<TreeViewTask> tasks, int parentid)
        {
            return tasks
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
                        Deadline = c.Deadline,
                        PriorityID = c.PriorityID,
                        Priority = c.Priority,
                        Follow = c.Follow,
                        PIC = c.PIC,
                        PICs = c.PICs,
                        JobTypeID = c.JobTypeID,
                        FromWho = c.FromWho,
                        FromWhere = c.FromWhere,
                        BeAssigneds = c.BeAssigneds,
                        EveryDay = c.EveryDay,
                        Deputies = c.Deputies,
                        DeputiesList = c.DeputiesList,
                        Quarterly = c.Quarterly,
                        Monthly = c.Monthly,
                        DeputyName = c.DeputyName,

                        children = GetChildren(tasks, c.ID)
                    })
                    .ToList();
        }

        public IEnumerable<TreeViewOC> GetAllDescendants(IEnumerable<TreeViewOC> rootNodes)
        {
            var descendants = rootNodes.SelectMany(x => GetAllDescendants(x.children));
            return rootNodes.Concat(descendants);
        }
        public IEnumerable<TreeView> GetAllDescendants(IEnumerable<TreeView> rootNodes)
        {
            var descendants = rootNodes.SelectMany(x => GetAllDescendants(x.children));
            return rootNodes.Concat(descendants);
        }
        public IEnumerable<TreeViewTask> GetAllTaskDescendants(IEnumerable<TreeViewTask> rootNodes)
        {
            var descendants = rootNodes.SelectMany(x => GetAllTaskDescendants(x.children));
            return rootNodes.Concat(descendants);
        }
        private int FindParentByChild(IEnumerable<Data.Models.Task> rootNodes, int taskID)
        {
            var parent = rootNodes.FirstOrDefault(x => x.ID.Equals(taskID)).ParentID;
            if (parent == 0)
                return rootNodes.FirstOrDefault(x => x.ID.Equals(taskID)).ID;
            else
                return FindParentByChild(rootNodes, parent);
        }
        private async Task<List<int>> GetListUsersAlert(int jobType, int userid, int projectid)
        {
            var listUsers = new List<int>();

            switch (jobType)
            {
                case (int)Data.Enum.JobType.Project:
                    var managers = await _context.Managers.Where(_ => _.ProjectID.Equals(projectid)).Select(_ => _.UserID).ToListAsync();
                    var members = await _context.TeamMembers.Where(_ => _.ProjectID.Equals(projectid)).Select(_ => _.UserID).ToListAsync();
                    listUsers.AddRange(managers.Union(members));
                    break;
                case (int)Data.Enum.JobType.Routine:
                case (int)Data.Enum.JobType.Abnormal:
                    var user = await _context.Users.FindAsync(userid);
                    var oc = await _context.OCs.FindAsync(user.OCID);
                    var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
                    var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();
                    var arrUsers = await _context.Users.Where(x => x.ID != user.ID && arrOCs.Contains(x.OCID)).Select(x => x.ID).ToListAsync();
                    listUsers.AddRange(arrUsers);
                    break;
                default:
                    break;
            }
            return listUsers;
        }

        private async Task<Tuple<List<int>>> AlertDeadlineChanging(Data.Enum.AlertDeadline alert, Data.Models.Task task, int userid, List<int> users)
        {
            var projectName = string.Empty;
            if (task.ProjectID > 0)
            {
                var project = await _context.Projects.FindAsync(task.ProjectID);
                projectName = project.Name;
            }
            var user = await _context.Users.FindAsync(userid);
            string urlResult = $"/todolist/{task.JobName.ToUrlEncode()}";
            var listUsers = new List<int>();
            switch (alert)
            {
                case Data.Enum.AlertDeadline.EveryDay:
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.ChangeEveryDay,
                        Message = CheckMessage(task.JobTypeID, projectName, user.Username, task.JobName, Data.Enum.AlertType.ChangeEveryDay, task.EveryDay),
                        Users = users.ToList(),
                        TaskID = task.ID,
                        URL = urlResult,
                        UserID = userid
                    });
                    listUsers.AddRange(users);
                    break;
                case Data.Enum.AlertDeadline.Monthly:
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.ChangeMonthly,
                        Message = CheckMessage(task.JobTypeID, projectName, user.Username, task.JobName, Data.Enum.AlertType.ChangeMonthly, task.Monthly),
                        Users = users.ToList(),
                        TaskID = task.ID,
                        URL = urlResult,
                        UserID = userid
                    });
                    listUsers.AddRange(users);
                    break;
                case Data.Enum.AlertDeadline.Quarterly:
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.ChangeQuarterly,
                        Message = CheckMessage(task.JobTypeID, projectName, user.Username, task.JobName, Data.Enum.AlertType.ChangeQuarterly, task.Quarterly),
                        Users = users.ToList(),
                        TaskID = task.ID,
                        URL = urlResult,
                        UserID = userid
                    });
                    listUsers.AddRange(users);
                    break;
                case Data.Enum.AlertDeadline.Deadline:
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.ChangeDeadline,
                        Message = CheckMessage(task.JobTypeID, projectName, user.Username, task.JobName, Data.Enum.AlertType.Deputy),
                        Users = users.ToList(),
                        TaskID = task.ID,
                        URL = urlResult,
                        UserID = userid
                    });
                    listUsers.AddRange(users);
                    break;
                default:
                    break;
            }
            return Tuple.Create(listUsers);
        }
        private string AlertMessage(string username, string jobName, string project, bool isProject, Data.Enum.AlertType alertType, string deadline = "#N/A")
        {
            var message = string.Empty;
            switch (alertType)
            {
                case Data.Enum.AlertType.Done:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has already finished the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account has already finished the task name ' {jobName} '";
                    break;
                case Data.Enum.AlertType.Remark:
                    break;
                case Data.Enum.AlertType.Undone:
                    break;
                case Data.Enum.AlertType.UpdateRemark:
                    break;
                case Data.Enum.AlertType.Assigned:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has assigned you the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account assigned you the task name ' {jobName} ' ";
                    break;
                case Data.Enum.AlertType.Deputy:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has assigned you as deputy of the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account has assigned you as deputy of the task name ' {jobName} '";
                    break;
                case Data.Enum.AlertType.Manager:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has assigned you as manager of {project} project";
                    break;
                case Data.Enum.AlertType.Member:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has assigned you as member of {project} project";
                    break;
                case Data.Enum.AlertType.ChangeDeadline:
                    break;
                case Data.Enum.AlertType.ChangeEveryDay:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextEveryDay(deadline)} of the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextEveryDay(deadline)} of the task name ' {jobName} '";
                    break;
                case Data.Enum.AlertType.ChangeMonthly:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextMonthly(deadline)} of the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextMonthly(deadline)} of the task name ' {jobName} '";
                    break;
                case Data.Enum.AlertType.ChangeQuarterly:
                    if (isProject)
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextQuarterly(deadline)} of the task name ' {jobName} ' in {project} project";
                    else
                        message = $"The {username.ToTitleCase()} account has changed deadline to {ToFullTextQuarterly(deadline)} of the task name ' {jobName} '";
                    break;
                default:
                    break;
            }
            return message;
        }
        private string CheckMessage(int jobtype, string project, string username, string jobName, Data.Enum.AlertType alertType, string deadline = "#N/A")
        {
            var message = string.Empty;
            switch (jobtype)
            {
                case (int)Data.Enum.JobType.Project:
                    message = AlertMessage(username, jobName, project, true, alertType, deadline);
                    break;
                case (int)Data.Enum.JobType.Routine:
                case (int)Data.Enum.JobType.Abnormal:
                    message = AlertMessage(username, jobName, project, false, alertType, deadline);
                    break;
            }
            return message;
        }
        #endregion

        #region LoadData
        public async Task<List<TreeViewTask>> GetListTree(string sort, string priority, int userid, string startDate, string endDate, string weekdays, string monthly, string quarterly)
        {
            try
            {

                //Lay tat ca list task chua hoan thanh va task co con chua hoan thnah
                var listTasks = await _context.Tasks
                .Where(x => (x.Status == false && x.FinishedMainTask == false) || (x.Status == true && x.FinishedMainTask == false))
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();

                //Tim tat ca task dc chi dinh lam pic
                var taskpic = _context.Tags.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var taskdeputy = _context.Deputies.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var taskPICDeputies = taskpic.Union(taskdeputy);

                //tim tat ca task dc chi dinh lam manager, member
                var taskManager = _context.Managers.Where(x => x.UserID.Equals(userid)).Select(x => x.ProjectID).ToArray();
                var taskMember = _context.TeamMembers.Where(x => x.UserID.Equals(userid)).Select(x => x.ProjectID).ToArray();
                var taskManagerMember = taskManager.Union(taskMember);

                //vao bang pic tim tat ca nhung task va thanh vien giao cho nhau hoac manager giao cho member
                //Tim dc tat ca cac task ma manager hoac thanh vien tao ra
                var taskpicProject = _context.Tasks.Where(x => taskManagerMember.Contains(x.CreatedBy)).Select(x => x.ID).ToArray();
                //Tim tiep nhung tag nao duoc giao cho user hien tai
                var beAssignProject = _context.Tags.Where(x => taskpicProject.Contains(x.TaskID) && x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var listbeAssignProject = new List<int>();
                var taskModel = await _context.Tasks.Where(x => beAssignProject.Contains(x.ID)).ToListAsync();

                foreach (var task in taskModel)
                {
                    var tasksTree = await GetListTree(task.ParentID, task.ID);
                    var arrTasks = GetAllTaskDescendants(tasksTree).Select(x => x.ID).ToList();
                    listbeAssignProject.AddRange(arrTasks);
                }



                listTasks = listTasks.Where(x => x.CreatedBy.Equals(userid) || taskPICDeputies.Contains(x.ID) || listbeAssignProject.Contains(x.ID)).Distinct().ToList();

                if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
                {
                    var timespan = new TimeSpan(0, 0, 0);
                    var start = DateTime.ParseExact(startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;
                    var end = DateTime.ParseExact(endDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;
                    listTasks = listTasks.Where(x => x.CreatedDate.Date >= start.Date && x.CreatedDate.Date <= end.Date).ToList();
                }

                //Loc theo weekdays
                if (!weekdays.IsNullOrEmpty())
                {
                    listTasks = listTasks.Where(x => x.EveryDay.ToSafetyString().ToLower().Equals(weekdays.ToLower())).ToList();
                }
                //loc theo thang
                if (!monthly.IsNullOrEmpty())
                {
                    listTasks = listTasks.Where(x => x.Monthly.ToSafetyString().ToLower().Equals(monthly.ToLower())).ToList();
                }
                //loc theo quy
                if (!quarterly.IsNullOrEmpty())
                {
                    listTasks = listTasks.Where(x => x.Quarterly.ToSafetyString().ToLower().Equals(quarterly.ToLower())).ToList();
                }

                if (!sort.IsNullOrEmpty())
                {
                    sort = sort.ToLower();
                    if (sort == Data.Enum.JobType.Project.ToSafetyString().ToLower())
                        listTasks = listTasks.Where(x => x.JobTypeID.Equals((int)Data.Enum.JobType.Project)).OrderByDescending(x => x.ProjectID).ToList();
                    if (sort == Data.Enum.JobType.Routine.ToSafetyString().ToLower())
                        listTasks = listTasks.Where(x => x.JobTypeID.Equals((int)Data.Enum.JobType.Routine)).OrderByDescending(x => x.CreatedDate).ToList();
                    if (sort == Data.Enum.JobType.Abnormal.ToSafetyString().ToLower())
                        listTasks = listTasks.Where(x => x.JobTypeID.Equals((int)Data.Enum.JobType.Abnormal)).OrderByDescending(x => x.CreatedDate).ToList();
                }
                if (!priority.IsNullOrEmpty())
                {
                    priority = priority.ToUpper();
                    listTasks = listTasks.Where(x => x.Priority.Equals(priority)).ToList();
                }
                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                foreach (var item in listTasks)
                {
                    var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();
                    var deputiesList = _context.Deputies.Where(x => x.TaskID == item.ID)
                       .Join(_context.Users,
                       de => de.UserID,
                       user => user.ID,
                       (de, user) => new
                       {
                           user.ID,
                           user.Username
                       })
                       .Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).ToList();
                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.Deputies = _context.Deputies.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToList();
                    levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.DeputiesList = deputiesList;
                    levelItem.DeputyName = string.Join(" , ", deputiesList.Select(x => x.Username).ToArray()).IsNotAvailable();
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay);
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly);
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly);
                    levelItem.Level = item.Level;
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.CreatedBy = item.CreatedBy;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();
                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();

                    if (item.DepartmentID > 0)
                    {
                        levelItem.From = ocModel.Find(item.DepartmentID).Name;
                    }
                    else
                    {
                        levelItem.From = userModel.Find(item.FromWhoID).Username;
                    }


                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    Follow = c.Follow,
                                    EveryDay = c.EveryDay,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    Deputies = c.Deputies,
                                    DeputiesList = c.DeputiesList,
                                    DeputyName = c.DeputyName,
                                    children = GetChildren(tasks, c.ID)
                                })
                               .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy.OrderByDescending(x => x.ProjectID).ToList();
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
            }

        }
        public async Task<List<TreeViewTask>> GetListTreeAbnormal(int ocid, string priority, int userid, string startDate, string endDate, string weekdays)
        {
            try
            {
                //Khai bao enum Jobtype la Abnormal
                var jobtype = (int)Data.Enum.JobType.Abnormal;
                //Buoc 1 tim danh sach user la cap duoi hoac cung cap voi user hien tai
                //var user = await _context.Users.FindAsync(userid);
                //var oc = await _context.OCs.FindAsync(user.OCID);
                //var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
                //var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();
                //var arrUsers = await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => x.ID).ToArrayAsync();

                //Lay tat ca task
                var listTasks = await _context.Tasks
                //.Where(x => (arrUsers.Contains(x.CreatedBy) && x.OCID.Equals(ocid) && x.JobTypeID.Equals(jobtype) && x.Status == false && x.FinishedMainTask == false) || (x.Status == true && x.FinishedMainTask == false))
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();

                //Lay tat ca task theo jobtype la abnormal va oc
                listTasks = listTasks.Where(x => x.OCID.Equals(ocid) && x.JobTypeID.Equals(jobtype)).ToList();

                //lay tat ca user cung cap hoac la cap duoi voi user hien tai

                //listTasks = listTasks.Where(x => arrUsers.Contains(x.CreatedBy)).ToList();

                //Lay tat ca task cha chua hoan thanh va task con cua no
                listTasks = listTasks.Where(x => (x.Status == false && x.FinishedMainTask == false) || (x.Status == true && x.FinishedMainTask == false)).ToList();
                listTasks = listTasks.Where(x => x.CreatedBy.Equals(userid)).ToList();
                if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
                {
                    var timespan = new TimeSpan(0, 0, 0);
                    var start = DateTime.ParseExact(startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;
                    var end = DateTime.ParseExact(endDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;

                    listTasks = listTasks.Where(x => x.CreatedDate.Date >= start.Date && x.CreatedDate.Date <= end.Date).ToList();
                }
                if (!weekdays.IsNullOrEmpty())
                {
                    listTasks = listTasks.Where(x => x.EveryDay.Equals(weekdays)).ToList();
                }

                if (!priority.IsNullOrEmpty())
                {
                    priority = priority.ToUpper();
                    listTasks = listTasks.Where(x => x.Priority.Equals(priority)).ToList();
                }
                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                var tagModel = _context.Tags;
                var projectModel = _context.Projects;
                var deputyModel = _context.Deputies;
                foreach (var item in listTasks)
                {
                    //var tasksTree = await GetListTree(item.ParentID, item.ID);
                    var arrTasks = FindParentByChild(listTasks, item.ID);
                    var beAssigneds = tagModel.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();
                    var deputiesList = deputyModel.Where(x => x.TaskID == item.ID)
                       .Join(_context.Users,
                       de => de.UserID,
                       user => user.ID,
                       (de, user) => new
                       {
                           user.ID,
                           user.Username
                       })
                       .Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).ToList();
                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.Deputies = _context.Deputies.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToList();

                    levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : projectModel.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.DeputiesList = deputiesList;
                    levelItem.Level = item.Level;
                    levelItem.PICs = tagModel.Where(x => x.TaskID == arrTasks).Select(x => x.UserID).ToList();
                    levelItem.Deputies = deputiesList.Select(_ => _.ID).ToList();
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.DeputyName = string.Join(" , ", deputiesList.Select(_ => _.Username).ToArray()).IsNotAvailable();
                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay);
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly);
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly);
                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();
                    levelItem.From = userModel.Find(item.FromWhoID).Username;
                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    Follow = c.Follow,
                                    Deputies = c.Deputies,
                                    DeputyName = c.DeputyName,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    EveryDay = c.EveryDay,
                                    DeputiesList = c.DeputiesList,
                                    children = GetChildren(tasks, c.ID)
                                })
                                .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy;
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
            }

        }
        public async Task<List<TreeViewTask>> GetListTreeProjectDetail(string sort, string priority, int userid, int projectid)
        {
            try
            {
                var jobtype = (int)Data.Enum.JobType.Project;

                var listTasks = await _context.Tasks
                .Where(x => x.JobTypeID.Equals(jobtype) && (x.ProjectID == projectid && x.Status == false && x.FinishedMainTask == false) || (x.ProjectID == projectid && x.Status == true && x.FinishedMainTask == false))
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();
                if (!priority.IsNullOrEmpty())
                {
                    priority = priority.ToUpper();
                    listTasks = listTasks.Where(x => x.Priority.Equals(priority)).ToList();
                }
                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                var tagModel = _context.Tags;
                var deputyModel = _context.Deputies;
                foreach (var item in listTasks)
                {
                    var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();

                    //var tasksTree = await GetListTree(item.ParentID, item.ID);
                    var arrTasks = FindParentByChild(listTasks, item.ID);

                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.PIC = string.Join(" , ", tagModel.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.Level = item.Level;
                    levelItem.PICs = tagModel.Where(x => x.TaskID == arrTasks).Select(x => x.UserID).ToList();
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.Deputies = deputyModel.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToList();

                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();
                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay).IsNotAvailable();
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly).IsNotAvailable();
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly).IsNotAvailable();
                    if (item.DepartmentID > 0)
                    {
                        levelItem.From = ocModel.Find(item.DepartmentID).Name;
                    }
                    else
                    {
                        levelItem.From = userModel.Find(item.FromWhoID).Username;
                    }

                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PICs = c.PICs,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    Follow = c.Follow,
                                    EveryDay = c.EveryDay,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    children = GetChildren(tasks, c.ID)
                                })
                                .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy;
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
            }

        }
        public async Task<List<TreeViewTask>> GetListTreeFollow(string sort, string priority, int userid)
        {
            try
            {
                var listTasks = await _context.Tasks
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();


                if (!sort.IsNullOrEmpty())
                {
                    if (sort == "project")
                        listTasks = listTasks.Where(x => x.ProjectID > 0).ToList();
                    if (sort == "routine")
                        listTasks = listTasks.Where(x => x.ProjectID == 0).ToList();
                }
                if (!priority.IsNullOrEmpty())
                {
                    priority = priority.ToUpper();
                    listTasks = listTasks.Where(x => x.Priority.Equals(priority)).ToList();
                }
                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                var tagModel = _context.Tags;
                var deputyModel = _context.Deputies;
                foreach (var item in listTasks)
                {
                    var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();

                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.PIC = string.Join(" , ", tagModel.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.Level = item.Level;
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.Deputies = deputyModel.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToList();

                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();

                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay).IsNotAvailable();
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly).IsNotAvailable();
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly).IsNotAvailable();

                    levelItem.From = item.OCID > 0 ? ocModel.Find(item.OCID).Name : userModel.Find(item.FromWhoID).Username;


                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    Follow = c.Follow,
                                    EveryDay = c.EveryDay,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    children = GetChildren(tasks, c.ID)
                                })
                                .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy;
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
            }

        }
        public async Task<List<TreeViewTask>> GetListTreeRoutine(string sort, string priority, int userid, int ocid)
        {
            try
            {
                var jobtype = (int)Data.Enum.JobType.Routine;
                var user = await _context.Users.FindAsync(userid);
                //var oc = await _context.OCs.FindAsync(user.OCID);
                //var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
                //var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();
                //var arrUsers = await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => x.ID).ToArrayAsync();

                if (ocid == 0)
                    return new List<TreeViewTask>();

                var listTasks = await _context.Tasks
                //.Where(x => arrUsers.Contains(x.CreatedBy) && x.JobTypeID.Equals(jobtype) && (x.OCID == ocid && x.Status == false && x.FinishedMainTask == false) || (x.OCID == ocid && x.Status == true && x.FinishedMainTask == false))
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();

                //Lay tat ca task theo jobtype la routine va theo oc
                listTasks = listTasks.Where(x => x.OCID.Equals(ocid) && x.JobTypeID.Equals(jobtype)).ToList();

                //Tim tat ca task dc chi dinh lam pic
                var taskpic = _context.Tags.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();

                var taskdeputy = _context.Deputies.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var taskPICDeputies = taskpic.Union(taskdeputy);


                //Lay tat ca task cha chua hoan thanh va task con cua no
                listTasks = listTasks.Where(x => (x.Status == false && x.FinishedMainTask == false) || (x.Status == true && x.FinishedMainTask == false)).ToList();

                //lay tat ca user cung cap hoac la cap duoi voi user hien tai hoac pic deputies

                listTasks = listTasks.Where(x => x.CreatedBy.Equals(userid) || taskPICDeputies.Contains(x.ID)).ToList();

                //Filter 
                if (!priority.IsNullOrEmpty())
                {
                    priority = priority.ToUpper();
                    listTasks = listTasks.Where(x => x.Priority.Equals(priority)).ToList();
                }

                //Duoi nay la load theo tree view con nao thi cha do
                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                var tagModel = _context.Tags;
                var deputyModel = _context.Deputies;
                var projectModel = _context.Projects;
                foreach (var item in listTasks)
                {
                    var beAssigneds = tagModel.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();
                    var deputiesList = deputyModel.Where(x => x.TaskID == item.ID)
                         .Join(userModel,
                         de => de.UserID,
                         user => user.ID,
                         (de, user) => new
                         {
                             user.ID,
                             user.Username
                         })
                         .Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).ToList();
                    //var tasksTree = await GetListTree(item.ParentID, item.ID);
                    var arrTasks = FindParentByChild(listTasks, item.ID);
                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.PIC = string.Join(" , ", tagModel.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : projectModel.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.DeputiesList = deputiesList;
                    levelItem.Level = item.Level;
                    levelItem.PICs = tagModel.Where(x => x.TaskID == arrTasks).Select(x => x.UserID).ToList();
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.DeputyName = string.Join(" , ", deputiesList.Select(_ => _.Username).ToArray()).IsNotAvailable();
                    levelItem.Deputies = deputiesList.Select(_ => _.ID).ToList();
                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();
                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();
                    levelItem.From = userModel.Find(item.FromWhoID).Username;
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay).IsNotAvailable();
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly).IsNotAvailable();
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly).IsNotAvailable();
                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PICs = c.PICs,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    Follow = c.Follow,
                                    Deputies = c.Deputies,
                                    DeputyName = c.DeputyName,
                                    DeputiesList = c.DeputiesList,
                                    EveryDay = c.EveryDay,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    children = GetChildren(tasks, c.ID)
                                })
                                .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy;
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
            }

        }
        public async Task<List<TreeViewTask>> GetListTreeHistory(int userid)
        {
            try
            {
                //Lay tat ca list task chua hoan thanh va task co con chua hoan thnah
                var listTasks = await _context.Tasks
                .Where(x => x.Status == true && x.FinishedMainTask == true)
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();

                //Tim tat ca task dc chi dinh lam pic
                var taskpic = _context.Tags.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var taskdeputy = _context.Deputies.Where(x => x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var taskPICDeputies = taskpic.Union(taskdeputy);

                //tim tat ca task dc chi dinh lam manager, member
                var taskManager = _context.Managers.Where(x => x.UserID.Equals(userid)).Select(x => x.ProjectID).ToArray();
                var taskMember = _context.TeamMembers.Where(x => x.UserID.Equals(userid)).Select(x => x.ProjectID).ToArray();
                var taskManagerMember = taskManager.Union(taskMember);

                //vao bang pic tim tat ca nhung task va thanh vien giao cho nhau hoac manager giao cho member
                //Tim dc tat ca cac task ma manager hoac thanh vien tao ra
                var taskpicProject = _context.Tasks.Where(x => taskManagerMember.Contains(x.CreatedBy)).Select(x => x.ID).ToArray();
                //Tim tiep nhung tag nao duoc giao cho user hien tai
                var beAssignProject = _context.Tags.Where(x => taskpicProject.Contains(x.TaskID) && x.UserID.Equals(userid)).Select(x => x.TaskID).ToArray();
                var listbeAssignProject = new List<int>();
                var taskModel = await _context.Tasks.Where(x => beAssignProject.Contains(x.ID)).ToListAsync();

                foreach (var task in taskModel)
                {
                    var tasksTree = await GetListTree(task.ParentID, task.ID);
                    var arrTasks = GetAllTaskDescendants(tasksTree).Select(x => x.ID).ToList();
                    listbeAssignProject.AddRange(arrTasks);
                }



                listTasks = listTasks.Where(x => x.CreatedBy.Equals(userid) || taskPICDeputies.Contains(x.ID) || listbeAssignProject.Contains(x.ID)).Distinct().ToList();

                //if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
                //{
                //    var timespan = new TimeSpan(0, 0, 0);
                //    var start = DateTime.ParseExact(startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;
                //    var end = DateTime.ParseExact(endDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).Date;
                //    listTasks = listTasks.Where(x => x.CreatedDate.Date >= start.Date && x.CreatedDate.Date <= end.Date).ToList();
                //}

                ////Loc theo weekdays
                //if (!weekdays.IsNullOrEmpty())
                //{
                //    listTasks = listTasks.Where(x => x.EveryDay.ToSafetyString().ToLower().Equals(weekdays.ToLower())).ToList();
                //}
                ////loc theo thang
                //if (!monthly.IsNullOrEmpty())
                //{
                //    listTasks = listTasks.Where(x => x.Monthly.ToSafetyString().ToLower().Equals(monthly.ToLower())).ToList();
                //}
                ////loc theo quy
                //if (!quarterly.IsNullOrEmpty())
                //{
                //    listTasks = listTasks.Where(x => x.Quarterly.ToSafetyString().ToLower().Equals(quarterly.ToLower())).ToList();
                //}



                var tasks = new List<TreeViewTask>();
                var ocModel = _context.OCs;
                var userModel = _context.Users;
                var subModel = _context.Follows;
                foreach (var item in listTasks)
                {
                    var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                         .Include(x => x.User)
                         .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();
                    var deputiesList = _context.Deputies.Where(x => x.TaskID == item.ID)
                       .Join(_context.Users,
                       de => de.UserID,
                       user => user.ID,
                       (de, user) => new
                       {
                           user.ID,
                           user.Username
                       })
                       .Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).ToList();
                    var levelItem = new TreeViewTask();
                    levelItem.ID = item.ID;
                    levelItem.Deputies = _context.Deputies.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToList();
                    levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray()).IsNotAvailable();
                    levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                    levelItem.ProjectID = item.ProjectID;
                    levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                    levelItem.BeAssigneds = beAssigneds;
                    levelItem.DeputiesList = deputiesList;
                    levelItem.DeputyName = string.Join(" , ", deputiesList.Select(x => x.Username).ToArray()).IsNotAvailable();
                    levelItem.EveryDay = ToFullTextEveryDay(item.EveryDay).IsNotAvailable();
                    levelItem.Quarterly = ToFullTextQuarterly(item.Quarterly).IsNotAvailable();
                    levelItem.Monthly = ToFullTextMonthly(item.Monthly).IsNotAvailable();
                    levelItem.Level = item.Level;
                    levelItem.Follow = subModel.Any(x => x.TaskID == item.ID && x.UserID == userid);
                    levelItem.ParentID = item.ParentID;
                    levelItem.Priority = CastPriority(item.Priority);
                    levelItem.PriorityID = item.Priority;
                    levelItem.CreatedBy = item.CreatedBy;
                    levelItem.JobTypeID = item.JobTypeID;
                    levelItem.Description = item.Description.IsNotAvailable();
                    if (item.DueDate.Equals(new DateTime()))
                    {
                        levelItem.DueDate = "#N/A";
                    }
                    else
                    {
                        levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                    }
                    levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                    levelItem.User = item.User;
                    levelItem.FromWhere = ocModel.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault() ?? new FromWhere();
                    levelItem.FromWho = userModel.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault() ?? new BeAssigned();
                    levelItem.JobName = item.JobName.IsNotAvailable();
                    levelItem.state = item.Status == false ? "Undone" : "Done";
                    levelItem.Remark = item.Remark.IsNotAvailable();

                    if (item.DepartmentID > 0)
                    {
                        levelItem.From = ocModel.Find(item.DepartmentID).Name;
                    }
                    else
                    {
                        levelItem.From = userModel.Find(item.FromWhoID).Username;
                    }


                    tasks.Add(levelItem);
                }

                List<TreeViewTask> hierarchy = new List<TreeViewTask>();
                if (tasks.Count == 1)
                {
                    if (!tasks.FirstOrDefault().HasChildren)
                        return tasks;
                }
                hierarchy = tasks.Where(c => c.ParentID == 0)
                                .Select(c => new TreeViewTask
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
                                    Deadline = c.Deadline,
                                    FromWho = c.FromWho,
                                    FromWhere = c.FromWhere,
                                    PIC = c.PIC,
                                    PriorityID = c.PriorityID,
                                    Priority = c.Priority,
                                    BeAssigneds = c.BeAssigneds,
                                    JobTypeID = c.JobTypeID,
                                    Follow = c.Follow,
                                    EveryDay = c.EveryDay,
                                    Quarterly = c.Quarterly,
                                    Monthly = c.Monthly,
                                    Deputies = c.Deputies,
                                    DeputiesList = c.DeputiesList,
                                    DeputyName = c.DeputyName,
                                    children = GetChildren(tasks, c.ID)
                                })
                               .ToList();


                HieararchyWalk(hierarchy);

                return hierarchy.OrderByDescending(x => x.ProjectID).ToList();
            }
            catch (Exception ex)
            {

                return new List<TreeViewTask>();
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
        public async Task<object> GetListUser(int userid, int projectid)
        {
            if (projectid > 0)
            {
                var userModel = _context.Users;
                // var manager = await _context.Managers.FindAsync(projectid);
                var member = await _context.TeamMembers.Where(x => x.ProjectID == projectid).Select(x => x.UserID).ToListAsync();
                return await _context.Users.Where(x => member.Contains(x.ID)).Select(x => new { x.ID, x.Username }).ToListAsync();
            }
            else
            {
                var user = await _userService.GetByID(userid);
                var ocID = user.OCID;
                var oc = await _context.OCs.FindAsync(ocID);
                var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
                var arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToArray();
                return await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => new { x.ID, x.Username }).ToListAsync();
            }
        }
        public async Task<object> From(int userid)
        {
            var user = await _userService.GetByID(userid);
            var ocID = user.OCID;
            var arrOCs = new List<int>();
            var ocs = new object();
            if (ocID > 0)
            {
                var oc = await _context.OCs.FindAsync(ocID);
                var OCS = await _ocService.GetListTreeOC(oc.ParentID, oc.ID);
                arrOCs = GetAllDescendants(OCS).Select(x => x.ID).ToList();
                ocs = await _context.OCs.Where(x => arrOCs.Contains(x.ID)).Select(x => new { x.ID, x.Name }).ToListAsync();
            }
            var users = await _context.Users.Where(x => arrOCs.Contains(x.OCID)).Select(x => new { x.ID, x.Username }).ToListAsync();
            return new
            {
                users,
                ocs
            };
        }
        public async Task<List<ProjectViewModel>> GetListProject()
        {
            return await _projectService.GetListProject();
        }
        public async Task<List<TreeViewTask>> GetListTree()
        {
            var listTasks = await _context.Tasks
                .Where(x => x.Status == false)
                .Include(x => x.User)
                .OrderBy(x => x.Level).ToListAsync();

            var tasks = new List<TreeViewTask>();
            foreach (var item in listTasks)
            {
                var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                     .Include(x => x.User)
                     .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();

                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray());
                levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                levelItem.ProjectID = item.ProjectID;
                levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                levelItem.BeAssigneds = beAssigneds;
                levelItem.Level = item.Level;

                levelItem.ParentID = item.ParentID;
                levelItem.Priority = CastPriority(item.Priority);
                levelItem.PriorityID = item.Priority;
                levelItem.Description = item.Description;
                levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                levelItem.User = item.User;
                levelItem.FromWhere = _context.OCs.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault();

                levelItem.FromWho = _context.Users.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault();
                levelItem.JobName = item.JobName.IsNotAvailable();
                levelItem.state = item.Status == false ? "Undone" : "Done";
                levelItem.Remark = item.Remark.IsNotAvailable();
                levelItem.From = item.OCID > 0 ? _context.OCs.Find(item.OCID).Name : _context.Users.Find(item.FromWhoID).Username.IsNotAvailable();
                tasks.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = tasks.Where(c => c.ParentID == 0)
                            .Select(c => new TreeViewTask
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
                                Deadline = c.Deadline,
                                FromWho = c.FromWho,
                                FromWhere = c.FromWhere,
                                PIC = c.PIC,
                                PriorityID = c.PriorityID,
                                Priority = c.Priority,
                                BeAssigneds = c.BeAssigneds,
                                Follow = c.Follow,
                                children = GetChildren(tasks, c.ID)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        public async Task<IEnumerable<TreeViewTask>> GetListTree(int parentID, int id)
        {
            var listTasks = await _context.Tasks
               .Where(x => (x.Status == false && x.FinishedMainTask == false) || (x.Status == true && x.FinishedMainTask == false))
               .Include(x => x.User)
               .OrderBy(x => x.Level).ToListAsync();

            var tasks = new List<TreeViewTask>();
            foreach (var item in listTasks)
            {
                var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                     .Include(x => x.User)
                     .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();

                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                //levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray());
                //levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                //levelItem.ProjectID = item.ProjectID;
                //levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                //levelItem.BeAssigneds = beAssigneds;
                levelItem.Level = item.Level;

                levelItem.ParentID = item.ParentID;
                //levelItem.Priority = CastPriority(item.Priority);
                //levelItem.PriorityID = item.Priority;
                //levelItem.Description = item.Description;
                //levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                //levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                //levelItem.User = item.User;
                //levelItem.FromWhere = _context.OCs.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault();

                //levelItem.FromWho = _context.Users.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault();
                //levelItem.JobName = item.JobName.IsNotAvailable();
                //levelItem.state = item.Status == false ? "Undone" : "Done";
                //levelItem.Remark = item.Remark.IsNotAvailable();

                //levelItem.From = item.OCID > 0 ? _context.OCs.Find(item.OCID).Name : _context.Users.Find(item.FromWhoID).Username.IsNotAvailable();

                tasks.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = tasks.Where(c => c.ID == id && c.ParentID == parentID)
                            .Select(c => new TreeViewTask
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
                                Deadline = c.Deadline,
                                FromWho = c.FromWho,
                                FromWhere = c.FromWhere,
                                PIC = c.PIC,
                                PriorityID = c.PriorityID,
                                Priority = c.Priority,
                                BeAssigneds = c.BeAssigneds,
                                Follow = c.Follow,
                                children = GetChildren(tasks, c.ID)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        private async Task<IEnumerable<TreeViewTask>> GetListTreeForUndo(int parentID, int id)
        {
            var listTasks = await _context.Tasks
               .Where(x => x.Status == true && x.FinishedMainTask == true)
               .Include(x => x.User)
               .OrderBy(x => x.Level).ToListAsync();

            var tasks = new List<TreeViewTask>();
            foreach (var item in listTasks)
            {
                var beAssigneds = _context.Tags.Where(x => x.TaskID == item.ID)
                     .Include(x => x.User)
                     .Select(x => new BeAssigned { ID = x.User.ID, Username = x.User.Username }).ToList();

                var levelItem = new TreeViewTask();
                levelItem.ID = item.ID;
                levelItem.PIC = string.Join(" , ", _context.Tags.Where(x => x.TaskID == item.ID).Include(x => x.User).Select(x => x.User.Username).ToArray());
                levelItem.ProjectName = item.ProjectID == 0 ? "#N/A" : _context.Projects.Find(item.ProjectID).Name;
                levelItem.ProjectID = item.ProjectID;
                levelItem.Deadline = String.Format("{0:s}", item.DueDate);
                levelItem.BeAssigneds = beAssigneds;
                levelItem.Level = item.Level;

                levelItem.ParentID = item.ParentID;
                levelItem.Priority = CastPriority(item.Priority);
                levelItem.PriorityID = item.Priority;
                levelItem.Description = item.Description;
                levelItem.DueDate = String.Format("{0:MMM d, yyyy}", item.DueDate);
                levelItem.CreatedDate = String.Format("{0:MMM d, yyyy}", item.CreatedDate);
                levelItem.User = item.User;
                levelItem.FromWhere = _context.OCs.Where(x => x.ID == item.OCID).Select(x => new FromWhere { ID = x.ID, Name = x.Name }).FirstOrDefault();

                levelItem.FromWho = _context.Users.Where(x => x.ID == item.FromWhoID).Select(x => new BeAssigned { ID = x.ID, Username = x.Username }).FirstOrDefault();
                levelItem.JobName = item.JobName.IsNotAvailable();
                levelItem.state = item.Status == false ? "Undone" : "Done";
                levelItem.Remark = item.Remark.IsNotAvailable();

                levelItem.From = item.OCID > 0 ? _context.OCs.Find(item.OCID).Name : _context.Users.Find(item.FromWhoID).Username.IsNotAvailable();

                tasks.Add(levelItem);
            }

            List<TreeViewTask> hierarchy = new List<TreeViewTask>();

            hierarchy = tasks.Where(c => c.ID == id && c.ParentID == parentID)
                            .Select(c => new TreeViewTask
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
                                Deadline = c.Deadline,
                                FromWho = c.FromWho,
                                FromWhere = c.FromWhere,
                                PIC = c.PIC,
                                PriorityID = c.PriorityID,
                                Priority = c.Priority,
                                BeAssigneds = c.BeAssigneds,
                                Follow = c.Follow,
                                children = GetChildren(tasks, c.ID)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        public async Task<object> GetDeputies()
        {
            return await _context.Users.Where(x => x.Username != "admin").Select(x => new { x.Username, x.ID }).ToListAsync();
        }
        #endregion

        #region Event( Create Task, Sub-Task, Follow, Undo, Delete, Done, Remark, ...)

        public async Task<object> Unsubscribe(int id, int userid)
        {
            try
            {
                if (_context.Follows.Any(x => x.TaskID == id && x.UserID == userid))
                {
                    var sub = await _context.Follows.FirstOrDefaultAsync(x => x.TaskID == id && x.UserID == userid);
                    var taskModel = await _context.Tasks.FindAsync(sub.TaskID);

                    var tasks = await GetListTree(taskModel.ParentID, taskModel.ID);
                    var arrTasks = GetAllTaskDescendants(tasks).Select(x => x.ID).ToArray();

                    var listTasks = await _context.Tasks.Where(x => arrTasks.Contains(x.ID)).Select(x => x.ID).ToListAsync();


                    var listSub = await _context.Follows.Where(x => listTasks.Contains(x.TaskID) && x.UserID == userid).ToListAsync();
                    _context.Follows.RemoveRange(listSub);

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
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
            try
            {
                if (!await _context.Tasks.AnyAsync(x => x.ID == task.ID))
                {
                    var item = _mapper.Map<Data.Models.Task>(task);

                    //Level cha tang len 1 va gan parentid cho subtask
                    var taskParent = _context.Tasks.Find(item.ParentID);
                    item.Level = taskParent.Level + 1;
                    item.ParentID = task.ParentID;
                    item.ProjectID = task.ProjectID;
                    item.JobTypeID = taskParent.JobTypeID;

                    await _context.Tasks.AddAsync(item);
                    await _context.SaveChangesAsync();

                    if (task.PIC != null)
                    {
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
                    }
                    if (task.Deputies != null)
                    {
                        var deputies = new List<Deputy>();
                        foreach (var deputy in task.Deputies)
                        {
                            deputies.Add(new Deputy
                            {
                                UserID = deputy,
                                TaskID = item.ID
                            });
                        }
                        await _context.Deputies.AddRangeAsync(deputies);
                    }
                    await _context.SaveChangesAsync();

                    return true;

                }
                else
                {
                    var edit = _context.Tasks.Find(task.ID);
                    edit.Priority = task.Priority.ToUpper();
                    edit.Description = task.Description;
                    edit.JobName = task.JobName;
                    edit.Priority = task.Priority;
                    edit.Remark = task.Remark;
                    edit.EveryDay = task.Everyday;
                    edit.Monthly = task.Monthly;
                    edit.Quarterly = task.Quarterly;
                    edit.Remark = task.Remark;
                    edit.OCID = task.OCID;
                    edit.DueDate = task.Deadline.ToParseIso8601();

                    if (task.PIC != null)
                    {
                        var tags = new List<Tag>();
                        var listDelete = await _context.Tags.Where(x => task.PIC.Contains(x.UserID) && x.TaskID == edit.ID).ToListAsync();
                        if (listDelete.Count > 0)
                        {
                            _context.Tags.RemoveRange(listDelete);
                        }

                        foreach (var pic in task.PIC)
                        {
                            tags.Add(new Tag
                            {
                                UserID = pic,
                                TaskID = edit.ID
                            });
                            await _context.Tags.AddRangeAsync(tags);
                        }
                    }
                    if (task.Deputies != null)
                    {
                        var deputies = new List<Deputy>();
                        var listDelete = await _context.Deputies.Where(x => task.Deputies.Contains(x.UserID) && x.TaskID == edit.ID).ToListAsync();
                        if (listDelete.Count > 0)
                        {
                            _context.Deputies.RemoveRange(listDelete);
                        }

                        foreach (var deputy in task.Deputies)
                        {
                            deputies.Add(new Deputy
                            {
                                UserID = deputy,
                                TaskID = edit.ID
                            });
                            await _context.Deputies.AddRangeAsync(deputies);
                        }
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<Tuple<bool, string>> CreateTask(CreateTaskViewModel task)
        {
            try
            {
                var jobTypeID = 0;
                var listUsers = new List<int>();
                switch (task.JobTypeID)
                {
                    case Data.Enum.JobType.Project:
                        jobTypeID = (int)Data.Enum.JobType.Project;
                        break;
                    case Data.Enum.JobType.Routine:
                        jobTypeID = (int)Data.Enum.JobType.Routine;
                        break;
                    case Data.Enum.JobType.Abnormal:
                        jobTypeID = (int)Data.Enum.JobType.Abnormal;
                        break;
                    default:
                        break;
                }

                if (!await _context.Tasks.AnyAsync(x => x.ID == task.ID))
                {
                    if (jobTypeID == 0)
                        return Tuple.Create(false, "");
                    task.Priority = task.Priority.ToUpper();
                    var item = _mapper.Map<Data.Models.Task>(task);
                    item.Level = 1;
                    item.JobTypeID = jobTypeID;
                    await _context.Tasks.AddAsync(item);
                    await _context.SaveChangesAsync();

                    if (task.PIC != null)
                    {
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
                        await _context.SaveChangesAsync();
                        var user = await _context.Users.FindAsync(task.UserID);
                        var projectName = string.Empty;
                        if (item.ProjectID > 0)
                        {
                            var project = await _context.Projects.FindAsync(item.ProjectID);
                            projectName = project.Name;
                        }
                        string urlResult = $"/todolist/{item.JobName.ToUrlEncode()}";
                        await _notificationService.Create(new CreateNotifyParams
                        {
                            AlertType = Data.Enum.AlertType.Assigned,
                            Message = CheckMessage(item.JobTypeID, projectName, user.Username, item.JobName, Data.Enum.AlertType.Assigned),
                            Users = task.PIC.ToList(),
                            TaskID = item.ID,
                            URL = urlResult,
                            UserID = task.UserID
                        });
                        listUsers.AddRange(task.PIC);

                    }
                    if (task.Deputies != null)
                    {
                        var deputies = new List<Deputy>();
                        foreach (var deputy in task.Deputies)
                        {
                            deputies.Add(new Deputy
                            {
                                UserID = deputy,
                                TaskID = item.ID
                            });
                        }
                        await _context.Deputies.AddRangeAsync(deputies);
                        await _context.SaveChangesAsync();
                        listUsers.AddRange(task.Deputies);

                    }
                    return Tuple.Create(true, string.Join(",", listUsers.Distinct()));
                }
                else
                {
                    var everyTemp = "#N/A";
                    var monTemp = "#N/A";
                    var quaTemp = "#N/A";
                    DateTime dueTemp = new DateTime();

                    var edit = _context.Tasks.Find(task.ID);
                    everyTemp = edit.EveryDay;
                    monTemp = edit.Monthly;
                    quaTemp = edit.Quarterly;
                    dueTemp = edit.DueDate;

                    edit.Priority = task.Priority.ToUpper();
                    edit.Description = task.Description;
                    edit.JobName = task.JobName;
                    edit.Priority = task.Priority;
                    edit.EveryDay = task.Everyday;
                    edit.Monthly = task.Monthly;
                    edit.Quarterly = task.Quarterly;
                    edit.Remark = task.Remark;
                    edit.DepartmentID = task.DepartmentID;
                    edit.DueDate = task.Deadline.ToParseIso8601();

                    if (task.PIC != null)
                    {
                        //Lay la danh sach assigned
                        var oldPIC = await _context.Tags.Where(x => x.TaskID == edit.ID).Select(x => x.UserID).ToArrayAsync();
                        var newPIC = task.PIC;
                        //loc ra danh sach cac ID co trong newPIC ma khong co trong oldPIC
                        var withOutInOldPIC = newPIC.Except(oldPIC).ToArray();
                        if (withOutInOldPIC.Length > 0)
                        {
                            var tags = new List<Tag>();
                            foreach (var pic in withOutInOldPIC)
                            {
                                tags.Add(new Tag
                                {
                                    UserID = pic,
                                    TaskID = edit.ID
                                });
                            }
                            if (tags.Count > 0)
                            {
                                await _context.Tags.AddRangeAsync(tags);
                            }
                            var projectName = string.Empty;
                            if (edit.ProjectID > 0)
                            {
                                var project = await _context.Projects.FindAsync(edit.ProjectID);
                                projectName = project.Name;
                            }
                            var user = await _context.Users.FindAsync(task.UserID);
                            string urlResult = $"/todolist/{edit.JobName.ToUrlEncode()}";
                            await _notificationService.Create(new CreateNotifyParams
                            {
                                AlertType = Data.Enum.AlertType.Assigned,
                                Message = CheckMessage(edit.JobTypeID, projectName, user.Username, edit.JobName, Data.Enum.AlertType.Assigned),
                                Users = withOutInOldPIC.ToList(),
                                TaskID = edit.ID,
                                URL = urlResult,
                                UserID = task.UserID
                            });
                            listUsers.AddRange(withOutInOldPIC);

                        }
                        else
                        {
                            //Day la userID se bi xoa
                            var withOutInNewPIC = oldPIC.Where(x => !newPIC.Contains(x)).ToArray();
                            var listDeletePIC = await _context.Tags.Where(x => withOutInNewPIC.Contains(x.UserID) && x.TaskID.Equals(edit.ID)).ToListAsync();
                            _context.Tags.RemoveRange(listDeletePIC);
                        }
                    }


                    if (task.Deputies != null)
                    {
                        //Lay la danh sach assigned
                        var oldDeputies = await _context.Deputies.Where(x => x.TaskID == edit.ID).Select(x => x.UserID).ToArrayAsync();
                        var newDeputies = task.Deputies;
                        //loc ra danh sach cac ID co trong newPIC ma khong co trong oldPIC
                        var withOutInOldDeputy = newDeputies.Except(oldDeputies).ToArray();
                        if (withOutInOldDeputy.Length > 0)
                        {
                            var deputies = new List<Deputy>();
                            foreach (var deputy in withOutInOldDeputy)
                            {
                                deputies.Add(new Deputy
                                {
                                    UserID = deputy,
                                    TaskID = edit.ID
                                });
                            }
                            if (deputies.Count > 0)
                            {
                                await _context.Deputies.AddRangeAsync(deputies);
                            }
                            var projectName = string.Empty;
                            if (edit.ProjectID > 0)
                            {
                                var project = await _context.Projects.FindAsync(edit.ProjectID);
                                projectName = project.Name;
                            }
                            var user = await _context.Users.FindAsync(task.UserID);
                            string urlResult = $"/todolist/{edit.JobName.ToUrlEncode()}";
                            await _notificationService.Create(new CreateNotifyParams
                            {
                                AlertType = Data.Enum.AlertType.Deputy,
                                Message = CheckMessage(edit.JobTypeID, projectName, user.Username, edit.JobName, Data.Enum.AlertType.Deputy),
                                Users = withOutInOldDeputy.ToList(),
                                TaskID = edit.ID,
                                URL = urlResult,
                                UserID = task.UserID
                            });
                            listUsers.AddRange(withOutInOldDeputy);
                        }
                        else
                        {
                            //Day la userID se bi xoa
                            var withOutInNewPIC = oldDeputies.Where(x => !newDeputies.Contains(x)).ToArray();
                            var listDeletePIC = await _context.Deputies.Where(x => withOutInNewPIC.Contains(x.UserID) && x.TaskID.Equals(edit.ID)).ToListAsync();
                            _context.Deputies.RemoveRange(listDeletePIC);
                        }
                    }
                    //Thong bao khi thay doi deadline
                    if (everyTemp.IsNotAvailable() != task.Everyday.IsNotAvailable())
                    {
                        var pics = await _context.Tags.Where(x => x.TaskID.Equals(edit.ID)).Select(x => x.UserID).ToListAsync();
                        var everyday = await AlertDeadlineChanging(Data.Enum.AlertDeadline.EveryDay, edit, task.UserID, pics);
                        edit.Monthly = "#N/A";
                        edit.Quarterly = "#N/A";
                        edit.DueDate = new DateTime();
                        listUsers.AddRange(everyday.Item1);
                    }
                    if (monTemp.IsNotAvailable() != task.Monthly.IsNotAvailable())
                    {
                        var pics = await _context.Tags.Where(x => x.TaskID.Equals(edit.ID)).Select(x => x.UserID).ToListAsync();
                        var mon = await AlertDeadlineChanging(Data.Enum.AlertDeadline.Monthly, edit, task.UserID, pics);
                        edit.EveryDay = "#N/A";
                        edit.Quarterly = "#N/A";
                        edit.DueDate = new DateTime();
                        listUsers.AddRange(mon.Item1);
                    }
                    if (quaTemp.IsNotAvailable() != task.Quarterly.IsNotAvailable())
                    {
                        var pics = await _context.Tags.Where(x => x.TaskID.Equals(edit.ID)).Select(x => x.UserID).ToListAsync();
                        var qua = await AlertDeadlineChanging(Data.Enum.AlertDeadline.Quarterly, edit, task.UserID, pics);
                        listUsers.AddRange(qua.Item1);
                        edit.Monthly = "#N/A";
                        edit.EveryDay = "#N/A";
                        edit.DueDate = new DateTime();
                    }
                    if (dueTemp.Date != task.Deadline.ToParseIso8601().Date)
                    {
                        var pics = await _context.Tags.Where(x => x.TaskID.Equals(edit.ID)).Select(x => x.UserID).ToListAsync();
                        var due = await AlertDeadlineChanging(Data.Enum.AlertDeadline.Deadline, edit, task.UserID, pics);
                        listUsers.AddRange(due.Item1);
                        edit.Monthly = "#N/A";
                        edit.Quarterly = "#N/A";
                        edit.EveryDay = "#N/A";
                    }
                }
                await _context.SaveChangesAsync();

                return Tuple.Create(true, string.Join(",", listUsers.Distinct()));
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "");
            }
        }

        public async Task<object> Delete(int id, int userid)
        {
            try
            {

                var item = await _context.Tasks.FindAsync(id);
                if (!item.CreatedBy.Equals(userid))
                    return false;
                var tasks = await GetListTree(item.ParentID, item.ID);
                var arrTasks = GetAllTaskDescendants(tasks).Select(x => x.ID).ToArray();

                _context.Tags.RemoveRange(await _context.Tags.Where(x => arrTasks.Contains(x.TaskID)).ToListAsync());
                _context.Tasks.RemoveRange(await _context.Tasks.Where(x => arrTasks.Contains(x.ID)).ToListAsync());
                _context.Follows.RemoveRange(await _context.Follows.Where(x => arrTasks.Contains(x.TaskID)).ToListAsync());

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Tuple<bool, bool, string>> Done(int id, int userid)
        {
            try
            {
                var flag = true;
                var item = await _context.Tasks.FindAsync(id);
                var projectName = string.Empty;
                if (item.ProjectID > 0)
                {
                    var project = await _context.Projects.FindAsync(item.ProjectID);
                    projectName = project.Name;
                }

                var user = await _context.Users.FindAsync(userid);

                var tasks = await GetListTree(item.ParentID, item.ID);
                var arrTasks = GetAllTaskDescendants(tasks).Select(x => x.ID).ToArray();
                var arrbool = _context.Tasks.Where(x => arrTasks.Contains(x.ID)).ToList();

                var listUsers = await GetListUsersAlert(item.JobTypeID, userid, item.ProjectID);

                //Neu khong co con thi chuyen qua history luon
                if (arrbool.Count.Equals(1) && item.Level.Equals(1))
                {
                    item.FinishedMainTask = true;
                }
                else if (arrbool.Count.Equals(1) && item.Level > 1)
                {
                    item.Status = true;
                    item.FinishedMainTask = false;
                }
                else //Neu co con chau thi fai hoan thanh con chau xong moi chuyen qua history
                {
                    //Kiem tra xem da hoan thanh tat ca cac task con chua
                    arrbool.Where(x => x.ID != id).Select(x => x.Status).ToList().ForEach(x =>
                    {
                        if (!x)
                        {
                            flag = false;
                            return;
                        }

                    });
                    //Neu hoan thanh cac task con thi chuyen qua history
                    if (flag)
                    {
                        arrbool.ForEach(task =>
                        {
                            task.Status = true;
                            task.FinishedMainTask = true;
                        });

                    }
                }
                //Hoan thanh task con roi moi cho hoan thanh main task
                if (!flag)
                    return Tuple.Create(false, false, "");

                item.Status = true;
                await _context.SaveChangesAsync();

                //Task nao theo doi thi moi thong bao
                var listUserfollowed = new List<int>();

                if (item.Level.Equals(1)) //Level = 1 thi chi thong bao khi task do hoan thanh
                    listUserfollowed = (await _context.Follows.Where(x => x.TaskID == item.ID).Select(x => x.UserID).ToListAsync()).ToList();
                else//level > 1 tuc la co con thi thong bao khi con no va no hoan thanh
                    listUserfollowed = (await _context.Follows.Where(x => arrTasks.Contains(x.TaskID)).Select(x => x.UserID).ToListAsync()).ToList();

                var followed = listUserfollowed.Count > 0 ? true : false;
                //listUsers = listUsers.Union(listUserfollowed.Select(x => x.UserID).ToList()).ToList();
                //Chi thong bao den nhung ai da followed
                var listPIC = await _context.Tags.Where(x => x.TaskID.Equals(item.ID)).Select(x => x.UserID).ToListAsync();
                var listDeputy = await _context.Deputies.Where(x => x.TaskID.Equals(item.ID)).Select(x => x.UserID).ToListAsync();
                var listManager = await _context.Managers.Where(x => x.ProjectID.Equals(item.ID)).Select(x => x.UserID).ToListAsync();
                var listMember = await _context.TeamMembers.Where(x => x.ProjectID.Equals(item.ID)).Select(x => x.UserID).ToListAsync();
                var listAlertAll = listPIC.Union(listDeputy).Union(listManager).Union(listMember);

                string urlTodolist = $"/todolist/{item.JobName.ToUrlEncode()}";
                await _notificationService.Create(new CreateNotifyParams
                {
                    AlertType = Data.Enum.AlertType.Done,
                    Message = CheckMessage(item.JobTypeID, projectName, user.Username, item.JobName, Data.Enum.AlertType.Done),
                    Users = listUserfollowed.Distinct().ToList(),
                    TaskID = item.ID,
                    URL = urlTodolist,
                    UserID = userid
                });

                if (followed)
                {
                    string urlResult = $"/follow/{item.JobName.ToUrlEncode()}";
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.Done,
                        Message = CheckMessage(item.JobTypeID, projectName, user.Username, item.JobName, Data.Enum.AlertType.Done),
                        Users = listUserfollowed.Distinct().ToList(),
                        TaskID = item.ID,
                        URL = urlResult,
                        UserID = userid
                    });
                }
                return Tuple.Create(true, followed, string.Join(",", listUserfollowed.Union(listAlertAll).ToArray()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Tuple.Create(false, false, "");
            }
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


        public async Task<Tuple<bool, bool, string>> Remark(RemarkViewModel remark)
        {
            try
            {
                if (!await _context.Tasks.AnyAsync(x => x.ID == remark.ID))
                    return Tuple.Create(false, false, "");
                string message = string.Empty;
                var update = await _context.Tasks.FindAsync(remark.ID);
                var user = await _context.Users.FindAsync(remark.UserID);

                update.Remark = remark.Remark;

                //var listUsers = await GetListUsersAlert(update.JobTypeID, remark.UserID, update.ProjectID); 
                //Chi thong bao khi da theo doi
                var listUserfollowed = (await _context.Follows.Where(x => x.TaskID.Equals(update.ID)).Select(x => x.UserID).ToListAsync()).ToList();
                string projectName = string.Empty;
                if (update.ProjectID > 0)
                {
                    projectName = (await _context.Projects.FindAsync(update.ProjectID)).Name;
                }
                await _context.SaveChangesAsync();
                //Task nao theo doi thi moi thong bao
                var followed = await _context.Follows.AnyAsync(x => x.TaskID.Equals(update.ID));
                if (followed)
                {
                    string urlResult = $"/follow/{update.JobName.ToUrlEncode()}";
                    await _notificationService.Create(new CreateNotifyParams
                    {
                        AlertType = Data.Enum.AlertType.Done,
                        Message = CheckMessageRemark(update.JobTypeID, update.Remark, projectName, update.JobName, user.Username),
                        Users = listUserfollowed,
                        TaskID = update.ID,
                        URL = urlResult,
                        UserID = remark.UserID
                    });
                }

                return Tuple.Create(true, followed, string.Join(",", listUserfollowed.ToArray()));
            }
            catch (Exception)
            {
                return Tuple.Create(true, false, "");
            }
        }

        public async Task<object> Follow(int userid, int taskid)
        {
            try
            {
                var taskModel = await _context.Tasks.FindAsync(taskid);
                var tasks = await GetListTree(taskModel.ParentID, taskModel.ID);
                var arrTasks = GetAllTaskDescendants(tasks).Select(x => x.ID).ToArray();

                var listTasks = await _context.Tasks.Where(x => arrTasks.Contains(x.ID)).ToListAsync();
                if (_context.Follows.Any(x => x.TaskID == taskid && x.UserID == userid))
                {
                    _context.Remove(_context.Follows.FirstOrDefault(x => x.TaskID == taskid && x.UserID == userid));
                    await _context.SaveChangesAsync();

                    return true;

                }
                var listSubcribes = new List<Follow>();
                listTasks.ForEach(task =>
                {
                    listSubcribes.Add(new Follow { TaskID = task.ID, UserID = userid });
                });
                await _context.AddRangeAsync(listSubcribes);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<object> Undo(int id)
        {
            if (!await _context.Tasks.AnyAsync(x => x.ID == id))
                return false;
            var item = await _context.Tasks.FindAsync(id);
            var tasks = await GetListTreeForUndo(item.ParentID, item.ID);

            var arrTasks = GetAllTaskDescendants(tasks).Select(x => x.ID).ToArray();
            var arrs = _context.Tasks.Where(x => arrTasks.Contains(x.ID)).ToList();
            arrs.ForEach(task =>
            {
                task.Status = false;
                task.FinishedMainTask = false;
            });
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
        #endregion
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
