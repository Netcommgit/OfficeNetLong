using System.ComponentModel.DataAnnotations;

namespace OfficeNet.Domain.Contracts
{
    public class HelpdeskCategoryDto
    {
        public int CategoryID { get; set; }
        [Required]
        public int DeptID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public bool Status { get; set; } = true;
    }
}
