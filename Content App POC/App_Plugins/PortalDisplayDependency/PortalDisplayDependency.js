(function () {
    'use strict';

    // Function to handle the dependency between cMSDisplay and portalDisplay
    function handlePortalDisplayDependency(isCommentsAdmin) {
        // Find the cMSDisplay field
        var cmsDisplayField = document.querySelector('[data-element="cMSDisplay"]');
        var portalDisplayField = document.querySelector('[data-element="portalDisplay"]');

        if (!cmsDisplayField || !portalDisplayField) {
            return;
        }

        // Get the cMSDisplay input
        var cmsDisplayInput = cmsDisplayField.querySelector('input[type="checkbox"], select');
        var portalDisplayInput = portalDisplayField.querySelector('input[type="checkbox"]');

        if (!cmsDisplayInput || !portalDisplayInput) {
            return;
        }

        // Function to update portalDisplay state based on cMSDisplay and user group
        function updatePortalDisplayState() {
            var cmsDisplayValue = cmsDisplayInput.type === 'checkbox' ? cmsDisplayInput.checked : cmsDisplayInput.value;
            var isCMSDisplayEnabled = cmsDisplayInput.type === 'checkbox' ? cmsDisplayValue : cmsDisplayValue === '1';

            // Disable/enable portalDisplay based on cMSDisplay and user group
            portalDisplayInput.disabled = !isCMSDisplayEnabled || !isCommentsAdmin;
            cmsDisplayInput.disabled = !isCommentsAdmin;

            // If cMSDisplay is disabled, uncheck portalDisplay
            if (!isCMSDisplayEnabled) {
                portalDisplayInput.checked = false;
                // Trigger change event to update the model
                var event = new Event('change', { bubbles: true });
                portalDisplayInput.dispatchEvent(event);
            }

            // Add visual indication
            var portalDisplayContainer = portalDisplayField.closest('.umb-property');
            var cmsDisplayContainer = cmsDisplayField.closest('.umb-property');
            if (portalDisplayContainer) {
                if (!isCMSDisplayEnabled || !isCommentsAdmin) {
                    portalDisplayContainer.classList.add('umb-property--disabled');
                    portalDisplayContainer.style.opacity = '0.6';
                } else {
                    portalDisplayContainer.classList.remove('umb-property--disabled');
                    portalDisplayContainer.style.opacity = '1';
                }
            }
            if (cmsDisplayContainer) {
                if (!isCommentsAdmin) {
                    cmsDisplayContainer.classList.add('umb-property--disabled');
                    cmsDisplayContainer.style.opacity = '0.6';
                } else {
                    cmsDisplayContainer.classList.remove('umb-property--disabled');
                    cmsDisplayContainer.style.opacity = '1';
                }
            }
        }

        // Add event listener to cMSDisplay
        cmsDisplayInput.addEventListener('change', updatePortalDisplayState);

        // Initial state update
        updatePortalDisplayState();
    }

    // Helper to check if user is in CommentsAdmin group
    function isUserCommentsAdmin(user) {
        if (!user || !user.userGroups) return false;
        return user.userGroups.some(function (g) {
            if (typeof g === 'string') return g.toLowerCase() === 'commentsadmin';
            if (g && g.name) return g.name.toLowerCase() === 'commentsadmin';
            if (g && g.alias) return g.alias.toLowerCase() === 'commentsadmin';
            return false;
        });
    }

    // Initialize when DOM is ready
    function initialize() {
        // Use Angular's userService to get current user
        if (window.angular && angular.module) {
            var injector = angular.element(document.body).injector();
            if (injector) {
                var userService = injector.get('userService');
                userService.getCurrentUser().then(function(user) {
                    var isCommentsAdmin = isUserCommentsAdmin(user);
                    setTimeout(function () {
                        handlePortalDisplayDependency(isCommentsAdmin);
                    }, 1000);
                });
            } else {
                // fallback if injector not ready
                setTimeout(initialize, 500);
            }
        } else {
            // fallback if angular not ready
            setTimeout(initialize, 500);
        }

        // Also listen for dynamic content loading
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(initialize, 500);
        });
    }

    // Run initialization
    initialize();

    // Also run when Umbraco content is loaded (for dynamic content)
    if (typeof angular !== 'undefined' && angular.module) {
        angular.module('umbraco').run(['$rootScope', function ($rootScope) {
            $rootScope.$on('contentLoaded', function () {
                setTimeout(initialize, 500);
            });
        }]);
    }
})(); 