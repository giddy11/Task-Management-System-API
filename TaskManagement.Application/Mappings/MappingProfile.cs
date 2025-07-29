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
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            //.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //.ForMember(dest => dest.FirstName, opt => opt.Ignore())
            //.ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTasks, opt => opt.Ignore())
            .ForMember(dest => dest.AccountType, opt => opt.Ignore())
            .ForMember(dest => dest.UserStatus, opt => opt.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType))
            .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.UserStatus))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTasks, opt => opt.Ignore());

        CreateMap<User, GetUserResponse>();
        CreateMap<User, CreateUserResponse>();

        CreateMap<Project, CreateProjectResponse>();
        CreateMap<Project, GetProjectResponse>();

        CreateMap<TodoTask, CreateTodoTaskResponse>();
        CreateMap<User, UserDto>(); 
        CreateMap<TodoTask, GetTodoTaskResponse>()
            .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Labels))
            .ForMember(dest => dest.Assignees, opt => opt.MapFrom(src => src.Assignees))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

        // Comment
        CreateMap<CreateCommentRequest, Comment>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.TodoTaskId, opt => opt.MapFrom(src => src.TaskId))
            //.ForAllOtherMembers(opt => opt.Ignore());
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTask, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdateCommentRequest, Comment>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            //.ForAllOtherMembers(opt => opt.Ignore());
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTask, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Label, GetLabelResponse>();
        CreateMap<Label, CreateLabelResponse>();
        CreateMap<Label, LabelDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

        CreateMap<Comment, GetCommentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<Comment, CreateCommentResponse>();
    }
}
