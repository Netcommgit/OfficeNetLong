using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.Thought
{
    public class ThoughtService:IThoughtService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThoughtService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly long _userId;
        private readonly IMapper _mapper;


        public ThoughtService(ApplicationDbContext context, ICurrentUserService currentUserService, ILogger<ThoughtService> logger,IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
            _userId = _currentUserService.GetUserId();
        }

        public async Task<List<ThoughtSaveModel>> GetThoughtOfTheDay(bool Flag)
        {
            try
            {
                var result =  new List<ThoughtOfDay>();
                if (Flag) {                 
                    result = await _context.ThoughtsOfTheDay
                    .Where(t => t.IsActive == true)
                    .OrderByDescending(t => t.ActDate) 
                    .ToListAsync();

                if (result == null)
                    return null;
                }
                else if(Flag == false){
                    result = await _context.ThoughtsOfTheDay
                    //.Where(t => t.IsActive == true)
                    .OrderByDescending(t => t.ActDate)
                    .ToListAsync();
                }
                var mappedResult = _mapper.Map<List<ThoughtSaveModel>>(result);
                return mappedResult;

            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Failed to fetch Thought of the Day", ex);
            }
        }


        public async Task<ThoughtSaveModel> SaveThought(ThoughtSaveModel thoughtOfDay)
        {
            if(thoughtOfDay.IsActive == true && thoughtOfDay.ThoughtID == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(@"UPDATE ThoughtsOfTheDay SET IsActive = 0,ActDate= null");
            }
            var entity = _mapper.Map<ThoughtOfDay>(thoughtOfDay);
            if (thoughtOfDay.ThoughtID == 0)
            {
                try
                {
                    if(entity.IsActive == null || entity.IsActive == false)
                    {
                        entity.ActDate = null;
                    }
                    entity.CratedBy = _userId;
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.ModifiedBy = _userId;
                    entity.ModifiedOn = DateTime.UtcNow;
                    _context.ThoughtsOfTheDay.Add(entity);
                    await _context.SaveChangesAsync();
                    thoughtOfDay.ThoughtID = entity.ThoughtID;
                    _logger.LogInformation("Thought of the day saved successfully.");
                    return thoughtOfDay;
                }
                catch (Exception ex)
                {
                    _logger.LogError("There is error while saving thought of the day", ex);
                    throw new Exception("There is error while saving thought of the day", ex);
                }
            }
            else
            {
                var existingThought =await _context.ThoughtsOfTheDay.FirstOrDefaultAsync(t => t.ThoughtID == thoughtOfDay.ThoughtID);
                if (existingThought == null)
                {
                    throw new KeyNotFoundException("Thought not found");
                }

                _mapper.Map(thoughtOfDay, existingThought);
                if (existingThought.IsActive == false || existingThought == null)
                {
                    existingThought.ActDate = null;
                }
                else if (existingThought.IsActive == true)
                {
                    await _context.Database.ExecuteSqlRawAsync(@"UPDATE ThoughtsOfTheDay SET IsActive = 0,ActDate =  null");
                }
                    existingThought.ModifiedBy = _userId;
                existingThought.ModifiedOn = DateTime.UtcNow;

                try
                {
                    _context.ThoughtsOfTheDay.Update(existingThought);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Thought of the day updated successfully.");
                    return thoughtOfDay;
                }
                catch (Exception ex)
                {
                    _logger.LogError("There is error while updating thought of the day", ex);
                    throw new Exception("There is error while updating thought of the day", ex);
                }
            }
        }

    }
}
