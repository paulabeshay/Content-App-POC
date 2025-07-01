using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Content_App_POC.CommentsMgt
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Comment>> GetByContentIdAsync(int contentId);
        Task<IEnumerable<Comment>> GetAllAsync();
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Guid id);
    }
} 