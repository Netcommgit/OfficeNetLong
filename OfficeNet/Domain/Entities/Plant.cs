using OfficeNet.Infrastructure.Mapping;
using OfficeNet.Service;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string PlantName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? PlantDescription { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? SAPCode { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public bool Status { get; set; }

    }
}
