using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class SurveyDetails
    {
        [Key]
        public int SurveyId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? SurveyName { get; set; }
        
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "date")]
        public DateTime? SurveyStart { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "date")]
        public DateTime? SurveyEnd { get; set; }
        [MaxLength(50)]
        public string? SurveyInstruction { get; set; }
        [MaxLength(50)]
        public string? SurveyConfirmation {get;set;}
        public int? SurveyView {get;set;}
        public int AuthView { get;set;}
        public int? PlantId { get; set; }
        public int? DepartmentId {get;set;}
        public bool IsExcel { get; set; }
        public bool SurveyStatus { get; set; }
        public bool Archieve { get; set; }
        public long? CreatedBy { get;set;}
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get;set;}
        public long? ModifiedBy { get;set;}
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
       
    }
}
