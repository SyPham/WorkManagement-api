using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace WorkManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;


        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListUser()
        {
            return Ok(await _userService.GetListUser());
        }
        [HttpGet("/GetUser/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(await _userService.GetByID(id));
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel user)
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