using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Infrastructure;
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
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AuthController> logger,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        IEmailService emailService)
        : base(unitOfWork, mapper, logger)
    {
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _emailService = emailService;
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

            if (user.UserStatus != UserStatus.Active)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Email not verified. Please verify your email to log in.")
                    .ResponseResult();
            }

            if (user.UserStatus == UserStatus.Suspended)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Sorry you have been suspended. Please contact admin")
                    .ResponseResult();
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid credentials.")
                    .ResponseResult();
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await UnitOfWork.UserRepository.AddRefreshTokenAsync(refreshTokenEntity);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var loginResponse = new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
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
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request containing the refresh token.</param>
    /// <returns>An IActionResult containing a new access token and refresh token.</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
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

            var refreshTokenEntity = await UnitOfWork.UserRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked || refreshTokenEntity.ExpiryDate < DateTime.UtcNow)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid or expired refresh token.")
                    .ResponseResult();
            }

            var user = await UnitOfWork.UserRepository.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null || user.UserStatus != UserStatus.Active)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("User not found or email not verified.")
                    .ResponseResult();
            }

            // Revoke the used refresh token
            refreshTokenEntity.IsRevoked = true;
            await UnitOfWork.UserRepository.UpdateRefreshTokenAsync(refreshTokenEntity);

            // Generate new access and refresh tokens
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(2),
                IsRevoked = false
            };

            await UnitOfWork.UserRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var response = new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return OperationResponse<RefreshTokenResponse>.SuccessfulResponse(response)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing token");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while refreshing the token")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Registers a new user and sends a verification code to their email.
    /// </summary>
    /// <param name="request">The user registration request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
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
                UserStatus = UserStatus.Inactive, // Pending until email is verified
                PasswordHash = _passwordHasher.HashPassword(null!, request.Password),
                VerificationCode = GenerateVerificationCode(),
                VerificationCodeExpiry = DateTime.UtcNow.AddHours(24) // Code expires in 24 hours
            };

            await UnitOfWork.UserRepository.AddAsync(newUser);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            // Send verification code via email
            var emailBody = $"Your verification code is: {newUser.VerificationCode}<br/><br/>" +
                            $"Please use this code to verify your email address.";
            await _emailService.SendEmailAsync(request.Email, "Verify Your Email", emailBody);

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

    /// <summary>
    /// Resends a verification code to the user's email.
    /// </summary>
    /// <param name="request">The resend verification code request containing the email.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost("resend-verification-code")]
    [ProducesResponseType(typeof(ResendVerificationCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationCodeRequest request)
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
            if (user == null || user.UserStatus == UserStatus.Active)
            {
                // Return generic response to prevent enumeration and avoid resending for verified users
                return OperationResponse<ResendVerificationCodeResponse>.SuccessfulResponse(
                    new ResendVerificationCodeResponse())
                    .ResponseResult();
            }

            // Generate and store new verification code
            user.VerificationCode = GenerateVerificationCode();
            user.VerificationCodeExpiry = DateTime.UtcNow.AddHours(24);

            await UnitOfWork.UserRepository.UpdateAsync(user);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            // Send new verification code via email
            var emailBody = $"Your new verification code is: {user.VerificationCode}<br/><br/>" +
                            $"Please use this code to verify your email address.";
            await _emailService.SendEmailAsync(request.Email, "Verify Your Email", emailBody);

            return OperationResponse<ResendVerificationCodeResponse>.SuccessfulResponse(
                new ResendVerificationCodeResponse())
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resending verification code for email {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while resending the verification code")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Verifies a user’s email using a verification code.
    /// </summary>
    /// <param name="request">The verify email request containing the email and code.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
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

            var user = await UnitOfWork.UserRepository.GetByEmailAndVerificationCodeAsync(request.Email, request.Code);
            if (user == null || user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid or expired verification code.")
                    .ResponseResult();
            }

            if (user.UserStatus == UserStatus.Active)
            {
                return OperationResponse<VerifyEmailResponse>.SuccessfulResponse(
                    new VerifyEmailResponse { Message = "Email already verified." })
                    .ResponseResult();
            }

            user.UserStatus = UserStatus.Active;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            await UnitOfWork.UserRepository.UpdateAsync(user);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            return OperationResponse<VerifyEmailResponse>.SuccessfulResponse(
                new VerifyEmailResponse())
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error verifying email for {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while verifying the email")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Changes a user's password using a reset token.
    /// </summary>
    /// <param name="request">The change password request containing the email, token, and new password.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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

            var user = await UnitOfWork.UserRepository.GetByEmailAndResetTokenAsync(request.Email, request.Token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Unauthorized)
                    .AddError("Invalid or expired password reset token.")
                    .ResponseResult();
            }

            // Update password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await UnitOfWork.UserRepository.UpdateAsync(user);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            return OperationResponse<ChangePasswordResponse>.SuccessfulResponse(
                new ChangePasswordResponse())
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing password for email {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while changing the password")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Generates and sends a new random password to the user's email.
    /// </summary>
    /// <param name="request">The forgot password request containing the user's email.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Conflict)
                    .AddError($"User with email {request.Email} doesn't exist.")
                    .ResponseResult();
            }

            var user = await UnitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return OperationResponse<ForgotPasswordResponse>.SuccessfulResponse(
                    new ForgotPasswordResponse())
                    .ResponseResult();
            }

            var newPassword = GenerateRandomPassword(16);
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            await UnitOfWork.UserRepository.UpdateAsync(user);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var emailBody = $"Your new password is: {newPassword}<br/><br/>" +
                            $"Please log in with this password and consider changing it in your account settings.";
            await _emailService.SendEmailAsync(request.Email, "Your New Password", emailBody);

            return OperationResponse<ForgotPasswordResponse>.SuccessfulResponse(
                new ForgotPasswordResponse())
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing forgot password request for email {Email}", request.Email);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while processing the forgot password request")
                .ResponseResult();
        }
    }

    private string GenerateVerificationCode()
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[6];
        RandomNumberGenerator.Fill(bytes);

        var code = new char[6];
        for (int i = 0; i < 6; i++)
        {
            code[i] = validChars[bytes[i] % validChars.Length];
        }

        return new string(code);
    }

    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string GenerateRandomPassword(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        var password = new char[length];
        for (int i = 0; i < length; i++)
        {
            password[i] = validChars[bytes[i] % validChars.Length];
        }

        return new string(password);
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