using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Identity.Client;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Filters;
using OfficeNet.Service;
using OfficeNet.Service.Survey;
using System.Drawing.Printing;
using System.Numerics;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyDetailsController : ControllerBase
    {
        private readonly ISurveyDetailsService _surveyDetailsService;
        private readonly ICurrentUserService _currentUserService;

        public SurveyDetailsController(ISurveyDetailsService surveyDetailsService, ICurrentUserService currentUserService)
        {
            _surveyDetailsService = surveyDetailsService;
            _currentUserService = currentUserService;
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> SaveSurveyAsync(ServiceSaveRequest surveyData)
        {
            try
            {
                var surveyDetails = surveyData.SurveyDetails;
                var userList = surveyData.UserList;
                var surveyQuestion = surveyData.surveyQuestion;
                var questionType = surveyData.questionType;

                var userId = _currentUserService.GetUserId();
                surveyDetails.CreatedBy = userId;
                surveyDetails.ModifiedBy = userId;
                surveyDetails.CreatedOn = DateTime.Now;
                surveyDetails.ModifiedOn = DateTime.Now;
                if (surveyDetails == null)
                {
                    return BadRequest();
                }
                var result = await _surveyDetailsService.SaveSurveyDetailsAsync(surveyDetails, userList, surveyQuestion, questionType);
                if (result.SurveyId > 0)
                {
                    return StatusCode(201, new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Survey created.",
                        //data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Survey creation failed.",
                        //data = result
                    });
                }
            }
            catch (Exception exx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred while saving the survey.",
                    Error = exx.Message // you can remove this in production for security reasons
                });
            }

        }
        [HttpGet("GetSurveyList")]
        [Authorize]
        public async Task<IActionResult> GetSurveyList(int PageNo, int PageSize)
        {
            try
            {
                var result = await _surveyDetailsService.GetSurveyListAsync(PageNo, PageSize);

                if (result.Data.Count > 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Survey data fetched successfully.",
                        result,
                    });
                }

                return Ok(new
                {
                    StatusCode = 204,
                    status = "error",
                    message = "No survey data found."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = "error",
                    message = "An unexpected error occurred while getting survey data.",
                    error = ex.Message // Optional: remove in production
                });
            }
        }

        [HttpGet("GetSurveyById")]
        [Authorize]
        public async Task<IActionResult> GetSurveyById(int surveyId)
        {
            var survey = await _surveyDetailsService.GetSurveyDetailById(surveyId);
            if (survey != null)
            {
                var questionList = await _surveyDetailsService.GetQuestionById(surveyId);
                return StatusCode(200, new
                {
                    message = "Survey data fetched successfuly.",
                    survey = survey,
                    questionList = questionList
                });
            }

            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred while saving the survey.",
                });
            }
        }

        [HttpPatch("UpdateSurvey")]
        [Authorize]
        public async Task<IActionResult> UpdateSurveyResultAsync(SurveyUpdateDto updateSurvey)
        {
            var surveyId = updateSurvey.surveyId;
            //updateSurvey.surveyDetails.modifiedOn = DateTime.Now;
            var result = await _surveyDetailsService.UpdateSurveyDetailsAsync(updateSurvey);
            return StatusCode(200, new
            {
                message = "Survey data Updated successfuly.",
                statusCode = 200,
                //result = result,
            });
        }

        [HttpPost("DeactivateSurvey")]
        [Authorize]
        public async Task<IActionResult> DeactivateSurvey(SurveyList surveyUpdateDto)
        {
            var result = await _surveyDetailsService.DeactivateSurvey(surveyUpdateDto);
            if (result != null)
            {
                return StatusCode(200, new
                {
                    message = "Survey deactivated successfully.",
                    statusCode = 200,
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred while deactivating the survey.",
                });
            }
        }
        [HttpPost("SaveSurveyResponse")]
        [Authorize]
        public async Task<IActionResult> SaveSurveyResponse(SurveyQuestionResponse questionResponse)
        {
            try
            {
                if (questionResponse == null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Invalid survey response data."
                    });
                }
                var result = await _surveyDetailsService.SaveQuestionResponse(questionResponse);
                return result != null ? StatusCode(201, new
                {
                    status = "success",
                    message = "Survey response saved successfully.",
                    //data = result
                }) : BadRequest(new
                {
                    status = "error",
                    message = "Failed to save survey response."
                });
            }
            catch (Exception exx)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new
                {
                    Message = exx.Message,
                });
            }

        }

        [HttpGet("GetSurveyUserList")]
        [Authorize]
        public async Task<IActionResult> GetSurveyUserList(int surveyId, bool isSubmitted, int PageNo, int PageSize)
        {
            try
            {
                var result = await _surveyDetailsService.GetSurveyUserListsWithCount(surveyId, isSubmitted, PageNo, PageSize);
                if (result.Users != null && result.Users.Any())
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Survey user list retrieved successfully.",
                        data = result.Users,
                        totalCount = result.TotalCount
                    });
                }
                return Ok(new
                {
                    StatusCode = 204,
                    status = "Warning",
                    message = "No survey user data found.",
                    data = new List<GetSurveyUserList>(),
                    totalCount = 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while getting survey user data.",
                    error = ex.Message // Optional: remove in production
                });
            }
        }

        [HttpGet("UserSurveyListAsync")]
        [Authorize]
        public async Task<IActionResult> UserSurveyListAsync(long UserId, bool IsSubmitted, int PageNo, int PageSize)
        {
            try
            {
                if (UserId != _currentUserService.GetUserId())
                {
                    return Ok(new
                    {
                        StatusCode = 204,
                        status = "error",
                        message = "Incorrect UserId."
                    });
                }
                var result = await _surveyDetailsService.UserSurveyList(_currentUserService.GetUserId(), IsSubmitted, PageNo, PageSize);
                //var resultPendingSurvey = await _surveyDetailsService.UserSurveyList(_currentUserService.GetUserId(), false, PageNo, PageSize);
                if (result.TotalCount != 0)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "User survey list retrieved successfully.",
                        data = result.Data,
                        TotalCount = result.TotalCount
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 204,
                        status = "error",
                        message = "No user survey data found.",
                        data = result.Data,
                    });
                }
            }
            catch (Exception exx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while getting user survey data.",
                    error = exx.Message
                });
            }
        }

        [HttpGet("GetSurveyQuestionsResponse")]
        [Authorize]
        public async Task<IActionResult> GetSurveyQuestionsResponse(int surveyId, long userId)
        {
            try
            {
                var result = await _surveyDetailsService.GetSurveyFlatResult(surveyId, userId);
                if (result != null)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Data Fetched",
                        result = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 204,
                        status = "error",
                        message = "No  survey data found.",
                        data = new List<SurveyResponse>()
                    });
                }

            }
            catch (Exception exx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while getting user survey data.",
                    error = exx.Message
                });
            }

        }

        [HttpPost("SaveSurveyQuestionsResponseByUserId")]
        [Authorize]
        public async Task<IActionResult> SaveSurveyQuestionsResponseByUserId(SaveSurveyResponse surveyResponse, long userId)
        {
            try
            {
                var result = await _surveyDetailsService.SaveSurveyFlatResultByUserIdAsync(surveyResponse, userId);
                if (result != null)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Survey Submitted Sucessfully."
                        //result = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 204,
                        status = "error",
                        message = "No  survey data found."
                        //,data = new List<SurveyResponse>()
                    });
                }
            }
            catch (Exception exx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while getting user survey data.",
                    error = exx.Message
                });
            }
        }
        [HttpGet("GetSurveyResult")]
        [Authorize]
        public async Task<IActionResult> GetSurveyResult(int surveyId)
        {
            try
            {
                var result = await _surveyDetailsService.GetSurveyResult(surveyId);


                if (result != null && result.Count > 0)
                {
                    var groupedResult = result
                         .GroupBy(x => new { x.QuestionId, x.QuestionText })
                         .Select(g => new QuestionResult
                         {
                             QuestionId = g.Key.QuestionId,
                             QuestionText = g.Key.QuestionText,
                             Options = g.Select(opt => new OptionResult
                             {
                                 OptionId = opt.OptionId,
                                 OptionText = opt.OptionText,
                                 AnsweredCount = opt.AnsweredCount,
                                 TotalAssignedUsers = opt.TotalAssignedUsers,
                                 ResponsePercentage = opt.ResponsePercentage
                             }).ToList()
                         }).ToList();

                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Survey results retrieved successfully.",
                        data = groupedResult
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 204,
                        status = "error",
                        message = "No survey results found.",
                        data = new List<SurveyResult>()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while getting survey results.",
                    error = ex.Message // Optional: remove in production
                });
            }
        }

        [HttpGet("ActivateSurveyForUsers")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ActivateSurveyForUsers(int surveyId, bool surveyStatus)
        {
            try
            {
                var result = await _surveyDetailsService.ActivateSurveyForUsers(surveyId, surveyStatus);
                if (result)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        status = "success",
                        message = "Survey activation status updated successfully."
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        status = "error",
                        message = "Failed to update survey activation status."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = 500,
                    status = "error",
                    message = "An unexpected error occurred while updating survey activation status.",
                    error = ex.Message
                });
            }
        }
    }
}
