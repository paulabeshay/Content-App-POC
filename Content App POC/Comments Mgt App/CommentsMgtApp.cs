using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models;
using Microsoft.Extensions.Configuration;

namespace Content_App_POC.Comments_Mgt
{
    public class CommentsMgtAppComponent : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Add our word counter content app into the composition aka into the DI
            builder.ContentApps().Append<CommentsMgtApp>();
        }
    }

    public class CommentsMgtApp : IContentAppFactory
    {
        private IConfiguration? _configuration;
        public CommentsMgtApp(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            // Can implement some logic with userGroups if needed
            // Allowing us to display the content app with some restrictions for certain groups
            if (userGroups.All(x => x.Alias.ToLowerInvariant() != _configuration?["CommentsConfig:CommentsAdminRole"]?.ToLowerInvariant() && x.Alias.ToLowerInvariant() != _configuration?["CommentsConfig:CommentsViewerRole"]?.ToLowerInvariant()))
                return null;

            // Only show app on content items
            if (source is not IContent content)
                return null;

            // Only show app on content items with template
            //if (content.TemplateId is null)
            //    return null;

            // Only show app on content with certain content type alias
            var property = content.Properties.FirstOrDefault(p => p.Alias == _configuration?["CommentsConfig:CMSDisplayToggle"]);
            if (property?.GetValue()?.ToString() != "1")
                return null;

            return new ContentApp
            {
                Alias = "commentsSection",
                Name = "Comments Section",
                Icon = "icon-reply-arrow",
                View = "/App_Plugins/CommentsMgt/commentssection.html",
                Weight = 0
            };
        }
    }
}
