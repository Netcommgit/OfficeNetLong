using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMapper _mapper;   
        private readonly ICurrentUserService _currentUserService;
        public DepartmentService(ApplicationDbContext context, ILogger<DepartmentService> logger,IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<UsersDepartment> DeleteDepartmentAsync(UsersDepartment department)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UsersDepartment>> GetDepartmentAsync()
        {
            try
            {
                var result = await _context.UsersDepartments.ToListAsync();
                if(result == null)
                {
                    _logger.LogInformation("No departments found.");
                }
                _logger.LogInformation($"Department data fetched successfully");
                return result;
            }
            catch (Exception ex) {
                _logger.LogError($"Some error occured while getting department data{ex}");
                throw new Exception($"Some error occured while getting department data{ex}");
            }            
        }

        public async Task<UsersDepartment> GetDepartmentById(int id)
        {
            throw new NotImplementedException();
        }

        //public async Task<UsersDepartment> SaveDepartmentAsync(UsersDepartment department)
        public async Task<UsersDepartment> SaveDepartmentAsync(DepartmentDto department)
        {
            
            var entitydepartment = _mapper.Map<UsersDepartment>(department);
            entitydepartment.CreatedBy = _currentUserService.GetUserId();
            entitydepartment.CreatedOn = DateTime.Now;  
            entitydepartment.ModifiedBy = _currentUserService.GetUserId();
            entitydepartment.ModifiedOn = DateTime.Now;
            try
            {
                await _context.UsersDepartments.AddAsync(entitydepartment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Department data Save Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Some error occured{ex}");
                throw new Exception($"An error occured{ex}");
            }
            return entitydepartment;
        }

        public async Task<UsersDepartment> UpdateDepartmentAsync(UsersDepartment department)
        {
            var existingDepartment = await _context.UsersDepartments.FindAsync(department.DeptID);
            if (existingDepartment != null) { 
                _logger.LogWarning($"Departmet with ID {department.DeptID} not found");
                return null;
            }
            try
            {
                existingDepartment.DeptName = department.DeptName;
                existingDepartment.Archive = department.Archive;
                existingDepartment.Status = department.Status;
                existingDepartment.GroupID = department.GroupID;
                existingDepartment.ModifiedBy = department.ModifiedBy;
                existingDepartment.ModifiedOn = department.ModifiedOn;
                var result  = await _context.SaveChangesAsync();
                _logger.LogInformation($"Department saved successfully");
                return existingDepartment;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Some error occured while updating documnets{ex}");
                throw new Exception($"Some error occured while updating documnets{ex}");
            }
            
        }
    }
}
