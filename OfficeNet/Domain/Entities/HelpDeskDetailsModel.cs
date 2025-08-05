using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Entities
{
    public class HelpDeskDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IssueID { get; set; }
        public int DeptID { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int EscalationUserId { get; set; }
        //public string AdminUserID { get; set; }
        public int EscalationOneUserID { get; set; }
        [MaxLength(200)]
        public string EscalationOneTime { get; set; }
        public int EscalationTwoUserID { get; set; }
        [MaxLength(200)]
        public string EscalationTwoTime { get; set; }
        public int? EscalationThree_UserID { get; set; }
        [MaxLength(200)]
        public string? EscalationThreeTime { get; set; }
        public int PlantID { get; set; }
        public int? CompanyId { get; set; }
        public bool? Status { get; set; }
        public int? TentativeClosureHour { get; set; }
        public int? TentativeClosureDay { get; set; }
        public bool? IsUploadExcel { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        [JsonConverter(typeof(DdMmYyyyDateConverter))]
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public virtual ICollection<HelpdeskAdminUser>? AdminUsers { get; set; }

    }

    public class HelpdeskAdminUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HdeskAdminId { get; set; }

        [Required]
        public int IssueID { get; set; }

        [Required]
        public long AdminUserID { get; set; }

        [ForeignKey(nameof(IssueID))]
        public virtual HelpDeskDetailModel HelpDeskDetail { get; set; }

        [ForeignKey(nameof(AdminUserID))]
        public virtual ApplicationUser AdminUser { get; set; }
    }
}
