using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Content_App_POC.CommentsMgt
{
    public interface ICommentService
    {
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsByContentIdAsync(int contentId);
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Guid id);
        Task SetStatusRecursiveAsync(Guid commentId, CommentStatusEnum status, string modifiedBy);
        Task SetDeletedRecursiveAsync(Guid commentId, string modifiedBy);
    }
} 