using System.ComponentModel.DataAnnotations;

namespace OfficeNet.Domain.Contracts
{
    public class HelpdeskDepartmentDto
    {
        public int DeptID { get; set; }

        [Required]
        [MaxLength(100)] 
        public string DeptName { get; set; }

        [MaxLength(50)] 
        public string DeptExtension { get; set; }

        public bool Status { get; set; }
    }
}
