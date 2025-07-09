using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.Department
{
    public interface IDepartmentService
    {
        Task<List<UsersDepartment>> GetDepartmentAsync();
        Task<UsersDepartment> UpdateDepartmentAsync(UsersDepartment department);
        Task<UsersDepartment> DeleteDepartmentAsync(UsersDepartment department);
        Task<UsersDepartment> SaveDepartmentAsync(UsersDepartment department);
        Task<UsersDepartment> GetDepartmentById(int id);
    }
}
