using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;
//using OfficeNet.Migrations;
using System.Data;


namespace OfficeNet.Service.Survey
{
    public class SurveyDetailsService : ISurveyDetailsService
    {
        private readonly ILogger<SurveyDetailsService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public SurveyDetailsService(ILogger<SurveyDetailsService> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }


        public async Task<SurveyDetails> SaveSurveyDetailsAsync(SurveyDetails surveyDetails, List<long> userList, SurveyQuestion surveyQuestion, List<string> questionType)
        {
            try
            {
                surveyDetails.CreatedBy = _currentUserService.GetUserId();
                surveyDetails.ModifiedBy = _currentUserService.GetUserId();
                surveyDetails.SurveyStatus = true;
                _context.SurveyDetail.Add(surveyDetails);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Survey details saved successfully.");
                //var authUser = new List<SurveyAuthenticateUser>();
                if (surveyDetails.SurveyId != null && surveyDetails.SurveyId != 0)
                {
                    if (userList.Count > 0 && surveyDetails.AuthView == 2)
                    {
                        for (int i = 0; i < userList.Count; i++)
                        {
                            var surveyAuthUser = new SurveyAuthenticateUser();
                            surveyAuthUser.SurveyId = surveyDetails.SurveyId;
                            surveyAuthUser.PlantId = surveyDetails.PlantId;
                            surveyAuthUser.UserId = userList[i];
                            surveyAuthUser.CreatedBy = _currentUserService.GetUserId(); 
                            surveyAuthUser.CreatedOn = DateTime.Now;
                            _context.SurveyAuthenticateUsers.Add(surveyAuthUser);
                        }

                    }
                    else if (userList.Count == 0 && surveyDetails.AuthView == 1)
                    {
                        var results = await _userManager.Users.Select(u => u.Id).ToListAsync();
                        foreach (var result in results)
                        {
                            var surveyAuthUser = new SurveyAuthenticateUser();
                            surveyAuthUser.SurveyId = surveyDetails.SurveyId;
                            surveyAuthUser.PlantId = surveyDetails.PlantId;
                            surveyAuthUser.UserId = result;
                            surveyAuthUser.CreatedBy = _currentUserService.GetUserId(); 
                            surveyAuthUser.CreatedOn = DateTime.Now;
                            _context.SurveyAuthenticateUsers.Add(surveyAuthUser);
                        }
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Survey Auth saved successfully");

                    surveyQuestion.SurveyId = surveyDetails.SurveyId;

                    var questionOrder = await _context.SurveyQuestions
                    .Where(q => q.SurveyId == surveyDetails.SurveyId)
                    .MaxAsync(q => (int?)q.QuestionOrder) ?? 0;
                    questionOrder += 1; // Increment the order for the new question

                    surveyQuestion.QuestionOrder = questionOrder; // Assuming you want to set the order based on existing questions
                    _context.SurveyQuestions.Add(surveyQuestion);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Survey Auth saved successfully");

                    if (surveyQuestion.QuestionId != 0 && surveyQuestion.QuestionId != null)
                    {
                        for (int i = 0; i < questionType.Count; i++)
                        {
                            var surveyOption = new SurveyOption();
                            surveyOption.QuestionId = surveyQuestion.QuestionId;
                            surveyOption.OptionText = questionType[i];
                            surveyOption.OptionOrder = i + 1;
                            surveyOption.Status = true;
                            surveyOption.Archive = false;
                            _context.SurveyOptions.Add(surveyOption);
                        }
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Survey option saved successfully");
                    }

                }

                return surveyDetails;
            }
            catch (Exception ex)
            {
                var errors = ex.Message;
                _logger.LogError($"Failed to Save Survey :{errors}", errors);
                throw new Exception($"Failed to save Survey :{errors}");
            }
        }

        public Task<SurveyAuthenticateUser> CreateSurveyAuthenticateUserAsync(SurveyAuthenticateUser surveyAuthenticateUser)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedSurveyResult> GetSurveyListAsync(int pageNo, int pageSize)
        {
            var result = new PagedSurveyResult();
            try
            {
                _logger.LogInformation("GetSurveyList called with PageNo={PageNo}, PageSize={PageSize}", pageNo, pageSize);

                 result.Data = await _context.SurveyListData
                    .FromSqlRaw("EXEC SP_GetSurveyList @PageNo = {0}, @PageSize = {1}", pageNo, pageSize)
                    .ToListAsync();  // Fetch data from DB and materialize

                var total = await _context.Set<resultCount>()
                    .FromSqlRaw("SELECT COUNT(*) AS TotalCount FROM SurveyDetail WHERE Archieve = 0")
                    .FirstOrDefaultAsync();
                result.TotalCount = total?.TotalCount ?? 0;
                _logger.LogInformation("SurveyList fetched successfully.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching SurveyList with PageNo={PageNo}, PageSize={PageSize}", pageNo, pageSize);
                return new PagedSurveyResult();
            }
        }


        public async Task<SurveyDetailWithUserDto> GetSurveyDetailById(int surveyId)
        {
            try
            {
                //var result = await _context.SurveyDetail.FirstOrDefaultAsync(x => x.SurveyId == surveyId);

                var result = await (from survey in _context.SurveyDetail
                                    where survey.SurveyId == surveyId
                                    select new SurveyDetailWithUserDto
                                    {
                                        SurveyId = survey.SurveyId,
                                        SurveyName = survey.SurveyName,
                                        SurveyStart = survey.SurveyStart,
                                        SurveyEnd = survey.SurveyEnd,
                                        SurveyInstruction = survey.SurveyInstruction,
                                        SurveyConfirmation = survey.SurveyConfirmation,
                                        SurveyView = survey.SurveyView,
                                        AuthView = survey.AuthView,
                                        PlantId = survey.PlantId,
                                        DepartmentId = survey.DepartmentId,
                                        //IsExcel = survey.IsExcel,
                                        //SurveyStatus = survey.SurveyStatus,
                                        //Archieve = survey.Archieve,
                                        //CreatedBy = survey.CreatedBy,
                                        //CreatedOn = survey.CreatedOn,
                                        //ModifiedBy = survey.ModifiedBy,
                                        //ModifiedOn = survey.ModifiedOn,

                                        userLists = (from su in _context.SurveyAuthenticateUsers
                                                     join u in _context.Users on su.UserId equals u.Id
                                                     where su.SurveyId == survey.SurveyId
                                                     select new SurveyUserList
                                                     {
                                                         Id = u.Id,
                                                         FirstName = u.FirstName,
                                                         LastName = u.LastName,
                                                     }).ToList(),
                                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogWarning($"SurveyDetail with ID {surveyId} not found.");
                    throw new KeyNotFoundException($"SurveyDetail with ID {surveyId} not found.");
                }
                _logger.LogInformation($"Survey Data fetched successfuly {surveyId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception($"Some Error Occured{ex}");
            }

        }

        public async Task<List<SurveyQuestionDto>> GetQuestionById(int surveyId)
        {
            try
            {
                var questionsWithOptions = await _context.SurveyQuestions.Where(q => q.SurveyId == surveyId).Select(q => new SurveyQuestionDto
                {
                    QuestionId = q.QuestionId,
                    SurveyId = q.SurveyId,
                    QuestionText = q.QuestionText,
                    //QuestionRequierd = q.QuestionRequierd,
                    //QuestionErrorMsg = q.QuestionErrorMsg,
                    //QuestionViewReport = q.QuestionViewReport,
                    QuestionType = q.QuestionType,
                    //QuestionOrder = q.QuestionOrder,
                    //Status = q.Status,
                    //Archieve = q.Archieve,
                    Options = _context.SurveyOptions.Where(o => o.QuestionId == q.QuestionId)
                        .Select(o => new SurveyOptionDto
                        {
                            OptionId = o.OptionId,
                            OptionText = o.OptionText,
                            OptionOrder = o.OptionOrder,
                            Status = o.Status,
                            Archive = o.Archive
                        }).ToList()
                }).ToListAsync();

                if (questionsWithOptions == null)
                {
                    _logger.LogWarning($"Survey Question with ID {surveyId} not found.");
                    throw new KeyNotFoundException($"Survey Question with ID {surveyId} not found.");
                }
                _logger.LogInformation($"Survey Question fetched successfuly {surveyId}");
                return questionsWithOptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception($"Some Error Occured{ex}");
            }
        }

        public async Task<SurveyUpdateDto> UpdateSurveyDetailsAsync(SurveyUpdateDto surveyUpdateDto)
        {
            var newQuetionId = 0;

            try
            {
                // 1. Get existing SurveyDetails from DB
                var existingSurveyDetails = await _context.SurveyDetail
                    .FirstOrDefaultAsync(s => s.SurveyId == surveyUpdateDto.surveyId);

                if (existingSurveyDetails == null)
                {
                    throw new Exception($"SurveyDetails with SurveyId {surveyUpdateDto.surveyId} not found.");
                }
                var updatedSurveyDetails = surveyUpdateDto.surveyDetails;
                if (updatedSurveyDetails != null)
                {
                    if (!string.IsNullOrEmpty(updatedSurveyDetails.surveyName))
                        existingSurveyDetails.SurveyName = updatedSurveyDetails.surveyName;
                    var parsedStart = DateHelper.ParseDate(updatedSurveyDetails.surveyStart);
                    if (parsedStart != DateTime.MinValue)
                        existingSurveyDetails.SurveyStart = parsedStart;

                    var parsedEnd = DateHelper.ParseDate(updatedSurveyDetails.surveyEnd);
                    if (parsedEnd != DateTime.MinValue)
                        existingSurveyDetails.SurveyEnd = parsedEnd;
                    if (!string.IsNullOrEmpty(updatedSurveyDetails.surveyInstruction))
                        existingSurveyDetails.SurveyInstruction = updatedSurveyDetails.surveyInstruction;
                    if (!string.IsNullOrEmpty(updatedSurveyDetails.surveyConfirmation))
                        existingSurveyDetails.SurveyConfirmation = updatedSurveyDetails.surveyConfirmation;
                    if (updatedSurveyDetails.surveyView.HasValue)
                        existingSurveyDetails.SurveyView = updatedSurveyDetails.surveyView.Value;
                    if (updatedSurveyDetails.authView.HasValue)
                        existingSurveyDetails.AuthView = updatedSurveyDetails.authView.Value;
                    if (updatedSurveyDetails.plantId.HasValue)
                        existingSurveyDetails.PlantId = updatedSurveyDetails.plantId.Value;
                    if (updatedSurveyDetails.departmentId.HasValue)
                        existingSurveyDetails.DepartmentId = updatedSurveyDetails.departmentId.Value;

                    existingSurveyDetails.ModifiedBy = updatedSurveyDetails.modifiedBy;
                    existingSurveyDetails.ModifiedOn = updatedSurveyDetails.modifiedOn;
                }
                //var surveyAuthenticateUser = new List<SurveyAuthenticateUser>();
                var updateUserList = surveyUpdateDto.userList;
                if (updateUserList != null && updateUserList.Length > 0)
                {
                    var surveyAuthenticateUser = updateUserList
                        .Select(uid => new SurveyAuthenticateUser
                        {
                            SurveyId = surveyUpdateDto.surveyId,
                            UserId = uid,
                            PlantId = existingSurveyDetails.PlantId,
                            CreatedOn = DateTime.Now,
                            CreatedBy = surveyUpdateDto.surveyDetails.modifiedBy
                        })
                        .ToList();


                    var existingAuthUsers = _context.SurveyAuthenticateUsers
                        .Where(u => u.SurveyId == surveyUpdateDto.surveyId).ToList();

                    var updatedUserIds = surveyAuthenticateUser.Select(u => u.UserId).ToHashSet();

                    // Remove users not in the updated list
                    var usersToRemove = existingAuthUsers
                        .Where(u => !updatedUserIds.Contains(u.UserId)).ToList();
                    _context.SurveyAuthenticateUsers.RemoveRange(usersToRemove);

                    // Add new users that don't exist yet
                    var existingUserIds = existingAuthUsers.Select(u => u.UserId).ToHashSet();
                    var usersToAdd = surveyAuthenticateUser
                        .Where(u => !existingUserIds.Contains(u.UserId))
                        .ToList();
                    _context.SurveyAuthenticateUsers.AddRange(usersToAdd);
                }

                var updatedSurveyQuestion = surveyUpdateDto.surveyQuestion;
                if (updatedSurveyQuestion != null && updatedSurveyQuestion.questionText != null && updatedSurveyQuestion.questionText != "")
                {

                    var existingQuestion = await _context.SurveyQuestions
                        .FirstOrDefaultAsync(q => q.SurveyId == surveyUpdateDto.surveyId && q.QuestionId == updatedSurveyQuestion.questionId);

                    if (existingQuestion != null && existingQuestion.QuestionId == updatedSurveyQuestion.questionId)
                    {
                        // Update existing question fields
                        if (!string.IsNullOrEmpty(updatedSurveyQuestion.questionText))
                            existingQuestion.QuestionText = updatedSurveyQuestion.questionText;
                        if (updatedSurveyQuestion.questionType.HasValue)
                            existingQuestion.QuestionType = updatedSurveyQuestion.questionType;

                    }
                    else if (surveyUpdateDto.surveyQuestion.questionText != null && surveyUpdateDto.surveyQuestion.questionText != "")
                    {
                        int nextOrder = _context.SurveyQuestions
                            .Where(q => q.SurveyId == surveyUpdateDto.surveyId)
                            .Select(q => (int?)q.QuestionOrder) // cast to nullable to allow Max() to return null
                            .Max() ?? 0; // if null, fallback to 0

                        nextOrder += 1;
                        // Add new question if none exists
                        var AddSurveyQuestion = new SurveyQuestion
                        {
                            SurveyId = surveyUpdateDto.surveyId,
                            QuestionText = surveyUpdateDto.surveyQuestion.questionText,
                            QuestionType = surveyUpdateDto.surveyQuestion.questionType,
                            QuestionOrder = nextOrder, // Set order based on existing questions
                        };

                        _context.SurveyQuestions.Add(AddSurveyQuestion);
                        await _context.SaveChangesAsync();
                        newQuetionId = AddSurveyQuestion.QuestionId;
                        existingQuestion = AddSurveyQuestion;
                    }



                    var updatedQuestionTypes = surveyUpdateDto.surveyQuestion.options;
                    if (updatedQuestionTypes != null)
                    {


                        var existingOptions = _context.SurveyOptions
                            .Where(o => o.QuestionId == existingQuestion.QuestionId)
                            .ToList();

                        // Extract option texts from the updated list
                        var updatedOptionTexts = updatedQuestionTypes
                            .Select(opt => opt.optionText)
                            .ToHashSet();

                        // Find old options that are no longer present
                        var optionsToRemove = existingOptions
                            .Where(o => !updatedOptionTexts.Contains(o.OptionText))
                            .ToList();

                        _context.SurveyOptions.RemoveRange(optionsToRemove);



                        var existingOptionTexts = existingOptions.Select(o => o.OptionText).ToHashSet();

                        var optionsToAdd = updatedQuestionTypes
                            .Where(opt => !existingOptionTexts.Contains(opt.optionText))
                            .Select((optText, idx) => new SurveyOption
                            {
                                QuestionId = existingQuestion.QuestionId,
                                OptionText = optText.optionText,
                                OptionOrder = idx + 1,
                                Status = true,
                                Archive = false
                            }).ToList();

                        _context.SurveyOptions.AddRange(optionsToAdd);
                    }
                }
                if (newQuetionId != 0)
                {
                    // Convert `questionOrder` to a List<QuestionOrder> to allow adding new items.
                    var questionOrderList = surveyUpdateDto.questionOrder?.ToList() ?? new List<QuestionOrder>();

                    questionOrderList.Add(new QuestionOrder
                    {
                        questionId = newQuetionId
                    });

                    // Assign the updated list back to `surveyUpdateDto.questionOrder` as an array.
                    surveyUpdateDto.questionOrder = questionOrderList.ToArray();
                }
                var questionOrder = surveyUpdateDto.questionOrder;
                if (questionOrder != null)
                {
                    // 1. Extract all questionIds from the provided list
                    var providedQuestionIds = questionOrder.Select(q => q.questionId).ToList();

                    // 2. Get all existing questions for the given survey
                    int surveyId = questionOrder.FirstOrDefault()?.surveyId ?? 0;
                    var existingQuestions = await _context.SurveyQuestions
                        .Where(q => q.SurveyId == surveyId)
                        .ToListAsync();

                    // 3. Identify questions to remove (not in the provided list)
                    var questionsToRemove = existingQuestions
                        .Where(q => !providedQuestionIds.Contains(q.QuestionId))
                        .ToList();

                    // 4. If any questions are to be removed, also remove their related options
                    if (questionsToRemove.Any())
                    {
                        var questionIdsToRemove = questionsToRemove.Select(q => q.QuestionId).ToList();

                        var optionsToRemove = await _context.SurveyOptions
                            .Where(o => questionIdsToRemove.Contains(o.QuestionId.Value))
                            .ToListAsync();

                        if (optionsToRemove.Any())
                        {
                            _context.SurveyOptions.RemoveRange(optionsToRemove);
                        }

                        _context.SurveyQuestions.RemoveRange(questionsToRemove);
                    }

                    // 5. Update order of remaining questions based on input
                    int orderCounter = 0;
                    foreach (var order in questionOrder)
                    {
                        var existingQuestion = existingQuestions.FirstOrDefault(q => q.QuestionId == order.questionId);
                        if (existingQuestion != null)
                        {
                            existingQuestion.QuestionOrder = ++orderCounter;
                            _context.SurveyQuestions.Update(existingQuestion);
                        }
                    }

                    // 6. Save all changes
                    await _context.SaveChangesAsync();


                }
                // Save all changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("Survey details patched successfully.");

                //return existingSurveyDetails;
                return new SurveyUpdateDto
                {
                    surveyId = surveyUpdateDto.surveyId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to patch Survey: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<SurveyList> DeactivateSurvey(SurveyList surveyUpdateDto)
        {
            try
            {
                if (surveyUpdateDto.SurveyId == null)
                {
                    throw new ArgumentException("SurveyId cannot be null.");
                }
                var survey = await _context.SurveyDetail
                    .FirstOrDefaultAsync(s => s.SurveyId == surveyUpdateDto.SurveyId);

                if (survey == null)
                {
                    throw new Exception("Survey not found.");
                }

                if (surveyUpdateDto.Archieve.HasValue)
                {
                    survey.Archieve = surveyUpdateDto.Archieve.Value;
                }

                if (surveyUpdateDto.SurveyStatus.HasValue)
                {
                    survey.SurveyStatus = surveyUpdateDto.SurveyStatus.Value;
                }

                await _context.SaveChangesAsync();

                var updatedSurvey = new SurveyList
                {
                    Archieve = survey.Archieve,
                    SurveyStatus = survey.SurveyStatus
                    // add others as needed
                };

                return updatedSurvey;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deactivate survey: {ex.Message}", ex);
                throw new Exception($"Failed to deactivate survey: {ex.Message}");
            }
        }

        public async Task<int> DeleteQuestion(int quesitonId)
        {
            try
            {
                var question = await _context.SurveyQuestions.FirstOrDefaultAsync(q => q.QuestionId == quesitonId);
                if (question != null)
                {
                    var options = await _context.SurveyOptions
                        .Where(o => o.QuestionId == quesitonId)
                        .ToListAsync();
                    _context.SurveyOptions.RemoveRange(options);
                    _context.SurveyQuestions.Remove(question);
                    var result = await _context.SaveChangesAsync();
                    _logger.LogInformation("Question Deleted successfully.");
                    return result;
                }
                else
                {
                    _logger.LogWarning($"Question with ID {quesitonId} not found.");
                    return 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while deleting question");
                throw new Exception("An error occurred while deleting the question.", e);
            }
        }

        public async Task<SurveyQuestionResponse> SaveQuestionResponse(SurveyQuestionResponse questionResponse)
        {
            try
            {
                if (questionResponse != null)
                {
                    questionResponse.CreatedOn = DateTime.Now;

                    var issubmitted = await _context.SurveyAuthenticateUsers.Where(u => u.SurveyId == questionResponse.SurveyId && u.UserId == questionResponse.CreatedBy).Select(u => u.IsSubmitted).FirstOrDefaultAsync();

                    var result = await _context.SurveyAuthenticateUsers
                              .Where(s => s.SurveyId == questionResponse.SurveyId && s.UserId == questionResponse.CreatedBy)
                              .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsSubmitted, true)
                              .SetProperty(u => u.SubmittedDate, DateTime.Now));
                    if (result != 0 && issubmitted == false)
                    {
                        questionResponse.ResId = await _context.SurveyAuthenticateUsers.CountAsync(s => s.IsSubmitted == true);
                        _context.SurveyQuestionResponses.Add(questionResponse);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogWarning("User has already submitted the survey response.");
                        throw new InvalidOperationException("User has already submitted the survey response.");
                    }

                    _logger.LogInformation("Question response saved successfully.");
                    return questionResponse;
                    //throw new ArgumentNullException(nameof(questionResponse), "Question response cannot be null.");
                }
                else
                {
                    _logger.LogError("Question response is null.");
                    throw new ArgumentNullException(nameof(questionResponse), "Question response cannot be null.");
                }

            }
            catch
            (Exception ex)
            {
                _logger.LogError($"Failed to save question response: {ex.Message}", ex);
                throw new Exception($"Failed to save question response: {ex.Message}");
            }
        }


        public async Task<(List<GetSurveyUserList> Users, int TotalCount)> GetSurveyUserListsWithCount(int surveyId, bool isSubmitted, int pageNo, int pageSize)
        {
            var result = new List<GetSurveyUserList>();
            int totalCount = 0;

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_SurveyUserList";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@SurveyId", SqlDbType.Int) { Value = surveyId });
                        command.Parameters.Add(new SqlParameter("@IsSubmitted", SqlDbType.Bit) { Value = isSubmitted });
                        command.Parameters.Add(new SqlParameter("@PageNo", SqlDbType.Int) { Value = pageNo });
                        command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });


                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Advance to the correct result set
                            if (await reader.ReadAsync())
                            {
                                // Optionally read first result set manually, e.g. total count
                                totalCount = reader.GetInt32(0);
                            }

                            // Move to next result set (actual data)
                            if (await reader.NextResultAsync())
                            {
                                var dt = new DataTable();
                                dt.Load(reader);

                                result = dt.AsEnumerable().Select(row => new GetSurveyUserList
                                {
                                    EmpCode = row.Field<string>("EmpCode") ?? "",
                                    EmployeeName = row.Field<string>("EmployeeName") ?? "",
                                    Department = row.Field<string>("Department") ?? "",
                                    Designation = row.IsNull("Designation") ? "" : row.Field<string>("Designation"),
                                    Location = row.IsNull("Location") ? "" : row.Field<string>("Location"),
                                    Status = row.IsNull("Status") ? "" : row.Field<string>("Status"),
                                    SubmissionDateTime = row.IsNull("SubmissionDateTime") ? "" : row.Field<string>("SubmissionDateTime")
                                }).ToList();
                            }
                        }

                    }
                }

                _logger.LogInformation("Survey user list and count fetched successfully for SurveyId: {SurveyId}", surveyId);
                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user list and count for SurveyId: {SurveyId}", surveyId);
                throw;
            }
        }

        public async Task<PagedUsersSurveyResult> UserSurveyList(long userId, bool IsSubmitted, int pageNo, int pageSize)
        {
            var result = new PagedUsersSurveyResult();
            try
            {
                 result.Data = await _context.UsersSurveyLists
                .FromSqlRaw("EXEC SP_GetSurveyByUsers @UserId = {0}, @IsSubmitted = {1}, @PageNo = {2}, @PageSize = {3}",
                            userId, IsSubmitted, pageNo, pageSize)
                .ToListAsync();
                var total = await _context.Set<resultCount>()
                    .FromSqlRaw(@"SELECT COUNT(*) AS TotalCount FROM SurveyAuthenticateUsers SAU LEFT JOIN SurveyDetail SD ON SAU.SurveyId =SD.SurveyId WHERE SAU.UserId = {0} AND SAU.IsSubmitted = {1} AND SD.SurveyStatus = 1 AND SD.Archieve = 0",  userId, IsSubmitted)
                    .FirstOrDefaultAsync();
                resultCount totalCount = total ?? new resultCount { TotalCount = 0 };
                result.TotalCount = totalCount.TotalCount ?? 0;
                _logger.LogInformation("User survey list fetched successfully for UserId: {UserId}", userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user survey list for UserId: {UserId}", userId);
                throw new Exception($"Failed to fetch user survey list: {ex.Message}");
            }
        }

        public async Task<SurveyResponse?> GetSurveyFlatResult(int surveyId, long userId)
        {
            var flatResults = await _context.SurveyFlatResults
                .FromSqlRaw("Exec SP_GetSurveyQuestionsWithOptions @SurveyId={0}, @UserId={1}", surveyId, userId)
                .ToListAsync();

            var surveyResponse = flatResults
                .GroupBy(fr => fr.SurveyId)
                .Select(surveyGroup =>
                {
                    var firstSurvey = surveyGroup.First();

                    return new SurveyResponse
                    {
                        SurveyId = firstSurvey.SurveyId,
                        SurveyName = firstSurvey.SurveyName,
                        SurveyInstruction = firstSurvey.SurveyInstruction,
                        SurveyView = firstSurvey.SurveyView,
                        Questions = surveyGroup
                            .GroupBy(q => new { q.QuestionId, q.QuestionText, q.QuestionOrder, q.QuestionType })
                            .Select(questionGroup => new QuestionResponseU
                            {
                                QuestionId = questionGroup.Key.QuestionId,
                                QuestionText = questionGroup.Key.QuestionText,
                                QuestionType = questionGroup.Key.QuestionType,
                                QuestionOrder = questionGroup.Key.QuestionOrder ?? 0,
                                Options = questionGroup
                                    .Where(o => o.OptionId.HasValue)
                                    .GroupBy(o => new { o.OptionId, o.OptionText, o.OptionOrder })
                                    .Select(optionGroup =>
                                    {
                                        var firstOption = optionGroup.First();

                                        return new OptionResponse
                                        {
                                            OptionId = firstOption.OptionId.Value,
                                            OptionText = firstOption.OptionText,
                                            OptionOrder = firstOption.OptionOrder ?? 0,
                                            Response = firstOption.ResponseText,
                                            boolResponse = string.Equals(firstOption.ResponseText, "True", StringComparison.OrdinalIgnoreCase)

                                        };
                                    }).ToList()
                            }).ToList()
                    };
                })
                .FirstOrDefault();



            return surveyResponse;
        }

        public async Task<SaveSurveyResponse> SaveSurveyFlatResultByUserIdAsync(SaveSurveyResponse surveyResponse, long userId)
        {
            var surveyId = surveyResponse.SurveyId;
            if (surveyResponse.Questions.Count > 0)
            {
                foreach (var question in surveyResponse.Questions)
                {
                    var questionId = question.QuestionId;

                    foreach (var option in question.Options)
                    {
                        var optionId = option.OptionId;

                        if ((question.QuestionType == 2) || (option.Response != null && !string.IsNullOrEmpty(option.Response)))
                        {
                            var useris = await _context.SurveyAuthenticateUsers.Where(s => s.SurveyId == surveyId && s.UserId == userId).FirstOrDefaultAsync();
                            if (useris == null)
                            {
                                _logger.LogWarning($"User with ID {userId} is not authenticated for SurveyId {surveyId}.");
                                throw new InvalidOperationException($"User with ID {userId} is not authenticated for SurveyId {surveyId}.");
                            }
                            var questionResponse = new SurveyQuestionResponse
                            {
                                SurveyId = surveyId,
                                QuestionId = questionId,
                                OptionId = optionId,
                                ResponseText = question.QuestionType == 2
                                ? option.boolResponse.ToString()
                                : option.Response,
                                //ResponseText = option.Response,
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now,
                                ResponseStatus = true
                            };

                            _context.SurveyQuestionResponses.Add(questionResponse);
                        }
                        else
                        {
                            _logger.LogWarning($"No response provided for QuestionId {questionId} and OptionId {optionId} for UserId {userId}.");
                        }
                    }
                }

                await _context.SaveChangesAsync();

                var entity = await _context.SurveyAuthenticateUsers.FirstOrDefaultAsync(e => e.SurveyId == surveyId && e.UserId == userId);
                if (entity != null)
                {

                    entity.IsSubmitted = true;
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation($"Survey response saved successfully for UserId: {userId} and SurveyId: {surveyId}.");
                return surveyResponse;
            }
            else
            {
                _logger.LogWarning($"No questions found for SurveyId: {surveyId}.");
                throw new InvalidOperationException($"No questions found for SurveyId: {surveyId}.");
            }
        }

        public async Task<List<SurveyResult>> GetSurveyResult(int surveyId)
        {
            try
            {
                var result = await _context.SurveyResults.FromSqlRaw("EXEC SP_GetSurveyResult @SurveyId = {0}", surveyId).ToListAsync();
                _logger.LogInformation($"Survey result fetched successfully for SurveyId: {surveyId}.");
                return result;
            }
            catch (Exception exx)
            {
                _logger.LogError($"Failed to fetch survey result for SurveyId: {surveyId}. Error: {exx.Message}");
                throw new Exception($"Failed to fetch survey result: {exx.Message}");
            }

        }

        public async Task<bool> ActivateSurveyForUsers(int surveyId, bool surveyStatus)
        {
            var survey = await _context.SurveyDetail.FirstOrDefaultAsync(s => s.SurveyId == surveyId);

            if (survey != null)
            {
                survey.SurveyStatus = surveyStatus;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Survey with ID {surveyId} status updated to {(surveyStatus ? "Active" : "Inactive")}.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Survey with ID {surveyId} not found for activation.");
                return false;
            }
        }

        //public Task UserSurveyList(long? v, bool isSubmitted, int pageNo, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
