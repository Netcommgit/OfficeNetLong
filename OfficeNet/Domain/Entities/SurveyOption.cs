using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class SurveyOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OptionId { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual SurveyQuestion Question { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string OptionText{get;set;}
        public int OptionOrder {  get; set; }
        public bool?Status { get; set; }
        public bool? Archive { get; set; }
    }
}
