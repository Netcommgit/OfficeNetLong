using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Entities;
using OfficeNet.Service.DiscussionForum;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionForumController : ControllerBase
    {
        private readonly IDiscussionForumService _discussionForumService;
        public DiscussionForumController(IDiscussionForumService discussionForumService)
        {
            _discussionForumService = discussionForumService;
        }



        [HttpPost("SaveDiscussionTopic")]
        [Authorize(Roles = "Admin,User")]
        //[Authorize]
        public async Task<IActionResult> SaveDiscussionTopic(DiscussionTopic discussionTopic)
        {
            if (discussionTopic == null)
            {
                return BadRequest("Discussion topic cannot be null.");
            }
            try
            {
                var result = await _discussionForumService.SaveDiscussionTopic(discussionTopic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving discussion topic: {ex.Message}");
            }
        }
    }
}
