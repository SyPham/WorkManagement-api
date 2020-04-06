using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.ViewModel.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Helpers;
using Service.Interface;
using WorkManagement.Helpers;
using WorkManagement.Hub;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly IHubContext<WorkingManagementHub> _hubContext;
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService, IHubContext<WorkingManagementHub> hubContext)
        {
            _commentService = commentService;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet("{taskID}/{userID}")]
        public async Task<IActionResult> GetAll(int taskID, int userID)
        {
            return Ok(await _commentService.GetAllTreeView(taskID, userID));
        }
        [AllowAnonymous]
        [HttpGet("{userID}")]
        public async Task<IActionResult> GetAllCommentWithTask(int userID)
        {
            return Ok(await _commentService.GetAllCommentWithTask(userID));
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(Comment comment)
        {
            string token = Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var model = await _commentService.Add(comment,userID);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", model.Item2, "message");
            return Ok(model.Item1);
        }
        [HttpPost]
        public async Task<IActionResult> AddSub(AddSubViewModel subComment)
        {
            string token = Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            subComment.CurrentUser = userID;
            var model = await _commentService.AddSub(subComment);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", model.Item2, "message");
            return Ok(model.Item1);
        }
    }
}