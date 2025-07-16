using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.UserManagement;
using TaskManagement.Application.UserManagement.Dtos;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserRepository userService) : ControllerBase
{
    private readonly IUserRepository _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var response = await _userService.CreateAsync(request);
        return response.ResponseResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _userService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var response = await _userService.GetAllAsync(page, pageSize);
        return response.ResponseResult();
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var response = await _userService.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _userService.DeleteAsync(id);
        return response.ResponseResult();
    }
}
