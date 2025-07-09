using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class DiscussionTopic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TopicId { get; set; }
        public int EmpId { get; set; }
        [MaxLength(50)]
        public string? Subject { get; set; }
        [MaxLength(15)]
        public string? IP { get; set; }
        //[JsonConverter(typeof(DdMmYyyyDateConverter))]
        //public DateTime? Date { get; set; }
        [MaxLength(100)]
        public string? Message { get; set; }
        public int? Replies { get; set; }
        public int Views { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsPublic { get; set; }
        public int? ViewGroupId { get; set; }
        public int? PlantId { get; set; }
        public string? DiscussionFiles { get; set; }
        public bool? IsApproved { get; set; }
        [MaxLength(20)]
        public string? DiscussionType { get; set; }
        public bool? CloseTopic { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }

        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "date")]
        public DateTime? ToDate { get; set; }
        
        public long? CreatedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
    }
}
