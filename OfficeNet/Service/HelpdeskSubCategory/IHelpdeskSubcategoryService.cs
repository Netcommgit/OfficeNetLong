using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;

namespace OfficeNet.Service.HelpdeskSubdepartment
{
    public interface IHelpdeskSubcategoryService
    {
        Task<PaginatedResult<HelpdeskSubcategoryDto>> GetAllSubCategoryAsync(PaginationRequest paginationRequest);
        Task<HelpdeskSubcategoryDto> GetSubCategoryByIdAsync(int subCategoryId);
        Task<HelpdeskSubcategoryDto> CreateSubCategoryAsync(HelpdeskSubcategoryDto subCategoryDto);
        Task<HelpdeskSubcategoryDto> UpdateSubCategoryAsync(int subCategoryId, HelpdeskSubcategoryDto subCategoryDto);
        Task<bool> DeleteSubCategoryAsync(int subCategoryId);
    }
}
