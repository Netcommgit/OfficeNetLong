namespace OfficeNet.Domain.Contracts
{

    public class SurveyUserList
    {
        //public int Id { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class SurveyDetailWithUserDto
    {
        public int SurveyId { get; set; }
        public string? SurveyName { get; set; }
        public DateTime? SurveyStart { get; set; }
        public DateTime? SurveyEnd { get; set; }
        public string? SurveyInstruction { get; set; }
        public string? SurveyConfirmation { get; set; }
        public int? SurveyView { get; set; }
        public int AuthView { get; set; }
        public int? PlantId { get; set; }
        public int? DepartmentId { get; set; }
        //public bool IsExcel { get; set; }
        //public bool SurveyStatus { get; set; }
        //public bool Archieve { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public string? ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        public List<SurveyUserList> userLists { get; set; }
    }
}
