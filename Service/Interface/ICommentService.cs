using Data.Models;
using Data.ViewModel.Comment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface ICommentService
    {
        Task<object> Add(Comment comment);
        //Task<object> Delete();
        Task<object> AddSub(AddSubViewModel subcomment);
        Task<object> Seen(int comtID, int userID);
        Task<IEnumerable<CommentTreeView>> GetAllTreeView(int taskid, int userid);
    }
}
