using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.TodoTasks;
using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles operations for managing to-do tasks, including creation, retrieval, updates, status/priority changes, user assignments, and label management.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TodoTaskController : ControllerBase
{
    private readonly ITodoTaskRepository _todoTaskRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">The to-do task repository service for handling task operations.</param>
    public TodoTaskController(ITodoTaskRepository taskService)
    {
        _todoTaskRepository = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    /// <summary>
    /// Creates a new to-do task.
    /// </summary>
    /// <param name="request">The request containing details for the new task.</param>
    /// <returns>A response indicating the result of the create operation.</returns>
    /// <response code="201">Task created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateTodoTaskRequest request)
    {
        var response = await _todoTaskRepository.CreateAsync(request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a to-do task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task.</param>
    /// <returns>The task details if found.</returns>
    /// <response code="200">Task retrieved successfully.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _todoTaskRepository.GetByIdAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a paginated list of all to-do tasks.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of tasks per page (default is 10).</param>
    /// <returns>A paginated list of tasks.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var response = await _todoTaskRepository.GetAllAsync(page, pageSize);
        return response.ResponseResult();
    }

    /// <summary>
    /// Updates an existing to-do task.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task to update.</param>
    /// <param name="request">The updated task details.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">Task updated successfully.</response>
    /// <response code="400">Invalid request data or ID mismatch.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, UpdateTodoTaskRequest request)
    {
        var response = await _todoTaskRepository.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Changes the status of a to-do task.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task.</param>
    /// <param name="status">The new task status.</param>
    /// <returns>A response indicating the result of the status change operation.</returns>
    /// <response code="200">Task status updated successfully.</response>
    /// <response code="400">Invalid status provided.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] TodoTaskStatus status)
    {
        var response = await _todoTaskRepository.ChangeStatusAsync(id, status);
        return response.ResponseResult();
    }

    /// <summary>
    /// Changes the priority of a to-do task.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task.</param>
    /// <param name="priority">The new task priority.</param>
    /// <returns>A response indicating the result of the priority change operation.</returns>
    /// <response code="200">Task priority updated successfully.</response>
    /// <response code="400">Invalid priority provided.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id}/priority")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePriority(Guid id, [FromBody] PriorityStatus priority)
    {
        var response = await _todoTaskRepository.ChangePriorityAsync(id, priority);
        return response.ResponseResult();
    }

    /// <summary>
    /// Assigns a user to a to-do task.
    /// </summary>
    /// <param name="taskId">The unique identifier (GUID) of the task.</param>
    /// <param name="userId">The unique identifier (GUID) of the user to assign.</param>
    /// <returns>A response indicating the result of the assignment operation.</returns>
    /// <response code="200">User assigned to task successfully.</response>
    /// <response code="400">Invalid task or user ID.</response>
    /// <response code="404">Task or user not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("{taskId}/assign/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignUser(Guid taskId, Guid userId)
    {
        var response = await _todoTaskRepository.AssignUserAsync(taskId, userId);
        return response.ResponseResult();
    }

    /// <summary>
    /// Removes a user assignment from a to-do task.
    /// </summary>
    /// <param name="taskId">The unique identifier (GUID) of the task.</param>
    /// <param name="userId">The unique identifier (GUID) of the user to remove.</param>
    /// <returns>A response indicating the result of the removal operation.</returns>
    /// <response code="200">User removed from task successfully.</response>
    /// <response code="400">Invalid task or user ID.</response>
    /// <response code="404">Task or user not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{taskId}/assign/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAssignee(Guid taskId, Guid userId)
    {
        var response = await _todoTaskRepository.RemoveAssigneeAsync(taskId, userId);
        return response.ResponseResult();
    }

    /// <summary>
    /// Deletes a to-do task by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task to delete.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <response code="204">Task deleted successfully.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _todoTaskRepository.DeleteAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Adds a label to a to-do task.
    /// </summary>
    /// <param name="request">The request containing the task and label IDs.</param>
    /// <returns>A response indicating the result of the add label operation.</returns>
    /// <response code="200">Label added to task successfully.</response>
    /// <response code="400">Invalid task or label ID.</response>
    /// <response code="404">Task or label not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("add-label")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddLabel([FromBody] LabelTaskRequest request)
    {
        var response = await _todoTaskRepository.AddLabelAsync(request.TodoTaskId, request.LabelId);
        return response.ResponseResult();
    }

    /// <summary>
    /// Removes a label from a to-do task.
    /// </summary>
    /// <param name="request">The request containing the task and label IDs.</param>
    /// <returns>A response indicating the result of the remove label operation.</returns>
    /// <response code="200">Label removed from task successfully.</response>
    /// <response code="400">Invalid task or label ID.</response>
    /// <response code="404">Task or label not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("remove-label")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveLabel([FromBody] LabelTaskRequest request)
    {
        var response = await _todoTaskRepository.RemoveLabelAsync(request.TodoTaskId, request.LabelId);
        return response.ResponseResult();
    }
}