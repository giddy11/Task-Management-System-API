using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.Application.UserManagement
{
    public class UserRepository(TaskManagementDbContext context, IMapper mapper) : IUserRepository
    {
        private readonly TaskManagementDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationResponse<CreateUserResponse>> CreateAsync(CreateUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return OperationResponse<CreateUserResponse>.FailedResponse()
                    .AddError("User with this email already exists");
            }

            var user = User.New(request.Email, request.FirstName, request.LastName);
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

        public async Task<OperationResponse<GetUserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return OperationResponse<GetUserResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("User not found");
            }

            user.Update(request.FirstName, request.LastName, request.Email);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetUserResponse>(user);
            return OperationResponse<GetUserResponse>.SuccessfulResponse(mapped);
        }
    }
}
