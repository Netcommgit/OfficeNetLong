using OfficeNet.Domain.Entities;

namespace OfficeNet.Domain.Contracts
{
    public class SurveyUpdateDto
    {
        public int surveyId { get; set; }
        public SurveyDetailsUpdateDto? surveyDetails { get; set; }
        public SurveyQuestionUpdtaeDto? surveyQuestion { get; set; }
        public long[]? userList { get; set; }
        public QuestionOrder[]? questionOrder { get; set; }
    }

    public class SurveyDetailsUpdateDto
    {
        public string? surveyName { get; set; }
        public string? surveyStart { get; set; }
        public string? surveyEnd { get; set; }
        public string? surveyInstruction { get; set; }
        public string? surveyConfirmation { get; set; }
        public int? surveyView { get; set; }
        public int? authView { get; set; }
        public int? plantId { get; set; }
        public int? departmentId { get; set; }
        public long? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
    }

    public class SurveyQuestionUpdtaeDto
    {
        public string? questionText { get; set; }
        public int? questionType { get; set; }
        public int? serial { get; set; }
        public int? questionId { get; set; }
        public int? surveyId { get; set; }
        public bool? questionRequierd { get; set; }
        public string? questionErrorMsg { get; set; }
        public int? questionViewReport { get; set; }
        public int? questionOrder { get; set; }
        public bool? status { get; set; }
        public bool? archieve { get; set; }
        public Option[]? options { get; set; }
    }
    public class Option
    {
        public int? optionId { get; set; }
        public string? optionText { get; set; }
        public int? optionOrder { get; set; }
        public bool? status { get; set; }
        public bool? archive { get; set; }
    }

    public class QuestionOrder
    {
        public int? questionId { get; set; }
        public int? surveyId { get; set; }
        public string questionText { get; set; }
        public bool? questionRequierd { get; set; }
        public string? questionErrorMsg { get; set; }
        public int? questionViewReport { get; set; }
        public int questionType { get; set; }
        public int? questionOrder { get; set; }
        public bool? status { get; set; }
        public bool? archieve { get; set; }
        public OptionOrder[]? options { get; set; }
    }

    public class OptionOrder
    {
        public int? optionId { get; set; }
        public string? optionText { get; set; }
        public int? optionOrder { get; set; }
        public bool? status { get; set; }
        public bool? archive { get; set; }
    }

}


