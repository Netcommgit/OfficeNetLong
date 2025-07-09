using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class SurveyQuestionResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResponseId { get; set; }
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string ResponseText { get; set; }
        public int? ResId { get; set; }
        public long CreatedBy { get; set; }   
        public DateTime? CreatedOn { get; set; }
        public bool ResponseStatus { get; set; } = true;
        public bool? Archieve { get; set; }

    }
}
