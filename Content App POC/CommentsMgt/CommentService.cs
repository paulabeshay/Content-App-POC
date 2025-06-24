using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Content_App_POC.CommentsMgt
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;
        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByContentIdAsync(int contentId)
        {
            return await _repository.GetByContentIdAsync(contentId);
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _repository.AddAsync(comment);
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            await _repository.UpdateAsync(comment);
        }

        public async Task DeleteCommentAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
} 