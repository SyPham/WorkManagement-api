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

namespace WorkManagement.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IProjectService _projectService;

        public ProjectsController(DataContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        // GET: api/Projects
        [HttpGet("{keyword}/{page}/{pageSize}")]
        public async Task<ActionResult> GetAllPaging(string keyword, int page, int pageSize)
        {
            return Ok(await _projectService.GetAllPaging(keyword, page, pageSize));
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID(int id)
        {
            var project = await _projectService.GetByID(id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Update(Project project)
        {
            return Ok(await _projectService.Update(project));
        }

        // POST: api/Projects
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Project>> Create(Project project)
        {
            return Ok(await _projectService.Create(project));
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            return Ok(await _projectService.Delete(id));
        }

      
    }
}
