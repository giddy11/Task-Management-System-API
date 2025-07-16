using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.Projects;
using TaskManagement.Persistence;

namespace TaskManagement.Application.Projects;

public class ProjectRepository(TaskManagementDbContext context, IMapper mapper) : IProjectRepository
{
    private readonly TaskManagementDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<OperationResponse<CreateProjectResponse>> CreateAsync(CreateProjectRequest request)
    {
        var creatorExists = await _context.Users.AnyAsync(u => u.Id == request.CreatedById);
        if (!creatorExists)
        {
            return OperationResponse<CreateProjectResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        var project = Project.New(
            request.Title,
            request.Description,
            request.CreatedById,
            request.StartDate,
            request.EndDate);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<CreateProjectResponse>(project);
        return OperationResponse<CreateProjectResponse>.SuccessfulResponse(response);
    }
}
