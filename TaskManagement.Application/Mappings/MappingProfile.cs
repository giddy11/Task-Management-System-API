using AutoMapper;
using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Labels.Dtos;
using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Domain;
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
        CreateMap<TodoTask, GetTodoTaskResponse>();
        CreateMap<User, UserDto>(); 

        CreateMap<TodoTask, GetTodoTaskResponse>()
            //.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Assignees, opt => opt.MapFrom(src => src.Assignees))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

        CreateMap<Label, GetLabelResponse>();
        CreateMap<Label, CreateLabelResponse>();

        CreateMap<Comment, GetCommentResponse>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

        CreateMap<Comment, CreateCommentResponse>();
    }
}
