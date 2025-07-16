using AutoMapper;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, GetUserResponse>();
        CreateMap<User, CreateUserResponse>();
    }
}
