using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Service.HelpdeskDepartment;
using OfficeNet.Service.OpinionPoll;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpdeskDepartmentController : ControllerBase
    {
        private readonly IHelpdeskDepartmentService _helpdeskDepartmentService;
        public HelpdeskDepartmentController(IHelpdeskDepartmentService helpdeskDepartmentService)
        {
            _helpdeskDepartmentService = helpdeskDepartmentService;
        }
        [HttpPost("GetHelpDeskDepartmentList")]
        [Authorize]
        public async Task<IActionResult> GetHelpDeskDepartment(PaginationRequest request)
        {
            try
            {
                var result = await _helpdeskDepartmentService.GetHelpdeskAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error occurred while fetching helpdesk departments: {ex.Message}" });
            }
        }

        [HttpPost("CreateHelpDeskDepartment")]
        [Authorize]
        public async Task<IActionResult> CreateHelpDeskDepartment(HelpdeskDepartmentDto helpdeskDepartment)
        {
            try
            {
                var result = await _helpdeskDepartmentService.CreateDepartmentAsync(helpdeskDepartment);
                if (result.DeptID != 0)
                {
                    return StatusCode(201,"Helpdesk Department Created Succesfully");
                }
                return BadRequest(new { message = "Failed to create helpdesk department." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error occurred while fetching helpdesk department: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteHelpdeskDepartment")]
        [Authorize]
        public async Task<IActionResult> DeleteHelpdeskDepartment(int deptId)
        {
            try
            {
                var result = await _helpdeskDepartmentService.DeleteDepartmentAsync(deptId);
                if (result)
                {
                    return Ok(new { message = "Helpdesk department deleted successfully." });
                }
                return NotFound(new { message = "Helpdesk department not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error occurred while deleting helpdesk department: {ex.Message}" });
            }
        }
        [HttpPut("UpdateHelpdeskDepartment")]
        [Authorize]
        public async Task<IActionResult> UpdateHelpdeskDepartment(HelpdeskDepartmentDto helpdeskDepartment)
        {
            try
            {
                var result = await _helpdeskDepartmentService.UpdateHelpdeskDepartmentAsync(helpdeskDepartment);
                if (result == true)
                {
                    return Ok(new { message = "Helpdesk department updated successfully." });
                }
                return BadRequest(new { message = "Failed to update helpdesk department." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error occurred while updating helpdesk department: {ex.Message}" });
            }
        }

    }
}
