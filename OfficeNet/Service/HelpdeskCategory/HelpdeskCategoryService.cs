using AutoMapper;
using Azure.Core;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.HelpdeskCategory
{
    public class HelpdeskCategoryService : IHelpdeskCategoryService
    {
        private IMapper _mapper;
        private ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<HelpdeskCategoryService> _logger;

        public HelpdeskCategoryService(IMapper mapper, ApplicationDbContext context, ICurrentUserService currentUserService, ILogger<HelpdeskCategoryService> logger)
        {
            _mapper = mapper;
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<HelpdeskCategoryDto> CreateHelpdeskCategoryAsync(HelpdeskCategoryDto helpdeskCategory)
        {
            var entityCategory = _mapper.Map<HelpdeskCategoryModel>(helpdeskCategory);
            entityCategory.CreatedBy = _currentUserService.GetUserId();
            entityCategory.CreatedOn = DateTime.UtcNow;
            entityCategory.ModifiedBy = _currentUserService.GetUserId();
            entityCategory.ModifiedOn = DateTime.UtcNow;
            _context.HelpdeskCategories.Add(entityCategory);
            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<HelpdeskCategoryDto>(entityCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating helpdesk category");
                throw;
            }

        }
        public async Task<HelpdeskCategoryDto> UpdateHelpdeskCategoryAsync(HelpdeskCategoryDto helpdeskCategory)
        {
            if (helpdeskCategory == null)
            {
                _logger.LogError("Helpdesk category cannot be null");
                throw new ArgumentNullException(nameof(helpdeskCategory), "Helpdesk category cannot be null");
            }
            var entityCategory = await _context.HelpdeskCategories.FindAsync(helpdeskCategory.CategoryID);
            if (entityCategory == null)
            {
                _logger.LogError($"Helpdesk category with ID {helpdeskCategory.CategoryID} not found");
                throw new KeyNotFoundException($"Helpdesk category with ID {helpdeskCategory.CategoryID} not found");
            }
            entityCategory.CategoryName = helpdeskCategory.CategoryName;
            entityCategory.DeptID = helpdeskCategory.DeptID;
            entityCategory.Status = helpdeskCategory.Status;
            entityCategory.ModifiedBy = _currentUserService.GetUserId();
            entityCategory.ModifiedOn = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<HelpdeskCategoryDto>(entityCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating helpdesk category");
                throw;
            }

        }

        public async Task<bool> DeleteHelpdeskCategoryAsync(int categoryId)
        {
            var helpdeskDepartment = await _context.HelpdeskCategories.FindAsync(categoryId);
            if (helpdeskDepartment == null)
            {
                _logger.LogWarning($"Helpdesk category with ID {categoryId} not found for deletion.");
                return false;
            }
            try
            {
                _context.HelpdeskCategories.Remove(helpdeskDepartment);
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Helpdesk department with ID {categoryId} deleted successfully.");
                if (result <= 0)
                {
                    _logger.LogWarning($"No rows affected when trying to delete helpdesk category with ID {categoryId}");
                    return false;
                }
                else if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting helpdesk category with ID {categoryId}");
                return false;
            }
        }

        public async Task<PaginatedResult<HelpdeskCategoryDto>> GetAllCategoriesAsync(PaginationRequest paginationRequest)
        {
            try
            {
                _logger.LogInformation("Fetching helpdesk departments. Page: {PageNumber}, Size: {PageSize}",
            paginationRequest.PageNumber, paginationRequest.PageSize);
                var query = _context.HelpdeskCategories
                    .Where(x => x.Status == true)
                    .OrderBy(x => x.CategoryName);

                var pagedResult = await query.ToPaginatedResultAsync(paginationRequest);

                var mappedItems = _mapper.Map<IEnumerable<HelpdeskCategoryDto>>(pagedResult.Items);

                return new PaginatedResult<HelpdeskCategoryDto>
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

        public Task<HelpdeskCategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
