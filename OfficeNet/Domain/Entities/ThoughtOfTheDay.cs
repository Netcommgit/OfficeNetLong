using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class ThoughtOfDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThoughtID { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Quote { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Author { get; set; }
        [Column(TypeName = "datetime")]
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        public DateTime? ActDate { get; set; }
        [Column(TypeName = "datetime")]
        //[JsonConverter(typeof(DdMmYyyyDateConverter))]
        public DateTime? CreatedOn { get; set; }
        public long? CratedBy { get; set; }
        //[JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public bool? IsActive { get; set; } = true;

    }
}
