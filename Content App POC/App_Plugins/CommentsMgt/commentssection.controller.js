var totalCount = 0;

angular.module("umbraco")
    .controller("My.CommentsSectionApp", function ($scope, $http, editorState, userService, contentResource) {

        console.log("CommentsSectionApp controller loaded");

        var vm = this;
        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;
        vm.Comments = [];
        vm.CanAdminComments = false;
        vm.showAddModal = false;
        vm.showEditModal = false;
        vm.showDeleteModal = false;
        vm.showReplyModal = false;
        vm.modalComment = null;
        vm.replyToComment = null;
        vm.newCommentText = '';
        vm.editingCommentText = '';
        vm.replyText = '';

        // Fetch comments for the current content id
        vm.loadComments = function() {
            $http.get("/api/comments/content/" + vm.CurrentNodeId)
                .then(function(response) {
                    vm.Comments = response.data;
                }, function(error) {
                    console.error("Failed to load comments", error);
                });
        };

        vm.loadComments();

        function checkCommentsAdmin(userGroups) {
            if (!userGroups) return false;
            return userGroups.some(function (g) {
                if (typeof g === 'string') return g.toLowerCase() === 'commentsadmin';
                if (g && g.name) return g.name.toLowerCase() === 'commentsadmin';
                if (g && g.alias) return g.alias.toLowerCase() === 'commentsadmin';
                return false;
            });
        }

        // Modal open/close helpers
        vm.openAddModal = function () {
            vm.newCommentText = '';
            vm.showAddModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeAddModal = function() {
            vm.showAddModal = false;
        };
        vm.openEditModal = function (comment) {
            vm.modalComment = angular.copy(comment);
            vm.editingCommentText = comment.commentText;
            vm.showEditModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeEditModal = function() {
            vm.showEditModal = false;
            vm.modalComment = null;
        };
        vm.openDeleteModal = function (comment) {
            vm.modalComment = comment;
            vm.showDeleteModal = true;
        };
        vm.closeDeleteModal = function() {
            vm.showDeleteModal = false;
            vm.modalComment = null;
        };

        // Reply modal functions
        vm.openReplyModal = function(comment) {
            vm.replyToComment = comment;
            vm.replyText = '';
            vm.showReplyModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeReplyModal = function() {
            vm.showReplyModal = false;
            vm.replyToComment = null;
            vm.replyText = '';
        };

        // Add comment (modal)
        vm.addComment = function() {
            if (!vm.newCommentText) return;
            var comment = {
                contentId: vm.CurrentNodeId,
                commentText: vm.newCommentText,
                createdBy: vm.UserName,
                modifiedBy: vm.UserName
            };
            $http.post('/api/comments', comment).then(function() {
                vm.newCommentText = '';
                vm.closeAddModal();
                vm.loadComments();
            });
        };

        // Edit comment (modal)
        vm.saveEdit = function() {
            if (!vm.modalComment) return;
            var updated = angular.copy(vm.modalComment);
            updated.commentText = vm.editingCommentText;
            updated.modifiedBy = vm.UserName;
            $http.put('/api/comments/' + updated.id, updated).then(function() {
                vm.closeEditModal();
                vm.loadComments();
            });
        };

        // Delete comment (modal)
        vm.confirmDelete = function() {
            if (!vm.modalComment) return;
            $http.delete('/api/comments/' + vm.modalComment.id).then(function() {
                vm.closeDeleteModal();
                vm.loadComments();
            });
        };

        // Submit reply
        vm.submitReply = function() {
            if (!vm.replyText || !vm.replyToComment) return;
            var reply = {
                contentId: vm.CurrentNodeId,
                commentText: vm.replyText,
                parentCommentId: vm.replyToComment.id,
                createdBy: vm.UserName,
                modifiedBy: vm.UserName
            };
            $http.post('/api/comments', reply).then(function() {
                vm.closeReplyModal();
                vm.loadComments();
            });
        };

        // Get user initials for avatar
        vm.getInitials = function(name) {
            if (!name) return '';
            var names = name.split(' ');
            if (names.length >= 2) {
                return (names[0].charAt(0) + names[1].charAt(0)).toUpperCase();
            }
            return name.charAt(0).toUpperCase();
        };

        // Initial user check
        var user = userService.getCurrentUser().then(function (user) {
            vm.UserName = user.name;
            vm.UserGroups = user.userGroups;
            vm.CanAdminComments = checkCommentsAdmin(user.userGroups);
        });

        // Toggle approval for a comment and its children
        vm.toggleApproval = function(comment) {
            var newStatus = !comment.isApproved;
            $http.put('/api/comments/' + comment.id + '/approval', {
                isApproved: newStatus,
                modifiedBy: vm.UserName
            }).then(function() {
                vm.loadComments();
            });
        };

        // Helper to check if a comment or any parent is disapproved
        vm.isDimmed = function(comment) {
            if (!comment.isApproved) return true;
            var parentId = comment.parentCommentId;
            while (parentId) {
                var parent = vm.Comments.find(function(c) { return c.id === parentId; });
                if (parent && !parent.isApproved) return true;
                parentId = parent ? parent.parentCommentId : null;
            }
            return false;
        };

    });