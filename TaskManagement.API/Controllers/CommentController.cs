using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Features.Comments.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles comment management operations such as creating, retrieving, updating, and deleting comments.
/// </summary>
[Route("comment")]
[Authorize]
public class CommentController : BaseController
{
    public CommentController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CommentController> logger) : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="request">The comment creation request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    /// <remarks>
    /// This endpoint creates a new comment for a specific task.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CommentCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
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

            // Check if user exists
            var userExists = await UnitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (userExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"User with ID {request.UserId} not found")
                    .ResponseResult();
            }

            // Check if task exists
            var taskExists = await UnitOfWork.TodoTaskRepository.GetByIdAsync(request.TaskId);
            if (taskExists == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Task with ID {request.TaskId} not found")
                    .ResponseResult();
            }

            var comment = Mapper.Map<Comment>(request);
            await UnitOfWork.CommentRepository.AddAsync(comment);
            var saveResult = await UnitOfWork.SaveChangesAsync();

            if (!saveResult.IsSuccessful)
            {
                return saveResult.ResponseResult();
            }

            var commentDto = Mapper.Map<CommentCreateResponse>(comment);
            return OperationResponse<CommentCreateResponse>.CreatedResponse(commentDto)
                .ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating comment");
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while creating the comment")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">The ID of the comment to update.</param>
    /// <param name="request">The comment update request.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CommentUpdateRequest request)
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

            var existingComment = await UnitOfWork.CommentRepository.GetByIdAsync(id);
            if (existingComment == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Comment with ID {id} not found")
                    .ResponseResult();
            }

            Mapper.Map(request, existingComment);
            await UnitOfWork.CommentRepository.UpdateAsync(existingComment);

            var saveResult = await UnitOfWork.SaveChangesAsync();
            return saveResult.ResponseResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating comment with ID {CommentId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while updating the comment")
                .ResponseResult();
        }
    }

    /// <summary>
    /// Deletes a comment by ID.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var comment = await UnitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return OperationResponse.FailedResponse(Application.Utils.StatusCode.NotFound)
                    .AddError($"Comment with ID {id} not found")
                    .ResponseResult();
            }

            await UnitOfWork.CommentRepository.DeleteAsync(comment);
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
            Logger.LogError(ex, "Error deleting comment with ID {CommentId}", id);
            return OperationResponse.FailedResponse(Application.Utils.StatusCode.InternalServerError)
                .AddError("An error occurred while deleting the comment")
                .ResponseResult();
        }
    }
}