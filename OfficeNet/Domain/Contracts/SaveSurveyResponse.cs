using OfficeNet.Infrastructure.Mapping;

namespace OfficeNet.Domain.Contracts
{
    public class SaveSurveyResponse
    {
        public int SurveyId { get; set; }
        public string? SurveyName { get; set; }
        public string? SurveyInstruction { get; set; }
        public int? SurveyView { get; set; }
        public List<SaveQuestionResponse> Questions { get; set; } = new();
    }

    public class SaveQuestionResponse
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public int? QuestionType { get; set; }
        public int QuestionOrder { get; set; }
        public List<SaveOptionResponse> Options { get; set; } = new();
    }

    public class SaveOptionResponse
    {
        public int OptionId { get; set; }
        public string? OptionText { get; set; }
        public int OptionOrder { get; set; }
        public string? Response { get; set; }
        public bool boolResponse { get; set; } = false;
    }

    //public class Response
    //{
    //    public int? ResponseId { get; set; }
    //    public string? ResponseText { get; set; }
    //    public string? CreatedBy { get; set; }
    //    [JsonConverter(typeof(DdMmYyyyDateConverter))]
    //    public DateTime? CreatedOn { get; set; }
    //}
}
