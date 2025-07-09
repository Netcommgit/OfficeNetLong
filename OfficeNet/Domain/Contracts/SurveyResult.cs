namespace OfficeNet.Domain.Contracts
{
    public class SurveyResult
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public int AnsweredCount { get; set; }
        public int TotalAssignedUsers { get; set; }
        public decimal ResponsePercentage { get; set; }
    }
}
