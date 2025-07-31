using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.Labels.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles label management operations such as creating, retrieving, updating, and deleting labels.
/// </summary>
[Route("label")]
[Authorize(Roles = "Admin")]
public class LabelController : BaseController
{
    public LabelController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LabelController> logger) : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// Creates a new label.
    /// </summary>
    /// <param name="request">The label creation request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new label.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(LabelCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] LabelCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddErrors(errors)
                    .ResponseResult();
            }

            var userExists = await UnitOfWork.UserRepository.GetByIdAsync(request.CreatedById);
            if (userExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {request.CreatedById} not found")
                    .ResponseResult();
            }

            var existingLabel = await UnitOfWork.LabelRepository.GetByNameAndCreatorAsync(request.Name, request.CreatedById);
            if (existingLabel != null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.Conflict)
                    .AddError($"Label with name '{request.Name}' already exists for user {request.CreatedById}")
                    .ResponseResult();
            }

            var label = Mapper.Map<Label>(request);
            await UnitOfWork.LabelRepository.AddAsync(label);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var labelDto = Mapper.Map<LabelCreateResponse>(label);
            return OperationResponse<LabelCreateResponse>.CreatedResponse(labelDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating label");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while creating the label")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves a label by ID.
    /// </summary>
    /// <param name="id">The ID of the label to retrieve.</param>
    /// <returns>An IActionResult containing the label details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LabelFetchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLabelById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddError("Invalid label ID")
                    .ResponseResult();
            }

            var label = await UnitOfWork.LabelRepository.GetByIdAsync(id);
            if (label == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Label with ID {id} not found")
                    .ResponseResult();
            }

            var labelDto = Mapper.Map<LabelFetchResponse>(label);
            return OperationResponse<LabelFetchResponse>.SuccessfulResponse(labelDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving label with ID {LabelId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving the label")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Retrieves all labels.
    /// </summary>
    /// <returns>An IActionResult containing a list of all labels.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LabelFetchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var labels = await UnitOfWork.LabelRepository.GetAllAsync();
            if (labels == null || !labels.Any())
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError("No labels found")
                    .ResponseResult();
            }

            var labelDtos = Mapper.Map<IEnumerable<LabelFetchResponse>>(labels);
            return OperationResponse<IEnumerable<LabelFetchResponse>>.SuccessfulResponse(labelDtos)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving labels");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while retrieving labels")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Updates an existing label.
    /// </summary>
    /// <param name="id">The ID of the label to update.</param>
    /// <param name="request">The label update request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LabelUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return OperationResponse.FailedResponse(Application.Utils.StatusCode.BadRequest)
                    .AddErrors(errors)
                    .ResponseResult();
            }

            var existingLabel = await UnitOfWork.LabelRepository.GetByIdAsync(id);
            if (existingLabel == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Label with ID {id} not found")
                    .ResponseResult();
            }

            if (existingLabel.Name != request.Name)
            {
                var labelWithName = await UnitOfWork.LabelRepository.GetByNameAndCreatorAsync(request.Name, existingLabel.CreatedById);
                if (labelWithName != null && labelWithName.Id != id)
                {
                    return OperationResponse.FailedResponse(Application.Utils.StatusCode.Conflict)
                        .AddError($"Label with name '{request.Name}' already exists for user {existingLabel.CreatedById}")
                        .ResponseResult();
                }
            }

            Mapper.Map(request, existingLabel);
            await UnitOfWork.LabelRepository.UpdateAsync(existingLabel);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating label with ID {LabelId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while updating the label")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Deletes a label by ID.
    /// </summary>
    /// <param name="id">The ID of the label to delete.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var label = await UnitOfWork.LabelRepository.GetByIdAsync(id);
            if (label == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Label with ID {id} not found")
                    .ResponseResult();
            }

            await UnitOfWork.LabelRepository.DeleteAsync(label);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            return OperationResponse.SuccessfulResponse()
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting label with ID {LabelId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while deleting the label")
                .ResponseResult();
        }
    }
}