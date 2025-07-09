using Microsoft.EntityFrameworkCore;

namespace OfficeNet.Domain.Contracts
{
    public class SurveyList
    {
        public long? SerialNo { get; set; }
        public int? SurveyId { get; set; }
        public string? SurveyName { get; set; }
        public DateTime? SurveyStart { get; set; }
        public DateTime? SurveyEnd { get; set; }
        public string? SurveyInstruction { get; set; }
        public string? SurveyConfirmation { get; set; }
        public int? Total { get; set; }
        public int? Respondent { get; set; }
        public int? NotRespondent { get; set; }
        public bool? SurveyStatus { get; set; }
        public bool? Archieve { get; set; }

    }

    [Keyless]
    public class resultCount
    {
        public int? TotalCount { get; set; }
    }

    public class PagedSurveyResult
    {
        public List<SurveyList> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }

}
