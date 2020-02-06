using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace WorkManagement.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;


        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }
        [HttpGet("/GetUser/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(await _userService.GetByID(id));
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            return Ok(await _userService.Create(user));
        }
        [HttpPost]
        public async Task<IActionResult> Update(User user )
        {
            return Ok(await _userService.Update(user));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _userService.Delete(id));
        }
        [HttpGet("{keyword}/{page}/{pageSize}")]
        public async Task<ActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            return Ok(await _userService.GetAllPaging(keyword, page, pageSize));
        }
    }
}