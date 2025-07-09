using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace OfficeNet.Domain.Entities
{
    public class DiscussionMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }         
        public int EmpId { get; set; }
        [MaxLength(100)]
        public string? Subject { get; set; }
        [MaxLength(15)]
        public string? IP { get; set; }
        [MaxLength(50)]
        public string? Message { get; set; }            
        public int TopicId { get; set; }           
        public int? ParentId { get; set; }
        [MaxLength(50)]
        public string? Position { get; set; }           
        public int? Indent { get; set; }                
        public bool IsApprovedStatus { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long?  ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
