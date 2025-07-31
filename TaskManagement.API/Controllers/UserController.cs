using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles user management operations such as creating, retrieving, updating, deleting, and changing the status of users.
/// </summary>
[Route("user")]
//[Authorize]
public class UserController : BaseController
{
    public UserController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserController> logger) : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new user.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(UserCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
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

            // Check if user with email already exists
            var existingUser = await UnitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Conflict)
                    .AddError($"User with email {request.Email} already exists")
                    .ResponseResult();
            }

            var user = Mapper.Map<User>(request);
            // Assume password hashing is handled in the mapping or repository
            await UnitOfWork.UserRepository.AddAsync(user);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var userDto = Mapper.Map<UserCreateResponse>(user);
            return OperationResponse<UserCreateResponse>.CreatedResponse(userDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating user");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while creating the user")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves a user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>An IActionResult containing the user details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserFetchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid user ID")
                    .ResponseResult();
            }

            var user = await UnitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {id} not found")
                    .ResponseResult();
            }

            var userDto = Mapper.Map<UserFetchResponse>(user);
            return OperationResponse<UserFetchResponse>.SuccessfulResponse(userDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving the user")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>An IActionResult containing a list of all users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserFetchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await UnitOfWork.UserRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError("No users found")
                    .ResponseResult();
            }

            var userDtos = Mapper.Map<IEnumerable<UserFetchResponse>>(users);
            return OperationResponse<IEnumerable<UserFetchResponse>>.SuccessfulResponse(userDtos)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving users");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving users")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="request">The user update request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
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

            var existingUser = await UnitOfWork.UserRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {id} not found")
                    .ResponseResult();
            }

            // Check if email is being changed and if it already exists
            if (existingUser.Email != request.Email)
            {
                var userWithEmail = await UnitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
                if (userWithEmail != null && userWithEmail.Id != id)
                {
                    return OperationResponse.FailedResponse(Application.Utils.StatusCode.Conflict)
                        .AddError($"Email {request.Email} is already in use")
                        .ResponseResult();
                }
            }

            Mapper.Map(request, existingUser);
            await UnitOfWork.UserRepository.UpdateAsync(existingUser);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while updating the user")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await UnitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {id} not found")
                    .ResponseResult();
            }

            await UnitOfWork.UserRepository.DeleteAsync(user);
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
            Logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while deleting the user")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Changes the status of a user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="status">The new user status.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] UserStatus status)
    {
        try
        {
            if (!Enum.IsDefined(typeof(UserStatus), status))
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid user status provided")
                    .ResponseResult();
            }

            var user = await UnitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {id} not found")
                    .ResponseResult();
            }

            user.UserStatus = status;
            await UnitOfWork.UserRepository.UpdateAsync(user);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing status for user with ID {UserId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while changing user status")
                .ResponseResult();
        }
    }
}