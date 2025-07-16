using AutoMapper;

namespace TaskManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, GetUserResponse>();
        CreateMap<User, CreateUserResponse>();
    }
}
