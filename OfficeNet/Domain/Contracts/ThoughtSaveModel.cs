using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Contracts
{
    public class ThoughtSaveModel
    {
        public int ThoughtID { get; set; }
        public string Quote { get; set; }
        public string? Author { get; set; }
        [Column(TypeName = "datetime")]
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        public DateTime? ActDate { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
