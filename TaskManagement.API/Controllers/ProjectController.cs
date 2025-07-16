using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Projects;
using TaskManagement.Application.Projects.Dtos;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectRepository projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var response = await _projectService.CreateAsync(request);
            return response.ResponseResult();
        }
    }
}
