using AutoMapper;
using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, GetUserResponse>();
        CreateMap<User, CreateUserResponse>();

        CreateMap<Project, CreateProjectResponse>();
        CreateMap<Project, GetProjectResponse>();

        CreateMap<TodoTask, CreateTodoTaskResponse>();
    }
}
