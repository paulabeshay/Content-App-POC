using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models;

namespace Content_App_POC.Word_Counter
{
    public class WordCounterAppComponent : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Add our word counter content app into the composition aka into the DI
            builder.ContentApps().Append<WordCounterApp>();
        }
    }

    public class WordCounterApp : IContentAppFactory
    {
        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            // Can implement some logic with userGroups if needed
            // Allowing us to display the content app with some restrictions for certain groups
            if (userGroups.All(x => x.Alias.ToLowerInvariant() != Umbraco.Cms.Core.Constants.Security.EditorGroupAlias))
                return null;

            // Only show app on content items
            if (source is not IContent content)
                return null;

            // Only show app on content items with template
            if (content.TemplateId is null)
                return null;

            // Only show app on content with certain content type alias
            if (!content.ContentType.Alias.Equals("article"))
                return null;

            return new ContentApp
            {
                Alias = "wordCounter",
                Name = "Word Counter",
                Icon = "icon-calculator",
                View = "/App_Plugins/WordCounter/wordcounter.html",
                Weight = 0
            };
        }
    }
}
