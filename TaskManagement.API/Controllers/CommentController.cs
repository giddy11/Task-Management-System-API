using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Comments;
using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Domain;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles operations for managing comments, including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _commentService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The comment repository service for handling comment operations.</param>
    /// <param name="mapper"></param>
    public CommentController(ICommentRepository commentService, IMapper mapper)
    {
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        var comment = _mapper.Map<Comment>(request);
        var response = await _commentService.CreateAsync(comment);
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
        var comment = _mapper.Map<Comment>(request);
        var response = await _commentService.UpdateAsync(id, comment);
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