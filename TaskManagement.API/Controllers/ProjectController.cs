using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Projects;
using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Domain.Projects;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles project management operations such as creating, retrieving, updating, deleting, and changing the status of projects.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _projectService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectController"/> class.
    /// </summary>
    /// <param name="projectService">The project repository service for handling project operations.</param>
    public ProjectController(IProjectRepository projectService)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request containing project details.</param>
    /// <returns>A response indicating the result of the create operation.</returns>
    /// <response code="201">Project created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var response = await _projectService.CreateAsync(request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the project.</param>
    /// <returns>The project details if found.</returns>
    /// <response code="200">Project retrieved successfully.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _projectService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a paginated list of all projects.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of projects per page (default is 10).</param>
    /// <returns>A paginated list of projects.</returns>
    /// <response code="200">Projects retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "UserOrAbove")]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var response = await _projectService.GetAllAsync(page, pageSize);
        return response.ResponseResult();
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the project to update.</param>
    /// <param name="request">The updated project details.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">Project updated successfully.</response>
    /// <response code="400">Invalid request data or ID mismatch.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
    {
        var response = await _projectService.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the project to delete.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <response code="204">Project deleted successfully.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _projectService.DeleteAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Changes the status of a project.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the project.</param>
    /// <param name="status">The new project status.</param>
    /// <returns>A response indicating the result of the status change operation.</returns>
    /// <response code="200">Project status updated successfully.</response>
    /// <response code="400">Invalid status provided.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ProjectStatus status)
    {
        var response = await _projectService.ChangeStatusAsync(id, status);
        return response.ResponseResult();
    }
}