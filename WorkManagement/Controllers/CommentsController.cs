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
using Service.Interface;
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
        public async Task<IActionResult> GetAll(int taskID,int userID)
        {
            return Ok(await _commentService.GetAllTreeView(taskID, userID));
        }
        [HttpPost]
        public async Task<IActionResult> Add(Comment comment)
        {
            return Ok(await _commentService.Add(comment));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddSub(AddSubViewModel subComment)
        {
            return Ok(await _commentService.AddSub(subComment));
        }
    }
}