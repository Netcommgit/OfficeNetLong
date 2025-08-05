using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class HelpdeskUserDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ReqID { get; set; }
        public int? UserID { get; set; }
        [MaxLength(100)]
        public string? TicketNo { get; set; }
        [MaxLength(100)]
        public string? RefTicketNo { get; set; }
        public int IssueID { get; set; }
        [MaxLength(200)]
        public string? Issue { get; set; }
        [MaxLength(255)]
        public string? Attachment { get; set; }
        [MaxLength(20)]
        public string? MobileNo { get; set; }
        public int? EscalationStatus { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedBy { get; set; }
        [MaxLength(255)]
        public string? Solution { get; set; }
        [MaxLength(255)]
        public string? AppoverAttachment { get; set; }
        public DateTime? CreateDt { get; set; }
        [MaxLength(100)]
        public string? UserRemark { get; set; }
        [MaxLength(255)]
        public string? UserReAttachment { get; set; }
        public DateTime? UserDate { get; set; }
        public int OldReqId { get; set; }
        [MaxLength(100)]
        public string? ReAssignRemark { get; set; }
        public bool? Is_Satisfied { get; set; }
        public bool? isActive { get; set; }
        public int ReminderMailStatus { get; set; }
        public DateTime? FrstReminderDate { get; set; }
        public DateTime? ScndReminderDate { get; set; }
        public DateTime? ThrdReminderDate { get; set; }
        [MaxLength(100)]
        public string? AssetCode { get; set; }
        public DateTime? ExpectedCompetionTime { get; set; }
        public int? WIPUserID { get; set; }
        public int? WIPTot { get; set; }
        [MaxLength(50)]
        public string? UserPriority { get; set; }
        [MaxLength(50)]
        public string? AdminPriority { get; set; }
        public DateTime? TentativeClosureDate { get; set; }
        [MaxLength(50)]
        public string? CompletionTime { get; set; }
        public int? TentativeClosureDay { get; set; }
        public int? TentativeClosureHour { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

}
