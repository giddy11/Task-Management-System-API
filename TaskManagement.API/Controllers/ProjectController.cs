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
[Authorize]
public class ProjectController : BaseController
{
    public ProjectController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectController> logger) : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new project.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectFetchResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ProjectCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddErrors(errors)
                    .ResponseResult();
            }

            // Check if user exists
            var userExists = await UnitOfWork.UserRepository.GetByIdAsync(request.CreatedById);
            if (userExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {request.CreatedById} not found")
                    .ResponseResult();
            }

            //// Validate date range
            //if (request.StartDate >= request.EndDate)
            //{
            //    return OperationResponse.FailedResponse(StatusCode.BadRequest)
            //        .AddError("Start date must be before End date")
            //        .ResponseResult();
            //}

            var project = Mapper.Map<Project>(request);

            await UnitOfWork.ProjectRepository.AddAsync(project);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var projectDto = Mapper.Map<ProjectFetchResponse>(project);
            return OperationResponse<ProjectFetchResponse>.CreatedResponse(projectDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating project");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while creating the project")
                .ResponseResult();
        }
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectFetchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid project ID")
                    .ResponseResult();
            }

            var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Project with ID {id} not found")
                    .ResponseResult();
            }

            var projectDto = Mapper.Map<ProjectFetchResponse>(project);
            return OperationResponse<ProjectFetchResponse>.SuccessfulResponse(projectDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving project with ID {ProjectId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving the project")
                .ResponseResult();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectFetchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var projects = await UnitOfWork.ProjectRepository.GetAllAsync();
            if (projects == null || !projects.Any())
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError("No projects found")
                    .ResponseResult();
            }

            var projectDtos = Mapper.Map<IEnumerable<ProjectFetchResponse>>(projects);
            return OperationResponse<IEnumerable<ProjectFetchResponse>>.SuccessfulResponse(projectDtos)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving projects");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving projects")
                .ResponseResult();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddErrors(errors)
                    .ResponseResult();
            }

            if (id != request.Id)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("ID in URL does not match ID in request body")
                    .ResponseResult();
            }

            var existingProject = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
            if (existingProject == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Project with ID {id} not found")
                    .ResponseResult();
            }

            Mapper.Map(request, existingProject);
            await UnitOfWork.ProjectRepository.UpdateAsync(existingProject);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating project with ID {ProjectId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while updating the project")
                .ResponseResult();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Project with ID {id} not found")
                    .ResponseResult();
            }

            await UnitOfWork.ProjectRepository.DeleteAsync(project);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            return OperationResponse.SuccessfulResponse()
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting project with ID {ProjectId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while deleting the project")
                .ResponseResult();
        }
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ProjectStatus status)
    {
        try
        {
            if (!Enum.IsDefined(typeof(ProjectStatus), status))
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid project status provided")
                    .ResponseResult();
            }

            var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Project with ID {id} not found")
                    .ResponseResult();
            }

            project.ProjectStatus = status;
            await UnitOfWork.ProjectRepository.UpdateAsync(project);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing status for project with ID {ProjectId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while changing project status")
                .ResponseResult();
        }
    }
}