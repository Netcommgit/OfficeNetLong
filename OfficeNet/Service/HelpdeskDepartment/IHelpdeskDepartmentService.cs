using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.PaginationModel;

namespace OfficeNet.Service.HelpdeskDepartment
{
    public interface IHelpdeskDepartmentService
    {
        Task<HelpdeskDepartmentDto> CreateDepartmentAsync(HelpdeskDepartmentDto helpdeskDepartmentDto);
        Task<bool> UpdateHelpdeskDepartmentAsync(HelpdeskDepartmentDto helpdeskDepartmentDto);
        Task<bool> DeleteDepartmentAsync(int deptId);
        Task<HelpdeskDepartmentDto> GetByIdAsync(int deptId);
        Task<PaginatedResult<HelpdeskDepartmentDto>> GetHelpdeskAsync(PaginationRequest request);

        Task<bool> IsNameExistsAsync(string deptName, int? deptId = null);
    }
}
