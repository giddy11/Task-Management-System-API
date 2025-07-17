using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.TodoTasks;
using TaskManagement.Application.TodoTasks.Dtos;

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
}
