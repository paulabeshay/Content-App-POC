using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content_App_POC.CommentsMgt
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentsMgtContext _context;
        public CommentRepository(CommentsMgtContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetByContentIdAsync(int contentId)
        {
            return await _context.Comments
                .Where(c => c.ContentId == contentId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsDeleted = true;
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetApprovalRecursiveAsync(int commentId, bool isApproved, string modifiedBy)
        {
            var toUpdate = await _context.Comments
                .Where(c => c.Id == commentId && !c.IsDeleted)
                .ToListAsync();
            var children = await _context.Comments
                .Where(c => c.ParentCommentId == commentId && !c.IsDeleted)
                .ToListAsync();
            while (children.Any())
            {
                toUpdate.AddRange(children);
                var nextIds = children.Select(c => c.Id).ToList();
                children = await _context.Comments
                    .Where(c => c.ParentCommentId != null && nextIds.Contains((int)c.ParentCommentId) && !c.IsDeleted)
                    .ToListAsync();
            }
            foreach (var comment in toUpdate)
            {
                comment.IsApproved = isApproved;
                comment.ModifiedBy = modifiedBy;
                comment.ModifiedOn = DateTime.UtcNow;
                _context.Comments.Update(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task SetDeletedRecursiveAsync(int commentId, string modifiedBy)
        {
            var toUpdate = await _context.Comments
                .Where(c => c.Id == commentId && !c.IsDeleted)
                .ToListAsync();
            var children = await _context.Comments
                .Where(c => c.ParentCommentId == commentId && !c.IsDeleted)
                .ToListAsync();
            while (children.Any())
            {
                toUpdate.AddRange(children);
                var nextIds = children.Select(c => c.Id).ToList();
                children = await _context.Comments
                    .Where(c => c.ParentCommentId != null && nextIds.Contains((int)c.ParentCommentId) && !c.IsDeleted)
                    .ToListAsync();
            }
            foreach (var comment in toUpdate)
            {
                comment.IsDeleted = true;
                comment.ModifiedBy = modifiedBy;
                comment.ModifiedOn = DateTime.UtcNow;
                _context.Comments.Update(comment);
            }
            await _context.SaveChangesAsync();
        }
    }
} 