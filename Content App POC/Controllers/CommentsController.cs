using Microsoft.AspNetCore.Mvc;
using Content_App_POC.CommentsMgt;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Content_App_POC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IConfiguration _configuration;

        public CommentsController(ICommentService commentService, IConfiguration configuration)
        {
            _commentService = commentService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
        {
            if (id != comment.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get user name for audit (if available)
            var userName = User?.Identity?.Name ?? "Administrator";
            await _commentService.SetDeletedRecursiveAsync(id, userName);
            return NoContent();
        }

        [HttpPut("{id}/approval")]
        public async Task<IActionResult> SetApproval(int id, [FromBody] ApprovalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.SetApprovalRecursiveAsync(id, dto.IsApproved, dto.ModifiedBy);
            return NoContent();
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            var config = new
            {
                CommentsAdminRole = _configuration["CommentsConfig:CommentsAdminRole"],
                CommentsViewerRole = _configuration["CommentsConfig:CommentsViewerRole"],
                CMSDisplayToggle = _configuration["CommentsConfig:CMSDisplayToggle"],
                PortalDisplayToggle = _configuration["CommentsConfig:PortalDisplayToggle"],
                AdminCanAddComment = _configuration["CommentsConfig:AdminCanAddComment"],
                CommentsAutoApproved = _configuration.GetValue<bool>("CommentsConfig:CommentsAutoApproved", false)
            };
            return Ok(config);
        }

        public class ApprovalDto
        {
            public bool IsApproved { get; set; }
            public string ModifiedBy { get; set; } = string.Empty;
        }
    }
} 