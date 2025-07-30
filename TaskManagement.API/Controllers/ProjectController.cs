using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.Projects.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.Projects;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles project management operations such as creating, retrieving, updating, deleting, and changing the status of projects.
/// </summary>
[Route("project")]
//[Authorize]
public class ProjectController : BaseController
{
    public ProjectController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
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
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ProjectCreateRequest request)
    {
        if (request == null)
        {
            return BadRequest("Project request cannot be null.");
        }
        if (request.StartDate >= request.EndDate)
        {
            return BadRequest("Start date must be earlier than end date.");
        }
        var project = Mapper.Map<Project>(request);
        await UnitOfWork.ProjectRepository.AddAsync(project);
        var response = await UnitOfWork.SaveChangesAsync();
        if (response.IsSuccessful)
        {
            var projectDto = Mapper.Map<ProjectFetchResponse>(project);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectDto.Id }, projectDto);
        }
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
    [ProducesResponseType<ProjectFetchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[Authorize(Policy = "UserOrAbove")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Project ID cannot be empty.");
        }
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        var projectDto = Mapper.Map<ProjectFetchResponse>(project);
        var response = OperationResponse<ProjectFetchResponse>.SuccessfulResponse(projectDto);
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
    [ProducesResponseType<IReadOnlyList<ProjectFetchResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[Authorize(Policy = "UserOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var projects = await UnitOfWork.ProjectRepository.GetAllAsync();
        if (projects == null || !projects.Any())
        {
            return NotFound("No projects found.");
        }
        var projectDtos = Mapper.Map<IReadOnlyList<ProjectFetchResponse>>(projects);
        var response = OperationResponse<IReadOnlyList<ProjectFetchResponse>>.SuccessfulResponse(projectDtos);
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
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, ProjectUpdateRequest request)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        // Map the request to the existing project
        Mapper.Map(request, project);

        await UnitOfWork.ProjectRepository.UpdateAsync(project);
        var response = await UnitOfWork.SaveChangesAsync();
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
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        await UnitOfWork.ProjectRepository.DeleteAsync(project);
        await UnitOfWork.SaveChangesAsync();
        return NoContent();
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
    //[Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ProjectStatus status)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        if (!Enum.IsDefined(typeof(ProjectStatus), status))
        {
            return BadRequest("Invalid project status provided.");
        }
        project.ProjectStatus = status;
        await UnitOfWork.ProjectRepository.UpdateAsync(project);
        var response = await UnitOfWork.SaveChangesAsync();
        return response.ResponseResult();
    }
}