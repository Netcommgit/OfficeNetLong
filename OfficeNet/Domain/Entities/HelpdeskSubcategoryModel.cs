using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class HelpdeskSubcategoryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCategoryID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        [MaxLength(200)]
        public string SubCategoryName { get; set; }
        public bool Status { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        [ForeignKey("CategoryID")]
        public virtual HelpdeskCategoryModel Category { get; set; }
    }
}
