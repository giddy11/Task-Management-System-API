using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.Auth.Dtos;
using TaskManagement.Application.UserManagement;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Auth;

public class AuthRepository(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IConfiguration configuration,
    IMapper mapper) : IAuthRepository
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<OperationResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return OperationResponse<LoginResponse>.FailedResponse(StatusCode.Unauthorized)
                .AddError("Invalid credentials.");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return OperationResponse<LoginResponse>.FailedResponse(StatusCode.Unauthorized)
                .AddError("Invalid credentials.");
        }

        var token = GenerateJwtToken(user);

        var loginResponse = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            AccountType = user.AccountType
        };

        return OperationResponse<LoginResponse>.SuccessfulResponse(loginResponse);
    }

    public async Task<OperationResponse<CreateUserResponse>> RegisterAsync(CreateUserRequest request)
    {
        if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
        {
            return OperationResponse<CreateUserResponse>.FailedResponse(StatusCode.BadRequest)
                .AddError("User with this email already exists.");
        }

        var newUser = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserStatus = UserStatus.Active
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        var creationResponse = await _userRepository.CreateAsync(newUser);

        if (!creationResponse.IsSuccessful)
        {
            return OperationResponse<CreateUserResponse>.FailedResponse()
                .AddErrors(creationResponse.Errors);
        }

        return creationResponse;
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
        new Claim(ClaimTypes.Surname, user.LastName),
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
