using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Domain.PaginationModel;
using OfficeNet.Infrastructure.Context;
using OfficeNet.Migrations;

namespace OfficeNet.Service.HelpdeskSubdepartment
{
    public class HelpdeskSubcategoryService : IHelpdeskSubcategoryService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<HelpdeskSubcategoryService> _logger;
        public HelpdeskSubcategoryService(
            IMapper mapper,
            ApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<HelpdeskSubcategoryService> logger)
        {
            _mapper = mapper;
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<HelpdeskSubcategoryDto> CreateSubCategoryAsync(HelpdeskSubcategoryDto subCategoryDto)
        {
            try
            {
                var entityHelpdeskSubCategory = _mapper.Map<HelpdeskSubcategoryModel>(subCategoryDto);
                entityHelpdeskSubCategory.CreatedBy = _currentUserService.GetUserId();
                entityHelpdeskSubCategory.CreatedOn = DateTime.Now;
                entityHelpdeskSubCategory.ModifiedBy = _currentUserService.GetUserId();
                entityHelpdeskSubCategory.ModifiedOn = DateTime.Now;
                await _context.HelpdeskSubcategories.AddAsync(entityHelpdeskSubCategory);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Helpdesk subcategory created successfully with ID: {entityHelpdeskSubCategory.SubCategoryID}");
                return _mapper.Map<HelpdeskSubcategoryDto>(entityHelpdeskSubCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating helpdesk subcategory: {ex.Message}");
                throw new Exception($"Error occurred while creating helpdesk subcategory: {ex.Message}");
            }
        }

        public async Task<bool> DeleteSubCategoryAsync(int subCategoryId)
        {
            var helpdeskSubCategory = await _context.HelpdeskSubcategories.FindAsync(subCategoryId);
            if (helpdeskSubCategory == null)
            {
                _logger.LogWarning($"Helpdesk subcategory with ID {subCategoryId} not found for deletion.");
                return false;
            }

            try
            {
                _context.HelpdeskSubcategories.Remove(helpdeskSubCategory);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Helpdesk subcategory with ID {subCategoryId} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting helpdesk subcategory with ID {subCategoryId}.");
                throw;
            }
        }

        public Task<PaginatedResult<HelpdeskSubcategoryDto>> GetAllSubCategoryAsync(PaginationRequest paginationRequest)
        {
            try
            {
                _logger.LogInformation("Fetching all helpdesk subcategories. Page: {PageNumber}, Size: {PageSize}",
                    paginationRequest.PageNumber, paginationRequest.PageSize);
                var query = _context.HelpdeskSubcategories
                    .Where(h => h.Status == true)
                    .OrderBy(h => h.SubCategoryName);

                var pagedResult = query.ToPaginatedResultAsync(paginationRequest);
                var mappedItems = _mapper.Map<IEnumerable<HelpdeskSubcategoryDto>>(pagedResult.Result.Items);
                return Task.FromResult(new PaginatedResult<HelpdeskSubcategoryDto>
                {
                    Items = mappedItems,
                    TotalCount = pagedResult.Result.TotalCount,
                    PageNumber = pagedResult.Result.PageNumber,
                    PageSize = pagedResult.Result.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all helpdesk subcategories");
                throw;
            }
        }

        public Task<HelpdeskSubcategoryDto> GetSubCategoryByIdAsync(int subCategoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<HelpdeskSubcategoryDto> UpdateSubCategoryAsync(int subCategoryId, HelpdeskSubcategoryDto subCategoryDto)
        {
            try
            {
                var existingEntity = await _context.HelpdeskSubcategories
                                                   .FirstOrDefaultAsync(s => s.SubCategoryID == subCategoryId);

                if (existingEntity == null)
                {
                    throw new Exception($"Subcategory with ID {subCategoryId} not found.");
                }

                // Update only allowed fields
                existingEntity.SubCategoryName = subCategoryDto.SubCategoryName;
                existingEntity.CategoryID = subCategoryDto.CategoryID;
                existingEntity.Status = subCategoryDto.Status;
                existingEntity.ModifiedBy = _currentUserService.GetUserId();
                existingEntity.ModifiedOn = DateTime.Now;

                await _context.SaveChangesAsync();

                return _mapper.Map<HelpdeskSubcategoryDto>(existingEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating helpdesk subcategory (ID: {subCategoryId})");
                throw;
            }
        }
    }
}
