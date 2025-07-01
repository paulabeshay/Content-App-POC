(function () {
    'use strict';

    // Function to handle the dependency between cMSDisplay and portalDisplay
    function handlePortalDisplayDependency() {
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

        // Function to update portalDisplay state based on cMSDisplay
        function updatePortalDisplayState() {
            var cmsDisplayValue = cmsDisplayInput.type === 'checkbox' ? cmsDisplayInput.checked : cmsDisplayInput.value;
            var isCMSDisplayEnabled = cmsDisplayInput.type === 'checkbox' ? cmsDisplayValue : cmsDisplayValue === '1';
            
            // Disable/enable portalDisplay based on cMSDisplay
            portalDisplayInput.disabled = !isCMSDisplayEnabled;
            
            // If cMSDisplay is disabled, uncheck portalDisplay
            if (!isCMSDisplayEnabled) {
                portalDisplayInput.checked = false;
                // Trigger change event to update the model
                var event = new Event('change', { bubbles: true });
                portalDisplayInput.dispatchEvent(event);
            }
            
            // Add visual indication
            var portalDisplayContainer = portalDisplayField.closest('.umb-property');
            if (portalDisplayContainer) {
                if (!isCMSDisplayEnabled) {
                    portalDisplayContainer.classList.add('umb-property--disabled');
                    portalDisplayContainer.style.opacity = '0.6';
                } else {
                    portalDisplayContainer.classList.remove('umb-property--disabled');
                    portalDisplayContainer.style.opacity = '1';
                }
            }
        }

        // Add event listener to cMSDisplay
        cmsDisplayInput.addEventListener('change', updatePortalDisplayState);
        
        // Initial state update
        updatePortalDisplayState();
    }

    // Initialize when DOM is ready
    function initialize() {
        // Wait for Umbraco to load the content
        setTimeout(function() {
            handlePortalDisplayDependency();
        }, 1000);
        
        // Also listen for dynamic content loading
        document.addEventListener('DOMContentLoaded', function() {
            setTimeout(handlePortalDisplayDependency, 500);
        });
    }

    // Run initialization
    initialize();
    
    // Also run when Umbraco content is loaded (for dynamic content)
    if (typeof angular !== 'undefined' && angular.module) {
        angular.module('umbraco').run(['$rootScope', function($rootScope) {
            $rootScope.$on('contentLoaded', function() {
                setTimeout(handlePortalDisplayDependency, 500);
            });
        }]);
    }
})(); 