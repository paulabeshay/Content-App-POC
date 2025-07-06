using System;
using System.ComponentModel.DataAnnotations;

namespace Content_App_POC.CommentsMgt
{
    public class CommentStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = "System";
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        [MaxLength(100)]
        public string ModifiedBy { get; set; } = "System";
        [Required]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsDeleted { get; set; } = false;
    }

    public enum CommentStatusEnum
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
} 