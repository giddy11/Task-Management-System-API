using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Auth;
using TaskManagement.Application.Auth.Dtos;
using TaskManagement.Application.UserManagement;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository; // Inject the new service

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials (email, password).</param>
    /// <returns>A JWT token if authentication is successful.</returns>
    /// <response code="200">Authentication successful, returns token.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authRepository.LoginAsync(request);
        return response.ResponseResult(); // Use your extension method
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">User registration details.</param>
    /// <returns>Confirmation of user registration.</returns>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Invalid input or email already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        var response = await _authRepository.RegisterAsync(request);
        return response.ResponseResult();
    }
}