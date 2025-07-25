using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Labels.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;
using TaskManagement.Persistence;

namespace TaskManagement.Application.Labels;

public class LabelRepository : ILabelRepository
{
    private readonly TaskManagementDbContext _context;
    private readonly IMapper _mapper;

    public LabelRepository(TaskManagementDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OperationResponse<CreateLabelResponse>> CreateAsync(CreateLabelRequest request)
    {
        var creatorExists = await _context.Users.AnyAsync(u => u.Id == request.CreatedById);
        if (!creatorExists)
        {
            return OperationResponse<CreateLabelResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Color = request.Color,
            CreatedById = request.CreatedById,
        };

        await _context.Labels.AddAsync(label);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<CreateLabelResponse>(label);
        return OperationResponse<CreateLabelResponse>.SuccessfulResponse(response);
    }

    public async Task<OperationResponse<List<GetLabelResponse>>> GetAllAsync()
    {
        var labels = await _context.Labels
            .Include(l => l.CreatedBy)
            .ToListAsync();

        var mapped = _mapper.Map<List<GetLabelResponse>>(labels);
        return OperationResponse<List<GetLabelResponse>>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<GetLabelResponse>> GetByIdAsync(Guid id)
    {
        var label = await _context.Labels
            .Include(l => l.CreatedBy)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (label is null)
        {
            return OperationResponse<GetLabelResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Label not found");
        }

        var mapped = _mapper.Map<GetLabelResponse>(label);
        return OperationResponse<GetLabelResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<GetLabelResponse>> UpdateAsync(Guid id, UpdateLabelRequest request)
    {
        var label = await _context.Labels.FindAsync(id);
        if (label is null)
        {
            return OperationResponse<GetLabelResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Label not found");
        }

        label.Name = request.Name;
        label.Color = request.Color;

        _context.Labels.Update(label);
        await _context.SaveChangesAsync();

        var updatedLabel = await _context.Labels
            .Include(l => l.CreatedBy)
            .FirstOrDefaultAsync(l => l.Id == id);

        var mapped = _mapper.Map<GetLabelResponse>(updatedLabel);
        return OperationResponse<GetLabelResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse<string>> DeleteAsync(Guid id)
    {
        var label = await _context.Labels.FindAsync(id);
        if (label is null)
        {
            return OperationResponse<string>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Label not found");
        }

        _context.Labels.Remove(label);
        await _context.SaveChangesAsync();
        return OperationResponse<string>.SuccessfulResponse("Label deleted successfully");
    }
}