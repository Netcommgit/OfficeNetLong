using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.PlantService
{
    public class PlantsMasterService : IPlantsMasterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlantsMasterService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PlantsMasterService(ApplicationDbContext context, ILogger<PlantsMasterService> logger, ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
        }
        public async Task<bool> DeletePlantAsync(int plantId)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(plantId);

                if (plant == null)
                {
                    return false;
                }

                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Data Deleted Successfully");
                return true;
            }
            catch (Exception ex) {
                _logger.LogError("Some Error Occured ");
                throw new Exception($"Some error occurred: {ex.Message}", ex);
            }
        }

        public Task DeletePlantByNameAsync(string plantName)
        {
            throw new NotImplementedException();
        }

        public Task<Plant> GetPlantByIdAsync(Plant plantId)
        {
            throw new NotImplementedException();
        }

        public Task<Plant> GetPlantByNameAsync(Plant plantName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Plant>> GetPlantListAsync()
        {
            try
            {
                var result = await _context.Plants.ToListAsync();
                if (result == null || result.Count == 0)
                    throw new ArgumentNullException("No plants  are available");

                _logger.LogInformation("Data Get Successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("There is error while fetching data", ex);
                throw new Exception("There is error while fetching data", ex);
            }
        }

        public async Task<Plant> SavePlantAsync(Plant objPlant)
        {
            if (objPlant == null)
                throw new ArgumentNullException(nameof(objPlant));
            if(objPlant.PlantId !=0 && objPlant.PlantId != null)
            {
                var existingPlant = await _context.Plants.FindAsync(objPlant.PlantId);
                if (existingPlant == null)
                {
                    _logger.LogWarning($"Plant with ID {objPlant.PlantId} not found.");
                    return null; 
                }

                existingPlant.PlantName = objPlant.PlantName;
                existingPlant.SAPCode = objPlant.SAPCode;
                existingPlant.Status = objPlant.Status;
                existingPlant.PlantDescription = objPlant.PlantDescription;
                existingPlant.ModifiedBy = _currentUserService.GetUserId();
                existingPlant.ModifiedOn = objPlant.ModifiedOn;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Plant with ID {objPlant.PlantId} updated successfully.");

                return existingPlant;

            }
            else
            {
                try
                {
                    objPlant.CreatedBy = _currentUserService.GetUserId();
                    objPlant.ModifiedBy = _currentUserService.GetUserId();
                    _context.Plants.Add(objPlant);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Plant Saved successfully");
                    return objPlant;
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while saving the plant.", ex);
                    throw new Exception("An error occurred while saving the plant.", ex);
                }
            }
                
        }


        public Task<Plant> UpdatePlantAsync(Plant objPlant)
        {
            throw new NotImplementedException();
        }

        
    }
}
