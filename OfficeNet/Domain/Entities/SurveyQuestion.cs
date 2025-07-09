using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using OfficeNet.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace OfficeNet.Domain.Entities
{
    public class SurveyQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }
        public int? SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        [ValidateNever]
        public virtual SurveyDetails Survey { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string QuestionText {  get; set; }
        public bool? QuestionRequierd {  get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? QuestionErrorMsg { get; set; }
        public int? QuestionViewReport {  get; set; }
        public int? QuestionType     { get;set; }
        public int? QuestionOrder { get; set; }
        public bool? Status { get; set; }
        public bool? Archieve {  get; set; }
    }
}
