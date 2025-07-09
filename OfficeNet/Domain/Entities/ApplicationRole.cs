using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    [Table("Roles")] 
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        [MaxLength(100)]
        public string? Description { get; set; }

        public bool IsSystemRole { get; set; } = false;

        public bool Status { get; set; } = true;

        public long? CreatedBy { get; set; } 
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
    }
}
