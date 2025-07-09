namespace OfficeNet.Domain.Entities
{
    public class ServiceSaveRequest
    {
        public SurveyDetails SurveyDetails { get; set; }
        public List<long> UserList { get; set; }
        public SurveyQuestion surveyQuestion { get; set; }
        public List<string> questionType { get; set; }
    }
}
