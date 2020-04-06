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
        Task<Tuple<bool, string>> Add(Comment comment,int currentUser);
        //Task<object> Delete();
        Task<Tuple<bool, string>> AddSub(AddSubViewModel subcomment);
        Task<object> Seen(int comtID, int userID);
        Task<IEnumerable<CommentTreeView>> GetAllTreeView(int taskid, int userid);
        Task<IEnumerable<TaskHasComment>> GetAllCommentWithTask(int userid);
    }
}
