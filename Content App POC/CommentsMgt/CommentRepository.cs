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
            // Enforce shownInPortal = false if status is Pending or Rejected
            if (comment.CommentStatusId == (int)CommentStatusesEnum.Pending || comment.CommentStatusId == (int)CommentStatusesEnum.Rejected)
            {
                comment.ShownInPortal = false;
            }
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

        public async Task UpdateCommentStatusAsync(Guid commentId, int newStatusId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                comment.CommentStatusId = newStatusId;
                comment.ModifiedOn = DateTime.UtcNow;
                // Enforce shownInPortal = false if status is Pending or Rejected
                if (newStatusId == (int)CommentStatusesEnum.Pending || newStatusId == (int)CommentStatusesEnum.Rejected)
                {
                    comment.ShownInPortal = false;
                }
                else if (newStatusId == (int)CommentStatusesEnum.Approved)
                {
                    comment.ShownInPortal = true;
                }
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CascadeStatusToChildrenAsync(Guid parentId, int newStatusId)
        {
            var children = await _context.Comments.Where(c => c.ParentId == parentId && !c.IsDeleted).ToListAsync();
            foreach (var child in children)
            {
                child.CommentStatusId = newStatusId;
                child.ModifiedOn = DateTime.UtcNow;
                // Enforce shownInPortal = false if status is Pending or Rejected
                if (newStatusId == (int)CommentStatusesEnum.Pending || newStatusId == (int)CommentStatusesEnum.Rejected)
                {
                    child.ShownInPortal = false;
                }
                else if (newStatusId == (int)CommentStatusesEnum.Approved)
                {
                    child.ShownInPortal = true;
                }
                _context.Comments.Update(child);
                // Recursively update grandchildren
                await CascadeStatusToChildrenAsync(child.Id, newStatusId);
            }
            await _context.SaveChangesAsync();
        }
    }
} 