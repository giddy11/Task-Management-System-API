using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Labels;
using TaskManagement.Application.Labels.Dtos;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LabelController : ControllerBase
{
    private readonly ILabelRepository _labelService;

    public LabelController(ILabelRepository labelService)
    {
        _labelService = labelService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _labelService.GetByIdAsync(id);
        return response.ResponseResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLabelRequest request)
    {
        var response = await _labelService.CreateAsync(request);
        return response.ResponseResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _labelService.GetAllAsync();
        return response.ResponseResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLabelRequest request)
    {
        var response = await _labelService.UpdateAsync(id, request);
        return response.ResponseResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _labelService.DeleteAsync(id);
        return response.ResponseResult();
    }
}
