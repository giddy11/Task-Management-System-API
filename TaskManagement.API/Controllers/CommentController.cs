using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Comments;
using TaskManagement.Application.Comments.Dtos;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentService;

        public CommentController(ICommentRepository commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _commentService.GetByIdAsync(id);
            return response.ResponseResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            var response = await _commentService.CreateAsync(request);
            return response.ResponseResult();
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetAllForTask(Guid taskId)
        {
            var response = await _commentService.GetAllForTaskAsync(taskId);
            return response.ResponseResult();
        }
    }
}
