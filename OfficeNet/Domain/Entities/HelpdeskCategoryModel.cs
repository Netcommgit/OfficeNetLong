using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class HelpdeskCategoryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        public int DeptID { get; set; } 
        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }
        public bool Status { get; set; } = true;
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } 
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [ForeignKey("DeptID")]
        public HelpdeskDepartmentModel HelpdeskDepartment { get; set; }
    }
}
