using Microsoft.AspNetCore.Mvc;
using Content_App_POC.CommentsMgt;
using System;
using System.Threading.Tasks;

namespace Content_App_POC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpGet("content/{contentId}")]
        public async Task<IActionResult> GetByContentId(int contentId)
        {
            var comments = await _commentService.GetCommentsByContentIdAsync(contentId);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Comment comment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.AddCommentAsync(comment);
            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Comment comment)
        {
            if (id != comment.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] int newStatusId, [FromQuery] bool cascade = false)
        {
            // Update the status of the comment
            await _commentService.UpdateCommentStatusAsync(id, newStatusId);

            // If cascade is true, update all children
            if (cascade)
            {
                await _commentService.CascadeStatusToChildrenAsync(id, newStatusId);
            }

            return NoContent();
        }
    }
} 