using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Infrastructure.Context;
using OfficeNet.Service.Department;
using OfficeNet.Service.ITHelpDesk;

namespace OfficeNet.Service.HelpdeskDepartment
{
    public class HelpdeskDepartmentService : IHelpdeskDepartmentService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<HelpdeskDepartmentService> _logger;

        public HelpdeskDepartmentService(
            IMapper mapper,
            ApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<HelpdeskDepartmentService> logger)
        {
            _mapper = mapper;
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<HelpdeskDepartmentDto> CreateDepartmentAsync(HelpdeskDepartmentDto helpdeskDepartmentDto)
        {
            try
            {


                var entityHelpdeskDepartment = _mapper.Map<HelpdeskDepartmentModel>(helpdeskDepartmentDto);
                entityHelpdeskDepartment.CreatedBy = _currentUserService.GetUserId();
                entityHelpdeskDepartment.CreatedOn = DateTime.Now;
                entityHelpdeskDepartment.ModifiedBy = _currentUserService.GetUserId();
                entityHelpdeskDepartment.ModifiedOn = DateTime.Now;
                await _context.HelpdeskDepartments.AddAsync(entityHelpdeskDepartment);
                await _context.SaveChangesAsync();
                //_context.ChangeTracker.Clear(); // Clear the change tracker to avoid memory issues with large datasets
                _logger.LogInformation($"Helpdesk department created successfully with ID: {entityHelpdeskDepartment.DeptID}");
                return _mapper.Map<HelpdeskDepartmentDto>(entityHelpdeskDepartment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating helpdesk department: {ex.Message}");
                throw new Exception($"Error occurred while creating helpdesk department: {ex.Message}");
            }

        }

        public async Task<bool> DeleteDepartmentAsync(int deptId)
        {
            var helpdeskDepartment = await _context.HelpdeskDepartments.FindAsync(deptId);
            if (helpdeskDepartment == null)
            {
                _logger.LogWarning($"Helpdesk department with ID {deptId} not found for deletion.");
                return false;
            }
            try
            {
                _context.HelpdeskDepartments.Remove(helpdeskDepartment);
               await _context.SaveChangesAsync();
                _logger.LogInformation($"Helpdesk department with ID {deptId} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while deleting helpdesk department with ID {deptId}: {ex.Message}");
                throw new Exception($"Error occurred while deleting helpdesk department: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<HelpdeskDepartmentDto>> GetHelpdeskAsync(PaginationRequest request)
        {
            try
            {
                _logger.LogInformation("Fetching helpdesk departments. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);
                var query = _context.HelpdeskDepartments
                    .Where(x => x.Status == true)
                    .OrderBy(x => x.DeptName);

                var pagedResult = await query.ToPaginatedResultAsync(request);

                var mappedItems = _mapper.Map<IEnumerable<HelpdeskDepartmentDto>>(pagedResult.Items);

                return new PaginatedResult<HelpdeskDepartmentDto>
                {
                    Items = mappedItems,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching helpdesk departments.");
                throw;
            }
        }


        public Task<HelpdeskDepartmentDto> GetByIdAsync(int deptId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsNameExistsAsync(string deptName, int? deptId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateHelpdeskDepartmentAsync(HelpdeskDepartmentDto helpdeskDepartmentDto)
        {
            if (helpdeskDepartmentDto == null)
            {
                _logger.LogError("UpdateHelpdeskDepartmentAsync: HelpdeskDepartmentDto is null.");
                throw new ArgumentNullException(nameof(helpdeskDepartmentDto));
            }

            var existingEntity = await _context.HelpdeskDepartments.FindAsync(helpdeskDepartmentDto.DeptID);

            if (existingEntity == null)
            {
                _logger.LogWarning($"UpdateHelpdeskDepartmentAsync: Helpdesk department with ID {helpdeskDepartmentDto.DeptID} not found.");
                throw new KeyNotFoundException($"Helpdesk department with ID {helpdeskDepartmentDto.DeptID} not found.");
            }

            bool isUpdated = false;

            // Compare and update fields if needed
            if (!string.IsNullOrWhiteSpace(helpdeskDepartmentDto.DeptName) &&
                existingEntity.DeptName != helpdeskDepartmentDto.DeptName)
            {
                existingEntity.DeptName = helpdeskDepartmentDto.DeptName;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(helpdeskDepartmentDto.DeptExtension) &&
                existingEntity.DeptExtension != helpdeskDepartmentDto.DeptExtension)
            {
                existingEntity.DeptExtension = helpdeskDepartmentDto.DeptExtension;
                isUpdated = true;
            }

            if (existingEntity.Status != helpdeskDepartmentDto.Status)
            {
                existingEntity.Status = helpdeskDepartmentDto.Status;
                isUpdated = true;
            }

            if (isUpdated)
            {
                existingEntity.ModifiedBy = _currentUserService.GetUserId();
                existingEntity.ModifiedOn = DateTime.UtcNow;

                try
                {
                    _context.HelpdeskDepartments.Update(existingEntity);
                    var result =  await _context.SaveChangesAsync();
                    _logger.LogInformation($"UpdateHelpdeskDepartmentAsync: Helpdesk department ID {existingEntity.DeptID} updated successfully.");
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UpdateHelpdeskDepartmentAsync: Error while updating helpdesk department ID {helpdeskDepartmentDto.DeptID}.");
                    throw new Exception("An error occurred while updating the helpdesk department.");
                }
            }

            _logger.LogInformation($"UpdateHelpdeskDepartmentAsync: No changes detected for department ID {existingEntity.DeptID}.");
            return false;
        }

    }
}
