using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;
//using OfficeNet.Migrations;
using System.Security.Claims;
using System.Threading.Channels;

namespace OfficeNet.Service.OpinionPoll
{
    public class OpinionPollService : IOpinionPollService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OpinionPollService> _logger;
        private readonly ICurrentUserService _currentUserService;
        public OpinionPollService(ApplicationDbContext context, ILogger<OpinionPollService> logger, ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
            _currentUserService = currentUserService;
        }
        
        public Task<OpinionPollTopic> GetOpinionPollTopicByIdAsync(int topicId)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<object> Data, int TotalCount)> GetOpinionPollTopicsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.OpinionPollTopics
                    .Where(t => t.Archive == false)
                    .OrderByDescending(t => t.CreatedOn);

                var totalCount = await query.CountAsync();

                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new
                    {

                        t.QuestionId,
                        t.Topic,
                        //t.CreatedOn,
                        t.Archive,
                        t.ShowResults,
                        t.SelectionType,
                        FromDate = t.FromDate.HasValue ? t.FromDate.Value.ToString("dd-MM-yyyy") : null,
                        ToDate = t.ToDate.HasValue ? t.ToDate.Value.ToString("dd-MM-yyyy") : null,
                        Status = t.ToDate.HasValue && t.ToDate.Value.Date < DateTime.UtcNow.Date? "Expired": t.Status.ToString(),
                        IsSubmitted = _context.OpinionPollAnswers.Any(a => a.QuestionId == t.QuestionId),
                        Options = t.OpinionPollOptions.Select(o => new
                        {
                            o.PollOptionId,
                            o.OptionName,
                            o.QuestionId
                        }).ToList(),

                        // Check if any answers exist for this QuestionId
                        
                    })
                    .ToListAsync();

                return (data.Cast<object>().ToList(), totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving opinion poll topics");
                throw new Exception("There was an error retrieving the opinion poll topics", ex);
            }
        }


        public async Task<OpinionPollTopic> SaveOpinionPollTopicAsync(OpinionPollTopic topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            try
            {
                topic.CreatedOn = DateTime.UtcNow;
                topic.ModifiedOn = DateTime.UtcNow;
                topic.Archive = false;
                topic.Status = true;
                _context.OpinionPollTopics.Add(topic);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Failed to save the opinion poll topic");
                }


                //foreach (var option in topic.OpinionPollOptions)
                //{
                //    option.PollOptionId = 0; // Reset the ID to ensure a new record is created
                //    option.QuestionId = topic.QuestionId; // Set FK manually
                //    _context.OpinionPollOptions.Add(option);
                //}

                //await _context.SaveChangesAsync();
                //if (result <= 0)
                //{
                //    throw new Exception("Failed to save the opinion poll topic");
                //}
                _logger.LogInformation("Opinion poll topic saved successfully with ID: {TopicId}", topic.QuestionId);
                var statusResult = await _context.Database.ExecuteSqlRawAsync(
                   @"UPDATE OpinionPollTopics 
                      SET Status = 0 
                      WHERE QuestionId != {0}", topic.QuestionId);
                return topic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving opinion poll topic");
                throw new Exception("There was an error saving the opinion poll topic", ex);
            }
        }

        public async Task<int> DeleteOpinionPollTopicAsync(int questionId)
        {
            try
            {
                var archiveResult = await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE OpinionPollTopics 
                      SET Archive = {0} 
                      WHERE QuestionId = {1}", true, questionId);
                if (archiveResult == 0)
                {
                    throw new Exception($"No opinion poll Question found with ID: {questionId}");
                }
                _logger.LogInformation("Opinion poll topic with ID: {questionID} archived successfully", questionId);
                return archiveResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting opinion poll topic with ID: {questionID}", questionId);
                throw new Exception($"There was an error deleting the opinion poll question with ID: {questionId}", ex);
            }
        }

        public async Task<int> UpdateOpinionStatus(bool Status, int questionId)
        {
            try
            {
                if (Status)
                {
                    await _context.Database.ExecuteSqlRawAsync(@"UPDATE OpinionPollTopics 
                  SET Status = {0} 
                  WHERE QuestionId != {1}", false, questionId);
                }
                var archiveResult = await _context.Database.ExecuteSqlRawAsync(
                    @"UPDATE OpinionPollTopics 
                  SET Status = {0} 
                  WHERE QuestionId = {1}", Status, questionId);
                if (archiveResult == 0)
                {
                    throw new Exception($"No opinion poll topic found with ID: {questionId}");
                }
                _logger.LogInformation("Opinion poll status updated successfully for topic ID: {TopicId}", questionId);
                return archiveResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating opinion poll status for topic ID: {TopicId}", questionId);
                throw new Exception($"There was an error updating the opinion poll status for topic ID: {questionId}", ex);
            }
        }

        public async Task<int> UpdateOpinionPollTopicAsync(OpinionPollTopic topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            try
            {
                //var existingTopic = await _context.OpinionPollTopics.FindAsync(topic.QuestionId);
                var existingTopic = await _context.OpinionPollTopics.Include(t => t.OpinionPollOptions).FirstOrDefaultAsync(t => t.QuestionId == topic.QuestionId);
                if (existingTopic == null)
                {
                    throw new Exception($"No opinion poll topic found with ID: {topic.QuestionId}");
                }
                

                existingTopic.Topic = topic.Topic;
                existingTopic.SelectionType = topic.SelectionType;
                existingTopic.ShowResults = topic.ShowResults;
                existingTopic.FromDate = topic.FromDate;
                existingTopic.ToDate = topic.ToDate;
                existingTopic.ModifiedBy = topic.ModifiedBy;
                existingTopic.ModifiedOn = DateTime.UtcNow;

                var incomingOptions = topic.OpinionPollOptions ?? new List<OpinionPollOption>();


                foreach (var existingOption in existingTopic.OpinionPollOptions.ToList())
                {
                    var updatedOption = incomingOptions.FirstOrDefault(o => o.PollOptionId == existingOption.PollOptionId);
                    if (updatedOption != null)
                    {
                        existingOption.OptionName = updatedOption.OptionName;
                    }
                    else
                    {

                        _context.OpinionPollOptions.Remove(existingOption);
                    }
                }


                var newOptions = incomingOptions
                    .Where(o => o.PollOptionId == 0)
                    .Select(o => new OpinionPollOption
                    {
                        OptionName = o.OptionName,
                        QuestionId = existingTopic.QuestionId
                    });

                await _context.OpinionPollOptions.AddRangeAsync(newOptions);

                var changes = await _context.SaveChangesAsync();



                if (changes <= 0)
                {
                    throw new Exception("Failed to update the opinion poll topic");
                }
                _logger.LogInformation("Opinion poll topic updated successfully with ID: {TopicId}", topic.QuestionId);

                return changes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating opinion poll topic with ID: {TopicId}", topic.QuestionId);
                throw new Exception($"There was an error updating the opinion poll topic with ID: {topic.QuestionId}", ex);
            }
            throw new NotImplementedException();
        }

        public async Task<object> GetActiveOpinionPoll()
        {
            var userId = _currentUserService.GetUserId();
            try
            {
                var result = await _context.ActivatePollDTOs.FromSqlRaw(@"EXEC SP_GetActiveOpinionPoll @UserId = {0}", userId).ToListAsync();
                if (result == null)
                {
                    _logger.LogWarning("No active opinion poll found.");
                    return null;
                }
                var modifiedTopics = result
                    .GroupBy(r => new { r.QuestionId, r.Topic, r.SelectionType,r.ShowResults })
                    .Select(group => new
                    {
                        group.Key.QuestionId,
                        group.Key.Topic,
                        group.Key.SelectionType,
                        group.Key.ShowResults,
                        OptionIds = group.Where(o => o.OptioinId != null).Select(o => o.OptioinId.Value).ToList(),
                        OpinionPollOptions = group.Select(o => new
                        {
                            o.PollOptionId,
                            o.OptionName,
                            //o.OptioinId,
                        }).ToList()
                    })
                    .FirstOrDefault();

                _logger.LogInformation("Retrieving active opinion poll topic");
                return modifiedTopics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active opinion poll");
                throw new Exception("There was an error retrieving the active opinion poll", ex);
            }

        }

        //public async Task<object> SaveOpinionPollAnswer(SaveOpinionAnswer topic)
        //{
        //    if (topic?.QuestionId == null || topic.OptionIds == null || !topic.OptionIds.Any())
        //        throw new ArgumentException("No options provided to save.");

        //    try
        //    {
        //        foreach (var optionId in topic.OptionIds)
        //        {
        //            if (optionId == 0)
        //                throw new ArgumentException("PollOptionId cannot be zero for saving an answer.");

        //            await _context.Database.ExecuteSqlRawAsync(
        //                @"EXEC SP_SaveOpinionAnswer @QuestionId = {0}, @OptionId = {1}, @UserId = {2}",
        //                topic.QuestionId, optionId, topic.ModifiedBy);
        //        }

        //        _logger.LogInformation("Opinion poll answer saved successfully for topic ID: {TopicId}", topic.QuestionId);
        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving opinion poll answer");
        //        throw new Exception("There was an error saving the opinion poll answer", ex);
        //    }
        //}

        public async Task<object> SaveOpinionPollAnswer(SaveOpinionAnswer topic)
        {
            if (topic?.QuestionId == null || topic.OptionIds == null || !topic.OptionIds.Any())
                throw new ArgumentException("No options provided to save.");

            var results = new List<object>();

            foreach (var optionId in topic.OptionIds)
            {
                if (optionId == 0)
                {
                    results.Add(new
                    {
                        OptionId = optionId,
                        Status = "Failed",
                        ErrorMessage = "PollOptionId cannot be zero."
                    });
                    continue;
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        @"EXEC SP_SaveOpinionAnswer @QuestionId = {0}, @OptionId = {1}, @UserId = {2}",
                        topic.QuestionId, optionId, topic.ModifiedBy);

                    results.Add(new
                    {
                        OptionId = optionId,
                        Status = "Success",
                        ErrorMessage = (string)null
                    });
                }
                catch (SqlException ex) when (ex.Message.Contains("already voted"))
                {
                    results.Add(new
                    {
                        OptionId = optionId,
                        Status = "AlreadyVoted",
                        ErrorMessage = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        OptionId = optionId,
                        Status = "Failed",
                        ErrorMessage = ex.Message
                    });
                }
            }

            _logger.LogInformation("Opinion poll answer process completed for topic ID: {TopicId}", topic.QuestionId);
            return results;
        }


        public Task<List<OpinionResult>> GetOpinionPollResultsAsync(int questionId)
        {
            if (questionId <= 0)
            {
                throw new ArgumentException("Invalid question ID provided.");
            }
            try
            {
                var result =  _context.OpinionResults.FromSqlRaw(@"EXEC SP_GetOpinionResult @QuestionId = {0}", questionId).ToListAsync();
                _logger.LogInformation("Retrieving opinion poll results for question ID: {QuestionId}", questionId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating question ID for opinion poll results retrieval");
                throw new Exception("There was an error validating the question ID", ex);
            }

        }

        public async Task<object> ViewResultEmployeeWise(int questionId,int pageNumber,int pageSize)
        {
            try
            {
                var data = await _context.EmployeeWiseOpinionResults
                       .FromSqlRaw("EXEC SP_GetEmployeWiseResult @QuestionId = {0}, @PageNumber = {1}, @PageSize = {2}",
                                    questionId, pageNumber, pageSize)
                       .ToListAsync();
                var TotalCount = await _context.ResultCounts.FromSqlRaw(@"SELECT COUNT(*) AS TotalCount FROM OpinionPollLogs WHERE QuestionId = {0}", questionId).FirstOrDefaultAsync();
                if (data == null)
                {
                    _logger.LogWarning("No employee-wise opinion results found for question ID: {QuestionId}", questionId);
                    return new List<EmployeeWiseOpinionResult>();
                }
                _logger.LogInformation("Retrieving employee-wise opinion results for question ID: {QuestionId}", questionId);
                return new
                {
                    TotalCount = TotalCount?.TotalCount ?? 0,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee-wise opinion results for question ID: {QuestionId}", questionId);
                throw new Exception($"There was an error retrieving employee-wise opinion results for question ID: {questionId}", ex);
            }

        }
    }
}
