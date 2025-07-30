using System.ComponentModel.DataAnnotations;

namespace OfficeNet.Domain.Contracts
{
    public class HelpdeskSubcategoryDto
    {
        public int SubCategoryID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        [MaxLength(200)]
        public string SubCategoryName { get; set; }
        public bool Status { get; set; }
    }
}
