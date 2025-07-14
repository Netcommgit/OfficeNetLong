using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Contracts
{
    public class DepartmentDto
    {
        public int DeptID { get; set; }

        [Required]
        [MaxLength(100)]
        public string DeptName { get; set; }
        public bool Status { get; set; }
        public bool? Archive { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(40)]
        public string? SAPCode { get; set; }
        public int? GroupID { get; set; }
    }
}
