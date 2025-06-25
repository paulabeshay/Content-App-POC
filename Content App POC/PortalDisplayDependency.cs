using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Extensions;

namespace Content_App_POC
{
    public class PortalDisplayDependencyComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register the notification handler for content saving
            builder.AddNotificationHandler<ContentSavingNotification, PortalDisplayDependencyHandler>();
            
            // Register the JavaScript and CSS for the backoffice
            builder.ManifestFilters().Append<PortalDisplayDependencyManifestFilter>();
        }
    }

    public class PortalDisplayDependencyHandler : INotificationHandler<ContentSavingNotification>
    {
        public void Handle(ContentSavingNotification notification)
        {
            foreach (var content in notification.SavedEntities)
            {
                // Check if this content has both cMSDisplay and portalDisplay properties
                var cmsDisplayProperty = content.Properties.FirstOrDefault(p => p.Alias == "cMSDisplay");
                var portalDisplayProperty = content.Properties.FirstOrDefault(p => p.Alias == "portalDisplay");

                if (cmsDisplayProperty != null && portalDisplayProperty != null)
                {
                    // If cMSDisplay is turned off (not "1"), disable portalDisplay
                    var cmsDisplayValue = cmsDisplayProperty.GetValue()?.ToString();
                    if (cmsDisplayValue != "1")
                    {
                        // Set portalDisplay to false
                        portalDisplayProperty.SetValue(false);
                    }
                }
            }
        }
    }

    public class PortalDisplayDependencyManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var manifest = new PackageManifest
            {
                PackageName = "PortalDisplayDependency",
                Scripts = new[]
                {
                    "/App_Plugins/PortalDisplayDependency/portalDisplayDependency.js"
                },
                Stylesheets = new[]
                {
                    "/App_Plugins/PortalDisplayDependency/portalDisplayDependency.css"
                }
            };

            manifests.Add(manifest);
        }
    }
} 