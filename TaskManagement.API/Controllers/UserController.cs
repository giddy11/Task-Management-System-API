using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.UserManagement;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Domain;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles user management operations such as creating, retrieving, updating, and deleting users.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles ="Admin")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user repository service for handling user operations.</param>
    /// <param name="mapper"></param>
    public UserController(IUserRepository userService, IMapper mapper)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user creation request containing user details.</param>
    /// <returns>A response indicating the result of the create operation.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = _mapper.Map<User>(request);
        var response = await _userService.CreateAsync(user);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user.</param>
    /// <returns>The user details if found.</returns>
    /// <response code="200">User retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _userService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of users per page (default is 10).</param>
    /// <returns>A paginated list of users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var response = await _userService.GetAllAsync(page, pageSize);
        return response.ResponseResult();
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="request">The updated user details.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Invalid request data or ID mismatch.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var user = _mapper.Map<User>(request);
        var response = await _userService.UpdateAsync(user);
        return response.ResponseResult();
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user to delete.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _userService.DeleteAsync(id);
        return response.ResponseResult();
    }
}