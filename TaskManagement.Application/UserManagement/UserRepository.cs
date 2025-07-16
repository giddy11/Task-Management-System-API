using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.Application.UserManagement
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(TaskManagementDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

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
    }
}
