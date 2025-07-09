using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OfficeNet.Domain.Entities
{
    public class UsersDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
