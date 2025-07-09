//using OfficeNet.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class OpinionPollOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int PollOptionId { get;set; }
        public int? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        [JsonIgnore]
        public OpinionPollTopic? OpinionPollTopic { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? OptionName { get; set; }
    }

    //public class OpinionPollOptionCreateDto
    //{
    //    public string OptionName { get; set; }
    //    public int? QuestionId { get; set; }
    //}

}
