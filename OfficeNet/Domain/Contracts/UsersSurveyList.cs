//using OfficeNet.Migrations;

namespace OfficeNet.Domain.Contracts
{
    public class UsersSurveyList
    {
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string SurveyStart { get; set; }
        public string SurveyEnd { get; set; }
        public string? SubmittedDate { get; set; }

    }

    public class PagedUsersSurveyResult
    {
        public List<UsersSurveyList> Data { get; set; } = new();
        public int TotalCount { get; set; }

    }
}
