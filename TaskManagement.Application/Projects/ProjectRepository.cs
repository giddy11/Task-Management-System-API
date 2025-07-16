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

    public async Task<OperationResponse<List<GetProjectResponse>>> GetAllAsync(int page, int pageSize)
    {
        var projects = await _context.Projects
            .Include(p => p.CreatedBy)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<List<GetProjectResponse>>(projects);
        return OperationResponse<List<GetProjectResponse>>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<GetProjectResponse>> GetByIdAsync(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
        {
            return OperationResponse<GetProjectResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Project not found");
        }

        var mapped = _mapper.Map<GetProjectResponse>(project);
        return OperationResponse<GetProjectResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<GetProjectResponse>> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
        {
            return OperationResponse<GetProjectResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Project not found");
        }

        project.Update(request.Title, request.Description, request.StartDate, request.EndDate);
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        var mapped = _mapper.Map<GetProjectResponse>(project);
        return OperationResponse<GetProjectResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<string>> DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
        {
            return OperationResponse<string>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Project not found");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return OperationResponse<string>.SuccessfulResponse("Project deleted successfully");
    }

    public async Task<OperationResponse<GetProjectResponse>> ChangeStatusAsync(Guid id, ProjectStatus status)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
        {
            return OperationResponse<GetProjectResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Project not found");
        }

        project.ChangeStatus(status);
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        var mapped = _mapper.Map<GetProjectResponse>(project);
        return OperationResponse<GetProjectResponse>.SuccessfulResponse(mapped);
    }
}
