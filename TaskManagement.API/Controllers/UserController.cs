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
}
