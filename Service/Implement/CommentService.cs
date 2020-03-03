using Data;
using Data.Models;
using Data.ViewModel.Comment;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _context;

        public CommentService(DataContext context)
        {
            _context = context;
        }

        public async Task<object> Add(Comment comment)
        {
            try
            {
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                await _context.AddAsync(new CommentDetail { CommentID = comment.ID, UserID = comment.UserID, Seen = true });
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            throw new NotImplementedException();
        }
        public async Task<object> AddSub(AddSubViewModel subcomment)
        {
            try
            {
                var comment = new Comment
                {
                    ParentID=subcomment.ParentID,
                    TaskID=subcomment.TaskID,
                    UserID=subcomment.UserID,
                    Content=subcomment.Content
                };
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                await _context.AddAsync(new CommentDetail { CommentID = comment.ID, UserID = comment.UserID, Seen = true });
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            throw new NotImplementedException();
        }
        private async Task<List<CommentTreeView>> GetAll(int taskID, int userID)
        {
            var detail = _context.CommentDetails;
            return await _context.Comments
                .Join(_context.Users,
                comt => comt.UserID,
                user => user.ID,
                (comt, user) => new { comt, user })
                .Where(x => x.comt.TaskID.Equals(taskID))
                .Select(_ => new CommentTreeView
                {
                    ID = _.comt.ID,
                    UserID = _.comt.UserID,
                    Username = _.user.Username,
                    ImageBase64 = _.user.ImageBase64,
                    Content = _.comt.Content,
                    ParentID=_.comt.ParentID,
                    CreatedTime=_.comt.CreatedTime,
                    Seen = detail.FirstOrDefault(d => d.CommentID.Equals(_.comt.ID) && d.UserID.Equals(userID)) == null ? false : true
                })
                .ToListAsync();
        }
        public List<CommentTreeView> GetChildren(List<CommentTreeView> comments, int parentid)
        {
            return comments
                    .Where(c => c.ParentID == parentid)
                    .Select(c => new CommentTreeView()
                    {
                        ID = c.ID,
                        UserID = c.UserID,
                        Username = c.Username,
                        Content = c.Content,
                        ImageBase64 = c.ImageBase64,
                        ParentID = c.ParentID,
                        CreatedTime = c.CreatedTime,
                        Seen=c.Seen,
                        children = GetChildren(comments, c.ID)
                    })
                    .ToList();
        }
        public async Task<IEnumerable<CommentTreeView>> GetAllTreeView(int taskid,int userid)
        {
            var listComments = await GetAll(taskid, userid);
            List<CommentTreeView> hierarchy = new List<CommentTreeView>();

          
            hierarchy = listComments.Where(c => c.ParentID.Equals(0))
                            .Select(c => new CommentTreeView()
                            {
                                ID = c.ID,
                                UserID = c.UserID,
                                Username = c.Username,
                                Content = c.Content,
                                ImageBase64 = c.ImageBase64,
                                ParentID = c.ParentID,
                                Seen = c.Seen,
                                CreatedTime = c.CreatedTime,
                                children = GetChildren(listComments, c.ID)
                            })
                            .ToList();


            HieararchyWalkTree(hierarchy);

            return hierarchy;
        }
        private void HieararchyWalkTree(List<CommentTreeView> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    //Console.WriteLine(string.Format("{0} {1}", item.Id, item.Text));
                    HieararchyWalkTree(item.children);
                }
            }
        }
        public async Task<object> Seen(int comtID, int userID)
        {
            try
            {
                var detail = await _context.CommentDetails.FirstOrDefaultAsync(d => d.CommentID.Equals(comtID) && d.UserID.Equals(userID));
                if (detail == null)
                {
                    await _context.AddAsync(new CommentDetail { CommentID = comtID, UserID = userID, Seen = true });
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
