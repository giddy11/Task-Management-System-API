using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.Auth.Dtos;
using TaskManagement.Application.Features.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles authentication operations such as user login and registration.
/// </summary>
[Route("auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public AuthController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AuthController> logger,
        IPasswordHasher<User> passwordHasher,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
        : base(unitOfWork, mapper, logger)
    {
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>An IActionResult containing the login response with a JWT token.</returns>
    /// <remarks>
    /// This endpoint authenticates a user and generates a JWT token upon successful login.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

            var user = await UnitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid credentials.")
                    .ResponseResult();
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid credentials.")
                    .ResponseResult();
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                AccountType = user.AccountType
            };

            return OperationResponse<LoginResponse>.SuccessfulResponse(loginResponse)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during user login for email {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while processing the login request")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The user registration request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new user account.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserCreateRequest request)
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
                    .AddError($"User with email {request.Email} already exists.")
                    .ResponseResult();
            }

            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserStatus = UserStatus.Active
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            await UnitOfWork.UserRepository.AddAsync(newUser);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var userResponse = Mapper.Map<UserCreateResponse>(newUser);
            return OperationResponse<UserCreateResponse>.CreatedResponse(userResponse)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error registering user with email {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while registering the user")
                .ResponseResult();
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured.");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured.");
        var tokenExpirationMinutes = Convert.ToDouble(_configuration["Jwt:TokenExpirationMinutes"] ?? "120");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.AccountType.ToString()),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var token = new JwtSecurityToken(
            jwtIssuer,
            jwtAudience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}