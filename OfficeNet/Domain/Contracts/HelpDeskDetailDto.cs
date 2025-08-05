using System.ComponentModel.DataAnnotations;

namespace OfficeNet.Domain.Contracts
{
    public class HelpDeskDetailDto
    {
        public int IssueID { get; set; }
        public int DeptID { get; set; }
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int EscalationUserId { get; set; }
        //public string AdminUserID { get; set; }
        public int EscalationOneUserID { get; set; }
        public string EscalationOneTime { get; set; }
        public int EscalationTwoUserID { get; set; }
        public string EscalationTwoTime { get; set; }
        public int? EscalationThree_UserID { get; set; }
        public string? EscalationThreeTime { get; set; }
        public int PlantID { get; set; }
        public int? CompanyId { get; set; }
        public bool? Status { get; set; }
        public int? TentativeClosureHour { get; set; }
        public int? TentativeClosureDay { get; set; }
        public bool? IsUploadExcel { get; set; }
        public List<HelpdeskAdminAssignmentDto>? AdminUsers { get; set; }
    }


    public class HelpdeskAdminAssignmentDto
    {
        [Required]
        public int IssueID { get; set; }

        [Required]
        public long AdminUserID { get; set; }
    }

}
