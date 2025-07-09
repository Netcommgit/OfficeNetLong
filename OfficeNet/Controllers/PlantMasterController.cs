using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Entities;
using OfficeNet.Service;
using OfficeNet.Service.PlantService;
using System.Security.Policy;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantMasterController : ControllerBase
    {
        private readonly IPlantsMasterService _plantService;
        private readonly ICurrentUserService _currentUserService;

        public PlantMasterController(IPlantsMasterService plantService, ICurrentUserService currentUserService)
        {
            _plantService = plantService;
            _currentUserService = currentUserService;
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> SavePlantData(Plant plant)
        {
            if (plant == null)
                return BadRequest("Invalid plant data.");

            try
            {
                if (plant.PlantId == 0 || plant.PlantId == null)
                {
                    plant.CreatedBy = _currentUserService.GetUserId();
                    plant.CreatedOn = DateTime.UtcNow;
                }
                plant.ModifiedBy = _currentUserService.GetUserId();
                plant.ModifiedOn = DateTime.UtcNow;
                var result = await _plantService.SavePlantAsync(plant);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the plant: {ex.Message}");
            }
        }

        [HttpGet("GetPlantList")]
        [Authorize]
        public async Task<IActionResult> GetPlantData()
        {
            var result = await _plantService.GetPlantListAsync();
            return Ok(result);
        }
        [HttpDelete("DeletePlant/{plantId}")]
        [Authorize]
        public async Task<IActionResult> DeletaPlantAsync(int plantId)
        {
            var result = await  _plantService.DeletePlantAsync(plantId);
            return Ok(result);
        }
    }
}
