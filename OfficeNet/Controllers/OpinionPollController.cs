using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Service;
using OfficeNet.Service.OpinionPoll;
using System.Drawing.Printing;
using System.Numerics;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionPollController : ControllerBase
    {
        private readonly IOpinionPollService _opinionPollService;
        private readonly ICurrentUserService _currentUserService;
        public OpinionPollController(IOpinionPollService opinionPollService, ICurrentUserService currentUserService)
        {
            _opinionPollService = opinionPollService ?? throw new ArgumentNullException(nameof(opinionPollService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }
        [HttpPost("SaveOpinionPollTopicAsync")]
        [Authorize]
        public async Task<IActionResult> SaveOpinionPollTopicAsync(OpinionPollTopic topic)
        {
            if (topic == null)
                return BadRequest("Invalid opinion poll topic data.");

            try
            {
                topic.CreatedBy = _currentUserService.GetUserId();
                //topic.CreatedOn = DateTime.UtcNow;
                topic.ModifiedBy = _currentUserService.GetUserId();
                //topic.ModifiedOn = DateTime.UtcNow;

                if (topic.FromDate > topic.ToDate)
                    return BadRequest("From date cannot be later than To date.");

                var result = await _opinionPollService.SaveOpinionPollTopicAsync(topic);

                return Ok(new
                {
                    StatusCode = 201,
                    status = "success",
                    message = "Opinion Poll Created Successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the opinion poll topic: {ex.Message}");
            }
        }

        [HttpGet("GetOpinionPollTopicsAsync")]
        [Authorize]
        public async Task<IActionResult> GetOpinionPollTopicsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var topics = await _opinionPollService.GetOpinionPollTopicsAsync( pageNumber,  pageSize);
                return Ok(new
                {
                    StatusCode = 200,
                    status = "success",
                    data = topics.Data,
                    TotalCount = topics.TotalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving opinion poll topics: {ex.Message}");
            }
        }

        [HttpGet("DeleteOpinionPollTopicAsync")]
        [Authorize]
        public async Task<IActionResult> DeleteOpinionPollTopicAsync(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("Invalid topic ID.");
            try
            {
                var result = await _opinionPollService.DeleteOpinionPollTopicAsync(questionId);
                if (result > 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Opinion Poll Topic Deleted Successfully."
                    });
                }
                else
                {
                    return NotFound("Opinion Poll Topic not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the opinion poll topic: {ex.Message}");
            }
        }

        [HttpGet("OpinionStatus")]
        [Authorize]
        public async Task<IActionResult>OpinionStatus(bool Status, int questionId)
        {
            if (questionId <= 0)
                return BadRequest("Invalid topic ID.");
            try
            {
                var topic = await _opinionPollService.UpdateOpinionStatus(Status, questionId);
                if(topic <= 0)
                {
                    return NotFound("Opinion Poll Topic not found or status update failed.");
                }
                return Ok(new
                {
                    StatusCode = 200,
                    status = "success",
                    message = "Opinion Poll Status Updated Successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the opinion poll status: {ex.Message}");
            }
        }

        [HttpPatch("UpdateOpinionPollTopicAsync")]
        [Authorize]
        public async Task<IActionResult> UpdateOpinionPollTopicAsync(OpinionPollTopic topic)
        {
            if (topic == null || topic.QuestionId <= 0)
                return BadRequest("Invalid opinion poll topic data.");
            try
            {
                topic.ModifiedBy = _currentUserService.GetUserId();
                //topic.ModifiedOn = DateTime.UtcNow;
                if (topic.FromDate > topic.ToDate)
                    return BadRequest("From date cannot be later than To date.");
                var result = await _opinionPollService.UpdateOpinionPollTopicAsync(topic);
                
                if (result > 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Opinion Poll Topic Updated Successfully."
                    });
                }
                else
                {
                    return NotFound("Opinion Poll Topic not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the opinion poll topic: {ex.Message}");
            }
        }

        [HttpGet("GetActiveOpinionPollAsync")]
        [Authorize]
        public async Task<IActionResult> GetActiveOpinionPollAsync()
        {
            try
            {
                var topic = await _opinionPollService.GetActiveOpinionPoll();
                if (topic == null)
                {
                    return Ok(new
                    {
                        StatusCode = 404,
                        status = "Data Not found",
                        data = topic
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    status = "success",
                    data = topic
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the opinion poll topic: {ex.Message}");
            }
        }

        [HttpPost("SaveOpinionPollAnswer")]
        [Authorize]
        public async Task<IActionResult> SaveOpinionPollAnswer(SaveOpinionAnswer topic)
        {
            //if(topic.ModifiedBy != _currentUserService.GetUserId())
            //{
            //    return Unauthorized("You  are not a authorised user");
            //}
            if (topic == null || topic.OptionIds == null || !topic.OptionIds.Any())
                return BadRequest("Invalid opinion poll answer data.");
            try
            {
                //topic.ModifiedBy = _currentUserService.GetUserId();
                //topic.CreatedOn = DateTime.UtcNow;
                var result = await _opinionPollService.SaveOpinionPollAnswer(topic);
                
                if (result != null)
                {
                    return Ok(new
                    {
                        StatusCode = 201,
                        status = "success",
                        message = "Opinion Poll Answer Saved Successfully.",
                        data = result
                    });
                }
                else
                {
                    return BadRequest("Failed to save opinion poll answer.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the opinion poll answer: {ex.Message}");
            }
        }
        [HttpGet("GetOpinionPollResultsAsync")]
        [Authorize]
        public async Task<IActionResult> GetOpinionPollResultsAsync(int questionId)
        {
            if (questionId <= 0)
                return BadRequest("Invalid question ID.");
            try
            {
                var results = await _opinionPollService.GetOpinionPollResultsAsync(questionId);
                if (results == null || !results.Any())
                {
                    return NotFound("No results found for the given question ID.");
                }
                return Ok(new
                {
                    StatusCode = 200,
                    status = "success",
                    data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving opinion poll results: {ex.Message}");
            }
        }
        
        [HttpGet("ViewResultEmployeeWise")]
        [Authorize]
        public async Task<IActionResult> ViewResultEmployeeWise(int questionId, int pageNumber, int pageSize)
        {
            if (questionId <= 0)
                return BadRequest("Invalid question ID.");
            try
            {
                dynamic results = await _opinionPollService.ViewResultEmployeeWise(questionId, pageNumber, pageSize);
                if (results == null)
                {
                    Ok(new
                    {
                        StatusCode = 404,
                        status = "success",
                        data = results
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    status = "success",
                    results.TotalCount,
                    results.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving employee-wise opinion poll results: {ex.Message}");
            }
        }
    }
}
