using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Service.HelpdeskCategory;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpdeskCategoriesController : ControllerBase
    {
        private readonly IHelpdeskCategoryService _helpdeskCategoryService;
        public HelpdeskCategoriesController(IHelpdeskCategoryService helpdeskCategoryService)
        {
            _helpdeskCategoryService = helpdeskCategoryService;
        }
        [HttpGet("GetAllCategories")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationRequest paginationRequest)
        {
            var categories = await _helpdeskCategoryService.GetAllCategoriesAsync(paginationRequest);
            return Ok(categories);
        }
        [HttpPost("CreateDepartmentCategory")]
        [Authorize]
        public async Task<IActionResult> CreateDepartmentCategory([FromBody] HelpdeskCategoryDto helpdeskCategory)
        {
            if (helpdeskCategory == null)
            {
                return BadRequest("Helpdesk category cannot be null");
            }
            var createdCategory = await _helpdeskCategoryService.CreateHelpdeskCategoryAsync(helpdeskCategory);
            return CreatedAtAction(nameof(GetAllCategories), new { id = createdCategory.CategoryID }, createdCategory);
        }
        [HttpPut("{categoryId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] HelpdeskCategoryDto helpdeskCategory)
        {
            if (helpdeskCategory == null || helpdeskCategory.CategoryID != categoryId)
            {
                return BadRequest("Helpdesk category cannot be null and must match the ID in the URL");
            }
            var updatedCategory = await _helpdeskCategoryService.UpdateHelpdeskCategoryAsync(helpdeskCategory);
            if (updatedCategory == null)
            {
                return NotFound($"Helpdesk category with ID {categoryId} not found");
            }
            return Ok(updatedCategory);
        }
        [HttpDelete("{categoryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var result = await _helpdeskCategoryService.DeleteHelpdeskCategoryAsync(categoryId);
            if (!result)
            {
                return NotFound($"Helpdesk category with ID {categoryId} not found");
            }
            return NoContent();
        }
    }
}
