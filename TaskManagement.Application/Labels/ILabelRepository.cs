using TaskManagement.Application.Labels.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Labels;

public interface ILabelRepository
{
    Task<OperationResponse<GetLabelResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<CreateLabelResponse>> CreateAsync(CreateLabelRequest request);
    Task<OperationResponse<List<GetLabelResponse>>> GetAllAsync();
}
