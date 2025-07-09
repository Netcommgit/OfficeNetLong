using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.PlantService
{
    public interface IPlantsMasterService
    {
        Task <List<Plant>> GetPlantListAsync();
        Task<Plant> GetPlantByIdAsync(Plant plantId);
        Task<Plant> SavePlantAsync(Plant objPlant);
        Task<Plant> GetPlantByNameAsync(Plant plantName);
        Task<bool> DeletePlantAsync(int plantId);
        Task DeletePlantByNameAsync(string plantName);
        Task <Plant>UpdatePlantAsync(Plant objPlant);
    }
}
