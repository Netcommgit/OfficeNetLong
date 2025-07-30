using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class HelpdeskDepartmentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeptID { get; set; }

        [Required]
        [MaxLength(100)] // adjust length as per your database schema
        public string DeptName { get; set; }

        [MaxLength(50)] // adjust as needed
        public string DeptExtension { get; set; }

        public bool Status { get; set; }

        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
    }
}
