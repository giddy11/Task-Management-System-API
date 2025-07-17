using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Labels;
using TaskManagement.Application.Labels.Dtos;

namespace TaskManagement.API.Controllers;

/// <summary>
/// Handles operations for managing labels, including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class LabelController : ControllerBase
{
    private readonly ILabelRepository _labelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelController"/> class.
    /// </summary>
    /// <param name="labelService">The label repository service for handling label operations.</param>
    public LabelController(ILabelRepository labelService)
    {
        _labelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
    }

    /// <summary>
    /// Retrieves a label by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the label.</param>
    /// <returns>The label details if found.</returns>
    /// <response code="200">Label retrieved successfully.</response>
    /// <response code="404">Label not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _labelService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    /// <summary>
    /// Creates a new label.
    /// </summary>
    /// <param name="request">The request containing details for the new label.</param>
    /// <returns>A response indicating the result of the create operation.</returns>
    /// <response code="201">Label created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateLabelRequest request)
    {
        var response = await _labelService.CreateAsync(request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Retrieves all labels.
    /// </summary>
    /// <returns>A list of all labels.</returns>
    /// <response code="200">Labels retrieved successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _labelService.GetAllAsync();
        return response.ResponseResult();
    }

    /// <summary>
    /// Updates an existing label.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the label to update.</param>
    /// <param name="request">The updated label details.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">Label updated successfully.</response>
    /// <response code="400">Invalid request data or ID mismatch.</response>
    /// <response code="404">Label not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLabelRequest request)
    {
        var response = await _labelService.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    /// <summary>
    /// Deletes a label by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the label to delete.</param>
    /// <returns>A response indicating the result of the delete operation.</returns>
    /// <response code="204">Label deleted successfully.</response>
    /// <response code="404">Label not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _labelService.DeleteAsync(id);
        return response.ResponseResult();
    }
}