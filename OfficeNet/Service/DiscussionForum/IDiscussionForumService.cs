using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.DiscussionForum
{
    public interface IDiscussionForumService
    {
        Task<DiscussionTopic> SaveDiscussionTopic(DiscussionTopic discussionTopic);
    }
}
