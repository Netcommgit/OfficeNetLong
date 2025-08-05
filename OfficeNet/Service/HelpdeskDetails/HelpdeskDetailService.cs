
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.HelpdeskDetails
{
    public class HelpdeskDetailService : IHelpdeskDetailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HelpdeskDetailService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public HelpdeskDetailService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<HelpdeskDetailService> logger,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<bool> CreateHelpdeskDetails(HelpDeskDetailDto helpDeskDetailDto)
        {
            bool retType = false;
            try
            {


                var entity = _mapper.Map<HelpDeskDetailModel>(helpDeskDetailDto);
                entity.CreatedOn = DateTime.UtcNow;
                entity.CreatedBy = _currentUserService.GetUserId();
                entity.ModifiedOn = DateTime.UtcNow;
                entity.ModifiedBy = _currentUserService.GetUserId();

                // Save HelpDeskDetail
                _context.HelpDeskDetails.Add(entity);
                var result =  await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    _logger.LogError("Failed to save HelpDeskDetail entity.");
                    return false;
                }
                else if (result > 0)
                {
                    retType = true;
                }

                // Save related AdminUser assignments (if any)
                if (helpDeskDetailDto.AdminUsers != null && helpDeskDetailDto.AdminUsers.Any())
                {
                    var adminUsers = helpDeskDetailDto.AdminUsers.Select(admin => new HelpdeskAdminUser
                    {
                        IssueID = entity.IssueID,
                        AdminUserID = admin.AdminUserID
                    }).ToList();

                    _context.HelpdeskAdminUser.AddRange(adminUsers);
                    var result1 =  await _context.SaveChangesAsync();
                    if (result1 <= 0)
                    {
                        _logger.LogError("Failed to save HelpdeskAdminUser entities.");
                        return false;
                    }
                    else if (result1 > 0)
                    {
                        retType = true;
                    }
                }

                return retType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating helpdesk details");
                return retType;
            }
        }
    }
}
