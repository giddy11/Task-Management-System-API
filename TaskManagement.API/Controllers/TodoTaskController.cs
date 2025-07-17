using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.TodoTasks;
using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoTaskController : ControllerBase
{
    private readonly ITodoTaskRepository _todoTaskRepository;

    public TodoTaskController(ITodoTaskRepository taskService)
    {
        _todoTaskRepository = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoTaskRequest request)
    {
        var response = await _todoTaskRepository.CreateAsync(request);
        return response.ResponseResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _todoTaskRepository.GetByIdAsync(id);
        return response.ResponseResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var response = await _todoTaskRepository.GetAllAsync(page, pageSize);
        return response.ResponseResult();
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, UpdateTodoTaskRequest request)
    {
        var response = await _todoTaskRepository.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] TodoTaskStatus status)
    {
        var response = await _todoTaskRepository.ChangeStatusAsync(id, status);
        return response.ResponseResult();
    }

    [HttpPatch("{id}/priority")]
    public async Task<IActionResult> ChangePriority(Guid id, [FromBody] PriorityStatus priority)
    {
        var response = await _todoTaskRepository.ChangePriorityAsync(id, priority);
        return response.ResponseResult();
    }

    [HttpPost("{taskId}/assign/{userId}")]
    public async Task<IActionResult> AssignUser(Guid taskId, Guid userId)
    {
        var response = await _todoTaskRepository.AssignUserAsync(taskId, userId);
        return response.ResponseResult();
    }

    [HttpDelete("{taskId}/assign/{userId}")]
    public async Task<IActionResult> RemoveAssignee(Guid taskId, Guid userId)
    {
        var response = await _todoTaskRepository.RemoveAssigneeAsync(taskId, userId);
        return response.ResponseResult();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _todoTaskRepository.DeleteAsync(id);
        return response.ResponseResult();
    }

    [HttpPost("add-label")]
    public async Task<IActionResult> AddLabel([FromBody] LabelTaskRequest request)
    {
        var response = await _todoTaskRepository.AddLabelAsync(request.TodoTaskId, request.LabelId);
        return response.ResponseResult();
    }

    [HttpDelete("remove-label")]
    public async Task<IActionResult> RemoveLabel([FromBody] LabelTaskRequest request)
    {
        var response = await _todoTaskRepository.RemoveLabelAsync(request.TodoTaskId, request.LabelId);
        return response.ResponseResult();
    }
}
