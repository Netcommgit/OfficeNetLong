using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
//using OfficeNet.Migrations;
using System.ComponentModel;

namespace OfficeNet.Service.Survey
{
    public interface ISurveyDetailsService
    {
        Task<SurveyDetails> SaveSurveyDetailsAsync(SurveyDetails surveyDetails, List<long> userList,SurveyQuestion surveyQuestion, List<string> questionType);
        Task<SurveyAuthenticateUser> CreateSurveyAuthenticateUserAsync(SurveyAuthenticateUser surveyAuthenticateUser);
        Task<PagedSurveyResult> GetSurveyListAsync(int pageNo, int pageSize);
        Task<SurveyDetailWithUserDto> GetSurveyDetailById(int surveyId);
        Task<List<SurveyQuestionDto>> GetQuestionById(int surveyId);
        Task<SurveyUpdateDto> UpdateSurveyDetailsAsync(SurveyUpdateDto surveyUpdateDto);
        Task<SurveyList> DeactivateSurvey(SurveyList surveyUpdateDto);
        Task<int> DeleteQuestion(int quesitonId);
        Task<SurveyQuestionResponse> SaveQuestionResponse(SurveyQuestionResponse questionResponse);
        Task<(List<GetSurveyUserList> Users, int TotalCount)> GetSurveyUserListsWithCount(int surveyId, bool isSubmitted, int pageNo, int pageSize);
        Task<PagedUsersSurveyResult> UserSurveyList(long userId,bool IsSubmitted, int pageNo, int pageSize);
        Task <SurveyResponse> GetSurveyFlatResult(int surveyId, long userId);
        Task<SaveSurveyResponse> SaveSurveyFlatResultByUserIdAsync(SaveSurveyResponse surveyResponse, long userId);
        Task<List<SurveyResult>> GetSurveyResult(int surveyId);
        Task<bool> ActivateSurveyForUsers(int surveyId, bool surveyStatus);
        //Task UserSurveyList(long? v, bool isSubmitted, int pageNo, int pageSize);
    }
}
