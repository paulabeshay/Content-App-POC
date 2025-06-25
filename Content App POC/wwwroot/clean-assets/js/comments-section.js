// Social-media-style Comments Section AJAX logic
(function() {
    const apiBase = '/api/comments';
    const container = document.getElementById('comments-section-container');
    if (!container) return;

    // Utility: format date
    function formatDate(dateStr) {
        const d = new Date(dateStr);
        return d.toLocaleString();
    }

    // Utility: get initials
    function getInitials(name) {
        if (!name) return '';
        const names = name.split(' ');
        if (names.length >= 2) return (names[0][0] + names[1][0]).toUpperCase();
        return name[0].toUpperCase();
    }

    // Utility: show notification
    function showNotification(message) {
        let notif = document.createElement('div');
        notif.className = 'smc-notification';
        notif.textContent = message;
        notif.style.position = 'fixed';
        notif.style.top = '20px';
        notif.style.right = '20px';
        notif.style.background = '#4caf50';
        notif.style.color = 'white';
        notif.style.padding = '10px 20px';
        notif.style.borderRadius = '5px';
        notif.style.boxShadow = '0 2px 8px rgba(0,0,0,0.15)';
        notif.style.zIndex = 9999;
        document.body.appendChild(notif);
        setTimeout(() => {
            notif.remove();
        }, 2500);
    }

    // Render comments
    function renderComments(comments, userName) {
        // Only include approved and not deleted comments
        const visibleComments = comments.filter(c => c.isApproved && !c.isDeleted);
        // Build a map for parent/child relationships
        const byParent = {};
        visibleComments.forEach(c => {
            if (!byParent[c.parentCommentId]) byParent[c.parentCommentId] = [];
            byParent[c.parentCommentId].push(c);
        });
        function renderList(parentId) {
            const list = byParent[parentId] || [];
            return list.map(comment => `
                <div class="smc-comment-card">
                    <div class="smc-comment-header">
                        <div class="smc-comment-avatar">${getInitials(comment.createdBy)}</div>
                        <div class="smc-comment-meta">
                            <span class="smc-comment-author">${comment.createdBy}</span>
                            <span class="smc-comment-date">${formatDate(comment.createdOn)}</span>
                        </div>
                    </div>
                    <div class="smc-comment-body">${comment.commentText}</div>
                    <div class="smc-comment-actions">
                        ${parentId === null ? `<button class=\"smc-reply-btn\" data-id=\"${comment.id}\">Reply</button>` : ''}
                    </div>
                    <div class="smc-comment-replies">
                        ${renderList(comment.id)}
                    </div>
                </div>
            `).join('');
        }
        return renderList(null);
    }

    // Fetch comments
    function loadComments() {
        const contentId = window.currentContentId || 0;
        fetch(apiBase + '/content/' + contentId)
            .then(r => r.json())
            .then(comments => {
                // Optionally, fetch user info for posting
                const userName = window.currentUserName || 'Anonymous';
                container.innerHTML = `
                    <div class="smc-add-comment">
                        <textarea id="smc-new-comment" placeholder="Write a comment..."></textarea>
                        <button id="smc-post-btn">Post</button>
                    </div>
                    <div class="smc-comments-list">
                        ${renderComments(comments, userName)}
                    </div>
                `;
                document.getElementById('smc-post-btn').onclick = function() {
                    const text = document.getElementById('smc-new-comment').value.trim();
                    if (!text) return;
                    fetch(apiBase, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            contentId: contentId,
                            commentText: text,
                            createdBy: userName,
                            modifiedBy: userName
                        })
                    }).then(() => {
                        showNotification('Comment added and may wait approval!');
                        loadComments();
                    });
                };
                // Reply buttons
                container.querySelectorAll('.smc-reply-btn').forEach(btn => {
                    btn.onclick = function() {
                        // Close any other open reply box
                        container.querySelectorAll('.smc-reply-box').forEach(box => box.remove());
                        container.querySelectorAll('.smc-reply-btn').forEach(b => b.disabled = false);
                        const commentId = btn.getAttribute('data-id');
                        const replyBox = document.createElement('div');
                        replyBox.className = 'smc-reply-box';
                        replyBox.innerHTML = `
                            <textarea placeholder="Write a reply..."></textarea>
                            <button>Reply</button>
                            <button class="smc-cancel-reply">Cancel</button>
                        `;
                        btn.parentNode.appendChild(replyBox);
                        replyBox.querySelector('button').onclick = function() {
                            const replyText = replyBox.querySelector('textarea').value.trim();
                            if (!replyText) return;
                            fetch(apiBase, {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify({
                                    contentId: contentId,
                                    commentText: replyText,
                                    parentCommentId: commentId,
                                    createdBy: userName,
                                    modifiedBy: userName
                                })
                            }).then(() => {
                                showNotification('Reply added and may wait approval!');
                                loadComments();
                            });
                        };
                        replyBox.querySelector('.smc-cancel-reply').onclick = function() {
                            replyBox.remove();
                            btn.disabled = false;
                        };
                        btn.disabled = true;
                    };
                });
            });
    }

    // Optionally, set current content id and user name from server-side
    window.currentContentId = window.currentContentId || 0;
    window.currentUserName = window.currentUserName || 'Anonymous';

    loadComments();
})(); 