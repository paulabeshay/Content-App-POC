using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Content_App_POC.CommentsMgt
{
    public interface ICommentService
    {
        Task<Comment?> GetCommentByIdAsync(int id);
        Task<IEnumerable<Comment>> GetCommentsByContentIdAsync(int contentId);
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int id);
        Task SetApprovalRecursiveAsync(int commentId, bool isApproved, string modifiedBy);
        Task SetDeletedRecursiveAsync(int commentId, string modifiedBy);
    }
} 