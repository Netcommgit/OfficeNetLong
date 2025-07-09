using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeNet.Domain.Entities
{
    public class DiscussionGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DGId { get; set; }
        [MaxLength(50)]
        public string? GroupName { get; set; }
        public bool Status { get; set; }
        public bool Archieve { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
    }

    [Keyless] 
    public class DiscussionUser
    {
        public int GroupId { get; set; }
        public int? UserId { get; set; }
    }

}
