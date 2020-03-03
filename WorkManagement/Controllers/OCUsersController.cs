using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using Service.Interface;
using Data.ViewModel.OC;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OCUsersController : ControllerBase
    {
        private readonly IOCUserService _ocUserService;

        public OCUsersController(IOCUserService ocUserService)
        {
            _ocUserService = ocUserService;
        }

        [HttpGet("{ocid}")]
        public async Task<ActionResult> GetListUser(int ocid)
        {
            return Ok(await _ocUserService.GetListUser(ocid));
        }

        [HttpGet("{userid}/{ocid}/{status}")]
        public async Task<ActionResult> AddOrUpdate(int userid, int ocid,bool status)
        {
            return Ok( await _ocUserService.AddOrUpdate(userid,ocid, status));
        }
    }
}
