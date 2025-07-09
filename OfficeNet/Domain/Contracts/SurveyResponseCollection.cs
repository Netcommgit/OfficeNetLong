using OfficeNet.Infrastructure.Mapping;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Contracts
{
    public class SurveyResponse
    {
        public int SurveyId { get; set; }
        public string? SurveyName { get; set; }
        public string? SurveyInstruction { get; set; }
        public int? SurveyView { get; set; }
        public List<QuestionResponseU> Questions { get; set; } = new();
    }

    public class QuestionResponseU
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public int? QuestionType { get; set; }
        public int QuestionOrder { get; set; }
        public List<OptionResponse> Options { get; set; } = new();
    }

    public class OptionResponse
    {
        public int OptionId { get; set; }
        public string? OptionText { get; set; }
        public int OptionOrder { get; set; }
        //public Response? Response { get; set; }
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


    public class SurveyFlatResult
    {
        public int SurveyId { get; set; }
        public string? SurveyName { get; set; }
        public string? SurveyInstruction { get; set; }
        public int? SurveyView { get; set; }

        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public int? QuestionType { get; set; }
        public int? QuestionOrder { get; set; }

        public int? OptionId { get; set; }
        public string? OptionText { get; set; }
        public int? OptionOrder { get; set; }

        public int? ResponseId { get; set; }
        public string? ResponseText { get; set; }
        public string? CreatedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        public DateTime? CreatedOn { get; set; }
    }


}
