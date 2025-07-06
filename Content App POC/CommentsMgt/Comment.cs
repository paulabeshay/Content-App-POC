using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content_App_POC.CommentsMgt
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int ContentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ContentParentAlias { get; set; } = string.Empty;
        [Required]
        public string CommentText { get; set; } = string.Empty;
        [Required]
        public int CommentStatusId { get; set; }
        [ForeignKey("CommentStatusId")]
        public CommentStatus? CommentStatus { get; set; }
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Comment? ParentComment { get; set; }
        [Required]
        public bool ShownInPortal { get; set; } = false;
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "Anonymous";
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        [MaxLength(100)]
        public string ModifiedBy { get; set; } = "Anonymous";
        [Required]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsDeleted { get; set; } = false;
    }
} 