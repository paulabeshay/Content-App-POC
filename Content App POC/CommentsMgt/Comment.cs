using System;
using System.ComponentModel.DataAnnotations;

namespace Content_App_POC.CommentsMgt
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        //[MaxLength(100)]
        public int ContentId { get; set; }
        [Required]
        public string CommentText { get; set; } = string.Empty;
        [Required]
        public bool IsApproved { get; set; } = false;
        public int? ParentCommentId { get; set; }
        [Required]
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        [MaxLength(50)]
        public string ModifiedBy { get; set; } = string.Empty;
        [Required]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsDeleted { get; set; } = false;
    }
} 