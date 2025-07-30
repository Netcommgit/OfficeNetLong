using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Service.HelpdeskSubdepartment;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpdeskSubcategoryController : ControllerBase
    {
        private readonly IHelpdeskSubcategoryService _helpdeskSubcategoryService;
        public HelpdeskSubcategoryController(IHelpdeskSubcategoryService helpdeskSubcategoryService)
        {
            _helpdeskSubcategoryService = helpdeskSubcategoryService;
        }
        [HttpPost("CreateSubCategory")]
        public async Task<IActionResult> CreateSubCategory([FromBody] HelpdeskSubcategoryDto subCategoryDto)
        {
            if (subCategoryDto == null)
            {
                return BadRequest("Invalid subcategory data.");
            }
            var createdSubCategory = await _helpdeskSubcategoryService.CreateSubCategoryAsync(subCategoryDto);
            return CreatedAtAction(nameof(GetSubCategoryById), new { id = createdSubCategory.SubCategoryID }, createdSubCategory);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            var subCategory = await _helpdeskSubcategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var result = await _helpdeskSubcategoryService.DeleteSubCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("GetAllSubCategories")]
        public async Task<IActionResult> GetAllSubCategories([FromQuery] PaginationRequest paginationRequest)
        {
            var subCategories = await _helpdeskSubcategoryService.GetAllSubCategoryAsync(paginationRequest);
            return Ok(subCategories);
        }
        [HttpPut("UpdateSubCategory{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] HelpdeskSubcategoryDto subCategoryDto)
        {
            if (subCategoryDto == null || id != subCategoryDto.SubCategoryID)
            {
                return BadRequest("Invalid subcategory data.");
            }
            var updatedSubCategory = await _helpdeskSubcategoryService.UpdateSubCategoryAsync(id, subCategoryDto);
            if (updatedSubCategory == null)
            {
                return NotFound();
            }
            return Ok(updatedSubCategory);
        }
    }
}
