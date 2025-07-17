using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Comments;
using TaskManagement.Application.Comments.Dtos;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles operations for managing comments, including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The comment repository service for handling comment operations.</param>
    public CommentController(ICommentRepository commentService)
    {
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
    }

    /// <summary>
    /// Retrieves a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the comment.</param>
    /// <returns>The comment details if found.</returns>
    /// <response code="200">Comment retrieved successfully.</response>
    /// <response code="404">Comment not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _commentService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="request">The request containing details for the new comment.</param>
    /// <returns>A response indicating the result of the create operation.</returns>
    /// <response code="201">Comment created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
    {
        var response = await _commentService.CreateAsync(request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves all comments for a specific to-do task.
    /// </summary>
    /// <param name="taskId">The unique identifier (GUID) of the task.</param>
    /// <returns>A list of comments associated with the task.</returns>
    /// <response code="200">Comments retrieved successfully.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("task/{taskId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForTask(Guid taskId)
    {
        var response = await _commentService.GetAllForTaskAsync(taskId);
        return response.ResponseResult();
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the comment to update.</param>
    /// <param name="request">The updated comment details.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">Comment updated successfully.</response>
    /// <response code="400">Invalid request data or ID mismatch.</response>
    /// <response code="404">Comment not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequest request)
    {
        var response = await _commentService.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Deletes a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the comment to delete.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <response code="204">Comment deleted successfully.</response>
    /// <response code="404">Comment not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _commentService.DeleteAsync(id);
        return response.ResponseResult();
    }
}