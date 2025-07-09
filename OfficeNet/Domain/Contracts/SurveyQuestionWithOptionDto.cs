namespace OfficeNet.Domain.Contracts
{
    public class SurveyOptionDto
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public int OptionOrder { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
    }

    public class SurveyQuestionDto
    {
        public int QuestionId { get; set; }
        public int? SurveyId { get; set; }
        public string QuestionText { get; set; }
        //public bool? QuestionRequierd { get; set; }
        //public string? QuestionErrorMsg { get; set; }
        //public int? QuestionViewReport { get; set; }
        public int? QuestionType { get; set; }
        //public int? QuestionOrder { get; set; }
        //public bool? Status { get; set; }
        //public bool? Archieve { get; set; }
        public List<SurveyOptionDto> Options { get; set; }
    }

}
