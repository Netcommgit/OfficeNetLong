using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;
using OfficeNet.Service.UserService;

namespace OfficeNet.Service.DiscussionForum
{
    public class DiscussionForumService : IDiscussionForumService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiscussionForumService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public DiscussionForumService(ApplicationDbContext context, ILogger<DiscussionForumService> logger, ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
        }
        public async Task<DiscussionTopic> SaveDiscussionTopic(DiscussionTopic discussionTopic)
        {
            if(discussionTopic.ViewGroupId == 4)
            {

            }
            try
            {
                discussionTopic.CreatedBy = _currentUserService.GetUserId();
                discussionTopic.CreatedOn = DateTime.Now;
                discussionTopic.ModifiedBy = _currentUserService.GetUserId();
                discussionTopic.ModifiedOn = DateTime.Now;
                _context.DiscussionTopics.Add(discussionTopic);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Discussion topic saved successfully.");
                
                if(discussionTopic.TopicId != 0)
                {
                    if(discussionTopic.ViewGroupId == 4)
                    {

                    }
                }
                 return discussionTopic;
            }
            catch (Exception ex)
            {
                _logger.LogError("There is error while saving discussion topic", ex);
                throw new Exception("There is error while saving discussion topic", ex);
            }
        }
    }
}
