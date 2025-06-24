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
            console.log("Add Modal Function called, ShowAddModal = " + vm.showAddModal);
            vm.newCommentText = '';
            vm.showAddModal = true;
            console.log("Add Modal Function called, ShowAddModal = " + vm.showAddModal);
        };
        vm.closeAddModal = function() {
            vm.showAddModal = false;
        };
        vm.openEditModal = function(comment) {
            vm.modalComment = angular.copy(comment);
            vm.editingCommentText = comment.commentText;
            vm.showEditModal = true;
        };
        vm.closeEditModal = function() {
            vm.showEditModal = false;
            vm.modalComment = null;
        };
        vm.openDeleteModal = function(comment) {
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

        // Initial user check
        var user = userService.getCurrentUser().then(function (user) {
            vm.UserName = user.name;
            vm.UserGroups = user.userGroups;
            vm.CanAdminComments = checkCommentsAdmin(user.userGroups);
        });

    });