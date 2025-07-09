using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.OpinionPoll
{
    public interface IOpinionPollService
    {
        //Task<(List<OpinionPollTopic> Data, int TotalCount)> GetOpinionPollTopicsAsync(int pageNumber, int pageSize);
        Task<(List<object> Data, int TotalCount)> GetOpinionPollTopicsAsync(int pageNumber, int pageSize);
        Task<OpinionPollTopic> GetOpinionPollTopicByIdAsync(int topicId);
        Task<OpinionPollTopic> SaveOpinionPollTopicAsync(OpinionPollTopic topic);
        Task<int> DeleteOpinionPollTopicAsync(int questionId);
        Task<int> UpdateOpinionStatus(bool Status, int questionId);  
        Task<int> UpdateOpinionPollTopicAsync(OpinionPollTopic topic);
        Task<object> GetActiveOpinionPoll();
        Task<object> SaveOpinionPollAnswer(SaveOpinionAnswer topic);
        Task<List<OpinionResult>> GetOpinionPollResultsAsync(int questionId);
        Task<object> ViewResultEmployeeWise(int questionId, int pageNumber, int pageSize);
    }
}
