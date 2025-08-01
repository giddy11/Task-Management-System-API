using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.TodoTasks.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles TodoTask management operations such as creating, retrieving, updating, deleting, changing status, and assigning labels.
/// </summary>
[Route("todo-task")]
[Authorize]
public class TodoTaskController : BaseController
{
    public TodoTaskController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TodoTaskController> logger) : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// Creates a new TodoTask.
    /// </summary>
    /// <param name="request">The TodoTask creation request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new TodoTask.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TodoTaskCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] TodoTaskCreateRequest request)
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

            var userExists = await UnitOfWork.UserRepository.GetByIdAsync(request.CreatedById);
            if (userExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {request.CreatedById} not found")
                    .ResponseResult();
            }

            var projectExists = await UnitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
            if (projectExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Project with ID {request.ProjectId} not found")
                    .ResponseResult();
            }

            var todoTask = Mapper.Map<TodoTask>(request);
            await UnitOfWork.TodoTaskRepository.AddAsync(todoTask);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var todoTaskDto = Mapper.Map<TodoTaskCreateResponse>(todoTask);
            return OperationResponse<TodoTaskCreateResponse>.CreatedResponse(todoTaskDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating TodoTask");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while creating the TodoTask")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves a TodoTask by ID.
    /// </summary>
    /// <param name="id">The ID of the TodoTask to retrieve.</param>
    /// <returns>An IActionResult containing the TodoTask details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoTaskFetchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTodoTaskById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid TodoTask ID")
                    .ResponseResult();
            }

            var todoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(id);
            if (todoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {id} not found")
                    .ResponseResult();
            }

            var todoTaskDto = Mapper.Map<TodoTaskFetchResponse>(todoTask);
            return OperationResponse<TodoTaskFetchResponse>.SuccessfulResponse(todoTaskDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving TodoTask with ID {TodoTaskId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving the TodoTask")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves all TodoTasks.
    /// </summary>
    /// <returns>An IActionResult containing a list of all TodoTasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoTaskFetchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var todoTasks = await UnitOfWork.TodoTaskRepository.GetAllAsync();

            var todoTaskDtos = Mapper.Map<IEnumerable<TodoTaskFetchResponse>>(todoTasks);
            return OperationResponse<IEnumerable<TodoTaskFetchResponse>>.SuccessfulResponse(todoTaskDtos)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving TodoTasks");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving TodoTasks")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Updates an existing TodoTask.
    /// </summary>
    /// <param name="id">The ID of the TodoTask to update.</param>
    /// <param name="request">The TodoTask update request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoTaskUpdateRequest request)
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

            var existingTodoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(id);
            if (existingTodoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {id} not found")
                    .ResponseResult();
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate >= request.EndDate)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Start date must be before End date")
                    .ResponseResult();
            }

            Mapper.Map(request, existingTodoTask);
            await UnitOfWork.TodoTaskRepository.UpdateAsync(existingTodoTask);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating TodoTask with ID {TodoTaskId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while updating the TodoTask")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Deletes a TodoTask by ID.
    /// </summary>
    /// <param name="id">The ID of the TodoTask to delete.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var todoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(id);
            if (todoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {id} not found")
                    .ResponseResult();
            }

            await UnitOfWork.TodoTaskRepository.DeleteAsync(todoTask);
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
            Logger.LogError(ex, "Error deleting TodoTask with ID {TodoTaskId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while deleting the TodoTask")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Changes the status of a TodoTask.
    /// </summary>
    /// <param name="id">The ID of the TodoTask.</param>
    /// <param name="status">The new TodoTask status.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] TodoTaskStatus status)
    {
        try
        {
            if (!Enum.IsDefined(typeof(TodoTaskStatus), status))
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid TodoTask status provided")
                    .ResponseResult();
            }

            var todoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(id);
            if (todoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {id} not found")
                    .ResponseResult();
            }

            todoTask.TodoTaskStatus = status;
            await UnitOfWork.TodoTaskRepository.UpdateAsync(todoTask);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing status for TodoTask with ID {TodoTaskId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while changing TodoTask status")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Changes the priority status of a TodoTask.
    /// </summary>
    /// <param name="id">The ID of the TodoTask.</param>
    /// <param name="priority">The new priority status.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPatch("{id}/priority")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePriority(Guid id, [FromBody] PriorityStatus priority)
    {
        try
        {
            if (!Enum.IsDefined(typeof(PriorityStatus), priority))
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid priority status provided")
                    .ResponseResult();
            }

            var todoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(id);
            if (todoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {id} not found")
                    .ResponseResult();
            }

            todoTask.PriorityStatus = priority;
            await UnitOfWork.TodoTaskRepository.UpdateAsync(todoTask);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing priority for TodoTask with ID {TodoTaskId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while changing TodoTask priority")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Assigns a label to a TodoTask.
    /// </summary>
    /// <param name="request">The label assignment request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("assign-label")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignLabel([FromBody] LabelTaskRequest request)
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

            var todoTask = await UnitOfWork.TodoTaskRepository.GetByIdAsync(request.TodoTaskId);
            if (todoTask == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"TodoTask with ID {request.TodoTaskId} not found")
                    .ResponseResult();
            }

            var label = await UnitOfWork.LabelRepository.GetByIdAsync(request.LabelId);
            if (label == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Label with ID {request.LabelId} not found")
                    .ResponseResult();
            }

            if (todoTask.Labels.Any(l => l.Id == request.LabelId))
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError($"Label with ID {request.LabelId} is already assigned to TodoTask")
                    .ResponseResult();
            }

            todoTask.Labels.Add(label);
            await UnitOfWork.TodoTaskRepository.UpdateAsync(todoTask);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error assigning label to TodoTask with ID {TodoTaskId}", request.TodoTaskId);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while assigning the label")
                .ResponseResult();
        }
    }
}