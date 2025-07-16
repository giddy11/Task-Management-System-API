using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Projects;
using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Domain.Projects;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectService;

        public ProjectController(IProjectRepository projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var response = await _projectService.CreateAsync(request);
            return response.ResponseResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _projectService.GetByIdAsync(id);
            return response.ResponseResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var response = await _projectService.GetAllAsync(page, pageSize);
            return response.ResponseResult();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
        {
            var response = await _projectService.UpdateAsync(id, request);
            return response.ResponseResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _projectService.DeleteAsync(id);
            return response.ResponseResult();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ProjectStatus status)
        {
            var response = await _projectService.ChangeStatusAsync(id, status);
            return response.ResponseResult();
        }
    }
}
