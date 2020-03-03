
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Hub
{
    public class WorkingManagementHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly Data.DataContext _context;

        public WorkingManagementHub(Data.DataContext context)
        {
            _context = context;
        }
        private async Task<string> GetUsername(string user)
        {
            try
            {
                var userid = user.ToInt();
               return (await _context.Users.FirstOrDefaultAsync(x => x.ID.Equals(userid))).Username.ToTitleCase();
            }
            catch (Exception ex)
            {
                return "Someone";
                throw;
            }
            throw new NotImplementedException();
        }
        private async Task<object> AddMessageGroup(int roomid, string message, int userid)
        {
            try
            {
                var project = await _context.Projects.FirstOrDefaultAsync(x => x.Room.Equals(roomid));
                var managers = await _context.Managers.Where(x => x.ProjectID.Equals(project.ID)).Select(x => x.UserID).ToListAsync();
                var members = await _context.TeamMembers.Where(x => x.ProjectID.Equals(project.ID)).Select(x => x.UserID).ToListAsync();
                var listAll = managers.Union(members);
                var listChats = new List<Data.Models.Chat>();
                var listParticipants = new List<Data.Models.Participant>();

                //Neu chua co participan thi them vao
                if (!await _context.Participants.AnyAsync(x => x.RoomID == roomid))
                {
                    foreach (var user in listAll)
                    {
                        listParticipants.Add(new Data.Models.Participant
                        {
                            UserID = user,
                            RoomID = roomid
                        });
                    }
                    await _context.AddRangeAsync(listParticipants);
                }
                //add message userid
                await _context.AddAsync(new Data.Models.Chat
                {
                    Message = message,
                    UserID = userid,
                    ProjectID = project.ID,
                    RoomID = roomid
                });
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
        }
        public async Task SendMessage(string user, string message)
        {
            var id = Context.ConnectionId;//"LzX9uE94Ovlp6Yx8s6PvhA"
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToUser(string connectionId, string message)
        {
            return Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {

            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public async Task JoinGroup(string group, string user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("ReceiveJoinGroup", user, await GetUsername(user));
        }
        public async Task Typing(string group,string user)
        {
            await Clients.Group(group).SendAsync("ReceiveTyping", user, await GetUsername(user));
        }
        public async Task StopTyping(string group, string user)
        {
            await Clients.Group(group).SendAsync("ReceiveStopTyping", user);
        }
        public async Task SendMessageToGroup(string group, string message, string user)
        {
            ////Luu vo db
            ////Chi gui den nhung nguoi tham gia phong
            ///
            int roomid = group.ToInt();
            int userid = user.ToInt();
            await AddMessageGroup(roomid, message, userid);
            await Clients.Group(group).SendAsync("ReceiveMessageGroup", message);
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }

    }
}
