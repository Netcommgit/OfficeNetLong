using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;

namespace OfficeNet.Service.HelpdeskCategory
{
    public interface IHelpdeskCategoryService
    {
        Task<PaginatedResult<HelpdeskCategoryDto>> GetAllCategoriesAsync(PaginationRequest paginationRequest);
        Task<HelpdeskCategoryDto> CreateHelpdeskCategoryAsync(HelpdeskCategoryDto helpdeskCategory);
        Task<HelpdeskCategoryDto> UpdateHelpdeskCategoryAsync(HelpdeskCategoryDto helpdeskCategory);
        Task<bool> DeleteHelpdeskCategoryAsync(int categoryId);
        Task<HelpdeskCategoryDto> GetCategoryByIdAsync(int categoryId);
    }
}
