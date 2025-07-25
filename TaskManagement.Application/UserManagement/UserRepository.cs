using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.Application.UserManagement;

public class UserRepository(TaskManagementDbContext context, IMapper mapper) : IUserRepository
{
    private readonly TaskManagementDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<OperationResponse<CreateUserResponse>> CreateAsync(User request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return OperationResponse<CreateUserResponse>.FailedResponse()
                .AddError("User with this email already exists");
        }

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<CreateUserResponse>(user);
        return OperationResponse<CreateUserResponse>.SuccessfulResponse(response);
    }

    public async Task<OperationResponse<List<GetUserResponse>>> GetAllAsync(int page, int pageSize)
    {
        var users = await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<List<GetUserResponse>>(users);
        return OperationResponse<List<GetUserResponse>>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<GetUserResponse>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return OperationResponse<GetUserResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        var mapped = _mapper.Map<GetUserResponse>(user);
        return OperationResponse<GetUserResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse> UpdateAsync(Guid id, User request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return OperationResponse<GetUserResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var mapped = _mapper.Map<GetUserResponse>(user);
        return OperationResponse.SuccessfulResponse();
    }

    public async Task<OperationResponse<string>> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return OperationResponse<string>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return OperationResponse<string>.SuccessfulResponse("User deleted successfully");
    }
}