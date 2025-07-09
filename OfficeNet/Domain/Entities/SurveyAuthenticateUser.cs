using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class SurveyAuthenticateUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int SurveyId { get; set; }           
        public long UserId { get; set; }             
        public int? PlantId { get; set; }            
        public bool? IsSubmitted { get; set; } = false;
        [Column(TypeName = "datetime")]
        public DateTime? SubmittedDate { get; set; }
        public int? AuthType { get; set; }           
        public bool? Archive { get; set; }           
        public long? CreatedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;   
        public long? ModifiedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now; 
    }
}
