using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Service.HelpdeskDetails;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpdeskDetailsController : ControllerBase
    {
        private readonly IHelpdeskDetailService _helpdeskDetailService;
        public HelpdeskDetailsController(IHelpdeskDetailService helpdeskDetailService)
        {
            _helpdeskDetailService = helpdeskDetailService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateHelpdeskDetails([FromBody] HelpDeskDetailDto helpDeskDetailDto)
        {
            if (helpDeskDetailDto == null)
            {
                return BadRequest("Helpdesk details cannot be null.");
            }
            var result = await _helpdeskDetailService.CreateHelpdeskDetails(helpDeskDetailDto);
            if (result)
            {
                return Ok("Helpdesk details created successfully.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating helpdesk details.");
            }
        }
    }
}
