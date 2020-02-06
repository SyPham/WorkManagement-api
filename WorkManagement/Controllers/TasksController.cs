using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.ViewModel.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers;
using Service.Interface;
using WorkManagement.Helpers;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [AllowAnonymous]
        [HttpGet("{name}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadTask(string name, int page, int pageSize)
        {
            string token = Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var OCID = JWTExtensions.GetDecodeTokenByProperty(token, "OCID").ToInt();

            return Ok(await _taskService.LoadTask(name, userID, OCID, page, pageSize));
        }

        [AllowAnonymous]
        [HttpGet("{name}/{page}/{pageSize}")]
        public async Task<IActionResult> LoadTaskHistory(string name, int page, int pageSize)
        {
            string token = Request.Headers["Authorization"];
            var userID = JWTExtensions.GetDecodeTokenByProperty(token, "nameid").ToInt();
            var OCID = JWTExtensions.GetDecodeTokenByProperty(token, "OCID").ToInt();

            return Ok(await _taskService.LoadTaskHistory(name, userID, OCID, page, pageSize));
        }

        [HttpGet("{taskId}/{remark}")]
        public async Task<IActionResult> CreateRemark(int taskId, string remark)
        {
            return Ok(await _taskService.CreateRemark(taskId, remark));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListTreeTask()
        {
            return Ok(await _taskService.GetListTree());
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody]CreateTaskViewModel createTask)
        {
            return Ok(await _taskService.CreateTask(createTask));
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubTask([FromBody]CreateTaskViewModel createTask)
        {
            return Ok(await _taskService.CreateSubTask(createTask));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody]UpdateTaskViewModel createTask)
        {
            return Ok(await _taskService.UpdateTask(createTask));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            return Ok(await _taskService.Delete(id));
        }
       
    }
}